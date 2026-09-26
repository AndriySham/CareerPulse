using CareerPulse.Application.DTOs.WorkExperiences;
using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Features.WorkExperience.Commands.CreateWorkExperience;
using CareerPulse.Application.Tests.TestHelpers;
using CareerPulse.Domain.Entities;
using CareerPulse.Domain.Enums;
using CareerPulse.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CareerPulse.Application.Tests.Features.WorkExperience;

public class CreateWorkExperienceCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidData_ShouldCreateWorkExperienceAndReturnDto()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var personalInfo = PersonalInfo.Create("Alice Smith", "alice@example.com");
        var resume = Resume.Create("Alice's Resume", ResumeTrack.FullStack, CareerLevel.Senior, "Dotnet Dev");
        var revision = resume.CreateFirstRevision("Dotnet Dev", personalInfo);

        context.Resumes.Add(resume);
        context.ResumeRevisions.Add(revision);
        await context.SaveChangesAsync(CancellationToken.None);

        var dto = new CreateWorkExperienceDto
        {
            ResumeRevisionId = revision.Id,
            CompanyName = "Epam",
            PositionTitle = ".NET Developer",
            StartMonth = 1,
            StartYear = 2025,
            EndMonth = 1,
            EndYear = 2026,
            IsCurrentJob = false,
            Description = "Description Healthcare platform",
            Achievements = "Designed and implemented a high-performance Redis caching layer for frequent database queries, reducing average response time by 20%",
            TechStack = ".NET, EF Core"
        };

        var command = new CreateWorkExperienceCommand(dto);
        var handler = new CreateWorkExperienceCommandHandler(context);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.ResumeRevisionId.Should().Be(revision.Id);
        result.CompanyName.Should().Be("Epam");
        result.PositionTitle.Should().Be(".NET Developer");
        result.StartMonth.Should().Be(1);
        result.StartYear.Should().Be(2025);
        result.EndMonth.Should().Be(1);
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
        dbWorkExperience.PositionTitle.Should().Be(".NET Developer");
        dbWorkExperience.StartMonth.Should().Be(1);
        dbWorkExperience.StartYear.Should().Be(2025);
        dbWorkExperience.EndMonth.Should().Be(1);
        dbWorkExperience.EndYear.Should().Be(2026);
        dbWorkExperience.IsCurrentJob.Should().BeFalse();
        dbWorkExperience.Description.Should().Be("Description Healthcare platform");
        dbWorkExperience.Achievements.Should().Be("Designed and implemented a high-performance Redis caching layer for frequent database queries, reducing average response time by 20%");
        dbWorkExperience.TechStack.Should().Be(".NET, EF Core");
    }

    [Fact]
    public async Task Handle_WithOnlyRequiredFields_ShouldCreateWorkExperienceWithNullOptionalField()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var personalInfo = PersonalInfo.Create("Alice Smith", "alice@example.com");
        var resume = Resume.Create("Alice's Resume", ResumeTrack.FullStack, CareerLevel.Senior, "Dotnet Dev");
        var revision = resume.CreateFirstRevision("Dotnet Dev", personalInfo);

        context.Resumes.Add(resume);
        context.ResumeRevisions.Add(revision);
        await context.SaveChangesAsync(CancellationToken.None);

        var dto = new CreateWorkExperienceDto
        {
            ResumeRevisionId = revision.Id,
            CompanyName = "Epam",
            PositionTitle = ".NET Developer",
            StartMonth = 1,
            StartYear = 2025,
            IsCurrentJob = true,
        };

        var command = new CreateWorkExperienceCommand(dto);
        var handler = new CreateWorkExperienceCommandHandler(context);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.ResumeRevisionId.Should().Be(revision.Id);
        result.CompanyName.Should().Be("Epam");
        result.PositionTitle.Should().Be(".NET Developer");
        result.StartMonth.Should().Be(1);
        result.StartYear.Should().Be(2025);
        result.IsCurrentJob.Should().BeTrue();
        result.EndMonth.Should().BeNull();
        result.EndYear.Should().BeNull();
        result.Description.Should().BeNull();
        result.Achievements.Should().BeNull();
        result.TechStack.Should().BeNull();

        var dbWorkExperience = await context.WorkExperiences.FirstOrDefaultAsync(x => x.Id == result.Id);
        dbWorkExperience.Should().NotBeNull();
        dbWorkExperience.Id.Should().Be(result.Id);
        dbWorkExperience.ResumeRevisionId.Should().Be(revision.Id);
        dbWorkExperience.CompanyName.Should().Be("Epam");
        dbWorkExperience.PositionTitle.Should().Be(".NET Developer");
        dbWorkExperience.StartMonth.Should().Be(1);
        dbWorkExperience.IsCurrentJob.Should().BeTrue();
        dbWorkExperience.EndMonth.Should().BeNull();
        dbWorkExperience.EndYear.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WithWhiteSpaceAroundCompanyNameAndPositionTitle_ShouldTrimNameAndTitle()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var personalInfo = PersonalInfo.Create("Alice Smith", "alice@example.com");
        var resume = Resume.Create("Alice's Resume", ResumeTrack.FullStack, CareerLevel.Senior, "Dotnet Dev");
        var revision = resume.CreateFirstRevision("Dotnet Dev", personalInfo);

        context.Resumes.Add(resume);
        context.ResumeRevisions.Add(revision);
        await context.SaveChangesAsync(CancellationToken.None);

        var dto = new CreateWorkExperienceDto
        {
            ResumeRevisionId = revision.Id,
            CompanyName = "  Epam       ",
            PositionTitle = "    .NET Developer ",
            StartMonth = 1,
            StartYear = 2025,
            IsCurrentJob = true,
        };

        var command = new CreateWorkExperienceCommand(dto);
        var handler = new CreateWorkExperienceCommandHandler(context);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.ResumeRevisionId.Should().Be(revision.Id);
        result.CompanyName.Should().Be("Epam");
        result.PositionTitle.Should().Be(".NET Developer");
        result.StartMonth.Should().Be(1);
        result.StartYear.Should().Be(2025);
        result.IsCurrentJob.Should().BeTrue();

        var dbWorkExperience = await context.WorkExperiences.FirstOrDefaultAsync(x => x.Id == result.Id);
        dbWorkExperience.Should().NotBeNull();
        dbWorkExperience.Id.Should().Be(result.Id);
        dbWorkExperience.ResumeRevisionId.Should().Be(revision.Id);
        dbWorkExperience.CompanyName.Should().Be("Epam");
        dbWorkExperience.PositionTitle.Should().Be(".NET Developer");
        dbWorkExperience.StartMonth.Should().Be(1);
        dbWorkExperience.IsCurrentJob.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenResumeRevisionIdNotExist_ShouldThrowResourceNotFoundException()
    {
        // Arrange 
        using var context = TestDbContext.CreateInMemory();

        var personalInfo = PersonalInfo.Create("Alice Smith", "alice@example.com");
        var resume = Resume.Create("Alice's Resume", ResumeTrack.FullStack, CareerLevel.Senior, "Dotnet Dev");
        var revision = resume.CreateFirstRevision("Dotnet Dev", personalInfo);

        context.Resumes.Add(resume);
        context.ResumeRevisions.Add(revision);
        await context.SaveChangesAsync(CancellationToken.None);

        var nonExistentRevisionId = Guid.NewGuid();
        var dto = new CreateWorkExperienceDto
        {
            ResumeRevisionId = nonExistentRevisionId,
            CompanyName = "Epam",
            PositionTitle = ".NET Developer",
            StartMonth = 1,
            StartYear = 2025,
            IsCurrentJob = false,
        };

        var command = new CreateWorkExperienceCommand(dto);
        var handler = new CreateWorkExperienceCommandHandler(context);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ResourceNotFoundException>()
            .WithMessage($"ResumeRevision with ID '{nonExistentRevisionId}' was not found.");
    }
}
