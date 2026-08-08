using CareerPulse.Application.DTOs.Companies;
using CareerPulse.Application.Features.Companies.Commands.CreateCompany;
using FluentValidation.TestHelper;
using Xunit;

namespace CareerPulse.Application.Tests.Features.Companies;

public class CreateCompanyCommandValidatorTests
{
    private readonly CreateCompanyCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenCommandIsValid_ShouldNotHaveAnyValidationErrors()
    {
        // Arrange
        var command = new CreateCompanyCommand("Acme Corp", "https://acme.com", "Manufacturing");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenDtoIsNull_ShouldHaveValidationErrorForDto()
    {
        // Arrange
        var command = new CreateCompanyCommand(null!);

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
    public void Validate_WhenNameIsEmptyOrNullOrWhitespace_ShouldHaveValidationError(string? invalidName)
    {
        // Arrange
        var dto = new CreateCompanyDto { Name = invalidName!, Website = "https://example.com" };
        var command = new CreateCompanyCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto.Name)
            .WithErrorMessage("Company name is required.");
    }

    [Fact]
    public void Validate_WhenNameExceeds300Chars_ShouldHaveValidationError()
    {
        // Arrange
        var longName = new string('A', 301);
        var dto = new CreateCompanyDto { Name = longName };
        var command = new CreateCompanyCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto.Name)
            .WithErrorMessage("Company name must not exceed 300 characters.");
    }

    [Fact]
    public void Validate_WhenNameIsExact300Chars_ShouldNotHaveValidationError()
    {
        // Arrange
        var exactName = new string('A', 300);
        var dto = new CreateCompanyDto { Name = exactName };
        var command = new CreateCompanyCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Dto.Name);
    }

    [Fact]
    public void Validate_WhenWebsiteExceeds500Chars_ShouldHaveValidationError()
    {
        // Arrange
        var longWebsite = "https://" + new string('a', 500);
        var dto = new CreateCompanyDto { Name = "Valid Name", Website = longWebsite };
        var command = new CreateCompanyCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto.Website)
            .WithErrorMessage("Website URL must not exceed 500 characters.");
    }

    [Fact]
    public void Validate_WhenWebsiteIsExact500Chars_ShouldNotHaveValidationError()
    {
        // Arrange
        var exactWebsite = new string('a', 500);
        var dto = new CreateCompanyDto { Name = "Valid Name", Website = exactWebsite };
        var command = new CreateCompanyCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Dto.Website);
    }

    [Fact]
    public void Validate_WhenIndustryExceeds150Chars_ShouldHaveValidationError()
    {
        // Arrange
        var longIndustry = new string('B', 151);
        var dto = new CreateCompanyDto { Name = "Valid Name", Industry = longIndustry };
        var command = new CreateCompanyCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto.Industry)
            .WithErrorMessage("Industry must not exceed 150 characters.");
    }

    [Fact]
    public void Validate_WhenIndustryIsExact150Chars_ShouldNotHaveValidationError()
    {
        // Arrange
        var exactIndustry = new string('B', 150);
        var dto = new CreateCompanyDto { Name = "Valid Name", Industry = exactIndustry };
        var command = new CreateCompanyCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Dto.Industry);
    }

    [Fact]
    public void Validate_WhenWebsiteAndIndustryAreNull_ShouldPassValidation()
    {
        // Arrange
        var dto = new CreateCompanyDto { Name = "Valid Name", Website = null, Industry = null };
        var command = new CreateCompanyCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
