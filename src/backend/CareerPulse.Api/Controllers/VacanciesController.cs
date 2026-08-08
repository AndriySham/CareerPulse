using CareerPulse.Application.DTOs.Vacancies;
using CareerPulse.Application.Features.Vacancies.Commands.CreateVacancy;
using CareerPulse.Application.Features.Vacancies.Commands.UpdateVacancy;
using CareerPulse.Application.Features.Vacancies.Queries.GetVacancies;
using CareerPulse.Application.Features.Vacancies.Queries.GetVacancyById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CareerPulse.Api.Controllers;

/// <summary>
/// REST API controller for Vacancy entity management.
/// </summary>
[ApiController]
[Route("api/vacancies")]
public class VacanciesController : ControllerBase
{
    private readonly ISender _mediator;

    public VacanciesController(ISender mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new Vacancy entity.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(VacancyDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<VacancyDto>> Create(
        [FromBody] CreateVacancyDto dto,
        CancellationToken ct)
    {
        var command = new CreateVacancyCommand(dto);
        var result = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Updates an existing Vacancy entity by ID.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(VacancyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<VacancyDto>> Update(
        Guid id,
        [FromBody] UpdateVacancyDto dto,
        CancellationToken ct)
    {
        var command = new UpdateVacancyCommand(id, dto);
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }

    /// <summary>
    /// Gets all vacancies with optional filtering by company ID.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<VacancyDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<VacancyDto>>> GetAll(
        [FromQuery] Guid? companyId,
        CancellationToken ct = default)
    {
        var query = new GetVacanciesQuery(companyId);
        var result = await _mediator.Send(query, ct);
        return Ok(result);
    }

    /// <summary>
    /// Gets a single Vacancy by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(VacancyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VacancyDto>> GetById(
        Guid id,
        CancellationToken ct)
    {
        var query = new GetVacancyByIdQuery(id);
        var result = await _mediator.Send(query, ct);
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }
}
