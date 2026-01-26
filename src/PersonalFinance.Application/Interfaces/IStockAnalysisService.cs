using PersonalFinance.Application.DTOs;

namespace PersonalFinance.Application.Interfaces;

public interface IStockAnalysisService
{
    Task<StockAnalysisDto> CreateAnalysisAsync(Guid userId, CreateStockAnalysisRequestDto request);
    Task<StockAnalysisDto?> GetAnalysisAsync(Guid id);
    Task<IEnumerable<StockAnalysisDto>> GetAnalysesBySymbolAsync(Guid userId, string symbol);
    Task<IEnumerable<StockAnalysisDto>> GetUserAnalysesAsync(Guid userId, int limit = 50);
    Task<StockResearchResponseDto> ResearchStockAsync(Guid userId, StockResearchRequestDto request);
}
