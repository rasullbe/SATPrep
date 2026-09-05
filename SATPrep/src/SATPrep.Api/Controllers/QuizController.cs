using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SATPrep.Api.DTOs;
using SATPrep.Api.Exceptions;
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<QuizGetDto>> GetById(long id)
        {
            try
            {
                var quiz = await _quizService.GetByIdAsync(id);
                if (quiz is null)
                    return NotFound();
                return Ok(quiz);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("details/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<QuizDetailGetDto>> GetDetailById(long id)
        {
            try
            {
                var quiz = await _quizService.GetDetailByIdAsync(id);
                if (quiz is null)
                    return NotFound();
                return Ok(quiz);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<QuizGetDto>>> GetAll()
        {
            try
            {
                var quizzes = await _quizService.GetAllAsync();
                return Ok(quizzes);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("creator/{creatorId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<QuizGetDto>>> GetByCreator(long creatorId)
        {
            try
            {
                var quizzes = await _quizService.GetByCreatorAsync(creatorId);
                return Ok(quizzes);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<QuizGetDto>> Create([FromBody] QuizCreateDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var quiz = await _quizService.CreateAsync(createDto);
                if (quiz is null)
                    return BadRequest("Failed to create quiz");

                return CreatedAtAction(nameof(GetById), new { id = quiz.QuizId }, quiz);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<QuizGetDto>> Update(long id, [FromBody] QuizUpdateDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var quiz = await _quizService.UpdateAsync(id, updateDto);
                if (quiz is null)
                    return NotFound();

                return Ok(quiz);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Delete(long id)
        {
            try
            {
                var result = await _quizService.DeleteAsync(id);
                if (!result)
                    return NotFound();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{quizId}/questions/{questionId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> AddQuestion(long quizId, long questionId, [FromQuery] int order)
        {
            try
            {
                var result = await _quizService.AddQuestionAsync(quizId, questionId, order);
                if (!result)
                    return BadRequest("Failed to add question to quiz");
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{quizId}/questions/{questionId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> RemoveQuestion(long quizId, long questionId)
        {
            try
            {
                var result = await _quizService.RemoveQuestionAsync(quizId, questionId);
                if (!result)
                    return BadRequest("Failed to remove question from quiz");
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
