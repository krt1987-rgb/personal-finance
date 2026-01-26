# Personal Finance Management System

An enterprise-grade personal finance management application built with .NET Core 10 API, PostgreSQL database, and Angular Material frontend.

## 🚀 Features

### Core Modules

1. **Family Management**
   - Manage family members and their relationships
   - Track dependents and their financial information
   - Family member profiles with contact details

2. **Provident Fund (PF) Management**
   - EPF, PPF, VPF, and NPS tracking
   - Employee and employer contribution tracking
   - Interest calculation and balance updates
   - Transaction history

3. **Bank Accounts & Fixed Deposits**
   - Multiple bank account management
   - Fixed deposit tracking with maturity calculations
   - Interest rate monitoring
   - Account balance tracking
   - Transaction history

4. **Stock Portfolio Management**
   - Real-time stock holdings tracking
   - Buy/Sell transaction management
   - Profit/Loss calculation
   - Portfolio performance metrics
   - **NEW: AI-Powered Stock Research Assistant** 🤖
     - Multi-provider AI support (OpenAI, Anthropic, Google Gemini, Ollama)
     - Multiple analysis types (Fundamental, Technical, Sentiment, etc.)
     - Intelligent caching to optimize costs
     - Interactive stock research with custom queries
   - Live price tracking integration (planned)

5. **Mutual Fund Management**
   - SIP and Lumpsum investment tracking
   - NAV-based valuation
   - Category-wise fund classification
   - Portfolio performance analysis

6. **Wealth Tracking & Reporting**
   - Consolidated wealth dashboard
   - Net worth calculation
   - Investment distribution analysis
   - Performance reports

### Technical Features

- **Clean Architecture**: Domain, Application, Infrastructure, and API layers
- **Entity Framework Core**: Code-first approach with PostgreSQL
- **JWT Authentication**: Secure API endpoints
- **Repository Pattern**: Abstraction layer for data access
- **Unit of Work Pattern**: Transaction management
- **Serilog**: Structured logging
- **Swagger/OpenAPI**: API documentation
- **Docker Support**: Containerized deployment
- **CORS**: Angular app integration ready

## 🏗️ Architecture

```
PersonalFinance/
├── src/
│   ├── PersonalFinance.Domain/          # Domain entities, enums, and interfaces
│   ├── PersonalFinance.Application/     # Business logic, DTOs, and services
│   ├── PersonalFinance.Infrastructure/  # Data access, repositories, and DbContext
│   └── PersonalFinance.API/            # Web API controllers and configuration
└── tests/
    ├── PersonalFinance.UnitTests/
    └── PersonalFinance.IntegrationTests/
```

## 🛠️ Technology Stack

### Backend
- **.NET 10**: Latest .NET framework
- **ASP.NET Core Web API**: RESTful API
- **Entity Framework Core 10**: ORM
- **PostgreSQL**: Primary database
- **JWT Bearer Authentication**: Security
- **Serilog**: Logging
- **Swagger/OpenAPI**: API documentation
- **FluentValidation**: Input validation
- **AutoMapper**: Object mapping

### Frontend (Planned)
- **Angular 18+**: Frontend framework
- **Angular Material**: UI components
- **RxJS**: Reactive programming
- **Chart.js**: Data visualization

### DevOps
- **Docker**: Containerization
- **Docker Compose**: Multi-container orchestration

