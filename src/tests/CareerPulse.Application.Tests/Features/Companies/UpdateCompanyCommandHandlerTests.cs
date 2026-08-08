using CareerPulse.Application.DTOs.Companies;
using CareerPulse.Application.Features.Companies.Commands.UpdateCompany;
using CareerPulse.Application.Tests.TestHelpers;
using CareerPulse.Domain.Entities;
using CareerPulse.Domain.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CareerPulse.Application.Tests.Features.Companies;

public class UpdateCompanyCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidData_ShouldUpdateCompanyAndReturnDto()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var company = Company.Create("Old Name", "https://old.com", "Old Industry");
        context.Companies.Add(company);
        await context.SaveChangesAsync();

        var handler = new UpdateCompanyCommandHandler(context);
        var dto = new UpdateCompanyDto
        {
            Name = "New Name",
            Website = "https://new.com",
            Industry = "New Industry",
            Notes = "Updated notes"
        };
        var command = new UpdateCompanyCommand(company.Id, dto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(company.Id);
        result.Name.Should().Be("New Name");
        result.Website.Should().Be("https://new.com");
        result.Industry.Should().Be("New Industry");
        result.Notes.Should().Be("Updated notes");
        result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        var dbCompany = await context.Companies.FirstOrDefaultAsync(c => c.Id == company.Id);
        dbCompany.Should().NotBeNull();
        dbCompany!.Name.Should().Be("New Name");
        dbCompany.Website.Should().Be("https://new.com");
        dbCompany.Industry.Should().Be("New Industry");
        dbCompany.Notes.Should().Be("Updated notes");
    }

    [Fact]
    public async Task Handle_WhenUpdatingSameEntityWithSameName_ShouldSucceedWithoutDuplicateError()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var company = Company.Create("Same Company");
        context.Companies.Add(company);
        await context.SaveChangesAsync();

        var handler = new UpdateCompanyCommandHandler(context);
        var dto = new UpdateCompanyDto
        {
            Name = "  same company  ",
            Website = "https://same.com",
            Industry = "Same Industry",
            Notes = "Some notes"
        };
        var command = new UpdateCompanyCommand(company.Id, dto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Name.Should().Be("same company");
        result.Website.Should().Be("https://same.com");
    }

    [Fact]
    public async Task Handle_WhenUpdatingWithAnotherExistingCompanyName_ShouldThrowDomainException()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var company1 = Company.Create("Company One");
        var company2 = Company.Create("Company Two");
        context.Companies.AddRange(company1, company2);
        await context.SaveChangesAsync();

        var handler = new UpdateCompanyCommandHandler(context);
        var dto = new UpdateCompanyDto { Name = "Company Two" };
        var command = new UpdateCompanyCommand(company1.Id, dto);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("Company with name 'Company Two' already exists.");
    }

    [Fact]
    public async Task Handle_WhenUpdatingWithAnotherExistingCompanyNameCaseInsensitive_ShouldThrowDomainException()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var company1 = Company.Create("Company One");
        var company2 = Company.Create("Company Two");
        context.Companies.AddRange(company1, company2);
        await context.SaveChangesAsync();

        var handler = new UpdateCompanyCommandHandler(context);
        var dto = new UpdateCompanyDto { Name = "  COMPANY TWO  " };
        var command = new UpdateCompanyCommand(company1.Id, dto);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("Company with name 'COMPANY TWO' already exists.");
    }

    [Fact]
    public async Task Handle_WhenCompanyDoesNotExist_ShouldThrowDomainException()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var nonExistentId = Guid.NewGuid();
        var handler = new UpdateCompanyCommandHandler(context);
        var dto = new UpdateCompanyDto { Name = "Valid Name" };
        var command = new UpdateCompanyCommand(nonExistentId, dto);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage($"Company with ID '{nonExistentId}' was not found.");
    }

    [Fact]
    public async Task Handle_WithWhitespaceInFields_ShouldTrimValuesAndUpdate()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var company = Company.Create("Original");
        context.Companies.Add(company);
        await context.SaveChangesAsync();

        var handler = new UpdateCompanyCommandHandler(context);
        var dto = new UpdateCompanyDto
        {
            Name = "  Trimmed Name  ",
            Website = "  https://trimmed.com  ",
            Industry = "  Trimmed Industry  ",
            Notes = "Untrimmed Notes"
        };
        var command = new UpdateCompanyCommand(company.Id, dto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Name.Should().Be("Trimmed Name");
        result.Website.Should().Be("https://trimmed.com");
        result.Industry.Should().Be("Trimmed Industry");
        result.Notes.Should().Be("Untrimmed Notes");
    }

    [Fact]
    public async Task Handle_WhenDtoNameIsEmptyOrWhitespace_ShouldThrowDomainException()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var company = Company.Create("Original Name");
        context.Companies.Add(company);
        await context.SaveChangesAsync();

        var handler = new UpdateCompanyCommandHandler(context);
        var dto = new UpdateCompanyDto { Name = "   " };
        var command = new UpdateCompanyCommand(company.Id, dto);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("Company name is required.");
    }
}
