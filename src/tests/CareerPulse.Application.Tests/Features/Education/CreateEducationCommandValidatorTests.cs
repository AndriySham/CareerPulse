using CareerPulse.Application.DTOs.Educations;
using CareerPulse.Application.Features.Educations.Commands.CreateEducation;
using FluentValidation.TestHelper;
using Xunit;

namespace CareerPulse.Application.Tests.Features.Education;

public class CreateEducationCommandValidatorTests
{
    private readonly CreateEducationCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenCommandIsValid_ShouldNotHaveAnyValidationErrors()
    {
        var dto = new CreateEducationDto
        {
            ResumeRevisionId = Guid.NewGuid(),
            InstitutionName = "KPI",
            StartYear = 2018,
            EndYear = 2022
        };

        var result = _validator.TestValidate(new CreateEducationCommand(dto));
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenDtoIsNull_ShouldHaveValidationErrorForDto()
    {
        var result = _validator.TestValidate(new CreateEducationCommand(null!));
        result.ShouldHaveValidationErrorFor(x => x.Dto)
            .WithErrorMessage("Request body cannot be null.");
    }

    [Fact]
    public void Validate_WhenResumeRevisionIdIsEmpty_ShouldHaveValidationError()
    {
        var dto = new CreateEducationDto
        {
            ResumeRevisionId = Guid.Empty,
            InstitutionName = "KPI"
        };

        var result = _validator.TestValidate(new CreateEducationCommand(dto));
        result.ShouldHaveValidationErrorFor(x => x.Dto.ResumeRevisionId)
            .WithErrorMessage("ResumeRevision ID is required.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WhenInstitutionNameIsEmpty_ShouldHaveValidationError(string? name)
    {
        var dto = new CreateEducationDto
        {
            ResumeRevisionId = Guid.NewGuid(),
            InstitutionName = name!
        };

        var result = _validator.TestValidate(new CreateEducationCommand(dto));
        result.ShouldHaveValidationErrorFor(x => x.Dto.InstitutionName)
            .WithErrorMessage("Education InstitutionName is required.");
    }
}
