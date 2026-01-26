using PersonalFinance.Application.DTOs;

namespace PersonalFinance.Application.Interfaces;

public interface IWealthSnapshotService
{
    Task<IEnumerable<WealthSnapshotDto>> GetAllAsync(Guid userId);
    Task<WealthSnapshotDto?> GetLatestAsync(Guid userId);
    Task<WealthSnapshotDto> CreateSnapshotAsync(Guid userId, CreateWealthSnapshotDto dto);
    Task<WealthTimelineDto> GetTimelineAsync(Guid userId, DateTime? fromDate = null, DateTime? toDate = null);
    Task<WealthComparisonDto?> ComparePeriodsAsync(Guid userId, DateTime fromDate, DateTime toDate);
    Task DeleteAsync(Guid id);
}

public interface INetWorthService
{
    Task<NetWorthDto> CalculateCurrentNetWorthAsync(Guid userId);
}
