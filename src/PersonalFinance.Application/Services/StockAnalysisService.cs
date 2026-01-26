using Microsoft.Extensions.Logging;
using PersonalFinance.Application.DTOs;
using PersonalFinance.Application.Interfaces;
using PersonalFinance.Domain.Entities;
using PersonalFinance.Domain.Enums;
using PersonalFinance.Domain.Interfaces;

namespace PersonalFinance.Application.Services;

public class StockAnalysisService : IStockAnalysisService
{
    private readonly IRepository<StockAnalysis> _analysisRepository;
    private readonly IRepository<AIModelConfiguration> _configRepository;
    private readonly IRepository<StockHolding> _stockHoldingRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAIService _aiService;
    private readonly ILogger<StockAnalysisService> _logger;

    public StockAnalysisService(
        IRepository<StockAnalysis> analysisRepository,
        IRepository<AIModelConfiguration> configRepository,
        IRepository<StockHolding> stockHoldingRepository,
        IUnitOfWork unitOfWork,
        IAIService aiService,
        ILogger<StockAnalysisService> logger)
    {
        _analysisRepository = analysisRepository;
        _configRepository = configRepository;
        _stockHoldingRepository = stockHoldingRepository;
        _unitOfWork = unitOfWork;
        _aiService = aiService;
        _logger = logger;
    }

    public async Task<StockAnalysisDto> CreateAnalysisAsync(Guid userId, CreateStockAnalysisRequestDto request)
    {
        // Check cache if enabled
        if (request.UseCache)
        {
            var cachedAnalysis = await GetCachedAnalysisAsync(userId, request.Symbol, request.AnalysisType);
            if (cachedAnalysis != null)
            {
                _logger.LogInformation("Returning cached analysis for {Symbol}", request.Symbol);
                cachedAnalysis.IsCached = true;
                return cachedAnalysis;
            }
        }

        // Get AI configuration
        var config = await GetAIConfigurationAsync(userId, request.AIModelConfigurationId);
        if (config == null)
        {
            throw new InvalidOperationException("No AI model configuration found. Please configure an AI provider first.");
        }

        // Get company name if not provided
        var companyName = request.CompanyName;
        if (string.IsNullOrEmpty(companyName) && request.StockHoldingId.HasValue)
        {
            var holding = await _stockHoldingRepository.GetByIdAsync(request.StockHoldingId.Value);
            companyName = holding?.CompanyName ?? request.Symbol;
        }
        companyName ??= request.Symbol;

        // Create analysis record
        var analysis = new StockAnalysis
        {
            UserId = userId,
            StockHoldingId = request.StockHoldingId,
            AIModelConfigurationId = config.Id,
            Symbol = request.Symbol.ToUpper(),
            CompanyName = companyName,
            AnalysisType = request.AnalysisType,
            Status = AnalysisStatus.InProgress,
            AnalysisPrompt = request.CustomPrompt
        };

        await _analysisRepository.AddAsync(analysis);
        await _unitOfWork.SaveChangesAsync();

        try
        {
            // Generate AI analysis
            var analysisResult = await _aiService.GenerateStockAnalysisAsync(
                analysis.Symbol,
                analysis.CompanyName,
                request.AnalysisType,
                config.ProviderType,
                config.ModelType,
                config.ApiKey,
                config.ApiEndpoint,
                request.CustomPrompt);

            // Parse and structure the analysis result
            var structuredResult = ParseAnalysisResult(analysisResult, request.AnalysisType);

            analysis.AnalysisResult = analysisResult;
            analysis.Summary = structuredResult.Summary;
            analysis.KeyInsights = structuredResult.KeyInsights;
            analysis.Risks = structuredResult.Risks;
            analysis.Recommendation = structuredResult.Recommendation;
            analysis.Status = AnalysisStatus.Completed;
            analysis.CompletedAt = DateTime.UtcNow;
            
            // Calculate confidence score
            analysis.ConfidenceScore = CalculateConfidenceScore(analysisResult, structuredResult);
            
            // Set cache expiration based on analysis type
            analysis.CachedUntil = DateTime.UtcNow.AddHours(GetCacheDurationHours(request.AnalysisType));

            await _analysisRepository.UpdateAsync(analysis);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Stock analysis completed for {Symbol}", request.Symbol);
        }
        catch (Exception ex)
        {
            analysis.Status = AnalysisStatus.Failed;
            analysis.ErrorMessage = ex.Message;
            await _analysisRepository.UpdateAsync(analysis);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogError(ex, "Failed to generate analysis for {Symbol}", request.Symbol);
            throw;
        }

        return MapToDto(analysis);
    }

