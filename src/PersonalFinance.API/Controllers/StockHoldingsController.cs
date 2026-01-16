using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.Domain.Entities;
using PersonalFinance.Domain.Interfaces;

namespace PersonalFinance.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StockHoldingsController : ControllerBase
{
    private readonly IRepository<StockHolding> _stockHoldingRepository;
    private readonly IRepository<StockTransaction> _stockTransactionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<StockHoldingsController> _logger;

    public StockHoldingsController(
        IRepository<StockHolding> stockHoldingRepository,
        IRepository<StockTransaction> stockTransactionRepository,
        IUnitOfWork unitOfWork,
        ILogger<StockHoldingsController> logger)
    {
        _stockHoldingRepository = stockHoldingRepository;
        _stockTransactionRepository = stockTransactionRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Get all stock holdings for the current user
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<StockHolding>>> GetStockHoldings()
    {
        try
        {
            // TODO: Get userId from JWT token claims
            var holdings = await _stockHoldingRepository.GetAllAsync();
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
    public async Task<ActionResult<StockHolding>> GetStockHolding(Guid id)
    {
        try
        {
            var holding = await _stockHoldingRepository.GetByIdAsync(id);
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
    public async Task<ActionResult<StockHolding>> CreateStockHolding(StockHolding stockHolding)
    {
        try
        {
            // TODO: Set userId from JWT token claims
            await _stockHoldingRepository.AddAsync(stockHolding);
            await _unitOfWork.SaveChangesAsync();
            
            return CreatedAtAction(nameof(GetStockHolding), new { id = stockHolding.Id }, stockHolding);
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
    public async Task<IActionResult> UpdateStockHolding(Guid id, StockHolding stockHolding)
    {
        if (id != stockHolding.Id)
        {
            return BadRequest("ID mismatch");
        }

        try
        {
            await _stockHoldingRepository.UpdateAsync(stockHolding);
            await _unitOfWork.SaveChangesAsync();
            
            return NoContent();
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
            var holding = await _stockHoldingRepository.GetByIdAsync(id);
            if (holding == null)
            {
                return NotFound();
            }

            await _stockHoldingRepository.DeleteAsync(holding);
            await _unitOfWork.SaveChangesAsync();
            
            return NoContent();
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
    public async Task<ActionResult<object>> GetPortfolioSummary()
    {
        try
        {
            var holdings = await _stockHoldingRepository.GetAllAsync();
            
            var summary = new
            {
                TotalInvestment = holdings.Sum(h => h.InvestedAmount),
                CurrentValue = holdings.Sum(h => h.CurrentValue ?? 0),
                TotalProfitLoss = holdings.Sum(h => h.ProfitLoss ?? 0),
                TotalHoldings = holdings.Count()
            };
            
            return Ok(summary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving portfolio summary");
            return StatusCode(500, "An error occurred while retrieving portfolio summary");
        }
    }
}
