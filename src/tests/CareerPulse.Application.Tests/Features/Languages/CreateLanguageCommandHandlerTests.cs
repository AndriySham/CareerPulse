using CareerPulse.Application.DTOs.Languages;
using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Features.Languages.Commands.CreateLanguage;
using CareerPulse.Application.Tests.TestHelpers;
using CareerPulse.Domain.Entities;
using CareerPulse.Domain.Enums;
using CareerPulse.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CareerPulse.Application.Tests.Features.Languages;

public class CreateLanguageCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidData_ShouldCreateLanguageAndReturnDto()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var personalInfo = PersonalInfo.Create("Alice Smith", "alice@example.com");
        var resume = Resume.Create("Alice's Resume", ResumeTrack.FullStack, CareerLevel.Senior, "Dotnet Dev");
        var revision = resume.CreateFirstRevision("Dotnet Dev", personalInfo);

        context.Resumes.Add(resume);
        context.ResumeRevisions.Add(revision);
        await context.SaveChangesAsync(CancellationToken.None);

        var dto = new CreateLanguageDto
        {
            ResumeRevisionId = revision.Id,
            LanguageName = "English",
            Proficiency = LanguageProficiency.B2
        };

        var command = new CreateLanguageCommand(dto);
        var handler = new CreateLanguageCommandHandler(context);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.ResumeRevisionId.Should().Be(revision.Id);
        result.LanguageName.Should().Be("English");
        result.Proficiency.Should().Be(LanguageProficiency.B2);

        var dbLanguage = await context.Languages
            .FirstOrDefaultAsync(x => x.Id == result.Id);
        dbLanguage.Should().NotBeNull();
        dbLanguage.Id.Should().Be(result.Id);
        dbLanguage.ResumeRevisionId.Should().Be(revision.Id);
        dbLanguage.LanguageName.Should().Be("English");
        dbLanguage.Proficiency.Should().Be(LanguageProficiency.B2);
    }

    [Fact]
    public async Task Handle_WithWhitespaceAroundLanguageName_ShouldTrimName()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var personalInfo = PersonalInfo.Create("Alice Smith", "alice@example.com");
        var resume = Resume.Create("Alice's Resume", ResumeTrack.FullStack, CareerLevel.Senior, "Dotnet Dev");
        var revision = resume.CreateFirstRevision("Dotnet Dev", personalInfo);

        context.Resumes.Add(resume);
        context.ResumeRevisions.Add(revision);
        await context.SaveChangesAsync(CancellationToken.None);

        var dto = new CreateLanguageDto
        {
            ResumeRevisionId = revision.Id,
            LanguageName = "  English   ",
            Proficiency = LanguageProficiency.B2
        };

        var command = new CreateLanguageCommand(dto);
        var handler = new CreateLanguageCommandHandler(context);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.LanguageName.Should().Be("English");

        var dbLanguage = await context.Languages
            .FirstOrDefaultAsync(x => x.Id == result.Id);

        dbLanguage.Should().NotBeNull();
        dbLanguage.LanguageName.Should().Be("English");
    }

    [Fact]
    public async Task Handle_WhenResumeRevisionDoesNotExist_ShouldThrowResourсeNotFoundException()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var personalInfo = PersonalInfo.Create("Alice Smith", "alice@example.com");
        var resume = Resume.Create("Alice's Resume", ResumeTrack.FullStack, CareerLevel.Senior, "Dotnet Dev");
        var revision = resume.CreateFirstRevision("Dotnet Dev", personalInfo);

        context.Resumes.Add(resume);
        context.ResumeRevisions.Add(revision);
        await context.SaveChangesAsync(CancellationToken.None);

        var nonExistentRevisionId = Guid.NewGuid();
        var dto = new CreateLanguageDto
        {
            ResumeRevisionId = nonExistentRevisionId,
            LanguageName = "English",
            Proficiency = LanguageProficiency.B2
        };

        var command = new CreateLanguageCommand(dto);
        var handler = new CreateLanguageCommandHandler(context);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ResourceNotFoundException>()
            .WithMessage($"ResumeRevision with ID '{dto.ResumeRevisionId}' was not found.");
    }
}
