using FluentValidation;

namespace CareerPulse.Application.Features.Interviews.Commands.CreateInterview;

/// <summary>
/// FluentValidation validator for CreateInterviewCommand.
/// </summary>
public sealed class CreateInterviewCommandValidator : AbstractValidator<CreateInterviewCommand>
{
    public CreateInterviewCommandValidator()
    {
        RuleFor(x => x.ApplicationId)
            .NotEmpty()
            .WithMessage("Application ID is required.");

        RuleFor(x => x.Dto)
            .NotNull()
            .WithMessage("Interview body cannot be null.");

        When(x => x.Dto != null, () =>
        {
            RuleFor(x => x.Dto.Type)
                .IsInEnum()
                .WithMessage("Invalid Interview Type.");

            RuleFor(x => x.Dto)
                .Must(x => x.ScheduledAt.HasValue || x.ConductedAt.HasValue)
                .WithMessage("Interview must have either a scheduled or conducted date.");

            RuleFor(x => x.Dto.ScheduledAt)
                .Must(BeUtc)
                .WithMessage("ScheduledAt must be int UTC, e.g. 2026-10-10T10:00:00Z.");

            RuleFor(x => x.Dto.ConductedAt)
                .Must(BeUtc)
                .WithMessage("ConductedAt must be in UTC, e.g. 2026-10-10T10:00:00Z.");
        });
    }

    private static bool BeUtc(DateTime? value)
        => value is null || value.Value.Kind == DateTimeKind.Utc;
}
