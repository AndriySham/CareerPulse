using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Features.Companies.Commands.RestoreCompany;
using CareerPulse.Application.Tests.TestHelpers;
using CareerPulse.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using Xunit;

namespace CareerPulse.Application.Tests.Features.Companies;

public class RestoreCompanyCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidDatа_ShouldRestoreCompany()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var company = Company.Create("Name");
        context.Companies.Add(company);
        await context.SaveChangesAsync(CancellationToken.None);

        company.Archive();
        var originalUpdatedAt = company.UpdatedAt;

        var handler = new RestoreCompanyCommandHandler(context);
        var companyId = company.Id;
        var command = new RestoreCompanyCommand(companyId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(company.Id);
        result.Name.Should().Be("Name");
        result.IsArchived.Should().BeFalse();
        result.UpdatedAt.Should().BeAfter(originalUpdatedAt);

        var dbCompany = await context.Companies
            .FirstOrDefaultAsync(c => c.Id == companyId);

        dbCompany.Should().NotBeNull();
        dbCompany.IsArchived.Should().BeFalse();
        dbCompany.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public async Task Handle_WhenCompanyIsAlreadyRestored_ShouldNotChangeUpdatedAt()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var company = Company.Create("Name");
        context.Companies.Add(company);
        await context.SaveChangesAsync(CancellationToken.None);

        var originalUpdatedAt = company.UpdatedAt;

        var handler = new RestoreCompanyCommandHandler(context);
        var command = new RestoreCompanyCommand(company.Id);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsArchived.Should().BeFalse();
        result.UpdatedAt.Should().Be(originalUpdatedAt);

        var dbCompany = await context.Companies
            .FirstOrDefaultAsync(c => c.Id == company.Id);

        dbCompany.Should().NotBeNull();
        dbCompany.IsArchived.Should().BeFalse();
        dbCompany.UpdatedAt.Should().Be(originalUpdatedAt);
    }

    [Fact]
    public async Task Handle_WhenCompanyDoesNotExist_ShouldThrowResourceNotFoundException()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var company = Company.Create("Name");
        context.Companies.Add(company);
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new RestoreCompanyCommandHandler(context);
        var anotherCompanyId = Guid.NewGuid();
        var command = new RestoreCompanyCommand(anotherCompanyId);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ResourceNotFoundException>()
            .WithMessage($"Company with ID '{anotherCompanyId}' was not found.");
    }
}
