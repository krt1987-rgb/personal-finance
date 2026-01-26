using Microsoft.Extensions.Logging;
using PersonalFinance.Application.DTOs;
using PersonalFinance.Application.Interfaces;
using PersonalFinance.Domain.Entities;
using PersonalFinance.Domain.Interfaces;
using PersonalFinance.Domain.Enums;

namespace PersonalFinance.Application.Services;

public class AIModelConfigurationService : IAIModelConfigurationService
{
    private readonly IRepository<AIModelConfiguration> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAIService _aiService;
    private readonly ILogger<AIModelConfigurationService> _logger;

    public AIModelConfigurationService(
        IRepository<AIModelConfiguration> repository,
        IUnitOfWork unitOfWork,
        IAIService aiService,
        ILogger<AIModelConfigurationService> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _aiService = aiService;
        _logger = logger;
    }

    public async Task<IEnumerable<AIModelConfigurationDto>> GetAllAsync(Guid userId)
    {
        var configs = await _repository.FindAsync(c => c.UserId == userId && !c.IsDeleted);
        return configs.Select(MapToDto).OrderByDescending(c => c.IsDefault).ThenBy(c => c.Priority);
    }

    public async Task<AIModelConfigurationDto?> GetByIdAsync(Guid id)
    {
        var config = await _repository.GetByIdAsync(id);
        return config == null || config.IsDeleted ? null : MapToDto(config);
    }

    public async Task<AIModelConfigurationDto?> GetDefaultAsync(Guid userId)
    {
        var configs = await _repository.FindAsync(c => c.UserId == userId && c.IsDefault && c.IsActive && !c.IsDeleted);
        var defaultConfig = configs.FirstOrDefault();
        return defaultConfig == null ? null : MapToDto(defaultConfig);
    }

    public async Task<AIModelConfigurationDto> CreateAsync(Guid userId, CreateAIModelConfigurationDto dto)
    {
        // Validate API key
        var isValid = await _aiService.ValidateApiKeyAsync(dto.ProviderType, dto.ApiKey, dto.ApiEndpoint);
        if (!isValid)
        {
            throw new InvalidOperationException("Invalid API key or configuration");
        }

        // If this is set as default, unset other defaults
        if (dto.IsDefault)
        {
            await UnsetOtherDefaultsAsync(userId);
        }

        var config = new AIModelConfiguration
        {
            UserId = userId,
            Name = dto.Name,
            ProviderType = dto.ProviderType,
            ModelType = dto.ModelType,
            CustomModelName = dto.CustomModelName,
            ApiKey = dto.ApiKey,
            ApiEndpoint = dto.ApiEndpoint,
            IsDefault = dto.IsDefault,
            Priority = dto.Priority,
            Temperature = dto.Temperature,
            MaxTokens = dto.MaxTokens,
            TopP = dto.TopP,
            FrequencyPenalty = dto.FrequencyPenalty,
            PresencePenalty = dto.PresencePenalty,
            RequestsPerMinute = dto.RequestsPerMinute,
            RequestsPerDay = dto.RequestsPerDay
        };

        await _repository.AddAsync(config);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("AI Model Configuration created: {ConfigId} for user {UserId}", config.Id, userId);
        return MapToDto(config);
    }

