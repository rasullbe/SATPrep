using System.Net;
using System.Text.Json;
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
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        int statusCode;
        string message;

        if (ex is ApiException apiEx)
        {
            statusCode = apiEx.StatusCode;
            message = apiEx.Message;
        }
        else
        {
            statusCode = 500;
            message = "Something went wrong. Please try again later.";
            _logger.LogError(ex, "Unhandled exception");
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var response = new { message };
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}