# MCP Server Integration Guide

## Overview

The Personal Finance application includes foundation support for **MCP (Model Context Protocol)** server integration. This feature enables real-time market data fetching from various financial data providers through a standardized protocol.

## Current Status

🟡 **Foundation Laid - Awaiting Full Implementation**

The infrastructure for MCP server integration has been implemented, including:
- Entity models and DTOs
- Service interfaces and basic implementations
- API controllers and endpoints
- Configuration management
- Connection testing framework

**What's Working:**
- ✅ MCP server configuration CRUD operations
- ✅ Connection testing framework
- ✅ Data fetching structure
- ✅ Batch operations support
- ✅ Rate limiting configuration

**What's Pending:**
- ⏳ Actual MCP protocol implementation
- ⏳ Provider-specific integrations (Yahoo Finance, Alpha Vantage, etc.)
- ⏳ Real-time data streaming
- ⏳ Caching layer for fetched data
- ⏳ Webhook support for price alerts

## Supported Providers

### Provider Types

1. **Yahoo Finance MCP** (`ProviderType: 0`)
   - Real-time stock prices
   - Historical data
   - Company fundamentals
   - Market news

2. **Alpha Vantage MCP** (`ProviderType: 1`)
   - Real-time and historical prices
   - Technical indicators
   - Fundamental data
   - Forex and crypto data

3. **NSE India MCP** (`ProviderType: 2`)
   - Indian stock market data
   - Real-time prices
   - Historical data
   - Corporate actions

4. **BSE India MCP** (`ProviderType: 3`)
   - Bombay Stock Exchange data
   - Real-time prices
   - Historical data

5. **Custom MCP** (`ProviderType: 99`)
   - Custom MCP server endpoint
   - Flexible configuration

## Data Types

The MCP integration supports various data types:

- `RealTimePrice` (0) - Current stock prices
- `HistoricalPrices` (1) - Historical price data
- `Fundamentals` (2) - Company fundamental data
- `News` (3) - Market news and updates
- `Financials` (4) - Financial statements
- `TechnicalIndicators` (5) - Technical analysis data

## Configuration

### Creating an MCP Server Configuration

**Endpoint:** `POST /api/mcpserver`

**Request Body:**
```json
{
  "name": "Yahoo Finance MCP",
  "providerType": 0,
  "apiEndpoint": "http://localhost:8080/mcp",
  "apiKey": "your-api-key-here",
  "isDefault": true,
  "priority": 10,
  "requestsPerMinute": 60,
  "requestsPerDay": 10000,
  "supportedDataTypes": [0, 1, 2, 3]
}
```

