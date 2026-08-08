using CareerPulse.Application.Features.Applications.Queries.GetApplicationById;
using CareerPulse.Application.Tests.TestHelpers;
using CareerPulse.Domain.Entities;
using CareerPulse.Domain.Enums;
using CareerPulse.Domain.ValueObjects;
using CareerPulse.Infrastructure.Persistence;
using FluentAssertions;
using Xunit;

using AppEntity = CareerPulse.Domain.Entities.Application;

namespace CareerPulse.Application.Tests.Features.Applications.Queries;

public class GetApplicationByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenApplicationExists_ShouldReturnApplicationDto()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var company = Company.Create("QueryCorp");
        var vacancy = Vacancy.Create(company.Id, "Backend Engineer");
        var personalInfo = PersonalInfo.Create("Bob Martin", "bob@example.com");
        var revision = ResumeRevision.CreateDraft(personalInfo, "Clean Coder");

        context.Companies.Add(company);
        context.Vacancies.Add(vacancy);
        context.ResumeRevisions.Add(revision);
        context.SaveChanges();

        var app = AppEntity.Create(company.Id, revision.Id, "GitHub Jobs", vacancy.Id);
        app.UpdateNotes("Good fit");
        context.Applications.Add(app);
        context.SaveChanges();

        var handler = new GetApplicationByIdQueryHandler(context);
        var query = new GetApplicationByIdQuery(app.Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(app.Id);
        result.CompanyId.Should().Be(company.Id);
        result.CompanyName.Should().Be("QueryCorp");
        result.VacancyId.Should().Be(vacancy.Id);
        result.VacancyTitle.Should().Be("Backend Engineer");
        result.ResumeRevisionId.Should().Be(revision.Id);
        result.Status.Should().Be(ApplicationStatus.Draft);
        result.JobSource.Should().Be("GitHub Jobs");
        result.Notes.Should().Be("Good fit");
    }

    [Fact]
    public async Task Handle_WhenApplicationDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var handler = new GetApplicationByIdQueryHandler(context);
        var query = new GetApplicationByIdQuery(Guid.NewGuid());

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}
