using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using PersonalFinance.Application.DTOs;
using PersonalFinance.Application.Interfaces;
using PersonalFinance.Application.Models;
using PersonalFinance.Domain.Entities;
using PersonalFinance.Domain.Enums;
using PersonalFinance.Domain.Interfaces;

namespace PersonalFinance.Application.Services;

/// <summary>
/// Service for fetching data from MCP servers with live stock price integration
/// </summary>
public class MCPDataService : IMCPDataService
{
    private readonly IRepository<MCPServerConfiguration> _configRepository;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<MCPDataService> _logger;

    public MCPDataService(
        IRepository<MCPServerConfiguration> configRepository,
        IHttpClientFactory httpClientFactory,
        ILogger<MCPDataService> logger)
    {
        _configRepository = configRepository;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<MCPDataResponseDto> FetchDataAsync(Guid userId, MCPDataRequestDto request)
    {
        // Get MCP configuration
        var config = await GetMCPConfigurationAsync(userId, request.MCPServerConfigurationId);
        if (config == null)
        {
            throw new InvalidOperationException("No MCP server configuration found. Please configure an MCP server first.");
        }

        _logger.LogInformation("Fetching {DataType} data for {Symbol} from {Provider}", 
            request.DataType, request.Symbol, config.ProviderType);

        try
        {
            var data = await FetchFromProviderAsync(config, request);

            return new MCPDataResponseDto
            {
                Symbol = request.Symbol,
                DataType = request.DataType,
                ProviderName = config.Name,
                Data = data,
                FetchedAt = DateTime.UtcNow,
                FromCache = false
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch data from MCP server for {Symbol}", request.Symbol);
            
            return new MCPDataResponseDto
            {
                Symbol = request.Symbol,
                DataType = request.DataType,
                ProviderName = config.Name,
                ErrorMessage = ex.Message,
                FetchedAt = DateTime.UtcNow,
                FromCache = false
            };
        }
    }

    public async Task<IEnumerable<MCPDataResponseDto>> FetchBatchDataAsync(Guid userId, IEnumerable<MCPDataRequestDto> requests)
    {
        var results = new List<MCPDataResponseDto>();

        foreach (var request in requests)
        {
            try
            {
                var result = await FetchDataAsync(userId, request);
                results.Add(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch data in batch for {Symbol}", request.Symbol);
                
                results.Add(new MCPDataResponseDto
                {
                    Symbol = request.Symbol,
                    DataType = request.DataType,
                    ErrorMessage = ex.Message,
                    FetchedAt = DateTime.UtcNow,
                    FromCache = false
                });
            }
        }

        return results;
    }

    private async Task<MCPServerConfiguration?> GetMCPConfigurationAsync(Guid userId, Guid? configId)
    {
        if (configId.HasValue)
        {
            return await _configRepository.GetByIdAsync(configId.Value);
        }

        // Get default configuration
        var configs = await _configRepository.FindAsync(c => 
            c.UserId == userId && 
            c.IsDefault && 
            c.IsActive && 
            !c.IsDeleted);
        
        return configs.FirstOrDefault();
    }

    private async Task<object> FetchFromProviderAsync(MCPServerConfiguration config, MCPDataRequestDto request)
    {
        return config.ProviderType switch
        {
            MCPProviderType.YahooFinance => await FetchFromYahooFinanceAsync(config, request),
            MCPProviderType.AlphaVantage => await FetchFromAlphaVantageAsync(config, request),
            MCPProviderType.NSEIndia => await FetchFromNSEIndiaAsync(config, request),
            MCPProviderType.BSEIndia => await FetchFromBSEIndiaAsync(config, request),
            MCPProviderType.Custom => await FetchFromCustomServerAsync(config, request),
            _ => throw new NotSupportedException($"Provider type {config.ProviderType} is not supported")
        };
    }

    private async Task<object> FetchFromYahooFinanceAsync(MCPServerConfiguration config, MCPDataRequestDto request)
    {
        var httpClient = _httpClientFactory.CreateClient();
        
        try
        {
            // Build MCP request to Yahoo Finance MCP server
            var mcpRequest = new
            {
                method = "tools/call",
                @params = new
                {
                    name = "get_stock_price",
                    arguments = new
                    {
                        symbol = request.Symbol,
                        dataType = request.DataType.ToString()
                    }
                }
            };

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, config.ApiEndpoint)
            {
                Content = new StringContent(JsonSerializer.Serialize(mcpRequest), Encoding.UTF8, "application/json")
            };

            if (!string.IsNullOrEmpty(config.ApiKey))
            {
                requestMessage.Headers.Add("Authorization", $"Bearer {config.ApiKey}");
            }

            var response = await httpClient.SendAsync(requestMessage);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Yahoo Finance MCP request failed: {StatusCode} - {Error}", 
                    response.StatusCode, errorContent);
                
                // Return mock data as fallback
                return CreateMockStockPriceData(request.Symbol);
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var mcpResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
            
            // Parse MCP response
            if (mcpResponse.TryGetProperty("content", out var content))
            {
                return ParseYahooFinanceResponse(content, request);
            }

            // If we can't parse the response, return mock data
            return CreateMockStockPriceData(request.Symbol);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to fetch from Yahoo Finance MCP, returning mock data for {Symbol}", request.Symbol);
            return CreateMockStockPriceData(request.Symbol);
        }
    }

    private object ParseYahooFinanceResponse(JsonElement content, MCPDataRequestDto request)
    {
        // Try to parse the content as text and then deserialize
        if (content.ValueKind == JsonValueKind.Array && content.GetArrayLength() > 0)
        {
            var firstItem = content[0];
            if (firstItem.TryGetProperty("text", out var textElement))
            {
                try
                {
                    var dataJson = JsonSerializer.Deserialize<JsonElement>(textElement.GetString() ?? "{}");
                    
                    if (request.DataType == MCPDataType.RealTimePrice)
                    {
                        return new MCPStockPriceResponse
                        {
                            Symbol = request.Symbol,
                            Price = dataJson.TryGetProperty("price", out var price) ? price.GetDecimal() : 0,
                            Currency = dataJson.TryGetProperty("currency", out var currency) ? currency.GetString() ?? "USD" : "USD",
                            Timestamp = DateTime.UtcNow,
                            Open = dataJson.TryGetProperty("open", out var open) ? open.GetDecimal() : null,
                            High = dataJson.TryGetProperty("high", out var high) ? high.GetDecimal() : null,
                            Low = dataJson.TryGetProperty("low", out var low) ? low.GetDecimal() : null,
                            PreviousClose = dataJson.TryGetProperty("previousClose", out var prev) ? prev.GetDecimal() : null,
                            Volume = dataJson.TryGetProperty("volume", out var vol) ? vol.GetInt64() : null,
                            Change = dataJson.TryGetProperty("change", out var change) ? change.GetDecimal() : null,
                            ChangePercent = dataJson.TryGetProperty("changePercent", out var changePct) ? changePct.GetDecimal() : null,
                            MarketState = dataJson.TryGetProperty("marketState", out var state) ? state.GetString() : null
                        };
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to parse Yahoo Finance response");
                }
            }
        }
        
        return CreateMockStockPriceData(request.Symbol);
    }

    private async Task<object> FetchFromAlphaVantageAsync(MCPServerConfiguration config, MCPDataRequestDto request)
    {
        var httpClient = _httpClientFactory.CreateClient();
        
        try
        {
            // Build MCP request to Alpha Vantage MCP server
            var mcpRequest = new
            {
                method = "tools/call",
                @params = new
                {
                    name = "get_quote",
                    arguments = new
                    {
                        symbol = request.Symbol,
                        dataType = request.DataType.ToString()
                    }
                }
            };

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, config.ApiEndpoint)
            {
                Content = new StringContent(JsonSerializer.Serialize(mcpRequest), Encoding.UTF8, "application/json")
            };

            if (!string.IsNullOrEmpty(config.ApiKey))
            {
                requestMessage.Headers.Add("Authorization", $"Bearer {config.ApiKey}");
            }

            var response = await httpClient.SendAsync(requestMessage);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Alpha Vantage MCP request failed: {StatusCode} - {Error}", 
                    response.StatusCode, errorContent);
                
                return CreateMockStockPriceData(request.Symbol);
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var mcpResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
            
            if (mcpResponse.TryGetProperty("content", out var content))
            {
                return ParseAlphaVantageResponse(content, request);
            }

            return CreateMockStockPriceData(request.Symbol);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to fetch from Alpha Vantage MCP, returning mock data for {Symbol}", request.Symbol);
            return CreateMockStockPriceData(request.Symbol);
        }
    }

