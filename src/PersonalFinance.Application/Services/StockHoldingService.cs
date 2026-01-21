using PersonalFinance.Application.DTOs;
using PersonalFinance.Application.Interfaces;
using PersonalFinance.Domain.Entities;
using PersonalFinance.Domain.Interfaces;

namespace PersonalFinance.Application.Services;

public class StockHoldingService : IStockHoldingService
{
    private readonly IRepository<StockHolding> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public StockHoldingService(IRepository<StockHolding> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<StockHoldingDto>> GetAllAsync(Guid userId)
    {
        var holdings = await _repository.FindAsync(h => h.UserId == userId);
        return holdings.Select(MapToDto);
    }

    public async Task<StockHoldingDto?> GetByIdAsync(Guid id)
    {
        var holding = await _repository.GetByIdAsync(id);
        return holding == null ? null : MapToDto(holding);
    }

    public async Task<StockHoldingDto> CreateAsync(CreateStockHoldingDto dto)
    {
        var holding = new StockHolding
        {
            UserId = dto.UserId,
            Symbol = dto.Symbol,
            CompanyName = dto.CompanyName,
            Exchange = dto.Exchange,
            Quantity = dto.Quantity,
            AverageBuyPrice = dto.AverageBuyPrice,
            ISIN = dto.ISIN,
            Sector = dto.Sector
        };

        await _repository.AddAsync(holding);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(holding);
    }

    public async Task UpdateAsync(Guid id, UpdateStockHoldingDto dto)
    {
        var holding = await _repository.GetByIdAsync(id);
        if (holding == null)
            throw new KeyNotFoundException($"Stock holding with ID {id} not found");

        if (dto.CompanyName != null) holding.CompanyName = dto.CompanyName;
        if (dto.Quantity.HasValue) holding.Quantity = dto.Quantity.Value;
        if (dto.AverageBuyPrice.HasValue) holding.AverageBuyPrice = dto.AverageBuyPrice.Value;
        if (dto.CurrentPrice.HasValue) holding.CurrentPrice = dto.CurrentPrice.Value;
        if (dto.Sector != null) holding.Sector = dto.Sector;

        await _repository.UpdateAsync(holding);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var holding = await _repository.GetByIdAsync(id);
        if (holding == null)
            throw new KeyNotFoundException($"Stock holding with ID {id} not found");

        await _repository.DeleteAsync(holding);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<PortfolioSummaryDto> GetPortfolioSummaryAsync(Guid userId)
    {
        var holdings = await _repository.FindAsync(h => h.UserId == userId);
        var holdingsList = holdings.ToList();

        return new PortfolioSummaryDto
        {
            TotalInvestment = holdingsList.Sum(h => h.InvestedAmount),
            CurrentValue = holdingsList.Sum(h => h.CurrentValue ?? 0),
            TotalProfitLoss = holdingsList.Sum(h => h.ProfitLoss ?? 0),
            TotalHoldings = holdingsList.Count
        };
    }

    private static StockHoldingDto MapToDto(StockHolding holding)
    {
        return new StockHoldingDto
        {
            Id = holding.Id,
            UserId = holding.UserId,
            Symbol = holding.Symbol,
            CompanyName = holding.CompanyName,
            Exchange = holding.Exchange,
            Quantity = holding.Quantity,
            AverageBuyPrice = holding.AverageBuyPrice,
            CurrentPrice = holding.CurrentPrice,
            LastPriceUpdate = holding.LastPriceUpdate,
            ISIN = holding.ISIN,
            Sector = holding.Sector,
            InvestedAmount = holding.InvestedAmount,
            CurrentValue = holding.CurrentValue,
            ProfitLoss = holding.ProfitLoss,
            ProfitLossPercentage = holding.ProfitLossPercentage
        };
    }
}
