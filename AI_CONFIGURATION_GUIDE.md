# AI Stock Research Assistant - Configuration Guide

## Overview

The Personal Finance application now includes an AI-powered stock research assistant that can analyze stocks using multiple AI providers including OpenAI, Anthropic Claude, Google Gemini, and local Ollama models.

## Features

### AI Analysis Types

1. **Quick Overview** - Brief summary of the stock (200 words or less)
2. **Fundamental Analysis** - Deep dive into company financials, revenue, profitability, debt levels
3. **Technical Analysis** - Price trends, indicators (RSI, MACD), chart patterns
4. **Sentiment Analysis** - Market sentiment, news analysis, analyst ratings
5. **Valuation** - P/E ratio, DCF analysis, fair value estimation
6. **Risk Assessment** - Business risks, financial risks, competitive risks
7. **Comprehensive Analysis** - All-in-one detailed analysis

### Key Capabilities

- **Multi-Provider Support**: Configure multiple AI providers simultaneously
- **Intelligent Caching**: Avoid redundant API calls and save costs
- **Flexible Configuration**: Customize temperature, max tokens, and other model parameters
- **Analysis History**: Track all past analyses for reference
- **Quick Access**: One-click AI research from stock holdings

## Configuration

### Step 1: Choose Your AI Provider

You can use one or more of the following providers:

#### OpenAI (GPT-4, GPT-3.5)
- Sign up at https://platform.openai.com/
- Generate an API key
- Cost: Pay-per-token pricing
- Models: GPT-4 (most capable), GPT-4-Turbo, GPT-3.5-Turbo (faster, cheaper)

#### Anthropic (Claude 3)
- Sign up at https://console.anthropic.com/
- Generate an API key
- Cost: Pay-per-token pricing
- Models: Claude 3 Opus (most capable), Sonnet (balanced), Haiku (fastest)

#### Google Gemini
- Sign up at https://ai.google.dev/
- Generate an API key
- Cost: Free tier available, then pay-per-token
- Models: Gemini Pro, Gemini Ultra

#### Ollama (Local/Self-hosted)
- Install Ollama from https://ollama.ai/
- Run models locally (no API key needed)
- Cost: Free (uses local compute)
- Models: Llama 2, Llama 3, Mistral, and many others

### Step 2: Configure AI Model via API

Use the `/api/aimodelconfigurations` endpoint to configure your AI provider.

#### Example: Configure OpenAI

```bash
curl -X POST https://your-api-url/api/aimodelconfigurations \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "My OpenAI GPT-4",
    "providerType": 0,
    "modelType": 1,
    "apiKey": "sk-your-openai-api-key",
    "isDefault": true,
    "priority": 1,
    "temperature": 0.7,
    "maxTokens": 2000
  }'
```

#### Example: Configure Anthropic Claude

```bash
curl -X POST https://your-api-url/api/aimodelconfigurations \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Claude 3 Sonnet",
    "providerType": 1,
    "modelType": 4,
    "apiKey": "sk-ant-your-anthropic-api-key",
    "isDefault": false,
    "priority": 2,
    "temperature": 0.7,
    "maxTokens": 1500
  }'
```

#### Example: Configure Ollama (Local)

```bash
curl -X POST https://your-api-url/api/aimodelconfigurations \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Local Llama 3",
    "providerType": 3,
    "modelType": 9,
    "apiKey": "not-required",
    "apiEndpoint": "http://localhost:11434",
    "isDefault": false,
    "priority": 3,
    "temperature": 0.7,
    "maxTokens": 1500
  }'
```

### Provider Type Enum Values

- 0: OpenAI
- 1: Anthropic
- 2: GoogleGemini
- 3: Ollama
- 4: Custom

### Model Type Enum Values

- 0: GPT4
- 1: GPT4Turbo
- 2: GPT35Turbo
- 3: Claude3Opus
- 4: Claude3Sonnet
- 5: Claude3Haiku
- 6: GeminiPro
- 7: GeminiUltra
- 8: Llama2
- 9: Llama3
- 10: Mistral
- 11: Custom

## Using the AI Stock Research Assistant

### From the Frontend (Angular)

1. Navigate to your stock holdings
2. Click the "🧠" (brain/psychology) icon next to any stock
3. Select the type of analysis you want
4. Click "Analyze"
5. Wait for the AI to generate insights

