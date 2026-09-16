using Microsoft.Extensions.Options;
using SATPrep.Api.Configurations.Settings;
using SATPrep.Api.Utilities;

namespace SATPrep.Api.Services;

public interface ICookieAuthWriter
{
    void WriteAuthCookies(HttpContext httpContext, string accessToken, string refreshToken, DateTime accessExpiresAt, DateTime refreshExpiresAt);
    void WriteCsrfCookie(HttpContext httpContext);
    void ClearAuthCookies(HttpContext httpContext);
    void WriteLoggedInCookie(HttpContext httpContext, bool loggedIn);
}

public class CookieAuthWriter : ICookieAuthWriter
{
    private readonly JwtOptions _options;

    public CookieAuthWriter(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public void WriteAuthCookies(HttpContext httpContext, string accessToken, string refreshToken, DateTime accessExpiresAt, DateTime refreshExpiresAt)
    {
        httpContext.Response.Cookies.Append(_options.CookieName, accessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = httpContext.Request.IsHttps,
            SameSite = SameSiteMode.Lax,
            Expires = accessExpiresAt
        });

        httpContext.Response.Cookies.Append(_options.RefreshCookieName, refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = httpContext.Request.IsHttps,
            SameSite = SameSiteMode.Strict,
            Path = "/api/auth",
            Expires = refreshExpiresAt
        });

        WriteLoggedInCookie(httpContext, true);
    }

    public void WriteCsrfCookie(HttpContext httpContext)
    {
        if (httpContext.Request.Cookies.ContainsKey(_options.CsrfCookieName))
            return;

        var token = TokenHasher.GenerateCsrfToken();
        httpContext.Response.Cookies.Append(_options.CsrfCookieName, token, new CookieOptions
        {
            HttpOnly = false,
            Secure = httpContext.Request.IsHttps,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddDays(90)
        });
    }

    public void ClearAuthCookies(HttpContext httpContext)
    {
        httpContext.Response.Cookies.Delete(_options.CookieName);
        httpContext.Response.Cookies.Delete(_options.RefreshCookieName);
        WriteLoggedInCookie(httpContext, false);
    }

    public void WriteLoggedInCookie(HttpContext httpContext, bool loggedIn)
    {
        httpContext.Response.Cookies.Append(_options.LoggedInCookieName, loggedIn ? "1" : "0", new CookieOptions
        {
            HttpOnly = false,
            Secure = httpContext.Request.IsHttps,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddDays(loggedIn ? 90 : -1)
        });
    }
}