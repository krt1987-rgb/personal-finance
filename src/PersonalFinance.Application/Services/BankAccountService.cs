using PersonalFinance.Application.DTOs;
using PersonalFinance.Application.Interfaces;
using PersonalFinance.Domain.Entities;
using PersonalFinance.Domain.Interfaces;

namespace PersonalFinance.Application.Services;

public class BankAccountService : IBankAccountService
{
    private readonly IRepository<BankAccount> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public BankAccountService(IRepository<BankAccount> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<BankAccountDto>> GetAllAsync(Guid userId)
    {
        var accounts = await _repository.FindAsync(a => a.UserId == userId);
        return accounts.Select(MapToDto);
    }

    public async Task<BankAccountDto?> GetByIdAsync(Guid id)
    {
        var account = await _repository.GetByIdAsync(id);
        return account == null ? null : MapToDto(account);
    }

    public async Task<BankAccountDto> CreateAsync(CreateBankAccountDto dto)
    {
        var account = new BankAccount
        {
            UserId = dto.UserId,
            BankName = dto.BankName,
            AccountNumber = dto.AccountNumber,
            IFSC = dto.IFSC,
            AccountType = dto.AccountType,
            CurrentBalance = dto.CurrentBalance,
            Currency = dto.Currency,
            IsActive = true
        };

        await _repository.AddAsync(account);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(account);
    }

    public async Task UpdateAsync(Guid id, UpdateBankAccountDto dto)
    {
        var account = await _repository.GetByIdAsync(id);
        if (account == null)
            throw new KeyNotFoundException($"Bank account with ID {id} not found");

        if (dto.BankName != null) account.BankName = dto.BankName;
        if (dto.CurrentBalance.HasValue) account.CurrentBalance = dto.CurrentBalance.Value;
        if (dto.IsActive.HasValue) account.IsActive = dto.IsActive.Value;

        await _repository.UpdateAsync(account);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var account = await _repository.GetByIdAsync(id);
        if (account == null)
            throw new KeyNotFoundException($"Bank account with ID {id} not found");

        await _repository.DeleteAsync(account);
        await _unitOfWork.SaveChangesAsync();
    }

    private static BankAccountDto MapToDto(BankAccount account)
    {
        return new BankAccountDto
        {
            Id = account.Id,
            UserId = account.UserId,
            BankName = account.BankName,
            AccountNumber = account.AccountNumber,
            IFSC = account.IFSC,
            AccountType = account.AccountType,
            CurrentBalance = account.CurrentBalance,
            Currency = account.Currency,
            IsActive = account.IsActive
        };
    }
}
