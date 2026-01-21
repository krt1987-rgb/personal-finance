using PersonalFinance.Domain.Common;
using PersonalFinance.Domain.Enums;

namespace PersonalFinance.Domain.Entities;

public class ProvidentFund : BaseEntity
{
    public Guid UserId { get; set; }
    public PFType PFType { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public string? UAN { get; set; }
    public decimal EmployeeContribution { get; set; }
    public decimal EmployerContribution { get; set; }
    public decimal CurrentBalance { get; set; }
    public decimal InterestRate { get; set; }
    public DateTime? LastUpdatedBalance { get; set; }
    public string? Organization { get; set; }
    
    // Navigation properties
    public User User { get; set; } = null!;
    public ICollection<PFTransaction> PFTransactions { get; set; } = new List<PFTransaction>();
}