    public async Task<StockAnalysisDto?> GetAnalysisAsync(Guid id)
    {
        var analysis = await _analysisRepository.GetByIdAsync(id);
        return analysis == null ? null : MapToDto(analysis);
    }

    public async Task<IEnumerable<StockAnalysisDto>> GetAnalysesBySymbolAsync(Guid userId, string symbol)
    {
        var analyses = await _analysisRepository.FindAsync(a => 
            a.UserId == userId && 
            a.Symbol == symbol.ToUpper() &&
            !a.IsDeleted);
        
        return analyses
            .OrderByDescending(a => a.CreatedAt)
            .Select(MapToDto)
            .ToList();
    }

    public async Task<IEnumerable<StockAnalysisDto>> GetUserAnalysesAsync(Guid userId, int limit = 50)
    {
        var analyses = await _analysisRepository.FindAsync(a => 
            a.UserId == userId && 
            !a.IsDeleted);
        
        return analyses
            .OrderByDescending(a => a.CreatedAt)
            .Take(limit)
            .Select(MapToDto)
            .ToList();
    }

    public async Task<StockResearchResponseDto> ResearchStockAsync(Guid userId, StockResearchRequestDto request)
    {
        var config = await GetAIConfigurationAsync(userId, request.AIModelConfigurationId);
        if (config == null)
        {
            throw new InvalidOperationException("No AI model configuration found. Please configure an AI provider first.");
        }

        var prompt = $"Research question about stock {request.Symbol}: {request.Query}";
        
        var response = await _aiService.GenerateCompletionAsync(
            prompt,
            config.ProviderType,
            config.ModelType,
            config.ApiKey,
            config.ApiEndpoint,
            temperature: 0.7m,
            maxTokens: 1000);

        return new StockResearchResponseDto
        {
            Symbol = request.Symbol,
            Query = request.Query,
            Response = response,
            ModelUsed = $"{config.ProviderType} - {config.ModelType}",
            GeneratedAt = DateTime.UtcNow,
            FromCache = false
        };
    }

    private async Task<AIModelConfiguration?> GetAIConfigurationAsync(Guid userId, Guid? configId)
    {
        if (configId.HasValue)
        {
            return await _configRepository.GetByIdAsync(configId.Value);
        }

        // Get default configuration
        var configs = await _configRepository.FindAsync(c => 
            c.UserId == userId && 
            c.IsDefault && 
            c.IsActive && 
            !c.IsDeleted);
        
        return configs.FirstOrDefault();
    }

    private async Task<StockAnalysisDto?> GetCachedAnalysisAsync(Guid userId, string symbol, StockAnalysisType analysisType)
    {
        var analyses = await _analysisRepository.FindAsync(a =>
            a.UserId == userId &&
            a.Symbol == symbol.ToUpper() &&
            a.AnalysisType == analysisType &&
            a.Status == AnalysisStatus.Completed &&
            a.CachedUntil.HasValue &&
            a.CachedUntil.Value > DateTime.UtcNow &&
            !a.IsDeleted);

        var cached = analyses.OrderByDescending(a => a.CompletedAt).FirstOrDefault();
        
        if (cached != null)
        {
            cached.CacheHitCount++;
            await _analysisRepository.UpdateAsync(cached);
            await _unitOfWork.SaveChangesAsync();
        }

        return cached == null ? null : MapToDto(cached);
    }

    private (string Summary, string KeyInsights, string Risks, string Recommendation) ParseAnalysisResult(
        string analysisResult, 
        StockAnalysisType analysisType)
    {
        // Simple parsing - in production, you might use more sophisticated NLP
        var lines = analysisResult.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        
        var summary = lines.Take(3).Any() ? string.Join(" ", lines.Take(3)) : "Analysis completed";
        var keyInsights = ExtractSection(analysisResult, new[] { "insights", "key points", "highlights" });
        var risks = ExtractSection(analysisResult, new[] { "risks", "concerns", "warnings" });
        var recommendation = ExtractSection(analysisResult, new[] { "recommendation", "conclusion", "verdict" });

        return (summary, keyInsights, risks, recommendation);
    }

    private string ExtractSection(string text, string[] keywords)
    {
        var lines = text.Split('\n');
        var sectionLines = new List<string>();
        var inSection = false;

        foreach (var line in lines)
        {
            var lowerLine = line.ToLower();
            
            if (keywords.Any(k => lowerLine.Contains(k)))
            {
                inSection = true;
                sectionLines.Add(line);
                continue;
            }

            if (inSection)
            {
                if (string.IsNullOrWhiteSpace(line) && sectionLines.Count > 3)
                {
                    break;
                }
                sectionLines.Add(line);
            }
        }

        return sectionLines.Any() ? string.Join("\n", sectionLines) : "N/A";
    }

