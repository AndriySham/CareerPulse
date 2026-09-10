using CareerPulse.Application.DTOs.Vacancies;
using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Features.Vacancies.Commands.CreateVacancy;
using CareerPulse.Application.Tests.TestHelpers;
using CareerPulse.Domain.Entities;
using CareerPulse.Domain.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CareerPulse.Application.Tests.Features.Vacancies;

public class CreateVacancyCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidData_ShouldCreateVacancyAndReturnDto()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var company = Company.Create("Tech Inc");
        context.Companies.Add(company);
        await context.SaveChangesAsync();

        var handler = new CreateVacancyCommandHandler(context);
        var postedAt = DateTime.UtcNow.AddDays(-1);
        var dto = new CreateVacancyDto
        {
            CompanyId = company.Id,
            Title = "Senior C# Developer",
            Description = "Exciting opportunity in .NET",
            Url = "https://techinc.com/careers/123",
            PostedAt = postedAt
        };
        var command = new CreateVacancyCommand(dto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.CompanyId.Should().Be(company.Id);
        result.Title.Should().Be("Senior C# Developer");
        result.Description.Should().Be("Exciting opportunity in .NET");
        result.Url.Should().Be("https://techinc.com/careers/123");
        result.PostedAt.Should().Be(postedAt);
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        var dbVacancy = await context.Vacancies.FirstOrDefaultAsync(v => v.Id == result.Id);
        dbVacancy.Should().NotBeNull();
        dbVacancy!.CompanyId.Should().Be(company.Id);
        dbVacancy.Title.Should().Be("Senior C# Developer");
        dbVacancy.Description.Should().Be("Exciting opportunity in .NET");
        dbVacancy.Url.Should().Be("https://techinc.com/careers/123");
        dbVacancy.PostedAt.Should().Be(postedAt);
    }

    [Fact]
    public async Task Handle_WithWhitespaceInFields_ShouldTrimValuesAndCreate()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var company = Company.Create("SoftCorp");
        context.Companies.Add(company);
        await context.SaveChangesAsync();

        var handler = new CreateVacancyCommandHandler(context);
        var dto = new CreateVacancyDto
        {
            CompanyId = company.Id,
            Title = "  QA Lead Engineer  ",
            Description = "Untrimmed description",
            Url = "  https://softcorp.com/qa  "
        };
        var command = new CreateVacancyCommand(dto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Title.Should().Be("QA Lead Engineer");
        result.Url.Should().Be("https://softcorp.com/qa");

        var dbVacancy = await context.Vacancies.FirstOrDefaultAsync(v => v.Id == result.Id);
        dbVacancy.Should().NotBeNull();
        dbVacancy!.Title.Should().Be("QA Lead Engineer");
        dbVacancy.Url.Should().Be("https://softcorp.com/qa");
    }

    [Fact]
    public async Task Handle_WhenCompanyDoesNotExist_ShouldThrowResourceNotFoundException()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var nonExistentCompanyId = Guid.NewGuid();
        var handler = new CreateVacancyCommandHandler(context);
        var dto = new CreateVacancyDto
        {
            CompanyId = nonExistentCompanyId,
            Title = "Orphan Vacancy"
        };
        var command = new CreateVacancyCommand(dto);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ResourceNotFoundException>()
            .WithMessage($"Company with ID '{nonExistentCompanyId}' was not found.");
    }

    [Fact]
    public async Task Handle_WhenTitleIsEmptyOrWhitespace_ShouldThrowDomainException()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var company = Company.Create("Innovate LLC");
        context.Companies.Add(company);
        await context.SaveChangesAsync();

        var handler = new CreateVacancyCommandHandler(context);
        var dto = new CreateVacancyDto
        {
            CompanyId = company.Id,
            Title = "   "
        };
        var command = new CreateVacancyCommand(dto);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("Vacancy title is required.");
    }

    [Fact]
    public async Task Handle_WithOptionalNullFields_ShouldCreateVacancyWithNullOptionaFields()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var company = Company.Create("BareBones Co");
        context.Companies.Add(company);
        await context.SaveChangesAsync();

        var dto = new CreateVacancyDto
        {
            CompanyId = company.Id,
            Title = "Simple Developer"
        };

        var handler = new CreateVacancyCommandHandler(context);
        var command = new CreateVacancyCommand(dto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.CompanyId.Should().Be(company.Id);
        result.Title.Should().Be("Simple Developer");

        result.Location.Should().BeNull();
        result.WorkMode.Should().BeNull();
        result.EmploymentType.Should().BeNull();
        result.SalaryMin.Should().BeNull();
        result.SalaryMax.Should().BeNull();
        result.SalaryCurrency.Should().BeNull();
        result.Description.Should().BeNull();
        result.Responsibilities.Should().BeNull();
        result.Requirements.Should().BeNull();
        result.NiceToHave.Should().BeNull();
        result.Benefits.Should().BeNull();
        result.Url.Should().BeNull();
        result.PostedAt.Should().BeNull();

        var dbVacancy = await context.Vacancies
            .FirstOrDefaultAsync(v => v.Id == result.Id);

        dbVacancy.Should().NotBeNull();

        dbVacancy!.Location.Should().BeNull();
        dbVacancy.WorkMode.Should().BeNull();
        dbVacancy.EmploymentType.Should().BeNull();
        dbVacancy.SalaryMin.Should().BeNull();
        dbVacancy.SalaryMax.Should().BeNull();
        dbVacancy.SalaryCurrency.Should().BeNull();
        dbVacancy.Description.Should().BeNull();
        dbVacancy.Responsibilities.Should().BeNull();
        dbVacancy.Requirements.Should().BeNull();
        dbVacancy.NiceToHave.Should().BeNull();
        dbVacancy.Benefits.Should().BeNull();
        dbVacancy.Url.Should().BeNull();
        dbVacancy.PostedAt.Should().BeNull();
    }
}