## 📋 Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL 16+](https://www.postgresql.org/download/)
- [Docker & Docker Compose](https://www.docker.com/get-started) (optional)
- [Node.js 20+](https://nodejs.org/) (for Angular frontend)

## 🚀 Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/krt1987-rgb/personal-finance.git
cd personal-finance
```

### 2. Setup Database

#### Option A: Using Docker Compose (Recommended)

```bash
docker-compose up -d postgres
```

#### Option B: Manual PostgreSQL Setup

1. Install PostgreSQL
2. Create database:
```sql
CREATE DATABASE PersonalFinanceDb;
```

### 3. Update Connection String

Edit `src/PersonalFinance.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=PersonalFinanceDb;Username=your_username;Password=your_password"
  }
}
```

### 4. Run Database Migrations

```bash
cd src/PersonalFinance.API
dotnet ef migrations add InitialCreate --project ../PersonalFinance.Infrastructure
dotnet ef database update
```

### 5. Run the Application

#### Option A: Using Docker Compose

```bash
docker-compose up
```

#### Option B: Using .NET CLI

```bash
cd src/PersonalFinance.API
dotnet run
```

The API will be available at:
- HTTPS: `https://localhost:5001`
- HTTP: `http://localhost:5000`
- Swagger UI: `https://localhost:5001/swagger`

## 🤖 AI Stock Research Assistant

The application includes a powerful AI-driven stock research assistant that provides intelligent analysis and insights.

### Features
- **Multi-Provider Support**: OpenAI (GPT-4, GPT-3.5), Anthropic (Claude 3), Google Gemini, and local Ollama models
- **Multiple Analysis Types**: 
  - Quick Overview
  - Fundamental Analysis
  - Technical Analysis  
  - Sentiment Analysis
  - Valuation
  - Risk Assessment
  - Comprehensive Analysis
- **Smart Caching**: Reduces API costs by caching recent analyses
- **Interactive Research**: Ask custom questions about any stock
- **One-Click Access**: Research any stock directly from your portfolio
- **Batch Analysis**: Analyze multiple stocks at once with progress tracking
- **Confidence Scoring**: AI-powered confidence scores based on analysis quality
- **Analysis History**: Track and filter all past analyses with pagination

### Quick Start

1. **Configure an AI Provider** (see [AI Configuration Guide](AI_CONFIGURATION_GUIDE.md))
```bash
curl -X POST http://localhost:5000/api/aimodelconfigurations \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "OpenAI GPT-4",
    "providerType": 0,
    "modelType": 1,
    "apiKey": "your-api-key",
    "isDefault": true
  }'
```

2. **Analyze a Stock**
```bash
curl -X POST http://localhost:5000/api/stockanalysis \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "symbol": "AAPL",
    "analysisType": 3,
    "useCache": true
  }'
```

For detailed configuration and usage, see the [AI Configuration Guide](AI_CONFIGURATION_GUIDE.md).

## 🔌 MCP Server Integration (Foundation)

The application now includes foundation support for MCP (Model Context Protocol) server integration for real-time market data.

### Features
- **Multi-Provider Support**: Yahoo Finance, Alpha Vantage, NSE India, BSE India, Custom servers
- **Data Types**: Real-time prices, historical data, fundamentals, news, financials, technical indicators
- **Connection Testing**: Test MCP server connectivity before use
- **Batch Data Fetching**: Fetch data for multiple symbols at once
- **Rate Limiting**: Configure requests per minute/day

### Quick Start

1. **Configure an MCP Server**
```bash
curl -X POST http://localhost:5000/api/mcpserver \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Yahoo Finance MCP",
    "providerType": 0,
    "apiEndpoint": "http://mcp-server:8080",
    "apiKey": "your-api-key",
    "isDefault": true,
    "supportedDataTypes": [0, 1, 2]
  }'
```

2. **Test Connection**
```bash
curl -X POST http://localhost:5000/api/mcpserver/{id}/test \
  -H "Authorization: Bearer YOUR_TOKEN"
```

3. **Fetch Data**
```bash
curl -X POST http://localhost:5000/api/mcpserver/fetch \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "symbol": "AAPL",
    "dataType": 0
  }'
```

**Note**: MCP integration is in foundation stage. Actual protocol implementation pending.

## 📚 API Documentation

Once the application is running, visit the Swagger UI at `https://localhost:5001/swagger` for interactive API documentation.

### Key Endpoints

