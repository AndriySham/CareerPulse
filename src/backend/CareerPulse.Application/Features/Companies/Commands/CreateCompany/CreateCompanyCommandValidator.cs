using FluentValidation;

namespace CareerPulse.Application.Features.Companies.Commands.CreateCompany;

/// <summary>
/// FluentValidation validator for CreateCompanyCommand.
/// </summary>
public sealed class CreateCompanyCommandValidator : AbstractValidator<CreateCompanyCommand>
{
    public CreateCompanyCommandValidator()
    {
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
        });
    }
}
