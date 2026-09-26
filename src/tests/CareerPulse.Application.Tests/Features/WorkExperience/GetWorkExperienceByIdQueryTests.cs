using CareerPulse.Application.Features.Languages.Queries.GetLanguageById;
using CareerPulse.Application.Features.WorkExperience.Queries.GetWorkExperienceById;
using CareerPulse.Application.Tests.TestHelpers;
using CareerPulse.Domain.Entities;
using CareerPulse.Domain.Enums;
using CareerPulse.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace CareerPulse.Application.Tests.Features.WorkExperience;

public class GetWorkExperienceByIdQueryTests
{
    [Fact]
    public async Task Handle_WhenWorkExperienceExists_ShouldReturnMappedWorkExperienceDto()
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

        var query = new GetWorkExperienceByIdQuery(workExperience.Id);
        var handler = new GetWorkExperienceByIdQueryHandler(context);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(workExperience.Id);
        result.ResumeRevisionId.Should().Be(workExperience.ResumeRevisionId);
        result.CompanyName.Should().Be("Epum");
        result.PositionTitle.Should().Be(".NET Developer");
        result.StartMonth.Should().Be(1);
        result.StartYear.Should().Be(2025);
        result.EndMonth.Should().Be(1);
        result.EndYear.Should().Be(2026);
        result.IsCurrentJob.Should().BeFalse();
        result.Description.Should().Be("Description");
        result.Achievements.Should().Be("Achievements");
        result.TechStack.Should().Be("C#");
    }

    [Fact]
    public async Task Handle_WhenWorkExperienceDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var query = new GetWorkExperienceByIdQuery(Guid.NewGuid());
        var handler = new GetWorkExperienceByIdQueryHandler(context);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}

