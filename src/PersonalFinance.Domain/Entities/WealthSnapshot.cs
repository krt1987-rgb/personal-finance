using PersonalFinance.Domain.Common;

namespace PersonalFinance.Domain.Entities;

public class WealthSnapshot : BaseEntity
{
    public Guid UserId { get; set; }
    public DateTime SnapshotDate { get; set; }
    
    // Aggregated Values
    public decimal TotalAssets { get; set; }
    public decimal TotalLiabilities { get; set; }
    public decimal NetWorth { get; set; }
    
    // Asset Breakdown
    public decimal StocksValue { get; set; }
    public decimal MutualFundsValue { get; set; }
    public decimal BankAccountsBalance { get; set; }
    public decimal FixedDepositsValue { get; set; }
    public decimal ProvidentFundsBalance { get; set; }
    public decimal RealEstateValue { get; set; }
    public decimal OtherAssetsValue { get; set; }
    
    // Liability Breakdown (for future use)
    public decimal HomeLoanOutstanding { get; set; }
    public decimal PersonalLoanOutstanding { get; set; }
    public decimal CreditCardOutstanding { get; set; }
    public decimal OtherLiabilitiesOutstanding { get; set; }
    
    // Metadata
    public string SnapshotType { get; set; } = "Manual"; // Manual, Automatic, End-of-Month
    public string? Notes { get; set; }
    
    // Navigation properties
    public User User { get; set; } = null!;
}
