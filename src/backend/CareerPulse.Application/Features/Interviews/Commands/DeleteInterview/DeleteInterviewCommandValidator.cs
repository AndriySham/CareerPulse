using FluentValidation;

namespace CareerPulse.Application.Features.Interviews.Commands.DeleteInterview;

/// <summary>
/// FluentValidation validator for DeleteInterviewCommand.
/// </summary>
public sealed class DeleteInterviewCommandValidator : AbstractValidator<DeleteInterviewCommand>
{
    public DeleteInterviewCommandValidator()
    {
        RuleFor(x => x.ApplicationId)
            .NotEmpty()
            .WithMessage("Application ID is required.");

        RuleFor(x => x.InterviewId)
            .NotEmpty()
            .WithMessage("Interview ID is required.");
    }
}
