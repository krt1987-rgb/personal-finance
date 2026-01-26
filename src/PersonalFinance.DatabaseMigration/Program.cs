using System;
using System.Linq;
using System.Reflection;
using DbUp;

namespace PersonalFinance.DatabaseMigration;

class Program
{
    static int Main(string[] args)
    {
        // Get connection string from command line argument or environment variable
        var connectionString = args.FirstOrDefault()
            ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? "Host=localhost;Port=5432;Database=PersonalFinanceDb;Username=postgres;Password=postgres";

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║  Personal Finance - Database Migration Tool (DBUp)        ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();

        Console.WriteLine($"Connection String: {MaskPassword(connectionString)}");
        Console.WriteLine();

        try
        {
            // Ensure the database exists
            EnsureDatabase.For.PostgresqlDatabase(connectionString);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✓ Database verified/created successfully");
            Console.ResetColor();
            Console.WriteLine();

            // Configure DBUp to run all SQL scripts in the Scripts folder
            var upgrader = DeployChanges.To
                .PostgresqlDatabase(connectionString)
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                .LogToConsole()
                .Build();

            // Check if any scripts need to be executed
            var scriptsToExecute = upgrader.GetScriptsToExecute();
            
            if (!scriptsToExecute.Any())
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ Database is already up to date. No migrations needed.");
                Console.ResetColor();
                return 0;
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Found {scriptsToExecute.Count} migration script(s) to execute:");
            foreach (var script in scriptsToExecute)
            {
                Console.WriteLine($"  - {script.Name}");
            }
            Console.ResetColor();
            Console.WriteLine();

            // Perform the upgrade
            Console.WriteLine("Executing migrations...");
            Console.WriteLine();
            
            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine();
                Console.WriteLine("✗ Migration failed!");
                Console.WriteLine($"Error: {result.Error}");
                Console.ResetColor();
                return -1;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("✓ Success! All migrations completed successfully.");
            Console.ResetColor();
            return 0;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine();
            Console.WriteLine("✗ An error occurred during migration:");
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine();
            Console.WriteLine("Stack Trace:");
            Console.WriteLine(ex.StackTrace);
            Console.ResetColor();
            return -1;
        }
    }

    private static string MaskPassword(string connectionString)
    {
        // Simple password masking for console output
        var parts = connectionString.Split(';');
        var masked = parts.Select(part =>
        {
            if (part.Trim().StartsWith("Password=", StringComparison.OrdinalIgnoreCase))
            {
                return "Password=********";
            }
            return part;
        });
        return string.Join(";", masked);
    }
}
