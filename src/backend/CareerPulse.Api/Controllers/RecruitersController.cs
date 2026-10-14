using CareerPulse.Application.DTOs.Recruiters;
using CareerPulse.Application.Features.Recruiters.Queries.GetRecruiterById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CareerPulse.Api.Controllers;

/// <summary>
///  REST API controller for Recruiter entity management.
/// </summary>
[Route("api/companies/{companyId:guid}/recruiters")]
[ApiController]
public class RecruitersController : ControllerBase
{
    private readonly ISender _mediator;

    public RecruitersController(ISender mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets a Recruiter by ID.
    /// </summary>
    [HttpGet("{recruiterId:guid}")]
    [ProducesResponseType(typeof(RecruiterDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RecruiterDto>> GetById(
        Guid recruiterId,
        Guid companyId,
        CancellationToken ct)
    {
        var query = new GetRecruiterByIdQuery(companyId, recruiterId);
        var result = await _mediator.Send(query, ct);
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }
}
