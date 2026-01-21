using PersonalFinance.Application.DTOs;
using PersonalFinance.Application.Interfaces;
using PersonalFinance.Domain.Entities;
using PersonalFinance.Domain.Enums;
using PersonalFinance.Domain.Interfaces;

namespace PersonalFinance.Application.Services;

public class FixedDepositService : IFixedDepositService
{
    private readonly IRepository<FixedDeposit> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public FixedDepositService(IRepository<FixedDeposit> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<FixedDepositDto>> GetAllAsync(Guid userId)
    {
        // For now, get all deposits. In a real implementation, filter by user's bank accounts
        var deposits = await _repository.GetAllAsync();
        return deposits.Select(MapToDto);
    }

    public async Task<FixedDepositDto?> GetByIdAsync(Guid id)
    {
        var deposit = await _repository.GetByIdAsync(id);
        return deposit == null ? null : MapToDto(deposit);
    }

    public async Task<FixedDepositDto> CreateAsync(CreateFixedDepositDto dto)
    {
        var maturityDate = dto.StartDate.AddMonths(dto.TenureInMonths);
        var maturityAmount = CalculateMaturityAmount(dto.PrincipalAmount, dto.InterestRate, dto.TenureInMonths);

        var deposit = new FixedDeposit
        {
            BankAccountId = dto.BankAccountId,
            FDNumber = dto.FDNumber,
            PrincipalAmount = dto.PrincipalAmount,
            InterestRate = dto.InterestRate,
            TenureInMonths = dto.TenureInMonths,
            StartDate = dto.StartDate,
            MaturityDate = maturityDate,
            MaturityAmount = maturityAmount,
            Status = FDStatus.Active,
            Notes = dto.Notes
        };

        await _repository.AddAsync(deposit);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(deposit);
    }

    public async Task UpdateAsync(Guid id, UpdateFixedDepositDto dto)
    {
        var deposit = await _repository.GetByIdAsync(id);
        if (deposit == null)
            throw new KeyNotFoundException($"Fixed deposit with ID {id} not found");

        if (dto.InterestRate.HasValue) deposit.InterestRate = dto.InterestRate.Value;
        if (dto.Status.HasValue) deposit.Status = dto.Status.Value;
        if (dto.Notes != null) deposit.Notes = dto.Notes;

        await _repository.UpdateAsync(deposit);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var deposit = await _repository.GetByIdAsync(id);
        if (deposit == null)
            throw new KeyNotFoundException($"Fixed deposit with ID {id} not found");

        await _repository.DeleteAsync(deposit);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<FixedDepositSummaryDto> GetSummaryAsync(Guid userId)
    {
        var deposits = await _repository.GetAllAsync();
        var depositsList = deposits.ToList();

        return new FixedDepositSummaryDto
        {
            TotalDeposits = depositsList.Count,
            TotalPrincipal = depositsList.Sum(d => d.PrincipalAmount),
            TotalMaturityAmount = depositsList.Sum(d => d.MaturityAmount),
            ActiveDeposits = depositsList.Count(d => d.Status == FDStatus.Active),
            MaturedDeposits = depositsList.Count(d => d.Status == FDStatus.Matured)
        };
    }

    private static decimal CalculateMaturityAmount(decimal principal, decimal rate, int months)
    {
        // Simple interest calculation: A = P(1 + rt)
        var years = months / 12.0m;
        return principal * (1 + (rate / 100) * years);
    }

    private static FixedDepositDto MapToDto(FixedDeposit deposit)
    {
        return new FixedDepositDto
        {
            Id = deposit.Id,
            BankAccountId = deposit.BankAccountId,
            FDNumber = deposit.FDNumber,
            PrincipalAmount = deposit.PrincipalAmount,
            InterestRate = deposit.InterestRate,
            TenureInMonths = deposit.TenureInMonths,
            StartDate = deposit.StartDate,
            MaturityDate = deposit.MaturityDate,
            MaturityAmount = deposit.MaturityAmount,
            Status = deposit.Status,
            Notes = deposit.Notes
        };
    }
}
