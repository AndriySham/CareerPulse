using FluentValidation;

namespace CareerPulse.Application.Features.Languages.Commands.UpdateLanguage;

/// <summary>
/// FluentValidation validator for UpdateLanguageCommand.
/// </summary>
public sealed class UpdateLanguageCommandValidator : AbstractValidator<UpdateLanguageCommand>
{
    public UpdateLanguageCommandValidator()
    {
        RuleFor(x => x.Dto)
            .NotEmpty()
            .WithMessage("Request body cannot be null.");

        When(x => x.Dto != null, () =>
        {
            RuleFor(x => x.Dto.LanguageName)
                .NotEmpty()
                .WithMessage("Language Name is required.")
                .MaximumLength(100)
                .WithMessage("Language Name must not exceed 100 characters.");

            RuleFor(x => x.Dto.Proficiency)
                .IsInEnum()
                .WithMessage("Valid LanguageProficiency is required.");
        });
    }
}
