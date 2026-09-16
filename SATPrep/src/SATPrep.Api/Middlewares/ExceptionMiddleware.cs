using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using SATPrep.Api.DTOs;
using SATPrep.Api.Exceptions;

namespace SATPrep.Api.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            context.Response.StatusCode = 499;
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var response = new ApiErrorResponse
        {
            StatusCode = (int)System.Net.HttpStatusCode.InternalServerError,
            Message = "Something went wrong. Please try again later.",
            TraceId = context.TraceIdentifier
        };

        if (ex is ApiException apiEx)
        {
            response.StatusCode = apiEx.StatusCode;
            response.Message = apiEx.Message;

            if (ex is BadRequestException badRequestEx && !string.IsNullOrEmpty(badRequestEx.Field))
                response.Errors.Add(new ApiFieldError { Field = badRequestEx.Field, Message = badRequestEx.Message });
            else if (ex is ConflictException conflictEx && !string.IsNullOrEmpty(conflictEx.Field))
                response.Errors.Add(new ApiFieldError { Field = conflictEx.Field, Message = conflictEx.Message });
        }
        else
        {
            _logger.LogError(ex, "Unhandled exception");
        }

        context.Response.StatusCode = response.StatusCode;
        await context.Response.WriteAsJsonAsync(response);
    }
}