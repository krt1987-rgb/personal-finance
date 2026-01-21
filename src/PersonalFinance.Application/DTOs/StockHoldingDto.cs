namespace PersonalFinance.Application.DTOs;

public class StockHoldingDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string Exchange { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal AverageBuyPrice { get; set; }
    public decimal? CurrentPrice { get; set; }
    public DateTime? LastPriceUpdate { get; set; }
    public string? ISIN { get; set; }
    public string? Sector { get; set; }
    public decimal InvestedAmount { get; set; }
    public decimal? CurrentValue { get; set; }
    public decimal? ProfitLoss { get; set; }
    public decimal? ProfitLossPercentage { get; set; }
}

public class CreateStockHoldingDto
{
    public Guid UserId { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string Exchange { get; set; } = "NSE";
    public decimal Quantity { get; set; }
    public decimal AverageBuyPrice { get; set; }
    public string? ISIN { get; set; }
    public string? Sector { get; set; }
}

public class UpdateStockHoldingDto
{
    public string? CompanyName { get; set; }
    public decimal? Quantity { get; set; }
    public decimal? AverageBuyPrice { get; set; }
    public decimal? CurrentPrice { get; set; }
    public string? Sector { get; set; }
}

public class PortfolioSummaryDto
{
    public decimal TotalInvestment { get; set; }
    public decimal CurrentValue { get; set; }
    public decimal TotalProfitLoss { get; set; }
    public int TotalHoldings { get; set; }
}
