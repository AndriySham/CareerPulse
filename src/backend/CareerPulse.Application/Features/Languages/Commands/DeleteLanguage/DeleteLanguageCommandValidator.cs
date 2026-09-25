using FluentValidation;

namespace CareerPulse.Application.Features.Languages.Commands.DeleteLanguage;

/// <summary>
/// FluentValidation validator for DeleteLanguageCommand.
/// </summary>
public sealed class DeleteLanguageCommandValidator : AbstractValidator<DeleteLanguageCommand>
{
    public DeleteLanguageCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Language ID is required.");
    }
}
