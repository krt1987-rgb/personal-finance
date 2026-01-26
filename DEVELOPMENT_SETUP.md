# Development Setup Guide

This guide will help you set up the Personal Finance Management System for local development.

## 📋 Prerequisites

### Required Software

1. **[.NET 10 SDK](https://dotnet.microsoft.com/download)**
   - Version: 10.0.0 or higher
   - Verify installation: `dotnet --version`

2. **[PostgreSQL 16+](https://www.postgresql.org/download/)**
   - Version: 16.0 or higher
   - Alternative: Use Docker (recommended for development)
   - Verify installation: `psql --version`

3. **[Docker & Docker Compose](https://www.docker.com/get-started)** (Recommended)
   - Docker Desktop for Windows/Mac
   - Docker Engine + Docker Compose for Linux
   - Verify installation: `docker --version` and `docker-compose --version`

4. **[Node.js 20+](https://nodejs.org/)** (For Angular frontend)
   - Version: 20.0.0 or higher
   - Includes npm package manager
   - Verify installation: `node --version` and `npm --version`

5. **[Git](https://git-scm.com/downloads)**
   - Version control system
   - Verify installation: `git --version`

### Optional Tools

- **[Visual Studio 2022](https://visualstudio.microsoft.com/)** or **[Visual Studio Code](https://code.visualstudio.com/)**
- **[Postman](https://www.postman.com/)** or **[Insomnia](https://insomnia.rest/)** for API testing
- **[pgAdmin](https://www.pgadmin.org/)** or **[DBeaver](https://dbeaver.io/)** for database management
- **[Angular CLI](https://angular.io/cli)**: `npm install -g @angular/cli`

## 🚀 Quick Start (Docker - Recommended)

### 1. Clone the Repository

```bash
git clone https://github.com/krt1987-rgb/personal-finance.git
cd personal-finance
```

### 2. Start the Application with Docker Compose

```bash
# Start PostgreSQL and API
docker-compose up -d

# View logs
docker-compose logs -f

# Stop services
docker-compose down
```

The services will be available at:
- **API**: http://localhost:5000 and https://localhost:5001
- **Swagger UI**: http://localhost:5000/swagger
- **PostgreSQL**: localhost:5432

### 3. Access the Application

- Open your browser to http://localhost:5000/swagger
- Use the Swagger UI to test API endpoints

## 🛠️ Manual Setup (Without Docker)

### 1. Clone the Repository

```bash
git clone https://github.com/krt1987-rgb/personal-finance.git
cd personal-finance
```

### 2. Setup PostgreSQL Database

#### Option A: Local PostgreSQL Installation

1. Start PostgreSQL service:
   ```bash
   # Windows (as service)
   # Already running if installed as service
   
   # macOS (using Homebrew)
   brew services start postgresql@16
   
   # Linux (systemd)
   sudo systemctl start postgresql
   ```

2. Create database:
   ```bash
   # Connect to PostgreSQL
   psql -U postgres
   
   # Create database
   CREATE DATABASE PersonalFinanceDb;
   
   # Create user (optional)
   CREATE USER pfuser WITH PASSWORD 'your_password';
   GRANT ALL PRIVILEGES ON DATABASE PersonalFinanceDb TO pfuser;
   
   # Exit
   \q
   ```

#### Option B: Docker PostgreSQL Only

```bash
docker-compose up -d postgres
```

### 3. Configure Connection String

Edit `src/PersonalFinance.API/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=PersonalFinanceDb;Username=postgres;Password=postgres"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

**For production (`appsettings.json`)**, use environment variables or secure configuration:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=${DB_HOST};Port=${DB_PORT};Database=${DB_NAME};Username=${DB_USER};Password=${DB_PASSWORD};SSL Mode=Require"
  }
}
```

### 4. Install .NET Dependencies

```bash
# Restore NuGet packages
dotnet restore

# Or restore for specific project
cd src/PersonalFinance.API
dotnet restore
```

### 5. Run Database Migrations

**Note**: Currently using EF Core migrations. DBUp migration system is being implemented.

```bash
cd src/PersonalFinance.API

# Install EF Core tools (if not already installed)
dotnet tool install --global dotnet-ef --version 10.0.2

# Verify installation
dotnet ef --version

# Apply migrations
dotnet ef database update --project ../PersonalFinance.Infrastructure
```

**Alternative**: Use API endpoints (development only):
```bash
# Start the API first
dotnet run --project src/PersonalFinance.API

# In another terminal
curl -X POST http://localhost:5000/api/database/migrate
```

### 6. Run the API

```bash
cd src/PersonalFinance.API
dotnet run
```

The API will start at:
- HTTP: http://localhost:5000
- HTTPS: https://localhost:5001
- Swagger: http://localhost:5000/swagger

### 7. Setup Angular Frontend (Optional)

```bash
cd src/PersonalFinance.Web

# Install dependencies
npm install

# Start development server
npm start
```

The Angular app will be available at http://localhost:4200

## 🔧 Development Workflow

### Building the Solution

```bash
# Build entire solution
dotnet build

# Build specific project
dotnet build src/PersonalFinance.API/PersonalFinance.API.csproj

# Build in Release mode
dotnet build --configuration Release
```

### Running Tests

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/PersonalFinance.UnitTests

# Run with coverage (requires coverlet)
dotnet test /p:CollectCoverage=true
```

### Code Formatting

```bash
# Format code
dotnet format

# Check formatting without making changes
dotnet format --verify-no-changes
```

### Database Operations

#### View Database Status
```bash
curl http://localhost:5000/api/database/status
```

#### Create Migration (EF Core)
```bash
cd src/PersonalFinance.API
dotnet ef migrations add MigrationName --project ../PersonalFinance.Infrastructure
```

#### Apply Migrations (EF Core)
```bash
cd src/PersonalFinance.API
dotnet ef database update --project ../PersonalFinance.Infrastructure
```

#### Generate SQL Script
```bash
cd src/PersonalFinance.API
dotnet ef migrations script --project ../PersonalFinance.Infrastructure --output migration.sql
```

### Hot Reload

.NET 10 supports hot reload for faster development:

```bash
cd src/PersonalFinance.API
dotnet watch run
```

Changes to C# files will automatically reload without restarting the application.

## 🔍 Debugging

### Visual Studio 2022

1. Open `PersonalFinance.sln`
2. Set `PersonalFinance.API` as startup project
3. Press F5 to start debugging

### Visual Studio Code

1. Open the repository folder
2. Install recommended extensions:
   - C# Dev Kit
   - C# Extensions
   - Docker
3. Press F5 to start debugging (uses `.vscode/launch.json` if configured)

### Browser DevTools

- Use browser developer tools for frontend debugging
- Network tab for API calls
- Console for JavaScript errors

## 🧪 Testing the API

### Using Swagger UI

1. Navigate to http://localhost:5000/swagger
2. Explore available endpoints
3. Try out endpoints directly in the browser

### Using curl

```bash
# Check API health
curl http://localhost:5000/api/health

# Get all stock holdings (requires authentication)
curl -H "Authorization: Bearer YOUR_TOKEN" http://localhost:5000/api/stockholdings

# Create a new bank account
curl -X POST http://localhost:5000/api/bankaccounts \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{
    "bankName": "HDFC Bank",
    "accountNumber": "12345678901234",
    "accountType": 0,
    "currentBalance": 50000.00
  }'
```

### Using Postman

1. Import the API collection (if available)
2. Set base URL: http://localhost:5000
3. Configure authentication token
4. Test endpoints

## 📦 Environment Variables

You can override configuration using environment variables:

```bash
# Connection string
export ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=PersonalFinanceDb;Username=postgres;Password=postgres"

# Logging level
export Logging__LogLevel__Default="Debug"

# Run the API
dotnet run --project src/PersonalFinance.API
```

On Windows (PowerShell):
```powershell
$env:ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=PersonalFinanceDb;Username=postgres;Password=postgres"
dotnet run --project src/PersonalFinance.API
```

## 🐳 Docker Development

### Build Docker Image

```bash
# Build API image
docker build -f src/PersonalFinance.API/Dockerfile -t personalfinance-api:dev .
```

### Run with Docker Compose

```bash
# Start all services
docker-compose up -d

# View logs
docker-compose logs -f api

# Restart API only
docker-compose restart api

# Stop all services
docker-compose down

# Stop and remove volumes (clean slate)
docker-compose down -v
```

### Access PostgreSQL in Docker

```bash
# Connect to PostgreSQL container
docker exec -it personalfinance-postgres psql -U postgres -d PersonalFinanceDb

# Or use connection string
psql "postgresql://postgres:postgres@localhost:5432/PersonalFinanceDb"
```

## 🤖 AI Configuration (Optional)

To use the AI-powered stock research features:

1. Configure an AI provider (see [AI_CONFIGURATION_GUIDE.md](AI_CONFIGURATION_GUIDE.md))
2. Add API key in `appsettings.Development.json` or as environment variable
3. Test AI endpoints in Swagger

Example:
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

## 🔒 Security for Development

### Default Credentials

**⚠️ Warning**: These are development-only credentials. Never use in production!

- PostgreSQL: `postgres` / `postgres`
- Database: `PersonalFinanceDb`

### JWT Authentication

Currently, some endpoints use `[AllowAnonymous]` for development convenience:
- `/api/database/*` endpoints
- Health check endpoints

These should be secured before production deployment.

## 🧹 Cleanup

### Reset Database

```bash
# Drop and recreate database
cd src/PersonalFinance.API
dotnet ef database drop --project ../PersonalFinance.Infrastructure
dotnet ef database update --project ../PersonalFinance.Infrastructure
```

### Clean Build Artifacts

```bash
# Clean solution
dotnet clean

# Remove bin and obj folders
find . -iname "bin" -o -iname "obj" | xargs rm -rf

# Clean Docker
docker-compose down -v
docker system prune -f
```

## 📚 Additional Resources

- [README.md](README.md) - Project overview
- [DATABASE_MIGRATION_GUIDE.md](DATABASE_MIGRATION_GUIDE.md) - Detailed migration instructions
- [AI_CONFIGURATION_GUIDE.md](AI_CONFIGURATION_GUIDE.md) - AI features setup
- [IMPLEMENTATION_STATUS.md](IMPLEMENTATION_STATUS.md) - Feature implementation status
- [.NET 10 Documentation](https://docs.microsoft.com/en-us/dotnet/)
- [Entity Framework Core Documentation](https://docs.microsoft.com/en-us/ef/core/)
- [Angular Documentation](https://angular.io/docs)

## 🆘 Troubleshooting

### Port Already in Use

```bash
# Find process using port 5000
# Linux/Mac
lsof -i :5000
kill -9 <PID>

# Windows
netstat -ano | findstr :5000
taskkill /PID <PID> /F
```

### PostgreSQL Connection Issues

1. Check if PostgreSQL is running
2. Verify connection string in `appsettings.Development.json`
3. Check firewall settings
4. Ensure database exists: `psql -U postgres -l`

### EF Core Migration Errors

```bash
# Clear migrations and start fresh
cd src/PersonalFinance.API
dotnet ef migrations remove --project ../PersonalFinance.Infrastructure
dotnet ef database drop --project ../PersonalFinance.Infrastructure
dotnet ef migrations add InitialCreate --project ../PersonalFinance.Infrastructure
dotnet ef database update --project ../PersonalFinance.Infrastructure
```

### Docker Issues

```bash
# Reset Docker environment
docker-compose down -v
docker system prune -af
docker-compose up -d
```

## 💡 Tips for Productive Development

1. **Use Hot Reload**: `dotnet watch run` for faster iterations
2. **Use Swagger**: Test APIs directly in the browser
3. **Use Docker**: Consistent environment across team
4. **Use Git**: Commit frequently with meaningful messages
5. **Use Breakpoints**: Debug effectively with Visual Studio or VS Code
6. **Read Logs**: Check logs in `logs/` directory or Docker logs
7. **Check Status**: Use `/api/database/status` to verify migrations

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/your-feature`
3. Make your changes
4. Run tests: `dotnet test`
5. Commit your changes: `git commit -am 'Add new feature'`
6. Push to the branch: `git push origin feature/your-feature`
7. Create a Pull Request

## 📝 Notes

- The project uses Clean Architecture pattern
- API follows REST conventions
- Database uses UTC timestamps
- All monetary values use `decimal` with precision 18,2
- Soft delete is implemented for all entities
- Audit fields track creation and updates

---

**Happy Coding! 🚀**
