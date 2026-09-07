using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SATPrep.Api.DTOs;
using SATPrep.Api.Services;

namespace SATPrep.Api.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<ActionResult<UserGetDto>> Register([FromBody] UserCreateDto dto)
        {
            var user = await _userService.RegisterAsync(dto);
            return StatusCode(StatusCodes.Status201Created, user);
        }

        [HttpPut("{id:long}")]
        public async Task<ActionResult<UserGetDto>> Update(long id, [FromBody] UserUpdateDto dto)
        {
            var user = await _userService.UpdateAsync(id, dto);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            var deleted = await _userService.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<UserGetDto>> GetUserById(long id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpGet("by-email")]
        public async Task<ActionResult<UserGetDto>> GetByEmail([FromQuery] string email)
        {
            var user = await _userService.GetByEmailAsync(email);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpGet("email-exists")]
        public async Task<ActionResult<bool>> EmailExists([FromQuery] string email)
        {
            var exists = await _userService.EmailExistsAsync(email);
            return Ok(exists);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserGetDto>>> ListUsers()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        [HttpPost("auth")]
        public async Task<ActionResult<UserGetDto>> AuthenticateUser([FromBody] UserLoginDto dto)
        {
            var user = await _userService.AuthenticateAsync(dto.Email, dto.Password);
            if (user == null)
            {
                return Unauthorized();
            }
            return Ok(user);
        }

        [HttpPut("{id:long}/password")]
        public async Task<IActionResult> ChangePassword(long id, [FromBody] ChangePasswordDto dto)
        {
            var changed = await _userService.ChangePasswordAsync(id, dto.CurrentPassword, dto.NewPassword);
            if (!changed)
            {
                return BadRequest();
            }
            return NoContent();
        }
    }
}