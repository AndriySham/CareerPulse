using CareerPulse.Application.DTOs.WorkExperiences;
using CareerPulse.Application.Features.WorkExperience.Commands.UpdateWorkExperience;
using FluentValidation.TestHelper;
using Xunit;

namespace CareerPulse.Application.Tests.Features.WorkExperience;

public class UpdateWorkExperienceCommandValidatorTests
{
    private readonly UpdateWorkExperienceCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenCommandIsValid_ShouldNotHaveAnyValidationErrors()
    {
        var dto = new UpdateWorkExperienceDto
        {
            CompanyName = "Epam",
            PositionTitle = "Senior .NET Developer",
            StartMonth = 1,
            StartYear = 2024,
            IsCurrentJob = true
        };

        var result = _validator.TestValidate(new UpdateWorkExperienceCommand(Guid.NewGuid(), dto));
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenIdIsEmpty_ShouldHaveValidationError()
    {
        var dto = new UpdateWorkExperienceDto
        {
            CompanyName = "Epam",
            PositionTitle = "Senior .NET Developer",
            StartMonth = 1,
            StartYear = 2024
        };

        var result = _validator.TestValidate(new UpdateWorkExperienceCommand(Guid.Empty, dto));
        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorMessage("WorkExperience ID is required.");
    }

    [Fact]
    public void Validate_WhenDtoIsNull_ShouldHaveValidationErrorForDto()
    {
        var result = _validator.TestValidate(new UpdateWorkExperienceCommand(Guid.NewGuid(), null!));
        result.ShouldHaveValidationErrorFor(x => x.Dto)
            .WithErrorMessage("Request body cannot be null.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WhenCompanyNameIsEmpty_ShouldHaveValidationError(string? name)
    {
        var dto = new UpdateWorkExperienceDto
        {
            CompanyName = name!,
            PositionTitle = "Senior .NET Developer",
            StartMonth = 1,
            StartYear = 2024
        };

        var result = _validator.TestValidate(new UpdateWorkExperienceCommand(Guid.NewGuid(), dto));
        result.ShouldHaveValidationErrorFor(x => x.Dto.CompanyName)
            .WithErrorMessage("Company Name is required.");
    }
}
