using SATPrep.Api.DTOs;
namespace SATPrep.Api.Services;

public interface IUserService
{
    Task<UserGetDto?> RegisterAsync(UserCreateDto createDto);
    Task<UserGetDto?> AuthenticateAsync(string email, string password);
    Task<UserGetDto?> GetByIdAsync(long userId);
    Task<UserGetDto?> GetByEmailAsync(string email);
    Task<List<UserGetDto>> GetAllAsync();
    Task<UserGetDto?> UpdateAsync(long userId, UserUpdateDto updateDto);
    Task<bool> DeleteAsync(long userId);
    Task<bool> EmailExistsAsync(string email);
    Task<bool> ChangePasswordAsync(long userId, string currentPassword, string newPassword);
}