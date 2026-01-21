using PersonalFinance.Domain.Enums;

namespace PersonalFinance.Application.DTOs;

public class ProvidentFundDto
{
    public Guid Id { get; set; }
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
}

public class CreateProvidentFundDto
{
    public Guid UserId { get; set; }
    public PFType PFType { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public string? UAN { get; set; }
    public decimal EmployeeContribution { get; set; }
    public decimal EmployerContribution { get; set; }
    public decimal CurrentBalance { get; set; }
    public decimal InterestRate { get; set; }
    public string? Organization { get; set; }
}

public class UpdateProvidentFundDto
{
    public decimal? EmployeeContribution { get; set; }
    public decimal? EmployerContribution { get; set; }
    public decimal? CurrentBalance { get; set; }
    public decimal? InterestRate { get; set; }
}

public class PFSummaryDto
{
    public int TotalAccounts { get; set; }
    public decimal TotalBalance { get; set; }
    public decimal TotalEmployeeContribution { get; set; }
    public decimal TotalEmployerContribution { get; set; }
    public List<PFTypeSummary> ByType { get; set; } = new();
}

public class PFTypeSummary
{
    public string Type { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal TotalBalance { get; set; }
}
