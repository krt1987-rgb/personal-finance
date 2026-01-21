using PersonalFinance.Application.DTOs;

namespace PersonalFinance.Application.Interfaces;

public interface IMutualFundService
{
    Task<IEnumerable<MutualFundHoldingDto>> GetAllAsync(Guid userId);
    Task<MutualFundHoldingDto?> GetByIdAsync(Guid id);
    Task<MutualFundHoldingDto> CreateAsync(CreateMutualFundHoldingDto dto);
    Task UpdateAsync(Guid id, UpdateMutualFundHoldingDto dto);
    Task DeleteAsync(Guid id);
    Task<MutualFundPortfolioSummaryDto> GetPortfolioSummaryAsync(Guid userId);
}
