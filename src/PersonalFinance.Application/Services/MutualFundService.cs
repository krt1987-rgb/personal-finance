using PersonalFinance.Application.DTOs;
using PersonalFinance.Application.Interfaces;
using PersonalFinance.Domain.Entities;
using PersonalFinance.Domain.Interfaces;

namespace PersonalFinance.Application.Services;

public class MutualFundService : IMutualFundService
{
    private readonly IRepository<MutualFundHolding> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public MutualFundService(IRepository<MutualFundHolding> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<MutualFundHoldingDto>> GetAllAsync(Guid userId)
    {
        var holdings = await _repository.FindAsync(h => h.UserId == userId);
        return holdings.Select(MapToDto);
    }

    public async Task<MutualFundHoldingDto?> GetByIdAsync(Guid id)
    {
        var holding = await _repository.GetByIdAsync(id);
        return holding == null ? null : MapToDto(holding);
    }

    public async Task<MutualFundHoldingDto> CreateAsync(CreateMutualFundHoldingDto dto)
    {
        var holding = new MutualFundHolding
        {
            UserId = dto.UserId,
            FolioNumber = dto.FolioNumber,
            SchemeName = dto.SchemeName,
            AMC = dto.AMC,
            ISIN = dto.ISIN,
            Category = dto.Category,
            Units = dto.Units,
            AverageNAV = dto.AverageNAV,
            InvestmentMode = dto.InvestmentMode
        };

        await _repository.AddAsync(holding);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(holding);
    }

    public async Task UpdateAsync(Guid id, UpdateMutualFundHoldingDto dto)
    {
        var holding = await _repository.GetByIdAsync(id);
        if (holding == null)
            throw new KeyNotFoundException($"Mutual fund holding with ID {id} not found");

        if (dto.Units.HasValue) holding.Units = dto.Units.Value;
        if (dto.AverageNAV.HasValue) holding.AverageNAV = dto.AverageNAV.Value;
        if (dto.CurrentNAV.HasValue) holding.CurrentNAV = dto.CurrentNAV.Value;

        await _repository.UpdateAsync(holding);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var holding = await _repository.GetByIdAsync(id);
        if (holding == null)
            throw new KeyNotFoundException($"Mutual fund holding with ID {id} not found");

        await _repository.DeleteAsync(holding);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<MutualFundPortfolioSummaryDto> GetPortfolioSummaryAsync(Guid userId)
    {
        var holdings = await _repository.FindAsync(h => h.UserId == userId);
        var holdingsList = holdings.ToList();

        return new MutualFundPortfolioSummaryDto
        {
            TotalInvestment = holdingsList.Sum(h => h.InvestedAmount),
            CurrentValue = holdingsList.Sum(h => h.CurrentValue ?? 0),
            TotalProfitLoss = holdingsList.Sum(h => h.ProfitLoss ?? 0),
            TotalHoldings = holdingsList.Count
        };
    }

    private static MutualFundHoldingDto MapToDto(MutualFundHolding holding)
    {
        return new MutualFundHoldingDto
        {
            Id = holding.Id,
            UserId = holding.UserId,
            FolioNumber = holding.FolioNumber,
            SchemeName = holding.SchemeName,
            AMC = holding.AMC,
            ISIN = holding.ISIN,
            Category = holding.Category,
            Units = holding.Units,
            AverageNAV = holding.AverageNAV,
            CurrentNAV = holding.CurrentNAV,
            LastNAVUpdate = holding.LastNAVUpdate,
            InvestmentMode = holding.InvestmentMode,
            InvestedAmount = holding.InvestedAmount,
            CurrentValue = holding.CurrentValue,
            ProfitLoss = holding.ProfitLoss,
            ProfitLossPercentage = holding.ProfitLossPercentage
        };
    }
}
