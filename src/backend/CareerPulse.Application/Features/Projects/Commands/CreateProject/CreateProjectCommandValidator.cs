using FluentValidation;

namespace CareerPulse.Application.Features.Projects.Commands.CreateProject;

/// <summary>
/// FluentValidation validator for CreateProjectCommand.
/// </summary>
public sealed class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(x => x.Dto)
            .NotNull()
            .WithMessage("Project body cannot be null.");

        When(x => x.Dto != null, () =>
        {
            RuleFor(x => x.Dto.ResumeRevisionId)
                .NotEmpty()
                .WithMessage("ResumeResivion ID is requered.");

            RuleFor(x => x.Dto.Role)
                .MaximumLength(200)
                .WithMessage("Role must not exceed 200 characters.");

            RuleFor(x => x.Dto.ProjectName)
                .NotEmpty()
                .WithMessage("Project Name is required.")
                .MaximumLength(300)
                .WithMessage("Project Name must not exceed 300 characters.");

            RuleFor(x => x.Dto.TechStack)
                .MaximumLength(1000)
                .WithMessage("TechStack must not exceed 1000 characters");

            RuleFor(x => x.Dto.RepositoryUrl)
                .MaximumLength(1000)
                .WithMessage("RepositotyUrl must not exceed 1000 characters.");

            RuleFor(x => x.Dto.LiveDemoUrl)
                .MaximumLength(1000)
                .WithMessage("LiveDemoUrl must not exceed 1000 characters.");
        });
    }
}
