using FluentValidation;

namespace CareerPulse.Application.Features.WorkExperience.Commands.DeleteWorkExperience;

/// <summary>
/// FluentValidation validator for DeleteWorkExperienceCommand.
/// </summary>
public sealed class DeleteWorkExperienceCommandValidator : AbstractValidator<DeleteWorkExperienceCommand>
{
    public DeleteWorkExperienceCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("WorkExperience ID is required.");
    }
}
