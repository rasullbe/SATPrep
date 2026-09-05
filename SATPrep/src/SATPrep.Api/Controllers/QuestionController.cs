using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SATPrep.Api.DTOs;
using SATPrep.Api.Exceptions;
using SATPrep.Api.Services;

namespace SATPrep.Api.Controllers
{
    [Route("api/questions")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionService _questionService;

        public QuestionController(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<QuestionGetDto>> GetById(long id)
        {
            try
            {
                var question = await _questionService.GetByIdAsync(id);
                if (question is null)
                    return NotFound();
                return Ok(question);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("admin/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<QuestionAdminDto>> GetByIdAdmin(long id)
        {
            try
            {
                var question = await _questionService.GetByIdAdminAsync(id);
                if (question is null)
                    return NotFound();
                return Ok(question);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<QuestionGetDto>>> GetAll()
        {
            try
            {
                var questions = await _questionService.GetAllAsync();
                return Ok(questions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("topic/{topicId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<QuestionGetDto>>> GetByTopic(long topicId)
        {
            try
            {
                var questions = await _questionService.GetByTopicAsync(topicId);
                return Ok(questions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<QuestionGetDto>> Create([FromBody] QuestionCreateDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var question = await _questionService.CreateAsync(createDto);
                if (question is null)
                    return BadRequest("Failed to create question");

                return CreatedAtAction(nameof(GetById), new { id = question.QuestionId }, question);
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
        public async Task<ActionResult<QuestionGetDto>> Update(long id, [FromBody] QuestionUpdateDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var question = await _questionService.UpdateAsync(id, updateDto);
                if (question is null)
                    return NotFound();

                return Ok(question);
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
                var result = await _questionService.DeleteAsync(id);
                if (!result)
                    return NotFound();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{questionId}/tags/{tagId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> AddTag(long questionId, long tagId)
        {
            try
            {
                var result = await _questionService.AddTagAsync(questionId, tagId);
                if (!result)
                    return BadRequest("Failed to add tag to question");
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{questionId}/tags/{tagId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> RemoveTag(long questionId, long tagId)
        {
            try
            {
                var result = await _questionService.RemoveTagAsync(questionId, tagId);
                if (!result)
                    return BadRequest("Failed to remove tag from question");
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
