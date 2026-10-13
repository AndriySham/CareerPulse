using FluentValidation;

namespace CareerPulse.Application.Features.ApplicationCommunications.Commands.UpdateApplicationCommunication;

/// <summary>
/// FluentValidation falidator for UpdateApplicationCommunicationCommand.
/// </summary>
public sealed class UpdateApplicationCommunicationCommandValidator : AbstractValidator<UpdateApplicationCommunicationCommand>
{
    public UpdateApplicationCommunicationCommandValidator()
    {
        RuleFor(x => x.ApplicationId)
            .NotEmpty()
            .WithMessage("Application ID is required.");

        RuleFor(x => x.ApplicationCommunicationId)
            .NotEmpty()
            .WithMessage("ApplicationCommunication ID is required.");

        RuleFor(x => x.Dto)
            .NotNull()
            .WithMessage("ApplicationCommunication body cannot be null.");

        When(x => x.Dto != null, () =>
        {
            RuleFor(x => x.Dto.OccurredAt)
                .Must(time => time.Kind == DateTimeKind.Utc)
                .WithMessage("OccurredAt must be in UTC, e.g. 2026-10-10T10:00:00Z.");

            RuleFor(x => x.Dto.Notes)
                .NotEmpty()
                .WithMessage("Notes are required.")
                .MaximumLength(4000)
                .WithMessage("Notes must not exceed 4000 characters.");
        });
    }
}
