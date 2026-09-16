using CareerPulse.Application.DTOs.Educations;
using CareerPulse.Application.DTOs.Vacancies;
using CareerPulse.Application.Features.Educations.CreateEducation;
using CareerPulse.Application.Features.Vacancies.Queries.GetVacancyById;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CareerPulse.Api.Controllers
{
    /// <summary>
    /// REST API controller for Education entity management.
    /// </summary>
    [Route("api/educations")]
    [ApiController]
    public class EducationsController : ControllerBase
    {
        private readonly ISender _mediator;

        public EducationsController(ISender mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Gets a single Education by ID.
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(EducationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EducationDto>> GetById(
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

        /// <summary>
        /// Creates a new Education entity.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(EducationDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EducationDto>> Create(
            [FromBody] CreateEducationDto dto,
            CancellationToken ct)
        {
            var command = new CreateEducationCommand(dto);
            var result = await _mediator.Send(command, ct);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
    }
}
