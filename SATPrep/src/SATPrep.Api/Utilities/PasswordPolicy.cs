using System.Text.RegularExpressions;
using SATPrep.Api.Exceptions;

namespace SATPrep.Api.Utilities;

public class PasswordPolicy
{
    public const int MaxLength = 72;

    public static void Validate(string password, string username)
    {
        if (string.IsNullOrEmpty(password) || password.Length < 8)
            throw new BadRequestException("Password must be at least 8 characters.", "password");

        // BCrypt silently truncates beyond 72 bytes — reject early to avoid false confidence.
        if (System.Text.Encoding.UTF8.GetByteCount(password) > MaxLength)
            throw new BadRequestException($"Password must be at most {MaxLength} bytes.", "password");

        if (!Regex.IsMatch(password, @"[A-Z]"))
            throw new BadRequestException("Password must contain at least one uppercase letter.", "password");

        if (!Regex.IsMatch(password, @"[0-9]"))
            throw new BadRequestException("Password must contain at least one number.", "password");

        if (!string.IsNullOrEmpty(username) &&
            password.Contains(username, StringComparison.OrdinalIgnoreCase))
            throw new BadRequestException("Password cannot contain your username.", "password");
    }
}
