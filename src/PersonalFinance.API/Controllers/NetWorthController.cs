using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.API.Extensions;
using PersonalFinance.Application.DTOs;
using PersonalFinance.Application.Interfaces;

namespace PersonalFinance.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NetWorthController : ControllerBase
{
    private readonly INetWorthService _netWorthService;
    private readonly ILogger<NetWorthController> _logger;

    public NetWorthController(
        INetWorthService netWorthService,
        ILogger<NetWorthController> logger)
    {
        _netWorthService = netWorthService;
        _logger = logger;
    }

    /// <summary>
    /// Get current net worth for the authenticated user
    /// </summary>
    [HttpGet("current")]
    public async Task<ActionResult<NetWorthDto>> GetCurrentNetWorth()
    {
        try
        {
            var userId = User.GetUserId();
            var netWorth = await _netWorthService.CalculateCurrentNetWorthAsync(userId);
            return Ok(netWorth);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating current net worth");
            return StatusCode(500, "An error occurred while calculating net worth");
        }
    }
}
