using PersonalFinance.Domain.Common;
using PersonalFinance.Domain.Enums;

namespace PersonalFinance.Domain.Entities;

public class StockAnalysis : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid? StockHoldingId { get; set; }
    public Guid? AIModelConfigurationId { get; set; }
    
    public string Symbol { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public StockAnalysisType AnalysisType { get; set; }
    public AnalysisStatus Status { get; set; } = AnalysisStatus.Pending;
    
    // Analysis content
    public string? AnalysisPrompt { get; set; }
    public string? AnalysisResult { get; set; }
    public string? Summary { get; set; }
    public string? KeyInsights { get; set; }
    public string? Risks { get; set; }
    public string? Recommendation { get; set; }
    
    // Metadata
    public decimal? ConfidenceScore { get; set; }
    public int? TokensUsed { get; set; }
    public decimal? AnalysisCost { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? ErrorMessage { get; set; }
    
    // Cache information
    public DateTime? CachedUntil { get; set; }
    public int CacheHitCount { get; set; } = 0;
    
    // Navigation properties
    public User User { get; set; } = null!;
    public StockHolding? StockHolding { get; set; }
    public AIModelConfiguration? AIModelConfiguration { get; set; }
}
