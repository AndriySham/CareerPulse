using CareerPulse.Application.DTOs.Languages;
using MediatR;

namespace CareerPulse.Application.Features.Languages.Queries.GetLanguages;

/// <summary>
/// Query to retrieve languages for a specific resume revision.
/// </summary>
public sealed record GetLanguagesQuery(Guid ResumeRevisionId) : IRequest<List<LanguageDto>>;
