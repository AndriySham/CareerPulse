using CareerPulse.Application.DTOs.Recruiters;
using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Features.Recruiters.Commands.UpdateRecruiter;
using CareerPulse.Application.Tests.TestHelpers;
using CareerPulse.Domain.Entities;
using CareerPulse.Domain.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CareerPulse.Application.Tests.Features.Recruiters;

public class UpdateRecruiterCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidData_ShouldUpdateRecruiterAndReturnDto()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var company = Company.Create("Epum");
        var recruiter = Recruiter.Create(company.Id, "Inna", "+380998887766", "inna@epum.com", "https://linkedin.com/in/inna", "https://t.me/inna", "Notes");
        context.Companies.Add(company);
        context.Recruiters.Add(recruiter);
        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var dto = new UpdateRecruiterDto
        {
            Name = "Inna Shevchenko",
            Phone = "+380958887766", 
            Email = "inna-shevchenko@epum.com", 
            LinkedInUrl = "https://linkedin.com/in/inna-shevchenko", 
            TelegramUrl = "https://t.me/inna-shevchenko", 
            Notes = "Notes updated"
        };
        var createdAt = recruiter.CreatedAt;

        var handler = new UpdateRecruiterCommandHandler(context);
        var command = new UpdateRecruiterCommand(company.Id, recruiter.Id, dto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Id.Should().NotBeEmpty();
        result.Should().BeEquivalentTo(new RecruiterDto
        {
            Id = result.Id,
            CompanyId = company.Id,
            Name = "Inna Shevchenko",
            Phone = "+380958887766",
            Email = "inna-shevchenko@epum.com",
            LinkedInUrl = "https://linkedin.com/in/inna-shevchenko",
            TelegramUrl = "https://t.me/inna-shevchenko",
            Notes = "Notes updated"
        });

        context.ChangeTracker.Clear();

        var dbRecruiter = await context.Recruiters.SingleAsync(x => x.Id == recruiter.Id);

        dbRecruiter.CompanyId.Should().Be(company.Id);
        dbRecruiter.Name.Should().Be("Inna Shevchenko");
        dbRecruiter.Phone.Should().Be("+380958887766");
        dbRecruiter.Email.Should().Be("inna-shevchenko@epum.com");
        dbRecruiter.LinkedInUrl.Should().Be("https://linkedin.com/in/inna-shevchenko");
        dbRecruiter.TelegramUrl.Should().Be("https://t.me/inna-shevchenko");
        dbRecruiter.Notes.Should().Be("Notes updated");
        dbRecruiter.CreatedAt.Should().Be(createdAt);
        dbRecruiter.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WithWhitespaceAroundFields_ShouldTrimValuesAndFeedback()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var company = Company.Create("Epum");
        var recruiter = Recruiter.Create(company.Id, "Inna", "+380998887766", "inna@epum.com", "https://linkedin.com/in/inna", "https://t.me/inna", "Notes");
        context.Companies.Add(company);
        context.Recruiters.Add(recruiter);
        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var dto = new UpdateRecruiterDto
        {
            Name = "Inna Shevchenko ",
            Phone = " +380958887766",
            Email = "   inna-shevchenko@epum.com  ",
            LinkedInUrl = " https://linkedin.com/in/inna-shevchenko ",
            TelegramUrl = "https://t.me/inna-shevchenko ",
            Notes = "   Notes updated"
        };

        var handler = new UpdateRecruiterCommandHandler(context);
        var command = new UpdateRecruiterCommand(company.Id, recruiter.Id, dto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(new RecruiterDto
        {
            Id = result.Id,
            CompanyId = company.Id,
            Name = "Inna Shevchenko",
            Phone = "+380958887766",
            Email = "inna-shevchenko@epum.com",
            LinkedInUrl = "https://linkedin.com/in/inna-shevchenko",
            TelegramUrl = "https://t.me/inna-shevchenko",
            Notes = "Notes updated"
        });

        context.ChangeTracker.Clear();

        var dbRecruiter = await context.Recruiters.SingleAsync(x => x.Id == recruiter.Id);

        dbRecruiter.Name.Should().Be("Inna Shevchenko");
        dbRecruiter.Phone.Should().Be("+380958887766");
        dbRecruiter.Email.Should().Be("inna-shevchenko@epum.com");
        dbRecruiter.LinkedInUrl.Should().Be("https://linkedin.com/in/inna-shevchenko");
        dbRecruiter.TelegramUrl.Should().Be("https://t.me/inna-shevchenko");
        dbRecruiter.Notes.Should().Be("Notes updated");
    }

    [Fact]
    public async Task Handle_WhenRecruiterDoesNotExist_ShouldThrowResourceNotFoundException()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var dto = new UpdateRecruiterDto
        {
            Name = "Inna Shevchenko",
            Phone = "+380958887766",
            Email = "inna-shevchenko@epum.com",
            LinkedInUrl = "https://linkedin.com/in/inna-shevchenko",
            TelegramUrl = "https://t.me/inna-shevchenko",
            Notes = "Notes updated"
        };

        var nonExistentCompanyId = Guid.NewGuid();
        var nonExistentRecruiterId = Guid.NewGuid();

        var handler = new UpdateRecruiterCommandHandler(context);
        var command = new UpdateRecruiterCommand(nonExistentCompanyId, nonExistentRecruiterId, dto);


        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ResourceNotFoundException>()
            .WithMessage($"Resource with Company ID '{nonExistentCompanyId}' " +
                $"and Recruiter ID '{nonExistentRecruiterId}' was not found.");
    }

    [Fact]
    public async Task Handle_WhenRecruiterBelongsToAnotherCompany_ShouldThrowAndNotChangeData()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var company = Company.Create("Epum");
        var anotherCompany = Company.Create("SoftServe");
        var recruiter = Recruiter.Create(anotherCompany.Id, "Inna", "+380998887766", "inna@epum.com", "https://linkedin.com/in/inna", "https://t.me/inna", "Notes");
        context.Companies.AddRange(company, anotherCompany);
        context.Recruiters.Add(recruiter);
        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var dto = new UpdateRecruiterDto
        {
            Name = "Inna Shevchenko",
            Phone = "+380958887766",
            Email = "inna-shevchenko@epum.com",
            LinkedInUrl = "https://linkedin.com/in/inna-shevchenko",
            TelegramUrl = "https://t.me/inna-shevchenko",
            Notes = "Notes updated"
        };

        var handler = new UpdateRecruiterCommandHandler(context);
        var command = new UpdateRecruiterCommand(company.Id, recruiter.Id, dto);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ResourceNotFoundException>()
            .WithMessage($"Resource with Company ID '{company.Id}' " +
                $"and Recruiter ID '{recruiter.Id}' was not found.");     
                
        context.ChangeTracker.Clear();

        var dbRecruiter = await context.Recruiters.SingleAsync(x => x.Id == recruiter.Id);

        dbRecruiter.CompanyId.Should().Be(anotherCompany.Id);
        dbRecruiter.Name.Should().Be("Inna");
        dbRecruiter.Phone.Should().Be("+380998887766");
        dbRecruiter.Email.Should().Be("inna@epum.com");
        dbRecruiter.LinkedInUrl.Should().Be("https://linkedin.com/in/inna");
        dbRecruiter.TelegramUrl.Should().Be("https://t.me/inna");
        dbRecruiter.Notes.Should().Be("Notes");
    }

    [Fact]
    public async Task Handle_ShouldUpdateOnlyTargetRecruiterOfCompany()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var company = Company.Create("Epum");
        var target = Recruiter.Create(company.Id, "Inna", notes: "Target notes");
        var sibling = Recruiter.Create(company.Id, "Maria", notes: "Sibling notes");
        context.Companies.Add(company);
        context.Recruiters.AddRange(target, sibling);
        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var dto = new UpdateRecruiterDto { Name = "Inna Shevchenko", Notes = "Updated notes" };
        var handler = new UpdateRecruiterCommandHandler(context);

        // Act
        await handler.Handle(
            new UpdateRecruiterCommand(company.Id, target.Id, dto), CancellationToken.None);

        // Assert
        context.ChangeTracker.Clear();

        var savedTarget = await context.Recruiters.SingleAsync(x => x.Id == target.Id);
        var savedSibling = await context.Recruiters.SingleAsync(x => x.Id == sibling.Id);

        savedTarget.Name.Should().Be("Inna Shevchenko");
        savedTarget.Notes.Should().Be("Updated notes");

        savedSibling.Name.Should().Be("Maria");
        savedSibling.Notes.Should().Be("Sibling notes");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Handle_WhenOptionalFieldsAreBlank_ShouldClearThemToNull(string? blank)
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var company = Company.Create("Epum");
        var recruiter = Recruiter.Create(company.Id, "Inna", "+380998887766", "inna@epum.com",
            "https://linkedin.com/in/inna", "https://t.me/inna", "Notes");
        context.Companies.Add(company);
        context.Recruiters.Add(recruiter);
        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var dto = new UpdateRecruiterDto
        {
            Name = "Inna",
            Phone = blank,
            Email = blank,
            LinkedInUrl = blank,
            TelegramUrl = blank,
            Notes = blank
        };

        var handler = new UpdateRecruiterCommandHandler(context);

        // Act
        var result = await handler.Handle(
            new UpdateRecruiterCommand(company.Id, recruiter.Id, dto), CancellationToken.None);

        // Assert
        result.Phone.Should().BeNull();
        result.Email.Should().BeNull();
        result.LinkedInUrl.Should().BeNull();
        result.TelegramUrl.Should().BeNull();
        result.Notes.Should().BeNull();

        context.ChangeTracker.Clear();

        var dbRecruiter = await context.Recruiters.SingleAsync(x => x.Id == recruiter.Id);

        dbRecruiter.Phone.Should().BeNull();
        dbRecruiter.Email.Should().BeNull();
        dbRecruiter.LinkedInUrl.Should().BeNull();
        dbRecruiter.TelegramUrl.Should().BeNull();
        dbRecruiter.Notes.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Handle_WhenNameIsBlank_ShouldThrowDomainExceptionAndNotChangeData(string name)
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var company = Company.Create("Epum");
        var recruiter = Recruiter.Create(company.Id, "Inna", "+380998887766", "inna@epum.com",
            "https://linkedin.com/in/inna", "https://t.me/inna", "Notes");
        context.Companies.Add(company);
        context.Recruiters.Add(recruiter);
        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var dto = new UpdateRecruiterDto
        {
            Name = name,
            Phone = "+380958887766",
            Notes = "Should not be saved"
        };

        var handler = new UpdateRecruiterCommandHandler(context);

        // Act
        var act = () => handler.Handle(
            new UpdateRecruiterCommand(company.Id, recruiter.Id, dto), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("Recruiter name is required.");

        context.ChangeTracker.Clear();

        var dbRecruiter = await context.Recruiters.SingleAsync(x => x.Id == recruiter.Id);
        dbRecruiter.Name.Should().Be("Inna");
        dbRecruiter.Phone.Should().Be("+380998887766");
        dbRecruiter.Notes.Should().Be("Notes");
    }
}
