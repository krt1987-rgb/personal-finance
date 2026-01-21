using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalFinance.Infrastructure.Data;

namespace PersonalFinance.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DatabaseController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DatabaseController> _logger;

    public DatabaseController(ApplicationDbContext context, ILogger<DatabaseController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Apply pending database migrations
    /// WARNING: Use with caution in production
    /// </summary>
    [HttpPost("migrate")]
    [AllowAnonymous] // Change to [Authorize] with admin role in production
    public async Task<ActionResult> ApplyMigrations()
    {
        try
        {
            _logger.LogInformation("Starting database migration...");
            
            // Get pending migrations
            var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();
            var pendingList = pendingMigrations.ToList();
            
            if (!pendingList.Any())
            {
                _logger.LogInformation("No pending migrations found");
                return Ok(new
                {
                    message = "Database is already up to date",
                    pendingMigrations = 0
                });
            }

            _logger.LogInformation("Found {Count} pending migrations: {Migrations}", 
                pendingList.Count, string.Join(", ", pendingList));

            // Apply migrations
            await _context.Database.MigrateAsync();
            
            _logger.LogInformation("Database migration completed successfully");
            
            return Ok(new
            {
                message = "Database migrations applied successfully",
                appliedMigrations = pendingList.Count,
                migrations = pendingList
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error applying database migrations");
            return StatusCode(500, new
            {
                message = "An error occurred while applying migrations",
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// Check database connection and migration status
    /// </summary>
    [HttpGet("status")]
    [AllowAnonymous] // Change to [Authorize] with admin role in production
    public async Task<ActionResult> GetDatabaseStatus()
    {
        try
        {
            // Check if database can be connected
            var canConnect = await _context.Database.CanConnectAsync();
            
            if (!canConnect)
            {
                return Ok(new
                {
                    canConnect = false,
                    message = "Cannot connect to database. Please check connection string."
                });
            }

            // Get applied migrations
            var appliedMigrations = await _context.Database.GetAppliedMigrationsAsync();
            var appliedList = appliedMigrations.ToList();

            // Get pending migrations
            var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();
            var pendingList = pendingMigrations.ToList();

            return Ok(new
            {
                canConnect = true,
                databaseExists = true,
                appliedMigrations = appliedList.Count,
                pendingMigrations = pendingList.Count,
                appliedMigrationsList = appliedList,
                pendingMigrationsList = pendingList,
                isUpToDate = !pendingList.Any()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking database status");
            return StatusCode(500, new
            {
                message = "An error occurred while checking database status",
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// Create the database if it doesn't exist
    /// WARNING: Use with caution
    /// </summary>
    [HttpPost("create")]
    [AllowAnonymous] // Change to [Authorize] with admin role in production
    public async Task<ActionResult> CreateDatabase()
    {
        try
        {
            _logger.LogInformation("Attempting to create database...");
            
            var created = await _context.Database.EnsureCreatedAsync();
            
            if (created)
            {
                _logger.LogInformation("Database created successfully");
                return Ok(new
                {
                    message = "Database created successfully",
                    created = true
                });
            }
            else
            {
                _logger.LogInformation("Database already exists");
                return Ok(new
                {
                    message = "Database already exists",
                    created = false
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating database");
            return StatusCode(500, new
            {
                message = "An error occurred while creating the database",
                error = ex.Message
            });
        }
    }
}
