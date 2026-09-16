using CareerPulse.Application.DTOs.Educations;
using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Features.Educations.Commands.UpdateEducation;
using CareerPulse.Application.Tests.TestHelpers;
using CareerPulse.Domain.Entities;
using CareerPulse.Domain.Enums;
using CareerPulse.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CareerPulse.Application.Tests.Features.Education;

public class UpdateEducationCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidData_ShouldUpdateEducationAndReturnDto()
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

        var dto = new UpdateEducationDto
        {
            ResumeRevisionId = revision.Id,
            InstitutionName = "Updated Dnipro University",
            Description = "Updated Master’s Degree in Engineering",
            StartYear = 2020,
            EndYear = 2025
        };

        var command = new UpdateEducationCommand(education.Id, dto);
        var handler = new UpdateEducationCommandHandler(context);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.ResumeRevisionId.Should().Be(revision.Id);
        result.InstitutionName.Should().Be("Updated Dnipro University");
        result.Description.Should().Be("Updated Master’s Degree in Engineering");
        result.StartYear.Should().Be(2020);
        result.EndYear.Should().Be(2025);

        var dbEducation = await context.Educations
            .FirstOrDefaultAsync(x => x.Id == result.Id, CancellationToken.None);
        dbEducation.Should().NotBeNull();
        dbEducation.Id.Should().Be(result.Id);
        dbEducation.ResumeRevisionId.Should().Be(revision.Id);
        dbEducation.InstitutionName.Should().Be("Updated Dnipro University");
        dbEducation.Description.Should().Be("Updated Master’s Degree in Engineering");
        dbEducation.StartYear.Should().Be(2020);
        dbEducation.EndYear.Should().Be(2025);
    }


    [Fact]
    public async Task Handle_WithOnlyRequiredFields_ShouldUpdateEducationWithNullOptionalFields()
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

        var dto = new UpdateEducationDto
        {
            ResumeRevisionId = revision.Id,
            InstitutionName = "Updated Dnipro University"
        };

        var command = new UpdateEducationCommand(education.Id, dto);
        var handler = new UpdateEducationCommandHandler(context);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.InstitutionName.Should().Be("Updated Dnipro University");
        result.Description.Should().BeNull();
        result.StartYear.Should().BeNull();
        result.EndYear.Should().BeNull();

        var dbEducation = await context.Educations
            .FirstOrDefaultAsync(x => x.Id == result.Id, CancellationToken.None);

        dbEducation!.Description.Should().BeNull();
        dbEducation.StartYear.Should().BeNull();
        dbEducation.EndYear.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WithWhitespaceAroundInstitutionName_ShouldTrimInstitutionName()
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

        var dto = new UpdateEducationDto
        {
            ResumeRevisionId = revision.Id,
            InstitutionName = "   Updated Dnipro University  ",
            Description = "Updated Master’s Degree in Engineering",
            StartYear = 2020,
            EndYear = 2025
        };

        var command = new UpdateEducationCommand(education.Id, dto);
        var handler = new UpdateEducationCommandHandler(context);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.InstitutionName.Should().Be("Updated Dnipro University");
    }

    [Fact]
    public async Task Handle_WhenEducationDoesNotExist_ShouldThrowResourceNotFoundException()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var personalInfo = PersonalInfo.Create("Alice Smith", "alice@example.com");
        var resume = Resume.Create("Alice's Resume", ResumeTrack.FullStack, CareerLevel.Senior, "Dotnet Dev");
        var revision1 = resume.CreateFirstRevision("Dotnet Dev 1", personalInfo);
        revision1.MarkAsApplied();
        var revision2 = resume.SpawnRevision(revision1);
        var education = Domain.Entities.Education.Create(revision2.Id, "Dnipro University", "Master’s Degree in Engineering", 2010, 2015);

        context.Resumes.Add(resume);
        context.ResumeRevisions.Add(revision1);
        context.ResumeRevisions.Add(revision2);
        context.Educations.Add(education);
        await context.SaveChangesAsync(CancellationToken.None);

        var dto = new UpdateEducationDto
        {
            ResumeRevisionId = revision1.Id,
            InstitutionName = "Updated Dnipro University",
            Description = "Updated Master’s Degree in Engineering",
            StartYear = 2020,
            EndYear = 2025
        };
        var nonExistentEducationId = Guid.NewGuid();

        var command = new UpdateEducationCommand(nonExistentEducationId, dto);
        var handler = new UpdateEducationCommandHandler(context);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ResourceNotFoundException>()
            .WithMessage($"Education with ID '{nonExistentEducationId}' was not found.");
    }

    [Fact]
    public async Task Handle_WhenEducationBelongsToAnotherRevision_ShouldThrowResourceNotFoundException()
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

        var nonExistentResumeRevisionId = Guid.NewGuid();
        var dto = new UpdateEducationDto
        {
            ResumeRevisionId = nonExistentResumeRevisionId,
            InstitutionName = "Updated Dnipro University",
            Description = "Updated Master’s Degree in Engineering",
            StartYear = 2020,
            EndYear = 2025
        };

        var command = new UpdateEducationCommand(education.Id, dto);
        var handler = new UpdateEducationCommandHandler(context);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ResourceNotFoundException>()
            .WithMessage($"Education with ID '{education.Id}' was not found.");
    }
}
