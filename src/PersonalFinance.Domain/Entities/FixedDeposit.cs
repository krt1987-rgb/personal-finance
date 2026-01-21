using PersonalFinance.Domain.Common;
using PersonalFinance.Domain.Enums;

namespace PersonalFinance.Domain.Entities;

public class FixedDeposit : BaseEntity
{
    public Guid BankAccountId { get; set; }
    public string FDNumber { get; set; } = string.Empty;
    public decimal PrincipalAmount { get; set; }
    public decimal InterestRate { get; set; }
    public int TenureInMonths { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime MaturityDate { get; set; }
    public decimal MaturityAmount { get; set; }
    public FDStatus Status { get; set; }
    public string? Notes { get; set; }
    
    // Navigation properties
    public BankAccount BankAccount { get; set; } = null!;
}