    private int GetCacheDurationHours(StockAnalysisType analysisType)
    {
        return analysisType switch
        {
            StockAnalysisType.QuickOverview => 2,
            StockAnalysisType.Technical => 4,
            StockAnalysisType.Sentiment => 6,
            StockAnalysisType.Fundamental => 24,
            StockAnalysisType.Comprehensive => 24,
            StockAnalysisType.RiskAssessment => 12,
            StockAnalysisType.Valuation => 24,
            _ => 12
        };
    }

    private static StockAnalysisDto MapToDto(StockAnalysis analysis)
    {
        return new StockAnalysisDto
        {
            Id = analysis.Id,
            UserId = analysis.UserId,
            StockHoldingId = analysis.StockHoldingId,
            AIModelConfigurationId = analysis.AIModelConfigurationId,
            Symbol = analysis.Symbol,
            CompanyName = analysis.CompanyName,
            AnalysisType = analysis.AnalysisType,
            Status = analysis.Status,
            AnalysisResult = analysis.AnalysisResult,
            Summary = analysis.Summary,
            KeyInsights = analysis.KeyInsights,
            Risks = analysis.Risks,
            Recommendation = analysis.Recommendation,
            ConfidenceScore = analysis.ConfidenceScore,
            TokensUsed = analysis.TokensUsed,
            AnalysisCost = analysis.AnalysisCost,
            CreatedAt = analysis.CreatedAt,
            CompletedAt = analysis.CompletedAt,
            ErrorMessage = analysis.ErrorMessage,
            IsCached = false
        };
    }

