using CareerPulse.Application.Features.Vacancies.Queries.GetVacancies;
using CareerPulse.Application.Tests.TestHelpers;
using CareerPulse.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace CareerPulse.Application.Tests.Features.Vacancies;

public class GetVacanciesQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenNoCompanyIdSpecified_ShouldReturnAllVacanciesOrderedByCreatedAtDescending()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var company1 = Company.Create("Alpha Corp");
        var company2 = Company.Create("Beta Inc");
        context.Companies.AddRange(company1, company2);
        await context.SaveChangesAsync();

        var vacancy1 = Vacancy.Create(company1.Id, "First Vacancy");
        context.Vacancies.Add(vacancy1);
        await context.SaveChangesAsync();

        await Task.Delay(20);

        var vacancy2 = Vacancy.Create(company2.Id, "Second Vacancy");
        context.Vacancies.Add(vacancy2);
        await context.SaveChangesAsync();

        var handler = new GetVacanciesQueryHandler(context);
        var query = new GetVacanciesQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result[0].Title.Should().Be("Second Vacancy");
        result[1].Title.Should().Be("First Vacancy");
    }

    [Fact]
    public async Task Handle_WhenCompanyIdIsProvided_ShouldReturnVacanciesForThatCompanyOnly()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var company1 = Company.Create("Target Corp");
        var company2 = Company.Create("Other Corp");
        context.Companies.AddRange(company1, company2);
        await context.SaveChangesAsync();

        var v1 = Vacancy.Create(company1.Id, "Target Vacancy 1");
        var v2 = Vacancy.Create(company1.Id, "Target Vacancy 2");
        var v3 = Vacancy.Create(company2.Id, "Other Vacancy");
        context.Vacancies.AddRange(v1, v2, v3);
        await context.SaveChangesAsync();

        var handler = new GetVacanciesQueryHandler(context);
        var query = new GetVacanciesQuery(CompanyId: company1.Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(v => v.CompanyId == company1.Id);
        result.Select(v => v.Title).Should().Contain(new[] { "Target Vacancy 1", "Target Vacancy 2" });
    }

    [Fact]
    public async Task Handle_WhenCompanyIdIsGuidEmpty_ShouldReturnAllVacancies()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var company = Company.Create("Universal Corp");
        context.Companies.Add(company);
        await context.SaveChangesAsync();

        var v1 = Vacancy.Create(company.Id, "Vacancy 1");
        var v2 = Vacancy.Create(company.Id, "Vacancy 2");
        context.Vacancies.AddRange(v1, v2);
        await context.SaveChangesAsync();

        var handler = new GetVacanciesQueryHandler(context);
        var query = new GetVacanciesQuery(CompanyId: Guid.Empty);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_WhenNoVacanciesExist_ShouldReturnEmptyList()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var handler = new GetVacanciesQueryHandler(context);
        var query = new GetVacanciesQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldMapVacancyPropertiesToDtoCorrectly()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var company = Company.Create("Detail Inc");
        context.Companies.Add(company);
        await context.SaveChangesAsync();

        var postedAt = DateTime.UtcNow.AddDays(-3);
        var vacancy = Vacancy.Create(company.Id, "Full Spec Vacancy", "Full description", "https://detail.com/job", postedAt);
        context.Vacancies.Add(vacancy);
        await context.SaveChangesAsync();

        var handler = new GetVacanciesQueryHandler(context);
        var query = new GetVacanciesQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        var dto = result[0];
        dto.Id.Should().Be(vacancy.Id);
        dto.CompanyId.Should().Be(company.Id);
        dto.Title.Should().Be("Full Spec Vacancy");
        dto.Description.Should().Be("Full description");
        dto.Url.Should().Be("https://detail.com/job");
        dto.PostedAt.Should().Be(postedAt);
        dto.CreatedAt.Should().Be(vacancy.CreatedAt);
        dto.UpdatedAt.Should().Be(vacancy.UpdatedAt);
    }
}
