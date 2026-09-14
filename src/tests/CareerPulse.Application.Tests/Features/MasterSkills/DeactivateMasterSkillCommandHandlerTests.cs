using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Features.MasterSkills.Commands.DeactivateMasterSkill;
using CareerPulse.Application.Tests.TestHelpers;
using CareerPulse.Domain.Entities;
using CareerPulse.Domain.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CareerPulse.Application.Tests.Features.MasterSkills;

public class DeactivateMasterSkillCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidData_ShouldDeactivateMasterSkill()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var skill = MasterSkill.Create("C#", SkillCategory.ProgrammingLanguage);
        context.MasterSkills.Add(skill);
        await context.SaveChangesAsync(CancellationToken.None);

        var command = new DeactivateMasterSkillCommand(skill.Id);
        var handler = new DeactivateMasterSkillCommandHandler(context);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var dbSkill = await context.MasterSkills.FirstOrDefaultAsync(x => x.Id == skill.Id);

        dbSkill.Should().NotBeNull();
        dbSkill!.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WhenMasterSkillDoesNotExist_ShouldThrowResourceNotFoundException()
    {
        // Assert
        using var context = TestDbContext.CreateInMemory();

        var skillId = Guid.NewGuid();
        var command = new DeactivateMasterSkillCommand(skillId);
        var handler = new DeactivateMasterSkillCommandHandler(context);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ResourceNotFoundException>()
            .WithMessage($"MasterSkill with ID '{skillId}' was not found");
    }

    [Fact]
    public async Task Handle_WhenMasterSkillAlreadyDeactivated_ShouldRemainDeactivated()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var skill = MasterSkill.Create("C#", SkillCategory.ProgrammingLanguage);
        context.MasterSkills.Add(skill);
        skill.Deactivate();
        await context.SaveChangesAsync(CancellationToken.None);

        var command = new DeactivateMasterSkillCommand(skill.Id);
        var handler = new DeactivateMasterSkillCommandHandler(context);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var dbSkill = await context.MasterSkills.FirstOrDefaultAsync(x => x.Id == skill.Id);

        dbSkill.Should().NotBeNull();
        dbSkill!.IsActive.Should().BeFalse();
    }
}
