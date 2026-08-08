using CareerPulse.Application.DTOs.Companies;
using CareerPulse.Application.Features.Companies.Commands.UpdateCompany;
using FluentValidation.TestHelper;
using Xunit;

namespace CareerPulse.Application.Tests.Features.Companies;

public class UpdateCompanyCommandValidatorTests
{
    private readonly UpdateCompanyCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenCommandIsValid_ShouldNotHaveAnyValidationErrors()
    {
        // Arrange
        var dto = new UpdateCompanyDto
        {
            Name = "Valid Corp",
            Website = "https://validcorp.com",
            Industry = "Finance",
            Notes = "Good company"
        };
        var command = new UpdateCompanyCommand(Guid.NewGuid(), dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenIdIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new UpdateCompanyDto { Name = "Valid Corp" };
        var command = new UpdateCompanyCommand(Guid.Empty, dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorMessage("Company ID is required.");
    }

    [Fact]
    public void Validate_WhenDtoIsNull_ShouldHaveValidationErrorForDto()
    {
        // Arrange
        var command = new UpdateCompanyCommand(Guid.NewGuid(), null!);

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
        var dto = new UpdateCompanyDto { Name = invalidName! };
        var command = new UpdateCompanyCommand(Guid.NewGuid(), dto);

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
        var dto = new UpdateCompanyDto { Name = longName };
        var command = new UpdateCompanyCommand(Guid.NewGuid(), dto);

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
        var dto = new UpdateCompanyDto { Name = exactName };
        var command = new UpdateCompanyCommand(Guid.NewGuid(), dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Dto.Name);
    }

    [Fact]
    public void Validate_WhenWebsiteExceeds500Chars_ShouldHaveValidationError()
    {
        // Arrange
        var longWebsite = "https://" + new string('w', 500);
        var dto = new UpdateCompanyDto { Name = "Valid Name", Website = longWebsite };
        var command = new UpdateCompanyCommand(Guid.NewGuid(), dto);

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
        var exactWebsite = new string('w', 500);
        var dto = new UpdateCompanyDto { Name = "Valid Name", Website = exactWebsite };
        var command = new UpdateCompanyCommand(Guid.NewGuid(), dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Dto.Website);
    }

    [Fact]
    public void Validate_WhenIndustryExceeds150Chars_ShouldHaveValidationError()
    {
        // Arrange
        var longIndustry = new string('I', 151);
        var dto = new UpdateCompanyDto { Name = "Valid Name", Industry = longIndustry };
        var command = new UpdateCompanyCommand(Guid.NewGuid(), dto);

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
        var exactIndustry = new string('I', 150);
        var dto = new UpdateCompanyDto { Name = "Valid Name", Industry = exactIndustry };
        var command = new UpdateCompanyCommand(Guid.NewGuid(), dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Dto.Industry);
    }

    [Fact]
    public void Validate_WhenNotesExceed4000Chars_ShouldHaveValidationError()
    {
        // Arrange
        var longNotes = new string('N', 4001);
        var dto = new UpdateCompanyDto { Name = "Valid Name", Notes = longNotes };
        var command = new UpdateCompanyCommand(Guid.NewGuid(), dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto.Notes)
            .WithErrorMessage("Notes must not exceed 4000 characters.");
    }

    [Fact]
    public void Validate_WhenNotesIsExact4000Chars_ShouldNotHaveValidationError()
    {
        // Arrange
        var exactNotes = new string('N', 4000);
        var dto = new UpdateCompanyDto { Name = "Valid Name", Notes = exactNotes };
        var command = new UpdateCompanyCommand(Guid.NewGuid(), dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Dto.Notes);
    }
}
