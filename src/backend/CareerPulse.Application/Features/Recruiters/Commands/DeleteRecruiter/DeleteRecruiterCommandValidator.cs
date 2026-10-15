using FluentValidation;

namespace CareerPulse.Application.Features.Recruiters.Commands.DeleteRecruiter;

/// <summary>
/// FluentValidation validator for DeleteRecruiterCommand.
/// </summary>
public sealed class DeleteRecruiterCommandValidator : AbstractValidator<DeleteRecruiterCommand>
{
    public DeleteRecruiterCommandValidator()
    {
        RuleFor(x => x.CompanyId)
            .NotEmpty()
            .WithMessage("Company ID is required.");

        RuleFor(x => x.RecruiterId)
            .NotEmpty()
            .WithMessage("Recruiter ID is required.");
    }
}
