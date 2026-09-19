using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Features.Projects.Commands.DeleteProject;
using CareerPulse.Application.Tests.TestHelpers;
using CareerPulse.Domain.Entities;
using CareerPulse.Domain.Enums;
using CareerPulse.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CareerPulse.Application.Tests.Features.Projects;

public class DeleteProjectCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenDataValid_ShouldDeleteProject()
    {
        // Arrange 
        using var context = TestDbContext.CreateInMemory();

        var personalInfo = PersonalInfo.Create("Alice Smith", "alice@example.com");
        var resume = Resume.Create("Alice's Resume", ResumeTrack.FullStack, CareerLevel.Senior, "Dotnet Dev");
        var revision = resume.CreateFirstRevision("Dotnet Dev", personalInfo);
        var project = Project.Create(revision.Id, "Healthcare platform", "Description Healthcare platform",
            ".NET Developer", "https://healthcare/repository.com", "https://helthcare/demo.com", ".NET, EF Core");

        context.Resumes.Add(resume);
        context.ResumeRevisions.Add(revision);
        context.Projects.Add(project);
        await context.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteProjectCommand(project.Id);
        var handler = new DeleteProjectCommandHandler(context);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var dbProject = await context.Projects.AnyAsync(x => x.Id == project.Id, CancellationToken.None);
        dbProject.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WhenProjectDoesNotExist_ShouldThrowResourсeNotFoundException()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var personalInfo = PersonalInfo.Create("Alice Smith", "alice@example.com");
        var resume = Resume.Create("Alice's Resume", ResumeTrack.FullStack, CareerLevel.Senior, "Dotnet Dev");
        var revision = resume.CreateFirstRevision("Dotnet Dev", personalInfo);
        var project = Project.Create(revision.Id, "Healthcare platform", "Description Healthcare platform",
            ".NET Developer", "https://healthcare/repository.com", "https://helthcare/demo.com", ".NET, EF Core");

        context.Resumes.Add(resume);
        context.ResumeRevisions.Add(revision);
        context.Projects.Add(project);
        await context.SaveChangesAsync(CancellationToken.None);

        var nonExistentProject = Guid.NewGuid();
        var command = new DeleteProjectCommand(nonExistentProject);
        var handler = new DeleteProjectCommandHandler(context);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ResourceNotFoundException>()
            .WithMessage($"Project with ID '{nonExistentProject}' was not found.");
    }

    [Fact]
    public async Task Handle_WhenResumeRevisionIsUsedByApplication_ShouldThrowConflictException()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var personalInfo = PersonalInfo.Create("Alice Smith", "alice@example.com");
        var resume = Resume.Create("Alice's Resume", ResumeTrack.FullStack, CareerLevel.Senior, "Dotnet Dev");
        var revision = resume.CreateFirstRevision("Dotnet Dev", personalInfo);
        var project = Project.Create(revision.Id, "Healthcare platform", "Description Healthcare platform",
            ".NET Developer", "https://healthcare/repository.com", "https://helthcare/demo.com", ".NET, EF Core");

        var company = Company.Create("Tech Corp", "https://techcorp.com");
        var vacancy = Vacancy.Create(company.Id, "Senior C# Developer", "https://techcorp.com/jobs/1");

        var application = Domain.Entities.Application.Create(company.Id, revision.Id, "https://dou.ua-vacancy-eot9te9", vacancy.Id);

        context.Resumes.Add(resume);
        context.ResumeRevisions.Add(revision);
        context.Projects.Add(project);
        context.Companies.Add(company);
        context.Vacancies.Add(vacancy);
        context.Applications.Add(application);
        await context.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteProjectCommand(project.Id);
        var handler = new DeleteProjectCommandHandler(context);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("Project cannot be deleted because its resume revision is already used in an application.");

        var projectExists = await context.Projects
            .AnyAsync(x => x.Id == project.Id, CancellationToken.None);
        projectExists.Should().BeTrue();
    }
}
