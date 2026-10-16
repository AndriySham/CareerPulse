using FluentValidation;

namespace CareerPulse.Application.Features.Recruiters.Commands.UpdateRecruiter;

/// <summary>
/// FluentValidation validator for UpdateRecruiterCommand.
/// </summary>
public sealed class UpdateRecruiterCommandValidator : AbstractValidator<UpdateRecruiterCommand>
{
    public UpdateRecruiterCommandValidator()
    {
        RuleFor(x => x.CompanyId)
            .NotEmpty()
            .WithMessage("Company ID is required.");

        RuleFor(x => x.RecruiterId)
            .NotEmpty()
            .WithMessage("Recruiter ID is required.");

        RuleFor(x => x.Dto)
            .NotNull()
            .WithMessage("Recruiter body cannot be null.");

        When(x => x.Dto != null, () =>
        {
            RuleFor(x => x.Dto.Name)
                .NotEmpty()
                .WithMessage("Recruiter Name is required.")
                .MaximumLength(100)
                .WithMessage("Recruiter Name must not exceed 100 characters.");

            RuleFor(x => x.Dto.Phone)
                .MaximumLength(20)
                .WithMessage("Phone must not exceed 20 characters.");

            RuleFor(x => x.Dto.Email)
                .MaximumLength(100)
                .WithMessage("Email must not exceed 100 characters.")
                .EmailAddress()
                .WithMessage("Email is not a valid email address.")
                .When(x => !string.IsNullOrWhiteSpace(x.Dto.Email));

            RuleFor(x => x.Dto.LinkedInUrl)
                .MaximumLength(500)
                .WithMessage("LinkedIn Url must not exceed 500 characters.")
                .Must(BeHttpUrl)
                .WithMessage("LinkedIn URL must be a valid http(s) URL.")
                .When(x => !string.IsNullOrWhiteSpace(x.Dto.LinkedInUrl));

            RuleFor(x => x.Dto.TelegramUrl)
                .MaximumLength(500)
                .WithMessage("Telegram Url must not exceed 500 characters.")
                .Must(BeHttpUrl)
                .WithMessage("Telegram URL must be a valid http(s) URL.")
                .When(x => !string.IsNullOrWhiteSpace(x.Dto.TelegramUrl));

            RuleFor(x => x.Dto.Notes)
                .MaximumLength(4000)
                .WithMessage("Notes must not exceed 4000 characters.");
        });
    }

    private static bool BeHttpUrl(string? value)
        => Uri.TryCreate(value?.Trim(), UriKind.Absolute, out var uri)
           && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
}
