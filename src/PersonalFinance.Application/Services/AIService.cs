using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using PersonalFinance.Application.Interfaces;
using PersonalFinance.Domain.Enums;

namespace PersonalFinance.Application.Services;

public class AIService : IAIService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<AIService> _logger;

    public AIService(IHttpClientFactory httpClientFactory, ILogger<AIService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<string> GenerateCompletionAsync(
        string prompt,
        AIProviderType providerType,
        AIModelType modelType,
        string apiKey,
        string? apiEndpoint = null,
        decimal? temperature = null,
        int? maxTokens = null)
    {
        return providerType switch
        {
            AIProviderType.OpenAI => await GenerateOpenAICompletionAsync(
                prompt, modelType, apiKey, apiEndpoint, temperature, maxTokens),
            AIProviderType.Anthropic => await GenerateAnthropicCompletionAsync(
                prompt, modelType, apiKey, apiEndpoint, temperature, maxTokens),
            AIProviderType.GoogleGemini => await GenerateGeminiCompletionAsync(
                prompt, modelType, apiKey, apiEndpoint, temperature, maxTokens),
            AIProviderType.Ollama => await GenerateOllamaCompletionAsync(
                prompt, modelType, apiEndpoint ?? "http://localhost:11434", temperature, maxTokens),
            _ => throw new NotSupportedException($"Provider {providerType} is not supported")
        };
    }

    public async Task<string> GenerateStockAnalysisAsync(
        string symbol,
        string companyName,
        StockAnalysisType analysisType,
        AIProviderType providerType,
        AIModelType modelType,
        string apiKey,
        string? apiEndpoint = null,
        string? customPrompt = null)
    {
        var prompt = customPrompt ?? BuildStockAnalysisPrompt(symbol, companyName, analysisType);
        return await GenerateCompletionAsync(
            prompt, providerType, modelType, apiKey, apiEndpoint, temperature: 0.7m, maxTokens: 2000);
    }

    public async Task<bool> ValidateApiKeyAsync(
        AIProviderType providerType,
        string apiKey,
        string? apiEndpoint = null)
    {
        try
        {
            var testPrompt = "Hello, this is a test.";
            
            // Select appropriate model type based on provider
            var modelType = providerType switch
            {
                AIProviderType.OpenAI => AIModelType.GPT35Turbo,
                AIProviderType.Anthropic => AIModelType.Claude3Haiku,
                AIProviderType.GoogleGemini => AIModelType.GeminiPro,
                AIProviderType.Ollama => AIModelType.Llama2,
                _ => AIModelType.GPT35Turbo
            };
            
            await GenerateCompletionAsync(
                testPrompt,
                providerType,
                modelType,
                apiKey,
                apiEndpoint,
                temperature: 0.5m,
                maxTokens: 10);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "API key validation failed for provider {Provider}", providerType);
            return false;
        }
    }

    private async Task<string> GenerateOpenAICompletionAsync(
        string prompt,
        AIModelType modelType,
        string apiKey,
        string? apiEndpoint,
        decimal? temperature,
        int? maxTokens)
    {
        var httpClient = _httpClientFactory.CreateClient();
        var endpoint = apiEndpoint ?? "https://api.openai.com/v1/chat/completions";
        
        var modelName = modelType switch
        {
            AIModelType.GPT4 => "gpt-4",
            AIModelType.GPT4Turbo => "gpt-4-turbo-preview",
            AIModelType.GPT35Turbo => "gpt-3.5-turbo",
            _ => "gpt-3.5-turbo"
        };

        var requestBody = new
        {
            model = modelName,
            messages = new[]
            {
                new { role = "system", content = "You are a financial analyst expert specializing in stock market analysis." },
                new { role = "user", content = prompt }
            },
            temperature = temperature ?? 0.7m,
            max_tokens = maxTokens ?? 1500
        };

        var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
        };
        request.Headers.Add("Authorization", $"Bearer {apiKey}");

        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(responseContent);
        
        return jsonDoc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString() ?? string.Empty;
    }

    private async Task<string> GenerateAnthropicCompletionAsync(
        string prompt,
        AIModelType modelType,
        string apiKey,
        string? apiEndpoint,
        decimal? temperature,
        int? maxTokens)
    {
        var httpClient = _httpClientFactory.CreateClient();
        var endpoint = apiEndpoint ?? "https://api.anthropic.com/v1/messages";
        
        var modelName = modelType switch
        {
            AIModelType.Claude3Opus => "claude-3-opus-20240229",
            AIModelType.Claude3Sonnet => "claude-3-sonnet-20240229",
            AIModelType.Claude3Haiku => "claude-3-haiku-20240307",
            _ => "claude-3-sonnet-20240229"
        };

        var requestBody = new
        {
            model = modelName,
            max_tokens = maxTokens ?? 1500,
            temperature = temperature ?? 0.7m,
            messages = new[]
            {
                new { role = "user", content = prompt }
            }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
        };
        request.Headers.Add("x-api-key", apiKey);
        request.Headers.Add("anthropic-version", "2023-06-01");

        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(responseContent);
        
        return jsonDoc.RootElement
            .GetProperty("content")[0]
            .GetProperty("text")
            .GetString() ?? string.Empty;
    }

    private async Task<string> GenerateGeminiCompletionAsync(
        string prompt,
        AIModelType modelType,
        string apiKey,
        string? apiEndpoint,
        decimal? temperature,
        int? maxTokens)
    {
        var httpClient = _httpClientFactory.CreateClient();
        var modelName = modelType switch
        {
            AIModelType.GeminiPro => "gemini-pro",
            AIModelType.GeminiUltra => "gemini-ultra",
            _ => "gemini-pro"
        };
        
        var endpoint = apiEndpoint ?? $"https://generativelanguage.googleapis.com/v1/models/{modelName}:generateContent?key={apiKey}";

        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            },
            generationConfig = new
            {
                temperature = temperature ?? 0.7m,
                maxOutputTokens = maxTokens ?? 1500
            }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
        };

        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(responseContent);
        
        return jsonDoc.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString() ?? string.Empty;
    }

    private async Task<string> GenerateOllamaCompletionAsync(
        string prompt,
        AIModelType modelType,
        string apiEndpoint,
        decimal? temperature,
        int? maxTokens)
    {
        var httpClient = _httpClientFactory.CreateClient();
        var endpoint = $"{apiEndpoint}/api/generate";
        
        var modelName = modelType switch
        {
            AIModelType.Llama2 => "llama2",
            AIModelType.Llama3 => "llama3",
            AIModelType.Mistral => "mistral",
            _ => "llama2"
        };

        var requestBody = new
        {
            model = modelName,
            prompt = prompt,
            stream = false,
            options = new
            {
                temperature = temperature ?? 0.7m,
                num_predict = maxTokens ?? 1500
            }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
        };

        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(responseContent);
        
        return jsonDoc.RootElement
            .GetProperty("response")
            .GetString() ?? string.Empty;
    }

    private string BuildStockAnalysisPrompt(string symbol, string companyName, StockAnalysisType analysisType)
    {
        var basePrompt = $"Analyze the stock {symbol} ({companyName}) focusing on ";
        
        return analysisType switch
        {
            StockAnalysisType.Fundamental => basePrompt + 
                "fundamental analysis. Include: revenue trends, profitability metrics, debt levels, competitive position, " +
                "management quality, and intrinsic value assessment. Provide actionable insights.",
            
            StockAnalysisType.Technical => basePrompt + 
                "technical analysis. Discuss: price trends, support/resistance levels, volume patterns, key indicators " +
                "(RSI, MACD, Moving Averages), chart patterns, and momentum. Provide trading insights.",
            
            StockAnalysisType.Sentiment => basePrompt + 
                "market sentiment and news analysis. Cover: recent news, analyst ratings, institutional holdings, " +
                "social media sentiment, and market perception. Assess overall sentiment.",
            
            StockAnalysisType.RiskAssessment => basePrompt + 
                "risk assessment. Evaluate: business risks, financial risks, market risks, regulatory risks, " +
                "competitive risks, and management risks. Provide a risk rating.",
            
            StockAnalysisType.Valuation => basePrompt + 
                "valuation analysis. Include: P/E ratio, PEG ratio, Price-to-Book, Price-to-Sales, DCF analysis, " +
                "comparables analysis, and fair value estimation. Determine if overvalued or undervalued.",
            
            StockAnalysisType.QuickOverview => basePrompt + 
                "a quick overview. Provide: current status, recent performance, key metrics, major catalysts, " +
                "and a brief recommendation in 200 words or less.",
            
            StockAnalysisType.Comprehensive => basePrompt + 
                "a comprehensive analysis covering fundamental, technical, and sentiment factors. " +
                "Include: company overview, financial health, competitive position, growth prospects, " +
                "valuation, risks, catalysts, and a detailed investment recommendation.",
            
            _ => basePrompt + "general stock analysis with key insights and recommendations."
        };
    }
}
