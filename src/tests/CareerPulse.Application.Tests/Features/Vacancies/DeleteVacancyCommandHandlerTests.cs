using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Features.Vacancies.Commands.DeleteVacancy;
using CareerPulse.Application.Tests.TestHelpers;
using CareerPulse.Domain.Entities;
using CareerPulse.Domain.Enums;
using CareerPulse.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CareerPulse.Application.Tests.Features.Vacancies;

public class DeleteVacancyCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidData_ShouldDeleteVacancy()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var company = Company.Create("Tech Corp", "https://techcorp.com");
        var vacancy = Vacancy.Create(company.Id, "Senior C# Developer", "https://techcorp.com/jobs/1");
        
        context.Add(company);
        context.Add(vacancy);
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new DeleteVacancyCommandHandler(context);
        var command = new DeleteVacancyCommand(vacancy.Id);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var vacancyExist = await context.Vacancies.AnyAsync(x => x.Id == vacancy.Id);
        vacancyExist.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WhenVacancyDoesNotExist_ShouldThrowResourseNotFoundException()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var company = Company.Create("Tech Corp", "https://techcorp.com");
        context.Add(company);
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new DeleteVacancyCommandHandler(context);
        var vacancyId = Guid.NewGuid();
        var command = new DeleteVacancyCommand(vacancyId);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ResourceNotFoundException>()
            .WithMessage($"Vacancy with ID '{vacancyId}' was not found.");
    }

    [Fact]
    public async Task Handle_WhenDeletionIsProhibited_ThrowConflictException()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var company = Company.Create("Tech Corp", "https://techcorp.com");
        var vacancy = Vacancy.Create(company.Id, "Senior C# Developer", "https://techcorp.com/jobs/1");
        var personalInfo = PersonalInfo.Create("John Doe", "john@example.com");
        var resume = Resume.Create("John's Resume", ResumeTrack.Backend, CareerLevel.Senior, "Experienced .NET Engineer");
        var revision = resume.CreateFirstRevision("Experienced .NET Engineer", personalInfo);
        var application = Domain.Entities.Application.Create(company.Id, revision.Id, "https://dou.ua", vacancy.Id);
        
        context.Companies.Add(company);
        context.Vacancies.Add(vacancy);
        context.Resumes.Add(resume);
        context.ResumeRevisions.Add(revision);
        context.Applications.Add(application);
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new DeleteVacancyCommandHandler(context);
        var command = new DeleteVacancyCommand(vacancy.Id);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert(
        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("There is an application for the vacancy. Deletion is prohibited.");

        var exists = await context.Vacancies.AnyAsync(x => x.Id == vacancy.Id);
        exists.Should().BeTrue();
    }
}
