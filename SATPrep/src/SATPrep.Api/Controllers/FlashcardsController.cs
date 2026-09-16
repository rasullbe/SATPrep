using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SATPrep.Api.DTOs;
using SATPrep.Api.Services;

namespace SATPrep.Api.Controllers;

[Route("api/flashcards")]
[ApiController]
[Authorize]
public class FlashcardsController : ApiControllerBase
{
    private readonly IFlashcardService _flashcardService;

    public FlashcardsController(IFlashcardService flashcardService)
    {
        _flashcardService = flashcardService;
    }

    [HttpGet]
    public async Task<ActionResult<List<FlashcardGetDto>>> GetAll()
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var flashcards = await _flashcardService.GetByUserAsync(userId);
        return Ok(flashcards);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<FlashcardGetDto>> GetById(long id)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var flashcard = await _flashcardService.GetByIdAsync(id);
        if (flashcard is null || flashcard.UserId != userId)
            return NotFound();

        return Ok(flashcard);
    }

    [HttpPost]
    public async Task<ActionResult<FlashcardGetDto>> Create([FromBody] FlashcardCreateDto createDto)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        createDto.UserId = userId;

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var flashcard = await _flashcardService.CreateAsync(createDto);
        if (flashcard is null)
            return BadRequest("Failed to create flashcard.");

        return CreatedAtAction(nameof(GetById), new { id = flashcard.FlashcardId }, flashcard);
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<FlashcardGetDto>> Update(long id, [FromBody] FlashcardUpdateDto updateDto)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var existing = await _flashcardService.GetByIdAsync(id);
        if (existing is null || existing.UserId != userId)
            return NotFound();

        var flashcard = await _flashcardService.UpdateAsync(id, updateDto);
        if (flashcard is null)
            return NotFound();

        return Ok(flashcard);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var existing = await _flashcardService.GetByIdAsync(id);
        if (existing is null || existing.UserId != userId)
            return NotFound();

        var result = await _flashcardService.DeleteAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }

    [HttpPost("{id:long}/review")]
    public async Task<ActionResult<FlashcardGetDto>> Review(long id, [FromBody] FlashcardReviewDto reviewDto)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var existing = await _flashcardService.GetByIdAsync(id);
        if (existing is null || existing.UserId != userId)
            return NotFound();

        var flashcard = await _flashcardService.ReviewAsync(id, reviewDto);
        if (flashcard is null)
            return NotFound();

        return Ok(flashcard);
    }
}