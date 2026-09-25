using MediatR;

namespace CareerPulse.Application.Features.Languages.Commands.DeleteLanguage;

/// <summary>
/// Command to delete a Language.
/// </summary>
public sealed record DeleteLanguageCommand(Guid Id) : IRequest;
