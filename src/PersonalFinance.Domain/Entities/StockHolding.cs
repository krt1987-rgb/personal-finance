using PersonalFinance.Domain.Common;

namespace PersonalFinance.Domain.Entities;

public class StockHolding : BaseEntity
{
    public Guid UserId { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string Exchange { get; set; } = "NSE";
    public decimal Quantity { get; set; }
    public decimal AverageBuyPrice { get; set; }
    public decimal? CurrentPrice { get; set; }
    public DateTime? LastPriceUpdate { get; set; }
    public string? ISIN { get; set; }
    public string? Sector { get; set; }
    
    // Navigation properties
    public User User { get; set; } = null!;
    public ICollection<StockTransaction> StockTransactions { get; set; } = new List<StockTransaction>();
    
    // Calculated properties
    public decimal InvestedAmount => Quantity * AverageBuyPrice;
    public decimal? CurrentValue => Quantity * (CurrentPrice ?? 0);
    public decimal? ProfitLoss => CurrentValue - InvestedAmount;
    public decimal? ProfitLossPercentage => InvestedAmount > 0 ? ((ProfitLoss ?? 0) / InvestedAmount) * 100 : 0;
}
