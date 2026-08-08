using FluentValidation;

namespace CareerPulse.Application.Features.Vacancies.Commands.UpdateVacancy;

/// <summary>
/// FluentValidation validator for UpdateVacancyCommand.
/// </summary>
public sealed class UpdateVacancyCommandValidator : AbstractValidator<UpdateVacancyCommand>
{
    public UpdateVacancyCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Vacancy ID is required.");

        RuleFor(x => x.Dto)
            .NotNull()
            .WithMessage("Request body cannot be null.");

        When(x => x.Dto != null, () =>
        {
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
