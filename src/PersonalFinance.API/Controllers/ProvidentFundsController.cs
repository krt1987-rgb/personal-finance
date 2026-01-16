using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.Domain.Entities;
using PersonalFinance.Domain.Interfaces;

namespace PersonalFinance.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProvidentFundsController : ControllerBase
{
    private readonly IRepository<ProvidentFund> _providentFundRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ProvidentFundsController> _logger;

    public ProvidentFundsController(
        IRepository<ProvidentFund> providentFundRepository,
        IUnitOfWork _unitOfWork,
        ILogger<ProvidentFundsController> logger)
    {
        _providentFundRepository = providentFundRepository;
        this._unitOfWork = _unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Get all provident funds
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProvidentFund>>> GetProvidentFunds()
    {
        try
        {
            var funds = await _providentFundRepository.GetAllAsync();
            return Ok(funds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving provident funds");
            return StatusCode(500, "An error occurred while retrieving provident funds");
        }
    }

    /// <summary>
    /// Get a specific provident fund by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ProvidentFund>> GetProvidentFund(Guid id)
    {
        try
        {
            var fund = await _providentFundRepository.GetByIdAsync(id);
            if (fund == null)
            {
                return NotFound();
            }
            return Ok(fund);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving provident fund {FundId}", id);
            return StatusCode(500, "An error occurred while retrieving the provident fund");
        }
    }

    /// <summary>
    /// Create a new provident fund
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ProvidentFund>> CreateProvidentFund(ProvidentFund providentFund)
    {
        try
        {
            await _providentFundRepository.AddAsync(providentFund);
            await _unitOfWork.SaveChangesAsync();
            
            return CreatedAtAction(nameof(GetProvidentFund), new { id = providentFund.Id }, providentFund);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating provident fund");
            return StatusCode(500, "An error occurred while creating the provident fund");
        }
    }

    /// <summary>
    /// Update an existing provident fund
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProvidentFund(Guid id, ProvidentFund providentFund)
    {
        if (id != providentFund.Id)
        {
            return BadRequest("ID mismatch");
        }

        try
        {
            await _providentFundRepository.UpdateAsync(providentFund);
            await _unitOfWork.SaveChangesAsync();
            
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating provident fund {FundId}", id);
            return StatusCode(500, "An error occurred while updating the provident fund");
        }
    }

    /// <summary>
    /// Delete a provident fund
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProvidentFund(Guid id)
    {
        try
        {
            var fund = await _providentFundRepository.GetByIdAsync(id);
            if (fund == null)
            {
                return NotFound();
            }

            await _providentFundRepository.DeleteAsync(fund);
            await _unitOfWork.SaveChangesAsync();
            
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting provident fund {FundId}", id);
            return StatusCode(500, "An error occurred while deleting the provident fund");
        }
    }

    /// <summary>
    /// Get PF summary with total balances
    /// </summary>
    [HttpGet("summary")]
    public async Task<ActionResult<object>> GetPFSummary()
    {
        try
        {
            var funds = await _providentFundRepository.GetAllAsync();
            
            var summary = new
            {
                TotalAccounts = funds.Count(),
                TotalBalance = funds.Sum(f => f.CurrentBalance),
                TotalEmployeeContribution = funds.Sum(f => f.EmployeeContribution),
                TotalEmployerContribution = funds.Sum(f => f.EmployerContribution),
                ByType = funds.GroupBy(f => f.PFType)
                    .Select(g => new
                    {
                        Type = g.Key.ToString(),
                        Count = g.Count(),
                        TotalBalance = g.Sum(f => f.CurrentBalance)
                    })
            };
            
            return Ok(summary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving PF summary");
            return StatusCode(500, "An error occurred while retrieving the summary");
        }
    }
}
