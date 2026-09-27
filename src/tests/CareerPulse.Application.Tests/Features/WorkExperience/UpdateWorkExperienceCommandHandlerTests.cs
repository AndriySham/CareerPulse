using CareerPulse.Application.DTOs.WorkExperiences;
using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Features.WorkExperience.Commands.UpdateWorkExperience;
using CareerPulse.Application.Tests.TestHelpers;
using CareerPulse.Domain.Entities;
using CareerPulse.Domain.Enums;
using CareerPulse.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CareerPulse.Application.Tests.Features.WorkExperience;

public class UpdateWorkExperienceCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidData_ShouldUpdateWorkExperienceAndReturnDto()
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

        var dto = new UpdateWorkExperienceDto
        {
            CompanyName = "Epam",
            PositionTitle = ".NET Developer updated",
            StartMonth = 2,
            StartYear = 2025,
            EndMonth = 2,
            EndYear = 2026,
            IsCurrentJob = false,
            Description = "Description Healthcare platform",
            Achievements = "Designed and implemented a high-performance Redis caching layer for frequent database queries, reducing average response time by 20%",
            TechStack = ".NET, EF Core"
        };

        var command = new UpdateWorkExperienceCommand(workExperience.Id, dto);
        var handler = new UpdateWorkExperienceCommandHandler(context);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.ResumeRevisionId.Should().Be(revision.Id);
        result.CompanyName.Should().Be("Epam");
        result.PositionTitle.Should().Be(".NET Developer updated");
        result.StartMonth.Should().Be(2);
        result.StartYear.Should().Be(2025);
        result.EndMonth.Should().Be(2);
        result.EndYear.Should().Be(2026);
        result.IsCurrentJob.Should().BeFalse();
        result.Description.Should().Be("Description Healthcare platform");
        result.Achievements.Should().Be("Designed and implemented a high-performance Redis caching layer for frequent database queries, reducing average response time by 20%");
        result.TechStack.Should().Be(".NET, EF Core");

        var dbWorkExperience = await context.WorkExperiences.FirstOrDefaultAsync(x => x.Id == result.Id);
        dbWorkExperience.Should().NotBeNull();
        dbWorkExperience.Id.Should().Be(result.Id);
        dbWorkExperience.ResumeRevisionId.Should().Be(revision.Id);
        dbWorkExperience.CompanyName.Should().Be("Epam");
        dbWorkExperience.PositionTitle.Should().Be(".NET Developer updated");
        dbWorkExperience.StartMonth.Should().Be(2);
        dbWorkExperience.StartYear.Should().Be(2025);
        dbWorkExperience.EndMonth.Should().Be(2);
        dbWorkExperience.EndYear.Should().Be(2026);
        dbWorkExperience.IsCurrentJob.Should().BeFalse();
        dbWorkExperience.Description.Should().Be("Description Healthcare platform");
        dbWorkExperience.Achievements.Should().Be("Designed and implemented a high-performance Redis caching layer for frequent database queries, reducing average response time by 20%");
        dbWorkExperience.TechStack.Should().Be(".NET, EF Core");
    }

    [Fact]
    public async Task Handle_WithOnlyRequiredFields_ShouldUpdateWorkExperienceWithNullOptionalFields()
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

        var dto = new UpdateWorkExperienceDto
        {
            CompanyName = "Epam",
            PositionTitle = ".NET Developer updated",
            StartMonth = 2,
            StartYear = 2025,
            IsCurrentJob = true,
        };

        var command = new UpdateWorkExperienceCommand(workExperience.Id, dto);
        var handler = new UpdateWorkExperienceCommandHandler(context);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.ResumeRevisionId.Should().Be(revision.Id);
        result.CompanyName.Should().Be("Epam");
        result.PositionTitle.Should().Be(".NET Developer updated");
        result.StartMonth.Should().Be(2);
        result.StartYear.Should().Be(2025);
        result.EndMonth.Should().BeNull();
        result.EndYear.Should().BeNull();
        result.IsCurrentJob.Should().BeTrue();
        result.Description.Should().BeNull();
        result.Achievements.Should().BeNull();
        result.TechStack.Should().BeNull();

        var dbWorkExperience = await context.WorkExperiences.FirstOrDefaultAsync(x => x.Id == result.Id);
        dbWorkExperience.Should().NotBeNull();
        dbWorkExperience.Id.Should().Be(result.Id);
        dbWorkExperience.ResumeRevisionId.Should().Be(revision.Id);
        dbWorkExperience.CompanyName.Should().Be("Epam");
        dbWorkExperience.PositionTitle.Should().Be(".NET Developer updated");
        dbWorkExperience.StartMonth.Should().Be(2);
        dbWorkExperience.StartYear.Should().Be(2025);
        dbWorkExperience.EndMonth.Should().BeNull();
        dbWorkExperience.EndYear.Should().BeNull();
        dbWorkExperience.IsCurrentJob.Should().BeTrue();
        dbWorkExperience.Description.Should().BeNull();
        dbWorkExperience.Achievements.Should().BeNull();
        dbWorkExperience.TechStack.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WithWhitespaceAroundCompanyNameAndPositionTitle_ShouldTrimNameAndTitle()
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

        var dto = new UpdateWorkExperienceDto
        {
            CompanyName = "  Epam  ",
            PositionTitle = "   .NET Developer updated      ",
            StartMonth = 2,
            StartYear = 2025,
            IsCurrentJob = true,
        };

        var command = new UpdateWorkExperienceCommand(workExperience.Id, dto);
        var handler = new UpdateWorkExperienceCommandHandler(context);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.CompanyName.Should().Be("Epam");
        result.PositionTitle.Should().Be(".NET Developer updated");

        var dbWorkExperience = await context.WorkExperiences
            .FirstOrDefaultAsync(x => x.Id == result.Id);

        dbWorkExperience.Should().NotBeNull();
        dbWorkExperience.CompanyName.Should().Be("Epam");
        dbWorkExperience.PositionTitle.Should().Be(".NET Developer updated");
    }

    [Fact]
    public async Task Handle_WhenWorkExperienceDoesNotExist_ShouldThrowResourceNotFoundException()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var personalInfo = PersonalInfo.Create("Alice Smith", "alice@example.com");
        var resume = Resume.Create("Alice's Resume", ResumeTrack.FullStack, CareerLevel.Senior, "Dotnet Dev");
        var revision = resume.CreateFirstRevision("Dotnet Dev 1", personalInfo);
        var workExperience = Domain.Entities.WorkExperience.Create(revision.Id, "Epam", ".NET Developer", 1, 2025, 1, 2026, false, "Description", "Achievements", "C#");

        context.Resumes.Add(resume);
        context.ResumeRevisions.Add(revision);
        context.WorkExperiences.Add(workExperience);
        await context.SaveChangesAsync(CancellationToken.None);

        var nonExistentWorkExperienceId = Guid.NewGuid();
        var dto = new UpdateWorkExperienceDto
        {
            CompanyName = "Epum",
            PositionTitle = ".NET Developer updated",
            StartMonth = 2,
            StartYear = 2025,
            IsCurrentJob = true,
        };

        var command = new UpdateWorkExperienceCommand(nonExistentWorkExperienceId, dto);
        var handler = new UpdateWorkExperienceCommandHandler(context);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ResourceNotFoundException>()
            .WithMessage($"WorkExperience with ID '{nonExistentWorkExperienceId}' was not found.");

        var dbWorkExperience = await context.WorkExperiences
            .FirstOrDefaultAsync(x => x.Id == workExperience.Id, CancellationToken.None);

        dbWorkExperience.CompanyName.Should().Be("Epam");
        dbWorkExperience.PositionTitle.Should().Be(".NET Developer");
        dbWorkExperience.StartMonth.Should().Be(1);
        dbWorkExperience.StartYear.Should().Be(2025);
    }

    [Fact]
    public async Task Handle_WhenResumeRevisionIsUsedByApplication_ShouldThrowConflictException()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var personalInfo = PersonalInfo.Create("Alice Smith", "alice@example.com");
        var resume = Resume.Create("Alice's Resume", ResumeTrack.FullStack, CareerLevel.Senior, "Dotnet Dev");
        var revision = resume.CreateFirstRevision("Dotnet Dev", personalInfo);
        var workExperience = Domain.Entities.WorkExperience.Create(revision.Id, "Epam", ".NET Developer", 1, 2025, 1, 2026, false, "Description", "Achievements", "C#");

        var company = Company.Create("Tech Corp", "https://techcorp.com");
        var vacancy = Vacancy.Create(company.Id, "Senior C# Developer", "https://techcorp.com/jobs/1");

        var application = Domain.Entities.Application.Create(company.Id, revision.Id, "https://dou.ua-vacancy-eot9te9", vacancy.Id);

        context.Resumes.Add(resume);
        context.ResumeRevisions.Add(revision);
        context.WorkExperiences.Add(workExperience);
        context.Companies.Add(company);
        context.Vacancies.Add(vacancy);
        context.Applications.Add(application);
        await context.SaveChangesAsync(CancellationToken.None);

        var dto = new UpdateWorkExperienceDto
        {
            CompanyName = "Epum",
            PositionTitle = ".NET Developer updated",
            StartMonth = 2,
            StartYear = 2025,
            IsCurrentJob = true,
        };

        var command = new UpdateWorkExperienceCommand(workExperience.Id, dto);
        var handler = new UpdateWorkExperienceCommandHandler(context);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("WorkExperience cannot be updated because its resume revision is already used in an application.");

        var dbWorkExperience = await context.WorkExperiences
            .FirstOrDefaultAsync(x => x.Id == workExperience.Id, CancellationToken.None);

        dbWorkExperience.CompanyName.Should().Be("Epam");
        dbWorkExperience.PositionTitle.Should().Be(".NET Developer");
        dbWorkExperience.StartMonth.Should().Be(1);
        dbWorkExperience.StartYear.Should().Be(2025);
        dbWorkExperience.EndMonth.Should().Be(1);
        dbWorkExperience.EndYear.Should().Be(2026);
        dbWorkExperience.IsCurrentJob.Should().BeFalse();
    }
}
