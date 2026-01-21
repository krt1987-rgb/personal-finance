using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.API.Extensions;
using PersonalFinance.Application.DTOs;
using PersonalFinance.Application.Interfaces;

namespace PersonalFinance.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProvidentFundsController : ControllerBase
{
    private readonly IProvidentFundService _providentFundService;
    private readonly ILogger<ProvidentFundsController> _logger;

    public ProvidentFundsController(
        IProvidentFundService providentFundService,
        ILogger<ProvidentFundsController> logger)
    {
        _providentFundService = providentFundService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProvidentFundDto>>> GetProvidentFunds()
    {
        try
        {
            var userId = User.GetUserId();
            var funds = await _providentFundService.GetAllAsync(userId);
            return Ok(funds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving provident funds");
            return StatusCode(500, "An error occurred while retrieving provident funds");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProvidentFundDto>> GetProvidentFund(Guid id)
    {
        try
        {
            var fund = await _providentFundService.GetByIdAsync(id);
            if (fund == null) return NotFound();
            return Ok(fund);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving provident fund {FundId}", id);
            return StatusCode(500, "An error occurred while retrieving the provident fund");
        }
    }

    [HttpPost]
    public async Task<ActionResult<ProvidentFundDto>> CreateProvidentFund(CreateProvidentFundDto createDto)
    {
        try
        {
            var fund = await _providentFundService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetProvidentFund), new { id = fund.Id }, fund);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating provident fund");
            return StatusCode(500, "An error occurred while creating the provident fund");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProvidentFund(Guid id, UpdateProvidentFundDto updateDto)
    {
        try
        {
            await _providentFundService.UpdateAsync(id, updateDto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating provident fund {FundId}", id);
            return StatusCode(500, "An error occurred while updating the provident fund");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProvidentFund(Guid id)
    {
        try
        {
            await _providentFundService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting provident fund {FundId}", id);
            return StatusCode(500, "An error occurred while deleting the provident fund");
        }
    }

    [HttpGet("summary")]
    public async Task<ActionResult<PFSummaryDto>> GetPFSummary()
    {
        try
        {
            var userId = User.GetUserId();
            var summary = await _providentFundService.GetSummaryAsync(userId);
            return Ok(summary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving PF summary");
            return StatusCode(500, "An error occurred while retrieving the summary");
        }
    }
}
