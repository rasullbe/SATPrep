using SATPrep.Api.Configurations.Settings;
using SATPrep.Api.DTOs;
using SATPrep.Api.Entities;
using SATPrep.Api.Exceptions;
using SATPrep.Api.Repositories;
using SATPrep.Api.Utilities;
using Microsoft.Extensions.Options;

namespace SATPrep.Api.Services;

public interface IAuthService
{
    Task<AuthSessionResponseDto> RegisterAsync(UserCreateDto dto);
    Task<AuthSessionResponseDto> LoginAsync(UserLoginDto dto, HttpContext httpContext);
    Task LogoutAsync(HttpContext httpContext);
    Task RefreshAsync(HttpContext httpContext);
    Task<AuthSessionResponseDto> GetMeAsync(HttpContext httpContext);
}

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ITokenService _tokenService;
    private readonly ICookieAuthWriter _cookieWriter;
    private readonly JwtOptions _jwtOptions;

    public AuthService(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        ITokenService tokenService,
        ICookieAuthWriter cookieWriter,
        IOptions<JwtOptions> jwtOptions)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _tokenService = tokenService;
        _cookieWriter = cookieWriter;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<AuthSessionResponseDto> RegisterAsync(UserCreateDto dto)
    {
        var existingUser = await _userRepository.GetByEmailAsync(dto.Email.Trim().ToLowerInvariant());
        if (existingUser is not null)
            throw new ConflictException("A user with this email already exists.", "email");

        PasswordPolicy.Validate(dto.Password, dto.Name);

        var passwordHash = PasswordHasher.Hash(dto.Password);
        var user = new User
        {
            Name = dto.Name?.Trim() ?? string.Empty,
            Email = dto.Email.Trim().ToLowerInvariant(),
            Password = passwordHash,
            Role = Role.User,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);
        if (!await _userRepository.SaveChangesAsync())
            throw new BadRequestException("Failed to create user.");

        var refreshToken = TokenHasher.GenerateRefreshToken();
        var tokenBundle = _tokenService.CreateTokenBundle(user, refreshToken);

        await StoreRefreshTokenAsync(refreshToken, user.UserId, tokenBundle.RefreshTokenExpiresAt);

        return new AuthSessionResponseDto(
            user.UserId,
            user.Name,
            user.Email,
            user.Role.ToString(),
            tokenBundle.AccessTokenExpiresAt);
    }

    public async Task<AuthSessionResponseDto> LoginAsync(UserLoginDto dto, HttpContext httpContext)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email.Trim().ToLowerInvariant());
        if (user is null)
        {
            PasswordHasher.VerifyDummy(dto.Password);
            throw new UnauthorizedException("Invalid email or password.");
        }

        if (!PasswordHasher.Verify(user.Password, dto.Password))
            throw new UnauthorizedException("Invalid email or password.");

        var refreshToken = TokenHasher.GenerateRefreshToken();
        var tokenBundle = _tokenService.CreateTokenBundle(user, refreshToken);

        await StoreRefreshTokenAsync(refreshToken, user.UserId, tokenBundle.RefreshTokenExpiresAt);

        _cookieWriter.WriteAuthCookies(
            httpContext,
            tokenBundle.AccessToken,
            tokenBundle.RefreshToken,
            tokenBundle.AccessTokenExpiresAt,
            tokenBundle.RefreshTokenExpiresAt);
        _cookieWriter.WriteCsrfCookie(httpContext);

        return new AuthSessionResponseDto(
            user.UserId,
            user.Name,
            user.Email,
            user.Role.ToString(),
            tokenBundle.AccessTokenExpiresAt);
    }

    public async Task LogoutAsync(HttpContext httpContext)
    {
        var refreshToken = httpContext.Request.Cookies[_jwtOptions.RefreshCookieName];
        if (!string.IsNullOrEmpty(refreshToken))
        {
            var stored = await _refreshTokenRepository.GetByTokenHashAsync(TokenHasher.Hash(refreshToken));
            if (stored is not null)
            {
                stored.RevokedAt = DateTime.UtcNow;
                await _refreshTokenRepository.SaveChangesAsync();
            }
        }

        _cookieWriter.ClearAuthCookies(httpContext);
    }

    public async Task RefreshAsync(HttpContext httpContext)
    {
        var refreshToken = httpContext.Request.Cookies[_jwtOptions.RefreshCookieName];
        if (string.IsNullOrEmpty(refreshToken))
            throw new UnauthorizedException("No refresh token found.");

        var storedToken = await _refreshTokenRepository.GetByTokenHashAsync(TokenHasher.Hash(refreshToken));
        if (storedToken is null || storedToken.IsRevoked || storedToken.IsExpired)
            throw new UnauthorizedException("Invalid or expired refresh token.");

        storedToken.RevokedAt = DateTime.UtcNow;
        await _refreshTokenRepository.SaveChangesAsync();

        var user = await _userRepository.GetByIdAsync(storedToken.UserId);
        if (user is null)
            throw new UnauthorizedException("User not found.");

        var newRefreshToken = TokenHasher.GenerateRefreshToken();
        var tokenBundle = _tokenService.CreateTokenBundle(user, newRefreshToken);

        await StoreRefreshTokenAsync(newRefreshToken, user.UserId, tokenBundle.RefreshTokenExpiresAt);

        _cookieWriter.WriteAuthCookies(
            httpContext,
            tokenBundle.AccessToken,
            tokenBundle.RefreshToken,
            tokenBundle.AccessTokenExpiresAt,
            tokenBundle.RefreshTokenExpiresAt);
        _cookieWriter.WriteCsrfCookie(httpContext);
    }

    public async Task<AuthSessionResponseDto> GetMeAsync(HttpContext httpContext)
    {
        var user = await GetCurrentUserAsync(httpContext);
        if (user is null)
            throw new UnauthorizedException("Not authenticated.");

        return new AuthSessionResponseDto(
            user.UserId,
            user.Name,
            user.Email,
            user.Role.ToString(),
            DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenMinutes));
    }

    private async Task<User?> GetCurrentUserAsync(HttpContext httpContext)
    {
        var token = httpContext.Request.Cookies[_jwtOptions.CookieName];
        if (string.IsNullOrEmpty(token))
            return null;

        var userId = _tokenService.GetUserIdFromToken(token);
        return userId.HasValue ? await _userRepository.GetByIdAsync(userId.Value) : null;
    }

    private async Task StoreRefreshTokenAsync(string token, long userId, DateTime expiresAt)
    {
        var entity = new RefreshToken
        {
            UserId = userId,
            TokenHash = TokenHasher.Hash(token),
            ExpiresAt = expiresAt,
            CreatedAt = DateTime.UtcNow
        };

        await _refreshTokenRepository.AddAsync(entity);
        await _refreshTokenRepository.SaveChangesAsync();
    }
}