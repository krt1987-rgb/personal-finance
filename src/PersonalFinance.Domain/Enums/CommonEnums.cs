namespace PersonalFinance.Domain.Enums;

public enum RelationshipType
{
    Spouse,
    Child,
    Parent,
    Sibling,
    GrandParent,
    GrandChild,
    Other
}

public enum AccountType
{
    Savings,
    Current,
    Salary,
    FixedDeposit,
    RecurringDeposit
}

public enum FDStatus
{
    Active,
    Matured,
    PrematureClosed,
    Renewed
}

public enum PFType
{
    EPF,        // Employee Provident Fund
    PPF,        // Public Provident Fund
    VPF,        // Voluntary Provident Fund
    NPS         // National Pension System
}

public enum TransactionType
{
    Credit,
    Debit
}

public enum StockTransactionType
{
    Buy,
    Sell,
    Dividend,
    Bonus,
    Split
}

public enum MutualFundCategory
{
    Equity,
    Debt,
    Hybrid,
    SolutionOriented,
    Other
}

public enum InvestmentMode
{
    Lumpsum,
    SIP
}

public enum MutualFundTransactionType
{
    Purchase,
    Redemption,
    SwitchIn,
    SwitchOut,
    Dividend
}
