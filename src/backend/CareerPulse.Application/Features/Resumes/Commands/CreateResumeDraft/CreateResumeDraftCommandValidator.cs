using FluentValidation;

namespace CareerPulse.Application.Features.Resumes.Commands.CreateResumeDraft;

public sealed class CreateResumeDraftCommandValidator : AbstractValidator<CreateResumeDraftCommand>
{
    public CreateResumeDraftCommandValidator()
    {
        RuleFor(x => x.Dto)
            .NotNull()
            .WithMessage("Request body cannot be null.");

        RuleFor(x => x.Dto.PersonalInfo)
            .NotNull()
            .WithMessage("PersonalInfo is required.");

        When(x => x.Dto.PersonalInfo != null, () =>
        {
            RuleFor(x => x.Dto.PersonalInfo.FullName)
                .NotEmpty()
                .WithMessage("FullName is required.");

            RuleFor(x => x.Dto.PersonalInfo.Email)
                .NotEmpty()
                .EmailAddress()
                .WithMessage("A valid Email is required.");
        });

        RuleFor(x => x.Dto.ProfessionalSummary)
            .NotEmpty()
            .WithMessage("ProfessionalSummary is required.");

        RuleForEach(x => x.Dto.Skills).ChildRules(skill =>
        {
            skill.RuleFor(s => s.MasterSkillId)
                .NotEmpty()
                .WithMessage("MasterSkillId is required.");

            skill.RuleFor(s => s.ProficiencyLevel)
                .InclusiveBetween(1, 5)
                .WithMessage("ProficiencyLevel must be between 1 and 5.");
        });
    }
}
