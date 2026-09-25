using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Features.Languages.Commands.DeleteLanguage;
using CareerPulse.Application.Tests.TestHelpers;
using CareerPulse.Domain.Entities;
using CareerPulse.Domain.Enums;
using CareerPulse.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CareerPulse.Application.Tests.Features.Languages;

public class DeleteLanguageCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenDataValid_ShouldDeleteLanguage()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var personalInfo = PersonalInfo.Create("Alice Smith", "alice@example.com");
        var resume = Resume.Create("Alice's Resume", ResumeTrack.FullStack, CareerLevel.Senior, "Dotnet Dev");
        var revision = resume.CreateFirstRevision("Dotnet Dev", personalInfo);
        var language = Language.Create(revision.Id, "Polish", LanguageProficiency.A1);

        context.Resumes.Add(resume);
        context.ResumeRevisions.Add(revision);
        context.Languages.Add(language);
        await context.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteLanguageCommand(language.Id);
        var handler = new DeleteLanguageCommandHandler(context);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var dbLanguage = await context.Languages.AnyAsync(x => x.Id == language.Id);
        dbLanguage.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WhenLanguageDoesNotExist_ShouldThrowResourceNotFoundException()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var personalInfo = PersonalInfo.Create("Alice Smith", "alice@example.com");
        var resume = Resume.Create("Alice's Resume", ResumeTrack.FullStack, CareerLevel.Senior, "Dotnet Dev");
        var revision = resume.CreateFirstRevision("Dotnet Dev", personalInfo);

        context.Resumes.Add(resume);
        context.ResumeRevisions.Add(revision);
        await context.SaveChangesAsync(CancellationToken.None);

        var nonExistentLanguageId = Guid.NewGuid();
        var command = new DeleteLanguageCommand(nonExistentLanguageId);
        var handler = new DeleteLanguageCommandHandler(context);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ResourceNotFoundException>()
            .WithMessage($"Language with ID '{nonExistentLanguageId}' was not found.");
    }

    [Fact]
    public async Task Handle_WhenResumeRevisionIsUsedByApplication_ShouldThrowConflictException()
    {
        using var context = TestDbContext.CreateInMemory();

        var personalInfo = PersonalInfo.Create("Alice Smith", "alice@example.com");
        var resume = Resume.Create("Alice's Resume", ResumeTrack.FullStack, CareerLevel.Senior, "Dotnet Dev");
        var revision = resume.CreateFirstRevision("Dotnet Dev", personalInfo);
        var language = Language.Create(revision.Id, "Polish", LanguageProficiency.A1);

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

        var command = new DeleteLanguageCommand(language.Id);
        var handler = new DeleteLanguageCommandHandler(context);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("Language cannot be deleted because its resume revision is already used in an application.");
        
        var languageExists = await context.Languages.AnyAsync(x => x.Id == language.Id);
        languageExists.Should().BeTrue();
    }
}
