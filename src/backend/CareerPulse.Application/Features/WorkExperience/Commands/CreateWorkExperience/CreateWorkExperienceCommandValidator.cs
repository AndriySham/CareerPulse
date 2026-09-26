using FluentValidation;
using System.Threading;

namespace CareerPulse.Application.Features.WorkExperience.Commands.CreateWorkExperience;

/// <summary>
/// FluentValidation validator for CreateWorkExperience command.
/// </summary>
public sealed class CreateWorkExperienceCommandValidator : AbstractValidator<CreateWorkExperienceCommand>
{
    public CreateWorkExperienceCommandValidator()
    {
        RuleFor(x => x.Dto)
            .NotEmpty()
            .WithMessage("Request body cannot be null.");

        When(x => x.Dto != null, () =>
        {
            RuleFor(x => x.Dto.ResumeRevisionId)
                .NotEmpty()
                .WithMessage("ResumeRevision ID is required.");

            RuleFor(x => x.Dto.CompanyName)
                .NotEmpty()
                .WithMessage("Company Name is required.")
                .MaximumLength(300)
                .WithMessage("Company Name must not exceed 300 characters.");

            RuleFor(x => x.Dto.PositionTitle)
                .NotEmpty()
                .WithMessage("Position Title is required.")
                .MaximumLength(300)
                .WithMessage("Position Title must not exceed 300 characters.");

            RuleFor(x => x.Dto.StartYear)
                .GreaterThan(0)
                .WithMessage("Start Year must be greater than 0.")
                .LessThanOrEqualTo(DateTime.Today.Year)
                .WithMessage("Start Year must not exceed current year.");

            RuleFor(x => x.Dto.StartMonth)
                .InclusiveBetween(1, 12)
                .WithMessage("Start Month must be between 1 and 12.")
                .LessThanOrEqualTo(DateTime.Today.Month)
                .When(x => x.Dto.StartYear == DateTime.Today.Year)
                .WithMessage("Start Month must not exceed current month.");

            RuleFor(x => x.Dto.EndYear)
                .GreaterThan(0)
                .When(x => x.Dto.EndYear.HasValue)
                .WithMessage("End Year must be greater than 0.");

            RuleFor(x => x.Dto.EndMonth)
                .InclusiveBetween(1, 12)
                .When(x => x.Dto.EndMonth.HasValue)
                .WithMessage("End Month must be between 1 and 12.");

            RuleFor(x => x.Dto.TechStack)
                .MaximumLength(1000)
                .WithMessage("Tech Stack must not exceed 1000 characters.");
        });
    }
}
