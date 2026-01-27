using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.API.Extensions;
using PersonalFinance.Application.DTOs;
using PersonalFinance.Application.Interfaces;

namespace PersonalFinance.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WealthSnapshotsController : ControllerBase
{
    private readonly IWealthSnapshotService _wealthSnapshotService;
    private readonly ILogger<WealthSnapshotsController> _logger;

    public WealthSnapshotsController(
        IWealthSnapshotService wealthSnapshotService,
        ILogger<WealthSnapshotsController> logger)
    {
        _wealthSnapshotService = wealthSnapshotService;
        _logger = logger;
    }

    /// <summary>
    /// Get all wealth snapshots for the authenticated user
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<WealthSnapshotDto>>> GetWealthSnapshots()
    {
        try
        {
            var userId = User.GetUserId();
            var snapshots = await _wealthSnapshotService.GetAllAsync(userId);
            return Ok(snapshots);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving wealth snapshots");
            return StatusCode(500, "An error occurred while retrieving wealth snapshots");
        }
    }

    /// <summary>
    /// Get the latest wealth snapshot for the authenticated user
    /// </summary>
    [HttpGet("latest")]
    public async Task<ActionResult<WealthSnapshotDto>> GetLatestSnapshot()
    {
        try
        {
            var userId = User.GetUserId();
            var snapshot = await _wealthSnapshotService.GetLatestAsync(userId);
            
            if (snapshot == null)
                return NotFound("No snapshots found");

            return Ok(snapshot);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving latest snapshot");
            return StatusCode(500, "An error occurred while retrieving latest snapshot");
        }
    }

    /// <summary>
    /// Create a new wealth snapshot
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<WealthSnapshotDto>> CreateSnapshot([FromBody] CreateWealthSnapshotDto dto)
    {
        try
        {
            var userId = User.GetUserId();
            var snapshot = await _wealthSnapshotService.CreateSnapshotAsync(userId, dto);
            return CreatedAtAction(nameof(GetLatestSnapshot), new { id = snapshot.Id }, snapshot);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating wealth snapshot");
            return StatusCode(500, "An error occurred while creating wealth snapshot");
        }
    }

    /// <summary>
    /// Get wealth timeline with optional date filtering
    /// </summary>
    [HttpGet("timeline")]
    public async Task<ActionResult<WealthTimelineDto>> GetTimeline(
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        try
        {
            var userId = User.GetUserId();
            var timeline = await _wealthSnapshotService.GetTimelineAsync(userId, fromDate, toDate);
            return Ok(timeline);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving wealth timeline");
            return StatusCode(500, "An error occurred while retrieving wealth timeline");
        }
    }

    /// <summary>
    /// Compare wealth between two time periods
    /// </summary>
    [HttpGet("compare")]
    public async Task<ActionResult<WealthComparisonDto>> ComparePeriods(
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate)
    {
        try
        {
            var userId = User.GetUserId();
            var comparison = await _wealthSnapshotService.ComparePeriodsAsync(userId, fromDate, toDate);
            
            if (comparison == null)
                return NotFound("No snapshots found for the specified periods");

            return Ok(comparison);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error comparing periods");
            return StatusCode(500, "An error occurred while comparing periods");
        }
    }

    /// <summary>
    /// Delete a wealth snapshot
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSnapshot(Guid id)
    {
        try
        {
            await _wealthSnapshotService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Wealth snapshot with ID {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting wealth snapshot");
            return StatusCode(500, "An error occurred while deleting wealth snapshot");
        }
    }
}
