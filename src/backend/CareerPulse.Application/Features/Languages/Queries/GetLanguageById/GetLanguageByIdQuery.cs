using CareerPulse.Application.DTOs.Languages;
using MediatR;

namespace CareerPulse.Application.Features.Languages.Queries.GetLanguageById;

/// <summary>
/// Query to retreive a single Language by ID.
/// 
/// </summary>
public record class GetLanguageByIdQuery(Guid Id) : IRequest<LanguageDto?>;
