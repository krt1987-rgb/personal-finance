using PersonalFinance.Application.DTOs;
using PersonalFinance.Application.Interfaces;
using PersonalFinance.Domain.Entities;
using PersonalFinance.Domain.Enums;
using PersonalFinance.Domain.Interfaces;

namespace PersonalFinance.Application.Services;

public class NetWorthService : INetWorthService
{
    private readonly IRepository<StockHolding> _stockRepository;
    private readonly IRepository<MutualFundHolding> _mutualFundRepository;
    private readonly IRepository<BankAccount> _bankAccountRepository;
    private readonly IRepository<FixedDeposit> _fixedDepositRepository;
    private readonly IRepository<ProvidentFund> _providentFundRepository;

    public NetWorthService(
        IRepository<StockHolding> stockRepository,
        IRepository<MutualFundHolding> mutualFundRepository,
        IRepository<BankAccount> bankAccountRepository,
        IRepository<FixedDeposit> fixedDepositRepository,
        IRepository<ProvidentFund> providentFundRepository)
    {
        _stockRepository = stockRepository;
        _mutualFundRepository = mutualFundRepository;
        _bankAccountRepository = bankAccountRepository;
        _fixedDepositRepository = fixedDepositRepository;
        _providentFundRepository = providentFundRepository;
    }

    public async Task<NetWorthDto> CalculateCurrentNetWorthAsync(Guid userId)
    {
        // Get all assets
        var stocks = await _stockRepository.FindAsync(s => s.UserId == userId);
        var mutualFunds = await _mutualFundRepository.FindAsync(m => m.UserId == userId);
        var bankAccounts = await _bankAccountRepository.FindAsync(b => b.UserId == userId);
        var providentFunds = await _providentFundRepository.FindAsync(p => p.UserId == userId);
        
        // Get fixed deposits through bank accounts
        var allFixedDeposits = await _fixedDepositRepository.GetAllAsync();
        var userBankAccountIds = bankAccounts.Select(b => b.Id).ToHashSet();
        var fixedDeposits = allFixedDeposits.Where(f => userBankAccountIds.Contains(f.BankAccountId));

        // Calculate asset values
        var stocksValue = stocks.Sum(s => s.CurrentValue ?? s.InvestedAmount);
        var mutualFundsValue = mutualFunds.Sum(m => m.CurrentValue ?? m.InvestedAmount);
        var bankAccountsBalance = bankAccounts.Sum(b => b.CurrentBalance);
        var fixedDepositsValue = fixedDeposits
            .Where(f => f.Status == FDStatus.Active)
            .Sum(f => f.MaturityAmount);
        var providentFundsBalance = providentFunds.Sum(p => p.CurrentBalance);

        var totalAssets = stocksValue + mutualFundsValue + bankAccountsBalance + 
                         fixedDepositsValue + providentFundsBalance;

        // Liabilities (for future implementation - currently 0)
        var totalLiabilities = 0m;

        return new NetWorthDto
        {
            TotalAssets = totalAssets,
            TotalLiabilities = totalLiabilities,
            NetWorth = totalAssets - totalLiabilities,
            AssetBreakdown = new Dictionary<string, decimal>
            {
                { "Stocks", stocksValue },
                { "MutualFunds", mutualFundsValue },
                { "BankAccounts", bankAccountsBalance },
                { "FixedDeposits", fixedDepositsValue },
                { "ProvidentFunds", providentFundsBalance }
            },
            LiabilityBreakdown = new Dictionary<string, decimal>(),
            CalculatedAt = DateTime.UtcNow
        };
    }
}
