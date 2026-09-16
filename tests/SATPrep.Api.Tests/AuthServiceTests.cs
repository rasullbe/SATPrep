using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SATPrep.Api.Configurations.Settings;
using SATPrep.Api.Data;
using SATPrep.Api.DTOs;
using SATPrep.Api.Exceptions;
using SATPrep.Api.Repositories;
using SATPrep.Api.Services;
using SATPrep.Api.Tests.Fixtures;

namespace SATPrep.Api.Tests;

public class AuthServiceTests
{
    private readonly TestDatabaseFixture _fixture = new();

    [Fact]
    public async Task Register_CreatesUser_ReturnsSession()
    {
        var context = _fixture.GetDbContext();
        var service = CreateAuthService(context);

        var result = await service.RegisterAsync(new UserCreateDto
        {
            Name = "Test User",
            Email = "register@example.com",
            Password = "StrongPass1"
        });

        Assert.NotNull(result);
        Assert.Equal("register@example.com", result.Email);

        var stored = await context.Users.FirstOrDefaultAsync(u => u.Email == "register@example.com");
        Assert.NotNull(stored);
        Assert.Equal("Test User", stored!.Name);
    }

    [Fact]
    public async Task Register_DuplicateEmail_ThrowsConflictException()
    {
        var context = _fixture.GetDbContext();
        var service = CreateAuthService(context);

        var dto = new UserCreateDto
        {
            Name = "Test User",
            Email = "duplicate@example.com",
            Password = "StrongPass1"
        };

        await service.RegisterAsync(dto);

        await Assert.ThrowsAsync<ConflictException>(() => service.RegisterAsync(dto));
    }

    [Fact]
    public async Task Login_WrongPassword_ThrowsUnauthorizedException()
    {
        var context = _fixture.GetDbContext();
        var service = CreateAuthService(context);

        await service.RegisterAsync(new UserCreateDto
        {
            Name = "Test User",
            Email = "login@example.com",
            Password = "StrongPass1"
        });

        await Assert.ThrowsAsync<UnauthorizedException>(() =>
            service.LoginAsync(
                new UserLoginDto { Email = "login@example.com", Password = "WrongPass1" },
                new DefaultHttpContext()));
    }

    [Fact]
    public async Task Login_CorrectPassword_ReturnsSession()
    {
        var context = _fixture.GetDbContext();
        var service = CreateAuthService(context);

        await service.RegisterAsync(new UserCreateDto
        {
            Name = "Test User",
            Email = "login@example.com",
            Password = "StrongPass1"
        });

        var result = await service.LoginAsync(
            new UserLoginDto { Email = "login@example.com", Password = "StrongPass1" },
            new DefaultHttpContext());

        Assert.NotNull(result);
        Assert.Equal("login@example.com", result.Email);
        Assert.Equal("Test User", result.Name);
        Assert.True(result.AccessTokenExpiresAt > DateTime.UtcNow);
    }

    private static AuthService CreateAuthService(AppDbContext context)
    {
        var jwtOptions = Options.Create(new JwtOptions
        {
            Secret = "SATPrepTestSecretKeyWithMoreThanSixtyFourCharactersForJwtSigningInTestsGreaterThan260Bits",
            Issuer = "SATPrep.Tests",
            Audience = "SATPrep.Tests.Clients",
            AccessTokenMinutes = 15,
            RefreshTokenDays = 30
        });

        return new AuthService(
            new UserRepository(context),
            new RefreshTokenRepository(context),
            new TokenService(jwtOptions),
            new StubCookieAuthWriter(),
            jwtOptions);
    }

    private sealed class StubCookieAuthWriter : ICookieAuthWriter
    {
        public void WriteAuthCookies(HttpContext httpContext, string accessToken, string refreshToken, DateTime accessExpiresAt, DateTime refreshExpiresAt)
        {
        }

        public void WriteCsrfCookie(HttpContext httpContext)
        {
        }

        public void ClearAuthCookies(HttpContext httpContext)
        {
        }

        public void WriteLoggedInCookie(HttpContext httpContext, bool loggedIn)
        {
        }
    }
}