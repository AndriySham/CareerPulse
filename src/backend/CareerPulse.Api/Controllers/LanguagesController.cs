using CareerPulse.Application.DTOs.Languages;
using CareerPulse.Application.Features.Languages.Commands.CreateLanguage;
using CareerPulse.Application.Features.Languages.Queries.GetLanguageById;
using CareerPulse.Application.Features.Languages.Queries.GetLanguages;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CareerPulse.Api.Controllers
{
    /// <summary>
    ///  REST API controller for Language entity management.
    /// </summary>
    [Route("api/language")]
    [ApiController]
    public class LanguagesController : ControllerBase
    {
        public readonly ISender _mediator;

        public LanguagesController(ISender mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get all languages for a specific resume revision.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<LanguageDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<LanguageDto>>> GetAll(
            [FromQuery] Guid resumeRevisionId,
            CancellationToken ct)
        {
            var query = new GetLanguagesQuery(resumeRevisionId);
            var result = await _mediator.Send(query, ct);
            return Ok(result);
        }

        /// <summary>
        /// Get Language entity by ID.
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LanguageDto>> GetById(
            Guid id,
            CancellationToken ct)
        {
            var query = new GetLanguageByIdQuery(id);
            var result = await _mediator.Send(query, ct);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        /// <summary>
        /// Creates a new Language entity.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(LanguageDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LanguageDto>> Create(
            [FromBody] CreateLanguageDto dto,
            CancellationToken ct)
        {
            var command = new CreateLanguageCommand(dto);
            var result = await _mediator.Send(command, ct);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
    }
}
