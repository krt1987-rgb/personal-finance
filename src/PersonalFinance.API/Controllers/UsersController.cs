using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.API.Extensions;
using PersonalFinance.Application.DTOs;
using PersonalFinance.Application.Interfaces;

namespace PersonalFinance.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Get current user profile
    /// </summary>
    [HttpGet("profile")]
    public async Task<ActionResult<UserDto>> GetProfile()
    {
        try
        {
            var userId = User.GetUserId();
            var user = await _userService.GetByIdAsync(userId);
            if (user == null) return NotFound();
            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user profile");
            return StatusCode(500, "An error occurred while retrieving profile");
        }
    }

    /// <summary>
    /// Update current user profile
    /// </summary>
    [HttpPut("profile")]
    public async Task<ActionResult> UpdateProfile(UpdateUserProfileDto updateDto)
    {
        try
        {
            var userId = User.GetUserId();
            await _userService.UpdateProfileAsync(userId, updateDto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user profile");
            return StatusCode(500, "An error occurred while updating profile");
        }
    }

    /// <summary>
    /// Get user by ID (Admin only)
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUser(Guid id)
    {
        try
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user {UserId}", id);
            return StatusCode(500, "An error occurred while retrieving user");
        }
    }

    /// <summary>
    /// Get all users (Admin only)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
    {
        try
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users");
            return StatusCode(500, "An error occurred while retrieving users");
        }
    }

    /// <summary>
    /// Create a new user (Admin only)
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateUser(CreateUserDto createDto)
    {
        try
        {
            var user = await _userService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "User creation failed");
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return StatusCode(500, "An error occurred while creating user");
        }
    }

    /// <summary>
    /// Update user (Admin only)
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateUser(Guid id, UpdateUserDto updateDto)
    {
        try
        {
            await _userService.UpdateAsync(id, updateDto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {UserId}", id);
            return StatusCode(500, "An error occurred while updating user");
        }
    }

    /// <summary>
    /// Delete user (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteUser(Guid id)
    {
        try
        {
            await _userService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {UserId}", id);
            return StatusCode(500, "An error occurred while deleting user");
        }
    }

    /// <summary>
    /// Deactivate user account (Admin only)
    /// </summary>
    [HttpPost("{id}/deactivate")]
    public async Task<ActionResult> DeactivateUser(Guid id)
    {
        try
        {
            var result = await _userService.DeactivateAsync(id);
            if (!result) return NotFound();
            return Ok(new { message = "User deactivated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating user {UserId}", id);
            return StatusCode(500, "An error occurred while deactivating user");
        }
    }

    /// <summary>
    /// Activate user account (Admin only)
    /// </summary>
    [HttpPost("{id}/activate")]
    public async Task<ActionResult> ActivateUser(Guid id)
    {
        try
        {
            var result = await _userService.ActivateAsync(id);
            if (!result) return NotFound();
            return Ok(new { message = "User activated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error activating user {UserId}", id);
            return StatusCode(500, "An error occurred while activating user");
        }
    }
}
