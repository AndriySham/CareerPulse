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
        });
    }
}
