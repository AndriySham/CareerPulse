using CareerPulse.Application.DTOs.Interviews;
using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Features.Interviews.Commands.CreateInterview;
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

public class CreateInterviewCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidData_ShouldPersistAllFields()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var company = Company.Create("QueryCorp");
        var vacancy = Vacancy.Create(company.Id, "Backend Engineer");
        var personalInfo = PersonalInfo.Create("Bob Martin", "bob@example.com");
        var resume = Resume.Create("Bob's Resume", ResumeTrack.Backend, CareerLevel.Senior, "Clean Coder");
        var revision = resume.CreateFirstRevision("Clean Coder", personalInfo);

        context.Companies.Add(company);
        context.Vacancies.Add(vacancy);
        context.Resumes.Add(resume);
        context.ResumeRevisions.Add(revision);
        context.SaveChanges();

        var app = AppEntity.Create(company.Id, revision.Id, "GitHub Jobs", vacancy.Id);
        context.Applications.Add(app);
        await context.SaveChangesAsync();

        var scheduledAt = DateTime.UtcNow.AddDays(3);
        var dto = new CreateInterviewDto
        {
            Type = InterviewType.HRInterview,
            ScheduledAt = scheduledAt,
            Notes = " Prepare system design ",
            Feedback = "Strong candidate"
        };

        var handler = new CreateInterviewCommandHandler(context);
        var command = new CreateInterviewCommand(app.Id, dto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.ApplicationId.Should().Be(app.Id);
        result.Notes.Should().Be("Prepare system design");
        result.Feedback.Should().Be("Strong candidate");

        context.ChangeTracker.Clear();

        var dbInterview = await context.Interviews
            .SingleAsync(x => x.Id == result.Id);
        dbInterview.ApplicationId.Should().Be(app.Id);
        dbInterview.Type.Should().Be(InterviewType.HRInterview);
        dbInterview.ScheduledAt.Should().Be(scheduledAt);
        dbInterview.ConductedAt.Should().BeNull();
        dbInterview.Notes.Should().Be("Prepare system design");
        dbInterview.Feedback.Should().Be("Strong candidate");
    }

    [Fact]
    public async Task Handle_WhenApplicationDoesNotExist_ShouldThrowAndNotSaveInterview()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var missingId = Guid.NewGuid();

        var dto = new CreateInterviewDto
        {
            Type = InterviewType.HRInterview,
            ScheduledAt = DateTime.UtcNow
        };

        var handler = new CreateInterviewCommandHandler(context);
        var command = new CreateInterviewCommand(missingId, dto);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ResourceNotFoundException>()
            .WithMessage($"Application with ID '{missingId}' was not found.");

        (await context.Interviews.AnyAsync()).Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WhenNoDatesProvided_ShouldThrowDomainException()
    {
        // Arrenge
        using var context = TestDbContext.CreateInMemory();

        var company = Company.Create("QueryCorp");
        var vacancy = Vacancy.Create(company.Id, "Backend Engineer");
        var personalInfo = PersonalInfo.Create("Bob Martin", "bob@example.com");
        var resume = Resume.Create("Bob's Resume", ResumeTrack.Backend, CareerLevel.Senior, "Clean Coder");
        var revision = resume.CreateFirstRevision("Clean Coder", personalInfo);

        context.Companies.Add(company);
        context.Vacancies.Add(vacancy);
        context.Resumes.Add(resume);
        context.ResumeRevisions.Add(revision);
        context.SaveChanges();

        var app = AppEntity.Create(company.Id, revision.Id, "GitHub Jobs", vacancy.Id);
        context.Applications.Add(app);
        await context.SaveChangesAsync();

        var dto = new CreateInterviewDto { Type = InterviewType.HRInterview };

        var handler = new CreateInterviewCommandHandler(context);
        var command = new CreateInterviewCommand(app.Id, dto);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("Interview must have either a scheduled or conducted date.");
    }
}
