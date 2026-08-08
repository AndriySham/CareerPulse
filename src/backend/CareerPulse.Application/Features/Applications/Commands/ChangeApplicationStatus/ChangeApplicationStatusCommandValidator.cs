using FluentValidation;

namespace CareerPulse.Application.Features.Applications.Commands.ChangeApplicationStatus;

/// <summary>
/// Validator for ChangeApplicationStatusCommand.
/// </summary>
public sealed class ChangeApplicationStatusCommandValidator : AbstractValidator<ChangeApplicationStatusCommand>
{
    public ChangeApplicationStatusCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Application ID is required.");

        RuleFor(x => x.Dto)
            .NotNull().WithMessage("ChangeApplicationStatusDto is required.");

        When(x => x.Dto != null, () =>
        {
            RuleFor(x => x.Dto.NewStatus)
                .IsInEnum().WithMessage("Valid ApplicationStatus is required.");

            RuleFor(x => x.Dto.Notes)
                .MaximumLength(4000).WithMessage("Notes cannot exceed 4000 characters.");
        });
    }
}
