using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SATPrep.Api.DTOs;
using SATPrep.Api.Services;

namespace SATPrep.Api.Controllers;

[Route("api/quiz-attempts")]
[ApiController]
[Authorize]
public class QuizAttemptsController : ApiControllerBase
{
    private readonly IQuizAttemptService _attemptService;

    public QuizAttemptsController(IQuizAttemptService attemptService)
    {
        _attemptService = attemptService;
    }

    [HttpGet]
    public async Task<ActionResult<List<QuizAttemptGetDto>>> GetAll()
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var attempts = await _attemptService.GetByUserAsync(userId);
        return Ok(attempts);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<QuizAttemptGetDto>> GetById(long id)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var attempt = await _attemptService.GetByIdAsync(id);
        if (attempt is null || attempt.UserId != userId)
            return NotFound();

        return Ok(attempt);
    }

    [HttpPost]
    public async Task<ActionResult<QuizAttemptGetDto>> Start([FromBody] CreateQuizAttemptDto dto)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var attempt = await _attemptService.StartAsync(userId, dto.QuizId);
        if (attempt is null)
            return BadRequest("Failed to start quiz attempt. Quiz may not exist.");

        return StatusCode(StatusCodes.Status201Created, attempt);
    }

    [HttpPost("{id:long}/complete")]
    public async Task<ActionResult<QuizAttemptResultDto>> Complete(long id, [FromBody] CompleteQuizAttemptDto dto)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var result = await _attemptService.CompleteAsync(id, userId, dto);
        if (result is null)
            return NotFound("Attempt not found, already completed, or you do not own this attempt.");

        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var attempt = await _attemptService.GetByIdAsync(id);
        if (attempt is null || attempt.UserId != userId)
            return NotFound();

        var result = await _attemptService.DeleteAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }
}