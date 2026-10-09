using CareerPulse.Application.DTOs.WorkExperiences;
using CareerPulse.Application.Features.WorkExperience.Commands.CreateWorkExperience;
using FluentValidation.TestHelper;
using Xunit;

namespace CareerPulse.Application.Tests.Features.WorkExperience;

public class CreateWorkExperienceCommandValidatorTests
{
    private readonly CreateWorkExperienceCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenCommandIsValid_ShouldNotHaveAnyValidationErrors()
    {
        var dto = new CreateWorkExperienceDto
        {
            ResumeRevisionId = Guid.NewGuid(),
            CompanyName = "Epam",
            PositionTitle = ".NET Developer",
            StartMonth = 1,
            StartYear = 2024,
            IsCurrentJob = true
        };

        var result = _validator.TestValidate(new CreateWorkExperienceCommand(dto));
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenDtoIsNull_ShouldHaveValidationErrorForDto()
    {
        var result = _validator.TestValidate(new CreateWorkExperienceCommand(null!));
        result.ShouldHaveValidationErrorFor(x => x.Dto)
            .WithErrorMessage("Request body cannot be null.");
    }

    [Fact]
    public void Validate_WhenResumeRevisionIdIsEmpty_ShouldHaveValidationError()
    {
        var dto = new CreateWorkExperienceDto
        {
            ResumeRevisionId = Guid.Empty,
            CompanyName = "Epam",
            PositionTitle = ".NET Developer",
            StartMonth = 1,
            StartYear = 2024
        };

        var result = _validator.TestValidate(new CreateWorkExperienceCommand(dto));
        result.ShouldHaveValidationErrorFor(x => x.Dto.ResumeRevisionId)
            .WithErrorMessage("ResumeRevision ID is required.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WhenCompanyNameIsEmpty_ShouldHaveValidationError(string? name)
    {
        var dto = new CreateWorkExperienceDto
        {
            ResumeRevisionId = Guid.NewGuid(),
            CompanyName = name!,
            PositionTitle = ".NET Developer",
            StartMonth = 1,
            StartYear = 2024
        };

        var result = _validator.TestValidate(new CreateWorkExperienceCommand(dto));
        result.ShouldHaveValidationErrorFor(x => x.Dto.CompanyName)
            .WithErrorMessage("Company Name is required.");
    }

    [Fact]
    public void Validate_WhenCompanyNameExceeds300Chars_ShouldHaveValidationError()
    {
        var dto = new CreateWorkExperienceDto
        {
            ResumeRevisionId = Guid.NewGuid(),
            CompanyName = new string('A', 301),
            PositionTitle = ".NET Developer",
            StartMonth = 1,
            StartYear = 2024
        };

        var result = _validator.TestValidate(new CreateWorkExperienceCommand(dto));
        result.ShouldHaveValidationErrorFor(x => x.Dto.CompanyName)
            .WithErrorMessage("Company Name must not exceed 300 characters.");
    }


    [Fact]
    public void Validate_WhenTechStackExceeds1000Chars_ShouldHaveValidationError()
    {
        var dto = new CreateWorkExperienceDto
        {
            ResumeRevisionId = Guid.NewGuid(),
            CompanyName = "Epam",
            PositionTitle = ".NET Developer",
            StartMonth = 1,
            StartYear = 2024,
            TechStack = new string('C', 1001)
        };

        var result = _validator.TestValidate(new CreateWorkExperienceCommand(dto));
        result.ShouldHaveValidationErrorFor(x => x.Dto.TechStack)
            .WithErrorMessage("Tech Stack must not exceed 1000 characters.");
    }
}
