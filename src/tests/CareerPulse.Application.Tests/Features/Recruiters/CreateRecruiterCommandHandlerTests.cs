using CareerPulse.Application.DTOs.Recruiters;
using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Features.Recruiters.Commands.CreateRecruiter;
using CareerPulse.Application.Tests.TestHelpers;
using CareerPulse.Domain.Entities;
using CareerPulse.Domain.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CareerPulse.Application.Tests.Features.Recruiters;

public class CreateRecruiterCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidData_ShouldPersistAllFields()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var company = Company.Create("Epum");
        context.Companies.Add(company);
        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var dto = new CreateRecruiterDto
        {
            Name = "Inna",
            Phone = "+380998887766",
            Email = "inna@epum.com",
            LinkedInUrl = "https://linkedin.com/in/inna",
            TelegramUrl = "https://t.me/inna",
            Notes = "Notes"
        };

        var handler = new CreateRecruiterCommandHandler(context);
        var command = new CreateRecruiterCommand(company.Id, dto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Id.Should().NotBeEmpty();    
        result.Should().BeEquivalentTo(new RecruiterDto
        {
            Id = result.Id,
            CompanyId = company.Id,
            Name = "Inna",
            Phone = "+380998887766",
            Email = "inna@epum.com",
            LinkedInUrl = "https://linkedin.com/in/inna",
            TelegramUrl = "https://t.me/inna",
            Notes = "Notes"
        });

        context.ChangeTracker.Clear();

        var dbRecruiter = await context.Recruiters.SingleAsync(x => x.Id == result.Id);

        dbRecruiter.CompanyId.Should().Be(company.Id);
        dbRecruiter.Name.Should().Be("Inna");
        dbRecruiter.Phone.Should().Be("+380998887766");
        dbRecruiter.Email.Should().Be("inna@epum.com");
        dbRecruiter.LinkedInUrl.Should().Be("https://linkedin.com/in/inna");
        dbRecruiter.TelegramUrl.Should().Be("https://t.me/inna");
        dbRecruiter.Notes.Should().Be("Notes");
    }

    [Fact]
    public async Task Handle_WhenCompanyDoesNotExist_ShouldThrowAndNotSaveRecruiter()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var missingId = Guid.NewGuid();

        var dto = new CreateRecruiterDto
        {
            Name = "Inna"
        };

        var handler = new CreateRecruiterCommandHandler(context);
        var command = new CreateRecruiterCommand(missingId, dto);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ResourceNotFoundException>()
            .WithMessage($"Company with ID '{missingId}' was not found.");

        (await context.Recruiters.AnyAsync()).Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WithWhitespaceInFields_ShouldTrimValuesAndCreate()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var company = Company.Create("Epum");
        context.Companies.Add(company);
        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var dto = new CreateRecruiterDto
        {
            Name = " Inna   ",
            Phone = "   +380998887766  ",
            Email = "  inna@epum.com ",
            LinkedInUrl = " https://linkedin.com/in/inna",
            TelegramUrl = "https://t.me/inna ",
            Notes = "Notes      "
        };

        var handler = new CreateRecruiterCommandHandler(context);
        var command = new CreateRecruiterCommand(company.Id, dto);


        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(new RecruiterDto
        {
            Id = result.Id,
            CompanyId = company.Id,
            Name = "Inna",
            Phone = "+380998887766",
            Email = "inna@epum.com",
            LinkedInUrl = "https://linkedin.com/in/inna",
            TelegramUrl = "https://t.me/inna",
            Notes = "Notes"
        });

        context.ChangeTracker.Clear();

        var dbRecruiter = await context.Recruiters.SingleAsync(x => x.Id == result.Id);

        dbRecruiter.CompanyId.Should().Be(company.Id);
        dbRecruiter.Name.Should().Be("Inna");
        dbRecruiter.Phone.Should().Be("+380998887766");
        dbRecruiter.Email.Should().Be("inna@epum.com");
        dbRecruiter.LinkedInUrl.Should().Be("https://linkedin.com/in/inna");
        dbRecruiter.TelegramUrl.Should().Be("https://t.me/inna");
        dbRecruiter.Notes.Should().Be("Notes");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Handle_WhenOptionalFieldsAreBlank_ShouldSaveThemAsNull(string blank)
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var company = Company.Create("Epum");
        context.Companies.Add(company);
        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var dto = new CreateRecruiterDto
        {
            Name = "Inna",
            Phone = blank,
            Email = blank,
            LinkedInUrl = blank,
            TelegramUrl = blank,
            Notes = blank
        };

        var handler = new CreateRecruiterCommandHandler(context);

        // Act
        var result = await handler.Handle(
            new CreateRecruiterCommand(company.Id, dto), CancellationToken.None);

        // Assert
        result.Phone.Should().BeNull();
        result.Email.Should().BeNull();
        result.LinkedInUrl.Should().BeNull();
        result.TelegramUrl.Should().BeNull();
        result.Notes.Should().BeNull();

        context.ChangeTracker.Clear();

        var saved = await context.Recruiters.SingleAsync(x => x.Id == result.Id);

        saved.Phone.Should().BeNull();
        saved.Email.Should().BeNull();
        saved.LinkedInUrl.Should().BeNull();
        saved.TelegramUrl.Should().BeNull();
        saved.Notes.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Handle_WhenNameIsBlank_ShouldThrowDomainExceptionAndNotSaveRecruiter(string name)
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var company = Company.Create("Epum");
        context.Companies.Add(company);
        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var handler = new CreateRecruiterCommandHandler(context);
        var command = new CreateRecruiterCommand(company.Id, new CreateRecruiterDto { Name = name });

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("Recruiter name is required.");

        (await context.Recruiters.AnyAsync()).Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WhenCompanyIsArchived_ShouldStillCreateRecruiter()
    {
        using var context = TestDbContext.CreateInMemory();
        var company = Company.Create("Epum");
        company.Archive();
        context.Companies.Add(company);
        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var handler = new CreateRecruiterCommandHandler(context);
        var command = new CreateRecruiterCommand(company.Id, new CreateRecruiterDto { Name = "Inna" });

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        context.ChangeTracker.Clear();

        (await context.Recruiters.AnyAsync(x => x.Id == result.Id && x.CompanyId == company.Id))
            .Should().BeTrue();
    }
}
