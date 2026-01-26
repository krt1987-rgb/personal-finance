using PersonalFinance.Domain.Enums;

namespace PersonalFinance.Application.DTOs;

/// <summary>
/// MCP Server Configuration DTO
/// </summary>
public class MCPServerConfigurationDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public MCPProviderType ProviderType { get; set; }
    public string ApiEndpoint { get; set; } = string.Empty;
    public MCPConnectionStatus ConnectionStatus { get; set; }
    public bool IsActive { get; set; }
    public bool IsDefault { get; set; }
    public int Priority { get; set; }
    public int? RequestsPerMinute { get; set; }
    public int? RequestsPerDay { get; set; }
    public DateTime? LastConnectedAt { get; set; }
    public string? LastErrorMessage { get; set; }
    public List<MCPDataType>? SupportedDataTypes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Create MCP Server Configuration Request
/// </summary>
public class CreateMCPServerConfigurationDto
{
    public string Name { get; set; } = string.Empty;
    public MCPProviderType ProviderType { get; set; }
    public string ApiEndpoint { get; set; } = string.Empty;
    public string? ApiKey { get; set; }
    public bool IsDefault { get; set; } = false;
    public int Priority { get; set; } = 0;
    public int? RequestsPerMinute { get; set; }
    public int? RequestsPerDay { get; set; }
    public List<MCPDataType>? SupportedDataTypes { get; set; }
}

/// <summary>
/// Update MCP Server Configuration Request
/// </summary>
public class UpdateMCPServerConfigurationDto
{
    public string? Name { get; set; }
    public string? ApiEndpoint { get; set; }
    public string? ApiKey { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsDefault { get; set; }
    public int? Priority { get; set; }
    public int? RequestsPerMinute { get; set; }
    public int? RequestsPerDay { get; set; }
    public List<MCPDataType>? SupportedDataTypes { get; set; }
}

/// <summary>
/// MCP Data Request
/// </summary>
public class MCPDataRequestDto
{
    public string Symbol { get; set; } = string.Empty;
    public MCPDataType DataType { get; set; }
    public Guid? MCPServerConfigurationId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public Dictionary<string, string>? AdditionalParameters { get; set; }
}

/// <summary>
/// MCP Data Response
/// </summary>
public class MCPDataResponseDto
{
    public string Symbol { get; set; } = string.Empty;
    public MCPDataType DataType { get; set; }
    public string ProviderName { get; set; } = string.Empty;
    public object? Data { get; set; }
    public DateTime FetchedAt { get; set; }
    public bool FromCache { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// MCP Connection Test Result
/// </summary>
public class MCPConnectionTestDto
{
    public Guid ConfigurationId { get; set; }
    public bool IsConnected { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
    public DateTime TestedAt { get; set; }
    public TimeSpan ResponseTime { get; set; }
}
