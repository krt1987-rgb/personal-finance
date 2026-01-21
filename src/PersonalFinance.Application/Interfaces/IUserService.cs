using PersonalFinance.Application.DTOs;

namespace PersonalFinance.Application.Interfaces;

public interface IUserService
{
    Task<UserDto?> GetByIdAsync(Guid id);
    Task<UserDto?> GetByEmailAsync(string email);
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task<UserDto> CreateAsync(CreateUserDto dto);
    Task UpdateAsync(Guid id, UpdateUserDto dto);
    Task UpdateProfileAsync(Guid id, UpdateUserProfileDto dto);
    Task DeleteAsync(Guid id);
    Task<bool> DeactivateAsync(Guid id);
    Task<bool> ActivateAsync(Guid id);
}
