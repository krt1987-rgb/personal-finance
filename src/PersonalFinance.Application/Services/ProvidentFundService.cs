using PersonalFinance.Application.DTOs;
using PersonalFinance.Application.Interfaces;
using PersonalFinance.Domain.Entities;
using PersonalFinance.Domain.Interfaces;

namespace PersonalFinance.Application.Services;

public class ProvidentFundService : IProvidentFundService
{
    private readonly IRepository<ProvidentFund> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public ProvidentFundService(IRepository<ProvidentFund> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<ProvidentFundDto>> GetAllAsync(Guid userId)
    {
        var funds = await _repository.FindAsync(f => f.UserId == userId);
        return funds.Select(MapToDto);
    }

    public async Task<ProvidentFundDto?> GetByIdAsync(Guid id)
    {
        var fund = await _repository.GetByIdAsync(id);
        return fund == null ? null : MapToDto(fund);
    }

    public async Task<ProvidentFundDto> CreateAsync(CreateProvidentFundDto dto)
    {
        var fund = new ProvidentFund
        {
            UserId = dto.UserId,
            PFType = dto.PFType,
            AccountNumber = dto.AccountNumber,
            UAN = dto.UAN,
            EmployeeContribution = dto.EmployeeContribution,
            EmployerContribution = dto.EmployerContribution,
            CurrentBalance = dto.CurrentBalance,
            InterestRate = dto.InterestRate,
            LastUpdatedBalance = DateTime.UtcNow,
            Organization = dto.Organization
        };

        await _repository.AddAsync(fund);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(fund);
    }

    public async Task UpdateAsync(Guid id, UpdateProvidentFundDto dto)
    {
        var fund = await _repository.GetByIdAsync(id);
        if (fund == null)
            throw new KeyNotFoundException($"Provident fund with ID {id} not found");

        if (dto.EmployeeContribution.HasValue) fund.EmployeeContribution = dto.EmployeeContribution.Value;
        if (dto.EmployerContribution.HasValue) fund.EmployerContribution = dto.EmployerContribution.Value;
        if (dto.CurrentBalance.HasValue)
        {
            fund.CurrentBalance = dto.CurrentBalance.Value;
            fund.LastUpdatedBalance = DateTime.UtcNow;
        }
        if (dto.InterestRate.HasValue) fund.InterestRate = dto.InterestRate.Value;

        await _repository.UpdateAsync(fund);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var fund = await _repository.GetByIdAsync(id);
        if (fund == null)
            throw new KeyNotFoundException($"Provident fund with ID {id} not found");

        await _repository.DeleteAsync(fund);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<PFSummaryDto> GetSummaryAsync(Guid userId)
    {
        var funds = await _repository.FindAsync(f => f.UserId == userId);
        var fundsList = funds.ToList();

        var byType = fundsList
            .GroupBy(f => f.PFType)
            .Select(g => new PFTypeSummary
            {
                Type = g.Key.ToString(),
                Count = g.Count(),
                TotalBalance = g.Sum(f => f.CurrentBalance)
            })
            .ToList();

        return new PFSummaryDto
        {
            TotalAccounts = fundsList.Count,
            TotalBalance = fundsList.Sum(f => f.CurrentBalance),
            TotalEmployeeContribution = fundsList.Sum(f => f.EmployeeContribution),
            TotalEmployerContribution = fundsList.Sum(f => f.EmployerContribution),
            ByType = byType
        };
    }

    private static ProvidentFundDto MapToDto(ProvidentFund fund)
    {
        return new ProvidentFundDto
        {
            Id = fund.Id,
            UserId = fund.UserId,
            PFType = fund.PFType,
            AccountNumber = fund.AccountNumber,
            UAN = fund.UAN,
            EmployeeContribution = fund.EmployeeContribution,
            EmployerContribution = fund.EmployerContribution,
            CurrentBalance = fund.CurrentBalance,
            InterestRate = fund.InterestRate,
            LastUpdatedBalance = fund.LastUpdatedBalance,
            Organization = fund.Organization
        };
    }
}
