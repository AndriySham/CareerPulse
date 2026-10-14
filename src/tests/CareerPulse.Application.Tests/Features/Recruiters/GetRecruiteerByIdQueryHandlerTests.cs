using CareerPulse.Application.DTOs.Recruiters;
using CareerPulse.Application.Features.Recruiters.Queries.GetRecruiterById;
using CareerPulse.Application.Tests.TestHelpers;
using CareerPulse.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace CareerPulse.Application.Tests.Features.Recruiters;

public class GetRecruiteerByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenRecruiterExists_ShouldReturnMappedRecruiterDto()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var company = Company.Create("Epum");
        var recruiter = Recruiter.Create(company.Id, "Inna", "+380998887766", "inna@epum.com", "https://linkedin.com/in/inna", "https://t.me/inna", "Notes");
        context.Companies.Add(company);
        context.Recruiters.Add(recruiter);
        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var handler = new GetRecruiterByIdQueryHandler(context);
        var query = new GetRecruiterByIdQuery(company.Id, recruiter.Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(new RecruiterDto
        {
            Id = recruiter.Id,
            CompanyId = company.Id,
            Name = "Inna",
            Phone = "+380998887766",
            Email = "inna@epum.com",
            LinkedInUrl = "https://linkedin.com/in/inna",
            TelegramUrl = "https://t.me/inna",
            Notes = "Notes"
        });
    }

    [Fact]
    public async Task Handle_WhenNothingExists_ShouldReturnNull()
    {
        // Arange
        using var context = TestDbContext.CreateInMemory();

        var handler = new GetRecruiterByIdQueryHandler(context);
        var query = new GetRecruiterByIdQuery(Guid.NewGuid(), Guid.NewGuid());

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WhenRecruiterBelongsToAnotherCompany_ShouldReturnNull()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var company = Company.Create("Epum");
        var anotherCompany = Company.Create("SoftServe");
        var recruiter = Recruiter.Create(anotherCompany.Id, "Inna");
        context.Companies.AddRange(company, anotherCompany);
        context.Recruiters.Add(recruiter);
        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var handler = new GetRecruiterByIdQueryHandler(context);

        // Act
        var withWrongCompany = await handler.Handle(
            new GetRecruiterByIdQuery(company.Id, recruiter.Id), CancellationToken.None);
        var withCorrectCompany = await handler.Handle(
            new GetRecruiterByIdQuery(anotherCompany.Id, recruiter.Id), CancellationToken.None);

        // Assert
        withWrongCompany.Should().BeNull();
        withCorrectCompany.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WhenCompanyExistsButRecruiterDoesNot_ShouldReturnNull()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var company = Company.Create("Epum");
        context.Companies.Add(company);
        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var handler = new GetRecruiterByIdQueryHandler(context);
        var query = new GetRecruiterByIdQuery(company.Id, Guid.NewGuid());

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}
