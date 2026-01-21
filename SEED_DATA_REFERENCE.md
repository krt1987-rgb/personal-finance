# Seed Data Reference - Indian Portfolio

## Overview
The application includes comprehensive seed data with Indian financial context for testing and demonstration purposes. This document provides details about the seeded data, including login credentials, portfolio breakdown, and testing scenarios.

## User Credentials

### User 1 - Rajesh Kumar
- **Email**: rajesh.kumar@example.com
- **Password**: Password123!
- **Phone**: +91-98765-43210
- **Date of Birth**: March 15, 1985

### User 2 - Priya Sharma
- **Email**: priya.sharma@example.com
- **Password**: Password123!
- **Phone**: +91-98765-43211
- **Date of Birth**: July 22, 1990

## Portfolio Breakdown

### Rajesh Kumar's Portfolio (~₹8.4 Crores)

#### Bank Accounts
| Bank | Account Type | Balance (INR) |
|------|-------------|---------------|
| HDFC Bank | Savings | ₹20,75,000 |
| ICICI Bank | Current | ₹10,37,500 |
| **Total** | | **₹31,12,500** |

#### Fixed Deposits
| Bank | Principal (INR) | Interest Rate | Maturity Date | Maturity Amount (INR) | Status |
|------|-----------------|---------------|---------------|----------------------|--------|
| HDFC Bank | ₹41,50,000 | 7.25% | Jan 1, 2026 | ₹47,11,080 | Active |
| ICICI Bank | ₹83,00,000 | 7.5% | Jun 1, 2025 | ₹95,02,670 | Active |
| **Total** | **₹1,24,50,000** | | | **₹1,42,13,750** | |

#### Provident Funds
| Type | Account Number | Balance (INR) | Monthly Contribution (INR) | Interest Rate |
|------|---------------|---------------|---------------------------|---------------|
| EPF | EPF-1234567890 | ₹2,07,50,000 | ₹2,49,000 (Employee + Employer) | 8.25% |
| PPF | PPF-0987654321 | ₹3,73,50,000 | ₹10,37,500 | 7.1% |
| **Total** | | **₹5,81,00,000** | | |

#### Stock Holdings (NSE)
| Symbol | Company | Quantity | Avg Buy Price (₹) | Current Price (₹) | Invested (₹) | Current Value (₹) | Profit/Loss (₹) | ROI |
|--------|---------|----------|-------------------|-------------------|--------------|-------------------|-----------------|-----|
| RELIANCE | Reliance Industries | 150 | 2,450.75 | 2,875.50 | 3,67,612 | 4,31,325 | +63,713 | +17.3% |
| TCS | Tata Consultancy Services | 100 | 3,280.50 | 3,965.20 | 3,28,050 | 3,96,520 | +68,470 | +20.9% |
| INFY | Infosys Ltd | 200 | 1,425.25 | 1,678.80 | 2,85,050 | 3,35,760 | +50,710 | +17.8% |
| **Total** | | **450** | | | **₹9,80,712** | **₹11,63,605** | **+₹1,82,893** | **+18.7%** |

#### Mutual Fund Holdings (Indian Funds)
| Fund | Fund House | Type | Units | NAV (₹) | Current NAV (₹) | Invested (₹) | Current Value (₹) | Profit/Loss (₹) | ROI |
|------|------------|------|-------|---------|-----------------|--------------|-------------------|-----------------|-----|
| SBI Bluechip Fund | SBI MF | Equity | 2,500 | 85.00 | 96.50 | 2,12,500 | 2,41,250 | +28,750 | +13.5% |
| HDFC Balanced Advantage | HDFC MF | Hybrid | 1,800 | 225.00 | 258.75 | 4,05,000 | 4,65,750 | +60,750 | +15.0% |
| **Total** | | | **4,300** | | | **₹6,17,500** | **₹7,07,000** | **+₹89,500** | **+14.5%** |

#### Total Net Worth
- Bank Accounts: ₹31,12,500
- Fixed Deposits: ₹1,24,50,000 (Principal)
- Provident Funds: ₹5,81,00,000
- Stock Holdings: ₹11,63,605
- Mutual Funds: ₹7,07,000
- **Total: ₹8,55,33,105 (~₹8.55 Crores)**

---

### Priya Sharma's Portfolio (~₹2.28 Crores)

#### Bank Accounts
| Bank | Account Type | Balance (INR) |
|------|-------------|---------------|
| State Bank of India | Savings | ₹29,05,000 |

#### Provident Funds
| Type | Balance (INR) | Monthly Contribution (INR) |
|------|---------------|---------------------------|
| NPS | ₹1,49,40,000 | ₹1,66,000 |

