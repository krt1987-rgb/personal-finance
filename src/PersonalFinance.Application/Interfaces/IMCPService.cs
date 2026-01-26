using PersonalFinance.Application.DTOs;

namespace PersonalFinance.Application.Interfaces;

/// <summary>
/// Service for managing MCP server configurations
/// </summary>
public interface IMCPServerConfigurationService
{
    Task<MCPServerConfigurationDto> CreateConfigurationAsync(Guid userId, CreateMCPServerConfigurationDto request);
    Task<MCPServerConfigurationDto?> GetConfigurationAsync(Guid id);
    Task<IEnumerable<MCPServerConfigurationDto>> GetUserConfigurationsAsync(Guid userId);
    Task<MCPServerConfigurationDto?> GetDefaultConfigurationAsync(Guid userId);
    Task<MCPServerConfigurationDto> UpdateConfigurationAsync(Guid id, UpdateMCPServerConfigurationDto request);
    Task DeleteConfigurationAsync(Guid id);
    Task<MCPConnectionTestDto> TestConnectionAsync(Guid id);
}

/// <summary>
/// Service for fetching data from MCP servers
/// </summary>
public interface IMCPDataService
{
    Task<MCPDataResponseDto> FetchDataAsync(Guid userId, MCPDataRequestDto request);
    Task<IEnumerable<MCPDataResponseDto>> FetchBatchDataAsync(Guid userId, IEnumerable<MCPDataRequestDto> requests);
}
