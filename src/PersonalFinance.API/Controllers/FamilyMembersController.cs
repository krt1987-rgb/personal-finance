using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.Domain.Entities;
using PersonalFinance.Domain.Interfaces;

namespace PersonalFinance.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FamilyMembersController : ControllerBase
{
    private readonly IRepository<FamilyMember> _familyMemberRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<FamilyMembersController> _logger;

    public FamilyMembersController(
        IRepository<FamilyMember> familyMemberRepository,
        IUnitOfWork _unitOfWork,
        ILogger<FamilyMembersController> logger)
    {
        _familyMemberRepository = familyMemberRepository;
        this._unitOfWork = _unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Get all family members
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<FamilyMember>>> GetFamilyMembers()
    {
        try
        {
            var members = await _familyMemberRepository.GetAllAsync();
            return Ok(members);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving family members");
            return StatusCode(500, "An error occurred while retrieving family members");
        }
    }

    /// <summary>
    /// Get a specific family member by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<FamilyMember>> GetFamilyMember(Guid id)
    {
        try
        {
            var member = await _familyMemberRepository.GetByIdAsync(id);
            if (member == null)
            {
                return NotFound();
            }
            return Ok(member);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving family member {MemberId}", id);
            return StatusCode(500, "An error occurred while retrieving the family member");
        }
    }

    /// <summary>
    /// Create a new family member
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<FamilyMember>> CreateFamilyMember(FamilyMember familyMember)
    {
        try
        {
            await _familyMemberRepository.AddAsync(familyMember);
            await _unitOfWork.SaveChangesAsync();
            
            return CreatedAtAction(nameof(GetFamilyMember), new { id = familyMember.Id }, familyMember);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating family member");
            return StatusCode(500, "An error occurred while creating the family member");
        }
    }

    /// <summary>
    /// Update an existing family member
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateFamilyMember(Guid id, FamilyMember familyMember)
    {
        if (id != familyMember.Id)
        {
            return BadRequest("ID mismatch");
        }

        try
        {
            await _familyMemberRepository.UpdateAsync(familyMember);
            await _unitOfWork.SaveChangesAsync();
            
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating family member {MemberId}", id);
            return StatusCode(500, "An error occurred while updating the family member");
        }
    }

    /// <summary>
    /// Delete a family member
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFamilyMember(Guid id)
    {
        try
        {
            var member = await _familyMemberRepository.GetByIdAsync(id);
            if (member == null)
            {
                return NotFound();
            }

            await _familyMemberRepository.DeleteAsync(member);
            await _unitOfWork.SaveChangesAsync();
            
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting family member {MemberId}", id);
            return StatusCode(500, "An error occurred while deleting the family member");
        }
    }
}
