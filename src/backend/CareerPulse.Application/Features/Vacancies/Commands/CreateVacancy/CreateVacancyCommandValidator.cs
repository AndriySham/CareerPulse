using FluentValidation;

namespace CareerPulse.Application.Features.Vacancies.Commands.CreateVacancy;

/// <summary>
/// FluentValidation validator for CreateVacancyCommand.
/// </summary>
public sealed class CreateVacancyCommandValidator : AbstractValidator<CreateVacancyCommand>
{
    public CreateVacancyCommandValidator()
    {
        RuleFor(x => x.Dto)
            .NotNull()
            .WithMessage("Request body cannot be null.");

        When(x => x.Dto != null, () =>
        {
            RuleFor(x => x.Dto.CompanyId)
                .NotEmpty()
                .WithMessage("Company ID is required.");

            RuleFor(x => x.Dto.Title)
                .NotEmpty()
                .WithMessage("Vacancy title is required.")
                .MaximumLength(300)
                .WithMessage("Vacancy title must not exceed 300 characters.");

            RuleFor(x => x.Dto.Url)
                .MaximumLength(1000)
                .WithMessage("Url must not exceed 1000 characters.");

            RuleFor(x => x.Dto.Location)
                .MaximumLength(300)
                .WithMessage("Location must not exceed 300 characters.");

            RuleFor(x => x.Dto.WorkMode)
                .IsInEnum()
                .WithMessage("Invalid WorkMode.");

            RuleFor(x => x.Dto.SalaryMin)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Minimum salary must not be negative.");

            RuleFor(x => x.Dto)
            .Must(x => !x.SalaryMin.HasValue ||
                        !x.SalaryMax.HasValue ||
                        x.SalaryMin <= x.SalaryMax)
            .WithMessage("Minimum salary must not exceed maximum salary.");

            RuleFor(x => x.Dto.SalaryMax)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Maximum salary must not be negative.")
                .LessThanOrEqualTo(1_000_000)
                .WithMessage("Max salary must not exceed 1 000 000");

            RuleFor(x => x.Dto.SalaryCurrency)
                .IsInEnum()
                .WithMessage("Invalid SalaryCurrency.");

            RuleFor(x => x.Dto.EmploymentType)
                .IsInEnum()
                .WithMessage("Invalid EmploymentType");

            RuleFor(x => x.Dto.LanguageRequirements)
                .NotNull()
                .WithMessage("Language requirements cannot be null.");

            RuleForEach(x => x.Dto.LanguageRequirements)
                .NotNull()
                .WithMessage("Language requirement item cannot be null.")
                .ChildRules(language =>
                {
                    language.RuleFor(x => x.LanguageName)
                        .NotEmpty()
                        .WithMessage("Language name is required.")
                        .MaximumLength(100)
                        .WithMessage("Language name must not exceed 100 characters.");

                    language.RuleFor(x => x.Proficiency)
                        .IsInEnum()
                        .WithMessage("Invalid language proficiency.");

                    language.RuleFor(x => x.ProficiencyDescription)
                       .MaximumLength(100)
                       .WithMessage("Proficiency description must not exceed 100 characters.");
                });
        });
    }
}
