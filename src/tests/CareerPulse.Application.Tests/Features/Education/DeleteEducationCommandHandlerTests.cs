using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Features.Educations.Commands.DeleteEducation;
using CareerPulse.Application.Tests.TestHelpers;
using CareerPulse.Domain.Entities;
using CareerPulse.Domain.Enums;
using CareerPulse.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CareerPulse.Application.Tests.Features.Education;

public class DeleteEducationCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenDataValid_ShouldDeleteEducation()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var personalInfo = PersonalInfo.Create("Alice Smith", "alice@example.com");
        var resume = Resume.Create("Alice's Resume", ResumeTrack.FullStack, CareerLevel.Senior, "Dotnet Dev");
        var revision = resume.CreateFirstRevision("Dotnet Dev", personalInfo);
        var education = Domain.Entities.Education.Create(revision.Id, "Dnipro University", "Master’s Degree in Engineering", 2010, 2015);

        context.Resumes.Add(resume);
        context.ResumeRevisions.Add(revision);
        context.Educations.Add(education);
        await context.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteEducationCommand(education.Id);
        var handler = new DeleteEducationCommandHandler(context);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var dbEducation = await context.Educations.AnyAsync(x => x.Id == education.Id);
        dbEducation.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WhenEducationDoesNotExist_ShouldThrowResourceNotFoundException()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var personalInfo = PersonalInfo.Create("Alice Smith", "alice@example.com");
        var resume = Resume.Create("Alice's Resume", ResumeTrack.FullStack, CareerLevel.Senior, "Dotnet Dev");
        var revision = resume.CreateFirstRevision("Dotnet Dev", personalInfo);

        context.Resumes.Add(resume);
        context.ResumeRevisions.Add(revision);
        await context.SaveChangesAsync(CancellationToken.None);

        var nonExistentEducationId = Guid.NewGuid();
        var command = new DeleteEducationCommand(nonExistentEducationId);
        var handler = new DeleteEducationCommandHandler(context);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ResourceNotFoundException>()
            .WithMessage($"Education with ID '{nonExistentEducationId}' was not found.");
    }

    [Fact]
    public async Task Handle_WhenResumeRevisionIsUsedByApplication_ShouldThrowConflictException()
    {
        using var context = TestDbContext.CreateInMemory();

        var personalInfo = PersonalInfo.Create("Alice Smith", "alice@example.com");
        var resume = Resume.Create("Alice's Resume", ResumeTrack.FullStack, CareerLevel.Senior, "Dotnet Dev");
        var revision = resume.CreateFirstRevision("Dotnet Dev", personalInfo);
        var education = Domain.Entities.Education.Create(revision.Id, "Dnipro University", "Master’s Degree in Engineering", 2010, 2015);

        var company = Company.Create("Tech Corp", "https://techcorp.com");
        var vacancy = Vacancy.Create(company.Id, "Senior C# Developer", "https://techcorp.com/jobs/1");

        var application = Domain.Entities.Application.Create(company.Id, revision.Id, "https://dou.ua-vacancy-eot9te9", vacancy.Id);

        context.Resumes.Add(resume);
        context.ResumeRevisions.Add(revision);
        context.Educations.Add(education);
        context.Companies.Add(company);
        context.Vacancies.Add(vacancy);
        context.Applications.Add(application);
        await context.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteEducationCommand(education.Id);
        var handler = new DeleteEducationCommandHandler(context);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("Education cannot be deleted because ins resume revision is already used in an application.");

    }
}
