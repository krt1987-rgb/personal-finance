using PersonalFinance.Domain.Enums;

namespace PersonalFinance.Application.DTOs;

public class MutualFundHoldingDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string FolioNumber { get; set; } = string.Empty;
    public string SchemeName { get; set; } = string.Empty;
    public string? AMC { get; set; }
    public string? ISIN { get; set; }
    public MutualFundCategory Category { get; set; }
    public decimal Units { get; set; }
    public decimal AverageNAV { get; set; }
    public decimal? CurrentNAV { get; set; }
    public DateTime? LastNAVUpdate { get; set; }
    public InvestmentMode InvestmentMode { get; set; }
    public decimal InvestedAmount { get; set; }
    public decimal? CurrentValue { get; set; }
    public decimal? ProfitLoss { get; set; }
    public decimal? ProfitLossPercentage { get; set; }
}

public class CreateMutualFundHoldingDto
{
    public Guid UserId { get; set; }
    public string FolioNumber { get; set; } = string.Empty;
    public string SchemeName { get; set; } = string.Empty;
    public string? AMC { get; set; }
    public string? ISIN { get; set; }
    public MutualFundCategory Category { get; set; }
    public decimal Units { get; set; }
    public decimal AverageNAV { get; set; }
    public InvestmentMode InvestmentMode { get; set; }
}

public class UpdateMutualFundHoldingDto
{
    public decimal? Units { get; set; }
    public decimal? AverageNAV { get; set; }
    public decimal? CurrentNAV { get; set; }
}

public class MutualFundPortfolioSummaryDto
{
    public decimal TotalInvestment { get; set; }
    public decimal CurrentValue { get; set; }
    public decimal TotalProfitLoss { get; set; }
    public int TotalHoldings { get; set; }
}