### Via API

#### Get Quick Overview

```bash
curl https://your-api-url/api/stockanalysis/quick-overview/AAPL \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

#### Create Custom Analysis

```bash
curl -X POST https://your-api-url/api/stockanalysis \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "symbol": "AAPL",
    "companyName": "Apple Inc.",
    "analysisType": 3,
    "useCache": true
  }'
```

#### Interactive Research (Ask Questions)

```bash
curl -X POST https://your-api-url/api/stockanalysis/research \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "symbol": "AAPL",
    "query": "What are the major risks facing Apple in 2024?"
  }'
```

## Best Practices

### Cost Optimization

1. **Enable Caching**: Set `useCache: true` to avoid re-analyzing the same stock
2. **Choose the Right Model**: 
   - Use GPT-3.5-Turbo or Claude Haiku for quick overviews
   - Reserve GPT-4 or Claude Opus for comprehensive analysis
3. **Set Token Limits**: Configure `maxTokens` appropriately to control costs
4. **Use Local Models**: For development, use Ollama with local models

### Performance Tips

1. **Set Default Provider**: Mark your most-used provider as default
2. **Configure Priorities**: Higher priority providers are used as fallback
3. **Cache Duration**: 
   - Quick Overview: 2 hours
   - Technical Analysis: 4 hours
   - Fundamental/Comprehensive: 24 hours

### Security

1. **API Key Storage**: API keys are encrypted in the database
2. **User Isolation**: Each user can only access their own configurations
3. **Key Validation**: API keys are validated before saving
4. **Rate Limiting**: Configure `requestsPerMinute` and `requestsPerDay`

## Advanced Configuration

### Custom Prompts

```bash
curl -X POST https://your-api-url/api/stockanalysis \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "symbol": "TSLA",
    "analysisType": 3,
    "customPrompt": "Analyze Tesla focusing on their AI and robotics initiatives. Include competitive analysis vs traditional automakers."
  }'
```

### Model Parameters

- **temperature** (0.0 - 1.0): Controls randomness
  - Lower (0.3-0.5): More focused, deterministic
  - Higher (0.7-0.9): More creative, varied
  
- **maxTokens**: Maximum response length
  - Quick analysis: 500-1000
  - Detailed analysis: 1500-2500
  
- **topP** (0.0 - 1.0): Nucleus sampling parameter
  - Default: 1.0
  - Lower values: More focused responses

## Troubleshooting

### "No AI model configuration found"
- Ensure you've configured at least one AI provider
- Check that the provider is marked as `isActive: true`
- Verify your API key is correct

### "Invalid API key"
- Double-check your API key from the provider's dashboard
- Ensure there are no extra spaces or characters
- Verify the API key has the correct permissions

### "Rate limit exceeded"
- Check your provider's usage dashboard
- Configure `requestsPerMinute` and `requestsPerDay` limits
- Consider using multiple providers for load balancing

### Slow Response Times
- Use faster models (GPT-3.5-Turbo, Claude Haiku)
- Reduce `maxTokens`
- Check your internet connection
- Consider using a local Ollama model

## Future Enhancements

Planned features for the AI Stock Research Assistant:

1. **MCP Server Integration**: Connect to external MCP servers for real-time data
2. **Batch Analysis**: Analyze multiple stocks at once
3. **Scheduled Analysis**: Automatic daily/weekly analysis of your portfolio
4. **AI Recommendations**: Buy/sell/hold recommendations with rationale
5. **Historical Tracking**: Track how AI recommendations perform over time
6. **Custom Training**: Fine-tune models on your investment preferences
7. **Multi-language Support**: Analysis in different languages
8. **Voice Assistant**: Voice-based stock queries

## API Reference

For complete API documentation, visit the Swagger UI at:
```
https://your-api-url/swagger
```

Navigate to:
- **AIModelConfigurations** controller for provider configuration
- **StockAnalysis** controller for analysis endpoints

## Support

For issues or questions:
1. Check the GitHub Issues page
2. Review the API logs for error details
3. Ensure your AI provider account has sufficient credits
4. Verify database migrations are up to date

## License

This feature is part of the Personal Finance Management System and follows the same MIT License.
