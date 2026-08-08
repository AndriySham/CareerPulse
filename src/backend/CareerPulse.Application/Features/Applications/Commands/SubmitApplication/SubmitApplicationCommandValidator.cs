using FluentValidation;

namespace CareerPulse.Application.Features.Applications.Commands.SubmitApplication;

/// <summary>
/// Validator for SubmitApplicationCommand.
/// </summary>
public sealed class SubmitApplicationCommandValidator : AbstractValidator<SubmitApplicationCommand>
{
    public SubmitApplicationCommandValidator()
    {
        RuleFor(x => x.Dto)
            .NotNull().WithMessage("SubmitApplicationDto is required.");

        RuleFor(x => x.Dto.CompanyId)
            .NotEmpty().WithMessage("CompanyId is required.");

        RuleFor(x => x.Dto.ResumeRevisionId)
            .NotEmpty().WithMessage("ResumeRevisionId is required.");

        RuleFor(x => x.Dto.JobSource)
            .NotEmpty().WithMessage("JobSource is required.")
            .MaximumLength(200).WithMessage("JobSource cannot exceed 200 characters.");

        RuleFor(x => x.Dto.Notes)
            .MaximumLength(4000).WithMessage("Notes cannot exceed 4000 characters.");
    }
}
