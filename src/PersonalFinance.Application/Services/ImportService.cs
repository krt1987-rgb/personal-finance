using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using OfficeOpenXml;
using PersonalFinance.Application.DTOs;
using PersonalFinance.Application.Interfaces;
using PersonalFinance.Domain.Entities;
using PersonalFinance.Domain.Enums;
using PersonalFinance.Domain.Interfaces;

namespace PersonalFinance.Application.Services;

public class ImportService : IImportService
{
    private readonly IRepository<StockHolding> _stockRepository;
    private readonly IRepository<BankAccount> _bankAccountRepository;
    private readonly IRepository<FixedDeposit> _fixedDepositRepository;
    private readonly IRepository<MutualFundHolding> _mutualFundRepository;
    private readonly IRepository<ProvidentFund> _providentFundRepository;
    private readonly IRepository<FamilyMember> _familyMemberRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ImportService(
        IRepository<StockHolding> stockRepository,
        IRepository<BankAccount> bankAccountRepository,
        IRepository<FixedDeposit> fixedDepositRepository,
        IRepository<MutualFundHolding> mutualFundRepository,
        IRepository<ProvidentFund> providentFundRepository,
        IRepository<FamilyMember> familyMemberRepository,
        IUnitOfWork unitOfWork)
    {
        _stockRepository = stockRepository;
        _bankAccountRepository = bankAccountRepository;
        _fixedDepositRepository = fixedDepositRepository;
        _mutualFundRepository = mutualFundRepository;
        _providentFundRepository = providentFundRepository;
        _familyMemberRepository = familyMemberRepository;
        _unitOfWork = unitOfWork;
        
        // Set EPPlus license context - suppress obsolete warning as the new API is not available
        #pragma warning disable CS0618
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        #pragma warning restore CS0618
    }

    public async Task<ImportResultDto> ImportStockHoldingsAsync(Stream fileStream, string fileName, Guid userId)
    {
        var result = new ImportResultDto();
        var records = new List<StockHoldingImportModel>();

        try
        {
            if (fileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                records = ReadCsv<StockHoldingImportModel>(fileStream);
            }
            else if (fileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase) || 
                     fileName.EndsWith(".xls", StringComparison.OrdinalIgnoreCase))
            {
                records = ReadExcel<StockHoldingImportModel>(fileStream);
            }
            else
            {
                result.Errors.Add(new ImportErrorDto { RowNumber = 0, Error = "Unsupported file format. Only CSV and Excel files are supported." });
                return result;
            }

            result.TotalRows = records.Count;

            for (int i = 0; i < records.Count; i++)
            {
                try
                {
                    var record = records[i];
                    var holding = new StockHolding
                    {
                        UserId = userId,
                        Symbol = record.Symbol ?? string.Empty,
                        CompanyName = record.CompanyName ?? string.Empty,
                        Exchange = record.Exchange ?? "NSE",
                        Quantity = record.Quantity,
                        AverageBuyPrice = record.AverageBuyPrice,
                        CurrentPrice = record.CurrentPrice,
                        ISIN = record.ISIN,
                        Sector = record.Sector
                    };

                    await _stockRepository.AddAsync(holding);
                    result.SuccessCount++;
                }
                catch (Exception ex)
                {
                    result.FailureCount++;
                    result.Errors.Add(new ImportErrorDto
                    {
                        RowNumber = i + 2, // +2 for header row and 1-based indexing
                        Error = ex.Message
                    });
                }
            }

            if (result.SuccessCount > 0)
            {
                await _unitOfWork.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            result.Errors.Add(new ImportErrorDto { RowNumber = 0, Error = $"File processing error: {ex.Message}" });
        }

        return result;
    }

