using CareerPulse.Application.Features.Companies.Queries.GetCompanyById;
using CareerPulse.Application.Tests.TestHelpers;
using CareerPulse.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace CareerPulse.Application.Tests.Features.Companies;

public class GetCompanyByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenCompanyExists_ShouldReturnMappedCompanyDto()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var company = Company.Create("Found Corp", "https://found.com", "Retail");
        company.Update("Found Corp", "https://found.com", "Retail", "Found notes");
        context.Companies.Add(company);
        await context.SaveChangesAsync();

        var handler = new GetCompanyByIdQueryHandler(context);
        var query = new GetCompanyByIdQuery(company.Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(company.Id);
        result.Name.Should().Be("Found Corp");
        result.Website.Should().Be("https://found.com");
        result.Industry.Should().Be("Retail");
        result.Notes.Should().Be("Found notes");
        result.IsArchived.Should().BeFalse();
        result.CreatedAt.Should().Be(company.CreatedAt);
        result.UpdatedAt.Should().Be(company.UpdatedAt);
    }

    [Fact]
    public async Task Handle_WhenCompanyIsArchived_ShouldStillReturnCompanyDto()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var company = Company.Create("Archived Corp");
        company.Archive();
        context.Companies.Add(company);
        await context.SaveChangesAsync();

        var handler = new GetCompanyByIdQueryHandler(context);
        var query = new GetCompanyByIdQuery(company.Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(company.Id);
        result.Name.Should().Be("Archived Corp");
        result.IsArchived.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenCompanyDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var handler = new GetCompanyByIdQueryHandler(context);
        var query = new GetCompanyByIdQuery(Guid.NewGuid());

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}
