using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SATPrep.Api.Configurations;
using SATPrep.Api.DTOs;
using SATPrep.Api.Services;

namespace SATPrep.Api.Controllers;

[Route("api/auth")]
[ApiController]
[Authorize]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthSessionResponseDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Register([FromBody] UserCreateDto dto)
    {
        var session = await _authService.RegisterAsync(dto);
        return StatusCode(StatusCodes.Status201Created, session);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthSessionResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
    {
        var session = await _authService.LoginAsync(dto, HttpContext);
        return Ok(session);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh()
    {
        await _authService.RefreshAsync(HttpContext);
        return Ok(new { Message = "Token refreshed." });
    }

    [HttpPost("logout")]
    [AllowAnonymous]
    public async Task<IActionResult> Logout()
    {
        await _authService.LogoutAsync(HttpContext);
        return Ok(new { Message = "Logged out." });
    }

    [HttpGet("me")]
    [ProducesResponseType(typeof(AuthSessionResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMe()
    {
        var session = await _authService.GetMeAsync(HttpContext);
        return Ok(session);
    }

    [HttpGet("csrf")]
    [AllowAnonymous]
    public IActionResult GetCsrf()
    {
        var options = HttpContext.RequestServices
            .GetRequiredService<Microsoft.Extensions.Options.IOptions<Configurations.Settings.JwtOptions>>();

        var cookieExists = HttpContext.Request.Cookies.ContainsKey(options.Value.CsrfCookieName);
        if (!cookieExists)
        {
            var cookieWriter = HttpContext.RequestServices.GetRequiredService<ICookieAuthWriter>();
            cookieWriter.WriteCsrfCookie(HttpContext);
        }

        return Ok(new { Message = "CSRF cookie set." });
    }
}