    public async Task<ImportResultDto> ImportBankAccountsAsync(Stream fileStream, string fileName, Guid userId)
    {
        var result = new ImportResultDto();
        var records = new List<BankAccountImportModel>();

        try
        {
            if (fileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                records = ReadCsv<BankAccountImportModel>(fileStream);
            }
            else if (fileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase) || 
                     fileName.EndsWith(".xls", StringComparison.OrdinalIgnoreCase))
            {
                records = ReadExcel<BankAccountImportModel>(fileStream);
            }
            else
            {
                result.Errors.Add(new ImportErrorDto { RowNumber = 0, Error = "Unsupported file format. Only CSV and Excel files are supported." });
                return result;
            }

            result.TotalRows = records.Count;

            for (int i = 0; i < records.Count; i++)
            {
                try
                {
                    var record = records[i];
                    var account = new BankAccount
                    {
                        UserId = userId,
                        BankName = record.BankName ?? string.Empty,
                        AccountNumber = record.AccountNumber ?? string.Empty,
                        IFSC = record.IFSC,
                        AccountType = ParseEnum<AccountType>(record.AccountType, AccountType.Savings),
                        CurrentBalance = record.CurrentBalance,
                        Currency = record.Currency ?? "INR",
                        IsActive = record.IsActive ?? true
                    };

                    await _bankAccountRepository.AddAsync(account);
                    result.SuccessCount++;
                }
                catch (Exception ex)
                {
                    result.FailureCount++;
                    result.Errors.Add(new ImportErrorDto
                    {
                        RowNumber = i + 2,
                        Error = ex.Message
                    });
                }
            }

            if (result.SuccessCount > 0)
            {
                await _unitOfWork.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            result.Errors.Add(new ImportErrorDto { RowNumber = 0, Error = $"File processing error: {ex.Message}" });
        }

        return result;
    }

    public async Task<ImportResultDto> ImportFixedDepositsAsync(Stream fileStream, string fileName, Guid userId)
    {
        var result = new ImportResultDto();
        var records = new List<FixedDepositImportModel>();

        try
        {
            if (fileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                records = ReadCsv<FixedDepositImportModel>(fileStream);
            }
            else if (fileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase) || 
                     fileName.EndsWith(".xls", StringComparison.OrdinalIgnoreCase))
            {
                records = ReadExcel<FixedDepositImportModel>(fileStream);
            }
            else
            {
                result.Errors.Add(new ImportErrorDto { RowNumber = 0, Error = "Unsupported file format. Only CSV and Excel files are supported." });
                return result;
            }

            result.TotalRows = records.Count;

            for (int i = 0; i < records.Count; i++)
            {
                try
                {
                    var record = records[i];
                    
                    // Get bank account by account number
                    var bankAccount = (await _bankAccountRepository.FindAsync(
                        ba => ba.UserId == userId && ba.AccountNumber == record.BankAccountNumber)).FirstOrDefault();
                    
                    if (bankAccount == null)
                    {
                        throw new Exception($"Bank account {record.BankAccountNumber} not found");
                    }

                    var fd = new FixedDeposit
                    {
                        BankAccountId = bankAccount.Id,
                        FDNumber = record.FDNumber ?? string.Empty,
                        PrincipalAmount = record.PrincipalAmount,
                        InterestRate = record.InterestRate,
                        StartDate = record.StartDate,
                        MaturityDate = record.MaturityDate,
                        TenureInMonths = record.TenureInMonths,
                        MaturityAmount = record.MaturityAmount,
                        Status = ParseEnum<FDStatus>(record.Status, FDStatus.Active)
                    };

                    await _fixedDepositRepository.AddAsync(fd);
                    result.SuccessCount++;
                }
                catch (Exception ex)
                {
                    result.FailureCount++;
                    result.Errors.Add(new ImportErrorDto
                    {
                        RowNumber = i + 2,
                        Error = ex.Message
                    });
                }
            }

            if (result.SuccessCount > 0)
            {
                await _unitOfWork.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            result.Errors.Add(new ImportErrorDto { RowNumber = 0, Error = $"File processing error: {ex.Message}" });
        }

        return result;
    }

