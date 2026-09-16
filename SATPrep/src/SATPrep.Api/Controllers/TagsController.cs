using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SATPrep.Api.Configurations;
using SATPrep.Api.DTOs;
using SATPrep.Api.Services;

namespace SATPrep.Api.Controllers;

[Route("api/tags")]
[ApiController]
[Authorize]
public class TagsController : ApiControllerBase
{
    private readonly ITagService _tagService;

    public TagsController(ITagService tagService)
    {
        _tagService = tagService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<TagGetDto>>> GetAll()
    {
        var tags = await _tagService.GetAllAsync();
        return Ok(tags);
    }

    [HttpGet("{id:long}")]
    [AllowAnonymous]
    public async Task<ActionResult<TagGetDto>> GetById(long id)
    {
        var tag = await _tagService.GetByIdAsync(id);
        if (tag is null)
            return NotFound();
        return Ok(tag);
    }

    [HttpPost]
    [Authorize(Policy = AuthPolicies.AdminOnly)]
    public async Task<ActionResult<TagGetDto>> Create([FromBody] TagCreateDto createDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var tag = await _tagService.CreateAsync(createDto);
        if (tag is null)
            return BadRequest("Failed to create tag.");

        return CreatedAtAction(nameof(GetById), new { id = tag.TagId }, tag);
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = AuthPolicies.AdminOnly)]
    public async Task<ActionResult<TagGetDto>> Update(long id, [FromBody] TagUpdateDto updateDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var tag = await _tagService.UpdateAsync(id, updateDto);
        if (tag is null)
            return NotFound();

        return Ok(tag);
    }

    [HttpDelete("{id:long}")]
    [Authorize(Policy = AuthPolicies.AdminOnly)]
    public async Task<IActionResult> Delete(long id)
    {
        var result = await _tagService.DeleteAsync(id);
        if (!result)
            return NotFound();
        return NoContent();
    }
}