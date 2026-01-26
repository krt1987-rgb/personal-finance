# Database Migration Scripts

This directory contains SQL migration scripts that are executed by DBUp in sequential order.

## Naming Convention

Scripts are executed in alphabetical order. Follow this naming pattern:

```
<number>_<description>.sql
```

Examples:
- `0001_AllMigrations.sql` - Initial database schema with all tables
- `0002_AddNewFeature.sql` - Add new feature
- `0003_UpdateExistingTable.sql` - Modify existing table

## Script Guidelines

1. **Idempotent**: Scripts should be safe to run multiple times
2. **Sequential**: Use incrementing numbers (0001, 0002, 0003, etc.)
3. **Descriptive**: Use clear names describing what the script does
4. **Test First**: Test scripts in development before committing
5. **Rollback**: Consider including rollback scripts for reversible changes

## Current Scripts

### 0001_AllMigrations.sql
Contains the complete initial database schema including:
- Users table
- BankAccounts table
- FamilyMembers table
- MutualFundHoldings table
- StockHoldings table
- ProvidentFunds table
- FixedDeposits table
- AIModelConfiguration table (AI features)
- MCPServerConfiguration table (MCP integration)
- All related transactions tables
- Indexes and constraints

This script is idempotent and safe to run multiple times.

## Adding New Migration Scripts

1. Create a new SQL file with the next sequential number:
   ```bash
   touch Scripts/0002_AddNewFeature.sql
   ```

2. Write your SQL migration:
   ```sql
   -- 0002_AddNewFeature.sql
   -- Description: Add new feature to the system
   
   -- Add your SQL here
   CREATE TABLE IF NOT EXISTS "NewTable" (
       "Id" uuid NOT NULL,
       "Name" text NOT NULL,
       CONSTRAINT "PK_NewTable" PRIMARY KEY ("Id")
   );
   ```

3. The script will be automatically embedded in the assembly during build

4. Run the migration tool to apply the script

## Running Migrations

### Development
```bash
cd src/PersonalFinance.DatabaseMigration
dotnet run
```

### With Custom Connection String
```bash
dotnet run "Host=localhost;Port=5432;Database=PersonalFinanceDb;Username=postgres;Password=postgres"
```

### Using Environment Variable
```bash
export ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=PersonalFinanceDb;Username=postgres;Password=postgres"
dotnet run
```

### Production
```bash
cd src/PersonalFinance.DatabaseMigration
dotnet run --configuration Release "your-production-connection-string"
```

## Migration History

DBUp tracks executed scripts in the `schemaversions` table. You can query this table to see which scripts have been applied:

```sql
SELECT * FROM public.schemaversions ORDER BY applied;
```

## Rollback

DBUp doesn't support automatic rollback. To rollback:

1. Create a new migration script that reverses the changes
2. Or restore from a database backup
3. Or manually execute rollback SQL statements

Example rollback script:
```sql
-- 0003_Rollback_AddNewFeature.sql
DROP TABLE IF EXISTS "NewTable";
```

## Best Practices

1. **Always backup** the database before running migrations in production
2. **Test thoroughly** in development and staging environments
3. **Review SQL** generated from migrations before applying
4. **Keep scripts small** - one logical change per script
5. **Document changes** in script comments
6. **Version control** all migration scripts
7. **Never modify** existing scripts that have been applied to production

## Transitioning from EF Core Migrations

The initial script (0001_AllMigrations.sql) contains all existing EF Core migrations combined into one idempotent script. Future changes should be added as new sequential scripts instead of EF Core migrations.

## Advantages of DBUp

1. **SQL-First**: Write SQL directly, full control over database changes
2. **Standalone**: Can run migrations without the application
3. **Simple**: Easy to understand and debug
4. **Flexible**: Works with any .NET hosting environment
5. **Trackable**: All changes are in version-controlled SQL files
6. **Reviewable**: SQL scripts can be reviewed in pull requests

## Resources

- [DBUp Documentation](https://dbup.readthedocs.io/)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)
- [Migration Best Practices](https://dbup.readthedocs.io/en/latest/more-info/best-practices/)
