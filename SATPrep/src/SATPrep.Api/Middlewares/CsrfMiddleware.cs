using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using SATPrep.Api.Configurations.Settings;

namespace SATPrep.Api.Middlewares;

public class CsrfMiddleware
{
    private readonly RequestDelegate _next;
    private readonly JwtOptions _jwtOptions;

    public CsrfMiddleware(RequestDelegate next, IOptions<JwtOptions> jwtOptions)
    {
        _next = next;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var isMutatingRequest = HttpMethods.IsPost(context.Request.Method)
            || HttpMethods.IsPut(context.Request.Method)
            || HttpMethods.IsPatch(context.Request.Method)
            || HttpMethods.IsDelete(context.Request.Method);

        if (isMutatingRequest)
        {
            var tokenFromCookie = context.Request.Cookies[_jwtOptions.CsrfCookieName];
            var tokenFromHeader = context.Request.Headers["X-CSRF-Token"].ToString();

            if (string.IsNullOrEmpty(tokenFromCookie) ||
                string.IsNullOrEmpty(tokenFromHeader) ||
                !CryptographicOperations.FixedTimeEquals(
                    System.Text.Encoding.UTF8.GetBytes(tokenFromCookie),
                    System.Text.Encoding.UTF8.GetBytes(tokenFromHeader)))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new { Message = "Invalid CSRF token." });
                return;
            }
        }

        await _next(context);
    }
}