using PersonalFinance.Application.DTOs;

namespace PersonalFinance.Application.Interfaces;

public interface IBankAccountService
{
    Task<IEnumerable<BankAccountDto>> GetAllAsync(Guid userId);
    Task<BankAccountDto?> GetByIdAsync(Guid id);
    Task<BankAccountDto> CreateAsync(CreateBankAccountDto dto);
    Task UpdateAsync(Guid id, UpdateBankAccountDto dto);
    Task DeleteAsync(Guid id);
}
