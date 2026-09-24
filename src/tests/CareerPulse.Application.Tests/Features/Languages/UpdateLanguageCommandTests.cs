using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Tests.TestHelpers;
using CareerPulse.Domain.Entities;
using CareerPulse.Domain.Enums;
using CareerPulse.Domain.ValueObjects;
using FluentAssertions;
using Xunit;
using CareerPulse.Application.DTOs.Languages;
using CareerPulse.Application.Features.Languages.Commands.UpdateLanguage;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Tests.Features.Languages;

public class UpdateLanguageCommandTests
{
    [Fact]
    public async Task Handle_WithValidData_ShouldUpdateLanguageAndReturnDto()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var personalInfo = PersonalInfo.Create("Alice Smith", "alice@example.com");
        var resume = Resume.Create("Alice's Resume", ResumeTrack.FullStack, CareerLevel.Senior, "Dotnet Dev");
        var revision = resume.CreateFirstRevision("Dotnet Dev", personalInfo);
        var language = Language.Create(revision.Id, "English", LanguageProficiency.A1);

        context.Resumes.Add(resume);
        context.ResumeRevisions.Add(revision);
        context.Languages.Add(language);
        await context.SaveChangesAsync(CancellationToken.None);

        var dto = new UpdateLanguageDto
        {
            LanguageName = "English",
            Proficiency = LanguageProficiency.A2
        };

        var command = new UpdateLanguageCommand(language.Id, dto);
        var handler = new UpdateLanguageCommandHandler(context);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.ResumeRevisionId.Should().Be(revision.Id);
        result.LanguageName.Should().Be("English");
        result.Proficiency.Should().Be(LanguageProficiency.A2);

        var dbLanguage = await context.Languages
            .FirstOrDefaultAsync(x => x.Id == result.Id, CancellationToken.None);
        dbLanguage.Should().NotBeNull();
        dbLanguage.Id.Should().Be(result.Id);
        dbLanguage.ResumeRevisionId.Should().Be(revision.Id);
        dbLanguage.LanguageName.Should().Be("English");
        dbLanguage.Proficiency.Should().Be(LanguageProficiency.A2);
    }

    [Fact]
    public async Task Handle_WithWhitespaceAroundLanguageName_ShouldTrimLanguageName()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var personalInfo = PersonalInfo.Create("Alice Smith", "alice@example.com");
        var resume = Resume.Create("Alice's Resume", ResumeTrack.FullStack, CareerLevel.Senior, "Dotnet Dev");
        var revision = resume.CreateFirstRevision("Dotnet Dev", personalInfo);
        var language = Language.Create(revision.Id, "English", LanguageProficiency.A1);

        context.Resumes.Add(resume);
        context.ResumeRevisions.Add(revision);
        context.Languages.Add(language);
        await context.SaveChangesAsync(CancellationToken.None);

        var dto = new UpdateLanguageDto
        {
            LanguageName = "  English   ",
            Proficiency = LanguageProficiency.A2
        };

        var command = new UpdateLanguageCommand(language.Id, dto);
        var handler = new UpdateLanguageCommandHandler(context);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.LanguageName.Should().Be("English");

        var dbLanguage = await context.Languages
            .FirstOrDefaultAsync(x => x.Id == result.Id, CancellationToken.None);
        dbLanguage.Should().NotBeNull();
        dbLanguage!.LanguageName.Should().Be("English");
    }

    [Fact]
    public async Task Handle_WhenLanguageDoesNotExist_ShouldThrowResourceNotFoundException()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var personalInfo = PersonalInfo.Create("Alice Smith", "alice@example.com");
        var resume = Resume.Create("Alice's Resume", ResumeTrack.FullStack, CareerLevel.Senior, "Dotnet Dev");
        var revision = resume.CreateFirstRevision("Dotnet Dev 1", personalInfo);
        var language = Language.Create(revision.Id, "English", LanguageProficiency.A1);

        context.Resumes.Add(resume);
        context.ResumeRevisions.Add(revision);
        context.Languages.Add(language);
        await context.SaveChangesAsync(CancellationToken.None);

        var nonExistentLanguageId = Guid.NewGuid();
        var dto = new UpdateLanguageDto
        {
            LanguageName = "  English   ",
            Proficiency = LanguageProficiency.A2
        };

        var command = new UpdateLanguageCommand(nonExistentLanguageId, dto);
        var handler = new UpdateLanguageCommandHandler(context);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ResourceNotFoundException>()
            .WithMessage($"Language with ID '{nonExistentLanguageId}' was not found.");
    }

    [Fact]
    public async Task Handle_WhenResumeRevisionIsUsedByApplication_ShouldThrowConflictException()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var personalInfo = PersonalInfo.Create("Alice Smith", "alice@example.com");
        var resume = Resume.Create("Alice's Resume", ResumeTrack.FullStack, CareerLevel.Senior, "Dotnet Dev");
        var revision = resume.CreateFirstRevision("Dotnet Dev", personalInfo);
        var language = Language.Create(revision.Id, "English", LanguageProficiency.A1);

        var company = Company.Create("Tech Corp", "https://techcorp.com");
        var vacancy = Vacancy.Create(company.Id, "Senior C# Developer", "https://techcorp.com/jobs/1");

        var application = Domain.Entities.Application.Create(company.Id, revision.Id, "https://dou.ua-vacancy-eot9te9", vacancy.Id);

        context.Resumes.Add(resume);
        context.ResumeRevisions.Add(revision);
        context.Languages.Add(language);
        context.Companies.Add(company);
        context.Vacancies.Add(vacancy);
        context.Applications.Add(application);
        await context.SaveChangesAsync(CancellationToken.None);

        var dto = new UpdateLanguageDto
        {
            LanguageName = "English",
            Proficiency = LanguageProficiency.A2
        };

        var command = new UpdateLanguageCommand(language.Id, dto);
        var handler = new UpdateLanguageCommandHandler(context);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("Language cannot be updated because its resume revision is already used in an application.");

        language.LanguageName.Should().Be("English");
        language.Proficiency.Should().Be(LanguageProficiency.A1);
    }
}
