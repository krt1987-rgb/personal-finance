namespace PersonalFinance.Domain.Enums;

public enum AIProviderType
{
    OpenAI,
    Anthropic,
    GoogleGemini,
    Ollama,
    Custom
}

public enum AIModelType
{
    GPT4,
    GPT4Turbo,
    GPT35Turbo,
    Claude3Opus,
    Claude3Sonnet,
    Claude3Haiku,
    GeminiPro,
    GeminiUltra,
    Llama2,
    Llama3,
    Mistral,
    Custom
}

public enum StockAnalysisType
{
    Fundamental,
    Technical,
    Sentiment,
    Comprehensive,
    QuickOverview,
    RiskAssessment,
    Valuation
}

public enum AnalysisStatus
{
    Pending,
    InProgress,
    Completed,
    Failed,
    Cached
}
