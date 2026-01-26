using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.API.Extensions;
using PersonalFinance.Application.DTOs;
using PersonalFinance.Application.Interfaces;

namespace PersonalFinance.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BankAccountsController : ControllerBase
{
    private readonly IBankAccountService _bankAccountService;
    private readonly IImportService _importService;
    private readonly ILogger<BankAccountsController> _logger;

    public BankAccountsController(
        IBankAccountService bankAccountService,
        IImportService importService,
        ILogger<BankAccountsController> logger)
    {
        _bankAccountService = bankAccountService;
        _importService = importService;
        _logger = logger;
    }

    /// <summary>
    /// Get all bank accounts
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BankAccountDto>>> GetBankAccounts()
    {
        try
        {
            var userId = User.GetUserId();
            var accounts = await _bankAccountService.GetAllAsync(userId);
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
    public async Task<ActionResult<BankAccountDto>> GetBankAccount(Guid id)
    {
        try
        {
            var account = await _bankAccountService.GetByIdAsync(id);
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
    public async Task<ActionResult<BankAccountDto>> CreateBankAccount(CreateBankAccountDto createDto)
    {
        try
        {
            var account = await _bankAccountService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetBankAccount), new { id = account.Id }, account);
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
    public async Task<IActionResult> UpdateBankAccount(Guid id, UpdateBankAccountDto updateDto)
    {
        try
        {
            await _bankAccountService.UpdateAsync(id, updateDto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
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
            await _bankAccountService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting bank account {AccountId}", id);
            return StatusCode(500, "An error occurred while deleting the bank account");
        }
    }

    /// <summary>
    /// Import bank accounts from CSV or Excel file
    /// </summary>
    [HttpPost("import")]
    public async Task<ActionResult<ImportResultDto>> ImportBankAccounts(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            var userId = User.GetUserId();
            using var stream = file.OpenReadStream();
            var result = await _importService.ImportBankAccountsAsync(stream, file.FileName, userId);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing bank accounts");
            return StatusCode(500, "An error occurred while importing bank accounts");
        }
    }
}
