using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.API.Extensions;
using PersonalFinance.Application.DTOs;
using PersonalFinance.Application.Interfaces;

namespace PersonalFinance.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FixedDepositsController : ControllerBase
{
    private readonly IFixedDepositService _fixedDepositService;
    private readonly IImportService _importService;
    private readonly ILogger<FixedDepositsController> _logger;

    public FixedDepositsController(
        IFixedDepositService fixedDepositService,
        IImportService importService,
        ILogger<FixedDepositsController> logger)
    {
        _fixedDepositService = fixedDepositService;
        _importService = importService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FixedDepositDto>>> GetFixedDeposits()
    {
        try
        {
            var userId = User.GetUserId();
            var deposits = await _fixedDepositService.GetAllAsync(userId);
            return Ok(deposits);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving fixed deposits");
            return StatusCode(500, "An error occurred while retrieving fixed deposits");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<FixedDepositDto>> GetFixedDeposit(Guid id)
    {
        try
        {
            var deposit = await _fixedDepositService.GetByIdAsync(id);
            if (deposit == null) return NotFound();
            return Ok(deposit);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving fixed deposit {DepositId}", id);
            return StatusCode(500, "An error occurred while retrieving the fixed deposit");
        }
    }

    [HttpPost]
    public async Task<ActionResult<FixedDepositDto>> CreateFixedDeposit(CreateFixedDepositDto createDto)
    {
        try
        {
            var deposit = await _fixedDepositService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetFixedDeposit), new { id = deposit.Id }, deposit);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating fixed deposit");
            return StatusCode(500, "An error occurred while creating the fixed deposit");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateFixedDeposit(Guid id, UpdateFixedDepositDto updateDto)
    {
        try
        {
            await _fixedDepositService.UpdateAsync(id, updateDto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating fixed deposit {DepositId}", id);
            return StatusCode(500, "An error occurred while updating the fixed deposit");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFixedDeposit(Guid id)
    {
        try
        {
            await _fixedDepositService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting fixed deposit {DepositId}", id);
            return StatusCode(500, "An error occurred while deleting the fixed deposit");
        }
    }

    [HttpGet("summary")]
    public async Task<ActionResult<FixedDepositSummaryDto>> GetFixedDepositSummary()
    {
        try
        {
            var userId = User.GetUserId();
            var summary = await _fixedDepositService.GetSummaryAsync(userId);
            return Ok(summary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving fixed deposit summary");
            return StatusCode(500, "An error occurred while retrieving the summary");
        }
    }

    /// <summary>
    /// Import fixed deposits from CSV or Excel file
    /// </summary>
    [HttpPost("import")]
    public async Task<ActionResult<ImportResultDto>> ImportFixedDeposits(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            var userId = User.GetUserId();
            using var stream = file.OpenReadStream();
            var result = await _importService.ImportFixedDepositsAsync(stream, file.FileName, userId);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing fixed deposits");
            return StatusCode(500, "An error occurred while importing fixed deposits");
        }
    }
}
