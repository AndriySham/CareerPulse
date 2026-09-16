using CareerPulse.Application.Features.Educations.Queries.GetEducationById;
using CareerPulse.Application.Tests.TestHelpers;
using CareerPulse.Domain.Entities;
using CareerPulse.Domain.Enums;
using CareerPulse.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace CareerPulse.Application.Tests.Features.Education;

public class GetEducationByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenEducationExists_ShouldReturnMappedEducationDto()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var personalInfo = PersonalInfo.Create("Alice Smith", "alice@example.com");
        var resume = Resume.Create("Alice's Resume", ResumeTrack.FullStack, CareerLevel.Senior, "Dotnet Dev");
        var revision = resume.CreateFirstRevision("Dotnet Dev", personalInfo);
        var education = Domain.Entities.Education.Create(revision.Id, "Dnipro University");

        context.Resumes.Add(resume);
        context.ResumeRevisions.Add(revision);
        context.Educations.Add(education);
        await context.SaveChangesAsync(CancellationToken.None);

        var query = new GetEducationByIdQuery(education.Id);
        var handler = new GetEducationByIdQueryHandler(context);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.InstitutionName.Should().Be("Dnipro University");
    }

    [Fact]
    public async Task Handle_WhenEducationDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var query = new GetEducationByIdQuery(Guid.NewGuid());
        var handler = new GetEducationByIdQueryHandler(context);

        // Act 
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}
