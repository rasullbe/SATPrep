using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SATPrep.Api.Configurations;
using SATPrep.Api.DTOs;
using SATPrep.Api.Services;

namespace SATPrep.Api.Controllers;

[Route("api/quizzes")]
[ApiController]
[Authorize]
public class QuizController : ApiControllerBase
{
    private readonly IQuizService _quizService;

    public QuizController(IQuizService quizService)
    {
        _quizService = quizService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<QuizGetDto>>> GetAll()
    {
        var quizzes = await _quizService.GetAllAsync();
        return Ok(quizzes);
    }

    [HttpGet("published")]
    [AllowAnonymous]
    public async Task<ActionResult<List<QuizGetDto>>> GetPublished()
    {
        var quizzes = await _quizService.GetPublishedAsync();
        return Ok(quizzes);
    }

    [HttpGet("{id:long}")]
    [AllowAnonymous]
    public async Task<ActionResult<QuizGetDto>> GetById(long id)
    {
        var quiz = await _quizService.GetByIdAsync(id);
        if (quiz is null)
            return NotFound();
        return Ok(quiz);
    }

    [HttpGet("details/{id:long}")]
    public async Task<ActionResult<QuizDetailGetDto>> GetDetailById(long id)
    {
        var quiz = await _quizService.GetDetailByIdAsync(id);
        if (quiz is null)
            return NotFound();
        return Ok(quiz);
    }

    [HttpGet("take/{id:long}")]
    public async Task<ActionResult<QuizTakeDto>> GetTakeData(long id)
    {
        var quiz = await _quizService.GetTakeDataAsync(id);
        if (quiz is null)
            return NotFound();
        return Ok(quiz);
    }

    [HttpGet("creator/{creatorId:long}")]
    public async Task<ActionResult<List<QuizGetDto>>> GetByCreator(long creatorId)
    {
        if (!TryGetUserId(out var userId) || userId != creatorId)
            return Forbid();

        var quizzes = await _quizService.GetByCreatorAsync(creatorId);
        return Ok(quizzes);
    }

    [HttpPost]
    public async Task<ActionResult<QuizGetDto>> Create([FromBody] QuizCreateDto createDto)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        createDto.CreatedById = userId;

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var quiz = await _quizService.CreateAsync(createDto);
        if (quiz is null)
            return BadRequest("Failed to create quiz");

        return CreatedAtAction(nameof(GetById), new { id = quiz.QuizId }, quiz);
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<QuizGetDto>> Update(long id, [FromBody] QuizUpdateDto updateDto)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var quiz = await _quizService.GetByIdAsync(id);
        if (quiz is null)
            return NotFound();
        if (quiz.CreatedById != userId)
            return Forbid();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var updated = await _quizService.UpdateAsync(id, updateDto);
        if (updated is null)
            return NotFound();

        return Ok(updated);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var quiz = await _quizService.GetByIdAsync(id);
        if (quiz is null)
            return NotFound();
        if (quiz.CreatedById != userId)
            return Forbid();

        var result = await _quizService.DeleteAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }

    [HttpPost("{quizId:long}/questions/{questionId:long}")]
    public async Task<IActionResult> AddQuestion(long quizId, long questionId, [FromQuery] int order)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var quiz = await _quizService.GetByIdAsync(quizId);
        if (quiz is null)
            return NotFound();
        if (quiz.CreatedById != userId)
            return Forbid();

        var result = await _quizService.AddQuestionAsync(quizId, questionId, order);
        if (!result)
            return BadRequest("Failed to add question to quiz");
        return NoContent();
    }

    [HttpDelete("{quizId:long}/questions/{questionId:long}")]
    public async Task<IActionResult> RemoveQuestion(long quizId, long questionId)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var quiz = await _quizService.GetByIdAsync(quizId);
        if (quiz is null)
            return NotFound();
        if (quiz.CreatedById != userId)
            return Forbid();

        var result = await _quizService.RemoveQuestionAsync(quizId, questionId);
        if (!result)
            return BadRequest("Failed to remove question from quiz");
        return NoContent();
    }
}