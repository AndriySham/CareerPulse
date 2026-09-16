using FluentValidation;

namespace CareerPulse.Application.Features.Educations.Commands.CreateEducation;

/// <summary>
/// FluentValidation validator for CreateEducationCommand.
/// </summary>
public sealed class CreateEducationCommandValidator : AbstractValidator<CreateEducationCommand>
{
    public CreateEducationCommandValidator()
    {
        RuleFor(x => x.Dto)
            .NotEmpty()
            .WithMessage("Request body cannot be null.");

        When(x => x.Dto != null, () =>
        {
            RuleFor(x => x.Dto.ResumeRevisionId)
                .NotEmpty()
                .WithMessage("ResumeRevision ID is required.");

            RuleFor(x => x.Dto.InstitutionName)
                .NotEmpty()
                .WithMessage("Education InstitutionName is required.")
                .MaximumLength(300)
                .WithMessage("Education InstitutionName must not exceed 300 characters.");

            RuleFor(x => x.Dto.Description)
                .MaximumLength(200)
                .WithMessage("Education Descriprion must not exceed 200 characters.");

            RuleFor(x => x.Dto.StartYear)
                .LessThanOrEqualTo(DateTime.Now.Year)
                .WithMessage("The year can not be longer than the current one.");

            RuleFor(x => x.Dto.EndYear)
                .LessThanOrEqualTo(DateTime.Now.Year + 10)
                .WithMessage($"The year can not be longer than {DateTime.Now.Year + 10}.");

            RuleFor(x => x)
                .Must(x => !x.Dto.StartYear.HasValue || !x.Dto.EndYear.HasValue
                    || x.Dto.EndYear >= x.Dto.StartYear)
                .WithMessage("The EndYear can not be less than StartYear.");
        });
    }
}
