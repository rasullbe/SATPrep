using System;

namespace SATPrep.Api.DTOs;

public record AuthTokenBundle(string AccessToken, DateTime AccessTokenExpiresAt, string RefreshToken, DateTime RefreshTokenExpiresAt);

public record AuthSessionResponseDto(long UserId, string Name, string Email, string Role, DateTime AccessTokenExpiresAt);