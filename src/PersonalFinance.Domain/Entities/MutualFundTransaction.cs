using PersonalFinance.Domain.Common;
using PersonalFinance.Domain.Enums;

namespace PersonalFinance.Domain.Entities;

public class MutualFundTransaction : BaseEntity
{
    public Guid MutualFundHoldingId { get; set; }
    public DateTime TransactionDate { get; set; }
    public MutualFundTransactionType TransactionType { get; set; }
    public decimal Units { get; set; }
    public decimal NAV { get; set; }
    public decimal Amount { get; set; }
    public string? TransactionNumber { get; set; }
    public string? Notes { get; set; }
    
    // Navigation properties
    public MutualFundHolding MutualFundHolding { get; set; } = null!;
}
