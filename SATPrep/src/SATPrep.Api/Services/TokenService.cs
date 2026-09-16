using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SATPrep.Api.Configurations.Settings;
using SATPrep.Api.DTOs;
using SATPrep.Api.Entities;

namespace SATPrep.Api.Services;

public interface ITokenService
{
    AuthTokenBundle CreateTokenBundle(User user, string refreshToken);
    ClaimsPrincipal? ValidateAccessToken(string token);
    long? GetUserIdFromToken(string token);
    string CreateAccessToken(User user);
}

public class TokenService : ITokenService
{
    private readonly JwtOptions _options;
    private readonly SymmetricSecurityKey _signingKey;

    public TokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
        _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret));
    }

    public AuthTokenBundle CreateTokenBundle(User user, string refreshToken)
    {
        var accessToken = CreateAccessToken(user);
        var now = DateTime.UtcNow;

        return new AuthTokenBundle(
            accessToken,
            now.AddMinutes(_options.AccessTokenMinutes),
            refreshToken,
            now.AddDays(_options.RefreshTokenDays));
    }

    public string CreateAccessToken(User user)
    {
        var now = DateTime.UtcNow;

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: now,
            expires: now.AddMinutes(_options.AccessTokenMinutes),
            signingCredentials: new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public ClaimsPrincipal? ValidateAccessToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = _signingKey,
            ValidateIssuer = true,
            ValidIssuer = _options.Issuer,
            ValidateAudience = true,
            ValidAudience = _options.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),
            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role
        };

        try
        {
            return handler.ValidateToken(token, validationParameters, out _);
        }
        catch
        {
            return null;
        }
    }

    public long? GetUserIdFromToken(string token)
    {
        var principal = ValidateAccessToken(token);
        var idClaim = principal?.FindFirst(ClaimTypes.NameIdentifier);
        return long.TryParse(idClaim?.Value, out var id) ? id : null;
    }
}