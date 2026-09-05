using System.ComponentModel.DataAnnotations;

namespace SATPrep.Api.DTOs;

public class TopicUpdateDto
{
    [MaxLength(200)]
    public string? Name { get; set; }
}
