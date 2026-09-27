using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Features.Educations.Commands.DeleteEducation;
using CareerPulse.Application.Features.WorkExperience.Commands.DeleteWorkExperience;
using CareerPulse.Application.Tests.TestHelpers;
using CareerPulse.Domain.Entities;
using CareerPulse.Domain.Enums;
using CareerPulse.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CareerPulse.Application.Tests.Features.WorkExperience;

public class DeleteWorkExperienceCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenDataValid_ShouldDeleteWorkExperience()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var personalInfo = PersonalInfo.Create("Alice Smith", "alice@example.com");
        var resume = Resume.Create("Alice's Resume", ResumeTrack.FullStack, CareerLevel.Senior, "Dotnet Dev");
        var revision = resume.CreateFirstRevision("Dotnet Dev", personalInfo);
        var workExperience = Domain.Entities.WorkExperience.Create(revision.Id, "Epum", ".NET Developer", 1, 2025, 1, 2026, false, "Description", "Achievements", "C#");

        context.Resumes.Add(resume);
        context.ResumeRevisions.Add(revision);
        context.WorkExperiences.Add(workExperience);
        await context.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteWorkExperienceCommand(workExperience.Id);
        var handler = new DeleteWorkExperienceCommandHandler(context);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var dbWorkExperience = await context.WorkExperiences.AnyAsync(x => x.Id == workExperience.Id);
        dbWorkExperience.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WhenWorkExperienceDoesNotExist_ShouldThrowResourceNotFoundException()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var personalInfo = PersonalInfo.Create("Alice Smith", "alice@example.com");
        var resume = Resume.Create("Alice's Resume", ResumeTrack.FullStack, CareerLevel.Senior, "Dotnet Dev");
        var revision = resume.CreateFirstRevision("Dotnet Dev", personalInfo);

        context.Resumes.Add(resume);
        context.ResumeRevisions.Add(revision);
        await context.SaveChangesAsync(CancellationToken.None);

        var nonExistentWorkExperienceId = Guid.NewGuid();
        var command = new DeleteWorkExperienceCommand(nonExistentWorkExperienceId);
        var handler = new DeleteWorkExperienceCommandHandler(context);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ResourceNotFoundException>()
            .WithMessage($"WorkExperience with ID '{nonExistentWorkExperienceId}' was not found.");
    }

    [Fact]
    public async Task Handle_WhenResumeRevisionIsUsedByApplication_ShouldThrowConflictException()
    {
        using var context = TestDbContext.CreateInMemory();

        var personalInfo = PersonalInfo.Create("Alice Smith", "alice@example.com");
        var resume = Resume.Create("Alice's Resume", ResumeTrack.FullStack, CareerLevel.Senior, "Dotnet Dev");
        var revision = resume.CreateFirstRevision("Dotnet Dev", personalInfo);
        var workExperience = Domain.Entities.WorkExperience.Create(revision.Id, "Epum", ".NET Developer", 1, 2025, 1, 2026, false, "Description", "Achievements", "C#");

        var company = Company.Create("Tech Corp", "https://techcorp.com");
        var vacancy = Vacancy.Create(company.Id, "Senior C# Developer", "https://techcorp.com/jobs/1");

        var application = Domain.Entities.Application.Create(company.Id, revision.Id, "https://dou.ua-vacancy-eot9te9", vacancy.Id);

        context.Resumes.Add(resume);
        context.ResumeRevisions.Add(revision);
        context.WorkExperiences.Add(workExperience);
        context.Companies.Add(company);
        context.Vacancies.Add(vacancy);
        context.Applications.Add(application);
        await context.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteWorkExperienceCommand(workExperience.Id);
        var handler = new DeleteWorkExperienceCommandHandler(context);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("WorkExperience cannot be deleted because its resume revision is already used in an application.");

        var dbWorkExperience = await context.WorkExperiences
            .FirstOrDefaultAsync(x => x.Id == workExperience.Id, CancellationToken.None);

        dbWorkExperience.Should().NotBeNull();
        dbWorkExperience!.Id.Should().Be(workExperience.Id);
        dbWorkExperience.CompanyName.Should().Be("Epum");
        dbWorkExperience.PositionTitle.Should().Be(".NET Developer");
    }

    [Fact]
    public async Task Handle_WhenResumeRevisionHasMultipleWorkExperiencesButIsNotUsedByApplication_ShouldDeleteWorkExperience()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var personalInfo = PersonalInfo.Create("Alice Smith", "alice@example.com");
        var resume = Resume.Create("Alice's Resume", ResumeTrack.FullStack, CareerLevel.Senior, "Dotnet Dev");
        var revision = resume.CreateFirstRevision("Dotnet Dev", personalInfo);
        var workExperienceToDelete = Domain.Entities.WorkExperience.Create(revision.Id, "Epum", ".NET Developer", 1, 2025, 1, 2026, false, "Description", "Achievements", "C#");
        var anotherWorkExperience = Domain.Entities.WorkExperience.Create(revision.Id, "Another Company", "Senior .NET Developer", 2, 2023, 12, 2024, false, "Another description", "Another achievements", "C#");

        context.Resumes.Add(resume);
        context.ResumeRevisions.Add(revision);
        context.WorkExperiences.AddRange(workExperienceToDelete, anotherWorkExperience);
        await context.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteWorkExperienceCommand(workExperienceToDelete.Id);
        var handler = new DeleteWorkExperienceCommandHandler(context);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedWorkExperienceExists = await context.WorkExperiences
            .AnyAsync(x => x.Id == workExperienceToDelete.Id);

        deletedWorkExperienceExists.Should().BeFalse();

        var anotherWorkExperienceExists = await context.WorkExperiences
            .AnyAsync(x => x.Id == anotherWorkExperience.Id);

        anotherWorkExperienceExists.Should().BeTrue();
    }
}
