using CareerPulse.Application.DTOs.Applications;
using CareerPulse.Application.Features.Applications.Commands.SubmitApplication;
using FluentValidation.TestHelper;
using Xunit;

namespace CareerPulse.Application.Tests.Features.Applications.Validators;

public class SubmitApplicationCommandValidatorTests
{
    private readonly SubmitApplicationCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenCommandIsValid_ShouldNotHaveAnyValidationErrors()
    {
        // Arrange
        var dto = new SubmitApplicationDto
        {
            CompanyId = Guid.NewGuid(),
            ResumeRevisionId = Guid.NewGuid(),
            JobSource = "LinkedIn",
            Notes = "Valid notes"
        };
        var command = new SubmitApplicationCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenDtoIsNull_ShouldHaveValidationErrorForDto()
    {
        // Arrange
        var command = new SubmitApplicationCommand(null!);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto)
            .WithErrorMessage("SubmitApplicationDto is required.");
    }

    [Fact]
    public void Validate_WhenCompanyIdIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new SubmitApplicationDto
        {
            CompanyId = Guid.Empty,
            ResumeRevisionId = Guid.NewGuid(),
            JobSource = "LinkedIn"
        };
        var command = new SubmitApplicationCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto.CompanyId)
            .WithErrorMessage("CompanyId is required.");
    }

    [Fact]
    public void Validate_WhenResumeRevisionIdIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new SubmitApplicationDto
        {
            CompanyId = Guid.NewGuid(),
            ResumeRevisionId = Guid.Empty,
            JobSource = "LinkedIn"
        };
        var command = new SubmitApplicationCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto.ResumeRevisionId)
            .WithErrorMessage("ResumeRevisionId is required.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_WhenJobSourceIsEmptyOrNull_ShouldHaveValidationError(string? invalidJobSource)
    {
        // Arrange
        var dto = new SubmitApplicationDto
        {
            CompanyId = Guid.NewGuid(),
            ResumeRevisionId = Guid.NewGuid(),
            JobSource = invalidJobSource!
        };
        var command = new SubmitApplicationCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto.JobSource)
            .WithErrorMessage("JobSource is required.");
    }

    [Fact]
    public void Validate_WhenJobSourceExceeds200Chars_ShouldHaveValidationError()
    {
        // Arrange
        var longJobSource = new string('A', 201);
        var dto = new SubmitApplicationDto
        {
            CompanyId = Guid.NewGuid(),
            ResumeRevisionId = Guid.NewGuid(),
            JobSource = longJobSource
        };
        var command = new SubmitApplicationCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto.JobSource)
            .WithErrorMessage("JobSource cannot exceed 200 characters.");
    }

    [Fact]
    public void Validate_WhenNotesExceed4000Chars_ShouldHaveValidationError()
    {
        // Arrange
        var longNotes = new string('N', 4001);
        var dto = new SubmitApplicationDto
        {
            CompanyId = Guid.NewGuid(),
            ResumeRevisionId = Guid.NewGuid(),
            JobSource = "LinkedIn",
            Notes = longNotes
        };
        var command = new SubmitApplicationCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto.Notes)
            .WithErrorMessage("Notes cannot exceed 4000 characters.");
    }
}
