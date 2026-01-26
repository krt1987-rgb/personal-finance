# PersonalFinance.DatabaseMigration

Standalone database migration tool using DBUp for managing database schema changes with SQL scripts.

## Overview

This project provides a simple, SQL-based approach to database migrations using DBUp. It replaces Entity Framework Core migrations with version-controlled SQL scripts that can be run independently of the main application.

## Features

- ✅ **SQL-First Approach**: Write SQL directly for full control
- ✅ **Standalone Execution**: Run migrations without starting the API
- ✅ **Version Control**: All migrations tracked in SQL files
- ✅ **Idempotent**: Safe to run multiple times
- ✅ **PostgreSQL Optimized**: Built specifically for PostgreSQL
- ✅ **Console Feedback**: Clear, colorful output showing migration status
- ✅ **Automatic Database Creation**: Creates database if it doesn't exist
- ✅ **Migration History**: Tracks executed scripts in `schemaversions` table

## Quick Start

### Prerequisites

- .NET 10 SDK
- PostgreSQL 16+ (running and accessible)

### Basic Usage

```bash
# Navigate to the project directory
cd src/PersonalFinance.DatabaseMigration

# Run with default connection string
dotnet run

# Run with custom connection string
dotnet run "Host=localhost;Port=5432;Database=PersonalFinanceDb;Username=postgres;Password=postgres"

# Run using environment variable
export ConnectionStrings__DefaultConnection="your-connection-string"
dotnet run
```

### Build and Run

```bash
# Build the project
dotnet build

# Run in Debug mode
dotnet run

# Build for Release
dotnet build --configuration Release

# Run Release build
dotnet run --configuration Release
```

## Connection String Options

### 1. Command Line Argument (Highest Priority)
```bash
dotnet run "Host=localhost;Port=5432;Database=PersonalFinanceDb;Username=postgres;Password=postgres"
```

### 2. Environment Variable
```bash
export ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=PersonalFinanceDb;Username=postgres;Password=postgres"
dotnet run
```

### 3. Default (Fallback)
If no connection string is provided, uses:
```
Host=localhost;Port=5432;Database=PersonalFinanceDb;Username=postgres;Password=postgres
```

## Migration Scripts

All migration scripts are located in the `Scripts/` directory and are embedded in the assembly during build.

### Naming Convention

Scripts are executed in alphabetical order:
```
<number>_<description>.sql
```

Examples:
- `0001_AllMigrations.sql` - Initial schema
- `0002_AddNewFeature.sql` - Add new tables/columns
- `0003_UpdateIndexes.sql` - Performance improvements

### Creating New Migrations

1. Create a new SQL file in the `Scripts/` directory:
   ```bash
   touch Scripts/0002_AddNewFeature.sql
   ```

2. Write your SQL migration:
   ```sql
   -- 0002_AddNewFeature.sql
   -- Description: Add notifications table
   
   CREATE TABLE IF NOT EXISTS "Notifications" (
       "Id" uuid NOT NULL,
       "UserId" uuid NOT NULL,
       "Message" text NOT NULL,
       "IsRead" boolean NOT NULL DEFAULT false,
       "CreatedAt" timestamp with time zone NOT NULL,
       CONSTRAINT "PK_Notifications" PRIMARY KEY ("Id"),
       CONSTRAINT "FK_Notifications_Users_UserId" 
           FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
   );
   
   CREATE INDEX IF NOT EXISTS "IX_Notifications_UserId" 
       ON "Notifications" ("UserId");
   ```

3. Build and run the migration:
   ```bash
   dotnet build
   dotnet run
   ```

## Project Structure

```
PersonalFinance.DatabaseMigration/
├── Scripts/                          # SQL migration scripts
│   ├── README.md                    # Scripts documentation
│   └── 0001_AllMigrations.sql      # Initial schema
├── Program.cs                       # DBUp configuration and execution
├── PersonalFinance.DatabaseMigration.csproj
└── README.md                        # This file
```

## How It Works

1. **Database Verification**: Checks if database exists, creates it if needed
2. **Script Discovery**: Finds all embedded SQL scripts
3. **History Check**: Queries `schemaversions` table for executed scripts
4. **Execution**: Runs only new scripts in alphabetical order
5. **History Update**: Records executed scripts with timestamp

## Migration History

DBUp creates a `schemaversions` table to track executed migrations:

```sql
-- View migration history
SELECT * FROM public.schemaversions ORDER BY applied;
```

Example output:
```
 schemaversion | scriptname                         | applied
---------------+------------------------------------+------------------------
             1 | 0001_AllMigrations.sql            | 2026-01-26 10:30:00
             2 | 0002_AddNewFeature.sql            | 2026-01-26 11:45:00
```

## Console Output Example

