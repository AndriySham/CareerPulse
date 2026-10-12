using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Features.Interviews.Commands.DeleteInterview;
using CareerPulse.Application.Tests.TestHelpers;
using CareerPulse.Domain.Entities;
using CareerPulse.Domain.Enums;
using CareerPulse.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

using AppEntity = CareerPulse.Domain.Entities.Application;

namespace CareerPulse.Application.Tests.Features.Interviews;

public class DeleteInterviewCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenDataValid_ShouldDeleteInterview()
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

        var handler = new DeleteInterviewCommandHandler(context);
        var command = new DeleteInterviewCommand(app.Id, interview.Id);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        context.ChangeTracker.Clear();

        var dbInterview = await context.Interviews.SingleOrDefaultAsync(x => x.Id == interview.Id &&
            x.ApplicationId == app.Id, CancellationToken.None);

        dbInterview.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WhenInterviewDoesNotExist_ShouldThrowResourceNotFoundException()
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

        var nonExistentApplicationId = Guid.NewGuid();
        var nonExistentInterviewId = Guid.NewGuid();

        var handler = new DeleteInterviewCommandHandler(context);
        var command = new DeleteInterviewCommand(nonExistentApplicationId, nonExistentInterviewId);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ResourceNotFoundException>()
            .WithMessage($"Resource with application ID '{nonExistentApplicationId}' and interview ID '{nonExistentInterviewId}' was not found.");

        var dbInterview = await context.Interviews.AnyAsync(x => x.Id == interview.Id);
        dbInterview.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenInterviewBelongsToAnotherApplication_ShouldThrowAndKeepInterview()
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

        var handler = new DeleteInterviewCommandHandler(context);
        var command = new DeleteInterviewCommand(otherApplicationId, interview.Id);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ResourceNotFoundException>()
            .WithMessage($"Resource with application ID '{otherApplicationId}' and interview ID '{interview.Id}' was not found.");

        context.ChangeTracker.Clear();

        var saved = await context.Interviews.SingleAsync(x => x.Id == interview.Id);
        saved.ApplicationId.Should().Be(app.Id);
    }

    [Fact]
    public async Task Handle_WhenApplicationExistsButInterviewDoesNot_ShouldThrowAndKeepExistingInterview()
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

        var nonExistentInterviewId = Guid.NewGuid();

        context.ChangeTracker.Clear();

        var handler = new DeleteInterviewCommandHandler(context);
        var command = new DeleteInterviewCommand(app.Id, nonExistentInterviewId);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ResourceNotFoundException>();

        context.ChangeTracker.Clear();

        (await context.Interviews.AnyAsync(x => x.Id == interview.Id)).Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldNotDeleteOtherInterviewsOfSameApplication()
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

        var sibling = Interview.Create(
            app.Id, InterviewType.HRInterview, DateTime.UtcNow.AddDays(1), null, null, null);
        context.Interviews.Add(sibling);
        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var handler = new DeleteInterviewCommandHandler(context);
        var command = new DeleteInterviewCommand(app.Id, interview.Id);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        context.ChangeTracker.Clear();

        var remaining = await context.Interviews
            .Where(x => x.ApplicationId == app.Id)
            .ToListAsync();

        remaining.Should().ContainSingle()
            .Which.Id.Should().Be(sibling.Id);
    }
}
