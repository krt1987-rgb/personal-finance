CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260121095824_InitialCreate') THEN
    CREATE TABLE "Users" (
        "Id" uuid NOT NULL,
        "Email" character varying(256) NOT NULL,
        "PasswordHash" text NOT NULL,
        "FirstName" character varying(100) NOT NULL,
        "LastName" character varying(100) NOT NULL,
        "PhoneNumber" text,
        "DateOfBirth" timestamp with time zone,
        "IsActive" boolean NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "UpdatedAt" timestamp with time zone,
        "CreatedBy" text,
        "UpdatedBy" text,
        "IsDeleted" boolean NOT NULL,
        CONSTRAINT "PK_Users" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260121095824_InitialCreate') THEN
    CREATE TABLE "BankAccounts" (
        "Id" uuid NOT NULL,
        "UserId" uuid NOT NULL,
        "BankName" text NOT NULL,
        "AccountNumber" text NOT NULL,
        "IFSC" text,
        "AccountType" integer NOT NULL,
        "CurrentBalance" numeric(18,2) NOT NULL,
        "Currency" text NOT NULL,
        "IsActive" boolean NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "UpdatedAt" timestamp with time zone,
        "CreatedBy" text,
        "UpdatedBy" text,
        "IsDeleted" boolean NOT NULL,
        CONSTRAINT "PK_BankAccounts" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_BankAccounts_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260121095824_InitialCreate') THEN
    CREATE TABLE "FamilyMembers" (
        "Id" uuid NOT NULL,
        "UserId" uuid NOT NULL,
        "FirstName" text NOT NULL,
        "LastName" text NOT NULL,
        "Relationship" integer NOT NULL,
        "DateOfBirth" timestamp with time zone,
        "Email" text,
        "PhoneNumber" text,
        "IsDependent" boolean NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "UpdatedAt" timestamp with time zone,
        "CreatedBy" text,
        "UpdatedBy" text,
        "IsDeleted" boolean NOT NULL,
        CONSTRAINT "PK_FamilyMembers" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_FamilyMembers_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260121095824_InitialCreate') THEN
    CREATE TABLE "MutualFundHoldings" (
        "Id" uuid NOT NULL,
        "UserId" uuid NOT NULL,
        "FolioNumber" text NOT NULL,
        "SchemeName" text NOT NULL,
        "AMC" text,
        "ISIN" text,
        "Category" integer NOT NULL,
        "Units" numeric(18,4) NOT NULL,
        "AverageNAV" numeric(18,4) NOT NULL,
        "CurrentNAV" numeric(18,4),
        "LastNAVUpdate" timestamp with time zone,
        "InvestmentMode" integer NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "UpdatedAt" timestamp with time zone,
        "CreatedBy" text,
        "UpdatedBy" text,
        "IsDeleted" boolean NOT NULL,
        CONSTRAINT "PK_MutualFundHoldings" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_MutualFundHoldings_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260121095824_InitialCreate') THEN
    CREATE TABLE "ProvidentFunds" (
        "Id" uuid NOT NULL,
        "UserId" uuid NOT NULL,
        "PFType" integer NOT NULL,
        "AccountNumber" text NOT NULL,
        "UAN" text,
        "EmployeeContribution" numeric(18,2) NOT NULL,
        "EmployerContribution" numeric(18,2) NOT NULL,
        "CurrentBalance" numeric(18,2) NOT NULL,
        "InterestRate" numeric(5,2) NOT NULL,
        "LastUpdatedBalance" timestamp with time zone,
        "Organization" text,
        "CreatedAt" timestamp with time zone NOT NULL,
        "UpdatedAt" timestamp with time zone,
        "CreatedBy" text,
        "UpdatedBy" text,
        "IsDeleted" boolean NOT NULL,
        CONSTRAINT "PK_ProvidentFunds" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_ProvidentFunds_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260121095824_InitialCreate') THEN
    CREATE TABLE "StockHoldings" (
        "Id" uuid NOT NULL,
        "UserId" uuid NOT NULL,
        "Symbol" text NOT NULL,
        "CompanyName" text NOT NULL,
        "Exchange" text NOT NULL,
        "Quantity" numeric(18,4) NOT NULL,
        "AverageBuyPrice" numeric(18,2) NOT NULL,
        "CurrentPrice" numeric(18,2),
        "LastPriceUpdate" timestamp with time zone,
        "ISIN" text,
        "Sector" text,
        "CreatedAt" timestamp with time zone NOT NULL,
        "UpdatedAt" timestamp with time zone,
        "CreatedBy" text,
        "UpdatedBy" text,
        "IsDeleted" boolean NOT NULL,
        CONSTRAINT "PK_StockHoldings" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_StockHoldings_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260121095824_InitialCreate') THEN
    CREATE TABLE "FixedDeposits" (
        "Id" uuid NOT NULL,
        "BankAccountId" uuid NOT NULL,
        "FDNumber" text NOT NULL,
        "PrincipalAmount" numeric(18,2) NOT NULL,
        "InterestRate" numeric(5,2) NOT NULL,
        "TenureInMonths" integer NOT NULL,
        "StartDate" timestamp with time zone NOT NULL,
        "MaturityDate" timestamp with time zone NOT NULL,
        "MaturityAmount" numeric(18,2) NOT NULL,
        "Status" integer NOT NULL,
        "Notes" text,
        "CreatedAt" timestamp with time zone NOT NULL,
        "UpdatedAt" timestamp with time zone,
        "CreatedBy" text,
        "UpdatedBy" text,
        "IsDeleted" boolean NOT NULL,
        CONSTRAINT "PK_FixedDeposits" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_FixedDeposits_BankAccounts_BankAccountId" FOREIGN KEY ("BankAccountId") REFERENCES "BankAccounts" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260121095824_InitialCreate') THEN
    CREATE TABLE "Transactions" (
        "Id" uuid NOT NULL,
        "BankAccountId" uuid NOT NULL,
        "TransactionDate" timestamp with time zone NOT NULL,
        "TransactionType" integer NOT NULL,
        "Category" text NOT NULL,
        "Amount" numeric(18,2) NOT NULL,
        "Description" text,
        "ReferenceNumber" text,
        "Balance" numeric(18,2) NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "UpdatedAt" timestamp with time zone,
        "CreatedBy" text,
        "UpdatedBy" text,
        "IsDeleted" boolean NOT NULL,
        CONSTRAINT "PK_Transactions" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_Transactions_BankAccounts_BankAccountId" FOREIGN KEY ("BankAccountId") REFERENCES "BankAccounts" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260121095824_InitialCreate') THEN
    CREATE TABLE "MutualFundTransactions" (
        "Id" uuid NOT NULL,
        "MutualFundHoldingId" uuid NOT NULL,
        "TransactionDate" timestamp with time zone NOT NULL,
        "TransactionType" integer NOT NULL,
        "Units" numeric(18,4) NOT NULL,
        "NAV" numeric(18,4) NOT NULL,
        "Amount" numeric(18,2) NOT NULL,
        "TransactionNumber" text,
        "Notes" text,
        "CreatedAt" timestamp with time zone NOT NULL,
        "UpdatedAt" timestamp with time zone,
        "CreatedBy" text,
        "UpdatedBy" text,
        "IsDeleted" boolean NOT NULL,
        CONSTRAINT "PK_MutualFundTransactions" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_MutualFundTransactions_MutualFundHoldings_MutualFundHolding~" FOREIGN KEY ("MutualFundHoldingId") REFERENCES "MutualFundHoldings" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260121095824_InitialCreate') THEN
    CREATE TABLE "PFTransactions" (
        "Id" uuid NOT NULL,
        "ProvidentFundId" uuid NOT NULL,
        "TransactionDate" timestamp with time zone NOT NULL,
        "TransactionType" integer NOT NULL,
        "EmployeeContribution" numeric(18,2) NOT NULL,
        "EmployerContribution" numeric(18,2) NOT NULL,
        "InterestCredited" numeric(18,2) NOT NULL,
        "ClosingBalance" numeric(18,2) NOT NULL,
        "Description" text,
        "CreatedAt" timestamp with time zone NOT NULL,
        "UpdatedAt" timestamp with time zone,
        "CreatedBy" text,
        "UpdatedBy" text,
        "IsDeleted" boolean NOT NULL,
        CONSTRAINT "PK_PFTransactions" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_PFTransactions_ProvidentFunds_ProvidentFundId" FOREIGN KEY ("ProvidentFundId") REFERENCES "ProvidentFunds" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260121095824_InitialCreate') THEN
    CREATE TABLE "StockTransactions" (
        "Id" uuid NOT NULL,
        "StockHoldingId" uuid NOT NULL,
        "TransactionDate" timestamp with time zone NOT NULL,
        "TransactionType" integer NOT NULL,
        "Quantity" numeric(18,4) NOT NULL,
        "Price" numeric(18,2) NOT NULL,
        "Brokerage" numeric(18,2) NOT NULL,
        "TotalAmount" numeric(18,2) NOT NULL,
        "Notes" text,
        "CreatedAt" timestamp with time zone NOT NULL,
        "UpdatedAt" timestamp with time zone,
        "CreatedBy" text,
        "UpdatedBy" text,
        "IsDeleted" boolean NOT NULL,
        CONSTRAINT "PK_StockTransactions" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_StockTransactions_StockHoldings_StockHoldingId" FOREIGN KEY ("StockHoldingId") REFERENCES "StockHoldings" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260121095824_InitialCreate') THEN
    CREATE INDEX "IX_BankAccounts_UserId" ON "BankAccounts" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260121095824_InitialCreate') THEN
    CREATE INDEX "IX_FamilyMembers_UserId" ON "FamilyMembers" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260121095824_InitialCreate') THEN
    CREATE INDEX "IX_FixedDeposits_BankAccountId" ON "FixedDeposits" ("BankAccountId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260121095824_InitialCreate') THEN
    CREATE INDEX "IX_MutualFundHoldings_UserId" ON "MutualFundHoldings" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260121095824_InitialCreate') THEN
    CREATE INDEX "IX_MutualFundTransactions_MutualFundHoldingId" ON "MutualFundTransactions" ("MutualFundHoldingId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260121095824_InitialCreate') THEN
    CREATE INDEX "IX_PFTransactions_ProvidentFundId" ON "PFTransactions" ("ProvidentFundId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260121095824_InitialCreate') THEN
    CREATE INDEX "IX_ProvidentFunds_UserId" ON "ProvidentFunds" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260121095824_InitialCreate') THEN
    CREATE INDEX "IX_StockHoldings_UserId" ON "StockHoldings" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260121095824_InitialCreate') THEN
    CREATE INDEX "IX_StockTransactions_StockHoldingId" ON "StockTransactions" ("StockHoldingId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260121095824_InitialCreate') THEN
    CREATE INDEX "IX_Transactions_BankAccountId" ON "Transactions" ("BankAccountId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260121095824_InitialCreate') THEN
    CREATE UNIQUE INDEX "IX_Users_Email" ON "Users" ("Email");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260121095824_InitialCreate') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260121095824_InitialCreate', '10.0.2');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260126185047_AddAIStockResearchFeature') THEN
    CREATE TABLE "AIModelConfigurations" (
        "Id" uuid NOT NULL,
        "UserId" uuid NOT NULL,
        "Name" character varying(200) NOT NULL,
        "ProviderType" integer NOT NULL,
        "ModelType" integer NOT NULL,
        "CustomModelName" character varying(200),
        "ApiKey" character varying(500) NOT NULL,
        "ApiEndpoint" character varying(500),
        "IsActive" boolean NOT NULL,
        "IsDefault" boolean NOT NULL,
        "Priority" integer NOT NULL,
        "Temperature" numeric(3,2),
        "MaxTokens" integer,
        "TopP" numeric(3,2),
        "FrequencyPenalty" integer,
        "PresencePenalty" integer,
        "RequestsPerMinute" integer,
        "RequestsPerDay" integer,
        "CreatedAt" timestamp with time zone NOT NULL,
        "UpdatedAt" timestamp with time zone,
        "CreatedBy" text,
        "UpdatedBy" text,
        "IsDeleted" boolean NOT NULL,
        CONSTRAINT "PK_AIModelConfigurations" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_AIModelConfigurations_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260126185047_AddAIStockResearchFeature') THEN
    CREATE TABLE "StockAnalyses" (
        "Id" uuid NOT NULL,
        "UserId" uuid NOT NULL,
        "StockHoldingId" uuid,
        "AIModelConfigurationId" uuid,
        "Symbol" character varying(20) NOT NULL,
        "CompanyName" character varying(200) NOT NULL,
        "AnalysisType" integer NOT NULL,
        "Status" integer NOT NULL,
        "AnalysisPrompt" text,
        "AnalysisResult" text,
        "Summary" text,
        "KeyInsights" text,
        "Risks" text,
        "Recommendation" text,
        "ConfidenceScore" numeric(3,2),
        "TokensUsed" integer,
        "AnalysisCost" numeric(10,4),
        "CompletedAt" timestamp with time zone,
        "ErrorMessage" text,
        "CachedUntil" timestamp with time zone,
        "CacheHitCount" integer NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "UpdatedAt" timestamp with time zone,
        "CreatedBy" text,
        "UpdatedBy" text,
        "IsDeleted" boolean NOT NULL,
        CONSTRAINT "PK_StockAnalyses" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_StockAnalyses_AIModelConfigurations_AIModelConfigurationId" FOREIGN KEY ("AIModelConfigurationId") REFERENCES "AIModelConfigurations" ("Id") ON DELETE SET NULL,
        CONSTRAINT "FK_StockAnalyses_StockHoldings_StockHoldingId" FOREIGN KEY ("StockHoldingId") REFERENCES "StockHoldings" ("Id") ON DELETE SET NULL,
        CONSTRAINT "FK_StockAnalyses_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260126185047_AddAIStockResearchFeature') THEN
    CREATE INDEX "IX_AIModelConfigurations_UserId" ON "AIModelConfigurations" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260126185047_AddAIStockResearchFeature') THEN
    CREATE INDEX "IX_StockAnalyses_AIModelConfigurationId" ON "StockAnalyses" ("AIModelConfigurationId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260126185047_AddAIStockResearchFeature') THEN
    CREATE INDEX "IX_StockAnalyses_StockHoldingId" ON "StockAnalyses" ("StockHoldingId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260126185047_AddAIStockResearchFeature') THEN
    CREATE INDEX "IX_StockAnalyses_UserId" ON "StockAnalyses" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260126185047_AddAIStockResearchFeature') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260126185047_AddAIStockResearchFeature', '10.0.2');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260126185759_FixPenaltyFieldsToDecimal') THEN
    ALTER TABLE "AIModelConfigurations" ALTER COLUMN "PresencePenalty" TYPE numeric;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260126185759_FixPenaltyFieldsToDecimal') THEN
    ALTER TABLE "AIModelConfigurations" ALTER COLUMN "FrequencyPenalty" TYPE numeric;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260126185759_FixPenaltyFieldsToDecimal') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260126185759_FixPenaltyFieldsToDecimal', '10.0.2');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260126192228_AddMCPServerConfiguration') THEN
    CREATE TABLE "MCPServerConfigurations" (
        "Id" uuid NOT NULL,
        "UserId" uuid NOT NULL,
        "Name" character varying(200) NOT NULL,
        "ProviderType" integer NOT NULL,
        "ApiEndpoint" character varying(500) NOT NULL,
        "ApiKey" character varying(500),
        "ConnectionStatus" integer NOT NULL,
        "IsActive" boolean NOT NULL,
        "IsDefault" boolean NOT NULL,
        "Priority" integer NOT NULL,
        "RequestsPerMinute" integer,
        "RequestsPerDay" integer,
        "LastConnectedAt" timestamp with time zone,
        "LastErrorMessage" character varying(1000),
        "SupportedDataTypes" character varying(200),
        "CreatedAt" timestamp with time zone NOT NULL,
        "UpdatedAt" timestamp with time zone,
        "CreatedBy" text,
        "UpdatedBy" text,
        "IsDeleted" boolean NOT NULL,
        CONSTRAINT "PK_MCPServerConfigurations" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_MCPServerConfigurations_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260126192228_AddMCPServerConfiguration') THEN
    CREATE INDEX "IX_MCPServerConfigurations_UserId" ON "MCPServerConfigurations" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260126192228_AddMCPServerConfiguration') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260126192228_AddMCPServerConfiguration', '10.0.2');
    END IF;
END $EF$;
COMMIT;

