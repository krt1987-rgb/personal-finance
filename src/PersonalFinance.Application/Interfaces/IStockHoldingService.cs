using PersonalFinance.Application.DTOs;

namespace PersonalFinance.Application.Interfaces;

public interface IStockHoldingService
{
    Task<IEnumerable<StockHoldingDto>> GetAllAsync(Guid userId);
    Task<StockHoldingDto?> GetByIdAsync(Guid id);
    Task<StockHoldingDto> CreateAsync(CreateStockHoldingDto dto);
    Task UpdateAsync(Guid id, UpdateStockHoldingDto dto);
    Task DeleteAsync(Guid id);
    Task<PortfolioSummaryDto> GetPortfolioSummaryAsync(Guid userId);
}
