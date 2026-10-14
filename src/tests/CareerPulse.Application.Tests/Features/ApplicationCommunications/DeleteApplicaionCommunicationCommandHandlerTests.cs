using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Features.ApplicationCommunications.Commands.DeleteApplicationCommunication;
using CareerPulse.Application.Tests.TestHelpers;
using CareerPulse.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

using static CareerPulse.Application.Tests.TestHelpers.TestDataSeeder;
using AppEntity = CareerPulse.Domain.Entities.Application;

namespace CareerPulse.Application.Tests.Features.ApplicationCommunications;

public class DeleteApplicaionCommunicationCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenDataValid_ShouldDeleteApplicationCommunication()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var app = await SeedApplicationAsync(context);

        var occurredAt = DateTime.UtcNow.AddDays(-1);
        var communication = ApplicationCommunication.Create(app.Id, occurredAt, "Notes");
        context.ApplicationCommunications.Add(communication);
        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var handler = new DeleteApplicationCommunicationCommandHandler(context);
        var command = new DeleteApplicationCommunicationCommand(app.Id, communication.Id);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        context.ChangeTracker.Clear();

        var dbCommunication = await context.ApplicationCommunications.SingleOrDefaultAsync(x => x.Id == communication.Id, CancellationToken.None);
        dbCommunication.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WhenCommunicationDoesNotExist_ShouldThrowResourceNotFoundException()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var app = await SeedApplicationAsync(context);

        context.ChangeTracker.Clear();

        var nonExistentCommunicationId = Guid.NewGuid();

        var handler = new DeleteApplicationCommunicationCommandHandler(context);
        var command = new DeleteApplicationCommunicationCommand(app.Id, nonExistentCommunicationId);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ResourceNotFoundException>()
            .WithMessage($"ApplicationCommunication with application ID '{app.Id}' " +
                $"and communication ID '{nonExistentCommunicationId}' was not found.");
    }

    [Fact]
    public async Task Handle_WhenCommunicationBelongsToAnotherApplication_ShouldThrowAndKeepCommunication()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var app = await SeedApplicationAsync(context);
        var anotherApplication = AppEntity.Create(app.CompanyId, app.ResumeRevisionId, "DOU", app.VacancyId);

        var occurredAt = DateTime.UtcNow.AddDays(-1);
        var communication = ApplicationCommunication.Create(anotherApplication.Id, occurredAt, "Notes");
        context.Applications.Add(anotherApplication);
        context.ApplicationCommunications.Add(communication);

        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();


        var handler = new DeleteApplicationCommunicationCommandHandler(context);
        var command = new DeleteApplicationCommunicationCommand(app.Id, communication.Id);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ResourceNotFoundException>()
            .WithMessage($"ApplicationCommunication with application ID '{app.Id}' " +
                $"and communication ID '{communication.Id}' was not found.");

        context.ChangeTracker.Clear();

        var dbCommunication = await context.ApplicationCommunications.SingleAsync(x => x.Id == communication.Id);
        dbCommunication.ApplicationId.Should().Be(anotherApplication.Id);
        dbCommunication.OccurredAt.Should().Be(occurredAt);
        dbCommunication.Notes.Should().Be("Notes");
    }
}
