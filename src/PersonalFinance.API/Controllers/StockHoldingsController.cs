using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.API.Extensions;
using PersonalFinance.Application.DTOs;
using PersonalFinance.Application.Interfaces;

namespace PersonalFinance.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StockHoldingsController : ControllerBase
{
    private readonly IStockHoldingService _stockHoldingService;
    private readonly ILogger<StockHoldingsController> _logger;

    public StockHoldingsController(
        IStockHoldingService stockHoldingService,
        ILogger<StockHoldingsController> logger)
    {
        _stockHoldingService = stockHoldingService;
        _logger = logger;
    }

    /// <summary>
    /// Get all stock holdings for the current user
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<StockHoldingDto>>> GetStockHoldings()
    {
        try
        {
            var userId = User.GetUserId();
            var holdings = await _stockHoldingService.GetAllAsync(userId);
            return Ok(holdings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving stock holdings");
            return StatusCode(500, "An error occurred while retrieving stock holdings");
        }
    }

    /// <summary>
    /// Get a specific stock holding by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<StockHoldingDto>> GetStockHolding(Guid id)
    {
        try
        {
            var holding = await _stockHoldingService.GetByIdAsync(id);
            if (holding == null)
            {
                return NotFound();
            }
            return Ok(holding);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving stock holding {HoldingId}", id);
            return StatusCode(500, "An error occurred while retrieving the stock holding");
        }
    }

    /// <summary>
    /// Create a new stock holding
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<StockHoldingDto>> CreateStockHolding(CreateStockHoldingDto createDto)
    {
        try
        {
            var holding = await _stockHoldingService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetStockHolding), new { id = holding.Id }, holding);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating stock holding");
            return StatusCode(500, "An error occurred while creating the stock holding");
        }
    }

    /// <summary>
    /// Update an existing stock holding
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateStockHolding(Guid id, UpdateStockHoldingDto updateDto)
    {
        try
        {
            await _stockHoldingService.UpdateAsync(id, updateDto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating stock holding {HoldingId}", id);
            return StatusCode(500, "An error occurred while updating the stock holding");
        }
    }

    /// <summary>
    /// Delete a stock holding
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStockHolding(Guid id)
    {
        try
        {
            await _stockHoldingService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting stock holding {HoldingId}", id);
            return StatusCode(500, "An error occurred while deleting the stock holding");
        }
    }

    /// <summary>
    /// Get portfolio summary with total investment, current value, and P&L
    /// </summary>
    [HttpGet("portfolio-summary")]
    public async Task<ActionResult<PortfolioSummaryDto>> GetPortfolioSummary()
    {
        try
        {
            var userId = User.GetUserId();
            var summary = await _stockHoldingService.GetPortfolioSummaryAsync(userId);
            return Ok(summary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving portfolio summary");
            return StatusCode(500, "An error occurred while retrieving portfolio summary");
        }
    }
}
