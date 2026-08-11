using CareerPulse.Application.DTOs.Applications;
using CareerPulse.Application.Features.Applications.Commands.SubmitApplication;
using CareerPulse.Application.Tests.TestHelpers;
using CareerPulse.Domain.Entities;
using CareerPulse.Domain.Enums;
using CareerPulse.Domain.Exceptions;
using CareerPulse.Domain.ValueObjects;
using CareerPulse.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

using AppEntity = CareerPulse.Domain.Entities.Application;

namespace CareerPulse.Application.Tests.Features.Applications.Commands;

public class SubmitApplicationCommandHandlerTests
{
    private static (Company company, ResumeRevision revision, Vacancy vacancy) SeedTestData(CareerPulseDbContext context)
    {
        var company = Company.Create("Tech Corp", "https://techcorp.com");
        var vacancy = Vacancy.Create(company.Id, "Senior C# Developer", "https://techcorp.com/jobs/1");
        var personalInfo = PersonalInfo.Create("John Doe", "john@example.com");
        var resume = Resume.Create("John's Resume", ResumeTrack.Backend, CareerLevel.Senior, "Experienced .NET Engineer");
        var revision = resume.CreateFirstRevision("Experienced .NET Engineer", personalInfo);

        context.Companies.Add(company);
        context.Vacancies.Add(vacancy);
        context.Resumes.Add(resume);
        context.ResumeRevisions.Add(revision);
        context.SaveChanges();

        return (company, revision, vacancy);
    }

    [Fact]
    public async Task Handle_WithValidData_SubmitImmediately_ShouldCreateApplicationAndMarkRevisionApplied()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var (company, revision, _) = SeedTestData(context);
        var handler = new SubmitApplicationCommandHandler(context);

        var dto = new SubmitApplicationDto
        {
            CompanyId = company.Id,
            ResumeRevisionId = revision.Id,
            JobSource = "LinkedIn",
            Notes = "Applied via referral",
            SubmitImmediately = true
        };
        var command = new SubmitApplicationCommand(dto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.CompanyId.Should().Be(company.Id);
        result.CompanyName.Should().Be("Tech Corp");
        result.ResumeRevisionId.Should().Be(revision.Id);
        result.Status.Should().Be(ApplicationStatus.Applied);
        result.JobSource.Should().Be("LinkedIn");
        result.Notes.Should().Be("Applied via referral");
        result.AppliedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        var dbApp = await context.Applications.FirstOrDefaultAsync(a => a.Id == result.Id);
        dbApp.Should().NotBeNull();
        dbApp!.Status.Should().Be(ApplicationStatus.Applied);

        var dbRevision = await context.ResumeRevisions.FirstOrDefaultAsync(r => r.Id == revision.Id);
        dbRevision!.Status.Should().Be(RevisionStatus.Applied);
    }

    [Fact]
    public async Task Handle_WithValidData_SubmitImmediatelyFalse_ShouldKeepDraftStatus()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var (company, revision, _) = SeedTestData(context);
        var handler = new SubmitApplicationCommandHandler(context);

        var dto = new SubmitApplicationDto
        {
            CompanyId = company.Id,
            ResumeRevisionId = revision.Id,
            JobSource = "DOU",
            SubmitImmediately = false
        };
        var command = new SubmitApplicationCommand(dto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.Should().Be(ApplicationStatus.Draft);
        result.AppliedAt.Should().BeNull();

        var dbRevision = await context.ResumeRevisions.FirstOrDefaultAsync(r => r.Id == revision.Id);
        dbRevision!.Status.Should().Be(RevisionStatus.Draft);
    }

    [Fact]
    public async Task Handle_WithVacancyBelongingToCompany_ShouldLinkVacancySuccessfully()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var (company, revision, vacancy) = SeedTestData(context);
        var handler = new SubmitApplicationCommandHandler(context);

        var dto = new SubmitApplicationDto
        {
            CompanyId = company.Id,
            ResumeRevisionId = revision.Id,
            VacancyId = vacancy.Id,
            JobSource = "Company Website",
            SubmitImmediately = true
        };
        var command = new SubmitApplicationCommand(dto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.VacancyId.Should().Be(vacancy.Id);
        result.VacancyTitle.Should().Be("Senior C# Developer");
    }

