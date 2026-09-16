using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SATPrep.Api.DTOs;
using SATPrep.Api.Services;

namespace SATPrep.Api.Controllers;

[Route("api/stats")]
[ApiController]
[Authorize]
public class StatsController : ApiControllerBase
{
    private readonly IStudyService _studyService;

    public StatsController(IStudyService studyService)
    {
        _studyService = studyService;
    }

    [HttpGet("dashboard")]
    [ProducesResponseType(typeof(DashboardDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDashboard()
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var dashboard = await _studyService.GetDashboardAsync(userId);
        return Ok(dashboard);
    }
}