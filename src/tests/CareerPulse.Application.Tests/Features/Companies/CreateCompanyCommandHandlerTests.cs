using CareerPulse.Application.DTOs.Companies;
using CareerPulse.Application.Features.Companies.Commands.CreateCompany;
using CareerPulse.Application.Tests.TestHelpers;
using CareerPulse.Domain.Entities;
using CareerPulse.Domain.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CareerPulse.Application.Tests.Features.Companies;

public class CreateCompanyCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidData_ShouldCreateCompanyAndReturnDto()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var handler = new CreateCompanyCommandHandler(context);
        var dto = new CreateCompanyDto
        {
            Name = "TechCorp",
            Website = "https://techcorp.com",
            Industry = "Software"
        };
        var command = new CreateCompanyCommand(dto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.Name.Should().Be("TechCorp");
        result.Website.Should().Be("https://techcorp.com");
        result.Industry.Should().Be("Software");
        result.Notes.Should().BeNull();
        result.IsArchived.Should().BeFalse();
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        var dbCompany = await context.Companies.FirstOrDefaultAsync(c => c.Id == result.Id);
        dbCompany.Should().NotBeNull();
        dbCompany!.Name.Should().Be("TechCorp");
        dbCompany.Website.Should().Be("https://techcorp.com");
        dbCompany.Industry.Should().Be("Software");
    }

    [Fact]
    public async Task Handle_WithWhitespaceInFields_ShouldTrimValuesAndCreate()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var handler = new CreateCompanyCommandHandler(context);
        var dto = new CreateCompanyDto
        {
            Name = "  Spaced Out Inc  ",
            Website = "  https://spacedout.com  ",
            Industry = "  Robotics  "
        };
        var command = new CreateCompanyCommand(dto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Name.Should().Be("Spaced Out Inc");
        result.Website.Should().Be("https://spacedout.com");
        result.Industry.Should().Be("Robotics");

        var dbCompany = await context.Companies.FirstOrDefaultAsync(c => c.Id == result.Id);
        dbCompany.Should().NotBeNull();
        dbCompany!.Name.Should().Be("Spaced Out Inc");
        dbCompany.Website.Should().Be("https://spacedout.com");
        dbCompany.Industry.Should().Be("Robotics");
    }

    [Fact]
    public async Task Handle_WithDuplicateNameSameCase_ShouldThrowDomainException()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var existingCompany = Company.Create("InnovateSoft");
        context.Companies.Add(existingCompany);
        await context.SaveChangesAsync();

        var handler = new CreateCompanyCommandHandler(context);
        var dto = new CreateCompanyDto { Name = "InnovateSoft" };
        var command = new CreateCompanyCommand(dto);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("Company with name 'InnovateSoft' already exists.");
    }

    [Fact]
    public async Task Handle_WithDuplicateNameDifferentCaseAndSpaces_ShouldThrowDomainException()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var existingCompany = Company.Create("InnovateSoft");
        context.Companies.Add(existingCompany);
        await context.SaveChangesAsync();

        var handler = new CreateCompanyCommandHandler(context);
        var dto = new CreateCompanyDto { Name = "  INNOVATESOFT  " };
        var command = new CreateCompanyCommand(dto);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("Company with name 'INNOVATESOFT' already exists.");
    }

    [Fact]
    public async Task Handle_WhenDtoNameIsEmptyOrWhitespace_ShouldThrowDomainException()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var handler = new CreateCompanyCommandHandler(context);
        var dto = new CreateCompanyDto { Name = "   " };
        var command = new CreateCompanyCommand(dto);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("Company name is required.");
    }

    [Fact]
    public async Task Handle_WithOptionalNullFields_ShouldCreateCompanyWithNullWebsiteAndIndustry()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var handler = new CreateCompanyCommandHandler(context);
        var command = new CreateCompanyCommand("Simple Co");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Name.Should().Be("Simple Co");
        result.Website.Should().BeNull();
        result.Industry.Should().BeNull();
        result.Notes.Should().BeNull();
    }

    [Fact]
    public void MapToDto_ShouldMapAllFieldsCorrectly()
    {
        // Arrange
        var company = Company.Create("MapTest", "https://map.test", "Testing");
        company.Update("MapTest", "https://map.test", "Testing", "Test notes");

        // Act
        var dto = CreateCompanyCommandHandler.MapToDto(company);

        // Assert
        dto.Id.Should().Be(company.Id);
        dto.Name.Should().Be("MapTest");
        dto.Website.Should().Be("https://map.test");
        dto.Industry.Should().Be("Testing");
        dto.Notes.Should().Be("Test notes");
        dto.IsArchived.Should().BeFalse();
        dto.CreatedAt.Should().Be(company.CreatedAt);
        dto.UpdatedAt.Should().Be(company.UpdatedAt);
    }
}
