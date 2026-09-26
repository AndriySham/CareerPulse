using CareerPulse.Application.DTOs.WorkExperiences;
using CareerPulse.Application.Features.WorkExperience.Queries.GetWorkExperienceById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CareerPulse.Api.Controllers;

/// <summary>
/// REST API controller for WorkExperience entity management.
/// </summary>
[Route("api/work-experience")]
[ApiController]
public class WorkExperiencesController : ControllerBase
{
    private readonly ISender _mediator;

    public WorkExperiencesController(ISender mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Query for retrieving WorkExperience entity.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<WorkExperienceDto>> GetById(
        Guid id,
        CancellationToken ct)
    {
        var query = new GetWorkExperienceByIdQuery(id);
        var result = await _mediator.Send(query, ct);
        return Ok(result);
    }
}
