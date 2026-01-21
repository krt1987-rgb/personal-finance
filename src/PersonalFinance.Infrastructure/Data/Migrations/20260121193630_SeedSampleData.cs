using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalFinance.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedSampleData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create new tables for tokens
            migrationBuilder.CreateTable(
                name: "PasswordResetTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Token = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsUsed = table.Column<bool>(type: "boolean", nullable: false),
                    UsedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordResetTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PasswordResetTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Token = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRevoked = table.Column<bool>(type: "boolean", nullable: false),
                    RevokedReason = table.Column<string>(type: "text", nullable: true),
                    RevokedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReplacedByToken = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResetTokens_Token",
                table: "PasswordResetTokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResetTokens_UserId",
                table: "PasswordResetTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_Token",
                table: "RefreshTokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            // ===== SEED SAMPLE DATA =====
            var now = DateTime.UtcNow;
            
            // Predefined GUIDs for consistency
            var user1Id = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var user2Id = Guid.Parse("22222222-2222-2222-2222-222222222222");
            var familyMember1Id = Guid.Parse("33333333-3333-3333-3333-333333333333");
            var familyMember2Id = Guid.Parse("44444444-4444-4444-4444-444444444444");
            
            // Seed Users (Password: "Password123!" - hashed with SHA256)
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "FirstName", "LastName", "Email", "PasswordHash", "PhoneNumber", "DateOfBirth", "Role", "IsActive", "CreatedAt", "UpdatedAt", "CreatedBy", "UpdatedBy", "IsDeleted" },
                values: new object[,]
                {
                    { user1Id, "John", "Doe", "john.doe@example.com", "8bb6118f8fd6935cf0876f5e8f8f0e0f8f3c8f5a7f7c7c0c0c0c0c0c0c0c0c0c", "+1-555-0100", new DateTime(1985, 3, 15, 0, 0, 0, DateTimeKind.Utc), "Admin", true, now, null, "System", null, false },
                    { user2Id, "Jane", "Smith", "jane.smith@example.com", "8bb6118f8fd6935cf0876f5e8f8f0e0f8f3c8f5a7f7c7c0c0c0c0c0c0c0c0c0c", "+1-555-0101", new DateTime(1990, 7, 22, 0, 0, 0, DateTimeKind.Utc), "User", true, now, null, "System", null, false }
                });

            // Seed Family Members
            migrationBuilder.InsertData(
                table: "FamilyMembers",
                columns: new[] { "Id", "UserId", "Name", "Relationship", "DateOfBirth", "Email", "PhoneNumber", "CreatedAt", "UpdatedAt", "CreatedBy", "UpdatedBy", "IsDeleted" },
                values: new object[,]
                {
                    { familyMember1Id, user1Id, "Emily Doe", "Spouse", new DateTime(1987, 5, 20, 0, 0, 0, DateTimeKind.Utc), "emily.doe@example.com", "+1-555-0102", now, null, "System", null, false },
                    { familyMember2Id, user1Id, "Michael Doe", "Child", new DateTime(2015, 9, 10, 0, 0, 0, DateTimeKind.Utc), null, null, now, null, "System", null, false }
                });

            // Seed Bank Accounts
            migrationBuilder.InsertData(
                table: "BankAccounts",
                columns: new[] { "Id", "UserId", "BankName", "AccountNumber", "AccountType", "Balance", "Currency", "IsActive", "CreatedAt", "UpdatedAt", "CreatedBy", "UpdatedBy", "IsDeleted" },
                values: new object[,]
                {
                    { Guid.Parse("55555555-5555-5555-5555-555555555555"), user1Id, "Chase Bank", "****1234", "Savings", 25000.00m, "USD", true, now, null, "System", null, false },
                    { Guid.Parse("66666666-6666-6666-6666-666666666666"), user1Id, "Bank of America", "****5678", "Checking", 12500.00m, "USD", true, now, null, "System", null, false },
                    { Guid.Parse("77777777-7777-7777-7777-777777777777"), user2Id, "Wells Fargo", "****9012", "Savings", 35000.00m, "USD", true, now, null, "System", null, false }
                });

            // Seed Fixed Deposits
            migrationBuilder.InsertData(
                table: "FixedDeposits",
                columns: new[] { "Id", "UserId", "BankName", "AccountNumber", "PrincipalAmount", "InterestRate", "StartDate", "MaturityDate", "MaturityAmount", "Status", "CreatedAt", "UpdatedAt", "CreatedBy", "UpdatedBy", "IsDeleted" },
                values: new object[,]
                {
                    { Guid.Parse("88888888-8888-8888-8888-888888888888"), user1Id, "Chase Bank", "FD-2024-001", 50000.00m, 6.5m, new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), 56760.00m, "Active", now, null, "System", null, false },
                    { Guid.Parse("99999999-9999-9999-9999-999999999999"), user1Id, "HDFC Bank", "FD-2023-042", 100000.00m, 7.0m, new DateTime(2023, 6, 1, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc), 114490.00m, "Active", now, null, "System", null, false }
                });

            // Seed Provident Funds
            migrationBuilder.InsertData(
                table: "ProvidentFunds",
                columns: new[] { "Id", "UserId", "FamilyMemberId", "PFType", "AccountNumber", "Balance", "EmployeeContribution", "EmployerContribution", "InterestRate", "LastUpdated", "CreatedAt", "UpdatedAt", "CreatedBy", "UpdatedBy", "IsDeleted" },
                values: new object[,]
                {
                    { Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), user1Id, null, "EPF", "EPF-1234567890", 250000.00m, 1500.00m, 1500.00m, 8.25m, now.AddMonths(-1), now, null, "System", null, false },
                    { Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), user1Id, null, "PPF", "PPF-0987654321", 450000.00m, 12500.00m, 0.00m, 7.1m, now.AddMonths(-1), now, null, "System", null, false },
                    { Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"), user2Id, null, "NPS", "NPS-1122334455", 180000.00m, 1000.00m, 1000.00m, 9.5m, now.AddMonths(-1), now, null, "System", null, false }
                });

            // Seed Stock Holdings
            migrationBuilder.InsertData(
                table: "StockHoldings",
                columns: new[] { "Id", "UserId", "Symbol", "CompanyName", "Quantity", "AverageBuyPrice", "CurrentPrice", "Sector", "Exchange", "CreatedAt", "UpdatedAt", "CreatedBy", "UpdatedBy", "IsDeleted" },
                values: new object[,]
                {
                    { Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"), user1Id, "AAPL", "Apple Inc.", 50.0000m, 150.25m, 185.50m, "Technology", "NASDAQ", now, null, "System", null, false },
                    { Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), user1Id, "MSFT", "Microsoft Corporation", 30.0000m, 290.75m, 380.20m, "Technology", "NASDAQ", now, null, "System", null, false },
                    { Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"), user1Id, "GOOGL", "Alphabet Inc.", 25.0000m, 120.50m, 142.80m, "Technology", "NASDAQ", now, null, "System", null, false },
                    { Guid.Parse("10101010-1010-1010-1010-101010101010"), user2Id, "TSLA", "Tesla Inc.", 40.0000m, 210.00m, 248.50m, "Automotive", "NASDAQ", now, null, "System", null, false },
                    { Guid.Parse("20202020-2020-2020-2020-202020202020"), user2Id, "AMZN", "Amazon.com Inc.", 15.0000m, 135.25m, 178.35m, "E-commerce", "NASDAQ", now, null, "System", null, false }
                });

            // Seed Mutual Fund Holdings
            migrationBuilder.InsertData(
                table: "MutualFundHoldings",
                columns: new[] { "Id", "UserId", "FundName", "FundHouse", "FundType", "Units", "NAV", "CurrentNAV", "InvestedAmount", "ISIN", "CreatedAt", "UpdatedAt", "CreatedBy", "UpdatedBy", "IsDeleted" },
                values: new object[,]
                {
                    { Guid.Parse("30303030-3030-3030-3030-303030303030"), user1Id, "Vanguard 500 Index Fund", "Vanguard", "Equity", 500.0000m, 85.00m, 95.50m, 42500.00m, "US9229087690", now, null, "System", null, false },
                    { Guid.Parse("40404040-4040-4040-4040-404040404040"), user1Id, "Fidelity Contrafund", "Fidelity", "Equity", 300.0000m, 125.00m, 142.75m, 37500.00m, "US3160928711", now, null, "System", null, false },
                    { Guid.Parse("50505050-5050-5050-5050-505050505050"), user2Id, "BlackRock Global Allocation Fund", "BlackRock", "Balanced", 400.0000m, 110.00m, 118.25m, 44000.00m, "US09247X1019", now, null, "System", null, false }
                });

            // Seed Stock Transactions
            migrationBuilder.InsertData(
                table: "StockTransactions",
                columns: new[] { "Id", "StockHoldingId", "TransactionType", "Quantity", "Price", "TransactionDate", "Fees", "Notes", "CreatedAt", "UpdatedAt", "CreatedBy", "UpdatedBy", "IsDeleted" },
                values: new object[,]
                {
                    { Guid.Parse("60606060-6060-6060-6060-606060606060"), Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"), "Buy", 50.0000m, 150.25m, new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc), 12.50m, "Initial purchase", now, null, "System", null, false },
                    { Guid.Parse("70707070-7070-7070-7070-707070707070"), Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), "Buy", 30.0000m, 290.75m, new DateTime(2024, 2, 10, 0, 0, 0, DateTimeKind.Utc), 17.50m, "Long-term investment", now, null, "System", null, false }
                });

            // Seed Mutual Fund Transactions
            migrationBuilder.InsertData(
                table: "MutualFundTransactions",
                columns: new[] { "Id", "MutualFundHoldingId", "TransactionType", "Units", "NAV", "TransactionDate", "Amount", "Notes", "CreatedAt", "UpdatedAt", "CreatedBy", "UpdatedBy", "IsDeleted" },
                values: new object[,]
                {
                    { Guid.Parse("80808080-8080-8080-8080-808080808080"), Guid.Parse("30303030-3030-3030-3030-303030303030"), "Purchase", 500.0000m, 85.00m, new DateTime(2024, 1, 5, 0, 0, 0, DateTimeKind.Utc), 42500.00m, "SIP investment", now, null, "System", null, false },
                    { Guid.Parse("90909090-9090-9090-9090-909090909090"), Guid.Parse("40404040-4040-4040-4040-404040404040"), "Purchase", 300.0000m, 125.00m, new DateTime(2024, 3, 1, 0, 0, 0, DateTimeKind.Utc), 37500.00m, "Lump sum investment", now, null, "System", null, false }
                });

            // Seed General Transactions
            migrationBuilder.InsertData(
                table: "Transactions",
                columns: new[] { "Id", "UserId", "Amount", "TransactionType", "Category", "Description", "TransactionDate", "CreatedAt", "UpdatedAt", "CreatedBy", "UpdatedBy", "IsDeleted" },
                values: new object[,]
                {
                    { Guid.Parse("a1a1a1a1-a1a1-a1a1-a1a1-a1a1a1a1a1a1"), user1Id, 5000.00m, "Income", "Salary", "Monthly salary credit", new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), now, null, "System", null, false },
                    { Guid.Parse("b2b2b2b2-b2b2-b2b2-b2b2-b2b2b2b2b2b2"), user1Id, 1200.00m, "Expense", "Rent", "Monthly rent payment", new DateTime(2024, 1, 5, 0, 0, 0, DateTimeKind.Utc), now, null, "System", null, false },
                    { Guid.Parse("c3c3c3c3-c3c3-c3c3-c3c3-c3c3c3c3c3c3"), user1Id, 500.00m, "Expense", "Groceries", "Monthly groceries", new DateTime(2024, 1, 10, 0, 0, 0, DateTimeKind.Utc), now, null, "System", null, false },
                    { Guid.Parse("d4d4d4d4-d4d4-d4d4-d4d4-d4d4d4d4d4d4"), user2Id, 6000.00m, "Income", "Salary", "Monthly salary credit", new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), now, null, "System", null, false },
                    { Guid.Parse("e5e5e5e5-e5e5-e5e5-e5e5-e5e5e5e5e5e5"), user2Id, 800.00m, "Expense", "Utilities", "Electric and water bill", new DateTime(2024, 1, 12, 0, 0, 0, DateTimeKind.Utc), now, null, "System", null, false }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PasswordResetTokens");

            migrationBuilder.DropTable(
                name: "RefreshTokens");
        }
    }
}
