using CareerPulse.Application.DTOs.Projects;
using CareerPulse.Application.Features.Educations.Queries.GetEducationById;
using CareerPulse.Application.Features.Projects.Commands.CreateProject;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CareerPulse.Api.Controllers;

/// <summary>
/// REST API controller for Project entity management.
/// </summary>
[Route("api/[controller]")] ///////////////////////////////////////////////////
[ApiController]
public class ProjectController : ControllerBase
{
    private readonly ISender _mediator;

    public ProjectController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectDto>> GetById(
        Guid id,
        CancellationToken ct)
    {
        var query = new GetEducationByIdQuery(id);
        var result = await _mediator.Send(query, ct);
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }

    /// <summary>
    /// Creates a new Project entity.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]//////////
    public async Task<ActionResult<ProjectDto>> Create(
        CreateProjectDto dto,
        CancellationToken ct)
    {
        var command = new CreateProjectCommand(dto);
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
}
