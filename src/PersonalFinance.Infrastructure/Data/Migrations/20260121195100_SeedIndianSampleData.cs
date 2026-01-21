using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalFinance.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedIndianSampleData : Migration
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

            // ===== SEED INDIAN SAMPLE DATA =====
            var now = DateTime.UtcNow;
            
            // Predefined GUIDs for consistency
            var user1Id = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var user2Id = Guid.Parse("22222222-2222-2222-2222-222222222222");
            var familyMember1Id = Guid.Parse("33333333-3333-3333-3333-333333333333");
            var familyMember2Id = Guid.Parse("44444444-4444-4444-4444-444444444444");
            
            // Seed Users (Password: "Password123!" - hashed with SHA256)
            // Password hash: 8C6976E5B5410415BDE908BD4DEE15DFB167A9C873FC4BB8A81F6F2AB448A918
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "FirstName", "LastName", "Email", "PasswordHash", "PhoneNumber", "DateOfBirth", "IsActive", "CreatedAt", "UpdatedAt", "CreatedBy", "UpdatedBy", "IsDeleted" },
                values: new object[,]
                {
                    { user1Id, "Rajesh", "Kumar", "rajesh.kumar@example.com", "8C6976E5B5410415BDE908BD4DEE15DFB167A9C873FC4BB8A81F6F2AB448A918", "+91-98765-43210", new DateTime(1985, 3, 15, 0, 0, 0, DateTimeKind.Utc), true, now, null, "System", null, false },
                    { user2Id, "Priya", "Sharma", "priya.sharma@example.com", "8C6976E5B5410415BDE908BD4DEE15DFB167A9C873FC4BB8A81F6F2AB448A918", "+91-98765-43211", new DateTime(1990, 7, 22, 0, 0, 0, DateTimeKind.Utc), true, now, null, "System", null, false }
                });

            // Seed Family Members
            migrationBuilder.InsertData(
                table: "FamilyMembers",
                columns: new[] { "Id", "UserId", "Name", "Relationship", "DateOfBirth", "Email", "PhoneNumber", "CreatedAt", "UpdatedAt", "CreatedBy", "UpdatedBy", "IsDeleted" },
                values: new object[,]
                {
                    { familyMember1Id, user1Id, "Anjali Kumar", "Spouse", new DateTime(1987, 5, 20, 0, 0, 0, DateTimeKind.Utc), "anjali.kumar@example.com", "+91-98765-43212", now, null, "System", null, false },
                    { familyMember2Id, user1Id, "Arjun Kumar", "Child", new DateTime(2015, 9, 10, 0, 0, 0, DateTimeKind.Utc), null, null, now, null, "System", null, false }
                });

            // Seed Bank Accounts (INR amounts)
            migrationBuilder.InsertData(
                table: "BankAccounts",
                columns: new[] { "Id", "UserId", "BankName", "AccountNumber", "AccountType", "Balance", "Currency", "IsActive", "CreatedAt", "UpdatedAt", "CreatedBy", "UpdatedBy", "IsDeleted" },
                values: new object[,]
                {
                    { Guid.Parse("55555555-5555-5555-5555-555555555555"), user1Id, "HDFC Bank", "****1234", "Savings", 2075000.00m, "INR", true, now, null, "System", null, false },
                    { Guid.Parse("66666666-6666-6666-6666-666666666666"), user1Id, "ICICI Bank", "****5678", "Current", 1037500.00m, "INR", true, now, null, "System", null, false },
                    { Guid.Parse("77777777-7777-7777-7777-777777777777"), user2Id, "State Bank of India", "****9012", "Savings", 2905000.00m, "INR", true, now, null, "System", null, false }
                });

            // Seed Fixed Deposits (INR amounts)
            migrationBuilder.InsertData(
                table: "FixedDeposits",
                columns: new[] { "Id", "UserId", "BankName", "AccountNumber", "PrincipalAmount", "InterestRate", "StartDate", "MaturityDate", "MaturityAmount", "Status", "CreatedAt", "UpdatedAt", "CreatedBy", "UpdatedBy", "IsDeleted" },
                values: new object[,]
                {
                    { Guid.Parse("88888888-8888-8888-8888-888888888888"), user1Id, "HDFC Bank", "FD-2024-001", 4150000.00m, 7.25m, new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), 4711080.00m, "Active", now, null, "System", null, false },
                    { Guid.Parse("99999999-9999-9999-9999-999999999999"), user1Id, "ICICI Bank", "FD-2023-042", 8300000.00m, 7.5m, new DateTime(2023, 6, 1, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc), 9502670.00m, "Active", now, null, "System", null, false }
                });

            // Seed Provident Funds (INR amounts)
            migrationBuilder.InsertData(
                table: "ProvidentFunds",
                columns: new[] { "Id", "UserId", "FamilyMemberId", "PFType", "AccountNumber", "Balance", "EmployeeContribution", "EmployerContribution", "InterestRate", "LastUpdated", "CreatedAt", "UpdatedAt", "CreatedBy", "UpdatedBy", "IsDeleted" },
                values: new object[,]
                {
                    { Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), user1Id, null, "EPF", "EPF-1234567890", 20750000.00m, 124500.00m, 124500.00m, 8.25m, now.AddMonths(-1), now, null, "System", null, false },
                    { Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), user1Id, null, "PPF", "PPF-0987654321", 37350000.00m, 1037500.00m, 0.00m, 7.1m, now.AddMonths(-1), now, null, "System", null, false },
                    { Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"), user2Id, null, "NPS", "NPS-1122334455", 14940000.00m, 83000.00m, 83000.00m, 9.5m, now.AddMonths(-1), now, null, "System", null, false }
                });

            // Seed Stock Holdings (NSE/BSE stocks with INR prices)
            migrationBuilder.InsertData(
                table: "StockHoldings",
                columns: new[] { "Id", "UserId", "Symbol", "CompanyName", "Quantity", "AverageBuyPrice", "CurrentPrice", "Sector", "Exchange", "CreatedAt", "UpdatedAt", "CreatedBy", "UpdatedBy", "IsDeleted" },
                values: new object[,]
                {
                    { Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"), user1Id, "RELIANCE", "Reliance Industries Ltd", 150.0000m, 2450.75m, 2875.50m, "Oil & Gas", "NSE", now, null, "System", null, false },
                    { Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), user1Id, "TCS", "Tata Consultancy Services", 100.0000m, 3280.50m, 3965.20m, "IT", "NSE", now, null, "System", null, false },
                    { Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"), user1Id, "INFY", "Infosys Ltd", 200.0000m, 1425.25m, 1678.80m, "IT", "NSE", now, null, "System", null, false },
                    { Guid.Parse("10101010-1010-1010-1010-101010101010"), user2Id, "HDFCBANK", "HDFC Bank Ltd", 120.0000m, 1580.00m, 1745.50m, "Banking", "NSE", now, null, "System", null, false },
                    { Guid.Parse("20202020-2020-2020-2020-202020202020"), user2Id, "WIPRO", "Wipro Ltd", 300.0000m, 425.50m, 498.35m, "IT", "NSE", now, null, "System", null, false }
                });

            // Seed Mutual Fund Holdings (Indian mutual funds with INR amounts)
            migrationBuilder.InsertData(
                table: "MutualFundHoldings",
                columns: new[] { "Id", "UserId", "FundName", "FundHouse", "FundType", "Units", "NAV", "CurrentNAV", "InvestedAmount", "ISIN", "CreatedAt", "UpdatedAt", "CreatedBy", "UpdatedBy", "IsDeleted" },
                values: new object[,]
                {
                    { Guid.Parse("30303030-3030-3030-3030-303030303030"), user1Id, "SBI Bluechip Fund", "SBI Mutual Fund", "Equity", 2500.0000m, 85.00m, 96.50m, 212500.00m, "INF200K01VX9", now, null, "System", null, false },
                    { Guid.Parse("40404040-4040-4040-4040-404040404040"), user1Id, "HDFC Balanced Advantage Fund", "HDFC Mutual Fund", "Hybrid", 1800.0000m, 225.00m, 258.75m, 405000.00m, "INF179K01VX7", now, null, "System", null, false },
                    { Guid.Parse("50505050-5050-5050-5050-505050505050"), user2Id, "ICICI Prudential Technology Fund", "ICICI Prudential", "Equity", 3000.0000m, 165.00m, 189.25m, 495000.00m, "INF109K01VX5", now, null, "System", null, false }
                });

            // Seed Stock Transactions
            migrationBuilder.InsertData(
                table: "StockTransactions",
                columns: new[] { "Id", "StockHoldingId", "TransactionType", "Quantity", "Price", "TransactionDate", "Fees", "Notes", "CreatedAt", "UpdatedAt", "CreatedBy", "UpdatedBy", "IsDeleted" },
                values: new object[,]
                {
                    { Guid.Parse("60606060-6060-6060-6060-606060606060"), Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"), "Buy", 150.0000m, 2450.75m, new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc), 735.00m, "Initial purchase", now, null, "System", null, false },
                    { Guid.Parse("70707070-7070-7070-7070-707070707070"), Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), "Buy", 100.0000m, 3280.50m, new DateTime(2024, 2, 10, 0, 0, 0, DateTimeKind.Utc), 984.00m, "Long-term investment", now, null, "System", null, false }
                });

            // Seed Mutual Fund Transactions
            migrationBuilder.InsertData(
                table: "MutualFundTransactions",
                columns: new[] { "Id", "MutualFundHoldingId", "TransactionType", "Units", "NAV", "TransactionDate", "Amount", "Notes", "CreatedAt", "UpdatedAt", "CreatedBy", "UpdatedBy", "IsDeleted" },
                values: new object[,]
                {
                    { Guid.Parse("80808080-8080-8080-8080-808080808080"), Guid.Parse("30303030-3030-3030-3030-303030303030"), "Purchase", 2500.0000m, 85.00m, new DateTime(2024, 1, 5, 0, 0, 0, DateTimeKind.Utc), 212500.00m, "SIP investment", now, null, "System", null, false },
                    { Guid.Parse("90909090-9090-9090-9090-909090909090"), Guid.Parse("40404040-4040-4040-4040-404040404040"), "Purchase", 1800.0000m, 225.00m, new DateTime(2024, 3, 1, 0, 0, 0, DateTimeKind.Utc), 405000.00m, "Lump sum investment", now, null, "System", null, false }
                });

            // Seed General Transactions (INR amounts)
            migrationBuilder.InsertData(
                table: "Transactions",
                columns: new[] { "Id", "UserId", "Amount", "TransactionType", "Category", "Description", "TransactionDate", "CreatedAt", "UpdatedAt", "CreatedBy", "UpdatedBy", "IsDeleted" },
                values: new object[,]
                {
                    { Guid.Parse("a1a1a1a1-a1a1-a1a1-a1a1-a1a1a1a1a1a1"), user1Id, 415000.00m, "Income", "Salary", "Monthly salary credit", new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), now, null, "System", null, false },
                    { Guid.Parse("b2b2b2b2-b2b2-b2b2-b2b2-b2b2b2b2b2b2"), user1Id, 99600.00m, "Expense", "Rent", "Monthly rent payment", new DateTime(2024, 1, 5, 0, 0, 0, DateTimeKind.Utc), now, null, "System", null, false },
                    { Guid.Parse("c3c3c3c3-c3c3-c3c3-c3c3-c3c3c3c3c3c3"), user1Id, 41500.00m, "Expense", "Groceries", "Monthly groceries", new DateTime(2024, 1, 10, 0, 0, 0, DateTimeKind.Utc), now, null, "System", null, false },
                    { Guid.Parse("d4d4d4d4-d4d4-d4d4-d4d4-d4d4d4d4d4d4"), user2Id, 498000.00m, "Income", "Salary", "Monthly salary credit", new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), now, null, "System", null, false },
                    { Guid.Parse("e5e5e5e5-e5e5-e5e5-e5e5-e5e5e5e5e5e5"), user2Id, 66400.00m, "Expense", "Utilities", "Electric and water bill", new DateTime(2024, 1, 12, 0, 0, 0, DateTimeKind.Utc), now, null, "System", null, false }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PasswordResetTokens");

            migrationBuilder.DropTable(
                name: "RefreshTokens");
            
            // Delete seed data
            migrationBuilder.DeleteData(table: "Transactions", keyColumn: "Id", keyValue: Guid.Parse("a1a1a1a1-a1a1-a1a1-a1a1-a1a1a1a1a1a1"));
            migrationBuilder.DeleteData(table: "Transactions", keyColumn: "Id", keyValue: Guid.Parse("b2b2b2b2-b2b2-b2b2-b2b2-b2b2b2b2b2b2"));
            migrationBuilder.DeleteData(table: "Transactions", keyColumn: "Id", keyValue: Guid.Parse("c3c3c3c3-c3c3-c3c3-c3c3-c3c3c3c3c3c3"));
            migrationBuilder.DeleteData(table: "Transactions", keyColumn: "Id", keyValue: Guid.Parse("d4d4d4d4-d4d4-d4d4-d4d4-d4d4d4d4d4d4"));
            migrationBuilder.DeleteData(table: "Transactions", keyColumn: "Id", keyValue: Guid.Parse("e5e5e5e5-e5e5-e5e5-e5e5-e5e5e5e5e5e5"));
            
            migrationBuilder.DeleteData(table: "MutualFundTransactions", keyColumn: "Id", keyValue: Guid.Parse("80808080-8080-8080-8080-808080808080"));
            migrationBuilder.DeleteData(table: "MutualFundTransactions", keyColumn: "Id", keyValue: Guid.Parse("90909090-9090-9090-9090-909090909090"));
            
            migrationBuilder.DeleteData(table: "StockTransactions", keyColumn: "Id", keyValue: Guid.Parse("60606060-6060-6060-6060-606060606060"));
            migrationBuilder.DeleteData(table: "StockTransactions", keyColumn: "Id", keyValue: Guid.Parse("70707070-7070-7070-7070-707070707070"));
            
            migrationBuilder.DeleteData(table: "MutualFundHoldings", keyColumn: "Id", keyValue: Guid.Parse("30303030-3030-3030-3030-303030303030"));
            migrationBuilder.DeleteData(table: "MutualFundHoldings", keyColumn: "Id", keyValue: Guid.Parse("40404040-4040-4040-4040-404040404040"));
            migrationBuilder.DeleteData(table: "MutualFundHoldings", keyColumn: "Id", keyValue: Guid.Parse("50505050-5050-5050-5050-505050505050"));
            
            migrationBuilder.DeleteData(table: "StockHoldings", keyColumn: "Id", keyValue: Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"));
            migrationBuilder.DeleteData(table: "StockHoldings", keyColumn: "Id", keyValue: Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"));
            migrationBuilder.DeleteData(table: "StockHoldings", keyColumn: "Id", keyValue: Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"));
            migrationBuilder.DeleteData(table: "StockHoldings", keyColumn: "Id", keyValue: Guid.Parse("10101010-1010-1010-1010-101010101010"));
            migrationBuilder.DeleteData(table: "StockHoldings", keyColumn: "Id", keyValue: Guid.Parse("20202020-2020-2020-2020-202020202020"));
            
            migrationBuilder.DeleteData(table: "ProvidentFunds", keyColumn: "Id", keyValue: Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"));
            migrationBuilder.DeleteData(table: "ProvidentFunds", keyColumn: "Id", keyValue: Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"));
            migrationBuilder.DeleteData(table: "ProvidentFunds", keyColumn: "Id", keyValue: Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"));
            
            migrationBuilder.DeleteData(table: "FixedDeposits", keyColumn: "Id", keyValue: Guid.Parse("88888888-8888-8888-8888-888888888888"));
            migrationBuilder.DeleteData(table: "FixedDeposits", keyColumn: "Id", keyValue: Guid.Parse("99999999-9999-9999-9999-999999999999"));
            
            migrationBuilder.DeleteData(table: "BankAccounts", keyColumn: "Id", keyValue: Guid.Parse("55555555-5555-5555-5555-555555555555"));
            migrationBuilder.DeleteData(table: "BankAccounts", keyColumn: "Id", keyValue: Guid.Parse("66666666-6666-6666-6666-666666666666"));
            migrationBuilder.DeleteData(table: "BankAccounts", keyColumn: "Id", keyValue: Guid.Parse("77777777-7777-7777-7777-777777777777"));
            
            migrationBuilder.DeleteData(table: "FamilyMembers", keyColumn: "Id", keyValue: Guid.Parse("33333333-3333-3333-3333-333333333333"));
            migrationBuilder.DeleteData(table: "FamilyMembers", keyColumn: "Id", keyValue: Guid.Parse("44444444-4444-4444-4444-444444444444"));
            
            migrationBuilder.DeleteData(table: "Users", keyColumn: "Id", keyValue: Guid.Parse("11111111-1111-1111-1111-111111111111"));
            migrationBuilder.DeleteData(table: "Users", keyColumn: "Id", keyValue: Guid.Parse("22222222-2222-2222-2222-222222222222"));
        }
    }
}
