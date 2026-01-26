export interface AIModelConfiguration {
  id: string;
  userId: string;
  name: string;
  providerType: AIProviderType;
  modelType: AIModelType;
  customModelName?: string;
  apiEndpoint?: string;
  isActive: boolean;
  isDefault: boolean;
  priority: number;
  temperature?: number;
  maxTokens?: number;
  topP?: number;
  frequencyPenalty?: number;
  presencePenalty?: number;
  requestsPerMinute?: number;
  requestsPerDay?: number;
  createdAt: Date;
  updatedAt?: Date;
}

export interface CreateAIModelConfiguration {
  name: string;
  providerType: AIProviderType;
  modelType: AIModelType;
  customModelName?: string;
  apiKey: string;
  apiEndpoint?: string;
  isDefault?: boolean;
  priority?: number;
  temperature?: number;
  maxTokens?: number;
  topP?: number;
  frequencyPenalty?: number;
  presencePenalty?: number;
  requestsPerMinute?: number;
  requestsPerDay?: number;
}

export interface StockAnalysis {
  id: string;
  userId: string;
  stockHoldingId?: string;
  aiModelConfigurationId?: string;
  symbol: string;
  companyName: string;
  analysisType: StockAnalysisType;
  status: AnalysisStatus;
  analysisResult?: string;
  summary?: string;
  keyInsights?: string;
  risks?: string;
  recommendation?: string;
  confidenceScore?: number;
  tokensUsed?: number;
  analysisCost?: number;
  createdAt: Date;
  completedAt?: Date;
  errorMessage?: string;
  isCached?: boolean;
}

export interface CreateStockAnalysisRequest {
  symbol: string;
  companyName?: string;
  stockHoldingId?: string;
  analysisType: StockAnalysisType;
  aiModelConfigurationId?: string;
  customPrompt?: string;
  useCache?: boolean;
}

export interface StockResearchRequest {
  symbol: string;
  query: string;
  aiModelConfigurationId?: string;
}

export interface StockResearchResponse {
  symbol: string;
  query: string;
  response: string;
  modelUsed?: string;
  tokensUsed?: number;
  generatedAt: Date;
  fromCache: boolean;
}

export enum AIProviderType {
  OpenAI = 0,
  Anthropic = 1,
  GoogleGemini = 2,
  Ollama = 3,
  Custom = 4
}

export enum AIModelType {
  GPT4 = 0,
  GPT4Turbo = 1,
  GPT35Turbo = 2,
  Claude3Opus = 3,
  Claude3Sonnet = 4,
  Claude3Haiku = 5,
  GeminiPro = 6,
  GeminiUltra = 7,
  Llama2 = 8,
  Llama3 = 9,
  Mistral = 10,
  Custom = 11
}

export enum StockAnalysisType {
  Fundamental = 0,
  Technical = 1,
  Sentiment = 2,
  Comprehensive = 3,
  QuickOverview = 4,
  RiskAssessment = 5,
  Valuation = 6
}

export enum AnalysisStatus {
  Pending = 0,
  InProgress = 1,
  Completed = 2,
  Failed = 3,
  Cached = 4
}
