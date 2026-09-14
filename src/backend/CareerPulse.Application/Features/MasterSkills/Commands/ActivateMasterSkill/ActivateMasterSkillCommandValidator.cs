using FluentValidation;

namespace CareerPulse.Application.Features.MasterSkills.Commands.ActivateMasterSkill;

/// <summary>
/// FluentValidation validator for ActivateMasterSkillCommand.
/// </summary>
public sealed class ActivateMasterSkillCommandValidator : AbstractValidator<ActivateMasterSkillCommand>
{
    public ActivateMasterSkillCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("MasterSkill ID is required.");
    }
}
