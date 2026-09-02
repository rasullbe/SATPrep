using System.ComponentModel.DataAnnotations;

namespace SATPrep.Api.DTOs;

public class UserUpdateDto
{
    [MaxLength(100)]
    public string? Name { get; set; }

    [EmailAddress, MaxLength(200)]
    public string? Email { get; set; }

    // Password changes should be handled via a dedicated endpoint
    [MinLength(6), MaxLength(200)]
    public string? NewPassword { get; set; }
}
