using CareerPulse.Application.DTOs.Projects;
using CareerPulse.Application.Features.Projects.Commands.CreateProject;
using FluentValidation.TestHelper;
using Xunit;

namespace CareerPulse.Application.Tests.Features.Projects;

public class CreateProjectCommandValidatorTests
{
    private readonly CreateProjectCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenCommandIsValid_ShouldNotHaveAnyValidationErrors()
    {
        var dto = new CreateProjectDto
        {
            ResumeRevisionId = Guid.NewGuid(),
            ProjectName = "CareerPulse CRM",
            Role = "Lead Developer",
            TechStack = ".NET 9, React"
        };

        var result = _validator.TestValidate(new CreateProjectCommand(dto));
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenDtoIsNull_ShouldHaveValidationErrorForDto()
    {
        var result = _validator.TestValidate(new CreateProjectCommand(null!));
        result.ShouldHaveValidationErrorFor(x => x.Dto)
            .WithErrorMessage("Project body cannot be null.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WhenProjectNameIsEmpty_ShouldHaveValidationError(string? name)
    {
        var dto = new CreateProjectDto
        {
            ResumeRevisionId = Guid.NewGuid(),
            ProjectName = name!
        };

        var result = _validator.TestValidate(new CreateProjectCommand(dto));
        result.ShouldHaveValidationErrorFor(x => x.Dto.ProjectName)
            .WithErrorMessage("Project Name is required.");
    }
}
