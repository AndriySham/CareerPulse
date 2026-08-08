using CareerPulse.Application.DTOs.Companies;
using CareerPulse.Application.Features.Companies.Commands.CreateCompany;
using CareerPulse.Application.Features.Companies.Commands.UpdateCompany;
using CareerPulse.Application.Features.Companies.Queries.GetCompanies;
using CareerPulse.Application.Features.Companies.Queries.GetCompanyById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CareerPulse.Api.Controllers;

/// <summary>
/// REST API controller for Company entity management.
/// </summary>
[ApiController]
[Route("api/companies")]
public class CompaniesController : ControllerBase
{
    private readonly ISender _mediator;

    public CompaniesController(ISender mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new Company entity.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CompanyDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CompanyDto>> Create(
        [FromBody] CreateCompanyDto dto,
        CancellationToken ct)
    {
        var command = new CreateCompanyCommand(dto);
        var result = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Updates an existing Company entity by ID.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(CompanyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CompanyDto>> Update(
        Guid id,
        [FromBody] UpdateCompanyDto dto,
        CancellationToken ct)
    {
        var command = new UpdateCompanyCommand(id, dto);
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }

    /// <summary>
    /// Gets all companies with optional filtering for archived items.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<CompanyDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CompanyDto>>> GetAll(
        [FromQuery] bool includeArchived = false,
        CancellationToken ct = default)
    {
        var query = new GetCompaniesQuery(includeArchived);
        var result = await _mediator.Send(query, ct);
        return Ok(result);
    }

    /// <summary>
    /// Gets a single Company by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CompanyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CompanyDto>> GetById(
        Guid id,
        CancellationToken ct)
    {
        var query = new GetCompanyByIdQuery(id);
        var result = await _mediator.Send(query, ct);
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }
}
