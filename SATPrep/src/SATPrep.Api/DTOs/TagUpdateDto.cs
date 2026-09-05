using System.ComponentModel.DataAnnotations;

namespace SATPrep.Api.DTOs;

public class TagUpdateDto
{
    [MaxLength(100)]
    public string? Name { get; set; }
}
