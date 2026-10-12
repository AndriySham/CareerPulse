using CareerPulse.Application.Features.Interviews.Queries.GetInterviewById;
using CareerPulse.Application.Tests.TestHelpers;
using CareerPulse.Domain.Entities;
using CareerPulse.Domain.Enums;
using CareerPulse.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

using AppEntity = CareerPulse.Domain.Entities.Application;

namespace CareerPulse.Application.Tests.Features.Interviews;

public class GetInterviewByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenInterviewExists_ShouldReturnMappedInterviewDto()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var company = Company.Create("QueryCorp");
        var vacancy = Vacancy.Create(company.Id, "Backend Engineer");
        var personalInfo = PersonalInfo.Create("Bob Martin", "bob@example.com");
        var resume = Resume.Create("Bob's Resume", ResumeTrack.Backend, CareerLevel.Senior, "Clean Coder");
        var revision = resume.CreateFirstRevision("Clean Coder", personalInfo);
        var app = AppEntity.Create(company.Id, revision.Id, "GitHub Jobs", vacancy.Id);
        var scheduledAt = DateTime.UtcNow;
        var interview = Interview.Create(app.Id, InterviewType.HRInterview, scheduledAt, null, "Interview notes", "Interview feedback");

        context.Companies.Add(company);
        context.Vacancies.Add(vacancy);
        context.Resumes.Add(resume);
        context.ResumeRevisions.Add(revision);
        context.Applications.Add(app);
        context.Interviews.Add(interview);
        await context.SaveChangesAsync();

        var handler = new GetInterviewByIdQueryHandler(context);
        var query = new GetInterviewByIdQuery(app.Id, interview.Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(interview.Id);
        result.Type.Should().Be(InterviewType.HRInterview);
        result.ScheduledAt.Should().Be(scheduledAt);
        result.ConductedAt.Should().BeNull();
        result.Notes.Should().Be("Interview notes");
        result.Feedback.Should().Be("Interview feedback");
    }

    [Fact]
    public async Task Handle_WhenInterviewDoesNotExist_ShouldReturnNull()
    {
        // Arange
        using var context = TestDbContext.CreateInMemory();

        var handler = new GetInterviewByIdQueryHandler(context);
        var query = new GetInterviewByIdQuery(Guid.NewGuid(), Guid.NewGuid());

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WhenInterviewBelongsToAnotherApplication_ShouldReturnNull()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var company = Company.Create("QueryCorp");
        var vacancy = Vacancy.Create(company.Id, "Backend Engineer");
        var personalInfo = PersonalInfo.Create("Bob Martin", "bob@example.com");
        var resume = Resume.Create("Bob's Resume", ResumeTrack.Backend, CareerLevel.Senior, "Clean Coder");
        var revision = resume.CreateFirstRevision("Clean Coder", personalInfo);
        var application = AppEntity.Create(company.Id, revision.Id, "LinkedIn", vacancy.Id);
        var anotherApplication = AppEntity.Create(company.Id, revision.Id, "Djinni", vacancy.Id);
        var interview = Interview.Create(application.Id, InterviewType.HRInterview, DateTime.UtcNow, null, "Interview notes", "Interview feedback");

        context.Companies.Add(company);
        context.Vacancies.Add(vacancy);
        context.Resumes.Add(resume);
        context.ResumeRevisions.Add(revision);
        context.Applications.AddRange(application, anotherApplication);
        context.Interviews.Add(interview);

        await context.SaveChangesAsync();

        var handler = new GetInterviewByIdQueryHandler(context);
        var query = new GetInterviewByIdQuery(anotherApplication.Id, interview.Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}
