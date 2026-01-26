namespace PersonalFinance.Application.DTOs;

public class NetWorthDto
{
    public decimal TotalAssets { get; set; }
    public decimal TotalLiabilities { get; set; }
    public decimal NetWorth { get; set; }
    public Dictionary<string, decimal> AssetBreakdown { get; set; } = new();
    public Dictionary<string, decimal> LiabilityBreakdown { get; set; } = new();
    public DateTime CalculatedAt { get; set; }
}

public class WealthSnapshotDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime SnapshotDate { get; set; }
    
    public decimal TotalAssets { get; set; }
    public decimal TotalLiabilities { get; set; }
    public decimal NetWorth { get; set; }
    
    public decimal StocksValue { get; set; }
    public decimal MutualFundsValue { get; set; }
    public decimal BankAccountsBalance { get; set; }
    public decimal FixedDepositsValue { get; set; }
    public decimal ProvidentFundsBalance { get; set; }
    public decimal RealEstateValue { get; set; }
    public decimal OtherAssetsValue { get; set; }
    
    public decimal HomeLoanOutstanding { get; set; }
    public decimal PersonalLoanOutstanding { get; set; }
    public decimal CreditCardOutstanding { get; set; }
    public decimal OtherLiabilitiesOutstanding { get; set; }
    
    public string SnapshotType { get; set; } = "Manual";
    public string? Notes { get; set; }
    
    public DateTime CreatedAt { get; set; }
}

public class CreateWealthSnapshotDto
{
    public string? Notes { get; set; }
    public string SnapshotType { get; set; } = "Manual";
}

public class WealthTimelineDto
{
    public List<WealthSnapshotDto> Snapshots { get; set; } = new();
    public decimal? TotalChange { get; set; }
    public decimal? PercentageChange { get; set; }
}

public class WealthComparisonDto
{
    public WealthSnapshotDto? FromSnapshot { get; set; }
    public WealthSnapshotDto? ToSnapshot { get; set; }
    public decimal NetWorthChange { get; set; }
    public decimal PercentageChange { get; set; }
    public Dictionary<string, decimal> AssetChanges { get; set; } = new();
}
