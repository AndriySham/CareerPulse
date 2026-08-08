using CareerPulse.Application.DTOs.Vacancies;
using CareerPulse.Application.Features.Vacancies.Commands.CreateVacancy;
using CareerPulse.Domain.Entities;
using CareerPulse.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace CareerPulse.Application.Tests.Features.Vacancies;

public class VacancyDomainTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldInstantiateVacancyCorrectly()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var title = "Senior Backend Engineer";
        var description = "Great role with .NET 9";
        var url = "https://example.com/jobs/123";
        var postedAt = DateTime.UtcNow.AddDays(-2);

        // Act
        var vacancy = Vacancy.Create(companyId, title, description, url, postedAt);

        // Assert
        vacancy.Should().NotBeNull();
        vacancy.Id.Should().NotBeEmpty();
        vacancy.CompanyId.Should().Be(companyId);
        vacancy.Title.Should().Be("Senior Backend Engineer");
        vacancy.Description.Should().Be("Great role with .NET 9");
        vacancy.Url.Should().Be("https://example.com/jobs/123");
        vacancy.PostedAt.Should().Be(postedAt);
        vacancy.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        vacancy.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Create_WithNullOptionalParameters_ShouldInstantiateVacancyWithNulls()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var title = "Frontend Developer";

        // Act
        var vacancy = Vacancy.Create(companyId, title);

        // Assert
        vacancy.CompanyId.Should().Be(companyId);
        vacancy.Title.Should().Be("Frontend Developer");
        vacancy.Description.Should().BeNull();
        vacancy.Url.Should().BeNull();
        vacancy.PostedAt.Should().BeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyOrWhitespaceTitle_ShouldThrowDomainException(string? invalidTitle)
    {
        // Arrange
        var companyId = Guid.NewGuid();

        // Act
        var act = () => Vacancy.Create(companyId, invalidTitle!);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Vacancy title is required.");
    }

    [Fact]
    public void Create_WithLeadingAndTrailingWhitespaceInTitleAndUrl_ShouldTrimStrings()
    {
        // Arrange
        var companyId = Guid.NewGuid();

        // Act
        var vacancy = Vacancy.Create(
            companyId,
            "  DevOps Lead  ",
            "  Some description  ",
            "  https://jobs.example.com/456  "
        );

        // Assert
        vacancy.Title.Should().Be("DevOps Lead");
        vacancy.Description.Should().Be("  Some description  ");
        vacancy.Url.Should().Be("https://jobs.example.com/456");
    }

    [Fact]
    public void Update_WithValidData_ShouldUpdateTitleDescriptionUrlAndSetUpdatedAt()
    {
        // Arrange
        var vacancy = Vacancy.Create(Guid.NewGuid(), "Original Title", "Original Desc", "https://old.url");
        var initialUpdatedAt = vacancy.UpdatedAt;

        // Act
        vacancy.Update("Updated Title", "Updated Desc", "https://new.url");

        // Assert
        vacancy.Title.Should().Be("Updated Title");
        vacancy.Description.Should().Be("Updated Desc");
        vacancy.Url.Should().Be("https://new.url");
        vacancy.UpdatedAt.Should().BeOnOrAfter(initialUpdatedAt);
        vacancy.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Update_WithNullOptionalFields_ShouldSetDescriptionAndUrlToNull()
    {
        // Arrange
        var vacancy = Vacancy.Create(Guid.NewGuid(), "Title", "Desc", "https://url.com");

        // Act
        vacancy.Update("Updated Title", null, null);

        // Assert
        vacancy.Title.Should().Be("Updated Title");
        vacancy.Description.Should().BeNull();
        vacancy.Url.Should().BeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Update_WithEmptyOrWhitespaceTitle_ShouldThrowDomainException(string? invalidTitle)
    {
        // Arrange
        var vacancy = Vacancy.Create(Guid.NewGuid(), "Valid Title");

        // Act
        var act = () => vacancy.Update(invalidTitle!);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Vacancy title is required.");
    }

    [Fact]
    public void Update_WithWhitespaceTitleAndUrl_ShouldTrimValues()
    {
        // Arrange
        var vacancy = Vacancy.Create(Guid.NewGuid(), "Title");

        // Act
        vacancy.Update("  Pricipal Engineer  ", "Desc", "  https://trimmed.com  ");

        // Assert
        vacancy.Title.Should().Be("Pricipal Engineer");
        vacancy.Url.Should().Be("https://trimmed.com");
    }

    [Fact]
    public void MapToDto_ShouldMapAllPropertiesFromEntityToVacancyDto()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var postedAt = DateTime.UtcNow.AddDays(-1);
        var vacancy = Vacancy.Create(companyId, "Fullstack Engineer", "Fullstack desc", "https://fullstack.io", postedAt);

        // Act
        var dto = CreateVacancyCommandHandler.MapToDto(vacancy);

        // Assert
        dto.Should().NotBeNull();
        dto.Id.Should().Be(vacancy.Id);
        dto.CompanyId.Should().Be(vacancy.CompanyId);
        dto.Title.Should().Be(vacancy.Title);
        dto.Description.Should().Be(vacancy.Description);
        dto.Url.Should().Be(vacancy.Url);
        dto.PostedAt.Should().Be(vacancy.PostedAt);
        dto.CreatedAt.Should().Be(vacancy.CreatedAt);
        dto.UpdatedAt.Should().Be(vacancy.UpdatedAt);
    }
}
