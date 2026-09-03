using FluentValidation;

namespace CareerPulse.Application.Features.Companies.Commands.ArchiveCompany;

/// <summary>
/// FluentValidation validator for ArchiveCompanyCommand.
/// </summary>
public sealed class ArchiveCompanyCommandValidator : AbstractValidator<ArchiveCompanyCommand>
{
    public ArchiveCompanyCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Company ID is required");
    }
}