#### Stock Holdings
- `GET /api/stockholdings` - Get all stock holdings
- `POST /api/stockholdings` - Create new holding
- `GET /api/stockholdings/{id}` - Get specific holding
- `PUT /api/stockholdings/{id}` - Update holding
- `DELETE /api/stockholdings/{id}` - Delete holding
- `GET /api/stockholdings/portfolio-summary` - Get portfolio summary

#### Bank Accounts
- `GET /api/bankaccounts` - Get all bank accounts
- `POST /api/bankaccounts` - Create new account
- `GET /api/bankaccounts/{id}` - Get specific account
- `PUT /api/bankaccounts/{id}` - Update account
- `DELETE /api/bankaccounts/{id}` - Delete account

#### Mutual Fund Holdings
- `GET /api/mutualfundholdings` - Get all MF holdings
- `POST /api/mutualfundholdings` - Create new holding
- `GET /api/mutualfundholdings/{id}` - Get specific holding
- `PUT /api/mutualfundholdings/{id}` - Update holding
- `DELETE /api/mutualfundholdings/{id}` - Delete holding
- `GET /api/mutualfundholdings/portfolio-summary` - Get portfolio summary

#### Database Management (Development Only)
- `GET /api/database/status` - Check database connection and migration status
- `POST /api/database/migrate` - Apply all pending migrations
- `POST /api/database/create` - Create database if not exists

#### AI Stock Research 🤖
- `GET /api/aimodelconfigurations` - Get all AI configurations
- `POST /api/aimodelconfigurations` - Create new AI provider configuration
- `GET /api/aimodelconfigurations/default` - Get default AI configuration
- `PUT /api/aimodelconfigurations/{id}` - Update AI configuration
- `DELETE /api/aimodelconfigurations/{id}` - Delete AI configuration
- `POST /api/stockanalysis` - Create AI-powered stock analysis
- `GET /api/stockanalysis/{id}` - Get analysis by ID
- `GET /api/stockanalysis/symbol/{symbol}` - Get all analyses for a symbol
- `GET /api/stockanalysis/quick-overview/{symbol}` - Get quick AI overview
- `POST /api/stockanalysis/research` - Interactive AI stock research
- `POST /api/stockanalysis/batch` - **NEW: Batch analysis for multiple stocks**
- `POST /api/stockanalysis/history` - **NEW: Get analysis history with filters**

#### MCP Server Integration 🔌
- `GET /api/mcpserver` - Get all MCP server configurations
- `POST /api/mcpserver` - Create new MCP server configuration
- `GET /api/mcpserver/{id}` - Get MCP configuration by ID
- `GET /api/mcpserver/default` - Get default MCP configuration
- `PUT /api/mcpserver/{id}` - Update MCP configuration
- `DELETE /api/mcpserver/{id}` - Delete MCP configuration
- `POST /api/mcpserver/{id}/test` - Test MCP server connection
- `POST /api/mcpserver/fetch` - Fetch data from MCP server
- `POST /api/mcpserver/fetch/batch` - Fetch batch data from MCP server

## 🔒 Security

- JWT-based authentication
- Password hashing (BCrypt)
- CORS configuration
- Input validation
- SQL injection prevention via EF Core

## 🧪 Testing

```bash
# Run unit tests
dotnet test tests/PersonalFinance.UnitTests

# Run integration tests
dotnet test tests/PersonalFinance.IntegrationTests
```

## 📦 Database Migrations

### Prerequisites: Install EF Core Tools

**Option 1: Install Globally (Recommended)**
```bash
dotnet tool install --global dotnet-ef --version 10.0.2
```

**Option 2: Install Locally (Per Project)**
```bash
dotnet tool restore
```

Verify installation:
```bash
dotnet ef --version
```

### Method 1: Using CLI (Traditional Approach)

#### Create a new migration

```bash
cd src/PersonalFinance.API
dotnet ef migrations add MigrationName --project ../PersonalFinance.Infrastructure
```

#### Apply migrations

