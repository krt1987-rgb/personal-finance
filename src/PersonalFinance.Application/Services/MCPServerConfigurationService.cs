using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using PersonalFinance.Application.DTOs;
using PersonalFinance.Application.Interfaces;
using PersonalFinance.Domain.Entities;
using PersonalFinance.Domain.Enums;
using PersonalFinance.Domain.Interfaces;

namespace PersonalFinance.Application.Services;

/// <summary>
/// Service for managing MCP server configurations
/// </summary>
public class MCPServerConfigurationService : IMCPServerConfigurationService
{
    private readonly IRepository<MCPServerConfiguration> _configRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<MCPServerConfigurationService> _logger;

    public MCPServerConfigurationService(
        IRepository<MCPServerConfiguration> configRepository,
        IUnitOfWork unitOfWork,
        IHttpClientFactory httpClientFactory,
        ILogger<MCPServerConfigurationService> logger)
    {
        _configRepository = configRepository;
        _unitOfWork = unitOfWork;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<MCPServerConfigurationDto> CreateConfigurationAsync(Guid userId, CreateMCPServerConfigurationDto request)
    {
        // If setting as default, unset other defaults
        if (request.IsDefault)
        {
            await UnsetDefaultConfigurationsAsync(userId);
        }

        var config = new MCPServerConfiguration
        {
            UserId = userId,
            Name = request.Name,
            ProviderType = request.ProviderType,
            ApiEndpoint = request.ApiEndpoint,
            ApiKey = request.ApiKey,
            IsDefault = request.IsDefault,
            Priority = request.Priority,
            RequestsPerMinute = request.RequestsPerMinute,
            RequestsPerDay = request.RequestsPerDay,
            SupportedDataTypes = request.SupportedDataTypes != null 
                ? string.Join(",", request.SupportedDataTypes.Select(d => (int)d))
                : null
        };

        await _configRepository.AddAsync(config);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Created MCP server configuration {ConfigId} for user {UserId}", config.Id, userId);

        return MapToDto(config);
    }

    public async Task<MCPServerConfigurationDto?> GetConfigurationAsync(Guid id)
    {
        var config = await _configRepository.GetByIdAsync(id);
        return config == null ? null : MapToDto(config);
    }

    public async Task<IEnumerable<MCPServerConfigurationDto>> GetUserConfigurationsAsync(Guid userId)
    {
        var configs = await _configRepository.FindAsync(c => c.UserId == userId && !c.IsDeleted);
        return configs.OrderByDescending(c => c.Priority).Select(MapToDto);
    }

    public async Task<MCPServerConfigurationDto?> GetDefaultConfigurationAsync(Guid userId)
    {
        var configs = await _configRepository.FindAsync(c => 
            c.UserId == userId && 
            c.IsDefault && 
            c.IsActive && 
            !c.IsDeleted);
        
        var config = configs.FirstOrDefault();
        return config == null ? null : MapToDto(config);
    }

    public async Task<MCPServerConfigurationDto> UpdateConfigurationAsync(Guid id, UpdateMCPServerConfigurationDto request)
    {
        var config = await _configRepository.GetByIdAsync(id);
        if (config == null)
        {
            throw new InvalidOperationException("MCP server configuration not found");
        }

        // If setting as default, unset other defaults
        if (request.IsDefault == true)
        {
            await UnsetDefaultConfigurationsAsync(config.UserId);
        }

        // Update fields
        if (request.Name != null) config.Name = request.Name;
        if (request.ApiEndpoint != null) config.ApiEndpoint = request.ApiEndpoint;
        if (request.ApiKey != null) config.ApiKey = request.ApiKey;
        if (request.IsActive.HasValue) config.IsActive = request.IsActive.Value;
        if (request.IsDefault.HasValue) config.IsDefault = request.IsDefault.Value;
        if (request.Priority.HasValue) config.Priority = request.Priority.Value;
        if (request.RequestsPerMinute.HasValue) config.RequestsPerMinute = request.RequestsPerMinute;
        if (request.RequestsPerDay.HasValue) config.RequestsPerDay = request.RequestsPerDay;
        if (request.SupportedDataTypes != null)
        {
            config.SupportedDataTypes = string.Join(",", request.SupportedDataTypes.Select(d => (int)d));
        }

        await _configRepository.UpdateAsync(config);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Updated MCP server configuration {ConfigId}", id);

        return MapToDto(config);
    }

    public async Task DeleteConfigurationAsync(Guid id)
    {
        var config = await _configRepository.GetByIdAsync(id);
        if (config == null)
        {
            throw new InvalidOperationException("MCP server configuration not found");
        }

        await _configRepository.DeleteAsync(config);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Deleted MCP server configuration {ConfigId}", id);
    }

    public async Task<MCPConnectionTestDto> TestConnectionAsync(Guid id)
    {
        var config = await _configRepository.GetByIdAsync(id);
        if (config == null)
        {
            throw new InvalidOperationException("MCP server configuration not found");
        }

        var startTime = DateTime.UtcNow;
        
        try
        {
            using var httpClient = _httpClientFactory.CreateClient();
            httpClient.Timeout = TimeSpan.FromSeconds(10);
            
            // Test connection by sending a simple MCP tools/list request
            var testRequest = new
            {
                method = "tools/list",
                @params = new { }
            };

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, config.ApiEndpoint)
            {
                Content = new StringContent(JsonSerializer.Serialize(testRequest), Encoding.UTF8, "application/json")
            };

            if (!string.IsNullOrEmpty(config.ApiKey))
            {
                requestMessage.Headers.Add("Authorization", $"Bearer {config.ApiKey}");
            }

            _logger.LogInformation("Testing connection to MCP server at {Endpoint}", config.ApiEndpoint);
            
            var response = await httpClient.SendAsync(requestMessage);
            var responseTime = DateTime.UtcNow - startTime;
            
            if (response.IsSuccessStatusCode)
            {
                config.ConnectionStatus = MCPConnectionStatus.Connected;
                config.LastConnectedAt = DateTime.UtcNow;
                config.LastErrorMessage = null;
                
                await _configRepository.UpdateAsync(config);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Successfully connected to MCP server {ConfigId} in {ResponseTime}ms", 
                    id, responseTime.TotalMilliseconds);

                return new MCPConnectionTestDto
                {
                    ConfigurationId = id,
                    IsConnected = true,
                    Status = $"Connected successfully (HTTP {(int)response.StatusCode})",
                    TestedAt = DateTime.UtcNow,
                    ResponseTime = responseTime
                };
            }
            else
            {
                var errorMessage = $"HTTP {(int)response.StatusCode}: {response.ReasonPhrase}";
                _logger.LogWarning("Connection test failed for MCP server {ConfigId}: {StatusCode} {ReasonPhrase}", 
                    id, (int)response.StatusCode, response.ReasonPhrase);
                
                config.ConnectionStatus = MCPConnectionStatus.Error;
                config.LastErrorMessage = errorMessage;
                
                await _configRepository.UpdateAsync(config);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogWarning("Connection test failed for MCP server {ConfigId}: {Error}", id, errorMessage);

                return new MCPConnectionTestDto
                {
                    ConfigurationId = id,
                    IsConnected = false,
                    Status = "Connection failed",
                    ErrorMessage = errorMessage,
                    TestedAt = DateTime.UtcNow,
                    ResponseTime = responseTime
                };
            }
        }
        catch (Exception ex)
        {
            config.ConnectionStatus = MCPConnectionStatus.Error;
            config.LastErrorMessage = ex.Message;
            
            await _configRepository.UpdateAsync(config);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogError(ex, "Failed to connect to MCP server {ConfigId}", id);

            return new MCPConnectionTestDto
            {
                ConfigurationId = id,
                IsConnected = false,
                Status = "Connection failed",
                ErrorMessage = ex.Message,
                TestedAt = DateTime.UtcNow,
                ResponseTime = DateTime.UtcNow - startTime
            };
        }
    }

    private async Task UnsetDefaultConfigurationsAsync(Guid userId)
    {
        var defaultConfigs = await _configRepository.FindAsync(c => 
            c.UserId == userId && 
            c.IsDefault && 
            !c.IsDeleted);

        foreach (var config in defaultConfigs)
        {
            config.IsDefault = false;
            await _configRepository.UpdateAsync(config);
        }
    }

    private static MCPServerConfigurationDto MapToDto(MCPServerConfiguration config)
    {
        return new MCPServerConfigurationDto
        {
            Id = config.Id,
            UserId = config.UserId,
            Name = config.Name,
            ProviderType = config.ProviderType,
            ApiEndpoint = config.ApiEndpoint,
            ConnectionStatus = config.ConnectionStatus,
            IsActive = config.IsActive,
            IsDefault = config.IsDefault,
            Priority = config.Priority,
            RequestsPerMinute = config.RequestsPerMinute,
            RequestsPerDay = config.RequestsPerDay,
            LastConnectedAt = config.LastConnectedAt,
            LastErrorMessage = config.LastErrorMessage,
            SupportedDataTypes = !string.IsNullOrEmpty(config.SupportedDataTypes)
                ? config.SupportedDataTypes.Split(',').Select(s => (MCPDataType)int.Parse(s)).ToList()
                : null,
            CreatedAt = config.CreatedAt,
            UpdatedAt = config.UpdatedAt
        };
    }
}
