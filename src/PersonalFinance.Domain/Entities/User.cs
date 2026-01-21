using PersonalFinance.Domain.Common;

namespace PersonalFinance.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public ICollection<FamilyMember> FamilyMembers { get; set; } = new List<FamilyMember>();
    public ICollection<BankAccount> BankAccounts { get; set; } = new List<BankAccount>();
    public ICollection<StockHolding> StockHoldings { get; set; } = new List<StockHolding>();
    public ICollection<MutualFundHolding> MutualFundHoldings { get; set; } = new List<MutualFundHolding>();
}
