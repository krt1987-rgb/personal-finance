using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.Domain.Entities;
using PersonalFinance.Domain.Interfaces;

namespace PersonalFinance.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FixedDepositsController : ControllerBase
{
    private readonly IRepository<FixedDeposit> _fixedDepositRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<FixedDepositsController> _logger;

    public FixedDepositsController(
        IRepository<FixedDeposit> fixedDepositRepository,
        IUnitOfWork _unitOfWork,
        ILogger<FixedDepositsController> logger)
    {
        _fixedDepositRepository = fixedDepositRepository;
        this._unitOfWork = _unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Get all fixed deposits
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<FixedDeposit>>> GetFixedDeposits()
    {
        try
        {
            var deposits = await _fixedDepositRepository.GetAllAsync();
            return Ok(deposits);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving fixed deposits");
            return StatusCode(500, "An error occurred while retrieving fixed deposits");
        }
    }

    /// <summary>
    /// Get a specific fixed deposit by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<FixedDeposit>> GetFixedDeposit(Guid id)
    {
        try
        {
            var deposit = await _fixedDepositRepository.GetByIdAsync(id);
            if (deposit == null)
            {
                return NotFound();
            }
            return Ok(deposit);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving fixed deposit {DepositId}", id);
            return StatusCode(500, "An error occurred while retrieving the fixed deposit");
        }
    }

    /// <summary>
    /// Create a new fixed deposit
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<FixedDeposit>> CreateFixedDeposit(FixedDeposit fixedDeposit)
    {
        try
        {
            await _fixedDepositRepository.AddAsync(fixedDeposit);
            await _unitOfWork.SaveChangesAsync();
            
            return CreatedAtAction(nameof(GetFixedDeposit), new { id = fixedDeposit.Id }, fixedDeposit);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating fixed deposit");
            return StatusCode(500, "An error occurred while creating the fixed deposit");
        }
    }

    /// <summary>
    /// Update an existing fixed deposit
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateFixedDeposit(Guid id, FixedDeposit fixedDeposit)
    {
        if (id != fixedDeposit.Id)
        {
            return BadRequest("ID mismatch");
        }

        try
        {
            await _fixedDepositRepository.UpdateAsync(fixedDeposit);
            await _unitOfWork.SaveChangesAsync();
            
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating fixed deposit {DepositId}", id);
            return StatusCode(500, "An error occurred while updating the fixed deposit");
        }
    }

    /// <summary>
    /// Delete a fixed deposit
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFixedDeposit(Guid id)
    {
        try
        {
            var deposit = await _fixedDepositRepository.GetByIdAsync(id);
            if (deposit == null)
            {
                return NotFound();
            }

            await _fixedDepositRepository.DeleteAsync(deposit);
            await _unitOfWork.SaveChangesAsync();
            
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting fixed deposit {DepositId}", id);
            return StatusCode(500, "An error occurred while deleting the fixed deposit");
        }
    }

    /// <summary>
    /// Get summary of all fixed deposits
    /// </summary>
    [HttpGet("summary")]
    public async Task<ActionResult<object>> GetFixedDepositSummary()
    {
        try
        {
            var deposits = await _fixedDepositRepository.GetAllAsync();
            
            var summary = new
            {
                TotalDeposits = deposits.Count(),
                TotalPrincipal = deposits.Sum(d => d.PrincipalAmount),
                TotalMaturityAmount = deposits.Sum(d => d.MaturityAmount),
                ActiveDeposits = deposits.Count(d => d.Status == Domain.Enums.FDStatus.Active),
                MaturedDeposits = deposits.Count(d => d.Status == Domain.Enums.FDStatus.Matured)
            };
            
            return Ok(summary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving fixed deposit summary");
            return StatusCode(500, "An error occurred while retrieving the summary");
        }
    }
}