    public async Task UpdateAsync(Guid id, UpdateAIModelConfigurationDto dto)
    {
        var config = await _repository.GetByIdAsync(id);
        if (config == null || config.IsDeleted)
            throw new KeyNotFoundException($"AI Model Configuration with ID {id} not found");

        if (dto.Name != null) config.Name = dto.Name;
        if (dto.ApiKey != null)
        {
            // Validate new API key
            var isValid = await _aiService.ValidateApiKeyAsync(
                config.ProviderType,
                dto.ApiKey,
                dto.ApiEndpoint ?? config.ApiEndpoint);
            
            if (!isValid)
            {
                throw new InvalidOperationException("Invalid API key");
            }
            config.ApiKey = dto.ApiKey;
        }
        if (dto.ApiEndpoint != null) config.ApiEndpoint = dto.ApiEndpoint;
        if (dto.IsActive.HasValue) config.IsActive = dto.IsActive.Value;
        if (dto.IsDefault.HasValue && dto.IsDefault.Value)
        {
            await UnsetOtherDefaultsAsync(config.UserId, id);
            config.IsDefault = true;
        }
        if (dto.Priority.HasValue) config.Priority = dto.Priority.Value;
        if (dto.Temperature.HasValue) config.Temperature = dto.Temperature.Value;
        if (dto.MaxTokens.HasValue) config.MaxTokens = dto.MaxTokens.Value;
        if (dto.TopP.HasValue) config.TopP = dto.TopP.Value;
        if (dto.FrequencyPenalty.HasValue) config.FrequencyPenalty = dto.FrequencyPenalty.Value;
        if (dto.PresencePenalty.HasValue) config.PresencePenalty = dto.PresencePenalty.Value;
        if (dto.RequestsPerMinute.HasValue) config.RequestsPerMinute = dto.RequestsPerMinute.Value;
        if (dto.RequestsPerDay.HasValue) config.RequestsPerDay = dto.RequestsPerDay.Value;

        config.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(config);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("AI Model Configuration updated: {ConfigId}", id);
    }

    public async Task DeleteAsync(Guid id)
    {
        var config = await _repository.GetByIdAsync(id);
        if (config == null || config.IsDeleted)
            throw new KeyNotFoundException($"AI Model Configuration with ID {id} not found");

        config.IsDeleted = true;
        config.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(config);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("AI Model Configuration deleted: {ConfigId}", id);
    }

    public async Task SetAsDefaultAsync(Guid id, Guid userId)
    {
        var config = await _repository.GetByIdAsync(id);
        if (config == null || config.IsDeleted || config.UserId != userId)
            throw new KeyNotFoundException($"AI Model Configuration with ID {id} not found");

        await UnsetOtherDefaultsAsync(userId, id);
        
        config.IsDefault = true;
        config.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(config);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("AI Model Configuration set as default: {ConfigId}", id);
    }

    public async Task<IEnumerable<AIProviderStatusDto>> GetProviderStatusAsync(Guid userId)
    {
        var configs = await _repository.FindAsync(c => c.UserId == userId && c.IsActive && !c.IsDeleted);
        
        return configs.Select(c => new AIProviderStatusDto
        {
            Name = c.Name,
            ProviderType = c.ProviderType,
            IsActive = c.IsActive,
            IsDefault = c.IsDefault,
            Priority = c.Priority,
            Status = "Active"
        }).ToList();
    }

    private async Task UnsetOtherDefaultsAsync(Guid userId, Guid? excludeId = null)
    {
        var defaultConfigs = await _repository.FindAsync(c => 
            c.UserId == userId && 
            c.IsDefault && 
            !c.IsDeleted &&
            (excludeId == null || c.Id != excludeId));

        foreach (var config in defaultConfigs)
        {
            config.IsDefault = false;
            config.UpdatedAt = DateTime.UtcNow;
            await _repository.UpdateAsync(config);
        }
    }

    private static AIModelConfigurationDto MapToDto(AIModelConfiguration config)
    {
        return new AIModelConfigurationDto
        {
            Id = config.Id,
            UserId = config.UserId,
            Name = config.Name,
            ProviderType = config.ProviderType,
            ModelType = config.ModelType,
            CustomModelName = config.CustomModelName,
            ApiEndpoint = config.ApiEndpoint,
            IsActive = config.IsActive,
            IsDefault = config.IsDefault,
            Priority = config.Priority,
            Temperature = config.Temperature,
            MaxTokens = config.MaxTokens,
            TopP = config.TopP,
            FrequencyPenalty = config.FrequencyPenalty,
            PresencePenalty = config.PresencePenalty,
            RequestsPerMinute = config.RequestsPerMinute,
            RequestsPerDay = config.RequestsPerDay,
            CreatedAt = config.CreatedAt,
            UpdatedAt = config.UpdatedAt
        };
    }
}
