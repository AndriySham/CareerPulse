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
        });
    }
}
