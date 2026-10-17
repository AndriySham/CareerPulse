using FluentValidation;

namespace CareerPulse.Application.Features.Applications.Commands.AssignRecruiter;

/// <summary>
/// Validator for AssignRecruiterCommand.
/// </summary>
public sealed class AssignRecruiterCommandValidator : AbstractValidator<AssignRecruiterCommand>
{
    public AssignRecruiterCommandValidator()
    {
        RuleFor(x => x.ApplicationId)
            .NotEmpty()
            .WithMessage("Application ID is required.");
    }
}