    public async Task<ImportResultDto> ImportMutualFundHoldingsAsync(Stream fileStream, string fileName, Guid userId)
    {
        var result = new ImportResultDto();
        var records = new List<MutualFundImportModel>();

        try
        {
            if (fileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                records = ReadCsv<MutualFundImportModel>(fileStream);
            }
            else if (fileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase) || 
                     fileName.EndsWith(".xls", StringComparison.OrdinalIgnoreCase))
            {
                records = ReadExcel<MutualFundImportModel>(fileStream);
            }
            else
            {
                result.Errors.Add(new ImportErrorDto { RowNumber = 0, Error = "Unsupported file format. Only CSV and Excel files are supported." });
                return result;
            }

            result.TotalRows = records.Count;

            for (int i = 0; i < records.Count; i++)
            {
                try
                {
                    var record = records[i];
                    var holding = new MutualFundHolding
                    {
                        UserId = userId,
                        FolioNumber = record.FolioNumber ?? string.Empty,
                        SchemeName = record.SchemeName ?? string.Empty,
                        AMC = record.AMC,
                        ISIN = record.ISIN,
                        Category = ParseEnum<MutualFundCategory>(record.Category, MutualFundCategory.Other),
                        Units = record.Units,
                        AverageNAV = record.AverageNAV,
                        CurrentNAV = record.CurrentNAV,
                        InvestmentMode = ParseEnum<InvestmentMode>(record.InvestmentMode, InvestmentMode.Lumpsum)
                    };

                    await _mutualFundRepository.AddAsync(holding);
                    result.SuccessCount++;
                }
                catch (Exception ex)
                {
                    result.FailureCount++;
                    result.Errors.Add(new ImportErrorDto
                    {
                        RowNumber = i + 2,
                        Error = ex.Message
                    });
                }
            }

            if (result.SuccessCount > 0)
            {
                await _unitOfWork.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            result.Errors.Add(new ImportErrorDto { RowNumber = 0, Error = $"File processing error: {ex.Message}" });
        }

        return result;
    }

    public async Task<ImportResultDto> ImportProvidentFundsAsync(Stream fileStream, string fileName, Guid userId)
    {
        var result = new ImportResultDto();
        var records = new List<ProvidentFundImportModel>();

        try
        {
            if (fileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                records = ReadCsv<ProvidentFundImportModel>(fileStream);
            }
            else if (fileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase) || 
                     fileName.EndsWith(".xls", StringComparison.OrdinalIgnoreCase))
            {
                records = ReadExcel<ProvidentFundImportModel>(fileStream);
            }
            else
            {
                result.Errors.Add(new ImportErrorDto { RowNumber = 0, Error = "Unsupported file format. Only CSV and Excel files are supported." });
                return result;
            }

            result.TotalRows = records.Count;

            for (int i = 0; i < records.Count; i++)
            {
                try
                {
                    var record = records[i];
                    var pf = new ProvidentFund
                    {
                        UserId = userId,
                        AccountNumber = record.AccountNumber ?? string.Empty,
                        PFType = ParseEnum<PFType>(record.PFType, PFType.EPF),
                        EmployeeContribution = record.EmployeeContribution ?? 0,
                        EmployerContribution = record.EmployerContribution ?? 0,
                        InterestRate = record.InterestRate,
                        CurrentBalance = record.CurrentBalance,
                        LastUpdatedBalance = record.LastUpdatedBalance ?? DateTime.UtcNow
                    };

                    await _providentFundRepository.AddAsync(pf);
                    result.SuccessCount++;
                }
                catch (Exception ex)
                {
                    result.FailureCount++;
                    result.Errors.Add(new ImportErrorDto
                    {
                        RowNumber = i + 2,
                        Error = ex.Message
                    });
                }
            }

            if (result.SuccessCount > 0)
            {
                await _unitOfWork.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            result.Errors.Add(new ImportErrorDto { RowNumber = 0, Error = $"File processing error: {ex.Message}" });
        }

        return result;
    }

    public async Task<ImportResultDto> ImportFamilyMembersAsync(Stream fileStream, string fileName, Guid userId)
    {
        var result = new ImportResultDto();
        var records = new List<FamilyMemberImportModel>();

        try
        {
            if (fileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                records = ReadCsv<FamilyMemberImportModel>(fileStream);
            }
            else if (fileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase) || 
                     fileName.EndsWith(".xls", StringComparison.OrdinalIgnoreCase))
            {
                records = ReadExcel<FamilyMemberImportModel>(fileStream);
            }
            else
            {
                result.Errors.Add(new ImportErrorDto { RowNumber = 0, Error = "Unsupported file format. Only CSV and Excel files are supported." });
                return result;
            }

            result.TotalRows = records.Count;

            for (int i = 0; i < records.Count; i++)
            {
                try
                {
                    var record = records[i];
                    var member = new FamilyMember
                    {
                        UserId = userId,
                        FirstName = record.FirstName ?? string.Empty,
                        LastName = record.LastName ?? string.Empty,
                        Relationship = ParseEnum<RelationshipType>(record.Relationship, RelationshipType.Other),
                        DateOfBirth = record.DateOfBirth,
                        Email = record.Email,
                        PhoneNumber = record.PhoneNumber,
                        IsDependent = record.IsDependent ?? false
                    };

                    await _familyMemberRepository.AddAsync(member);
                    result.SuccessCount++;
                }
                catch (Exception ex)
                {
                    result.FailureCount++;
                    result.Errors.Add(new ImportErrorDto
                    {
                        RowNumber = i + 2,
                        Error = ex.Message
                    });
                }
            }

            if (result.SuccessCount > 0)
            {
                await _unitOfWork.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            result.Errors.Add(new ImportErrorDto { RowNumber = 0, Error = $"File processing error: {ex.Message}" });
        }

        return result;
    }

