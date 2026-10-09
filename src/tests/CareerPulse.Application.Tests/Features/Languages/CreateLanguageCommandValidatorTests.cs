using CareerPulse.Application.DTOs.Languages;
using CareerPulse.Application.Features.Languages.Commands.CreateLanguage;
using CareerPulse.Domain.Enums;
using FluentValidation.TestHelper;
using Xunit;

namespace CareerPulse.Application.Tests.Features.Languages;

public class CreateLanguageCommandValidatorTests
{
    private readonly CreateLanguageCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenCommandIsValid_ShouldNotHaveAnyValidationErrors()
    {
        var dto = new CreateLanguageDto
        {
            ResumeRevisionId = Guid.NewGuid(),
            LanguageName = "English",
            Proficiency = LanguageProficiency.C1
        };

        var result = _validator.TestValidate(new CreateLanguageCommand(dto));
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenDtoIsNull_ShouldHaveValidationErrorForDto()
    {
        var result = _validator.TestValidate(new CreateLanguageCommand(null!));
        result.ShouldHaveValidationErrorFor(x => x.Dto)
            .WithErrorMessage("Request body cannot be null.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WhenLanguageNameIsEmpty_ShouldHaveValidationError(string? name)
    {
        var dto = new CreateLanguageDto
        {
            ResumeRevisionId = Guid.NewGuid(),
            LanguageName = name!,
            Proficiency = LanguageProficiency.B2
        };

        var result = _validator.TestValidate(new CreateLanguageCommand(dto));
        result.ShouldHaveValidationErrorFor(x => x.Dto.LanguageName)
            .WithErrorMessage("Language Name is required.");
    }
}
