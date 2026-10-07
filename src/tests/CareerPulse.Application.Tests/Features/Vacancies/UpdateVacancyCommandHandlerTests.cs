using CareerPulse.Application.DTOs.Vacancies;
using CareerPulse.Application.DTOs.VacancyLanguageRequirement;
using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Features.Vacancies.Commands.UpdateVacancy;
using CareerPulse.Application.Tests.TestHelpers;
using CareerPulse.Domain.Entities;
using CareerPulse.Domain.Enums;
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
        var vacancy = Vacancy.Create(company.Id, "Full Spec Vacancy", "Kyiv", WorkMode.Remote, EmploymentType.FullTime, 1000, 5000, SalaryCurrency.USD, "Full description", "Full responsibilities", "Full requirements", " Full NiceToHave", "Full Benefits", "https://detail.com/job");
        context.Vacancies.Add(vacancy);
        await context.SaveChangesAsync();

        var handler = new UpdateVacancyCommandHandler(context);
        var dto = new UpdateVacancyDto
        {
            Title = "Mid-level Dev",
            Description = "New desc",
            Url = "https://new.com",
            LanguageRequirements = []
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

        context.ChangeTracker.Clear();

        var dbVacancy = await context.Vacancies.FirstOrDefaultAsync(v => v.Id == vacancy.Id);
        dbVacancy.Should().NotBeNull();
        dbVacancy!.Title.Should().Be("Mid-level Dev");
        dbVacancy.Description.Should().Be("New desc");
        dbVacancy.Url.Should().Be("https://new.com");
    }

    [Fact]
    public async Task Handle_WhenVacancyDoesNotExist_ShouldThrowResourceNotFoundException()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var nonExistentId = Guid.NewGuid();
        var handler = new UpdateVacancyCommandHandler(context);
        var dto = new UpdateVacancyDto { Title = "Valid Title", LanguageRequirements = [] };
        var command = new UpdateVacancyCommand(nonExistentId, dto);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ResourceNotFoundException>()
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
            Url = "  https://trimmed.url.com  ",
            LanguageRequirements = []
        };
        var command = new UpdateVacancyCommand(vacancy.Id, dto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Title.Should().Be("Trimmed Title");
        result.Url.Should().Be("https://trimmed.url.com");

        context.ChangeTracker.Clear();

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
        var dto = new UpdateVacancyDto { Title = "   ", LanguageRequirements = [] };
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
        var vacancy = Vacancy.Create(company.Id, "Full Spec Vacancy", "Kyiv", WorkMode.Remote, EmploymentType.FullTime, 1000, 5000, SalaryCurrency.USD, "Full description", "Full responsibilities", "Full requirements", " Full NiceToHave", "Full Benefits", "https://detail.com/job");
        context.Vacancies.Add(vacancy);
        await context.SaveChangesAsync();

        var handler = new UpdateVacancyCommandHandler(context);
        var dto = new UpdateVacancyDto
        {
            Title = "Developer",
            Description = null,
            Url = null,
            LanguageRequirements = []
        };
        var command = new UpdateVacancyCommand(vacancy.Id, dto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Description.Should().BeNull();
        result.Url.Should().BeNull();

        context.ChangeTracker.Clear();

        var dbVacancy = await context.Vacancies.FirstOrDefaultAsync(v => v.Id == vacancy.Id);
        dbVacancy.Should().NotBeNull();
        dbVacancy!.Description.Should().BeNull();
        dbVacancy.Url.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WhenReplacingLanguageRequirements_ShouldUpdateSuccessfully()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var company = Company.Create("Meta");
        context.Companies.Add(company);

        var vacancy = Vacancy.Create(company.Id, "C# Developer");
        vacancy.AddLanguageRequirement("English", VacancyLanguageProficiency.B2, null);
        vacancy.AddLanguageRequirement("German", VacancyLanguageProficiency.B1, null);
        context.Vacancies.Add(vacancy);
        await context.SaveChangesAsync(CancellationToken.None);

        var dto = new UpdateVacancyDto
        {
            Title = "C# Developer",
            LanguageRequirements =
            [
                new UpdateVacancyLanguageRequirementDto
                {
                    LanguageName = "English",
                    Proficiency = VacancyLanguageProficiency.C1,
                    ProficiencyDescription = "Advanced English"
                },
                new UpdateVacancyLanguageRequirementDto
                {
                    LanguageName = "French",
                    Proficiency = VacancyLanguageProficiency.B2,
                    ProficiencyDescription = null
                }
            ]
        };

        var command = new UpdateVacancyCommand(vacancy.Id, dto);
        var handler = new UpdateVacancyCommandHandler(context);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert — application result
        result.Title.Should().Be("C# Developer");
        result.LanguageRequirements.Should().HaveCount(2);

        // Assert — persisted state
        context.ChangeTracker.Clear();

        var dbVacancy = await context.Vacancies
            .Include(x => x.LanguageRequirements)
            .SingleAsync(x => x.Id == vacancy.Id);

        dbVacancy.LanguageRequirements
            .Select(x => new { x.LanguageName, x.Proficiency, x.ProficiencyDescription })
            .Should()
            .BeEquivalentTo(
            [
                new
                {
                    LanguageName = "English",
                    Proficiency = VacancyLanguageProficiency.C1,
                    ProficiencyDescription = "Advanced English"
                },
                new
                {
                    LanguageName = "French",
                    Proficiency = VacancyLanguageProficiency.B2,
                    ProficiencyDescription = (string?)null
                }
            ]);
    }

    [Fact]
    public async Task Handle_WhenLanguageRequirementsAreEmpty_ShouldRemoveExistingRequirements()
    {
        // Arrange 
        using var context = TestDbContext.CreateInMemory();

        var company = Company.Create("Meta");
        context.Companies.Add(company);

        var vacancy = Vacancy.Create(company.Id, "C# Developer");
        vacancy.AddLanguageRequirement("English", VacancyLanguageProficiency.B2, null);
        vacancy.AddLanguageRequirement("German", VacancyLanguageProficiency.B1, null);
        context.Vacancies.Add(vacancy);
        await context.SaveChangesAsync(CancellationToken.None);

        var dto = new UpdateVacancyDto
        {
            Title = "C# Developer",
            LanguageRequirements = []
        };

        var command = new UpdateVacancyCommand(vacancy.Id, dto);
        var handler = new UpdateVacancyCommandHandler(context);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.LanguageRequirements.Should().BeEmpty();

        context.ChangeTracker.Clear();

        var dbVacancy = await context.Vacancies
            .Include(x => x.LanguageRequirements)
            .SingleAsync(x => x.Id == vacancy.Id);

        dbVacancy.LanguageRequirements.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WhenAddingLanguageRequirements_ShouldPersistThem()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var company = Company.Create("Meta");
        context.Companies.Add(company);

        var vacancy = Vacancy.Create(company.Id, "C# Developer");
        context.Vacancies.Add(vacancy);

        await context.SaveChangesAsync(CancellationToken.None);

        var dto = new UpdateVacancyDto
        {
            Title = "C# Developer",
            LanguageRequirements =
            [
                new UpdateVacancyLanguageRequirementDto
                {
                    LanguageName = "English",
                    Proficiency = VacancyLanguageProficiency.C1,
                    ProficiencyDescription = "Advanced English"
                },
                new UpdateVacancyLanguageRequirementDto
                {
                    LanguageName = "Ukrainian",
                    Proficiency = VacancyLanguageProficiency.C2,
                    ProficiencyDescription = null
                }
            ]
        };

        var command = new UpdateVacancyCommand(vacancy.Id, dto);
        var handler = new UpdateVacancyCommandHandler(context);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.LanguageRequirements
            .Select(x => x.LanguageName)
            .Should()
            .BeEquivalentTo(["English", "Ukrainian"]);

        context.ChangeTracker.Clear();

        var dbVacancy = await context.Vacancies
            .Include(x => x.LanguageRequirements)
            .SingleAsync(x => x.Id == vacancy.Id);

        dbVacancy.LanguageRequirements
            .Select(x => new { x.LanguageName, x.Proficiency, x.ProficiencyDescription })
            .Should()
        .BeEquivalentTo(
        [
            new
            {
                LanguageName = "English",
                Proficiency = VacancyLanguageProficiency.C1,
                ProficiencyDescription = "Advanced English"
            },
            new
            {
                LanguageName = "Ukrainian",
                Proficiency = VacancyLanguageProficiency.C2,
                ProficiencyDescription = (string?)null
            }
        ]);
    }

    [Fact]
    public async Task Handle_WhenLanguageRequirementsContainDuplicates_ShouldThrowDomainException()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var company = Company.Create("Meta");
        context.Companies.Add(company);

        var vacancy = Vacancy.Create(company.Id, "C# Developer");
        context.Vacancies.Add(vacancy);
        await context.SaveChangesAsync(CancellationToken.None);

        var dto = new UpdateVacancyDto
        {
            Title = "C# Developer",
            LanguageRequirements =
            [
                new UpdateVacancyLanguageRequirementDto
                {
                    LanguageName = "English",
                    Proficiency = VacancyLanguageProficiency.B2
                },
                new UpdateVacancyLanguageRequirementDto
                {
                    LanguageName = "english",
                    Proficiency = VacancyLanguageProficiency.C1
                }
            ]
        };

        var command = new UpdateVacancyCommand(vacancy.Id, dto);
        var handler = new UpdateVacancyCommandHandler(context);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("Duplicate language requirements are not allowed.");
    }

    [Fact]
    public async Task Handle_WhenLanguageRequirementsContainDuplicates_ShouldNotChangeAnyData()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var company = Company.Create("Meta");
        context.Companies.Add(company);

        var vacancy = Vacancy.Create(company.Id, "C# Developer");
        vacancy.AddLanguageRequirement("English", VacancyLanguageProficiency.B2, null);
        context.Vacancies.Add(vacancy);
        await context.SaveChangesAsync(CancellationToken.None);

        var dto = new UpdateVacancyDto
        {
            Title = "Changed Title",
            LanguageRequirements =
            [
                new UpdateVacancyLanguageRequirementDto { LanguageName = "German" },
                new UpdateVacancyLanguageRequirementDto { LanguageName = " german " }
            ]
        };

        var handler = new UpdateVacancyCommandHandler(context);

        // Act
        var act = () => handler.Handle(new UpdateVacancyCommand(vacancy.Id, dto), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>();

        context.ChangeTracker.Clear();

        var dbVacancy = await context.Vacancies
            .Include(x => x.LanguageRequirements)
            .SingleAsync(x => x.Id == vacancy.Id);

        dbVacancy.Title.Should().Be("C# Developer");
        dbVacancy.LanguageRequirements.Should().ContainSingle()
            .Which.LanguageName.Should().Be("English");
    }


    [Fact]
    public async Task Handle_WhenLanguageNameIsBlank_ShouldThrowDomainExceptionAndKeepExistingRequirements()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var company = Company.Create("Meta");
        context.Companies.Add(company);

        var vacancy = Vacancy.Create(company.Id, "C# Developer");
        vacancy.AddLanguageRequirement("English", VacancyLanguageProficiency.B2, null);
        context.Vacancies.Add(vacancy);
        await context.SaveChangesAsync(CancellationToken.None);

        var dto = new UpdateVacancyDto
        {
            Title = "C# Developer",
            LanguageRequirements =
            [
                new UpdateVacancyLanguageRequirementDto { LanguageName = "   " }
            ]
        };

        var handler = new UpdateVacancyCommandHandler(context);

        // Act
        var act = () => handler.Handle(new UpdateVacancyCommand(vacancy.Id, dto), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("LanguageName is required.");

        context.ChangeTracker.Clear();

        var dbVacancy = await context.Vacancies
            .Include(x => x.LanguageRequirements)
            .SingleAsync(x => x.Id == vacancy.Id);

        dbVacancy.LanguageRequirements.Should().ContainSingle()
            .Which.LanguageName.Should().Be("English");
    }

    [Fact]
    public async Task Handle_WithWhitespaceInLanguageFields_ShouldTrimValues()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var company = Company.Create("Meta");
        context.Companies.Add(company);

        var vacancy = Vacancy.Create(company.Id, "C# Developer");
        context.Vacancies.Add(vacancy);
        await context.SaveChangesAsync(CancellationToken.None);

        var dto = new UpdateVacancyDto
        {
            Title = "C# Developer",
            LanguageRequirements =
            [
                new UpdateVacancyLanguageRequirementDto
            {
                LanguageName = "  German  ",
                Proficiency = VacancyLanguageProficiency.B1,
                ProficiencyDescription = "  Conversational  "
            }
            ]
        };

        var handler = new UpdateVacancyCommandHandler(context);

        // Act
        var result = await handler.Handle(new UpdateVacancyCommand(vacancy.Id, dto), CancellationToken.None);

        // Assert
        result.LanguageRequirements.Should().ContainSingle()
            .Which.LanguageName.Should().Be("German");

        context.ChangeTracker.Clear();

        var saved = (await context.Vacancies
            .Include(x => x.LanguageRequirements)
            .SingleAsync(x => x.Id == vacancy.Id))
            .LanguageRequirements.Single();

        saved.LanguageName.Should().Be("German");
        saved.ProficiencyDescription.Should().Be("Conversational");
    }

    [Fact]
    public async Task Handle_WhenReplacingLanguageRequirements_ShouldNotAffectOtherVacancies()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var company = Company.Create("Meta");
        context.Companies.Add(company);

        var vacancy = Vacancy.Create(company.Id, "C# Developer");
        vacancy.AddLanguageRequirement("English", VacancyLanguageProficiency.B2, null);

        var other = Vacancy.Create(company.Id, "Java Developer");
        other.AddLanguageRequirement("English", VacancyLanguageProficiency.C1, "Fluent");
        other.AddLanguageRequirement("German", VacancyLanguageProficiency.B1, null);

        context.Vacancies.AddRange(vacancy, other);
        await context.SaveChangesAsync(CancellationToken.None);

        var dto = new UpdateVacancyDto
        {
            Title = "C# Developer",
            LanguageRequirements = []
        };

        var handler = new UpdateVacancyCommandHandler(context);

        // Act
        await handler.Handle(new UpdateVacancyCommand(vacancy.Id, dto), CancellationToken.None);

        // Assert
        context.ChangeTracker.Clear();

        var updated = await context.Vacancies
            .Include(x => x.LanguageRequirements)
            .SingleAsync(x => x.Id == vacancy.Id);
        var untouched = await context.Vacancies
            .Include(x => x.LanguageRequirements)
            .SingleAsync(x => x.Id == other.Id);

        updated.LanguageRequirements.Should().BeEmpty();
        untouched.LanguageRequirements.Select(x => x.LanguageName)
            .Should().BeEquivalentTo(["English", "German"]);
    }
}
