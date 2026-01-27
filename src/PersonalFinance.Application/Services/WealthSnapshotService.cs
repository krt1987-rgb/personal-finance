using PersonalFinance.Application.DTOs;
using PersonalFinance.Application.Interfaces;
using PersonalFinance.Domain.Entities;
using PersonalFinance.Domain.Interfaces;

namespace PersonalFinance.Application.Services;

public class WealthSnapshotService : IWealthSnapshotService
{
    private readonly IRepository<WealthSnapshot> _repository;
    private readonly INetWorthService _netWorthService;
    private readonly IUnitOfWork _unitOfWork;

    public WealthSnapshotService(
        IRepository<WealthSnapshot> repository,
        INetWorthService netWorthService,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _netWorthService = netWorthService;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<WealthSnapshotDto>> GetAllAsync(Guid userId)
    {
        var snapshots = await _repository.FindAsync(s => s.UserId == userId);
        return snapshots.OrderByDescending(s => s.SnapshotDate).Select(MapToDto);
    }

    public async Task<WealthSnapshotDto?> GetLatestAsync(Guid userId)
    {
        var snapshots = await _repository.FindAsync(s => s.UserId == userId);
        var latest = snapshots.OrderByDescending(s => s.SnapshotDate).FirstOrDefault();
        return latest == null ? null : MapToDto(latest);
    }

    public async Task<WealthSnapshotDto> CreateSnapshotAsync(Guid userId, CreateWealthSnapshotDto dto)
    {
        // Calculate current net worth
        var netWorth = await _netWorthService.CalculateCurrentNetWorthAsync(userId);

        var snapshot = new WealthSnapshot
        {
            UserId = userId,
            SnapshotDate = DateTime.UtcNow,
            TotalAssets = netWorth.TotalAssets,
            TotalLiabilities = netWorth.TotalLiabilities,
            NetWorth = netWorth.NetWorth,
            StocksValue = netWorth.AssetBreakdown.GetValueOrDefault("Stocks", 0),
            MutualFundsValue = netWorth.AssetBreakdown.GetValueOrDefault("MutualFunds", 0),
            BankAccountsBalance = netWorth.AssetBreakdown.GetValueOrDefault("BankAccounts", 0),
            FixedDepositsValue = netWorth.AssetBreakdown.GetValueOrDefault("FixedDeposits", 0),
            ProvidentFundsBalance = netWorth.AssetBreakdown.GetValueOrDefault("ProvidentFunds", 0),
            RealEstateValue = 0,
            OtherAssetsValue = 0,
            HomeLoanOutstanding = 0,
            PersonalLoanOutstanding = 0,
            CreditCardOutstanding = 0,
            OtherLiabilitiesOutstanding = 0,
            SnapshotType = dto.SnapshotType,
            Notes = dto.Notes
        };

        await _repository.AddAsync(snapshot);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(snapshot);
    }

    public async Task<WealthTimelineDto> GetTimelineAsync(Guid userId, DateTime? fromDate = null, DateTime? toDate = null)
    {
        var snapshots = await _repository.FindAsync(s => s.UserId == userId);
        
        var filteredSnapshots = snapshots
            .Where(s => (!fromDate.HasValue || s.SnapshotDate >= fromDate.Value) &&
                       (!toDate.HasValue || s.SnapshotDate <= toDate.Value))
            .OrderBy(s => s.SnapshotDate)
            .ToList();

        var snapshotDtos = filteredSnapshots.Select(MapToDto).ToList();

        decimal? totalChange = null;
        decimal? percentageChange = null;

        if (filteredSnapshots.Count >= 2)
        {
            var firstSnapshot = filteredSnapshots.First();
            var lastSnapshot = filteredSnapshots.Last();
            totalChange = lastSnapshot.NetWorth - firstSnapshot.NetWorth;
            percentageChange = firstSnapshot.NetWorth != 0 
                ? (totalChange.Value / firstSnapshot.NetWorth) * 100 
                : 0;
        }

        return new WealthTimelineDto
        {
            Snapshots = snapshotDtos,
            TotalChange = totalChange,
            PercentageChange = percentageChange
        };
    }

    public async Task<WealthComparisonDto?> ComparePeriodsAsync(Guid userId, DateTime fromDate, DateTime toDate)
    {
        var snapshots = await _repository.FindAsync(s => s.UserId == userId);
        
        var fromSnapshot = snapshots
            .Where(s => s.SnapshotDate.Date <= fromDate.Date)
            .OrderByDescending(s => s.SnapshotDate)
            .FirstOrDefault();

        var toSnapshot = snapshots
            .Where(s => s.SnapshotDate.Date <= toDate.Date)
            .OrderByDescending(s => s.SnapshotDate)
            .FirstOrDefault();

        if (fromSnapshot == null || toSnapshot == null)
            return null;

        var netWorthChange = toSnapshot.NetWorth - fromSnapshot.NetWorth;
        var percentageChange = fromSnapshot.NetWorth != 0 
            ? (netWorthChange / fromSnapshot.NetWorth) * 100 
            : 0;

        return new WealthComparisonDto
        {
            FromSnapshot = MapToDto(fromSnapshot),
            ToSnapshot = MapToDto(toSnapshot),
            NetWorthChange = netWorthChange,
            PercentageChange = percentageChange,
            AssetChanges = new Dictionary<string, decimal>
            {
                { "Stocks", toSnapshot.StocksValue - fromSnapshot.StocksValue },
                { "MutualFunds", toSnapshot.MutualFundsValue - fromSnapshot.MutualFundsValue },
                { "BankAccounts", toSnapshot.BankAccountsBalance - fromSnapshot.BankAccountsBalance },
                { "FixedDeposits", toSnapshot.FixedDepositsValue - fromSnapshot.FixedDepositsValue },
                { "ProvidentFunds", toSnapshot.ProvidentFundsBalance - fromSnapshot.ProvidentFundsBalance }
            }
        };
    }

    public async Task DeleteAsync(Guid id)
    {
        var snapshot = await _repository.GetByIdAsync(id);
        if (snapshot == null)
            throw new KeyNotFoundException($"Wealth snapshot with ID {id} not found");

        snapshot.IsDeleted = true;
        await _unitOfWork.SaveChangesAsync();
    }

    private static WealthSnapshotDto MapToDto(WealthSnapshot snapshot)
    {
        return new WealthSnapshotDto
        {
            Id = snapshot.Id,
            UserId = snapshot.UserId,
            SnapshotDate = snapshot.SnapshotDate,
            TotalAssets = snapshot.TotalAssets,
            TotalLiabilities = snapshot.TotalLiabilities,
            NetWorth = snapshot.NetWorth,
            StocksValue = snapshot.StocksValue,
            MutualFundsValue = snapshot.MutualFundsValue,
            BankAccountsBalance = snapshot.BankAccountsBalance,
            FixedDepositsValue = snapshot.FixedDepositsValue,
            ProvidentFundsBalance = snapshot.ProvidentFundsBalance,
            RealEstateValue = snapshot.RealEstateValue,
            OtherAssetsValue = snapshot.OtherAssetsValue,
            HomeLoanOutstanding = snapshot.HomeLoanOutstanding,
            PersonalLoanOutstanding = snapshot.PersonalLoanOutstanding,
            CreditCardOutstanding = snapshot.CreditCardOutstanding,
            OtherLiabilitiesOutstanding = snapshot.OtherLiabilitiesOutstanding,
            SnapshotType = snapshot.SnapshotType,
            Notes = snapshot.Notes,
            CreatedAt = snapshot.CreatedAt
        };
    }
}
