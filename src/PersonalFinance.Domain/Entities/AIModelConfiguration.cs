using PersonalFinance.Domain.Common;
using PersonalFinance.Domain.Enums;

namespace PersonalFinance.Domain.Entities;

public class AIModelConfiguration : BaseEntity
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public AIProviderType ProviderType { get; set; }
    public AIModelType ModelType { get; set; }
    public string? CustomModelName { get; set; }
    public string ApiKey { get; set; } = string.Empty;
    public string? ApiEndpoint { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDefault { get; set; } = false;
    public int Priority { get; set; } = 0;
    
    // Model-specific settings
    public decimal? Temperature { get; set; }
    public int? MaxTokens { get; set; }
    public decimal? TopP { get; set; }
    public decimal? FrequencyPenalty { get; set; }
    public decimal? PresencePenalty { get; set; }
    
    // Rate limiting
    public int? RequestsPerMinute { get; set; }
    public int? RequestsPerDay { get; set; }
    
    // Navigation properties
    public User User { get; set; } = null!;
    public ICollection<StockAnalysis> StockAnalyses { get; set; } = new List<StockAnalysis>();
}
