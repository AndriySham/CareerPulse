using FluentValidation;

namespace CareerPulse.Application.Features.MasterSkills.Queries.ResolveSkills;

/// <summary>
/// FluentValidation validator for ResolveSkillsQuery.
/// </summary>
public sealed class ResolveSkillsQueryValidator : AbstractValidator<ResolveSkillsQuery>
{
    public ResolveSkillsQueryValidator()
    {
        RuleFor(x => x.RawSkills)
            .NotNull().WithMessage("RawSkills collection cannot be null.");
    }
}
