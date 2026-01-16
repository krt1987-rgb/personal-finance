using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.Domain.Entities;
using PersonalFinance.Domain.Interfaces;

namespace PersonalFinance.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MutualFundHoldingsController : ControllerBase
{
    private readonly IRepository<MutualFundHolding> _mutualFundRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<MutualFundHoldingsController> _logger;

    public MutualFundHoldingsController(
        IRepository<MutualFundHolding> mutualFundRepository,
        IUnitOfWork unitOfWork,
        ILogger<MutualFundHoldingsController> logger)
    {
        _mutualFundRepository = mutualFundRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Get all mutual fund holdings
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MutualFundHolding>>> GetMutualFundHoldings()
    {
        try
        {
            var holdings = await _mutualFundRepository.GetAllAsync();
            return Ok(holdings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving mutual fund holdings");
            return StatusCode(500, "An error occurred while retrieving mutual fund holdings");
        }
    }

    /// <summary>
    /// Get a specific mutual fund holding by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<MutualFundHolding>> GetMutualFundHolding(Guid id)
    {
        try
        {
            var holding = await _mutualFundRepository.GetByIdAsync(id);
            if (holding == null)
            {
                return NotFound();
            }
            return Ok(holding);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving mutual fund holding {HoldingId}", id);
            return StatusCode(500, "An error occurred while retrieving the mutual fund holding");
        }
    }

    /// <summary>
    /// Create a new mutual fund holding
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<MutualFundHolding>> CreateMutualFundHolding(MutualFundHolding mutualFundHolding)
    {
        try
        {
            await _mutualFundRepository.AddAsync(mutualFundHolding);
            await _unitOfWork.SaveChangesAsync();
            
            return CreatedAtAction(nameof(GetMutualFundHolding), new { id = mutualFundHolding.Id }, mutualFundHolding);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating mutual fund holding");
            return StatusCode(500, "An error occurred while creating the mutual fund holding");
        }
    }

    /// <summary>
    /// Update an existing mutual fund holding
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMutualFundHolding(Guid id, MutualFundHolding mutualFundHolding)
    {
        if (id != mutualFundHolding.Id)
        {
            return BadRequest("ID mismatch");
        }

        try
        {
            await _mutualFundRepository.UpdateAsync(mutualFundHolding);
            await _unitOfWork.SaveChangesAsync();
            
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating mutual fund holding {HoldingId}", id);
            return StatusCode(500, "An error occurred while updating the mutual fund holding");
        }
    }

    /// <summary>
    /// Delete a mutual fund holding
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMutualFundHolding(Guid id)
    {
        try
        {
            var holding = await _mutualFundRepository.GetByIdAsync(id);
            if (holding == null)
            {
                return NotFound();
            }

            await _mutualFundRepository.DeleteAsync(holding);
            await _unitOfWork.SaveChangesAsync();
            
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting mutual fund holding {HoldingId}", id);
            return StatusCode(500, "An error occurred while deleting the mutual fund holding");
        }
    }

    /// <summary>
    /// Get mutual fund portfolio summary
    /// </summary>
    [HttpGet("portfolio-summary")]
    public async Task<ActionResult<object>> GetPortfolioSummary()
    {
        try
        {
            var holdings = await _mutualFundRepository.GetAllAsync();
            
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
            _logger.LogError(ex, "Error retrieving mutual fund portfolio summary");
            return StatusCode(500, "An error occurred while retrieving portfolio summary");
        }
    }
}
