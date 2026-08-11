using CareerPulse.Application.Features.Applications.Queries.GetApplications;
using CareerPulse.Application.Tests.TestHelpers;
using CareerPulse.Domain.Entities;
using CareerPulse.Domain.Enums;
using CareerPulse.Domain.ValueObjects;
using CareerPulse.Infrastructure.Persistence;
using FluentAssertions;
using Xunit;

using AppEntity = CareerPulse.Domain.Entities.Application;

namespace CareerPulse.Application.Tests.Features.Applications.Queries;

public class GetApplicationsQueryHandlerTests
{
    private static (Company company1, Company company2, Vacancy vacancy1, ResumeRevision revision) SeedDatabase(CareerPulseDbContext context)
    {
        var company1 = Company.Create("Alpha Inc");
        var company2 = Company.Create("Beta LLC");
        var vacancy1 = Vacancy.Create(company1.Id, "Dev 1");
        var personalInfo = PersonalInfo.Create("Charlie Brown", "charlie@example.com");
        var resume = Resume.Create("Charlie's Resume", ResumeTrack.Frontend, CareerLevel.Middle, "Engineer");
        var revision = resume.CreateFirstRevision("Engineer", personalInfo);

        context.Companies.AddRange(company1, company2);
        context.Vacancies.Add(vacancy1);
        context.Resumes.Add(resume);
        context.ResumeRevisions.Add(revision);
        context.SaveChanges();

        var app1 = AppEntity.Create(company1.Id, revision.Id, "LinkedIn", vacancy1.Id);
        var app2 = AppEntity.Create(company2.Id, revision.Id, "DOU");
        app2.TransitionTo(ApplicationStatus.Applied);

        context.Applications.AddRange(app1, app2);
        context.SaveChanges();

        return (company1, company2, vacancy1, revision);
    }

    [Fact]
    public async Task Handle_WithoutFilters_ShouldReturnAllApplications()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        SeedDatabase(context);

        var handler = new GetApplicationsQueryHandler(context);
        var query = new GetApplicationsQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_WithStatusFilter_ShouldReturnOnlyMatchingStatus()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        SeedDatabase(context);

        var handler = new GetApplicationsQueryHandler(context);
        var query = new GetApplicationsQuery(Status: ApplicationStatus.Applied);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.Single().Status.Should().Be(ApplicationStatus.Applied);
        result.Single().CompanyName.Should().Be("Beta LLC");
    }

    [Fact]
    public async Task Handle_WithCompanyFilter_ShouldReturnOnlyMatchingCompany()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var (company1, _, _, _) = SeedDatabase(context);

        var handler = new GetApplicationsQueryHandler(context);
        var query = new GetApplicationsQuery(CompanyId: company1.Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.Single().CompanyId.Should().Be(company1.Id);
    }

    [Fact]
    public async Task Handle_WithVacancyFilter_ShouldReturnOnlyMatchingVacancy()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var (_, _, vacancy1, _) = SeedDatabase(context);

        var handler = new GetApplicationsQueryHandler(context);
        var query = new GetApplicationsQuery(VacancyId: vacancy1.Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.Single().VacancyId.Should().Be(vacancy1.Id);
    }
}
