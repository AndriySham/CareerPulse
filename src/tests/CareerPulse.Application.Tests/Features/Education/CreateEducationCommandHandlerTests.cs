using CareerPulse.Application.DTOs.Educations;
using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Features.Educations.Commands.CreateEducation;
using CareerPulse.Application.Tests.TestHelpers;
using CareerPulse.Domain.Entities;
using CareerPulse.Domain.Enums;
using CareerPulse.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CareerPulse.Application.Tests.Features.Education;

public class CreateEducationCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidData_ShouldCreateEducationAndReturnDto()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var personalInfo = PersonalInfo.Create("Alice Smith", "alice@example.com");
        var resume = Resume.Create("Alice's Resume", ResumeTrack.FullStack, CareerLevel.Senior, "Dotnet Dev");
        var revision = resume.CreateFirstRevision("Dotnet Dev", personalInfo);

        context.Resumes.Add(resume);
        context.ResumeRevisions.Add(revision);
        await context.SaveChangesAsync(CancellationToken.None);

        var dto = new CreateEducationDto
        {
            ResumeRevisionId = revision.Id,
            InstitutionName = "Dnipro Ukrainian State University of Science & Technologies",
            Description = "Master’s Degree in Engineering",
            StartYear = 2020,
            EndYear = 2025
        };

        var command = new CreateEducationCommand(dto);
        var handler = new CreateEducationCommandHandler(context);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.ResumeRevisionId.Should().Be(revision.Id);
        result.InstitutionName.Should().Be("Dnipro Ukrainian State University of Science & Technologies");
        result.Description.Should().Be("Master’s Degree in Engineering");
        result.StartYear.Should().Be(2020);
        result.EndYear.Should().Be(2025);

        var dbEducation = await context.Educations
            .FirstOrDefaultAsync(x => x.Id == result.Id, CancellationToken.None);
        dbEducation.Should().NotBeNull();
        dbEducation.Id.Should().Be(result.Id);
        dbEducation.ResumeRevisionId.Should().Be(revision.Id);
        dbEducation.InstitutionName.Should().Be("Dnipro Ukrainian State University of Science & Technologies");
        dbEducation.Description.Should().Be("Master’s Degree in Engineering");
        dbEducation.StartYear.Should().Be(2020);
        dbEducation.EndYear.Should().Be(2025);
    }

    [Fact]
    public async Task Handle_WithOnlyRequiredFields_ShouldCreateEducationWithNullOptionalFields()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var personalInfo = PersonalInfo.Create("Alice Smith", "alice@example.com");
        var resume = Resume.Create("Alice's Resume", ResumeTrack.FullStack, CareerLevel.Senior, "Dotnet Dev");
        var revision = resume.CreateFirstRevision("Dotnet Dev", personalInfo);

        context.Resumes.Add(resume);
        context.ResumeRevisions.Add(revision);
        await context.SaveChangesAsync(CancellationToken.None);

        var dto = new CreateEducationDto
        {
            ResumeRevisionId = revision.Id,
            InstitutionName = "Dnipro University"
        };

        var command = new CreateEducationCommand(dto);
        var handler = new CreateEducationCommandHandler(context);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.InstitutionName.Should().Be("Dnipro University");
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
    public async Task Handle_WithWhitespaceAroundInstitutionName_ShouldTrimName()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var personalInfo = PersonalInfo.Create("Alice Smith", "alice@example.com");
        var resume = Resume.Create("Alice's Resume", ResumeTrack.FullStack, CareerLevel.Senior, "Dotnet Dev");
        var revision = resume.CreateFirstRevision("Dotnet Dev", personalInfo);

        context.Resumes.Add(resume);
        context.ResumeRevisions.Add(revision);
        await context.SaveChangesAsync(CancellationToken.None);

        var dto = new CreateEducationDto
        {
            ResumeRevisionId = revision.Id,
            InstitutionName = "   Dnipro University   ",
            StartYear = 2020
        };

        var command = new CreateEducationCommand(dto);
        var handler = new CreateEducationCommandHandler(context);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.InstitutionName.Should().Be("Dnipro University");
    }

    [Fact]
    public async Task Handle_WhenResumeRevisionDoesNotExist_ShouldThrowResourseNotFoundException()
    {
        // Arrenge
        using var context = TestDbContext.CreateInMemory();

        var personalInfo = PersonalInfo.Create("Alice Smith", "alice@example.com");
        var resume = Resume.Create("Alice's Resume", ResumeTrack.FullStack, CareerLevel.Senior, "Dotnet Dev");
        var revision = resume.CreateFirstRevision("Dotnet Dev", personalInfo);

        context.Resumes.Add(resume);
        context.ResumeRevisions.Add(revision);
        await context.SaveChangesAsync(CancellationToken.None);

        var nonExistentRevisionId = Guid.NewGuid();
        var dto = new CreateEducationDto
        {
            ResumeRevisionId = nonExistentRevisionId,
            InstitutionName = "Dnipro Ukrainian State University of Science & Technologies",
            Description = "Master’s Degree in Engineering",
            StartYear = 2020,
            EndYear = 2025
        };

        var command = new CreateEducationCommand(dto);
        var handler = new CreateEducationCommandHandler(context);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ResourceNotFoundException>()
            .WithMessage($"ResumeRevision with ID '{dto.ResumeRevisionId}' was not found.");
    }    
}

