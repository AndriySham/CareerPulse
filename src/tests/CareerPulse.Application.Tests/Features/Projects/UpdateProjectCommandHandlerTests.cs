using CareerPulse.Application.DTOs.Projects;
using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Features.Projects;
using CareerPulse.Application.Features.Projects.Commands.UpdateProject;
using CareerPulse.Application.Tests.TestHelpers;
using CareerPulse.Domain.Entities;
using CareerPulse.Domain.Enums;
using CareerPulse.Domain.Exceptions;
using CareerPulse.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CareerPulse.Application.Tests.Features.Projects;

public class UpdateProjectCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidData_ShouldUpdateProductAndReturnDto()
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

        var dto = new UpdateProjectDto
        {
            Role = ".NET Developer updated",
            ProjectName = "Healthcare platform updated",
            Description = "Description Healthcare platform updated",
            TechStack = ".NET, EF Core updated",
            RepositoryUrl = "https://healthcare/repository-updated.com",
            LiveDemoUrl = "https://helthcare/demo-updated.com"
        };

        var command = new UpdateProjectCommand(project.Id, dto);
        var handler = new UpdateProjectCommandHandler(context);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.ResumeRevisionId.Should().Be(revision.Id);
        result.ProjectName.Should().Be("Healthcare platform updated");
        result.Role.Should().Be(".NET Developer updated");
        result.Description.Should().Be("Description Healthcare platform updated");
        result.TechStack.Should().Be(".NET, EF Core updated");
        result.RepositoryUrl.Should().Be("https://healthcare/repository-updated.com");
        result.LiveDemoUrl.Should().Be("https://helthcare/demo-updated.com");

        var dbProject = await context.Projects
            .FirstOrDefaultAsync(x => x.Id == result.Id, CancellationToken.None);
        dbProject.Should().NotBeNull();
        dbProject.Id.Should().Be(result.Id);
        dbProject.ResumeRevisionId.Should().Be(revision.Id);
        dbProject.ProjectName.Should().Be("Healthcare platform updated");
        dbProject.Role.Should().Be(".NET Developer updated");
        dbProject.Description.Should().Be("Description Healthcare platform updated");
        dbProject.TechStack.Should().Be(".NET, EF Core updated");
        dbProject.RepositoryUrl.Should().Be("https://healthcare/repository-updated.com");
        dbProject.LiveDemoUrl.Should().Be("https://helthcare/demo-updated.com");
    }

    [Fact]
    public async Task Handle_WithOnlyRequiredFields_ShouldUpdateProjectWithNullOptionalFields()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var personalInfo = PersonalInfo.Create("Alice Smith", "alice@example.com");
        var resume = Resume.Create("Alice's Resume", ResumeTrack.FullStack, CareerLevel.Senior, "Dotnet Dev");
        var revision = resume.CreateFirstRevision("Dotnet Dev", personalInfo);
        var project = Project.Create(revision.Id, "Healthcare platform");

        context.Resumes.Add(resume);
        context.ResumeRevisions.Add(revision);
        context.Projects.Add(project);
        await context.SaveChangesAsync(CancellationToken.None);

        var dto = new UpdateProjectDto
        {
            ProjectName = "Healthcare platform updated",
        };

        var command = new UpdateProjectCommand(project.Id, dto);
        var handler = new UpdateProjectCommandHandler(context);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.ResumeRevisionId.Should().Be(revision.Id);
        result.ProjectName.Should().Be("Healthcare platform updated");
        result.Role.Should().BeNull();
        result.Description.Should().BeNull();
        result.TechStack.Should().BeNull();
        result.RepositoryUrl.Should().BeNull();
        result.LiveDemoUrl.Should().BeNull();

        var dbProject = await context.Projects
            .FirstOrDefaultAsync(x => x.Id == result.Id, CancellationToken.None);
        dbProject.Should().NotBeNull();
        dbProject.Id.Should().Be(result.Id);
        dbProject.ResumeRevisionId.Should().Be(revision.Id);
        dbProject.ProjectName.Should().Be("Healthcare platform updated");
        dbProject.Role.Should().BeNull();
        dbProject.Description.Should().BeNull();
        dbProject.TechStack.Should().BeNull();
        dbProject.RepositoryUrl.Should().BeNull();
        dbProject.LiveDemoUrl.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WithWhitespaceAroundProperties_ShouldTrimProperties()
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

        var dto = new UpdateProjectDto
        {
            Role = " .NET Developer updated ",
            ProjectName = "  Healthcare platform updated     ",
            Description = "  Description Healthcare platform updated        ",
            TechStack = "    .NET, EF Core updated ",
            RepositoryUrl = " https://healthcare/repository-updated.com ",
            LiveDemoUrl = " https://helthcare/demo-updated.com "
        };

        var command = new UpdateProjectCommand(project.Id, dto);
        var handler = new UpdateProjectCommandHandler(context);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.ResumeRevisionId.Should().Be(revision.Id);
        result.ProjectName.Should().Be("Healthcare platform updated");
        result.Role.Should().Be(".NET Developer updated");
        result.Description.Should().Be("Description Healthcare platform updated");
        result.TechStack.Should().Be(".NET, EF Core updated");
        result.RepositoryUrl.Should().Be("https://healthcare/repository-updated.com");
        result.LiveDemoUrl.Should().Be("https://helthcare/demo-updated.com");

        var dbProject = await context.Projects
            .FirstOrDefaultAsync(x => x.Id == result.Id, CancellationToken.None);
        dbProject.Should().NotBeNull();
        dbProject.Id.Should().Be(result.Id);
        dbProject.ResumeRevisionId.Should().Be(revision.Id);
        dbProject.ProjectName.Should().Be("Healthcare platform updated");
        dbProject.Role.Should().Be(".NET Developer updated");
        dbProject.Description.Should().Be("Description Healthcare platform updated");
        dbProject.TechStack.Should().Be(".NET, EF Core updated");
        dbProject.RepositoryUrl.Should().Be("https://healthcare/repository-updated.com");
        dbProject.LiveDemoUrl.Should().Be("https://helthcare/demo-updated.com");
    }

    [Fact]
    public async Task Handle_WhenProjectDoesNotExist_ShouldThrowResourceNotFoundException()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var personalInfo = PersonalInfo.Create("Alice Smith", "alice@example.com");
        var resume = Resume.Create("Alice's Resume", ResumeTrack.FullStack, CareerLevel.Senior, "Dotnet Dev");
        var revision = resume.CreateFirstRevision("Dotnet Dev 1", personalInfo);
        var project = Project.Create(revision.Id, "Healthcare platform", "Description Healthcare platform",
                ".NET Developer", "https://healthcare/repository.com", "https://helthcare/demo.com", ".NET, EF Core");

        context.Resumes.Add(resume);
        context.ResumeRevisions.Add(revision);
        context.Projects.Add(project);
        await context.SaveChangesAsync(CancellationToken.None);

        var nonExistentProjectId = Guid.NewGuid();
        var dto = new UpdateProjectDto
        {
            Role = ".NET Developer updated",
            ProjectName = "Healthcare platform updated",
            Description = "Description Healthcare platform updated",
            TechStack = ".NET, EF Core updated",
            RepositoryUrl = "https://healthcare/repository-updated.com",
            LiveDemoUrl = "https://helthcare/demo-updated.com"
        };

        var command = new UpdateProjectCommand(nonExistentProjectId, dto);
        var handler = new UpdateProjectCommandHandler(context);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ResourceNotFoundException>()
            .WithMessage($"Project with ID '{nonExistentProjectId}' was not found.");
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

        var dto = new UpdateProjectDto
        {
            Role = ".NET Developer updated",
            ProjectName = "Healthcare platform updated",
            Description = "Description Healthcare platform updated",
            TechStack = ".NET, EF Core updated",
            RepositoryUrl = "https://healthcare/repository-updated.com",
            LiveDemoUrl = "https://helthcare/demo-updated.com"
        };

        var command = new UpdateProjectCommand(project.Id, dto);
        var handler = new UpdateProjectCommandHandler(context);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("Project cannot be updated because its resume revision is already used in an application.");
    }

    [Fact]
    public async Task Handle_WhenProjectNameIsWhitespace_ShouldThrowDomainException()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var personalInfo = PersonalInfo.Create("Alice Smith", "alice@example.com");
        var resume = Resume.Create("Alice's Resume", ResumeTrack.FullStack, CareerLevel.Senior, "Dotnet Dev");
        var revision = resume.CreateFirstRevision("Dotnet Dev", personalInfo);
        var project = Project.Create(revision.Id, "Healthcare platform");

        context.Resumes.Add(resume);
        context.ResumeRevisions.Add(revision);
        context.Projects.Add(project);
        await context.SaveChangesAsync(CancellationToken.None);

        var dto = new UpdateProjectDto
        {
            ProjectName = "     "
        };

        var command = new UpdateProjectCommand(project.Id, dto);
        var handler = new UpdateProjectCommandHandler(context);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("ProjectName is required.");
    }
}
