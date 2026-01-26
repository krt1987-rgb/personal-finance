# Release and Deployment Guide

This guide covers building, testing, and deploying the Personal Finance Management System to production environments.

## 📋 Table of Contents

1. [Pre-Deployment Checklist](#pre-deployment-checklist)
2. [Building for Production](#building-for-production)
3. [Database Deployment](#database-deployment)
4. [Application Deployment](#application-deployment)
5. [Docker Deployment](#docker-deployment)
6. [Cloud Deployment](#cloud-deployment)
7. [Monitoring and Logging](#monitoring-and-logging)
8. [Backup and Recovery](#backup-and-recovery)
9. [Security Considerations](#security-considerations)
10. [Rollback Procedures](#rollback-procedures)

## ✅ Pre-Deployment Checklist

### Code Quality
- [ ] All tests passing (`dotnet test`)
- [ ] Code builds without warnings (`dotnet build --configuration Release`)
- [ ] Code formatting verified (`dotnet format --verify-no-changes`)
- [ ] Security scan completed (if applicable)
- [ ] Dependencies updated to stable versions
- [ ] No hardcoded secrets or credentials

### Configuration
- [ ] Environment-specific configurations prepared
- [ ] Connection strings using environment variables
- [ ] API keys and secrets stored securely (Azure Key Vault, AWS Secrets Manager, etc.)
- [ ] CORS settings configured for production domain
- [ ] Logging levels set appropriately (Warning or Error for production)
- [ ] JWT secret key is strong and unique for production

### Database
- [ ] Backup of production database taken
- [ ] Migration scripts tested in staging environment
- [ ] Database connection strings verified
- [ ] Database user has minimum required permissions

### Security
- [ ] HTTPS enforced
- [ ] Development endpoints removed or secured (`/api/database/*`)
- [ ] `[AllowAnonymous]` removed from protected endpoints
- [ ] Authentication and authorization tested
- [ ] Rate limiting configured
- [ ] CORS restricted to specific origins

### Documentation
- [ ] Release notes prepared
- [ ] API documentation updated
- [ ] Changelog updated
- [ ] Deployment runbook reviewed

## 🏗️ Building for Production

### 1. Clean and Build

```bash
# Clean previous builds
dotnet clean --configuration Release

# Restore dependencies
dotnet restore

# Build in Release mode
dotnet build --configuration Release --no-restore
```

### 2. Run Tests

```bash
# Run all tests
dotnet test --configuration Release --no-build

# Run with coverage
dotnet test --configuration Release --no-build /p:CollectCoverage=true
```

### 3. Publish Application

```bash
# Publish API
dotnet publish src/PersonalFinance.API/PersonalFinance.API.csproj \
  --configuration Release \
  --output ./publish/api \
  --runtime linux-x64 \
  --self-contained false

# Or for self-contained deployment (includes .NET runtime)
dotnet publish src/PersonalFinance.API/PersonalFinance.API.csproj \
  --configuration Release \
  --output ./publish/api \
  --runtime linux-x64 \
  --self-contained true
```

The published files will be in `./publish/api/` directory.

### 4. Build Angular Frontend

```bash
cd src/PersonalFinance.Web

# Install dependencies
npm ci

# Build for production
npm run build --prod

# Output will be in dist/personal-finance-web/
```

## 💾 Database Deployment

### Method 1: Using EF Core Migrations (Current)

**⚠️ Note**: DBUp migration system is being implemented to replace this.

#### Generate SQL Script

```bash
cd src/PersonalFinance.API

# Generate SQL for all migrations
dotnet ef migrations script \
  --project ../PersonalFinance.Infrastructure \
  --output ../migrations.sql \
  --idempotent

# Or for specific migration range
dotnet ef migrations script \
  --project ../PersonalFinance.Infrastructure \
  --from PreviousMigration \
  --to TargetMigration \
  --output ../migrations.sql
```

#### Review and Apply

1. **Review the SQL script** carefully
2. **Test in staging** environment first
3. **Backup production database**
4. **Apply the migration**:

```bash
# Apply using psql
psql -U username -d PersonalFinanceDb -f migrations.sql

# Or using dotnet ef (not recommended for production)
# Only use if you have direct access and understand the risks
cd src/PersonalFinance.API
dotnet ef database update --project ../PersonalFinance.Infrastructure
```

### Method 2: Using DBUp (Coming Soon)

DBUp will provide a standalone database migration tool with SQL scripts.

```bash
# Run database migration project (once implemented)
cd src/PersonalFinance.DatabaseMigration
dotnet run --configuration Release
```

### Database Connection String for Production

Use environment variables or secure configuration:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=${DB_HOST};Port=${DB_PORT};Database=${DB_NAME};Username=${DB_USER};Password=${DB_PASSWORD};SSL Mode=Require;Trust Server Certificate=false"
  }
}
```

Or set as environment variable:
```bash
export ConnectionStrings__DefaultConnection="Host=your-host;Port=5432;Database=PersonalFinanceDb;Username=dbuser;Password=secure_password;SSL Mode=Require"
```

## 🚀 Application Deployment

### Deployment to Linux Server (Manual)

#### 1. Install Prerequisites

```bash
# Install .NET 10 Runtime (not SDK for production)
wget https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --runtime aspnetcore --version 10.0.0

# Or install using package manager
# See: https://learn.microsoft.com/en-us/dotnet/core/install/linux
```

#### 2. Upload Published Files

```bash
# Copy published files to server
scp -r ./publish/api user@server:/var/www/personalfinance-api

# Or use rsync
rsync -avz ./publish/api/ user@server:/var/www/personalfinance-api/
```

#### 3. Configure as Systemd Service

Create `/etc/systemd/system/personalfinance-api.service`:

```ini
[Unit]
Description=Personal Finance API
After=network.target postgresql.service

[Service]
Type=notify
User=www-data
WorkingDirectory=/var/www/personalfinance-api
ExecStart=/usr/bin/dotnet /var/www/personalfinance-api/PersonalFinance.API.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=personalfinance-api
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
```

#### 4. Start the Service

```bash
# Reload systemd
sudo systemctl daemon-reload

# Enable service to start on boot
sudo systemctl enable personalfinance-api

# Start the service
sudo systemctl start personalfinance-api

# Check status
sudo systemctl status personalfinance-api

# View logs
sudo journalctl -u personalfinance-api -f
```

#### 5. Configure Nginx Reverse Proxy

Create `/etc/nginx/sites-available/personalfinance-api`:

```nginx
server {
    listen 80;
    server_name api.yourcompany.com;
    
    # Redirect HTTP to HTTPS
    return 301 https://$server_name$request_uri;
}

server {
    listen 443 ssl http2;
    server_name api.yourcompany.com;
    
    ssl_certificate /etc/letsencrypt/live/api.yourcompany.com/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/api.yourcompany.com/privkey.pem;
    
    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
    
    # Increase buffer sizes for large requests
    client_max_body_size 50M;
    proxy_buffer_size 128k;
    proxy_buffers 4 256k;
    proxy_busy_buffers_size 256k;
}
```

Enable and restart Nginx:

```bash
sudo ln -s /etc/nginx/sites-available/personalfinance-api /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl restart nginx
```

## 🐳 Docker Deployment

### 1. Build Production Docker Image

```bash
# Build the image
docker build \
  -f src/PersonalFinance.API/Dockerfile \
  -t personalfinance-api:latest \
  -t personalfinance-api:v1.0.0 \
  .

# Test the image locally
docker run -d \
  -p 5000:8080 \
  -e ConnectionStrings__DefaultConnection="Host=host.docker.internal;Port=5432;Database=PersonalFinanceDb;Username=postgres;Password=postgres" \
  --name personalfinance-api-test \
  personalfinance-api:latest

# Check logs
docker logs -f personalfinance-api-test

# Stop and remove
docker stop personalfinance-api-test
docker rm personalfinance-api-test
```

### 2. Push to Container Registry

#### Docker Hub

```bash
docker login

# Tag image
docker tag personalfinance-api:latest yourusername/personalfinance-api:latest
docker tag personalfinance-api:latest yourusername/personalfinance-api:v1.0.0

# Push image
docker push yourusername/personalfinance-api:latest
docker push yourusername/personalfinance-api:v1.0.0
```

#### Azure Container Registry

```bash
# Login to ACR
az acr login --name yourregistry

# Tag image
docker tag personalfinance-api:latest yourregistry.azurecr.io/personalfinance-api:latest
docker tag personalfinance-api:latest yourregistry.azurecr.io/personalfinance-api:v1.0.0

# Push image
docker push yourregistry.azurecr.io/personalfinance-api:latest
docker push yourregistry.azurecr.io/personalfinance-api:v1.0.0
```

### 3. Production Docker Compose

Create `docker-compose.prod.yml`:

```yaml
version: '3.8'

services:
  postgres:
    image: postgres:16-alpine
    container_name: personalfinance-postgres-prod
    environment:
      POSTGRES_DB: ${DB_NAME}
      POSTGRES_USER: ${DB_USER}
      POSTGRES_PASSWORD: ${DB_PASSWORD}
    volumes:
      - postgres_data:/var/lib/postgresql/data
      - ./backups:/backups
    networks:
      - personalfinance-network
    restart: unless-stopped
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U ${DB_USER}"]
      interval: 10s
      timeout: 5s
      retries: 5

  api:
    image: yourusername/personalfinance-api:latest
    container_name: personalfinance-api-prod
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Host=postgres;Port=5432;Database=${DB_NAME};Username=${DB_USER};Password=${DB_PASSWORD}
      - JwtSettings__Secret=${JWT_SECRET}
      - JwtSettings__Issuer=${JWT_ISSUER}
      - JwtSettings__Audience=${JWT_AUDIENCE}
    ports:
      - "5000:8080"
    depends_on:
      postgres:
        condition: service_healthy
    networks:
      - personalfinance-network
    restart: unless-stopped
    volumes:
      - ./logs:/app/logs
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:8080/health"]
      interval: 30s
      timeout: 10s
      retries: 3

volumes:
  postgres_data:

networks:
  personalfinance-network:
    driver: bridge
```

Create `.env` file (do not commit this):

```env
DB_NAME=PersonalFinanceDb
DB_USER=pfuser
DB_PASSWORD=your_secure_password
JWT_SECRET=your_jwt_secret_key_at_least_32_characters
JWT_ISSUER=PersonalFinance
JWT_AUDIENCE=PersonalFinanceUsers
```

Deploy:

```bash
# Start services
docker-compose -f docker-compose.prod.yml up -d

# View logs
docker-compose -f docker-compose.prod.yml logs -f

# Stop services
docker-compose -f docker-compose.prod.yml down
```

## ☁️ Cloud Deployment

### Azure App Service

#### 1. Create Resources

```bash
# Login to Azure
az login

# Create resource group
az group create --name PersonalFinance-RG --location eastus

# Create App Service plan
az appservice plan create \
  --name PersonalFinance-Plan \
  --resource-group PersonalFinance-RG \
  --sku B1 \
  --is-linux

# Create web app
az webapp create \
  --name personalfinance-api \
  --resource-group PersonalFinance-RG \
  --plan PersonalFinance-Plan \
  --runtime "DOTNETCORE:10.0"

# Create PostgreSQL server
az postgres flexible-server create \
  --name personalfinance-db \
  --resource-group PersonalFinance-RG \
  --location eastus \
  --admin-user dbadmin \
  --admin-password "YourSecurePassword123!" \
  --sku-name Standard_B1ms \
  --tier Burstable \
  --version 16
```

#### 2. Configure Application Settings

```bash
# Set connection string
az webapp config connection-string set \
  --name personalfinance-api \
  --resource-group PersonalFinance-RG \
  --connection-string-type PostgreSQL \
  --settings DefaultConnection="Host=personalfinance-db.postgres.database.azure.com;Database=PersonalFinanceDb;Username=dbadmin;Password=YourSecurePassword123!;SSL Mode=Require"

# Set app settings
az webapp config appsettings set \
  --name personalfinance-api \
  --resource-group PersonalFinance-RG \
  --settings \
    ASPNETCORE_ENVIRONMENT=Production \
    JwtSettings__Secret="your_jwt_secret_key" \
    JwtSettings__Issuer="PersonalFinance" \
    JwtSettings__Audience="PersonalFinanceUsers"
```

#### 3. Deploy Application

```bash
# Deploy from local build
cd publish/api
zip -r ../personalfinance-api.zip .
az webapp deployment source config-zip \
  --name personalfinance-api \
  --resource-group PersonalFinance-RG \
  --src ../personalfinance-api.zip

# Or deploy from GitHub
az webapp deployment source config \
  --name personalfinance-api \
  --resource-group PersonalFinance-RG \
  --repo-url https://github.com/yourusername/personal-finance \
  --branch main \
  --manual-integration
```

### AWS Elastic Beanstalk

```bash
# Install EB CLI
pip install awsebcli

# Initialize EB application
eb init -p "64bit Amazon Linux 2023 v3.0.0 running .NET 10" personalfinance-api

# Create environment
eb create personalfinance-api-prod --database.engine postgres

# Deploy
eb deploy

# View logs
eb logs

# Open in browser
eb open
```

### Google Cloud Run

```bash
# Build and push to Google Container Registry
gcloud builds submit --tag gcr.io/PROJECT_ID/personalfinance-api

# Deploy to Cloud Run
gcloud run deploy personalfinance-api \
  --image gcr.io/PROJECT_ID/personalfinance-api \
  --platform managed \
  --region us-central1 \
  --allow-unauthenticated \
  --set-env-vars ConnectionStrings__DefaultConnection="your_connection_string"
```

## 📊 Monitoring and Logging

### Application Insights (Azure)

Add to `appsettings.Production.json`:

```json
{
  "ApplicationInsights": {
    "InstrumentationKey": "your-instrumentation-key",
    "EnableAdaptiveSampling": true,
    "EnablePerformanceCounterCollectionModule": true
  }
}
```

### Serilog Configuration

Production logging configuration in `appsettings.Production.json`:

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Warning",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning",
        "Microsoft.AspNetCore": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "File",
        "Args": {
          "path": "/app/logs/log-.txt",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 30,
          "outputTemplate": "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
        }
      },
      {
        "Name": "Console",
        "Args": {
          "outputTemplate": "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
        }
      }
    ]
  }
}
```

### Health Checks

Ensure health check endpoint is working:

```bash
# Test health endpoint
curl https://api.yourcompany.com/health

# Expected response
{"status":"Healthy","totalDuration":"00:00:00.0123456"}
```

## 💾 Backup and Recovery

### Database Backup

#### Automated Backup Script

```bash
#!/bin/bash
# backup-database.sh

TIMESTAMP=$(date +"%Y%m%d_%H%M%S")
BACKUP_DIR="/backups"
DB_HOST="localhost"
DB_NAME="PersonalFinanceDb"
DB_USER="postgres"

mkdir -p $BACKUP_DIR

# Create backup
pg_dump -h $DB_HOST -U $DB_USER -d $DB_NAME -F c -f "$BACKUP_DIR/backup_$TIMESTAMP.dump"

# Compress backup
gzip "$BACKUP_DIR/backup_$TIMESTAMP.dump"

# Delete backups older than 30 days
find $BACKUP_DIR -name "backup_*.dump.gz" -mtime +30 -delete

echo "Backup completed: backup_$TIMESTAMP.dump.gz"
```

#### Schedule with Cron

```bash
# Add to crontab
crontab -e

# Daily backup at 2 AM
0 2 * * * /opt/scripts/backup-database.sh >> /var/log/backup.log 2>&1
```

### Database Restore

```bash
# Restore from backup
gunzip backup_20260126_020000.dump.gz
pg_restore -h localhost -U postgres -d PersonalFinanceDb -c backup_20260126_020000.dump
```

### Application Backup

- Store published artifacts in version control or artifact repository
- Keep Docker images in container registry with version tags
- Document configuration and environment variables
- Backup SSL certificates and keys

## 🔒 Security Considerations

### Production Security Checklist

- [ ] **HTTPS Only**: Enforce HTTPS, disable HTTP
- [ ] **Secrets Management**: Use Azure Key Vault, AWS Secrets Manager, or environment variables
- [ ] **Database Security**:
  - [ ] Use strong passwords
  - [ ] Limit database user permissions
  - [ ] Enable SSL/TLS for database connections
  - [ ] Regular security patches
- [ ] **API Security**:
  - [ ] JWT with strong secret key
  - [ ] Rate limiting enabled
  - [ ] CORS restricted to specific origins
  - [ ] Input validation
  - [ ] SQL injection prevention (via EF Core)
- [ ] **Remove Development Features**:
  - [ ] Remove or secure `/api/database/*` endpoints
  - [ ] Remove `[AllowAnonymous]` from protected endpoints
  - [ ] Disable Swagger in production (or secure it)
- [ ] **Monitoring**: Enable logging and monitoring
- [ ] **Updates**: Keep dependencies updated
- [ ] **Firewall**: Configure firewall rules
- [ ] **Backups**: Regular automated backups

### Environment Variables for Production

Never hardcode secrets. Use environment variables:

```bash
export ConnectionStrings__DefaultConnection="..."
export JwtSettings__Secret="..."
export ASPNETCORE_ENVIRONMENT="Production"
```

## 🔄 Rollback Procedures

### Application Rollback

#### Docker Deployment

```bash
# Deploy previous version
docker-compose -f docker-compose.prod.yml down
docker-compose -f docker-compose.prod.yml pull
# Edit docker-compose.prod.yml to use previous image tag
docker-compose -f docker-compose.prod.yml up -d
```

#### Systemd Service

```bash
# Stop service
sudo systemctl stop personalfinance-api

# Replace files with previous version
sudo rm -rf /var/www/personalfinance-api
sudo cp -r /var/www/backups/personalfinance-api-v1.0.0 /var/www/personalfinance-api

# Start service
sudo systemctl start personalfinance-api
```

### Database Rollback

```bash
# Rollback to specific migration (EF Core)
cd src/PersonalFinance.API
dotnet ef database update PreviousMigrationName --project ../PersonalFinance.Infrastructure

# Or restore from backup
pg_restore -h localhost -U postgres -d PersonalFinanceDb -c backup_file.dump
```

## 📈 Post-Deployment Verification

### Smoke Tests

```bash
# Health check
curl https://api.yourcompany.com/health

# API version
curl https://api.yourcompany.com/api/version

# Database connectivity (if endpoint available)
curl https://api.yourcompany.com/api/database/status

# Test authentication
curl -X POST https://api.yourcompany.com/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"test@example.com","password":"testpassword"}'
```

### Monitoring Checklist

- [ ] Application is running and responding
- [ ] Database connections working
- [ ] No errors in logs
- [ ] Health check endpoint returning healthy status
- [ ] SSL certificate valid
- [ ] HTTPS redirect working
- [ ] CORS configured correctly
- [ ] Authentication working
- [ ] Key features tested

## 📝 Release Notes Template

```markdown
# Release v1.0.0 - 2026-01-26

## New Features
- Feature 1 description
- Feature 2 description

## Improvements
- Improvement 1
- Improvement 2

## Bug Fixes
- Bug fix 1
- Bug fix 2

## Breaking Changes
- Breaking change 1

## Database Changes
- Migration: AddNewFeature
- Description: Added tables for new feature

## Deployment Notes
- Update environment variable: NEW_SETTING
- Run migration before deploying app
- Clear cache after deployment

## Rollback Plan
- Rollback to v0.9.0
- Rollback migration: dotnet ef database update PreviousMigration
```

---

**For additional help, see:**
- [DEVELOPMENT_SETUP.md](DEVELOPMENT_SETUP.md) - Development environment setup
- [DATABASE_MIGRATION_GUIDE.md](DATABASE_MIGRATION_GUIDE.md) - Database migration details
- [README.md](README.md) - Project overview

**Need support? Check logs and monitoring first, then contact the development team.**
