using FluentValidation;

namespace CareerPulse.Application.Features.Resumes.Commands.SpawnResumeVersion;

public sealed class SpawnResumeVersionCommandValidator : AbstractValidator<SpawnResumeVersionCommand>
{
    public SpawnResumeVersionCommandValidator()
    {
        RuleFor(x => x.ParentRevisionId)
            .NotEmpty()
            .WithMessage("ParentRevisionId is required.");
    }
}