**Response:**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "userId": "user-id",
  "name": "Yahoo Finance MCP",
  "providerType": 0,
  "apiEndpoint": "http://localhost:8080/mcp",
  "connectionStatus": 0,
  "isActive": true,
  "isDefault": true,
  "priority": 10,
  "requestsPerMinute": 60,
  "requestsPerDay": 10000,
  "supportedDataTypes": [0, 1, 2, 3],
  "createdAt": "2024-01-01T00:00:00Z"
}
```

### Testing Connection

**Endpoint:** `POST /api/mcpserver/{id}/test`

**Response:**
```json
{
  "configurationId": "550e8400-e29b-41d4-a716-446655440000",
  "isConnected": true,
  "status": "Connected successfully",
  "testedAt": "2024-01-01T00:00:00Z",
  "responseTime": "00:00:00.250"
}
```

## Fetching Data

### Single Data Request

**Endpoint:** `POST /api/mcpserver/fetch`

**Request Body:**
```json
{
  "symbol": "AAPL",
  "dataType": 0,
  "mcpServerConfigurationId": "550e8400-e29b-41d4-a716-446655440000"
}
```

**Response:**
```json
{
  "symbol": "AAPL",
  "dataType": 0,
  "providerName": "Yahoo Finance MCP",
  "data": {
    "price": 175.50,
    "currency": "USD",
    "timestamp": "2024-01-01T15:30:00Z"
  },
  "fetchedAt": "2024-01-01T15:30:01Z",
  "fromCache": false
}
```

### Batch Data Request

**Endpoint:** `POST /api/mcpserver/fetch/batch`

**Request Body:**
```json
[
  {
    "symbol": "AAPL",
    "dataType": 0
  },
  {
    "symbol": "GOOGL",
    "dataType": 0
  },
  {
    "symbol": "MSFT",
    "dataType": 0
  }
]
```

**Response:**
```json
[
  {
    "symbol": "AAPL",
    "dataType": 0,
    "providerName": "Yahoo Finance MCP",
    "data": { "price": 175.50 },
    "fetchedAt": "2024-01-01T15:30:01Z",
    "fromCache": false
  },
  {
    "symbol": "GOOGL",
    "dataType": 0,
    "providerName": "Yahoo Finance MCP",
    "data": { "price": 142.30 },
    "fetchedAt": "2024-01-01T15:30:01Z",
    "fromCache": false
  }
]
```

## Rate Limiting

MCP server configurations support rate limiting to prevent API quota exhaustion:

- **RequestsPerMinute**: Maximum requests allowed per minute
- **RequestsPerDay**: Maximum requests allowed per day

When limits are reached, the connection status will change to `RateLimited` (3).

## Connection Status

MCP servers can have the following connection statuses:

- `Disconnected` (0) - Not connected
- `Connected` (1) - Connected and active
- `Error` (2) - Connection error occurred
- `RateLimited` (3) - Rate limit exceeded

## Best Practices

### 1. Configuration Management
- Set one MCP server as default for automatic selection
- Use priority values to control fallback order
- Test connection after creating a configuration

### 2. Data Fetching
- Use batch requests for multiple symbols to reduce overhead
- Implement caching on the client side when possible
- Monitor rate limits to avoid service disruption

### 3. Error Handling
- Always check the `errorMessage` field in responses
- Implement retry logic with exponential backoff
- Monitor connection status regularly

### 4. Security
- Store API keys securely
- Use HTTPS endpoints for MCP servers
- Rotate API keys periodically

## Implementation Roadmap

### Phase 1: Foundation (✅ Complete)
- [x] Entity models and enums
- [x] DTOs for requests and responses
- [x] Service interfaces
- [x] Basic service implementations
- [x] API controllers
- [x] Configuration management

### Phase 2: Protocol Integration (⏳ Pending)
- [ ] Implement actual MCP protocol communication
- [ ] Provider-specific adapters
- [ ] WebSocket support for real-time data
- [ ] Data caching layer
- [ ] Error recovery mechanisms

### Phase 3: Advanced Features (📋 Planned)
- [ ] Price alerts and webhooks
- [ ] Historical data synchronization
- [ ] Technical indicator calculations
- [ ] News aggregation
- [ ] Portfolio price updates

## Example Use Cases

### 1. Real-Time Portfolio Valuation
Fetch current prices for all stocks in a portfolio to calculate real-time portfolio value.

### 2. Historical Analysis
Retrieve historical price data to perform trend analysis and backtesting.

### 3. News Integration
Fetch latest news for stocks in the portfolio to stay informed about market events.

### 4. Technical Analysis
Get technical indicators (RSI, MACD, etc.) to support trading decisions.

## API Reference

### Endpoints Summary

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/mcpserver` | Get all configurations |
| POST | `/api/mcpserver` | Create configuration |
| GET | `/api/mcpserver/{id}` | Get configuration by ID |
| GET | `/api/mcpserver/default` | Get default configuration |
| PUT | `/api/mcpserver/{id}` | Update configuration |
| DELETE | `/api/mcpserver/{id}` | Delete configuration |
| POST | `/api/mcpserver/{id}/test` | Test connection |
| POST | `/api/mcpserver/fetch` | Fetch data |
| POST | `/api/mcpserver/fetch/batch` | Fetch batch data |

## Troubleshooting

### Connection Test Fails
1. Verify API endpoint is correct
2. Check API key is valid
3. Ensure MCP server is running
4. Check firewall/network settings

### Rate Limit Errors
1. Review `RequestsPerMinute` and `RequestsPerDay` settings
2. Implement request throttling
3. Consider upgrading API plan

### Data Not Returned
1. Check if symbol is valid
2. Verify supported data types for provider
3. Review error message in response

## Future Enhancements

- **Caching Layer**: Implement Redis-based caching for frequently accessed data
- **WebSocket Support**: Real-time price streaming
- **Webhook Integration**: Push notifications for price alerts
- **Multi-Provider Failover**: Automatic failover to backup providers
- **Data Validation**: Enhanced validation of fetched data
- **Analytics**: Usage tracking and performance metrics

## Support

For issues or questions:
1. Check the troubleshooting section
2. Review API documentation
3. Contact support team

## License

MIT License (same as main project)
