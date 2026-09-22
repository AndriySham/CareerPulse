using CareerPulse.Application.DTOs.Languages;
using MediatR;

namespace CareerPulse.Application.Features.Languages.Commands.CreateLanguage;

/// <summary>
/// Command to create a new Language.
/// </summary>
public sealed record CreateLanguageCommand(CreateLanguageDto Dto) : IRequest<LanguageDto>;
