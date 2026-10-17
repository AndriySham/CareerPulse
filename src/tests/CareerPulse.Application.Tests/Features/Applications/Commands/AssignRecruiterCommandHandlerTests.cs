using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Features.Applications.Commands.AssignRecruiter;
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

public class AssignRecruiterCommandHandlerTests
{
    private static (Company company, ResumeRevision revision, AppEntity app, Recruiter recruiter) SeedApplicationWithRecruiter(
        CareerPulseDbContext context)
    {
        var company = Company.Create("TargetCorp");
        var personalInfo = PersonalInfo.Create("John Candidate", "john@example.com");
        var resume = Resume.Create("John's Resume", ResumeTrack.Backend, CareerLevel.Middle, "Backend Dev");
        var revision = resume.CreateFirstRevision("Backend Dev", personalInfo);

        context.Companies.Add(company);
        context.Resumes.Add(resume);
        context.ResumeRevisions.Add(revision);
        context.SaveChanges();

        var recruiter = Recruiter.Create(
            company.Id,
            "Jane Recruiter",
            "+1234567890",
            "jane@targetcorp.com",
            "https://linkedin.com/in/janerecruiter",
            "https://t.me/janerecruiter",
            "Top tech recruiter");

        context.Recruiters.Add(recruiter);
        context.SaveChanges();

        var app = AppEntity.Create(company.Id, revision.Id, "LinkedIn");
        context.Applications.Add(app);
        context.SaveChanges();

        return (company, revision, app, recruiter);
    }

    [Fact]
    public async Task Handle_ValidRecruiter_SameCompany_ShouldAssignRecruiterSuccessfully()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var (_, _, app, recruiter) = SeedApplicationWithRecruiter(context);

        var handler = new AssignRecruiterCommandHandler(context);
        var command = new AssignRecruiterCommand(app.Id, recruiter.Id);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.RecruiterId.Should().Be(recruiter.Id);
        result.Recruiter.Should().NotBeNull();
        result.Recruiter!.Name.Should().Be("Jane Recruiter");
        result.Recruiter.Email.Should().Be("jane@targetcorp.com");

        var updatedInDb = await context.Applications.Include(a => a.Recruiter).FirstAsync(a => a.Id == app.Id);
        updatedInDb.RecruiterId.Should().Be(recruiter.Id);
        updatedInDb.Recruiter.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_NullRecruiterId_ShouldUnassignRecruiterSuccessfully()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var (_, _, app, recruiter) = SeedApplicationWithRecruiter(context);
        app.AssignRecruiter(recruiter);
        context.SaveChanges();

        var handler = new AssignRecruiterCommandHandler(context);
        var command = new AssignRecruiterCommand(app.Id, null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.RecruiterId.Should().BeNull();
        result.Recruiter.Should().BeNull();

        var updatedInDb = await context.Applications.FirstAsync(a => a.Id == app.Id);
        updatedInDb.RecruiterId.Should().BeNull();
    }

    [Fact]
    public async Task Handle_EmptyGuidRecruiterId_ShouldUnassignRecruiterSuccessfully()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var (_, _, app, recruiter) = SeedApplicationWithRecruiter(context);
        app.AssignRecruiter(recruiter);
        context.SaveChanges();

        var handler = new AssignRecruiterCommandHandler(context);
        var command = new AssignRecruiterCommand(app.Id, Guid.Empty);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.RecruiterId.Should().BeNull();
        result.Recruiter.Should().BeNull();
    }

    [Fact]
    public async Task Handle_NonExistentApplication_ShouldThrowResourceNotFoundException()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var (_, _, _, recruiter) = SeedApplicationWithRecruiter(context);

        var handler = new AssignRecruiterCommandHandler(context);
        var command = new AssignRecruiterCommand(Guid.NewGuid(), recruiter.Id);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ResourceNotFoundException>();
    }

    [Fact]
    public async Task Handle_NonExistentRecruiter_ShouldThrowResourceNotFoundException()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var (_, _, app, _) = SeedApplicationWithRecruiter(context);

        var handler = new AssignRecruiterCommandHandler(context);
        var command = new AssignRecruiterCommand(app.Id, Guid.NewGuid());

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ResourceNotFoundException>();
    }

    [Fact]
    public async Task Handle_RecruiterBelongsToDifferentCompany_ShouldThrowDomainException()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var (_, _, app, _) = SeedApplicationWithRecruiter(context);

        var otherCompany = Company.Create("OtherCorp");
        context.Companies.Add(otherCompany);
        context.SaveChanges();

        var otherRecruiter = Recruiter.Create(otherCompany.Id, "Bob from OtherCorp");
        context.Recruiters.Add(otherRecruiter);
        context.SaveChanges();

        var handler = new AssignRecruiterCommandHandler(context);
        var command = new AssignRecruiterCommand(app.Id, otherRecruiter.Id);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage($"*{otherCompany.Id}*");
    }
}
