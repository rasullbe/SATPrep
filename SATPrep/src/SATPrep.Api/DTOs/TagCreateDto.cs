using System.ComponentModel.DataAnnotations;

namespace SATPrep.Api.DTOs;

public class TagCreateDto
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;
}
