using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SATPrep.Api.Configurations;
using SATPrep.Api.DTOs;
using SATPrep.Api.Services;

namespace SATPrep.Api.Controllers;

[Route("api/topics")]
[ApiController]
[Authorize]
public class TopicsController : ApiControllerBase
{
    private readonly ITopicService _topicService;

    public TopicsController(ITopicService topicService)
    {
        _topicService = topicService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<TopicGetDto>>> GetAll()
    {
        var topics = await _topicService.GetAllAsync();
        return Ok(topics);
    }

    [HttpGet("{id:long}")]
    [AllowAnonymous]
    public async Task<ActionResult<TopicGetDto>> GetById(long id)
    {
        var topic = await _topicService.GetByIdAsync(id);
        if (topic is null)
            return NotFound();
        return Ok(topic);
    }

    [HttpGet("subject/{subjectId:long}")]
    [AllowAnonymous]
    public async Task<ActionResult<List<TopicGetDto>>> GetBySubject(long subjectId)
    {
        var topics = await _topicService.GetBySubjectAsync(subjectId);
        return Ok(topics);
    }

    [HttpPost]
    [Authorize(Policy = AuthPolicies.AdminOnly)]
    public async Task<ActionResult<TopicGetDto>> Create([FromBody] TopicCreateDto createDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var topic = await _topicService.CreateAsync(createDto);
        if (topic is null)
            return BadRequest("Failed to create topic.");

        return CreatedAtAction(nameof(GetById), new { id = topic.TopicId }, topic);
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = AuthPolicies.AdminOnly)]
    public async Task<ActionResult<TopicGetDto>> Update(long id, [FromBody] TopicUpdateDto updateDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var topic = await _topicService.UpdateAsync(id, updateDto);
        if (topic is null)
            return NotFound();

        return Ok(topic);
    }

    [HttpDelete("{id:long}")]
    [Authorize(Policy = AuthPolicies.AdminOnly)]
    public async Task<IActionResult> Delete(long id)
    {
        var result = await _topicService.DeleteAsync(id);
        if (!result)
            return NotFound();
        return NoContent();
    }
}