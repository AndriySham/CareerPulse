using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Features.Recruiters.Commands.DeleteRecruiter;
using CareerPulse.Application.Tests.TestHelpers;
using CareerPulse.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

using static CareerPulse.Application.Tests.TestHelpers.TestDataSeeder;
using AppEntity = CareerPulse.Domain.Entities.Application;

namespace CareerPulse.Application.Tests.Features.Recruiters;

public class DeleteRecruiterCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenDataValid_ShouldDeleteRecruiter()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var company = Company.Create("Epum");
        var recruiter = Recruiter.Create(company.Id, "Inna", "+380998887766", "inna@epum.com", "https://linkedin.com/in/inna", "https://t.me/inna", "Notes");
        context.Companies.Add(company);
        context.Recruiters.Add(recruiter);
        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var handler = new DeleteRecruiterCommandHandler(context);
        var command = new DeleteRecruiterCommand(company.Id, recruiter.Id);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        context.ChangeTracker.Clear();

        var dbRecruiter = await context.Recruiters.AnyAsync(x => x.Id == recruiter.Id, CancellationToken.None);
        dbRecruiter.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WhenRecruiterDoesNotExist_ShouldThrowAndKeepOtherRecruiters()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var company = Company.Create("Epum");
        var existing = Recruiter.Create(company.Id, "Inna");
        context.Companies.Add(company);
        context.Recruiters.Add(existing);
        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var nonExistentRecruiterId = Guid.NewGuid();

        var handler = new DeleteRecruiterCommandHandler(context);
        var command = new DeleteRecruiterCommand(company.Id, nonExistentRecruiterId);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ResourceNotFoundException>()
            .WithMessage($"Resource with Company ID '{company.Id}' and Recruiter ID '{nonExistentRecruiterId}' was not found.");

        context.ChangeTracker.Clear();

        var dbRecruiter = await context.Recruiters.AnyAsync(x => x.Id == existing.Id);
        dbRecruiter.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenRecruiterBelongsToAnotherCompany_ShouldThrowAndKeepRecruiter()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var company = Company.Create("Epum");
        var anotherCompany = Company.Create("SoftServe");
        var recruiter = Recruiter.Create(company.Id, "Inna", "+380998887766", "inna@epum.com", "https://linkedin.com/in/inna", "https://t.me/inna", "Notes");
        context.Companies.AddRange(company, anotherCompany);
        context.Recruiters.Add(recruiter);
        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var handler = new DeleteRecruiterCommandHandler(context);
        var command = new DeleteRecruiterCommand(anotherCompany.Id, recruiter.Id);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ResourceNotFoundException>()
            .WithMessage($"Resource with Company ID '{anotherCompany.Id}' and Recruiter ID '{recruiter.Id}' was not found.");

        context.ChangeTracker.Clear();

        var dbRecruiter = await context.Recruiters.SingleAsync(x => x.Id == recruiter.Id);
        dbRecruiter.CompanyId.Should().Be(company.Id);
    }

    [Fact]
    public async Task Handle_ShouldNotDeleteOtherRecruitersOfSameCompany()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var company = Company.Create("Epum");
        var toDelete = Recruiter.Create(company.Id, "Inna");
        var sibling = Recruiter.Create(company.Id, "Maria");
        context.Companies.Add(company);
        context.Recruiters.AddRange(toDelete, sibling);
        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var handler = new DeleteRecruiterCommandHandler(context);
        var command = new DeleteRecruiterCommand(company.Id, toDelete.Id);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        context.ChangeTracker.Clear();

        var remaining = await context.Recruiters.Where(x => x.CompanyId == company.Id).ToListAsync();
        remaining.Should().ContainSingle().Which.Id.Should().Be(sibling.Id);
    }

    [Fact]
    public async Task Handle_WhenRecruiterIsLinkedToApplication_ShouldThrowConflictAndKeepRecruiterAndLink()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var app = await SeedApplicationAsync(context);
        var recruiter = Recruiter.Create(app.CompanyId, "Inna", "+380998887766", "inna@epum.com", "https://linkedin.com/in/inna", "https://t.me/inna", "Notes");
        context.Recruiters.Add(recruiter);
        context.Entry(app).Property(a => a.RecruiterId).CurrentValue = recruiter.Id;
        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var handler = new DeleteRecruiterCommandHandler(context);
        var command = new DeleteRecruiterCommand(app.CompanyId, recruiter.Id);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage($"Recruiter '{recruiter.Id}' is linked to existing applications and cannot be deleted.");

        context.ChangeTracker.Clear();

        (await context.Recruiters.AnyAsync(x => x.Id == recruiter.Id)).Should().BeTrue();

        var savedApp = await context.Applications.SingleAsync(x => x.Id == app.Id);
        savedApp.RecruiterId.Should().Be(recruiter.Id);
    }
}
