# Data Import Templates

This directory contains sample CSV templates for importing data into the Personal Finance Management System.

## Supported File Formats

- **CSV** (.csv) - Comma-separated values
- **Excel** (.xlsx, .xls) - Microsoft Excel spreadsheets

## Available Templates

### 1. Stock Holdings (`stock-holdings-sample.csv`)
**Columns:**
- `Symbol` - Stock symbol (e.g., RELIANCE, TCS)
- `CompanyName` - Full company name
- `Exchange` - Stock exchange (NSE, BSE)
- `Quantity` - Number of shares
- `AverageBuyPrice` - Average purchase price per share
- `CurrentPrice` - Current market price (optional)
- `ISIN` - International Securities Identification Number (optional)
- `Sector` - Industry sector (optional)

### 2. Bank Accounts (`bank-accounts-sample.csv`)
**Columns:**
- `BankName` - Name of the bank
- `AccountNumber` - Bank account number
- `IFSC` - IFSC code (optional)
- `AccountType` - Type of account (Savings, Current, Salary, FixedDeposit, RecurringDeposit)
- `CurrentBalance` - Current balance in the account
- `Currency` - Currency code (default: INR)
- `IsActive` - Account active status (true/false, default: true)

### 3. Fixed Deposits (`fixed-deposits-sample.csv`)
**Columns:**
- `BankAccountNumber` - Associated bank account number (must exist in Bank Accounts)
- `FDNumber` - Fixed deposit number
- `PrincipalAmount` - Principal deposit amount
- `InterestRate` - Annual interest rate (%)
- `TenureInMonths` - Duration in months
- `StartDate` - Start date (YYYY-MM-DD format)
- `MaturityDate` - Maturity date (YYYY-MM-DD format)
- `MaturityAmount` - Maturity amount
- `Status` - Status (Active, Matured, PrematureClosed, Renewed)

### 4. Mutual Fund Holdings (`mutual-fund-holdings-sample.csv`)
**Columns:**
- `FolioNumber` - Folio number
- `SchemeName` - Mutual fund scheme name
- `AMC` - Asset Management Company (optional)
- `ISIN` - ISIN code (optional)
- `Category` - Category (Equity, Debt, Hybrid, SolutionOriented, Other)
- `Units` - Number of units
- `AverageNAV` - Average NAV at purchase
- `CurrentNAV` - Current NAV (optional)
- `InvestmentMode` - Investment mode (Lumpsum, SIP)

### 5. Provident Funds (`provident-funds-sample.csv`)
**Columns:**
- `AccountNumber` - PF account number
- `PFType` - Type (EPF, PPF, VPF, NPS)
- `EmployeeContribution` - Employee contribution amount
- `EmployerContribution` - Employer contribution amount
- `InterestRate` - Annual interest rate (%)
- `CurrentBalance` - Current balance
- `LastUpdatedBalance` - Last update date (YYYY-MM-DD format, optional)

### 6. Family Members (`family-members-sample.csv`)
**Columns:**
- `FirstName` - First name
- `LastName` - Last name
- `Relationship` - Relationship (Spouse, Child, Parent, Sibling, GrandParent, GrandChild, Other)
- `DateOfBirth` - Date of birth (YYYY-MM-DD format, optional)
- `Email` - Email address (optional)
- `PhoneNumber` - Phone number (optional)
- `IsDependent` - Dependent status (true/false, default: false)

## How to Use

1. Download the appropriate sample template
2. Fill in your data following the format shown in the sample
3. Save the file as CSV or Excel
4. In the application, navigate to the relevant module
5. Click the "Import" button
6. Select your file and upload
7. Review the import results

## Important Notes

- **Date Format:** Use YYYY-MM-DD format for all dates (e.g., 2024-01-15)
- **Decimal Numbers:** Use decimal point (.) not comma (,) for decimal values
- **Boolean Values:** Use true/false for boolean fields
- **Required Fields:** Fields without "(optional)" are required
- **Case Sensitivity:** Enum values are case-insensitive (e.g., "savings" = "Savings")
- **Header Row:** The first row must contain column headers exactly as specified
- **Empty Values:** Optional fields can be left empty
- **Character Encoding:** Use UTF-8 encoding for CSV files

## Validation

The import process validates:
- Data types (numbers, dates, booleans)
- Required fields
- Enum values
- Foreign key relationships (e.g., BankAccountNumber in Fixed Deposits)

If validation fails, the system will:
- Show the number of successful and failed imports
- Display detailed error messages for each failed row
- Allow you to fix the errors and re-import

## Support

For issues or questions about importing data, please refer to the application documentation or contact support.
