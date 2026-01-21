using PersonalFinance.Domain.Common;
using PersonalFinance.Domain.Enums;

namespace PersonalFinance.Domain.Entities;

public class PFTransaction : BaseEntity
{
    public Guid ProvidentFundId { get; set; }
    public DateTime TransactionDate { get; set; }
    public TransactionType TransactionType { get; set; }
    public decimal EmployeeContribution { get; set; }
    public decimal EmployerContribution { get; set; }
    public decimal InterestCredited { get; set; }
    public decimal ClosingBalance { get; set; }
    public string? Description { get; set; }
    
    // Navigation properties
    public ProvidentFund ProvidentFund { get; set; } = null!;
}
