using FluentValidation;

namespace CareerPulse.Application.Features.Projects.Commands.UpdateProject;

/// <summary>
/// FluentValidation for UpdateProjectCommand.
/// </summary>
public sealed class UpdateProjectCommandValidator : AbstractValidator<UpdateProjectCommand>
{
    public UpdateProjectCommandValidator()
    {
        RuleFor(x => x.Dto)
            .NotNull()
            .WithMessage("Project body cannot be null.");

        When(x => x.Dto != null, () =>
        {
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
                .WithMessage("RepositoryUrl must not exceed 1000 characters.");

            RuleFor(x => x.Dto.LiveDemoUrl)
                .MaximumLength(1000)
                .WithMessage("LiveDemoUrl must not exceed 1000 characters.");
        });
    }
}
