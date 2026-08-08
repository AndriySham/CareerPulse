using CareerPulse.Application.DTOs.Vacancies;
using CareerPulse.Application.Features.Vacancies.Commands.UpdateVacancy;
using CareerPulse.Application.Tests.TestHelpers;
using CareerPulse.Domain.Entities;
using CareerPulse.Domain.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CareerPulse.Application.Tests.Features.Vacancies;

public class UpdateVacancyCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidData_ShouldUpdateVacancyAndReturnDto()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var company = Company.Create("TechCorp");
        context.Companies.Add(company);
        var vacancy = Vacancy.Create(company.Id, "Junior Dev", "Old desc", "https://old.com");
        context.Vacancies.Add(vacancy);
        await context.SaveChangesAsync();

        var handler = new UpdateVacancyCommandHandler(context);
        var dto = new UpdateVacancyDto
        {
            Title = "Mid-level Dev",
            Description = "New desc",
            Url = "https://new.com"
        };
        var command = new UpdateVacancyCommand(vacancy.Id, dto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(vacancy.Id);
        result.CompanyId.Should().Be(company.Id);
        result.Title.Should().Be("Mid-level Dev");
        result.Description.Should().Be("New desc");
        result.Url.Should().Be("https://new.com");
        result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        var dbVacancy = await context.Vacancies.FirstOrDefaultAsync(v => v.Id == vacancy.Id);
        dbVacancy.Should().NotBeNull();
        dbVacancy!.Title.Should().Be("Mid-level Dev");
        dbVacancy.Description.Should().Be("New desc");
        dbVacancy.Url.Should().Be("https://new.com");
    }

    [Fact]
    public async Task Handle_WhenVacancyDoesNotExist_ShouldThrowDomainException()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var nonExistentId = Guid.NewGuid();
        var handler = new UpdateVacancyCommandHandler(context);
        var dto = new UpdateVacancyDto { Title = "Valid Title" };
        var command = new UpdateVacancyCommand(nonExistentId, dto);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage($"Vacancy with ID '{nonExistentId}' was not found.");
    }

    [Fact]
    public async Task Handle_WithWhitespaceInFields_ShouldTrimValuesAndUpdate()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var company = Company.Create("InnovateCorp");
        context.Companies.Add(company);
        var vacancy = Vacancy.Create(company.Id, "Original Title");
        context.Vacancies.Add(vacancy);
        await context.SaveChangesAsync();

        var handler = new UpdateVacancyCommandHandler(context);
        var dto = new UpdateVacancyDto
        {
            Title = "  Trimmed Title  ",
            Description = "Description text",
            Url = "  https://trimmed.url.com  "
        };
        var command = new UpdateVacancyCommand(vacancy.Id, dto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Title.Should().Be("Trimmed Title");
        result.Url.Should().Be("https://trimmed.url.com");

        var dbVacancy = await context.Vacancies.FirstOrDefaultAsync(v => v.Id == vacancy.Id);
        dbVacancy.Should().NotBeNull();
        dbVacancy!.Title.Should().Be("Trimmed Title");
        dbVacancy.Url.Should().Be("https://trimmed.url.com");
    }

    [Fact]
    public async Task Handle_WhenTitleIsEmptyOrWhitespace_ShouldThrowDomainException()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var company = Company.Create("BuildCorp");
        context.Companies.Add(company);
        var vacancy = Vacancy.Create(company.Id, "Existing Vacancy");
        context.Vacancies.Add(vacancy);
        await context.SaveChangesAsync();

        var handler = new UpdateVacancyCommandHandler(context);
        var dto = new UpdateVacancyDto { Title = "   " };
        var command = new UpdateVacancyCommand(vacancy.Id, dto);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("Vacancy title is required.");
    }

    [Fact]
    public async Task Handle_WhenUpdatingOptionalFieldsToNull_ShouldUpdateDescriptionAndUrlToNull()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var company = Company.Create("ClearCorp");
        context.Companies.Add(company);
        var vacancy = Vacancy.Create(company.Id, "Developer", "Has description", "https://has.url");
        context.Vacancies.Add(vacancy);
        await context.SaveChangesAsync();

        var handler = new UpdateVacancyCommandHandler(context);
        var dto = new UpdateVacancyDto
        {
            Title = "Developer",
            Description = null,
            Url = null
        };
        var command = new UpdateVacancyCommand(vacancy.Id, dto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Description.Should().BeNull();
        result.Url.Should().BeNull();

        var dbVacancy = await context.Vacancies.FirstOrDefaultAsync(v => v.Id == vacancy.Id);
        dbVacancy.Should().NotBeNull();
        dbVacancy!.Description.Should().BeNull();
        dbVacancy.Url.Should().BeNull();
    }
}