#### Stock Holdings (NSE)
| Symbol | Company | Quantity | Avg Buy Price (₹) | Current Price (₹) | Invested (₹) | Current Value (₹) | Profit/Loss (₹) | ROI |
|--------|---------|----------|-------------------|-------------------|--------------|-------------------|-----------------|-----|
| HDFCBANK | HDFC Bank | 120 | 1,580.00 | 1,745.50 | 1,89,600 | 2,09,460 | +19,860 | +10.5% |
| WIPRO | Wipro Ltd | 300 | 425.50 | 498.35 | 1,27,650 | 1,49,505 | +21,855 | +17.1% |
| **Total** | | **420** | | | **₹3,17,250** | **₹3,58,965** | **+₹41,715** | **+13.1%** |

#### Mutual Fund Holdings
| Fund | Fund House | Type | Units | Current Value (INR) |
|------|------------|------|-------|---------------------|
| ICICI Prudential Technology | ICICI Prudential | Equity | 3,000 | ₹5,67,750 |

#### Total Net Worth
- Bank Accounts: ₹29,05,000
- Provident Funds: ₹1,49,40,000
- Stock Holdings: ₹3,58,965
- Mutual Funds: ₹5,67,750
- **Total: ₹2,27,71,715 (~₹2.28 Crores)**

## Transaction History

### Sample Transactions Seeded (in INR)
- Income transactions (salary credits: ₹4.15L and ₹4.98L)
- Expense transactions (rent, groceries, utilities)
- Stock purchase transactions with fees
- Mutual fund SIP and lump sum investments

## Testing Scenarios

### 1. Authentication Testing
```bash
# Login as Rajesh Kumar
POST /api/auth/login
{
  "email": "rajesh.kumar@example.com",
  "password": "Password123!"
}

# Login as Priya Sharma
POST /api/auth/login
{
  "email": "priya.sharma@example.com",
  "password": "Password123!"
}
```

### 2. Portfolio Summary Testing
```bash
# Get stock portfolio summary
GET /api/stockholdings/portfolio-summary
Authorization: Bearer {token}

# Get mutual fund portfolio summary
GET /api/mutualfundholdings/portfolio-summary
Authorization: Bearer {token}

# Get PF summary
GET /api/providentfunds/summary
Authorization: Bearer {token}

# Get FD summary
GET /api/fixeddeposits/summary
Authorization: Bearer {token}
```

### 3. Profit/Loss Calculations
The seeded data demonstrates:
- Stock investments showing +18.7% average ROI (Rajesh) and +13.1% (Priya)
- Mutual funds showing +14.5% ROI (Rajesh) and +14.7% (Priya)
- Fixed deposits with competitive Indian interest rates (7.25% - 7.5%)
- Provident funds with standard EPF (8.25%), PPF (7.1%), and NPS (9.5%) rates

### 4. Multi-User Data Isolation
Test that:
- Rajesh Kumar can only see his 3 stock holdings
- Priya Sharma can only see her 2 stock holdings
- Family member relationships are properly linked
- Bank accounts are user-specific

## Database Migration

To apply the seed data:

```bash
# Using EF Core CLI
dotnet ef database update --project src/PersonalFinance.Infrastructure

# Or using the API endpoint
POST /api/database/migrate
```

## Notes

- All passwords are hashed using SHA256 (Hash: 8C6976E5B5410415BDE908BD4DEE15DFB167A9C873FC4BB8A81F6F2AB448A918)
- GUIDs are predefined for consistency in testing
- Dates use UTC timezone
- Soft delete is set to `false` for all records
- All financial amounts use INR currency
- Decimal precision: 18,2 for amounts, 18,4 for units/shares
- Stock prices are from NSE (National Stock Exchange of India)
- Mutual fund data uses popular Indian fund houses (SBI MF, HDFC MF, ICICI Prudential)
- Interest rates match current Indian market rates

## Family Structure

**Rajesh Kumar's Family:**
- Spouse: Anjali Kumar (May 20, 1987)
- Child: Arjun Kumar (September 10, 2015)

This family structure allows testing of:
- Family member management
- Relationships tracking
- Multi-member financial planning

## Indian Financial Context

**Banks**: HDFC Bank, ICICI Bank, State Bank of India
**Stock Exchange**: NSE (National Stock Exchange)
**Popular Stocks**: Reliance, TCS, Infosys, HDFC Bank, Wipro
**Mutual Funds**: SBI Bluechip Fund, HDFC Balanced Advantage, ICICI Prudential Technology Fund
**Provident Funds**: EPF (8.25%), PPF (7.1%), NPS (9.5%)
**FD Interest Rates**: 7.25% - 7.5% p.a.
