using CareerPulse.Application.Features.WorkExperience.Queries.GetWorkExperiences;
using CareerPulse.Application.Tests.TestHelpers;
using CareerPulse.Domain.Entities;
using CareerPulse.Domain.Enums;
using CareerPulse.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace CareerPulse.Application.Tests.Features.WorkExperience;

public class GetWorkExperiencesQueryHandlerTests
{
    [Fact]
    public async Task Handle_GetWorkExperiencesForResumeRevisionId_ShouldReturnAllWorkExperiences()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var personalInfo = PersonalInfo.Create("Alice Smith", "alice@example.com");
        var resume = Resume.Create("Alice's Resume", ResumeTrack.FullStack, CareerLevel.Senior, "Dotnet Dev");
        var revision = resume.CreateFirstRevision("Dotnet Dev", personalInfo);
        var workExperience1 = Domain.Entities.WorkExperience.Create(revision.Id, "Epum", ".NET Developer", 1, 2024, 1, 2025, false, "Description1", "Achievements1", "C#");
        var workExperience2 = Domain.Entities.WorkExperience.Create(revision.Id, "SoftServe", ".NET Developer2", 1, 2025, 1, 2026, false, "Description2", "Achievements2", "TypeScript");

        context.Resumes.Add(resume);
        context.ResumeRevisions.Add(revision);
        context.WorkExperiences.Add(workExperience1);
        context.WorkExperiences.Add(workExperience2);
        await context.SaveChangesAsync(CancellationToken.None);

        var query = new GetWorkExperiencesQuery(revision.Id);
        var handler = new GetWorkExperiencesQueryHandler(context);

        // Act 
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result[0].Id.Should().Be(workExperience1.Id);
        result[0].ResumeRevisionId.Should().Be(workExperience1.ResumeRevisionId);
        result[0].CompanyName.Should().Be("Epum");
        result[0].PositionTitle.Should().Be(".NET Developer");
        result[0].StartMonth.Should().Be(1);
        result[0].StartYear.Should().Be(2024);
        result[0].EndMonth.Should().Be(1);
        result[0].EndYear.Should().Be(2025);
        result[0].IsCurrentJob.Should().BeFalse();
        result[0].Description.Should().Be("Description1");
        result[0].Achievements.Should().Be("Achievements1");
        result[0].TechStack.Should().Be("C#");
        result[1].Id.Should().Be(workExperience2.Id);
        result[1].ResumeRevisionId.Should().Be(workExperience2.ResumeRevisionId);
        result[1].CompanyName.Should().Be("SoftServe");
        result[1].PositionTitle.Should().Be(".NET Developer2");
        result[1].StartMonth.Should().Be(1);
        result[1].StartYear.Should().Be(2025);
        result[1].EndMonth.Should().Be(1);
        result[1].EndYear.Should().Be(2026);
        result[1].IsCurrentJob.Should().BeFalse();
        result[1].Description.Should().Be("Description2");
        result[1].Achievements.Should().Be("Achievements2");
        result[1].TechStack.Should().Be("TypeScript");
    }

    [Fact]
    public async Task Hadnle_WhenWorkExperienceDoesNotExist_ShouldReturnEmptyList()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var personalInfo = PersonalInfo.Create("Alice Smith", "alice@example.com");
        var resume = Resume.Create("Alice's Resume", ResumeTrack.FullStack, CareerLevel.Senior, "Dotnet Dev");
        var revision = resume.CreateFirstRevision("Dotnet Dev", personalInfo);

        context.Resumes.Add(resume);
        context.ResumeRevisions.Add(revision);
        await context.SaveChangesAsync(CancellationToken.None);

        var query = new GetWorkExperiencesQuery(revision.Id);
        var handler = new GetWorkExperiencesQueryHandler(context);

        // Act 
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }
}
