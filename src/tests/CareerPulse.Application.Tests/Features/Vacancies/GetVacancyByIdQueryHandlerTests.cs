using CareerPulse.Application.Features.Vacancies.Queries.GetVacancyById;
using CareerPulse.Application.Tests.TestHelpers;
using CareerPulse.Domain.Entities;
using CareerPulse.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace CareerPulse.Application.Tests.Features.Vacancies;

public class GetVacancyByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenVacancyExists_ShouldReturnMappedVacancyDto()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var company = Company.Create("Single Vacancy Corp");
        context.Companies.Add(company);
        await context.SaveChangesAsync();

        var postedAt = DateTime.UtcNow.AddDays(-5);
        var vacancy = Vacancy.Create(company.Id, "Staff Engineer", "Kyiv", WorkMode.Remote, EmploymentType.FullTime, 1000, 5000, SalaryCurrency.USD, "Full description", "Full responsibilities", "Full requirements", " Full NiceToHave", "Full Benefits", "https://single.com/job", postedAt);
        context.Vacancies.Add(vacancy);
        await context.SaveChangesAsync();

        var handler = new GetVacancyByIdQueryHandler(context);
        var query = new GetVacancyByIdQuery(vacancy.Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(vacancy.Id);
        result.CompanyId.Should().Be(company.Id);
        result.Title.Should().Be("Staff Engineer");
        result.Description.Should().Be("Full description");
        result.Url.Should().Be("https://single.com/job");
        result.Location.Should().Be("Kyiv");
        result.WorkMode.Should().Be(WorkMode.Remote);
        result.SalaryMin.Should().Be(1000);
        result.SalaryMax.Should().Be(5000);
        result.SalaryCurrency.Should().Be(SalaryCurrency.USD);
        result.PostedAt.Should().Be(postedAt);
        result.CreatedAt.Should().Be(vacancy.CreatedAt);
        result.UpdatedAt.Should().Be(vacancy.UpdatedAt);
    }

    [Fact]
    public async Task Handle_WhenVacancyDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var handler = new GetVacancyByIdQueryHandler(context);
        var query = new GetVacancyByIdQuery(Guid.NewGuid());

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}
