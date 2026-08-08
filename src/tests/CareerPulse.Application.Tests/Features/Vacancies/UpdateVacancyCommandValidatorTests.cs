using CareerPulse.Application.DTOs.Vacancies;
using CareerPulse.Application.Features.Vacancies.Commands.UpdateVacancy;
using FluentValidation.TestHelper;
using Xunit;

namespace CareerPulse.Application.Tests.Features.Vacancies;

public class UpdateVacancyCommandValidatorTests
{
    private readonly UpdateVacancyCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenCommandIsValid_ShouldNotHaveAnyValidationErrors()
    {
        // Arrange
        var dto = new UpdateVacancyDto
        {
            Title = "Principal Architect",
            Description = "Updated description",
            Url = "https://company.com/vacancy"
        };
        var command = new UpdateVacancyCommand(Guid.NewGuid(), dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenIdIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new UpdateVacancyDto { Title = "Valid Title" };
        var command = new UpdateVacancyCommand(Guid.Empty, dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorMessage("Vacancy ID is required.");
    }

    [Fact]
    public void Validate_WhenDtoIsNull_ShouldHaveValidationErrorForDto()
    {
        // Arrange
        var command = new UpdateVacancyCommand(Guid.NewGuid(), null!);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto)
            .WithErrorMessage("Request body cannot be null.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WhenTitleIsEmptyOrNullOrWhitespace_ShouldHaveValidationError(string? invalidTitle)
    {
        // Arrange
        var dto = new UpdateVacancyDto { Title = invalidTitle! };
        var command = new UpdateVacancyCommand(Guid.NewGuid(), dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto.Title)
            .WithErrorMessage("Vacancy title is required.");
    }

    [Fact]
    public void Validate_WhenTitleExceeds300Chars_ShouldHaveValidationError()
    {
        // Arrange
        var longTitle = new string('T', 301);
        var dto = new UpdateVacancyDto { Title = longTitle };
        var command = new UpdateVacancyCommand(Guid.NewGuid(), dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto.Title)
            .WithErrorMessage("Vacancy title must not exceed 300 characters.");
    }

    [Fact]
    public void Validate_WhenTitleIsExact300Chars_ShouldNotHaveValidationError()
    {
        // Arrange
        var exactTitle = new string('T', 300);
        var dto = new UpdateVacancyDto { Title = exactTitle };
        var command = new UpdateVacancyCommand(Guid.NewGuid(), dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Dto.Title);
    }

    [Fact]
    public void Validate_WhenUrlExceeds1000Chars_ShouldHaveValidationError()
    {
        // Arrange
        var longUrl = "https://" + new string('u', 1000);
        var dto = new UpdateVacancyDto { Title = "Valid Title", Url = longUrl };
        var command = new UpdateVacancyCommand(Guid.NewGuid(), dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto.Url)
            .WithErrorMessage("Url must not exceed 1000 characters.");
    }

    [Fact]
    public void Validate_WhenUrlIsExact1000Chars_ShouldNotHaveValidationError()
    {
        // Arrange
        var exactUrl = new string('u', 1000);
        var dto = new UpdateVacancyDto { Title = "Valid Title", Url = exactUrl };
        var command = new UpdateVacancyCommand(Guid.NewGuid(), dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Dto.Url);
    }

    [Fact]
    public void Validate_WhenOptionalFieldsAreNull_ShouldPassValidation()
    {
        // Arrange
        var dto = new UpdateVacancyDto
        {
            Title = "Valid Title",
            Description = null,
            Url = null
        };
        var command = new UpdateVacancyCommand(Guid.NewGuid(), dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
