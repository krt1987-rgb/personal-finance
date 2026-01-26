using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalFinance.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAIStockResearchFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AIModelConfigurations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ProviderType = table.Column<int>(type: "integer", nullable: false),
                    ModelType = table.Column<int>(type: "integer", nullable: false),
                    CustomModelName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ApiKey = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ApiEndpoint = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    Temperature = table.Column<decimal>(type: "numeric(3,2)", precision: 3, scale: 2, nullable: true),
                    MaxTokens = table.Column<int>(type: "integer", nullable: true),
                    TopP = table.Column<decimal>(type: "numeric(3,2)", precision: 3, scale: 2, nullable: true),
                    FrequencyPenalty = table.Column<int>(type: "integer", nullable: true),
                    PresencePenalty = table.Column<int>(type: "integer", nullable: true),
                    RequestsPerMinute = table.Column<int>(type: "integer", nullable: true),
                    RequestsPerDay = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AIModelConfigurations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AIModelConfigurations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StockAnalyses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    StockHoldingId = table.Column<Guid>(type: "uuid", nullable: true),
                    AIModelConfigurationId = table.Column<Guid>(type: "uuid", nullable: true),
                    Symbol = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CompanyName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    AnalysisType = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    AnalysisPrompt = table.Column<string>(type: "text", nullable: true),
                    AnalysisResult = table.Column<string>(type: "text", nullable: true),
                    Summary = table.Column<string>(type: "text", nullable: true),
                    KeyInsights = table.Column<string>(type: "text", nullable: true),
                    Risks = table.Column<string>(type: "text", nullable: true),
                    Recommendation = table.Column<string>(type: "text", nullable: true),
                    ConfidenceScore = table.Column<decimal>(type: "numeric(3,2)", precision: 3, scale: 2, nullable: true),
                    TokensUsed = table.Column<int>(type: "integer", nullable: true),
                    AnalysisCost = table.Column<decimal>(type: "numeric(10,4)", precision: 10, scale: 4, nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true),
                    CachedUntil = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CacheHitCount = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockAnalyses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockAnalyses_AIModelConfigurations_AIModelConfigurationId",
                        column: x => x.AIModelConfigurationId,
                        principalTable: "AIModelConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_StockAnalyses_StockHoldings_StockHoldingId",
                        column: x => x.StockHoldingId,
                        principalTable: "StockHoldings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_StockAnalyses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AIModelConfigurations_UserId",
                table: "AIModelConfigurations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_StockAnalyses_AIModelConfigurationId",
                table: "StockAnalyses",
                column: "AIModelConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_StockAnalyses_StockHoldingId",
                table: "StockAnalyses",
                column: "StockHoldingId");

            migrationBuilder.CreateIndex(
                name: "IX_StockAnalyses_UserId",
                table: "StockAnalyses",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StockAnalyses");

            migrationBuilder.DropTable(
                name: "AIModelConfigurations");
        }
    }
}
