using CareerPulse.Application.Features.Companies.Queries.GetCompanies;
using CareerPulse.Application.Tests.TestHelpers;
using CareerPulse.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace CareerPulse.Application.Tests.Features.Companies;

public class GetCompaniesQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenIncludeArchivedIsFalse_ShouldReturnOnlyNonArchivedCompaniesOrderedByName()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var companyB = Company.Create("Beta LLC");
        var companyA = Company.Create("Alpha Inc");
        var companyC = Company.Create("Gamma Corp");
        companyC.Archive();

        context.Companies.AddRange(companyB, companyA, companyC);
        await context.SaveChangesAsync();

        var handler = new GetCompaniesQueryHandler(context);
        var query = new GetCompaniesQuery(IncludeArchived: false);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result[0].Name.Should().Be("Alpha Inc");
        result[1].Name.Should().Be("Beta LLC");
        result.Should().NotContain(c => c.Name == "Gamma Corp");
    }

    [Fact]
    public async Task Handle_WhenIncludeArchivedIsTrue_ShouldReturnAllCompaniesIncludingArchivedOrderedByName()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var companyB = Company.Create("Beta LLC");
        var companyA = Company.Create("Alpha Inc");
        var companyC = Company.Create("Gamma Corp");
        companyC.Archive();

        context.Companies.AddRange(companyB, companyA, companyC);
        await context.SaveChangesAsync();

        var handler = new GetCompaniesQueryHandler(context);
        var query = new GetCompaniesQuery(IncludeArchived: true);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(3);
        result[0].Name.Should().Be("Alpha Inc");
        result[1].Name.Should().Be("Beta LLC");
        result[2].Name.Should().Be("Gamma Corp");
        result.Single(c => c.Name == "Gamma Corp").IsArchived.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenNoCompaniesExist_ShouldReturnEmptyList()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var handler = new GetCompaniesQueryHandler(context);
        var query = new GetCompaniesQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldMapCompanyPropertiesToDtoCorrectly()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var company = Company.Create("Detailed Corp", "https://detailed.com", "Technology");
        company.Update("Detailed Corp", "https://detailed.com", "Technology", "Detailed notes");
        context.Companies.Add(company);
        await context.SaveChangesAsync();

        var handler = new GetCompaniesQueryHandler(context);
        var query = new GetCompaniesQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        var dto = result[0];
        dto.Id.Should().Be(company.Id);
        dto.Name.Should().Be("Detailed Corp");
        dto.Website.Should().Be("https://detailed.com");
        dto.Industry.Should().Be("Technology");
        dto.Notes.Should().Be("Detailed notes");
        dto.IsArchived.Should().BeFalse();
        dto.CreatedAt.Should().Be(company.CreatedAt);
        dto.UpdatedAt.Should().Be(company.UpdatedAt);
    }
}
