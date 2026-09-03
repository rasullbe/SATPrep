using SATPrep.Api.DTOs;
using SATPrep.Api.Mappings;
using SATPrep.Api.Repositories;
using SATPrep.Api.Utilities;

namespace SATPrep.Api.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserGetDto?> RegisterAsync(UserCreateDto createDto)
    {
        if (createDto is null)
            throw new ArgumentNullException(nameof(createDto));

        if (await _userRepository.GetByEmailAsync(createDto.Email) is not null)
            return null;

        var passwordHash = PasswordHasher.Hash(createDto.Password);
        var user = createDto.ToEntity(passwordHash);

        await _userRepository.AddAsync(user);
        if (!await _userRepository.SaveChangesAsync())
            return null;

        return user.ToGetDto();
    }

    public async Task<UserGetDto?> AuthenticateAsync(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return null;

        var user = await _userRepository.GetByEmailAsync(email);
        if (user is null)
        {
            PasswordHasher.VerifyDummy(password);
            return null;
        }

        if (!PasswordHasher.Verify(user.Password, password))
            return null;

        return user.ToGetDto();
    }

    public async Task<UserGetDto?> GetByIdAsync(long userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        return user?.ToGetDto();
    }

    public async Task<UserGetDto?> GetByEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return null;

        var user = await _userRepository.GetByEmailAsync(email);
        return user?.ToGetDto();
    }

    public async Task<List<UserGetDto>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(u => u.ToGetDto()).ToList();
    }

    public async Task<UserGetDto?> UpdateAsync(long userId, UserUpdateDto updateDto)
    {
        if (updateDto is null)
            throw new ArgumentNullException(nameof(updateDto));

        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
            return null;

        string? newPasswordHash = null;
        if (!string.IsNullOrWhiteSpace(updateDto.NewPassword))
            newPasswordHash = PasswordHasher.Hash(updateDto.NewPassword);

        user.UpdateFrom(updateDto, newPasswordHash);

        if (!await _userRepository.SaveChangesAsync())
            return null;

        return user.ToGetDto();
    }

    public async Task<bool> DeleteAsync(long userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
            return false;

        return await _userRepository.SaveChangesAsync();
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        var user = await _userRepository.GetByEmailAsync(email);
        return user is not null;
    }

    public async Task<bool> ChangePasswordAsync(long userId, string currentPassword, string newPassword)
    {
        if (string.IsNullOrWhiteSpace(currentPassword) || string.IsNullOrWhiteSpace(newPassword))
            return false;

        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
            return false;

        if (!PasswordHasher.Verify(user.Password, currentPassword))
            return false;

        user.Password = PasswordHasher.Hash(newPassword);

        return await _userRepository.SaveChangesAsync();
    }
}
