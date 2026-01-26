using PersonalFinance.Application.DTOs;

namespace PersonalFinance.Application.Interfaces;

/// <summary>
/// Service for importing data from CSV and Excel files
/// </summary>
public interface IImportService
{
    /// <summary>
    /// Import stock holdings from a file
    /// </summary>
    Task<ImportResultDto> ImportStockHoldingsAsync(Stream fileStream, string fileName, Guid userId);

    /// <summary>
    /// Import bank accounts from a file
    /// </summary>
    Task<ImportResultDto> ImportBankAccountsAsync(Stream fileStream, string fileName, Guid userId);

    /// <summary>
    /// Import fixed deposits from a file
    /// </summary>
    Task<ImportResultDto> ImportFixedDepositsAsync(Stream fileStream, string fileName, Guid userId);

    /// <summary>
    /// Import mutual fund holdings from a file
    /// </summary>
    Task<ImportResultDto> ImportMutualFundHoldingsAsync(Stream fileStream, string fileName, Guid userId);

    /// <summary>
    /// Import provident funds from a file
    /// </summary>
    Task<ImportResultDto> ImportProvidentFundsAsync(Stream fileStream, string fileName, Guid userId);

    /// <summary>
    /// Import family members from a file
    /// </summary>
    Task<ImportResultDto> ImportFamilyMembersAsync(Stream fileStream, string fileName, Guid userId);
}
