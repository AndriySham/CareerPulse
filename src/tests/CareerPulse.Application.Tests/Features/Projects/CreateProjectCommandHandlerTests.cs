using CareerPulse.Application.DTOs.Projects;
using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Features.Projects.Commands.CreateProject;
using CareerPulse.Application.Tests.TestHelpers;
using CareerPulse.Domain.Entities;
using CareerPulse.Domain.Enums;
using CareerPulse.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CareerPulse.Application.Tests.Features.Projects;

public class CreateProjectCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidData_ShouldCreateProjectAndReturnDto()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var personalInfo = PersonalInfo.Create("Alice Smith", "alice@example.com");
        var resume = Resume.Create("Alice's Resume", ResumeTrack.FullStack, CareerLevel.Senior, "Dotnet Dev");
        var revision = resume.CreateFirstRevision("Dotnet Dev", personalInfo);

        context.Resumes.Add(resume);
        context.ResumeRevisions.Add(revision);
        await context.SaveChangesAsync(CancellationToken.None);

        var dto = new CreateProjectDto
        {
            ResumeRevisionId = revision.Id,
            Role = ".NET Developer",
            ProjectName = "Healthcare platform",
            Description = "Description Healthcare platform",
            TechStack = ".NET, EF Core",
            RepositoryUrl = "https://healthcare/repository.com",
            LiveDemoUrl = "https://helthcare/demo.com"
        };

        var command = new CreateProjectCommand(dto);
        var handler = new CreateProjectCommandHandler(context);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.ResumeRevisionId.Should().Be(revision.Id);
        result.Role.Should().Be(".NET Developer");
        result.ProjectName.Should().Be("Healthcare platform");
        result.Description.Should().Be("Description Healthcare platform");
        result.TechStack.Should().Be(".NET, EF Core");
        result.RepositoryUrl.Should().Be("https://healthcare/repository.com");
        result.LiveDemoUrl.Should().Be("https://helthcare/demo.com");

        var dbProject = await context.Projects.FirstOrDefaultAsync(x => x.Id == result.Id);
        dbProject.Should().NotBeNull();
        dbProject.Id.Should().Be(result.Id);
        dbProject.ResumeRevisionId.Should().Be(revision.Id);
        dbProject.Role.Should().Be(".NET Developer");
        dbProject.ProjectName.Should().Be("Healthcare platform");
        dbProject.Description.Should().Be("Description Healthcare platform");
        dbProject.TechStack.Should().Be(".NET, EF Core");
        dbProject.RepositoryUrl.Should().Be("https://healthcare/repository.com");
        dbProject.LiveDemoUrl.Should().Be("https://helthcare/demo.com");
    }

    [Fact]
    public async Task Handle_WithOnlyRequiredFields_ShouldCreateProjectWithNullOptionalField()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var personalInfo = PersonalInfo.Create("Alice Smith", "alice@example.com");
        var resume = Resume.Create("Alice's Resume", ResumeTrack.FullStack, CareerLevel.Senior, "Dotnet Dev");
        var revision = resume.CreateFirstRevision("Dotnet Dev", personalInfo);

        context.Resumes.Add(resume);
        context.ResumeRevisions.Add(revision);
        await context.SaveChangesAsync(CancellationToken.None);

        var dto = new CreateProjectDto
        {
            ResumeRevisionId = revision.Id,
            ProjectName = "Healthcare platform",
        };

        var command = new CreateProjectCommand(dto);
        var handler = new CreateProjectCommandHandler(context);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.ResumeRevisionId.Should().Be(revision.Id);
        result.Role.Should().BeNull();
        result.ProjectName.Should().Be("Healthcare platform");
        result.Description.Should().BeNull();
        result.TechStack.Should().BeNull();
        result.RepositoryUrl.Should().BeNull();
        result.LiveDemoUrl.Should().BeNull();

        var dbProject = await context.Projects.FirstOrDefaultAsync(x => x.Id == result.Id);
        dbProject.Should().NotBeNull();
        dbProject.Id.Should().Be(result.Id);
        dbProject.ResumeRevisionId.Should().Be(revision.Id);
        dbProject.Role.Should().BeNull();
        dbProject.ProjectName.Should().Be("Healthcare platform");
        dbProject.Description.Should().BeNull();
        dbProject.TechStack.Should().BeNull();
        dbProject.RepositoryUrl.Should().BeNull();
        dbProject.LiveDemoUrl.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WithWhiteSpaceAroundProjectName_ShouldTrimName()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var personalInfo = PersonalInfo.Create("Alice Smith", "alice@example.com");
        var resume = Resume.Create("Alice's Resume", ResumeTrack.FullStack, CareerLevel.Senior, "Dotnet Dev");
        var revision = resume.CreateFirstRevision("Dotnet Dev", personalInfo);

        context.Resumes.Add(resume);
        context.ResumeRevisions.Add(revision);
        await context.SaveChangesAsync(CancellationToken.None);

        var dto = new CreateProjectDto
        {
            ResumeRevisionId = revision.Id,
            ProjectName = "  Healthcare platform    ",
        };

        var command = new CreateProjectCommand(dto);
        var handler = new CreateProjectCommandHandler(context);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.ProjectName.Should().Be("Healthcare platform");

        var dbProject = await context.Projects.FirstOrDefaultAsync(x => x.Id == result.Id);
        dbProject.ProjectName.Should().Be("Healthcare platform");
    }

    [Fact]
    public async Task Handle_WhenResumeRevisionIdNotExist_ShouldThrowResourceNotFoundException()
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
        var dto = new CreateProjectDto
        {
            ResumeRevisionId = nonExistentRevisionId,
            ProjectName = "Healthcare platform"
        };

        var command = new CreateProjectCommand(dto);
        var handler = new CreateProjectCommandHandler(context);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ResourceNotFoundException>()
            .WithMessage($"ResumeRevision with ID '{nonExistentRevisionId}' was not found");
    }

    //Handle_WhenProjectNameIsWhitespace_ShouldThrowDomainException
}
