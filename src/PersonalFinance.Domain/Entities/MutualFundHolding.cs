using PersonalFinance.Domain.Common;
using PersonalFinance.Domain.Enums;

namespace PersonalFinance.Domain.Entities;

public class MutualFundHolding : BaseEntity
{
    public Guid UserId { get; set; }
    public string FolioNumber { get; set; } = string.Empty;
    public string SchemeName { get; set; } = string.Empty;
    public string? AMC { get; set; }
    public string? ISIN { get; set; }
    public MutualFundCategory Category { get; set; }
    public decimal Units { get; set; }
    public decimal AverageNAV { get; set; }
    public decimal? CurrentNAV { get; set; }
    public DateTime? LastNAVUpdate { get; set; }
    public InvestmentMode InvestmentMode { get; set; }
    
    // Navigation properties
    public User User { get; set; } = null!;
    public ICollection<MutualFundTransaction> MutualFundTransactions { get; set; } = new List<MutualFundTransaction>();
    
    // Calculated properties
    public decimal InvestedAmount => Units * AverageNAV;
    public decimal? CurrentValue => Units * (CurrentNAV ?? 0);
    public decimal? ProfitLoss => CurrentValue - InvestedAmount;
    public decimal? ProfitLossPercentage => InvestedAmount > 0 ? ((ProfitLoss ?? 0) / InvestedAmount) * 100 : 0;
}
