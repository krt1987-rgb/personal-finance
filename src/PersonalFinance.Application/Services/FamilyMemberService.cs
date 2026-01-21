using PersonalFinance.Application.DTOs;
using PersonalFinance.Application.Interfaces;
using PersonalFinance.Domain.Entities;
using PersonalFinance.Domain.Interfaces;

namespace PersonalFinance.Application.Services;

public class FamilyMemberService : IFamilyMemberService
{
    private readonly IRepository<FamilyMember> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public FamilyMemberService(IRepository<FamilyMember> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<FamilyMemberDto>> GetAllAsync(Guid userId)
    {
        var members = await _repository.FindAsync(m => m.UserId == userId);
        return members.Select(MapToDto);
    }

    public async Task<FamilyMemberDto?> GetByIdAsync(Guid id)
    {
        var member = await _repository.GetByIdAsync(id);
        return member == null ? null : MapToDto(member);
    }

    public async Task<FamilyMemberDto> CreateAsync(CreateFamilyMemberDto dto)
    {
        var member = new FamilyMember
        {
            UserId = dto.UserId,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Relationship = dto.Relationship,
            DateOfBirth = dto.DateOfBirth,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            IsDependent = dto.IsDependent
        };

        await _repository.AddAsync(member);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(member);
    }

    public async Task UpdateAsync(Guid id, UpdateFamilyMemberDto dto)
    {
        var member = await _repository.GetByIdAsync(id);
        if (member == null)
            throw new KeyNotFoundException($"Family member with ID {id} not found");

        if (dto.FirstName != null) member.FirstName = dto.FirstName;
        if (dto.LastName != null) member.LastName = dto.LastName;
        if (dto.Email != null) member.Email = dto.Email;
        if (dto.PhoneNumber != null) member.PhoneNumber = dto.PhoneNumber;
        if (dto.IsDependent.HasValue) member.IsDependent = dto.IsDependent.Value;

        await _repository.UpdateAsync(member);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var member = await _repository.GetByIdAsync(id);
        if (member == null)
            throw new KeyNotFoundException($"Family member with ID {id} not found");

        await _repository.DeleteAsync(member);
        await _unitOfWork.SaveChangesAsync();
    }

    private static FamilyMemberDto MapToDto(FamilyMember member)
    {
        return new FamilyMemberDto
        {
            Id = member.Id,
            UserId = member.UserId,
            FirstName = member.FirstName,
            LastName = member.LastName,
            Relationship = member.Relationship,
            DateOfBirth = member.DateOfBirth,
            Email = member.Email,
            PhoneNumber = member.PhoneNumber,
            IsDependent = member.IsDependent
        };
    }
}
