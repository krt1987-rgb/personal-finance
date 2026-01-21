using PersonalFinance.Domain.Enums;

namespace PersonalFinance.Application.DTOs;

public class BankAccountDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string BankName { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string? IFSC { get; set; }
    public AccountType AccountType { get; set; }
    public decimal CurrentBalance { get; set; }
    public string Currency { get; set; } = "INR";
    public bool IsActive { get; set; }
}

public class CreateBankAccountDto
{
    public Guid UserId { get; set; }
    public string BankName { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string? IFSC { get; set; }
    public AccountType AccountType { get; set; }
    public decimal CurrentBalance { get; set; }
    public string Currency { get; set; } = "INR";
}

public class UpdateBankAccountDto
{
    public string? BankName { get; set; }
    public decimal? CurrentBalance { get; set; }
    public bool? IsActive { get; set; }
}
