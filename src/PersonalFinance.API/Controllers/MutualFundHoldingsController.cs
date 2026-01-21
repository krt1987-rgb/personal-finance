using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.Application.DTOs;
using PersonalFinance.Application.Interfaces;

namespace PersonalFinance.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MutualFundHoldingsController : ControllerBase
{
    private readonly IMutualFundService _mutualFundService;
    private readonly ILogger<MutualFundHoldingsController> _logger;

    public MutualFundHoldingsController(
        IMutualFundService mutualFundService,
        ILogger<MutualFundHoldingsController> logger)
    {
        _mutualFundService = mutualFundService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MutualFundHoldingDto>>> GetMutualFundHoldings()
    {
        try
        {
            var userId = Guid.Empty; // TODO: Get from JWT
            var holdings = await _mutualFundService.GetAllAsync(userId);
            return Ok(holdings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving mutual fund holdings");
            return StatusCode(500, "An error occurred while retrieving mutual fund holdings");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MutualFundHoldingDto>> GetMutualFundHolding(Guid id)
    {
        try
        {
            var holding = await _mutualFundService.GetByIdAsync(id);
            if (holding == null) return NotFound();
            return Ok(holding);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving mutual fund holding {HoldingId}", id);
            return StatusCode(500, "An error occurred while retrieving the mutual fund holding");
        }
    }

    [HttpPost]
    public async Task<ActionResult<MutualFundHoldingDto>> CreateMutualFundHolding(CreateMutualFundHoldingDto createDto)
    {
        try
        {
            var holding = await _mutualFundService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetMutualFundHolding), new { id = holding.Id }, holding);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating mutual fund holding");
            return StatusCode(500, "An error occurred while creating the mutual fund holding");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMutualFundHolding(Guid id, UpdateMutualFundHoldingDto updateDto)
    {
        try
        {
            await _mutualFundService.UpdateAsync(id, updateDto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating mutual fund holding {HoldingId}", id);
            return StatusCode(500, "An error occurred while updating the mutual fund holding");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMutualFundHolding(Guid id)
    {
        try
        {
            await _mutualFundService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting mutual fund holding {HoldingId}", id);
            return StatusCode(500, "An error occurred while deleting the mutual fund holding");
        }
    }

    [HttpGet("portfolio-summary")]
    public async Task<ActionResult<MutualFundPortfolioSummaryDto>> GetPortfolioSummary()
    {
        try
        {
            var userId = Guid.Empty; // TODO: Get from JWT
            var summary = await _mutualFundService.GetPortfolioSummaryAsync(userId);
            return Ok(summary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving mutual fund portfolio summary");
            return StatusCode(500, "An error occurred while retrieving portfolio summary");
        }
    }
}