    [Fact]
    public async Task Handle_WhenCompanyNotFound_ShouldThrowDomainException()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var (_, revision, _) = SeedTestData(context);
        var handler = new SubmitApplicationCommandHandler(context);

        var nonExistentCompanyId = Guid.NewGuid();
        var dto = new SubmitApplicationDto
        {
            CompanyId = nonExistentCompanyId,
            ResumeRevisionId = revision.Id,
            JobSource = "LinkedIn"
        };
        var command = new SubmitApplicationCommand(dto);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage($"Company with ID {nonExistentCompanyId} was not found.");
    }

    [Fact]
    public async Task Handle_WhenResumeRevisionNotFound_ShouldThrowDomainException()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var (company, _, _) = SeedTestData(context);
        var handler = new SubmitApplicationCommandHandler(context);

        var nonExistentRevisionId = Guid.NewGuid();
        var dto = new SubmitApplicationDto
        {
            CompanyId = company.Id,
            ResumeRevisionId = nonExistentRevisionId,
            JobSource = "LinkedIn"
        };
        var command = new SubmitApplicationCommand(dto);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage($"ResumeRevision with ID {nonExistentRevisionId} was not found.");
    }

    [Fact]
    public async Task Handle_WhenVacancyNotFound_ShouldThrowDomainException()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var (company, revision, _) = SeedTestData(context);
        var handler = new SubmitApplicationCommandHandler(context);

        var nonExistentVacancyId = Guid.NewGuid();
        var dto = new SubmitApplicationDto
        {
            CompanyId = company.Id,
            ResumeRevisionId = revision.Id,
            VacancyId = nonExistentVacancyId,
            JobSource = "LinkedIn"
        };
        var command = new SubmitApplicationCommand(dto);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage($"Vacancy with ID {nonExistentVacancyId} was not found.");
    }

    [Fact]
    public async Task Handle_WhenVacancyBelongsToDifferentCompany_ShouldThrowDomainException()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var (company1, revision, _) = SeedTestData(context);

        var company2 = Company.Create("Other Corp");
        var vacancyCompany2 = Vacancy.Create(company2.Id, "Frontend Dev");
        context.Companies.Add(company2);
        context.Vacancies.Add(vacancyCompany2);
        await context.SaveChangesAsync();

        var handler = new SubmitApplicationCommandHandler(context);

        var dto = new SubmitApplicationDto
        {
            CompanyId = company1.Id,
            ResumeRevisionId = revision.Id,
            VacancyId = vacancyCompany2.Id,
            JobSource = "LinkedIn"
        };
        var command = new SubmitApplicationCommand(dto);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage($"Vacancy {vacancyCompany2.Id} does not belong to Company {company1.Id}.");
    }

    [Fact]
    public void MapToDto_ShouldMapAllPropertiesAndAllowedTransitions()
    {
        // Arrange
        var company = Company.Create("MapCorp");
        var vacancy = Vacancy.Create(company.Id, "FullStack Dev");
        var personalInfo = PersonalInfo.Create("Jane Smith", "jane@example.com");
        var resume = Resume.Create("Jane's Resume", ResumeTrack.FullStack, CareerLevel.Middle, "FullStack Dev");
        var revision = resume.CreateFirstRevision("Summary", personalInfo);

        var app = AppEntity.Create(company.Id, revision.Id, "Direct", vacancy.Id);
        app.UpdateNotes("Map notes");

        // Act
        var dto = SubmitApplicationCommandHandler.MapToDto(app);

        // Assert
        dto.Id.Should().Be(app.Id);
        dto.CompanyId.Should().Be(company.Id);
        dto.VacancyId.Should().Be(vacancy.Id);
        dto.ResumeRevisionId.Should().Be(revision.Id);
        dto.Status.Should().Be(ApplicationStatus.Draft);
        dto.JobSource.Should().Be("Direct");
        dto.Notes.Should().Be("Map notes");
        dto.AllowedTransitions.Should().BeEquivalentTo(new[] { ApplicationStatus.Applied });
    }
}