```bash
dotnet ef database update --project ../PersonalFinance.Infrastructure
```

#### Remove last migration

```bash
dotnet ef migrations remove --project ../PersonalFinance.Infrastructure
```

### Method 2: Using API Endpoints (Quick & Easy for Development)

We provide convenient API endpoints for database management during development:

#### Check Database Status
```http
GET /api/database/status
```
Returns migration status, pending migrations, and connection info.

#### Apply All Pending Migrations
```http
POST /api/database/migrate
```
Applies all pending migrations to your database.

#### Create Database
```http
POST /api/database/create
```
Creates the database if it doesn't exist.

**Example with curl:**
```bash
# Check status
curl http://localhost:5000/api/database/status

# Apply migrations
curl -X POST http://localhost:5000/api/database/migrate
```

**Using Swagger UI:**
1. Run the API: `dotnet run --project src/PersonalFinance.API`
2. Open: `http://localhost:5000/swagger`
3. Navigate to **Database** controller
4. Try the endpoints

⚠️ **Security Warning**: These endpoints are set to `[AllowAnonymous]` for development. Remove or secure them before production deployment!

### Detailed Migration Guide

For comprehensive migration instructions including Supabase configuration, see [DATABASE_MIGRATION_GUIDE.md](DATABASE_MIGRATION_GUIDE.md)

## 📚 Research & Planning Documents

### Wealth Tracking Enhancement Research
Comprehensive research on features needed to track wealth over time:
- **[Wealth Tracking Research](WEALTH_TRACKING_RESEARCH.md)** - 80+ page detailed analysis covering 38 prioritized features across 3 phases, complete with data models, API designs, competitive analysis, and implementation roadmap
- **[Wealth Tracking Quick Guide](WEALTH_TRACKING_QUICK_GUIDE.md)** - Concise 4-week implementation guide with checklists, mockups, and success metrics

**Key Recommendations**:
- 🔴 **Priority 1 (Must Have)**: Net worth dashboard, liability tracking, historical snapshots, basic visualizations
- 🟡 **Priority 2 (Should Have)**: Goal tracking, expense tracking, tax planning, real estate tracking
- 🟢 **Priority 3 (Nice to Have)**: Multi-currency, automated imports, predictive analytics, mobile app

See research documents for complete analysis and phased implementation plan.

## 🌐 Future Enhancements

### Phase 1 (Current)
- ✅ Backend API with core modules
- ✅ Database design and implementation
- ✅ Basic CRUD operations
- ✅ Docker support

### Phase 2 (In Progress)
- 🔄 Angular Material frontend
- 🔄 User authentication UI
- 🔄 Dashboard with charts
- 🔄 Live stock price integration

### Phase 3 (Completed & In Progress)
- ✅ **AI-powered stock research assistant**
- ✅ **Multi-provider AI configuration**
- ✅ **Stock analysis with caching**
- ✅ **Confidence score algorithm**
- ✅ **Batch analysis for multiple stocks**
- ✅ **Enhanced analysis history tracking with filtering**
- ✅ **MCP server integration foundation**
- 📋 Live stock price integration via MCP
- 📋 Predictive analytics
- 📋 Automated reporting
- 📋 Mobile app (React Native)

### Phase 4 (Planned - Wealth Tracking)
Based on comprehensive research (see documents above):
- 📋 **Net worth dashboard & calculation**
- 📋 **Liability tracking** (loans, credit cards, mortgages)
- 📋 **Wealth snapshots** (historical tracking over time)
- 📋 **Visualizations** (net worth trends, asset allocation charts)
- 📋 **Goal tracking & planning**
- 📋 **Expense & income tracking**
- 📋 **Tax planning & reporting**
- 📋 **Real estate & other assets**

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## 📝 License

This project is licensed under the MIT License.

## 👥 Authors

- **Personal Finance Team**

## 🙏 Acknowledgments

- .NET Community
- PostgreSQL Team
- Angular Team
- All contributors