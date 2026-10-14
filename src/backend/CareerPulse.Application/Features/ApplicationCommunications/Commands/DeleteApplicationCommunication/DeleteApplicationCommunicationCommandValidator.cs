using FluentValidation;

namespace CareerPulse.Application.Features.ApplicationCommunications.Commands.DeleteApplicationCommunication;

/// <summary>
/// FluentValidaton validator for DeleteApplicationCommunicationCommand.
/// </summary>
public sealed class DeleteApplicationCommunicationCommandValidator : AbstractValidator<DeleteApplicationCommunicationCommand>
{
    public DeleteApplicationCommunicationCommandValidator()
    {
        RuleFor(x => x.ApplicationCommunicationId)
            .NotEmpty()
            .WithMessage("ApplicationCommunication ID is required.");

        RuleFor(x => x.ApplicationId)
            .NotEmpty()
            .WithMessage("Application ID is required.");
    }
}