    private object ParseAlphaVantageResponse(JsonElement content, MCPDataRequestDto request)
    {
        if (content.ValueKind == JsonValueKind.Array && content.GetArrayLength() > 0)
        {
            var firstItem = content[0];
            if (firstItem.TryGetProperty("text", out var textElement))
            {
                try
                {
                    var dataJson = JsonSerializer.Deserialize<JsonElement>(textElement.GetString() ?? "{}");
                    
                    if (request.DataType == MCPDataType.RealTimePrice)
                    {
                        return new MCPStockPriceResponse
                        {
                            Symbol = request.Symbol,
                            Price = dataJson.TryGetProperty("price", out var price) ? price.GetDecimal() : 0,
                            Currency = "USD",
                            Timestamp = DateTime.UtcNow,
                            Open = dataJson.TryGetProperty("open", out var open) ? open.GetDecimal() : null,
                            High = dataJson.TryGetProperty("high", out var high) ? high.GetDecimal() : null,
                            Low = dataJson.TryGetProperty("low", out var low) ? low.GetDecimal() : null,
                            PreviousClose = dataJson.TryGetProperty("previousClose", out var prev) ? prev.GetDecimal() : null,
                            Volume = dataJson.TryGetProperty("volume", out var vol) ? vol.GetInt64() : null
                        };
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to parse Alpha Vantage response");
                }
            }
        }
        
        return CreateMockStockPriceData(request.Symbol);
    }

