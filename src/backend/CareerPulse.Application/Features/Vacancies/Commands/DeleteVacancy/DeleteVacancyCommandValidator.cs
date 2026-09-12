using FluentValidation;

namespace CareerPulse.Application.Features.Vacancies.Commands.DeleteVacancy;

/// <summary>
/// FluentValidation validator for DeleteVacancyCommand.
/// </summary>

public class DeleteVacancyCommandValidator : AbstractValidator<DeleteVacancyCommand>
{
    public DeleteVacancyCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Vacancy ID is required");
    }
}
