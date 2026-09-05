using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Features.Companies.Commands.ArchiveCompany;
using CareerPulse.Application.Tests.TestHelpers;
using CareerPulse.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CareerPulse.Application.Tests.Features.Companies;

public class ArchiveCompanyCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidData_ShouldArchiveCompany()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var company = Company.Create("Name");
        context.Companies.Add(company);
        await context.SaveChangesAsync();

        var originalUpdatedAt = company.UpdatedAt;

        var handler = new ArchiveCompanyCommandHandler(context);
        var companyId = company.Id;
        var command = new ArchiveCompanyCommand(companyId);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var dbCompany = await context.Companies
            .FirstOrDefaultAsync(c => c.Id == companyId);

        dbCompany.Should().NotBeNull();
        dbCompany.IsArchived.Should().BeTrue();
        dbCompany.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public async Task Handle_WhenCompanyIsAlreadyArchived_ShouldNotChangeUpdatedAt()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var company = Company.Create("Name");
        company.Archive();

        context.Companies.Add(company);
        await context.SaveChangesAsync();

        var originalUpdatedAt = company.UpdatedAt;

        var handler = new ArchiveCompanyCommandHandler(context);
        var command = new ArchiveCompanyCommand(company.Id);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var dbCompany = await context.Companies
            .FirstOrDefaultAsync(c => c.Id == company.Id);

        dbCompany.Should().NotBeNull();
        dbCompany!.IsArchived.Should().BeTrue();
        dbCompany.UpdatedAt.Should().Be(originalUpdatedAt);
    }

    [Fact]
    public async Task Handle_WhenCompanyDoesNotExist_ShouldThrowResourseNotFoundException()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var company = Company.Create("Name", "https://name.com", "Industry");
        context.Companies.Add(company);
        await context.SaveChangesAsync();

        var handler = new ArchiveCompanyCommandHandler(context);
        var anotherCompanyId = Guid.NewGuid();
        var command = new ArchiveCompanyCommand(anotherCompanyId);

        // Act 
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ResourceNotFoundException>()
            .WithMessage($"Company with ID '{anotherCompanyId}' was not found.");
    }
}
