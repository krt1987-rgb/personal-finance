using PersonalFinance.Domain.Enums;

namespace PersonalFinance.Application.DTOs;

public class FixedDepositDto
{
    public Guid Id { get; set; }
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
}

public class CreateFixedDepositDto
{
    public Guid BankAccountId { get; set; }
    public string FDNumber { get; set; } = string.Empty;
    public decimal PrincipalAmount { get; set; }
    public decimal InterestRate { get; set; }
    public int TenureInMonths { get; set; }
    public DateTime StartDate { get; set; }
    public string? Notes { get; set; }
}

public class UpdateFixedDepositDto
{
    public decimal? InterestRate { get; set; }
    public FDStatus? Status { get; set; }
    public string? Notes { get; set; }
}

public class FixedDepositSummaryDto
{
    public int TotalDeposits { get; set; }
    public decimal TotalPrincipal { get; set; }
    public decimal TotalMaturityAmount { get; set; }
    public int ActiveDeposits { get; set; }
    public int MaturedDeposits { get; set; }
}
