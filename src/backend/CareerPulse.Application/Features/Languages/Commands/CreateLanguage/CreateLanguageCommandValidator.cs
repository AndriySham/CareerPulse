using FluentValidation;

namespace CareerPulse.Application.Features.Languages.Commands.CreateLanguage;

/// <summary>
/// FluentValidation validator for CreateLanguageCommand.
/// </summary>
public sealed class CreateLanguageCommandValidator : AbstractValidator<CreateLanguageCommand>
{
    public CreateLanguageCommandValidator()
    {
        RuleFor(x => x.Dto)
            .NotEmpty()
            .WithMessage("Request body cannot be null.");

        When(x => x.Dto != null, () =>
        {
            RuleFor(x => x.Dto.ResumeRevisionId)
                .NotEmpty()
                .WithMessage("ResumeRevision ID is required.");

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
