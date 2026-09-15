using FluentValidation;

namespace CareerPulse.Application.Features.MasterSkills.Commands.UpdateMasterSkill;

/// <summary>
/// FluentValidation validator for UpdateMasterSkillCommand.
/// </summary>

public sealed class UpdateMasterSkillCommandValidator : AbstractValidator<UpdateMasterSkillCommand>
{
    public UpdateMasterSkillCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("MasterSkill ID is required.");

        RuleFor(x => x.Dto)
            .NotNull()
            .WithMessage("Request body can not be null");

        When(x => x.Dto != null, () =>
        {
            RuleFor(x => x.Dto.Name)
                .NotEmpty()
                .WithMessage("MasterSkill name is required.")
                .MaximumLength(200)
                .WithMessage("MasterSkill name must not exceed 200 characters.");

            RuleFor(x => x.Dto.Category)
                .IsInEnum()
                .WithMessage("Invalid SkillCategory.");

            RuleFor(x => x.Dto.Aliases)
                .NotNull()
                .WithMessage("Aliases can not be null.");
        });
    }
}
