using CareerPulse.Application.DTOs.Vacancies;
using CareerPulse.Application.Features.Vacancies.Commands.CreateVacancy;
using FluentValidation.TestHelper;
using Xunit;

namespace CareerPulse.Application.Tests.Features.Vacancies;

public class CreateVacancyCommandValidatorTests
{
    private readonly CreateVacancyCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenCommandIsValid_ShouldNotHaveAnyValidationErrors()
    {
        // Arrange
        var command = new CreateVacancyCommand(
            Guid.NewGuid(),
            "Software Architect",
            "Designing scalable systems",
            "https://company.com/architect",
            DateTime.UtcNow
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenDtoIsNull_ShouldHaveValidationErrorForDto()
    {
        // Arrange
        var command = new CreateVacancyCommand(null!);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto)
            .WithErrorMessage("Request body cannot be null.");
    }

    [Fact]
    public void Validate_WhenCompanyIdIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateVacancyDto
        {
            CompanyId = Guid.Empty,
            Title = "Backend Engineer"
        };
        var command = new CreateVacancyCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto.CompanyId)
            .WithErrorMessage("Company ID is required.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WhenTitleIsEmptyOrNullOrWhitespace_ShouldHaveValidationError(string? invalidTitle)
    {
        // Arrange
        var dto = new CreateVacancyDto
        {
            CompanyId = Guid.NewGuid(),
            Title = invalidTitle!
        };
        var command = new CreateVacancyCommand(dto);

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
        var dto = new CreateVacancyDto
        {
            CompanyId = Guid.NewGuid(),
            Title = longTitle
        };
        var command = new CreateVacancyCommand(dto);

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
        var dto = new CreateVacancyDto
        {
            CompanyId = Guid.NewGuid(),
            Title = exactTitle
        };
        var command = new CreateVacancyCommand(dto);

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
        var dto = new CreateVacancyDto
        {
            CompanyId = Guid.NewGuid(),
            Title = "Valid Title",
            Url = longUrl
        };
        var command = new CreateVacancyCommand(dto);

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
        var dto = new CreateVacancyDto
        {
            CompanyId = Guid.NewGuid(),
            Title = "Valid Title",
            Url = exactUrl
        };
        var command = new CreateVacancyCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Dto.Url);
    }

    [Fact]
    public void Validate_WhenOptionalFieldsAreNull_ShouldPassValidation()
    {
        // Arrange
        var dto = new CreateVacancyDto
        {
            CompanyId = Guid.NewGuid(),
            Title = "Valid Title",
            Description = null,
            Url = null,
            PostedAt = null
        };
        var command = new CreateVacancyCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
