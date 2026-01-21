using PersonalFinance.Application.DTOs;

namespace PersonalFinance.Application.Interfaces;

public interface IFixedDepositService
{
    Task<IEnumerable<FixedDepositDto>> GetAllAsync(Guid userId);
    Task<FixedDepositDto?> GetByIdAsync(Guid id);
    Task<FixedDepositDto> CreateAsync(CreateFixedDepositDto dto);
    Task UpdateAsync(Guid id, UpdateFixedDepositDto dto);
    Task DeleteAsync(Guid id);
    Task<FixedDepositSummaryDto> GetSummaryAsync(Guid userId);
}
