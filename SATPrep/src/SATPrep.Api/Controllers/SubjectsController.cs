using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SATPrep.Api.Configurations;
using SATPrep.Api.DTOs;
using SATPrep.Api.Services;

namespace SATPrep.Api.Controllers;

[Route("api/subjects")]
[ApiController]
[Authorize]
public class SubjectsController : ApiControllerBase
{
    private readonly ISubjectService _subjectService;

    public SubjectsController(ISubjectService subjectService)
    {
        _subjectService = subjectService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<SubjectGetDto>>> GetAll()
    {
        var subjects = await _subjectService.GetAllAsync();
        return Ok(subjects);
    }

    [HttpGet("{id:long}")]
    [AllowAnonymous]
    public async Task<ActionResult<SubjectGetDto>> GetById(long id)
    {
        var subject = await _subjectService.GetByIdAsync(id);
        if (subject is null)
            return NotFound();
        return Ok(subject);
    }

    [HttpGet("by-name")]
    public async Task<ActionResult<SubjectGetDto>> GetByName([FromQuery] string name)
    {
        var subject = await _subjectService.GetByNameAsync(name);
        if (subject is null)
            return NotFound();
        return Ok(subject);
    }

    [HttpPost]
    [Authorize(Policy = AuthPolicies.AdminOnly)]
    public async Task<ActionResult<SubjectGetDto>> Create([FromBody] SubjectCreateDto createDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var subject = await _subjectService.CreateAsync(createDto);
        if (subject is null)
            return BadRequest("Failed to create subject.");

        return CreatedAtAction(nameof(GetById), new { id = subject.SubjectId }, subject);
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = AuthPolicies.AdminOnly)]
    public async Task<ActionResult<SubjectGetDto>> Update(long id, [FromBody] SubjectUpdateDto updateDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var subject = await _subjectService.UpdateAsync(id, updateDto);
        if (subject is null)
            return NotFound();

        return Ok(subject);
    }

    [HttpDelete("{id:long}")]
    [Authorize(Policy = AuthPolicies.AdminOnly)]
    public async Task<IActionResult> Delete(long id)
    {
        var result = await _subjectService.DeleteAsync(id);
        if (!result)
            return NotFound();
        return NoContent();
    }
}