    // Helper methods
    private List<T> ReadCsv<T>(Stream stream)
    {
        using var reader = new StreamReader(stream);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HeaderValidated = null,
            MissingFieldFound = null
        });
        return csv.GetRecords<T>().ToList();
    }

    private List<T> ReadExcel<T>(Stream stream) where T : new()
    {
        var records = new List<T>();
        using var package = new ExcelPackage(stream);
        var worksheet = package.Workbook.Worksheets[0];
        
        if (worksheet.Dimension == null)
            return records;

        var properties = typeof(T).GetProperties();
        var headerRow = 1;
        var columnMapping = new Dictionary<int, string>();

        // Map columns based on header row
        for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
        {
            var headerValue = worksheet.Cells[headerRow, col].Value?.ToString();
            if (!string.IsNullOrEmpty(headerValue))
            {
                columnMapping[col] = headerValue;
            }
        }

        // Read data rows
        for (int row = headerRow + 1; row <= worksheet.Dimension.End.Row; row++)
        {
            var record = new T();
            foreach (var prop in properties)
            {
                var columnIndex = columnMapping.FirstOrDefault(x => 
                    x.Value.Equals(prop.Name, StringComparison.OrdinalIgnoreCase)).Key;
                
                if (columnIndex > 0)
                {
                    var cellValue = worksheet.Cells[row, columnIndex].Value;
                    if (cellValue != null)
                    {
                        try
                        {
                            var convertedValue = Convert.ChangeType(cellValue, 
                                Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                            prop.SetValue(record, convertedValue);
                        }
                        catch
                        {
                            // Keep default value if conversion fails
                        }
                    }
                }
            }
            records.Add(record);
        }

        return records;
    }

    private TEnum ParseEnum<TEnum>(string? value, TEnum defaultValue) where TEnum : struct
    {
        if (string.IsNullOrWhiteSpace(value))
            return defaultValue;

        return Enum.TryParse<TEnum>(value, true, out var result) ? result : defaultValue;
    }
}

// Import models for CSV/Excel parsing
public class StockHoldingImportModel
{
    public string? Symbol { get; set; }
    public string? CompanyName { get; set; }
    public string? Exchange { get; set; }
    public decimal Quantity { get; set; }
    public decimal AverageBuyPrice { get; set; }
    public decimal? CurrentPrice { get; set; }
    public string? ISIN { get; set; }
    public string? Sector { get; set; }
}

public class BankAccountImportModel
{
    public string? BankName { get; set; }
    public string? AccountNumber { get; set; }
    public string? IFSC { get; set; }
    public string? AccountType { get; set; }
    public decimal CurrentBalance { get; set; }
    public string? Currency { get; set; }
    public bool? IsActive { get; set; }
}

public class FixedDepositImportModel
{
    public string? BankAccountNumber { get; set; }
    public string? FDNumber { get; set; }
    public decimal PrincipalAmount { get; set; }
    public decimal InterestRate { get; set; }
    public int TenureInMonths { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime MaturityDate { get; set; }
    public decimal MaturityAmount { get; set; }
    public string? Status { get; set; }
}

public class MutualFundImportModel
{
    public string? FolioNumber { get; set; }
    public string? SchemeName { get; set; }
    public string? AMC { get; set; }
    public string? ISIN { get; set; }
    public string? Category { get; set; }
    public decimal Units { get; set; }
    public decimal AverageNAV { get; set; }
    public decimal? CurrentNAV { get; set; }
    public string? InvestmentMode { get; set; }
}

public class ProvidentFundImportModel
{
    public string? AccountNumber { get; set; }
    public string? PFType { get; set; }
    public decimal? EmployeeContribution { get; set; }
    public decimal? EmployerContribution { get; set; }
    public decimal InterestRate { get; set; }
    public decimal CurrentBalance { get; set; }
    public DateTime? LastUpdatedBalance { get; set; }
}

public class FamilyMemberImportModel
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Relationship { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public bool? IsDependent { get; set; }
}
