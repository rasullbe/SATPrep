using Microsoft.AspNetCore.Mvc;
using SATPrep.Api.DTOs;
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
        public async Task<ActionResult<QuestionGetDto>> GetById(long id)
        {
            var question = await _questionService.GetByIdAsync(id);
            if (question is null)
                return NotFound();
            return Ok(question);
        }

        [HttpGet("admin/{id}")]
        public async Task<ActionResult<QuestionAdminDto>> GetByIdAdmin(long id)
        {
            var question = await _questionService.GetByIdAdminAsync(id);
            if (question is null)
                return NotFound();
            return Ok(question);
        }

        [HttpGet]
        public async Task<ActionResult<List<QuestionGetDto>>> GetAll()
        {
            var questions = await _questionService.GetAllAsync();
            return Ok(questions);
        }

        [HttpGet("topic/{topicId}")]
        public async Task<ActionResult<List<QuestionGetDto>>> GetByTopic(long topicId)
        {
            var questions = await _questionService.GetByTopicAsync(topicId);
            return Ok(questions);
        }

        [HttpPost]
        public async Task<ActionResult<QuestionGetDto>> Create([FromBody] QuestionCreateDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var question = await _questionService.CreateAsync(createDto);
            if (question is null)
                return BadRequest("Failed to create question");

            return CreatedAtAction(nameof(GetById), new { id = question.QuestionId }, question);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<QuestionGetDto>> Update(long id, [FromBody] QuestionUpdateDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var question = await _questionService.UpdateAsync(id, updateDto);
            if (question is null)
                return NotFound();

            return Ok(question);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(long id)
        {
            var result = await _questionService.DeleteAsync(id);
            if (!result)
                return NotFound();
            return NoContent();
        }

        [HttpPost("{questionId}/tags/{tagId}")]
        public async Task<ActionResult> AddTag(long questionId, long tagId)
        {
            var result = await _questionService.AddTagAsync(questionId, tagId);
            if (!result)
                return BadRequest("Failed to add tag to question");
            return NoContent();
        }

        [HttpDelete("{questionId}/tags/{tagId}")]
        public async Task<ActionResult> RemoveTag(long questionId, long tagId)
        {
            var result = await _questionService.RemoveTagAsync(questionId, tagId);
            if (!result)
                return BadRequest("Failed to remove tag from question");
            return NoContent();
        }
    }
}