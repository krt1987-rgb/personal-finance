using PersonalFinance.Domain.Enums;

namespace PersonalFinance.Application.Interfaces;

public interface IAIService
{
    Task<string> GenerateCompletionAsync(
        string prompt,
        AIProviderType providerType,
        AIModelType modelType,
        string apiKey,
        string? apiEndpoint = null,
        decimal? temperature = null,
        int? maxTokens = null);
    
    Task<string> GenerateStockAnalysisAsync(
        string symbol,
        string companyName,
        StockAnalysisType analysisType,
        AIProviderType providerType,
        AIModelType modelType,
        string apiKey,
        string? apiEndpoint = null,
        string? customPrompt = null);
    
    Task<bool> ValidateApiKeyAsync(
        AIProviderType providerType,
        string apiKey,
        string? apiEndpoint = null);
}
