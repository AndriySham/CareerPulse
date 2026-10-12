using CareerPulse.Application.DTOs.Interviews;
using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Features.Interviews.Commands.UpdateInterview;
using CareerPulse.Application.Tests.TestHelpers;
using CareerPulse.Domain.Entities;
using CareerPulse.Domain.Enums;
using CareerPulse.Domain.Exceptions;
using CareerPulse.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

using AppEntity = CareerPulse.Domain.Entities.Application;

namespace CareerPulse.Application.Tests.Features.Interviews;

public class UpdateInterviewCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidData_ShouldUpdateInterviewAndReturnDto()
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

        context.ChangeTracker.Clear();

        var newScheduledAt = DateTime.UtcNow.AddDays(-1);
        var conductedAt = DateTime.UtcNow;

        var dto = new UpdateInterviewDto
        {
            Type = InterviewType.TechnicalInterview,
            ScheduledAt = newScheduledAt,
            ConductedAt = conductedAt,
            Notes = " Prepare system design ",
            Feedback = "Strong candidate"
        };

        var handler = new UpdateInterviewCommandHandler(context);
        var command = new UpdateInterviewCommand(app.Id, interview.Id, dto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(interview.Id);
        result.Type.Should().Be(InterviewType.TechnicalInterview);
        result.ScheduledAt.Should().Be(newScheduledAt);
        result.ConductedAt.Should().Be(conductedAt);
        result.Notes.Should().Be("Prepare system design");
        result.Feedback.Should().Be("Strong candidate");

        context.ChangeTracker.Clear();

        var dbInterview = await context.Interviews
            .SingleAsync(x => x.Id == interview.Id && x.ApplicationId == app.Id, CancellationToken.None);

        dbInterview.Id.Should().Be(result.Id);
        dbInterview.Type.Should().Be(InterviewType.TechnicalInterview);
        dbInterview.ScheduledAt.Should().Be(newScheduledAt);
        dbInterview.ConductedAt.Should().Be(conductedAt);
        dbInterview.Notes.Should().Be("Prepare system design");
        dbInterview.Feedback.Should().Be("Strong candidate");
        dbInterview.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WithWhitespaceAroundNotesAndFeedback_ShouldTrimNotesAndFeedback()
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

        context.ChangeTracker.Clear();

        var newScheduledAt = DateTime.UtcNow.AddDays(-1);
        var conductedAt = DateTime.UtcNow;

        var dto = new UpdateInterviewDto
        {
            Type = InterviewType.HRInterview,
            ScheduledAt = newScheduledAt,
            ConductedAt = conductedAt,
            Notes = "    Prepare system design  ",
            Feedback = "    Strong candidate "
        };

        var handler = new UpdateInterviewCommandHandler(context);
        var command = new UpdateInterviewCommand(app.Id, interview.Id, dto);    

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Notes.Should().Be("Prepare system design");
        result.Feedback.Should().Be("Strong candidate");

        context.ChangeTracker.Clear();

        var dbInterview = await context.Interviews
            .SingleAsync(x => x.Id == interview.Id, CancellationToken.None);

        dbInterview.Notes.Should().Be("Prepare system design");
        dbInterview.Feedback.Should().Be("Strong candidate");
    }

    [Fact]
    public async Task Handle_WhenInterviewDoesNotExist_ShouldThrowResourceNotFoundException()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var dto = new UpdateInterviewDto
        {
            Type = InterviewType.HRInterview,
            ScheduledAt = DateTime.UtcNow,
            ConductedAt = null,
            Notes = " Prepare system design ",
            Feedback = "Strong candidate"
        };
        var nonExistentApplicationId = Guid.NewGuid();
        var nonExistentInterviewId = Guid.NewGuid();

        var handler = new UpdateInterviewCommandHandler(context);
        var command = new UpdateInterviewCommand(nonExistentApplicationId, nonExistentInterviewId, dto);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ResourceNotFoundException>()
            .WithMessage($"Resource with application ID '{nonExistentApplicationId}' and interview ID '{nonExistentInterviewId}' was not found.");
    }

    [Fact]
    public async Task Handle_WhenInterviewBelongsToAnotherApplication_ShouldThrowAndNotChangeData()
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

        var otherApplicationId = Guid.NewGuid();

        context.ChangeTracker.Clear();

        var dto = new UpdateInterviewDto
        {
            Type = InterviewType.TechnicalInterview,
            ScheduledAt = scheduledAt,
            Notes = "Should not be saved"
        };

        var handler = new UpdateInterviewCommandHandler(context);
        var command = new UpdateInterviewCommand(otherApplicationId, interview.Id, dto);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ResourceNotFoundException>();

        context.ChangeTracker.Clear();

        var dbInterview = await context.Interviews.SingleAsync(x => x.Id == interview.Id);
        dbInterview.Type.Should().Be(InterviewType.HRInterview);
        dbInterview.ScheduledAt.Should().Be(scheduledAt);
        dbInterview.ConductedAt.Should().BeNull();
        dbInterview.Notes.Should().Be("Interview notes");
        dbInterview.Feedback.Should().Be("Interview feedback");
    }

    [Fact]
    public async Task Handle_WhenNoDatesProvided_ShouldThrowDomainExceptionAndNotChangeData()
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

        context.ChangeTracker.Clear();

        var dto = new UpdateInterviewDto
        {
            Type = InterviewType.TechnicalInterview,
            ScheduledAt = null,
            ConductedAt = null,
            Notes = "Should not be saved"
        };

        var handler = new UpdateInterviewCommandHandler(context);
        var command = new UpdateInterviewCommand(app.Id, interview.Id, dto);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("Interview must have either a scheduled or conducted date.");

        context.ChangeTracker.Clear();
        var dbInterview = await context.Interviews.SingleAsync(x => x.Id == interview.Id);
        dbInterview.Type.Should().Be(InterviewType.HRInterview);
        dbInterview.ScheduledAt.Should().Be(scheduledAt);
        dbInterview.Notes.Should().Be("Interview notes");
    }
}
