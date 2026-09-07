using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SATPrep.Api.DTOs;
using SATPrep.Api.Services;

namespace SATPrep.Api.Controllers
{
    [Route("api/quizzes")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quizService;

        public QuizController(IQuizService quizService)
        {
            _quizService = quizService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<QuizGetDto>> GetById(long id)
        {
            var quiz = await _quizService.GetByIdAsync(id);
            if (quiz is null)
                return NotFound();
            return Ok(quiz);
        }

        [HttpGet("details/{id}")]
        public async Task<ActionResult<QuizDetailGetDto>> GetDetailById(long id)
        {
            var quiz = await _quizService.GetDetailByIdAsync(id);
            if (quiz is null)
                return NotFound();
            return Ok(quiz);
        }

        [HttpGet]
        public async Task<ActionResult<List<QuizGetDto>>> GetAll()
        {
            var quizzes = await _quizService.GetAllAsync();
            return Ok(quizzes);
        }

        [HttpGet("creator/{creatorId}")]
        public async Task<ActionResult<List<QuizGetDto>>> GetByCreator(long creatorId)
        {
            var quizzes = await _quizService.GetByCreatorAsync(creatorId);
            return Ok(quizzes);
        }

        [HttpPost]
        public async Task<ActionResult<QuizGetDto>> Create([FromBody] QuizCreateDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var quiz = await _quizService.CreateAsync(createDto);
            if (quiz is null)
                return BadRequest("Failed to create quiz");

            return CreatedAtAction(nameof(GetById), new { id = quiz.QuizId }, quiz);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<QuizGetDto>> Update(long id, [FromBody] QuizUpdateDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var quiz = await _quizService.UpdateAsync(id, updateDto);
            if (quiz is null)
                return NotFound();

            return Ok(quiz);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(long id)
        {
            var result = await _quizService.DeleteAsync(id);
            if (!result)
                return NotFound();
            return NoContent();
        }

        [HttpPost("{quizId}/questions/{questionId}")]
        public async Task<ActionResult> AddQuestion(long quizId, long questionId, [FromQuery] int order)
        {
            var result = await _quizService.AddQuestionAsync(quizId, questionId, order);
            if (!result)
                return BadRequest("Failed to add question to quiz");
            return NoContent();
        }

        [HttpDelete("{quizId}/questions/{questionId}")]
        public async Task<ActionResult> RemoveQuestion(long quizId, long questionId)
        {
            var result = await _quizService.RemoveQuestionAsync(quizId, questionId);
            if (!result)
                return BadRequest("Failed to remove question from quiz");
            return NoContent();
        }
    }
}