using FluentValidation;

namespace CareerPulse.Application.Features.MasterSkills.Commands.CreateMasterSkill;

/// <summary>
/// FluentValidation validator for CreateMasterSkillCommand.
/// </summary>
public sealed class CreateMasterSkillCommandValidator : AbstractValidator<CreateMasterSkillCommand>
{
    public CreateMasterSkillCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("MasterSkill name is required.")
            .MaximumLength(200).WithMessage("MasterSkill name must not exceed 200 characters.");

        RuleFor(x => x.Category)
            .IsInEnum().WithMessage("Valid SkillCategory is required.");

        RuleForEach(x => x.Aliases)
            .NotEmpty().WithMessage("Alias name cannot be empty.")
            .MaximumLength(200).WithMessage("Alias name must not exceed 200 characters.");
    }
}
