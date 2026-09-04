using FluentValidation;

namespace CareerPulse.Application.Features.Companies.Commands.RestoreCompany;

/// <summary>
/// FluentValidation validator for RestoreCompanyCommand.
/// </summary>
public sealed class RestoreCompanyCommandValidator : AbstractValidator<RestoreCompanyCommand>
{
    public RestoreCompanyCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Company ID is required");
    }
}
