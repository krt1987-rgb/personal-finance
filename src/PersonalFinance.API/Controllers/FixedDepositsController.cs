using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.Application.DTOs;
using PersonalFinance.Application.Interfaces;

namespace PersonalFinance.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FixedDepositsController : ControllerBase
{
    private readonly IFixedDepositService _fixedDepositService;
    private readonly ILogger<FixedDepositsController> _logger;

    public FixedDepositsController(
        IFixedDepositService fixedDepositService,
        ILogger<FixedDepositsController> logger)
    {
        _fixedDepositService = fixedDepositService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FixedDepositDto>>> GetFixedDeposits()
    {
        try
        {
            var userId = Guid.Empty; // TODO: Get from JWT
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
            var userId = Guid.Empty; // TODO: Get from JWT
            var summary = await _fixedDepositService.GetSummaryAsync(userId);
            return Ok(summary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving fixed deposit summary");
            return StatusCode(500, "An error occurred while retrieving the summary");
        }
    }
}
