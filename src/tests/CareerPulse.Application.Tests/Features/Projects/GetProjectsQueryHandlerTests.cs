using CareerPulse.Application.Features.Projects.Queries.GetProjects;
using CareerPulse.Application.Tests.TestHelpers;
using CareerPulse.Domain.Entities;
using CareerPulse.Domain.Enums;
using CareerPulse.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace CareerPulse.Application.Tests.Features.Projects;

public class GetProjectsQueryHandlerTests
{
    [Fact]
    public async Task Handle_GetProjectFromResumeRevisionId_ShouldReturnAllProjects()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var personalInfo = PersonalInfo.Create("Alice Smith", "alice@example.com");
        var resume = Resume.Create("Alice's Resume", ResumeTrack.FullStack, CareerLevel.Senior, "Dotnet Dev");
        var revision = resume.CreateFirstRevision("Dotnet Dev", personalInfo);
        var project = Project.Create(revision.Id, "Healthcare platform", "Description Healthcare platform",
            ".NET Developer", "https://healthcare/repository.com", "https://helthcare/demo.com", ".NET, EF Core");
        var project2 = Project.Create(revision.Id, "Healthcare platform 2", "Description Healthcare platform 2",
            ".NET Developer 2", "https://healthcare/repository2.com", "https://helthcare/demo2.com", ".NET, EF Core 2");

        context.Resumes.Add(resume);
        context.ResumeRevisions.Add(revision);
        context.Projects.AddRange(project, project2);
        await context.SaveChangesAsync(CancellationToken.None);

        var query = new GetProjectsQuery(revision.Id);
        var handler = new GetProjectsQueryHandler(context);

        // Act 
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result[0].ResumeRevisionId.Should().Be(revision.Id);
        result[0].ProjectName.Should().Be("Healthcare platform");
        result[0].Description.Should().Be("Description Healthcare platform");
        result[0].Role.Should().Be(".NET Developer");
        result[0].RepositoryUrl.Should().Be("https://healthcare/repository.com");
        result[0].LiveDemoUrl.Should().Be("https://helthcare/demo.com");
        result[0].TechStack.Should().Be(".NET, EF Core");
        result[1].ResumeRevisionId.Should().Be(revision.Id);
        result[1].ProjectName.Should().Be("Healthcare platform 2");
        result[1].Description.Should().Be("Description Healthcare platform 2");
        result[1].Role.Should().Be(".NET Developer 2");
        result[1].RepositoryUrl.Should().Be("https://healthcare/repository2.com");
        result[1].LiveDemoUrl.Should().Be("https://helthcare/demo2.com");
        result[1].TechStack.Should().Be(".NET, EF Core 2");
    }

    [Fact]
    public async Task Handle_WhenProjectsDoNotExist_ShouldReturnEmptyList()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var personalInfo = PersonalInfo.Create("Alice Smith", "alice@example.com");
        var resume = Resume.Create("Alice's Resume", ResumeTrack.FullStack, CareerLevel.Senior, "Dotnet Dev");
        var revision = resume.CreateFirstRevision("Dotnet Dev", personalInfo);

        context.Resumes.Add(resume);
        context.ResumeRevisions.Add(revision);
        await context.SaveChangesAsync(CancellationToken.None);

        var query = new GetProjectsQuery(revision.Id);
        var handler = new GetProjectsQueryHandler(context);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }
}
