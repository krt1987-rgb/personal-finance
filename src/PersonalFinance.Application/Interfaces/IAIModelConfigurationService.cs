using PersonalFinance.Application.DTOs;

namespace PersonalFinance.Application.Interfaces;

public interface IAIModelConfigurationService
{
    Task<IEnumerable<AIModelConfigurationDto>> GetAllAsync(Guid userId);
    Task<AIModelConfigurationDto?> GetByIdAsync(Guid id);
    Task<AIModelConfigurationDto?> GetDefaultAsync(Guid userId);
    Task<AIModelConfigurationDto> CreateAsync(Guid userId, CreateAIModelConfigurationDto dto);
    Task UpdateAsync(Guid id, UpdateAIModelConfigurationDto dto);
    Task DeleteAsync(Guid id);
    Task SetAsDefaultAsync(Guid id, Guid userId);
    Task<IEnumerable<AIProviderStatusDto>> GetProviderStatusAsync(Guid userId);
}
