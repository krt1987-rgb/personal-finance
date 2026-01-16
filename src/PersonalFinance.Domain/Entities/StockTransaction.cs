using PersonalFinance.Domain.Common;
using PersonalFinance.Domain.Enums;

namespace PersonalFinance.Domain.Entities;

public class StockTransaction : BaseEntity
{
    public Guid StockHoldingId { get; set; }
    public DateTime TransactionDate { get; set; }
    public StockTransactionType TransactionType { get; set; }
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal Brokerage { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Notes { get; set; }
    
    // Navigation properties
    public StockHolding StockHolding { get; set; } = null!;
}
