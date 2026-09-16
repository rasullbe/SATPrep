using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SATPrep.Api.DTOs;
using SATPrep.Api.Exceptions;

namespace SATPrep.Api.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected bool TryGetUserId(out long userId)
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return long.TryParse(idClaim, out userId);
    }

    protected (int Skip, int Take) ClampPaging(int skip, int take, int defaultTake = 20, int maxTake = 50)
    {
        skip = Math.Max(0, skip);
        take = take <= 0 ? defaultTake : Math.Clamp(take, 1, maxTake);
        return (skip, take);
    }

    protected ApiErrorResponse Error(int statusCode, string message, string? field = null, string? code = null)
    {
        var response = new ApiErrorResponse
        {
            StatusCode = statusCode,
            Message = message,
            TraceId = HttpContext.TraceIdentifier
        };

        if (!string.IsNullOrEmpty(field))
            response.Errors.Add(new ApiFieldError { Field = field, Message = message });

        return response;
    }
}