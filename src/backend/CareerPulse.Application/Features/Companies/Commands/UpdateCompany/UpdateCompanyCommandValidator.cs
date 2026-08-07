using FluentValidation;

namespace CareerPulse.Application.Features.Companies.Commands.UpdateCompany;

/// <summary>
/// FluentValidation validator for UpdateCompanyCommand.
/// </summary>
public sealed class UpdateCompanyCommandValidator : AbstractValidator<UpdateCompanyCommand>
{
    public UpdateCompanyCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Company ID is required.");

        RuleFor(x => x.Dto)
            .NotNull()
            .WithMessage("Request body cannot be null.");

        When(x => x.Dto != null, () =>
        {
            RuleFor(x => x.Dto.Name)
                .NotEmpty()
                .WithMessage("Company name is required.")
                .MaximumLength(300)
                .WithMessage("Company name must not exceed 300 characters.");

            RuleFor(x => x.Dto.Website)
                .MaximumLength(500)
                .WithMessage("Website URL must not exceed 500 characters.");

            RuleFor(x => x.Dto.Industry)
                .MaximumLength(150)
                .WithMessage("Industry must not exceed 150 characters.");

            RuleFor(x => x.Dto.Notes)
                .MaximumLength(4000)
                .WithMessage("Notes must not exceed 4000 characters.");
        });
    }
}
