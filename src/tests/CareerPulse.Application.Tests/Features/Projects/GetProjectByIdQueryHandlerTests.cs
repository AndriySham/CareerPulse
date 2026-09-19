using CareerPulse.Application.Features.Projects.Queries.GetProjectById;
using CareerPulse.Application.Tests.TestHelpers;
using CareerPulse.Domain.Entities;
using CareerPulse.Domain.Enums;
using CareerPulse.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace CareerPulse.Application.Tests.Features.Projects;

public class GetProjectByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenProjectExists_ShouldReturnMappedProjectDto()
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

        var query = new GetProjectByIdQuery(project.Id);
        var handler = new GetProjectByIdQueryHandler(context);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.ResumeRevisionId.Should().Be(revision.Id);
        result.ProjectName.Should().Be("Healthcare platform");
        result.Description.Should().Be("Description Healthcare platform");
        result.Role.Should().Be(".NET Developer");
        result.RepositoryUrl.Should().Be("https://healthcare/repository.com");
        result.LiveDemoUrl.Should().Be("https://helthcare/demo.com");
        result.TechStack.Should().Be(".NET, EF Core");
    }

    [Fact]
    public async Task Handle_WhenProjectDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var query = new GetProjectByIdQuery(Guid.NewGuid());
        var handler = new GetProjectByIdQueryHandler(context);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}
