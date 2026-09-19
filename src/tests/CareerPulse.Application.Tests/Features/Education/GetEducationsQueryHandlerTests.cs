using CareerPulse.Application.Features.Educations.Queries.GetEducations;
using CareerPulse.Application.Tests.TestHelpers;
using CareerPulse.Domain.Entities;
using CareerPulse.Domain.Enums;
using CareerPulse.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace CareerPulse.Application.Tests.Features.Education;

public class GetEducationsQueryHandlerTests
{
    [Fact]
    public async Task Handle_GetEducationForResumeRevisionId_ShouldReturnAllEducation()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var personalInfo = PersonalInfo.Create("Alice Smith", "alice@example.com");
        var resume = Resume.Create("Alice's Resume", ResumeTrack.FullStack, CareerLevel.Senior, "Dotnet Dev");
        var revision = resume.CreateFirstRevision("Dotnet Dev", personalInfo);
        var education1 = Domain.Entities.Education.Create(revision.Id, "Dnipro University Bachelor", "Bachelor", 2015, 2019);
        var education2 = Domain.Entities.Education.Create(revision.Id, "Dnipro University Magister", "Magister", 2019, 2020);
        
        context.Resumes.Add(resume);
        context.ResumeRevisions.Add(revision);
        context.Educations.Add(education1);
        context.Educations.Add(education2);
        await context.SaveChangesAsync(CancellationToken.None);

        var query = new GetEducationsQuery(revision.Id);
        var handler = new GetEducationsQueryHandler(context);

        // Act 
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result[0].InstitutionName.Should().Be("Dnipro University Magister");
        result[0].Description.Should().Be("Magister");
        result[0].EndYear.Should().Be(2020);
        result[0].StartYear.Should().Be(2019);
        result[1].InstitutionName.Should().Be("Dnipro University Bachelor");
        result[1].Description.Should().Be("Bachelor");
        result[1].EndYear.Should().Be(2019);
        result[1].StartYear.Should().Be(2015);
    }

    [Fact]
    public async Task Hadnle_WhenEducationDoesNotExcist_ShouldReturnEmptyList()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var personalInfo = PersonalInfo.Create("Alice Smith", "alice@example.com");
        var resume = Resume.Create("Alice's Resume", ResumeTrack.FullStack, CareerLevel.Senior, "Dotnet Dev");
        var revision = resume.CreateFirstRevision("Dotnet Dev", personalInfo);

        context.Resumes.Add(resume);
        context.ResumeRevisions.Add(revision);
        await context.SaveChangesAsync(CancellationToken.None);

        var query = new GetEducationsQuery(revision.Id);
        var handler = new GetEducationsQueryHandler(context);

        // Act 
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }
}
