using CareerPulse.Application.DTOs.Languages;
using MediatR;

namespace CareerPulse.Application.Features.Languages.Commands.UpdateLanguage;

/// <summary>
/// Command to update a Language.
/// </summary>
public sealed record UpdateLanguageCommand(Guid Id, UpdateLanguageDto Dto) 
    : IRequest<LanguageDto>;
