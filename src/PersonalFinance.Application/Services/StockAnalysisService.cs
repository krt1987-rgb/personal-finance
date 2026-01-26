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
            analysis.ConfidenceScore = 0.85m; // TODO: Implement confidence scoring
            
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
}