    public async Task<BatchAnalysisResponseDto> CreateBatchAnalysisAsync(Guid userId, BatchAnalysisRequestDto request)
    {
        var batchId = Guid.NewGuid();
        var startedAt = DateTime.UtcNow;
        var results = new List<StockAnalysisDto>();
        var cachedCount = 0;
        var failedCount = 0;

        _logger.LogInformation("Starting batch analysis for {Count} symbols", request.Symbols.Count);

        foreach (var symbol in request.Symbols)
        {
            try
            {
                var analysisRequest = new CreateStockAnalysisRequestDto
                {
                    Symbol = symbol,
                    AnalysisType = request.AnalysisType,
                    AIModelConfigurationId = request.AIModelConfigurationId,
                    UseCache = request.UseCache
                };

                var analysis = await CreateAnalysisAsync(userId, analysisRequest);
                results.Add(analysis);

                if (analysis.IsCached)
                {
                    cachedCount++;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to analyze symbol {Symbol} in batch", symbol);
                failedCount++;
                
                // Add failed analysis to results
                results.Add(new StockAnalysisDto
                {
                    Symbol = symbol,
                    Status = AnalysisStatus.Failed,
                    ErrorMessage = ex.Message,
                    CreatedAt = DateTime.UtcNow
                });
            }
        }

        return new BatchAnalysisResponseDto
        {
            BatchId = batchId,
            TotalSymbols = request.Symbols.Count,
            CompletedCount = results.Count(r => r.Status == AnalysisStatus.Completed),
            FailedCount = failedCount,
            CachedCount = cachedCount,
            Results = results,
            StartedAt = startedAt,
            CompletedAt = DateTime.UtcNow,
            Status = "Completed"
        };
    }

    public async Task<AnalysisHistoryResponseDto> GetAnalysisHistoryAsync(Guid userId, AnalysisHistoryRequestDto request)
    {
        // Build query
        var query = await _analysisRepository.FindAsync(a => 
            a.UserId == userId && 
            !a.IsDeleted);

        // Apply filters
        if (!string.IsNullOrEmpty(request.Symbol))
        {
            query = query.Where(a => a.Symbol == request.Symbol.ToUpper());
        }

        if (request.AnalysisType.HasValue)
        {
            query = query.Where(a => a.AnalysisType == request.AnalysisType.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(a => a.Status == request.Status.Value);
        }

        if (request.FromDate.HasValue)
        {
            query = query.Where(a => a.CreatedAt >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(a => a.CreatedAt <= request.ToDate.Value);
        }

        // Get total count
        var totalCount = query.Count();

        // Apply sorting
        query = request.SortBy?.ToLower() switch
        {
            "symbol" => request.SortDescending ? query.OrderByDescending(a => a.Symbol) : query.OrderBy(a => a.Symbol),
            "completedat" => request.SortDescending ? query.OrderByDescending(a => a.CompletedAt) : query.OrderBy(a => a.CompletedAt),
            "confidencescore" => request.SortDescending ? query.OrderByDescending(a => a.ConfidenceScore) : query.OrderBy(a => a.ConfidenceScore),
            _ => request.SortDescending ? query.OrderByDescending(a => a.CreatedAt) : query.OrderBy(a => a.CreatedAt)
        };

        // Apply pagination
        var analyses = query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(MapToDto)
            .ToList();

        return new AnalysisHistoryResponseDto
        {
            Analyses = analyses,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
        };
    }

    private decimal CalculateConfidenceScore(string analysisResult, 
        (string Summary, string KeyInsights, string Risks, string Recommendation) structuredResult)
    {
        decimal score = 0;
        var factors = new List<decimal>();

        // Factor 1: Response completeness (0-25 points)
        var completenessScore = CalculateCompletenessScore(structuredResult);
        factors.Add(completenessScore);

        // Factor 2: Response length and detail (0-25 points)
        var detailScore = CalculateDetailScore(analysisResult);
        factors.Add(detailScore);

        // Factor 3: Structured data quality (0-25 points)
        var structureScore = CalculateStructureScore(structuredResult);
        factors.Add(structureScore);

        // Factor 4: Sentiment consistency (0-25 points)
        var consistencyScore = CalculateSentimentConsistency(structuredResult);
        factors.Add(consistencyScore);

        // Calculate weighted average
        score = factors.Average();

        // Normalize to 0-100 scale
        return Math.Round(Math.Min(100, Math.Max(0, score)), 2);
    }

    private decimal CalculateCompletenessScore((string Summary, string KeyInsights, string Risks, string Recommendation) result)
    {
        decimal score = 0;
        
        // Check if each section is present and not N/A
        if (!string.IsNullOrWhiteSpace(result.Summary) && result.Summary != "N/A")
            score += 6.25m;
        
        if (!string.IsNullOrWhiteSpace(result.KeyInsights) && result.KeyInsights != "N/A")
            score += 6.25m;
        
        if (!string.IsNullOrWhiteSpace(result.Risks) && result.Risks != "N/A")
            score += 6.25m;
        
        if (!string.IsNullOrWhiteSpace(result.Recommendation) && result.Recommendation != "N/A")
            score += 6.25m;

        return score;
    }

    private decimal CalculateDetailScore(string analysisResult)
    {
        if (string.IsNullOrWhiteSpace(analysisResult))
            return 0;

        var wordCount = analysisResult.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
        
        // Score based on word count
        // 0-50 words: 0-5 points
        // 51-150 words: 5-15 points
        // 151-300 words: 15-20 points
        // 301+ words: 20-25 points
        
        decimal score = wordCount switch
        {
            <= 50 => Math.Min(5, wordCount * 0.1m),
            <= 150 => 5 + Math.Min(10, (wordCount - 50) * 0.1m),
            <= 300 => 15 + Math.Min(5, (wordCount - 150) * 0.033m),
            _ => 25
        };

        return score;
    }

    private decimal CalculateStructureScore((string Summary, string KeyInsights, string Risks, string Recommendation) result)
    {
        decimal score = 0;
        
        // Check length and quality of each section
        if (result.Summary?.Length > 20)
            score += 6.25m;
        
        if (result.KeyInsights?.Length > 30)
            score += 6.25m;
        
        if (result.Risks?.Length > 20)
            score += 6.25m;
        
        if (result.Recommendation?.Length > 20)
            score += 6.25m;

        return score;
    }

    private decimal CalculateSentimentConsistency((string Summary, string KeyInsights, string Risks, string Recommendation) result)
    {
        // Check for consistency between recommendation and risks
        var recommendation = result.Recommendation?.ToLower() ?? "";
        var risks = result.Risks?.ToLower() ?? "";
        
        decimal score = 12.5m; // Base score

        // Positive indicators in recommendation
        var positiveWords = new[] { "buy", "strong buy", "invest", "positive", "good", "excellent", "recommended" };
        var negativeWords = new[] { "sell", "avoid", "negative", "poor", "risky", "not recommended", "stay away" };
        
        var hasPositiveRec = positiveWords.Any(w => recommendation.Contains(w));
        var hasNegativeRec = negativeWords.Any(w => recommendation.Contains(w));
        
        // High risk should align with negative recommendation
        if (risks.Contains("high risk") || risks.Contains("significant risk"))
        {
            if (hasNegativeRec || recommendation.Contains("caution"))
                score += 12.5m; // Consistent
        }
        else
        {
            // Low/moderate risk should align with positive or neutral recommendation
            if (hasPositiveRec || recommendation.Contains("consider"))
                score += 12.5m; // Consistent
        }

        return score;
    }
}
