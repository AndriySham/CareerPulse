using FluentValidation;

namespace CareerPulse.Application.Features.Educations.Commands.DeleteEducation;

/// <summary>
/// FluentValidation validator for DeleteEducationCommand.
/// </summary>
public sealed class DeleteEducationCommandValidator : AbstractValidator<DeleteEducationCommand>
{
    public DeleteEducationCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Education ID is required.");
    }
}
