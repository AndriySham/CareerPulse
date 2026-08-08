using CareerPulse.Application.DTOs.Applications;
using CareerPulse.Application.Features.Applications.Commands.ChangeApplicationStatus;
using CareerPulse.Application.Tests.TestHelpers;
using CareerPulse.Domain.Entities;
using CareerPulse.Domain.Enums;
using CareerPulse.Domain.Exceptions;
using CareerPulse.Domain.ValueObjects;
using CareerPulse.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

using AppEntity = CareerPulse.Domain.Entities.Application;

namespace CareerPulse.Application.Tests.Features.Applications.Commands;

public class ChangeApplicationStatusCommandHandlerTests
{
    private static (Company company, ResumeRevision revision, AppEntity app) SeedApplication(
        CareerPulseDbContext context,
        ApplicationStatus initialStatus = ApplicationStatus.Draft)
    {
        var company = Company.Create("StatusCorp");
        var personalInfo = PersonalInfo.Create("Alice Smith", "alice@example.com");
        var revision = ResumeRevision.CreateDraft(personalInfo, "Dotnet Dev");

        context.Companies.Add(company);
        context.ResumeRevisions.Add(revision);
        context.SaveChanges();

        var app = AppEntity.Create(company.Id, revision.Id, "LinkedIn");
        if (initialStatus != ApplicationStatus.Draft)
        {
            app.TransitionTo(initialStatus);
            if (initialStatus == ApplicationStatus.Applied)
            {
                revision.MarkAsApplied();
            }
        }
        context.Applications.Add(app);
        context.SaveChanges();

        return (company, revision, app);
    }

    [Fact]
    public async Task Handle_ValidTransition_DraftToApplied_ShouldUpdateStatusAndMarkRevisionApplied()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var (_, revision, app) = SeedApplication(context, ApplicationStatus.Draft);

        var handler = new ChangeApplicationStatusCommandHandler(context);
        var dto = new ChangeApplicationStatusDto
        {
            NewStatus = ApplicationStatus.Applied,
            Notes = "Submitted via website"
        };
        var command = new ChangeApplicationStatusCommand(app.Id, dto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(ApplicationStatus.Applied);
        result.Notes.Should().Be("Submitted via website");

        var dbApp = await context.Applications.FirstOrDefaultAsync(a => a.Id == app.Id);
        dbApp!.Status.Should().Be(ApplicationStatus.Applied);

        var dbRevision = await context.ResumeRevisions.FirstOrDefaultAsync(r => r.Id == revision.Id);
        dbRevision!.Status.Should().Be(RevisionStatus.Applied);
    }

    [Fact]
    public async Task Handle_ValidTransition_AppliedToViewed_ShouldUpdateStatus()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var (_, _, app) = SeedApplication(context, ApplicationStatus.Applied);

        var handler = new ChangeApplicationStatusCommandHandler(context);
        var dto = new ChangeApplicationStatusDto
        {
            NewStatus = ApplicationStatus.Viewed
        };
        var command = new ChangeApplicationStatusCommand(app.Id, dto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.Should().Be(ApplicationStatus.Viewed);
        result.AllowedTransitions.Should().BeEquivalentTo(new[]
        {
            ApplicationStatus.HRInterview,
            ApplicationStatus.Rejected
        });
    }

    [Fact]
    public async Task Handle_InvalidTransition_DraftToHRInterview_ShouldThrowDomainException()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var (_, _, app) = SeedApplication(context, ApplicationStatus.Draft);

        var handler = new ChangeApplicationStatusCommandHandler(context);
        var dto = new ChangeApplicationStatusDto
        {
            NewStatus = ApplicationStatus.HRInterview
        };
        var command = new ChangeApplicationStatusCommand(app.Id, dto);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("*Invalid status transition*");
    }

    [Fact]
    public async Task Handle_WhenApplicationNotFound_ShouldThrowDomainException()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var handler = new ChangeApplicationStatusCommandHandler(context);
        var nonExistentId = Guid.NewGuid();

        var dto = new ChangeApplicationStatusDto { NewStatus = ApplicationStatus.Applied };
        var command = new ChangeApplicationStatusCommand(nonExistentId, dto);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage($"Application with ID {nonExistentId} was not found.");
    }
}
