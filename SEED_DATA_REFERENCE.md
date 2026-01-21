# Seed Data Reference

This document provides details about the sample data seeded into the database for testing and visualization purposes.

## Overview

The `SeedSampleData` migration includes realistic financial data across all modules to help you visualize and test the application.

## Users

Two sample users are created with the following credentials:

| User | Email | Password | Role | Description |
|------|-------|----------|------|-------------|
| John Doe | john.doe@example.com | Password123! | Admin | Primary admin user with full portfolio |
| Jane Smith | jane.smith@example.com | Password123! | User | Standard user with basic holdings |

**Note**: Password is hashed using SHA256. For production, use BCrypt or Argon2.

## Family Members

| Name | Relationship | Belongs To | Age Group |
|------|--------------|------------|-----------|
| Emily Doe | Spouse | John Doe | Adult |
| Michael Doe | Child | John Doe | Minor |

## Bank Accounts

### John Doe's Accounts
- **Chase Bank** - Savings Account (****1234): $25,000.00
- **Bank of America** - Checking Account (****5678): $12,500.00

### Jane Smith's Accounts
- **Wells Fargo** - Savings Account (****9012): $35,000.00

**Total Banking Assets**: $72,500.00

## Fixed Deposits

| Bank | Account | Principal | Rate | Start Date | Maturity Date | Maturity Amount | Owner |
|------|---------|-----------|------|------------|---------------|-----------------|-------|
| Chase Bank | FD-2024-001 | $50,000 | 6.5% | Jan 1, 2024 | Jan 1, 2026 | $56,760 | John Doe |
| HDFC Bank | FD-2023-042 | $100,000 | 7.0% | Jun 1, 2023 | Jun 1, 2025 | $114,490 | John Doe |

**Total FD Principal**: $150,000.00  
**Expected Maturity Value**: $171,250.00

## Provident Funds

| Type | Account Number | Balance | Monthly Contribution | Interest Rate | Owner |
|------|----------------|---------|----------------------|---------------|-------|
| EPF | EPF-1234567890 | $250,000 | Employee: $1,500, Employer: $1,500 | 8.25% | John Doe |
| PPF | PPF-0987654321 | $450,000 | $12,500 | 7.1% | John Doe |
| NPS | NPS-1122334455 | $180,000 | Employee: $1,000, Employer: $1,000 | 9.5% | Jane Smith |

**Total PF Balance**: $880,000.00

## Stock Portfolio

### John Doe's Holdings

| Symbol | Company | Shares | Avg Buy Price | Current Price | Invested | Current Value | P&L | Sector |
|--------|---------|--------|---------------|---------------|----------|---------------|-----|---------|
| AAPL | Apple Inc. | 50 | $150.25 | $185.50 | $7,512.50 | $9,275.00 | **+$1,762.50** | Technology |
| MSFT | Microsoft Corp. | 30 | $290.75 | $380.20 | $8,722.50 | $11,406.00 | **+$2,683.50** | Technology |
| GOOGL | Alphabet Inc. | 25 | $120.50 | $142.80 | $3,012.50 | $3,570.00 | **+$557.50** | Technology |

**John's Total**: Invested $19,247.50 → Current $24,251.00 → **Profit: +$5,003.50 (+26.0%)**

### Jane Smith's Holdings

| Symbol | Company | Shares | Avg Buy Price | Current Price | Invested | Current Value | P&L | Sector |
|--------|---------|--------|---------------|---------------|----------|---------------|-----|---------|
| TSLA | Tesla Inc. | 40 | $210.00 | $248.50 | $8,400.00 | $9,940.00 | **+$1,540.00** | Automotive |
| AMZN | Amazon.com | 15 | $135.25 | $178.35 | $2,028.75 | $2,675.25 | **+$646.50** | E-commerce |

**Jane's Total**: Invested $10,428.75 → Current $12,615.25 → **Profit: +$2,186.50 (+21.0%)**