```
╔════════════════════════════════════════════════════════════╗
║  Personal Finance - Database Migration Tool (DBUp)        ║
╚════════════════════════════════════════════════════════════╝

Connection String: Host=localhost;Port=5432;Database=PersonalFinanceDb;Username=postgres;Password=********

✓ Database verified/created successfully

Found 2 migration script(s) to execute:
  - PersonalFinance.DatabaseMigration.Scripts.0001_AllMigrations.sql
  - PersonalFinance.DatabaseMigration.Scripts.0002_AddNewFeature.sql

Executing migrations...

Beginning database upgrade
Checking whether journal table exists..
Journal table does not exist
Executing Database Server script '0001_AllMigrations.sql'
Executing Database Server script '0002_AddNewFeature.sql'
Upgrade successful

✓ Success! All migrations completed successfully.
```

## Deployment

### Development
```bash
cd src/PersonalFinance.DatabaseMigration
dotnet run
```

### Staging/Production
```bash
cd src/PersonalFinance.DatabaseMigration
dotnet run --configuration Release "production-connection-string"
```

### Docker Integration

Add to your `docker-compose.yml`:

```yaml
services:
  db-migration:
    build:
      context: .
      dockerfile: src/PersonalFinance.DatabaseMigration/Dockerfile
    environment:
      - ConnectionStrings__DefaultConnection=Host=postgres;Port=5432;Database=PersonalFinanceDb;Username=postgres;Password=postgres
    depends_on:
      postgres:
        condition: service_healthy
    networks:
      - personalfinance-network
```

### CI/CD Integration

#### GitHub Actions Example
```yaml
- name: Run Database Migrations
  run: |
    cd src/PersonalFinance.DatabaseMigration
    dotnet run "${{ secrets.DB_CONNECTION_STRING }}"
```

#### Azure DevOps Example
```yaml
- task: DotNetCoreCLI@2
  inputs:
    command: 'run'
    projects: 'src/PersonalFinance.DatabaseMigration/*.csproj'
    arguments: '"$(DbConnectionString)"'
```

## Best Practices

1. **Test First**: Always test migrations in development before production
2. **Idempotent**: Use `IF NOT EXISTS` and `IF EXISTS` for safety
3. **Small Changes**: Keep each migration focused on one logical change
4. **Backup**: Always backup production database before migrations
5. **Review**: Review generated SQL in pull requests
6. **Sequential**: Never reorder or modify existing migration scripts
7. **Descriptive**: Use clear, descriptive names for migration files

## Rollback

DBUp doesn't support automatic rollback. Options:

### 1. Create Rollback Script
```sql
-- 0003_Rollback_AddNotifications.sql
DROP TABLE IF EXISTS "Notifications";
```

### 2. Database Restore
Restore from backup taken before migration.

### 3. Manual SQL
Execute rollback statements manually:
```sql
DROP TABLE "Notifications";
```

## Troubleshooting

### Error: "Cannot connect to database"
- Verify PostgreSQL is running
- Check connection string format
- Ensure credentials are correct
- Test connection: `psql -h localhost -U postgres -d PersonalFinanceDb`

### Error: "Database does not exist"
The tool automatically creates the database if it doesn't exist.

### Error: "Permission denied"
Ensure the database user has sufficient permissions:
```sql
GRANT ALL PRIVILEGES ON DATABASE PersonalFinanceDb TO your_user;
```

### Scripts Not Found
Ensure scripts are embedded resources in the .csproj:
```xml
<ItemGroup>
  <EmbeddedResource Include="Scripts\**\*.sql" />
</ItemGroup>
```

### Migration Already Applied
DBUp tracks executed scripts. If a script was already run, it will be skipped.

To re-run a script (for development only):
```sql
DELETE FROM schemaversions WHERE scriptname = '0002_AddNewFeature.sql';
```

## Comparison: DBUp vs EF Core Migrations

| Feature | DBUp | EF Core Migrations |
|---------|------|-------------------|
| SQL Control | Full SQL control | Generated C# code |
| Standalone | ✅ Yes | ❌ No (requires API) |
| Version Control | ✅ SQL files | C# migration classes |
| Learning Curve | Low | Medium-High |
| IDE Support | Any text editor | Visual Studio/Rider |
| Rollback | Manual | Automated |
| Performance | Fast | Fast |
| Team Collaboration | Easy (SQL) | Can be complex |

## Migration from EF Core

The initial script `0001_AllMigrations.sql` contains all existing EF Core migrations. To fully migrate:

1. ✅ DBUp project created
2. ✅ Initial migration script added
3. ⏭️ Update application to not run EF migrations automatically
4. ⏭️ Remove EF migration dependencies (optional)
5. ⏭️ Future changes: Create SQL scripts in `Scripts/` directory

## Additional Resources

- [DBUp Documentation](https://dbup.readthedocs.io/)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)
- [SQL Best Practices](https://www.postgresql.org/docs/current/sql-syntax.html)
- [Project README](../../README.md)
- [Database Migration Guide](../../DATABASE_MIGRATION_GUIDE.md)

## Support

For issues or questions:
1. Check the [Scripts README](Scripts/README.md)
2. Review the [troubleshooting section](#troubleshooting)
3. Check application logs
4. Contact the development team

---

**Happy Migrating! 🚀**
