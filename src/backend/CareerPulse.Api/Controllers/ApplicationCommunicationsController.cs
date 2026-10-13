using CareerPulse.Application.DTOs.ApplicationCommunications;
using CareerPulse.Application.Features.ApplicationCommunications.Queries.GetApplicationCommunicationById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CareerPulse.Api.Controllers;

/// <summary>
/// REST API controller for ApplicationCommunication entity management.
/// </summary>
[Route("api/applications/{applicationId:guid}/communications")]
[ApiController]
public class ApplicationCommunicationsController : ControllerBase
{
    private readonly ISender _mediator;

    public ApplicationCommunicationsController(ISender mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets an ApplicationCommunication by ID.
    /// </summary>
    [HttpGet("{applicationCommunicationId:guid}")]
    [ProducesResponseType(typeof(ApplicationCommunicationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApplicationCommunicationDto>> GetById(
        Guid applicationId,
        Guid applicationCommunicationId,
        CancellationToken ct)
    {
        var query = new GetApplicationCommunicationByIdQuery(applicationId, applicationCommunicationId);
        var result = await _mediator.Send(query, ct);
        if (result is null)
        {
            return NotFound();
        }
        return Ok(result);
    }
}
