using FluentValidation;

namespace CareerPulse.Application.Features.MasterSkills.Commands.DeactivateMasterSkill;

/// <summary>
/// FluentValidation validator for DeactivateMasterSkillCommand.
/// </summary>
public sealed class DeactivateMasterSkillCommandValidator 
    : AbstractValidator<DeactivateMasterSkillCommand>
{
    public DeactivateMasterSkillCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("MasterSkill ID is required.");
    }
}
