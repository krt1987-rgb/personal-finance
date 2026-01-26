using PersonalFinance.Domain.Common;
using PersonalFinance.Domain.Enums;

namespace PersonalFinance.Domain.Entities;

/// <summary>
/// MCP (Model Context Protocol) Server Configuration
/// Stores configuration for connecting to MCP servers for real-time data
/// </summary>
public class MCPServerConfiguration : BaseEntity
{
    public Guid UserId { get; set; }
    
    /// <summary>
    /// User-friendly name for the configuration
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// MCP provider type
    /// </summary>
    public MCPProviderType ProviderType { get; set; }
    
    /// <summary>
    /// API endpoint URL
    /// </summary>
    public string ApiEndpoint { get; set; } = string.Empty;
    
    /// <summary>
    /// API key for authentication
    /// </summary>
    public string? ApiKey { get; set; }
    
    /// <summary>
    /// Connection status
    /// </summary>
    public MCPConnectionStatus ConnectionStatus { get; set; } = MCPConnectionStatus.Disconnected;
    
    /// <summary>
    /// Whether this configuration is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Whether this is the default MCP server
    /// </summary>
    public bool IsDefault { get; set; } = false;
    
    /// <summary>
    /// Priority when multiple servers are configured (higher = more priority)
    /// </summary>
    public int Priority { get; set; } = 0;
    
    /// <summary>
    /// Rate limit - requests per minute
    /// </summary>
    public int? RequestsPerMinute { get; set; }
    
    /// <summary>
    /// Rate limit - requests per day
    /// </summary>
    public int? RequestsPerDay { get; set; }
    
    /// <summary>
    /// Last successful connection timestamp
    /// </summary>
    public DateTime? LastConnectedAt { get; set; }
    
    /// <summary>
    /// Last error message if connection failed
    /// </summary>
    public string? LastErrorMessage { get; set; }
    
    /// <summary>
    /// Data types supported by this server
    /// Stored as comma-separated values of MCPDataType enum
    /// </summary>
    public string? SupportedDataTypes { get; set; }
    
    // Navigation properties
    public User User { get; set; } = null!;
}
