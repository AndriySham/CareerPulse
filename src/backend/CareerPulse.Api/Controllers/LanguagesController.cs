using CareerPulse.Application.DTOs.Languages;
using CareerPulse.Application.Features.Languages.Queries.GetLanguageById;
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
    }
}
