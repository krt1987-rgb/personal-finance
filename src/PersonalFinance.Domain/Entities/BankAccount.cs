using PersonalFinance.Domain.Common;
using PersonalFinance.Domain.Enums;

namespace PersonalFinance.Domain.Entities;

public class BankAccount : BaseEntity
{
    public Guid UserId { get; set; }
    public string BankName { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string? IFSC { get; set; }
    public AccountType AccountType { get; set; }
    public decimal CurrentBalance { get; set; }
    public string Currency { get; set; } = "INR";
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public User User { get; set; } = null!;
    public ICollection<FixedDeposit> FixedDeposits { get; set; } = new List<FixedDeposit>();
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
