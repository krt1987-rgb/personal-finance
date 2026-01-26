using PersonalFinance.Domain.Enums;

namespace PersonalFinance.Application.DTOs;

// AI Model Configuration DTOs
public class AIModelConfigurationDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public AIProviderType ProviderType { get; set; }
    public AIModelType ModelType { get; set; }
    public string? CustomModelName { get; set; }
    public string? ApiEndpoint { get; set; }
    public bool IsActive { get; set; }
    public bool IsDefault { get; set; }
    public int Priority { get; set; }
    public decimal? Temperature { get; set; }
    public int? MaxTokens { get; set; }
    public decimal? TopP { get; set; }
    public decimal? FrequencyPenalty { get; set; }
    public decimal? PresencePenalty { get; set; }
    public int? RequestsPerMinute { get; set; }
    public int? RequestsPerDay { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateAIModelConfigurationDto
{
    public string Name { get; set; } = string.Empty;
    public AIProviderType ProviderType { get; set; }
    public AIModelType ModelType { get; set; }
    public string? CustomModelName { get; set; }
    public string ApiKey { get; set; } = string.Empty;
    public string? ApiEndpoint { get; set; }
    public bool IsDefault { get; set; } = false;
    public int Priority { get; set; } = 0;
    public decimal? Temperature { get; set; }
    public int? MaxTokens { get; set; }
    public decimal? TopP { get; set; }
    public decimal? FrequencyPenalty { get; set; }
    public decimal? PresencePenalty { get; set; }
    public int? RequestsPerMinute { get; set; }
    public int? RequestsPerDay { get; set; }
}

public class UpdateAIModelConfigurationDto
{
    public string? Name { get; set; }
    public string? ApiKey { get; set; }
    public string? ApiEndpoint { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsDefault { get; set; }
    public int? Priority { get; set; }
    public decimal? Temperature { get; set; }
    public int? MaxTokens { get; set; }
    public decimal? TopP { get; set; }
    public decimal? FrequencyPenalty { get; set; }
    public decimal? PresencePenalty { get; set; }
    public int? RequestsPerMinute { get; set; }
    public int? RequestsPerDay { get; set; }
}

// Stock Analysis DTOs
public class StockAnalysisDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid? StockHoldingId { get; set; }
    public Guid? AIModelConfigurationId { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public StockAnalysisType AnalysisType { get; set; }
    public AnalysisStatus Status { get; set; }
    public string? AnalysisResult { get; set; }
    public string? Summary { get; set; }
    public string? KeyInsights { get; set; }
    public string? Risks { get; set; }
    public string? Recommendation { get; set; }
    public decimal? ConfidenceScore { get; set; }
    public int? TokensUsed { get; set; }
    public decimal? AnalysisCost { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? ErrorMessage { get; set; }
    public bool IsCached { get; set; }
}

public class CreateStockAnalysisRequestDto
{
    public string Symbol { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public Guid? StockHoldingId { get; set; }
    public StockAnalysisType AnalysisType { get; set; } = StockAnalysisType.Comprehensive;
    public Guid? AIModelConfigurationId { get; set; }
    public string? CustomPrompt { get; set; }
    public bool UseCache { get; set; } = true;
}

public class StockResearchRequestDto
{
    public string Symbol { get; set; } = string.Empty;
    public string Query { get; set; } = string.Empty;
    public Guid? AIModelConfigurationId { get; set; }
}

public class StockResearchResponseDto
{
    public string Symbol { get; set; } = string.Empty;
    public string Query { get; set; } = string.Empty;
    public string Response { get; set; } = string.Empty;
    public string? ModelUsed { get; set; }
    public int? TokensUsed { get; set; }
    public DateTime GeneratedAt { get; set; }
    public bool FromCache { get; set; }
}

public class AIProviderStatusDto
{
    public string Name { get; set; } = string.Empty;
    public AIProviderType ProviderType { get; set; }
    public bool IsActive { get; set; }
    public bool IsDefault { get; set; }
    public int Priority { get; set; }
    public string? Status { get; set; }
    public int? RemainingRequests { get; set; }
}

// Batch Analysis DTOs
public class BatchAnalysisRequestDto
{
    public List<string> Symbols { get; set; } = new();
    public StockAnalysisType AnalysisType { get; set; } = StockAnalysisType.QuickOverview;
    public Guid? AIModelConfigurationId { get; set; }
    public bool UseCache { get; set; } = true;
}

public class BatchAnalysisResponseDto
{
    public Guid BatchId { get; set; }
    public int TotalSymbols { get; set; }
    public int CompletedCount { get; set; }
    public int FailedCount { get; set; }
    public int CachedCount { get; set; }
    public List<StockAnalysisDto> Results { get; set; } = new();
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string Status { get; set; } = "InProgress";
}

// Analysis History DTOs
public class AnalysisHistoryRequestDto
{
    public string? Symbol { get; set; }
    public StockAnalysisType? AnalysisType { get; set; }
    public AnalysisStatus? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; } = "CreatedAt";
    public bool SortDescending { get; set; } = true;
}

public class AnalysisHistoryResponseDto
{
    public List<StockAnalysisDto> Analyses { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}
