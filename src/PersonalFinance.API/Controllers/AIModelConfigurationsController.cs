using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.API.Extensions;
using PersonalFinance.Application.DTOs;
using PersonalFinance.Application.Interfaces;

namespace PersonalFinance.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AIModelConfigurationsController : ControllerBase
{
    private readonly IAIModelConfigurationService _aiConfigService;
    private readonly ILogger<AIModelConfigurationsController> _logger;

    public AIModelConfigurationsController(
        IAIModelConfigurationService aiConfigService,
        ILogger<AIModelConfigurationsController> logger)
    {
        _aiConfigService = aiConfigService;
        _logger = logger;
    }

    /// <summary>
    /// Get all AI model configurations for the current user
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AIModelConfigurationDto>>> GetConfigurations()
    {
        try
        {
            var userId = User.GetUserId();
            var configs = await _aiConfigService.GetAllAsync(userId);
            return Ok(configs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving AI model configurations");
            return StatusCode(500, "An error occurred while retrieving AI model configurations");
        }
    }

    /// <summary>
    /// Get a specific AI model configuration by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<AIModelConfigurationDto>> GetConfiguration(Guid id)
    {
        try
        {
            var config = await _aiConfigService.GetByIdAsync(id);
            if (config == null)
            {
                return NotFound();
            }
            return Ok(config);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving AI model configuration {ConfigId}", id);
            return StatusCode(500, "An error occurred while retrieving the AI model configuration");
        }
    }

    /// <summary>
    /// Get the default AI model configuration for the current user
    /// </summary>
    [HttpGet("default")]
    public async Task<ActionResult<AIModelConfigurationDto>> GetDefaultConfiguration()
    {
        try
        {
            var userId = User.GetUserId();
            var config = await _aiConfigService.GetDefaultAsync(userId);
            if (config == null)
            {
                return NotFound("No default AI model configuration found");
            }
            return Ok(config);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving default AI model configuration");
            return StatusCode(500, "An error occurred while retrieving the default AI model configuration");
        }
    }

    /// <summary>
    /// Create a new AI model configuration
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<AIModelConfigurationDto>> CreateConfiguration(CreateAIModelConfigurationDto createDto)
    {
        try
        {
            var userId = User.GetUserId();
            var config = await _aiConfigService.CreateAsync(userId, createDto);
            return CreatedAtAction(nameof(GetConfiguration), new { id = config.Id }, config);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid AI configuration: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating AI model configuration");
            return StatusCode(500, "An error occurred while creating the AI model configuration");
        }
    }

    /// <summary>
    /// Update an existing AI model configuration
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateConfiguration(Guid id, UpdateAIModelConfigurationDto updateDto)
    {
        try
        {
            await _aiConfigService.UpdateAsync(id, updateDto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid AI configuration update: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating AI model configuration {ConfigId}", id);
            return StatusCode(500, "An error occurred while updating the AI model configuration");
        }
    }

    /// <summary>
    /// Delete an AI model configuration
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteConfiguration(Guid id)
    {
        try
        {
            await _aiConfigService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting AI model configuration {ConfigId}", id);
            return StatusCode(500, "An error occurred while deleting the AI model configuration");
        }
    }

    /// <summary>
    /// Set an AI model configuration as default
    /// </summary>
    [HttpPost("{id}/set-default")]
    public async Task<IActionResult> SetAsDefault(Guid id)
    {
        try
        {
            var userId = User.GetUserId();
            await _aiConfigService.SetAsDefaultAsync(id, userId);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting AI model configuration as default {ConfigId}", id);
            return StatusCode(500, "An error occurred while setting the AI model configuration as default");
        }
    }

    /// <summary>
    /// Get status of all configured AI providers
    /// </summary>
    [HttpGet("providers/status")]
    public async Task<ActionResult<IEnumerable<AIProviderStatusDto>>> GetProviderStatus()
    {
        try
        {
            var userId = User.GetUserId();
            var status = await _aiConfigService.GetProviderStatusAsync(userId);
            return Ok(status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving AI provider status");
            return StatusCode(500, "An error occurred while retrieving AI provider status");
        }
    }
}