**Overall Stock Portfolio**: $29,676.25 invested → $36,866.25 current → **+$7,190.00 profit (+24.2%)**

## Mutual Fund Portfolio

### John Doe's Holdings

| Fund Name | Fund House | Type | Units | Invested NAV | Current NAV | Invested | Current Value | Returns |
|-----------|------------|------|-------|--------------|-------------|----------|---------------|---------|
| Vanguard 500 Index Fund | Vanguard | Equity | 500 | $85.00 | $95.50 | $42,500 | $47,750 | **+$5,250 (+12.4%)** |
| Fidelity Contrafund | Fidelity | Equity | 300 | $125.00 | $142.75 | $37,500 | $42,825 | **+$5,325 (+14.2%)** |

### Jane Smith's Holdings

| Fund Name | Fund House | Type | Units | Invested NAV | Current NAV | Invested | Current Value | Returns |
|-----------|------------|------|-------|--------------|-------------|----------|---------------|---------|
| BlackRock Global Allocation | BlackRock | Balanced | 400 | $110.00 | $118.25 | $44,000 | $47,300 | **+$3,300 (+7.5%)** |

**Total MF Portfolio**: $124,000 invested → $137,875 current → **+$13,875 profit (+11.2%)**

## Transaction History

Sample transactions showing income and expenses:

### Income
- John Doe: $5,000 (Salary) - Jan 1, 2024
- Jane Smith: $6,000 (Salary) - Jan 1, 2024

### Expenses
- John Doe: $1,200 (Rent) - Jan 5, 2024
- John Doe: $500 (Groceries) - Jan 10, 2024
- Jane Smith: $800 (Utilities) - Jan 12, 2024

## Portfolio Summary

### John Doe's Total Wealth
- Bank Accounts: $37,500
- Fixed Deposits: $150,000 (Principal)
- Provident Funds: $700,000 (EPF + PPF)
- Stocks: $24,251
- Mutual Funds: $90,575
- **Total Net Worth**: ~$1,002,326

### Jane Smith's Total Wealth
- Bank Accounts: $35,000
- Provident Funds: $180,000 (NPS)
- Stocks: $12,615
- Mutual Funds: $47,300
- **Total Net Worth**: ~$274,915

## Using the Seed Data

### Apply the Migration

**Option 1: Using API (Recommended)**
```bash
# Start the API
cd src/PersonalFinance.API
dotnet run

# Apply migrations via API endpoint
curl -X POST http://localhost:5000/api/database/migrate
```

**Option 2: Using CLI**
```bash
cd src/PersonalFinance.API
dotnet ef database update --project ../PersonalFinance.Infrastructure
```

### Login Credentials

Use these credentials to test the application:

```
Admin User:
Email: john.doe@example.com
Password: Password123!

Regular User:
Email: jane.smith@example.com
Password: Password123!
```

### Testing Features

With this seed data, you can test:
- ✅ User authentication and authorization
- ✅ Portfolio visualization (stocks showing gains)
- ✅ Mutual fund performance tracking
- ✅ Fixed deposit maturity calculations
- ✅ Provident fund summaries by type
- ✅ Family member management
- ✅ Transaction history and categorization
- ✅ Multi-account banking overview
- ✅ Wealth dashboard with real numbers

## Data Relationships

The seed data demonstrates:
- One-to-many: User → Bank Accounts, Stock Holdings, MF Holdings
- Foreign keys: All transactions linked to appropriate holdings
- Soft delete support: All records have `IsDeleted = false`
- Audit trail: All records have `CreatedAt`, `CreatedBy` populated
- Realistic calculations: Current prices showing actual market gains/losses

## Notes

- All dates use UTC timezone
- Currency is in USD
- Stock prices are realistic but fictional
- Password hash is simplified for demo (use BCrypt in production)
- GUIDs are predefined for consistency and easy testing
- Interest rates and maturity calculations are realistic
- Portfolio shows actual profit/loss scenarios
