using System.Collections.Generic;

namespace SATPrep.Api.DTOs;

public class ApiFieldError
{
    public string Field { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public class ApiErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<ApiFieldError> Errors { get; set; } = new();
    public string? TraceId { get; set; }
    public string Timestamp { get; set; } = System.DateTime.UtcNow.ToString("o");
}