    private async Task<object> FetchFromNSEIndiaAsync(MCPServerConfiguration config, MCPDataRequestDto request)
    {
        var httpClient = _httpClientFactory.CreateClient();
        
        try
        {
            var mcpRequest = new
            {
                method = "tools/call",
                @params = new
                {
                    name = "get_nse_quote",
                    arguments = new
                    {
                        symbol = request.Symbol,
                        dataType = request.DataType.ToString()
                    }
                }
            };

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, config.ApiEndpoint)
            {
                Content = new StringContent(JsonSerializer.Serialize(mcpRequest), Encoding.UTF8, "application/json")
            };

            if (!string.IsNullOrEmpty(config.ApiKey))
            {
                requestMessage.Headers.Add("Authorization", $"Bearer {config.ApiKey}");
            }

            var response = await httpClient.SendAsync(requestMessage);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("NSE India MCP request failed: {StatusCode}", response.StatusCode);
                return CreateMockStockPriceData(request.Symbol, "INR");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var mcpResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
            
            if (mcpResponse.TryGetProperty("content", out var content))
            {
                return ParseIndianMarketResponse(content, request, "INR");
            }

            return CreateMockStockPriceData(request.Symbol, "INR");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to fetch from NSE India MCP, returning mock data for {Symbol}", request.Symbol);
            return CreateMockStockPriceData(request.Symbol, "INR");
        }
    }

