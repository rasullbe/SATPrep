using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SATPrep.Api.Configurations;
using SATPrep.Api.DTOs;
using SATPrep.Api.Services;

namespace SATPrep.Api.Controllers;

[Route("api/users")]
[ApiController]
public class UsersController : ApiControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("{id:long}")]
    [Authorize]
    public async Task<ActionResult<UserGetDto>> GetUserById(long id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpGet("by-email")]
    public async Task<ActionResult<UserGetDto>> GetByEmail([FromQuery] string email)
    {
        var user = await _userService.GetByEmailAsync(email);
        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpGet("email-exists")]
    public async Task<ActionResult<bool>> EmailExists([FromQuery] string email)
    {
        var exists = await _userService.EmailExistsAsync(email);
        return Ok(exists);
    }

    [HttpGet]
    [Authorize(Policy = AuthPolicies.AdminOnly)]
    public async Task<ActionResult<IEnumerable<UserGetDto>>> ListUsers()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }

    [HttpPut("{id:long}")]
    [Authorize]
    public async Task<ActionResult<UserGetDto>> Update(long id, [FromBody] UserUpdateDto dto)
    {
        if (!TryGetUserId(out var userId) || userId != id)
            return Forbid();

        var user = await _userService.UpdateAsync(id, dto);
        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpDelete("{id:long}")]
    [Authorize]
    public async Task<IActionResult> Delete(long id)
    {
        if (!TryGetUserId(out var userId) || userId != id)
            return Forbid();

        var deleted = await _userService.DeleteAsync(id);
        if (!deleted)
            return NotFound();

        return NoContent();
    }

    [HttpPut("{id:long}/password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword(long id, [FromBody] ChangePasswordDto dto)
    {
        if (!TryGetUserId(out var userId) || userId != id)
            return Forbid();

        var changed = await _userService.ChangePasswordAsync(id, dto.CurrentPassword, dto.NewPassword);
        if (!changed)
            return BadRequest();

        return NoContent();
    }
}