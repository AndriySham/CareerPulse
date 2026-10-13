using CareerPulse.Application.DTOs.ApplicationCommunications;
using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Features.ApplicationCommunications.Commands.CreateApplicationCommunication;
using CareerPulse.Application.Tests.TestHelpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

using static CareerPulse.Application.Tests.TestHelpers.TestDataSeeder;
using AppEntity = CareerPulse.Domain.Entities.Application;

namespace CareerPulse.Application.Tests.Features.ApplicationCommunications;

public class CreateApplicationCommunicationCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidData_ShouldPersistAllFields()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var app = await SeedApplicationAsync(context);

        var occurredAt = DateTime.UtcNow.AddHours(-2);
        var dto = new CreateApplicationCommunicationDto
        {
            OccurredAt = occurredAt,
            Notes = "Prepare system design",
        };

        var handler = new CreateApplicationCommunicationCommandHandler(context);
        var command = new CreateApplicationCommunicationCommand(app.Id, dto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.OccurredAt.Should().Be(occurredAt);
        result.Notes.Should().Be("Prepare system design");

        context.ChangeTracker.Clear();

        var dbCommunication = await context.ApplicationCommunications
            .SingleAsync(x => x.Id == result.Id);
        dbCommunication.OccurredAt.Should().Be(occurredAt);
        dbCommunication.Notes.Should().Be("Prepare system design");
    }

    [Fact]
    public async Task Handle_WhenApplicationDoesNotExist_ShouldThrowAndNotSaveCommunication()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var missingId = Guid.NewGuid();

        var occurredAt = DateTime.UtcNow.AddHours(-2);
        var dto = new CreateApplicationCommunicationDto
        {
            OccurredAt = occurredAt,
            Notes = " Prepare system design ",
        };
        var handler = new CreateApplicationCommunicationCommandHandler(context);
        var command = new CreateApplicationCommunicationCommand(missingId, dto);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ResourceNotFoundException>()
            .WithMessage($"Application with ID '{missingId}' was not found.");

        (await context.ApplicationCommunications.AnyAsync()).Should().BeFalse();
    }
}
