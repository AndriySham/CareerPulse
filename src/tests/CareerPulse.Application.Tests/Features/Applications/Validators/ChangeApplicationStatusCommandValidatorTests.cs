using CareerPulse.Application.DTOs.Applications;
using CareerPulse.Application.Features.Applications.Commands.ChangeApplicationStatus;
using CareerPulse.Domain.Enums;
using FluentValidation.TestHelper;
using Xunit;

namespace CareerPulse.Application.Tests.Features.Applications.Validators;

public class ChangeApplicationStatusCommandValidatorTests
{
    private readonly ChangeApplicationStatusCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenCommandIsValid_ShouldNotHaveAnyValidationErrors()
    {
        // Arrange
        var dto = new ChangeApplicationStatusDto
        {
            NewStatus = ApplicationStatus.Applied,
            Notes = "Valid notes"
        };
        var command = new ChangeApplicationStatusCommand(Guid.NewGuid(), dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenIdIsEmpty_ShouldHaveValidationErrorForId()
    {
        // Arrange
        var dto = new ChangeApplicationStatusDto { NewStatus = ApplicationStatus.Applied };
        var command = new ChangeApplicationStatusCommand(Guid.Empty, dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorMessage("Application ID is required.");
    }

    [Fact]
    public void Validate_WhenDtoIsNull_ShouldHaveValidationErrorForDto()
    {
        // Arrange
        var command = new ChangeApplicationStatusCommand(Guid.NewGuid(), null!);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto)
            .WithErrorMessage("ChangeApplicationStatusDto is required.");
    }

    [Fact]
    public void Validate_WhenNewStatusIsInvalidEnum_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new ChangeApplicationStatusDto { NewStatus = (ApplicationStatus)999 };
        var command = new ChangeApplicationStatusCommand(Guid.NewGuid(), dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto.NewStatus)
            .WithErrorMessage("Valid ApplicationStatus is required.");
    }

    [Fact]
    public void Validate_WhenNotesExceed4000Chars_ShouldHaveValidationError()
    {
        // Arrange
        var longNotes = new string('X', 4001);
        var dto = new ChangeApplicationStatusDto
        {
            NewStatus = ApplicationStatus.Applied,
            Notes = longNotes
        };
        var command = new ChangeApplicationStatusCommand(Guid.NewGuid(), dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto.Notes)
            .WithErrorMessage("Notes cannot exceed 4000 characters.");
    }
}
