using System.ComponentModel.DataAnnotations;

namespace SATPrep.Api.DTOs;

public class UserCreateDto
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(6), MaxLength(200)]
    public string Password { get; set; } = string.Empty;
}
