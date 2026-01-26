using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.API.Extensions;
using PersonalFinance.Application.DTOs;
using PersonalFinance.Application.Interfaces;

namespace PersonalFinance.API.Controllers;

/// <summary>
/// MCP (Model Context Protocol) Server Configuration Controller
/// Manages MCP server configurations for real-time market data
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MCPServerController : ControllerBase
{
    private readonly IMCPServerConfigurationService _mcpConfigService;
    private readonly IMCPDataService _mcpDataService;
    private readonly ILogger<MCPServerController> _logger;

    public MCPServerController(
        IMCPServerConfigurationService mcpConfigService,
        IMCPDataService mcpDataService,
        ILogger<MCPServerController> logger)
    {
        _mcpConfigService = mcpConfigService;
        _mcpDataService = mcpDataService;
        _logger = logger;
    }

    /// <summary>
    /// Get all MCP server configurations for the current user
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MCPServerConfigurationDto>>> GetConfigurations()
    {
        try
        {
            var userId = User.GetUserId();
            var configs = await _mcpConfigService.GetUserConfigurationsAsync(userId);
            return Ok(configs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving MCP configurations");
            return StatusCode(500, "An error occurred while retrieving MCP configurations");
        }
    }

    /// <summary>
    /// Create a new MCP server configuration
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<MCPServerConfigurationDto>> CreateConfiguration(CreateMCPServerConfigurationDto request)
    {
        try
        {
            var userId = User.GetUserId();
            var config = await _mcpConfigService.CreateConfigurationAsync(userId, request);
            return CreatedAtAction(nameof(GetConfiguration), new { id = config.Id }, config);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating MCP configuration");
            return StatusCode(500, "An error occurred while creating the MCP configuration");
        }
    }

    /// <summary>
    /// Get a specific MCP server configuration by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<MCPServerConfigurationDto>> GetConfiguration(Guid id)
    {
        try
        {
            var config = await _mcpConfigService.GetConfigurationAsync(id);
            if (config == null)
            {
                return NotFound();
            }
            return Ok(config);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving MCP configuration {ConfigId}", id);
            return StatusCode(500, "An error occurred while retrieving the MCP configuration");
        }
    }

    /// <summary>
    /// Get the default MCP server configuration
    /// </summary>
    [HttpGet("default")]
    public async Task<ActionResult<MCPServerConfigurationDto>> GetDefaultConfiguration()
    {
        try
        {
            var userId = User.GetUserId();
            var config = await _mcpConfigService.GetDefaultConfigurationAsync(userId);
            if (config == null)
            {
                return NotFound("No default MCP configuration found");
            }
            return Ok(config);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving default MCP configuration");
            return StatusCode(500, "An error occurred while retrieving the default MCP configuration");
        }
    }

    /// <summary>
    /// Update an MCP server configuration
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<MCPServerConfigurationDto>> UpdateConfiguration(Guid id, UpdateMCPServerConfigurationDto request)
    {
        try
        {
            var config = await _mcpConfigService.UpdateConfigurationAsync(id, request);
            return Ok(config);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid MCP configuration update: {Message}", ex.Message);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating MCP configuration {ConfigId}", id);
            return StatusCode(500, "An error occurred while updating the MCP configuration");
        }
    }

    /// <summary>
    /// Delete an MCP server configuration
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteConfiguration(Guid id)
    {
        try
        {
            await _mcpConfigService.DeleteConfigurationAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid MCP configuration deletion: {Message}", ex.Message);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting MCP configuration {ConfigId}", id);
            return StatusCode(500, "An error occurred while deleting the MCP configuration");
        }
    }

    /// <summary>
    /// Test connection to an MCP server
    /// </summary>
    [HttpPost("{id}/test")]
    public async Task<ActionResult<MCPConnectionTestDto>> TestConnection(Guid id)
    {
        try
        {
            var result = await _mcpConfigService.TestConnectionAsync(id);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid MCP connection test: {Message}", ex.Message);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error testing MCP connection {ConfigId}", id);
            return StatusCode(500, "An error occurred while testing the MCP connection");
        }
    }

    /// <summary>
    /// Fetch data from MCP server
    /// </summary>
    [HttpPost("fetch")]
    public async Task<ActionResult<MCPDataResponseDto>> FetchData(MCPDataRequestDto request)
    {
        try
        {
            var userId = User.GetUserId();
            var result = await _mcpDataService.FetchDataAsync(userId, request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid MCP data fetch: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching data from MCP server for {Symbol}", request.Symbol);
            return StatusCode(500, "An error occurred while fetching data from the MCP server");
        }
    }

    /// <summary>
    /// Fetch batch data from MCP server
    /// </summary>
    [HttpPost("fetch/batch")]
    public async Task<ActionResult<IEnumerable<MCPDataResponseDto>>> FetchBatchData(List<MCPDataRequestDto> requests)
    {
        try
        {
            var userId = User.GetUserId();
            var results = await _mcpDataService.FetchBatchDataAsync(userId, requests);
            return Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching batch data from MCP server");
            return StatusCode(500, "An error occurred while fetching batch data from the MCP server");
        }
    }
}
