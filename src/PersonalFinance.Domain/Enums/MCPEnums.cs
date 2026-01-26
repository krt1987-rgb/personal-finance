namespace PersonalFinance.Domain.Enums;

/// <summary>
/// MCP (Model Context Protocol) server provider types
/// </summary>
public enum MCPProviderType
{
    /// <summary>
    /// Yahoo Finance MCP server for stock data
    /// </summary>
    YahooFinance = 0,
    
    /// <summary>
    /// Alpha Vantage MCP server
    /// </summary>
    AlphaVantage = 1,
    
    /// <summary>
    /// NSE India MCP server
    /// </summary>
    NSEIndia = 2,
    
    /// <summary>
    /// BSE India MCP server
    /// </summary>
    BSEIndia = 3,
    
    /// <summary>
    /// Custom MCP server
    /// </summary>
    Custom = 99
}

/// <summary>
/// MCP data types that can be fetched
/// </summary>
public enum MCPDataType
{
    /// <summary>
    /// Real-time stock price
    /// </summary>
    RealTimePrice = 0,
    
    /// <summary>
    /// Historical prices
    /// </summary>
    HistoricalPrices = 1,
    
    /// <summary>
    /// Company fundamentals
    /// </summary>
    Fundamentals = 2,
    
    /// <summary>
    /// Market news
    /// </summary>
    News = 3,
    
    /// <summary>
    /// Financial statements
    /// </summary>
    Financials = 4,
    
    /// <summary>
    /// Technical indicators
    /// </summary>
    TechnicalIndicators = 5
}

/// <summary>
/// Status of MCP server connection
/// </summary>
public enum MCPConnectionStatus
{
    /// <summary>
    /// Not connected
    /// </summary>
    Disconnected = 0,
    
    /// <summary>
    /// Connected and active
    /// </summary>
    Connected = 1,
    
    /// <summary>
    /// Connection error
    /// </summary>
    Error = 2,
    
    /// <summary>
    /// Rate limited
    /// </summary>
    RateLimited = 3
}
