using PersonalFinance.Application.DTOs;

namespace PersonalFinance.Application.Interfaces;

public interface IFamilyMemberService
{
    Task<IEnumerable<FamilyMemberDto>> GetAllAsync(Guid userId);
    Task<FamilyMemberDto?> GetByIdAsync(Guid id);
    Task<FamilyMemberDto> CreateAsync(CreateFamilyMemberDto dto);
    Task UpdateAsync(Guid id, UpdateFamilyMemberDto dto);
    Task DeleteAsync(Guid id);
}
