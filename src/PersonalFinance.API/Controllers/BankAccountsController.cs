using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.Domain.Entities;
using PersonalFinance.Domain.Interfaces;

namespace PersonalFinance.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BankAccountsController : ControllerBase
{
    private readonly IRepository<BankAccount> _bankAccountRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<BankAccountsController> _logger;

    public BankAccountsController(
        IRepository<BankAccount> bankAccountRepository,
        IUnitOfWork unitOfWork,
        ILogger<BankAccountsController> logger)
    {
        _bankAccountRepository = bankAccountRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Get all bank accounts
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BankAccount>>> GetBankAccounts()
    {
        try
        {
            var accounts = await _bankAccountRepository.GetAllAsync();
            return Ok(accounts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bank accounts");
            return StatusCode(500, "An error occurred while retrieving bank accounts");
        }
    }

    /// <summary>
    /// Get a specific bank account by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<BankAccount>> GetBankAccount(Guid id)
    {
        try
        {
            var account = await _bankAccountRepository.GetByIdAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            return Ok(account);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bank account {AccountId}", id);
            return StatusCode(500, "An error occurred while retrieving the bank account");
        }
    }

    /// <summary>
    /// Create a new bank account
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<BankAccount>> CreateBankAccount(BankAccount bankAccount)
    {
        try
        {
            await _bankAccountRepository.AddAsync(bankAccount);
            await _unitOfWork.SaveChangesAsync();
            
            return CreatedAtAction(nameof(GetBankAccount), new { id = bankAccount.Id }, bankAccount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating bank account");
            return StatusCode(500, "An error occurred while creating the bank account");
        }
    }

    /// <summary>
    /// Update an existing bank account
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBankAccount(Guid id, BankAccount bankAccount)
    {
        if (id != bankAccount.Id)
        {
            return BadRequest("ID mismatch");
        }

        try
        {
            await _bankAccountRepository.UpdateAsync(bankAccount);
            await _unitOfWork.SaveChangesAsync();
            
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating bank account {AccountId}", id);
            return StatusCode(500, "An error occurred while updating the bank account");
        }
    }

    /// <summary>
    /// Delete a bank account
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBankAccount(Guid id)
    {
        try
        {
            var account = await _bankAccountRepository.GetByIdAsync(id);
            if (account == null)
            {
                return NotFound();
            }

            await _bankAccountRepository.DeleteAsync(account);
            await _unitOfWork.SaveChangesAsync();
            
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting bank account {AccountId}", id);
            return StatusCode(500, "An error occurred while deleting the bank account");
        }
    }
}
