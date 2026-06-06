using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PollingSystem.Application.DTOs;
using PollingSystem.Application.Services;
using PollingSystem.Domain.Enums;

namespace PollingSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PollsController(PollService pollService) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<PollSummaryDto>> Create([FromBody] CreatePollRequest request, CancellationToken ct)
    {
        var result = await pollService.CreatePollAsync(request, ct);
        return CreatedAtAction(nameof(GetResults), new { id = result.Id }, result);
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IReadOnlyList<PollSummaryDto>>> List(CancellationToken ct)
        => Ok(await pollService.ListPollsAsync(ct));

    [HttpPost("{id:guid}/vote")]
    [Authorize(Roles = nameof(UserRole.User))]
    public async Task<IActionResult> Vote(Guid id, [FromBody] VoteRequest request, CancellationToken ct)
    {
        var voterId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException();

        await pollService.VoteAsync(id, request.OptionId, voterId, ct);
        return NoContent();
    }

    [HttpGet("{id:guid}/results")]
    [Authorize(Roles = $"{nameof(UserRole.Analyst)},{nameof(UserRole.Admin)}")]
    public async Task<ActionResult<PollResultsDto>> GetResults(Guid id, CancellationToken ct)
        => Ok(await pollService.GetResultsAsync(id, ct));
}
