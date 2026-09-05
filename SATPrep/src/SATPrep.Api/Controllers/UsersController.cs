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
        [ProducesResponseType(typeof(UserGetDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserGetDto>> Register([FromBody] UserCreateDto dto)
        {
            var user = await _userService.RegisterAsync(dto);
            return StatusCode(StatusCodes.Status201Created, user);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(UserGetDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserGetDto>> Update(int id, [FromBody] UserUpdateDto dto)
        {
            var user = await _userService.UpdateAsync(id, dto);
            return Ok(user);
        }
    }
}