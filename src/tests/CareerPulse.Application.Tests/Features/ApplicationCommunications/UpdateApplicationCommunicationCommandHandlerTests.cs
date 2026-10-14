using CareerPulse.Application.DTOs.ApplicationCommunications;
using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Features.ApplicationCommunications.Commands.UpdateApplicationCommunication;
using CareerPulse.Application.Tests.TestHelpers;
using CareerPulse.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

using static CareerPulse.Application.Tests.TestHelpers.TestDataSeeder;

namespace CareerPulse.Application.Tests.Features.ApplicationCommunications;

public class UpdateApplicationCommunicationCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidData_ShouldUpdateApplicationCommunicationAndReturnDto()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var app = await SeedApplicationAsync(context);

        var occurredAt = DateTime.UtcNow.AddDays(-1);
        var communication = ApplicationCommunication.Create(app.Id, occurredAt, "Notes");
        context.ApplicationCommunications.Add(communication);
        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var newOccurredAt = DateTime.UtcNow;

        var dto = new UpdateApplicationCommunicationDto
        {
            OccurredAt = newOccurredAt,
            Notes = "Updated notes",
        };

        var handler = new UpdateApplicationCommunicationCommandHandler(context);
        var command = new UpdateApplicationCommunicationCommand(app.Id, communication.Id, dto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(communication.Id);
        result.OccurredAt.Should().Be(newOccurredAt);
        result.Notes.Should().Be("Updated notes");

        context.ChangeTracker.Clear();

        var dbCommunication = await context.ApplicationCommunications
            .SingleAsync(x => x.Id == communication.Id, CancellationToken.None);

        dbCommunication.Id.Should().Be(result.Id);
        dbCommunication.OccurredAt.Should().Be(newOccurredAt);
        dbCommunication.Notes.Should().Be("Updated notes");
        dbCommunication.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WhenApplicationDoesNotExist_ShouldThrowResourceNotFoundException()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var newOccurredAt = DateTime.UtcNow;
        var dto = new UpdateApplicationCommunicationDto
        {
            OccurredAt = newOccurredAt,
            Notes = "Updated notes",
        };
        var nonExistentApplicationId = Guid.NewGuid();
        var nonExistentAppCommunicationId = Guid.NewGuid();

        var handler = new UpdateApplicationCommunicationCommandHandler(context);
        var command = new UpdateApplicationCommunicationCommand(nonExistentApplicationId, nonExistentAppCommunicationId, dto);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ResourceNotFoundException>()
            .WithMessage($"ApplicationCommunication with application ID '{nonExistentApplicationId}' " +
                $"and applicationCommunication ID '{nonExistentAppCommunicationId}' was not found.");
    }

    [Fact]
    public async Task Handle_WhenApplicationCommunicationBelongsToAnotherApplication_ShouldThrowAndNotChangeData()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var app = await SeedApplicationAsync(context);
        
        var otherApplication = Domain.Entities.Application.Create(app.CompanyId, app.ResumeRevisionId, "DOU", app.VacancyId);
        var occurredAt = DateTime.UtcNow;
        var communication = ApplicationCommunication.Create(app.Id, occurredAt, "Notes");
        
        context.Applications.Add(otherApplication);
        context.ApplicationCommunications.Add(communication);
        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();


        var dto = new UpdateApplicationCommunicationDto
        {
            OccurredAt = DateTime.UtcNow,
            Notes = "Updated notes",
        };

        var handler = new UpdateApplicationCommunicationCommandHandler(context);
        var command = new UpdateApplicationCommunicationCommand(otherApplication.Id, communication.Id, dto);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ResourceNotFoundException>();

        context.ChangeTracker.Clear();

        var dbCommunication = await context.ApplicationCommunications.SingleAsync(x => x.Id == communication.Id);
        dbCommunication.OccurredAt.Should().Be(occurredAt);
        dbCommunication.Notes.Should().Be("Notes");
    }

    [Fact]
    public async Task Handle_WithWhitespaceAroundNotes_ShouldTrimNotes()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var app = await SeedApplicationAsync(context);

        var occurredAt = DateTime.UtcNow.AddDays(-1);
        var communication = ApplicationCommunication.Create(app.Id, occurredAt, "Original notes");

        context.ApplicationCommunications.Add(communication);
        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var newOccurredAt = DateTime.UtcNow;
        var dto = new UpdateApplicationCommunicationDto
        {
            OccurredAt = newOccurredAt,
            Notes = "  Updated notes  "
        };

        var handler = new UpdateApplicationCommunicationCommandHandler(context);
        var command = new UpdateApplicationCommunicationCommand(app.Id, communication.Id, dto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Notes.Should().Be("Updated notes");
        result.OccurredAt.Should().Be(newOccurredAt);

        context.ChangeTracker.Clear();

        var dbCommunication = await context.ApplicationCommunications
            .SingleAsync(x => x.Id == communication.Id);

        dbCommunication.Notes.Should().Be("Updated notes");
        dbCommunication.OccurredAt.Should().Be(newOccurredAt);
        dbCommunication.UpdatedAt.Should().NotBeNull();
    }
}
