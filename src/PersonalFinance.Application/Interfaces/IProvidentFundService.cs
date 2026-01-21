using PersonalFinance.Application.DTOs;

namespace PersonalFinance.Application.Interfaces;

public interface IProvidentFundService
{
    Task<IEnumerable<ProvidentFundDto>> GetAllAsync(Guid userId);
    Task<ProvidentFundDto?> GetByIdAsync(Guid id);
    Task<ProvidentFundDto> CreateAsync(CreateProvidentFundDto dto);
    Task UpdateAsync(Guid id, UpdateProvidentFundDto dto);
    Task DeleteAsync(Guid id);
    Task<PFSummaryDto> GetSummaryAsync(Guid userId);
}
