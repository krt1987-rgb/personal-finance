namespace PersonalFinance.Application.Models;

/// <summary>
/// Standard response model for real-time stock price data
/// </summary>
public class MCPStockPriceResponse
{
    public string Symbol { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Currency { get; set; } = "USD";
    public DateTime Timestamp { get; set; }
    public decimal? Open { get; set; }
    public decimal? High { get; set; }
    public decimal? Low { get; set; }
    public decimal? PreviousClose { get; set; }
    public long? Volume { get; set; }
    public decimal? Change { get; set; }
    public decimal? ChangePercent { get; set; }
    public string? MarketState { get; set; }
}

/// <summary>
/// Historical price data point
/// </summary>
public class MCPHistoricalPricePoint
{
    public DateTime Date { get; set; }
    public decimal Open { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Close { get; set; }
    public long Volume { get; set; }
    public decimal? AdjustedClose { get; set; }
}

/// <summary>
/// Response for historical price data
/// </summary>
public class MCPHistoricalPriceResponse
{
    public string Symbol { get; set; } = string.Empty;
    public List<MCPHistoricalPricePoint> Prices { get; set; } = new();
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
