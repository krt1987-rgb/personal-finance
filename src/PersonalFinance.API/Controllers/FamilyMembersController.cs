using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.API.Extensions;
using PersonalFinance.Application.DTOs;
using PersonalFinance.Application.Interfaces;

namespace PersonalFinance.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FamilyMembersController : ControllerBase
{
    private readonly IFamilyMemberService _familyMemberService;
    private readonly IImportService _importService;
    private readonly ILogger<FamilyMembersController> _logger;

    public FamilyMembersController(
        IFamilyMemberService familyMemberService,
        IImportService importService,
        ILogger<FamilyMembersController> logger)
    {
        _familyMemberService = familyMemberService;
        _importService = importService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FamilyMemberDto>>> GetFamilyMembers()
    {
        try
        {
            var userId = User.GetUserId();
            var members = await _familyMemberService.GetAllAsync(userId);
            return Ok(members);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving family members");
            return StatusCode(500, "An error occurred while retrieving family members");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<FamilyMemberDto>> GetFamilyMember(Guid id)
    {
        try
        {
            var member = await _familyMemberService.GetByIdAsync(id);
            if (member == null) return NotFound();
            return Ok(member);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving family member {MemberId}", id);
            return StatusCode(500, "An error occurred while retrieving the family member");
        }
    }

    [HttpPost]
    public async Task<ActionResult<FamilyMemberDto>> CreateFamilyMember(CreateFamilyMemberDto createDto)
    {
        try
        {
            var member = await _familyMemberService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetFamilyMember), new { id = member.Id }, member);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating family member");
            return StatusCode(500, "An error occurred while creating the family member");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateFamilyMember(Guid id, UpdateFamilyMemberDto updateDto)
    {
        try
        {
            await _familyMemberService.UpdateAsync(id, updateDto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating family member {MemberId}", id);
            return StatusCode(500, "An error occurred while updating the family member");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFamilyMember(Guid id)
    {
        try
        {
            await _familyMemberService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting family member {MemberId}", id);
            return StatusCode(500, "An error occurred while deleting the family member");
        }
    }

    /// <summary>
    /// Import family members from CSV or Excel file
    /// </summary>
    [HttpPost("import")]
    public async Task<ActionResult<ImportResultDto>> ImportFamilyMembers(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            var userId = User.GetUserId();
            using var stream = file.OpenReadStream();
            var result = await _importService.ImportFamilyMembersAsync(stream, file.FileName, userId);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing family members");
            return StatusCode(500, "An error occurred while importing family members");
        }
    }
}
