using CareerPulse.Application.DTOs.ApplicationCommunications;
using CareerPulse.Application.Features.ApplicationCommunications.Queries.GetApplicationCommunicationById;
using CareerPulse.Application.Tests.TestHelpers;
using CareerPulse.Domain.Entities;
using FluentAssertions;
using Xunit;

using static CareerPulse.Application.Tests.TestHelpers.TestDataSeeder;
using AppEntity = CareerPulse.Domain.Entities.Application;

namespace CareerPulse.Application.Tests.Features;

public class GetApplicationCommunicationByIdQueryTests
{
    [Fact]
    public async Task Handle_WhenApplicationCommunicationExists_ShouldReturnMappedDto()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var app = await SeedApplicationAsync(context);

        var occurredAt = DateTime.UtcNow.AddHours(-2);
        var communication = ApplicationCommunication.Create(app.Id, occurredAt, "First communication with HR.");
        context.ApplicationCommunications.Add(communication);
        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var handler = new GetApplicationCommunicationByIdQueryHandler(context);
        var query = new GetApplicationCommunicationByIdQuery(app.Id, communication.Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(new ApplicationCommunicationDto
        {
            Id = communication.Id,
            OccurredAt = occurredAt,
            Notes = "First communication with HR."
        });
    }

    [Fact]
    public async Task Handle_WhenApplicationCommunicationDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var handler = new GetApplicationCommunicationByIdQueryHandler(context);
        var query = new GetApplicationCommunicationByIdQuery(Guid.NewGuid(), Guid.NewGuid());

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WhenApplicationCommunicationBelongsToAnotherApplication_ShouldReturnNull()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var app = await SeedApplicationAsync(context);

        var occurredAt = DateTime.UtcNow.AddHours(-2);
        var appCom = ApplicationCommunication.Create(app.Id, occurredAt, "First communication with HR.");

        var anotherApplication = AppEntity.Create(app.CompanyId, app.ResumeRevisionId, "Djinni", app.VacancyId);

        context.Applications.Add(anotherApplication);
        context.ApplicationCommunications.Add(appCom);
        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var handler = new GetApplicationCommunicationByIdQueryHandler(context);
        var query = new GetApplicationCommunicationByIdQuery(anotherApplication.Id, appCom.Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WhenCommunicationBelongsToAnotherApplication_ShouldReturnNull()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var app = await SeedApplicationAsync(context);
        var anotherApplication = AppEntity.Create(app.CompanyId, app.ResumeRevisionId, "Djinni", app.VacancyId);
        var communication = ApplicationCommunication.Create(
            app.Id, DateTime.UtcNow.AddHours(-2), "First communication with HR.");

        context.Applications.Add(anotherApplication);
        context.ApplicationCommunications.Add(communication);
        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var handler = new GetApplicationCommunicationByIdQueryHandler(context);

        // Act
        var withWrongApplication = await handler.Handle(
            new GetApplicationCommunicationByIdQuery(anotherApplication.Id, communication.Id),
            CancellationToken.None);
        var withCorrectApplication = await handler.Handle(
            new GetApplicationCommunicationByIdQuery(app.Id, communication.Id),
            CancellationToken.None);

        // Assert
        withWrongApplication.Should().BeNull();
        withCorrectApplication.Should().NotBeNull();
    }
}
