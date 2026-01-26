# Database Migration Guide

## Migration System

This project uses **DBUp** for database migrations, providing a SQL-first approach with version-controlled migration scripts.

> **Note**: The project is transitioning from Entity Framework Core migrations to DBUp. EF Core is still available but DBUp is the recommended approach for new migrations.

## Quick Start with DBUp (Recommended)

### 1. Run Database Migrations

```bash
# Navigate to the migration project
cd src/PersonalFinance.DatabaseMigration

# Run migrations (uses default connection string or environment variable)
dotnet run

# Or with custom connection string
dotnet run "Host=localhost;Port=5432;Database=PersonalFinanceDb;Username=postgres;Password=postgres"
```

### 2. Using Docker

```bash
# Run just the database migration
docker-compose --profile migration up db-migration

# Or run all services (which will run migrations first)
docker-compose up
```

### 3. Check Migration Status

```bash
# Connect to database and view migration history
psql -h localhost -U postgres -d PersonalFinanceDb -c "SELECT * FROM schemaversions ORDER BY applied;"
```

## DBUp Migration System

### Why DBUp?

- ✅ **SQL-First**: Write SQL directly for full control
- ✅ **Standalone**: Run migrations without starting the API
- ✅ **Simple**: Easy to understand and debug
- ✅ **Version Control**: All changes in SQL files
- ✅ **Team Friendly**: SQL scripts are easy to review

### Creating New Migrations

1. **Create a new SQL file** in `src/PersonalFinance.DatabaseMigration/Scripts/`:
   ```bash
   cd src/PersonalFinance.DatabaseMigration/Scripts
   touch 0002_AddNewFeature.sql
   ```

2. **Write your SQL migration** (use idempotent statements):
   ```sql
   -- 0002_AddNewFeature.sql
   -- Description: Add notifications table
   
   CREATE TABLE IF NOT EXISTS "Notifications" (
       "Id" uuid NOT NULL,
       "UserId" uuid NOT NULL,
       "Message" text NOT NULL,
       "IsRead" boolean NOT NULL DEFAULT false,
       "CreatedAt" timestamp with time zone NOT NULL,
       CONSTRAINT "PK_Notifications" PRIMARY KEY ("Id")
   );
   ```

3. **Run the migration**:
   ```bash
   cd src/PersonalFinance.DatabaseMigration
   dotnet run
   ```

### Migration Scripts

All migration scripts are in `src/PersonalFinance.DatabaseMigration/Scripts/`:
- `0001_AllMigrations.sql` - Complete initial schema (all existing EF migrations combined)
- `0002_YourFeature.sql` - Your next migration
- etc.

Scripts are executed in **alphabetical order**. See [DatabaseMigration README](../src/PersonalFinance.DatabaseMigration/README.md) for details.

---

## Alternative: Entity Framework Core (Legacy)

> **Note**: This method is being phased out in favor of DBUp. Use DBUp for new migrations.

### Prerequisites: Install EF Core Tools
```bash
dotnet tool install --global dotnet-ef --version 10.0.2
```

### Option 2: Install Locally (Per Project)
```bash
dotnet tool restore
```
This will install the tools defined in `.config/dotnet-tools.json`

## Verifying Installation
```bash
dotnet ef --version
```
You should see: `Entity Framework Core .NET Command-line Tools 10.0.2`

## Running Migrations

### Method 1: Using CLI (Manual)

#### Create a new migration:
```bash
cd src/PersonalFinance.API
dotnet ef migrations add MigrationName --project ../PersonalFinance.Infrastructure --context ApplicationDbContext
```

#### Apply migrations to database:
```bash
cd src/PersonalFinance.API
dotnet ef database update --project ../PersonalFinance.Infrastructure
```

#### Remove last migration (if not applied):
```bash
cd src/PersonalFinance.API
dotnet ef migrations remove --project ../PersonalFinance.Infrastructure
```

### Method 2: Using API Endpoints (Convenient for Development)

We've added API endpoints to manage migrations programmatically:

#### 1. Check Database Status
```
GET /api/database/status
```
Returns:
- Connection status
- Applied migrations count
- Pending migrations count
- List of all migrations

#### 2. Apply Pending Migrations
```
POST /api/database/migrate
```
Applies all pending migrations to the database.

#### 3. Create Database (if it doesn't exist)
```
POST /api/database/create
```
Creates the database structure.

### Example using curl:
```bash
# Check status
curl http://localhost:5000/api/database/status

# Apply migrations
curl -X POST http://localhost:5000/api/database/migrate

# Create database
curl -X POST http://localhost:5000/api/database/create
```

### Example using Swagger:
1. Run the API: `dotnet run --project src/PersonalFinance.API`
2. Open browser: `http://localhost:5000/swagger`
3. Navigate to **Database** section
4. Try the endpoints

## Connection String Configuration

### Update appsettings.json
Edit `src/PersonalFinance.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=your-supabase-host;Port=5432;Database=your-db-name;Username=your-username;Password=your-password;SSL Mode=Require"
  }
}
```

### For Supabase:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=db.xxxxxxxxxxxx.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=your-password;SSL Mode=Require"
  }
}
```

## Troubleshooting

### Error: "Could not execute because the specified command or file was not found"
**Solution**: Install EF Core tools using one of the methods above.

### Error: "Assets file not found"
**Solution**: Run `dotnet restore` in the solution directory.

### Error: "Cannot connect to database"
**Solution**: 
1. Verify your connection string in `appsettings.json`
2. Check that your database server is running
3. For Supabase, ensure you're using the correct credentials from your project settings

### Error: "Migration already exists"
**Solution**: 
- Remove the last migration: `dotnet ef migrations remove --project ../PersonalFinance.Infrastructure`
- Or create a new migration with a different name

## Security Warning ⚠️

**IMPORTANT**: The `/api/database/*` endpoints are currently set to `[AllowAnonymous]` for development convenience.

**Before deploying to production:**
1. Change `[AllowAnonymous]` to `[Authorize]` with admin role requirement
2. Or remove these endpoints entirely and use CLI for migrations
3. Never expose migration endpoints publicly without proper authentication

## Best Practices

### Development
- Use API endpoints for quick database setup
- Use CLI for creating new migrations during development

### Production
- **Always** use CLI or CI/CD pipelines to apply migrations
- **Never** use API endpoints in production
- Test migrations in staging environment first
- Backup database before applying migrations
- Review migration SQL before applying: `dotnet ef migrations script`

## Additional Commands

### Generate SQL script for migrations:
```bash
cd src/PersonalFinance.API
dotnet ef migrations script --project ../PersonalFinance.Infrastructure --output migration.sql
```

### Update to specific migration:
```bash
cd src/PersonalFinance.API
dotnet ef database update MigrationName --project ../PersonalFinance.Infrastructure
```

### Rollback to previous migration:
```bash
cd src/PersonalFinance.API
dotnet ef database update PreviousMigrationName --project ../PersonalFinance.Infrastructure
```

### Drop database (CAUTION):
```bash
cd src/PersonalFinance.API
dotnet ef database drop --project ../PersonalFinance.Infrastructure
```