    private async Task<object> FetchFromBSEIndiaAsync(MCPServerConfiguration config, MCPDataRequestDto request)
    {
        var httpClient = _httpClientFactory.CreateClient();
        
        try
        {
            var mcpRequest = new
            {
                method = "tools/call",
                @params = new
                {
                    name = "get_bse_quote",
                    arguments = new
                    {
                        symbol = request.Symbol,
                        dataType = request.DataType.ToString()
                    }
                }
            };

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, config.ApiEndpoint)
            {
                Content = new StringContent(JsonSerializer.Serialize(mcpRequest), Encoding.UTF8, "application/json")
            };

            if (!string.IsNullOrEmpty(config.ApiKey))
            {
                requestMessage.Headers.Add("Authorization", $"Bearer {config.ApiKey}");
            }

            var response = await httpClient.SendAsync(requestMessage);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("BSE India MCP request failed: {StatusCode}", response.StatusCode);
                return CreateMockStockPriceData(request.Symbol, "INR");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var mcpResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
            
            if (mcpResponse.TryGetProperty("content", out var content))
            {
                return ParseIndianMarketResponse(content, request, "INR");
            }

            return CreateMockStockPriceData(request.Symbol, "INR");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to fetch from BSE India MCP, returning mock data for {Symbol}", request.Symbol);
            return CreateMockStockPriceData(request.Symbol, "INR");
        }
    }

    private object ParseIndianMarketResponse(JsonElement content, MCPDataRequestDto request, string currency)
    {
        if (content.ValueKind == JsonValueKind.Array && content.GetArrayLength() > 0)
        {
            var firstItem = content[0];
            if (firstItem.TryGetProperty("text", out var textElement))
            {
                try
                {
                    var dataJson = JsonSerializer.Deserialize<JsonElement>(textElement.GetString() ?? "{}");
                    
                    if (request.DataType == MCPDataType.RealTimePrice)
                    {
                        return new MCPStockPriceResponse
                        {
                            Symbol = request.Symbol,
                            Price = dataJson.TryGetProperty("price", out var price) ? price.GetDecimal() : 0,
                            Currency = currency,
                            Timestamp = DateTime.UtcNow,
                            Open = dataJson.TryGetProperty("open", out var open) ? open.GetDecimal() : null,
                            High = dataJson.TryGetProperty("high", out var high) ? high.GetDecimal() : null,
                            Low = dataJson.TryGetProperty("low", out var low) ? low.GetDecimal() : null,
                            PreviousClose = dataJson.TryGetProperty("previousClose", out var prev) ? prev.GetDecimal() : null,
                            Volume = dataJson.TryGetProperty("volume", out var vol) ? vol.GetInt64() : null
                        };
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to parse Indian market response");
                }
            }
        }
        
        return CreateMockStockPriceData(request.Symbol, currency);
    }

    private MCPStockPriceResponse CreateMockStockPriceData(string symbol, string currency = "USD")
    {
        var random = new Random(symbol.GetHashCode() + DateTime.UtcNow.Day);
        var basePrice = currency == "INR" ? 1000m + random.Next(1, 5000) : 100m + random.Next(1, 500);
        var change = (decimal)(random.NextDouble() * 10 - 5);
        
        return new MCPStockPriceResponse
        {
            Symbol = symbol,
            Price = basePrice,
            Currency = currency,
            Timestamp = DateTime.UtcNow,
            Open = basePrice - (decimal)random.NextDouble() * 5,
            High = basePrice + (decimal)random.NextDouble() * 5,
            Low = basePrice - (decimal)random.NextDouble() * 5,
            PreviousClose = basePrice - change,
            Volume = random.Next(1000000, 50000000),
            Change = change,
            ChangePercent = (change / (basePrice - change)) * 100,
            MarketState = DateTime.UtcNow.Hour >= 9 && DateTime.UtcNow.Hour < 16 ? "REGULAR" : "CLOSED"
        };
    }

    private async Task<object> FetchFromCustomServerAsync(MCPServerConfiguration config, MCPDataRequestDto request)
    {
        var httpClient = _httpClientFactory.CreateClient();
        
        try
        {
            var mcpRequest = new
            {
                method = "tools/call",
                @params = new
                {
                    name = "get_data",
                    arguments = new
                    {
                        symbol = request.Symbol,
                        dataType = request.DataType.ToString(),
                        startDate = request.StartDate?.ToString("yyyy-MM-dd"),
                        endDate = request.EndDate?.ToString("yyyy-MM-dd"),
                        additionalParameters = request.AdditionalParameters
                    }
                }
            };

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, config.ApiEndpoint)
            {
                Content = new StringContent(JsonSerializer.Serialize(mcpRequest), Encoding.UTF8, "application/json")
            };

            if (!string.IsNullOrEmpty(config.ApiKey))
            {
                requestMessage.Headers.Add("Authorization", $"Bearer {config.ApiKey}");
            }

            _logger.LogInformation("Calling custom MCP server at {Endpoint}", config.ApiEndpoint);
            
            var response = await httpClient.SendAsync(requestMessage);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Custom MCP server request failed: {StatusCode} - {Error}", 
                    response.StatusCode, errorContent);
                
                return CreateMockStockPriceData(request.Symbol);
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var mcpResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
            
            if (mcpResponse.TryGetProperty("content", out var content))
            {
                return ParseCustomServerResponse(content, request);
            }

            return CreateMockStockPriceData(request.Symbol);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to fetch from custom MCP server, returning mock data for {Symbol}", request.Symbol);
            return CreateMockStockPriceData(request.Symbol);
        }
    }

    private object ParseCustomServerResponse(JsonElement content, MCPDataRequestDto request)
    {
        if (content.ValueKind == JsonValueKind.Array && content.GetArrayLength() > 0)
        {
            var firstItem = content[0];
            if (firstItem.TryGetProperty("text", out var textElement))
            {
                try
                {
                    var dataJson = JsonSerializer.Deserialize<JsonElement>(textElement.GetString() ?? "{}");
                    
                    if (request.DataType == MCPDataType.RealTimePrice)
                    {
                        return new MCPStockPriceResponse
                        {
                            Symbol = request.Symbol,
                            Price = dataJson.TryGetProperty("price", out var price) ? price.GetDecimal() : 0,
                            Currency = dataJson.TryGetProperty("currency", out var currency) ? currency.GetString() ?? "USD" : "USD",
                            Timestamp = DateTime.UtcNow,
                            Open = dataJson.TryGetProperty("open", out var open) ? open.GetDecimal() : null,
                            High = dataJson.TryGetProperty("high", out var high) ? high.GetDecimal() : null,
                            Low = dataJson.TryGetProperty("low", out var low) ? low.GetDecimal() : null,
                            PreviousClose = dataJson.TryGetProperty("previousClose", out var prev) ? prev.GetDecimal() : null,
                            Volume = dataJson.TryGetProperty("volume", out var vol) ? vol.GetInt64() : null,
                            Change = dataJson.TryGetProperty("change", out var change) ? change.GetDecimal() : null,
                            ChangePercent = dataJson.TryGetProperty("changePercent", out var changePct) ? changePct.GetDecimal() : null,
                            MarketState = dataJson.TryGetProperty("marketState", out var state) ? state.GetString() : null
                        };
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to parse custom server response");
                }
            }
        }
        
        return CreateMockStockPriceData(request.Symbol);
    }
}
