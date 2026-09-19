using FluentValidation;

namespace CareerPulse.Application.Features.Projects.Commands.DeleteProject;

/// <summary>
/// FluentValidation validator for DeleteProjectCommand.
/// </summary>
public sealed class DeleteProjectCommandValidator : AbstractValidator<DeleteProjectCommand>
{
    public DeleteProjectCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Project ID is required.");
    }
}
