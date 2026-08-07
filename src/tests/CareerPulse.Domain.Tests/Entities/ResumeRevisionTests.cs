using CareerPulse.Domain.Entities;
using CareerPulse.Domain.Enums;
using CareerPulse.Domain.Exceptions;
using CareerPulse.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace CareerPulse.Domain.Tests.Entities;

public class ResumeRevisionTests
{
    private static PersonalInfo CreateValidPersonalInfo()
        => PersonalInfo.Create("John Doe", "john.doe@example.com");

    [Fact]
    public void CreateDraft_ShouldInitializeWithVersion1AndDraftStatus()
    {
        // Arrange & Act
        var revision = ResumeRevision.CreateDraft(CreateValidPersonalInfo(), "Software Developer");

        // Assert
        revision.Id.Should().NotBeEmpty();
        revision.Status.Should().Be(RevisionStatus.Draft);
        revision.Version.Should().Be(1);
        revision.ParentRevisionId.Should().BeNull();
        revision.ProfessionalSummary.Should().Be("Software Developer");
        revision.Skills.Should().BeEmpty();
        revision.FileReference.Should().BeNull();
    }

    [Fact]
    public void MarkAsApplied_WhenStatusIsDraft_ShouldTransitionToApplied()
    {
        // Arrange
        var revision = ResumeRevision.CreateDraft(CreateValidPersonalInfo(), "Summary");

        // Act
        revision.MarkAsApplied();

        // Assert
        revision.Status.Should().Be(RevisionStatus.Applied);
    }

    [Fact]
    public void MarkAsApplied_WhenStatusIsAlreadyApplied_ShouldThrowDomainException()
    {
        // Arrange
        var revision = ResumeRevision.CreateDraft(CreateValidPersonalInfo(), "Summary");
        revision.MarkAsApplied();

        // Act
        var act = () => revision.MarkAsApplied();

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("ResumeRevision is already in Applied (Read-Only) state.");
    }

    [Fact]
    public void UpdateSummary_WhenStatusIsApplied_ShouldThrowDomainException()
    {
        // Arrange
        var revision = ResumeRevision.CreateDraft(CreateValidPersonalInfo(), "Summary");
        revision.MarkAsApplied();

        // Act
        var act = () => revision.UpdateSummary("Updated Summary");

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("*Applied (Read-Only)*");
    }

    [Fact]
    public void UpdatePersonalInfo_WhenStatusIsApplied_ShouldThrowDomainException()
    {
        // Arrange
        var revision = ResumeRevision.CreateDraft(CreateValidPersonalInfo(), "Summary");
        revision.MarkAsApplied();

        // Act
        var act = () => revision.UpdatePersonalInfo(PersonalInfo.Create("Jane Doe", "jane@example.com"));

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("*Applied (Read-Only)*");
    }

    [Fact]
    public void SetFileReference_WhenStatusIsApplied_ShouldThrowDomainException()
    {
        // Arrange
        var revision = ResumeRevision.CreateDraft(CreateValidPersonalInfo(), "Summary");
        revision.MarkAsApplied();

        // Act
        var act = () => revision.SetFileReference("resumes/v1.pdf");

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("*Applied (Read-Only)*");
    }

    [Fact]
    public void AddSkill_WhenStatusIsApplied_ShouldThrowDomainException()
    {
        // Arrange
        var revision = ResumeRevision.CreateDraft(CreateValidPersonalInfo(), "Summary");
        revision.MarkAsApplied();

        // Act
        var act = () => revision.AddSkill(Guid.NewGuid(), 4);

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("*Applied (Read-Only)*");
    }

    [Fact]
    public void RemoveSkill_WhenStatusIsApplied_ShouldThrowDomainException()
    {
        // Arrange
        var revision = ResumeRevision.CreateDraft(CreateValidPersonalInfo(), "Summary");
        var skillId = Guid.NewGuid();
        revision.AddSkill(skillId, 3);
        revision.MarkAsApplied();

        // Act
        var act = () => revision.RemoveSkill(skillId);

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("*Applied (Read-Only)*");
    }

    [Fact]
    public void AddSkill_WhenDuplicateMasterSkillId_ShouldThrowDomainException()
    {
        // Arrange
        var revision = ResumeRevision.CreateDraft(CreateValidPersonalInfo(), "Summary");
        var skillId = Guid.NewGuid();
        revision.AddSkill(skillId, 4);

        // Act
        var act = () => revision.AddSkill(skillId, 5);

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("Skill already exists in this revision.");
    }

    [Fact]
    public void RemoveSkill_WhenSkillNotFound_ShouldThrowDomainException()
    {
        // Arrange
        var revision = ResumeRevision.CreateDraft(CreateValidPersonalInfo(), "Summary");
        var skillId = Guid.NewGuid();

        // Act
        var act = () => revision.RemoveSkill(skillId);

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage($"Skill {skillId} not found in this revision.");
    }

    [Fact]
    public void SpawnNewVersion_WhenStatusIsApplied_ShouldCreateDraftWithIncrementedVersionAndParentId()
    {
        // Arrange
        var original = ResumeRevision.CreateDraft(CreateValidPersonalInfo(), "Original Summary");
        var skillId = Guid.NewGuid();
        original.AddSkill(skillId, 4);
        original.SetFileReference("resumes/v1.pdf");
        original.MarkAsApplied();

        // Act
        var spawned = original.SpawnNewVersion();

        // Assert
        spawned.Id.Should().NotBeEmpty();
        spawned.Id.Should().NotBe(original.Id);
        spawned.Status.Should().Be(RevisionStatus.Draft);
        spawned.Version.Should().Be(2);
        spawned.ParentRevisionId.Should().Be(original.Id);
        spawned.PersonalInfo.Should().Be(original.PersonalInfo);
        spawned.ProfessionalSummary.Should().Be(original.ProfessionalSummary);
        spawned.FileReference.Should().BeNull(); // FileReference reset per ADR 005
    }

    [Fact]
    public void SpawnNewVersion_WhenStatusIsApplied_ShouldLeaveOriginalRevisionUntouched()
    {
        // Arrange
        var original = ResumeRevision.CreateDraft(CreateValidPersonalInfo(), "Original Summary");
        original.SetFileReference("resumes/v1.pdf");
        original.MarkAsApplied();

        // Act
        _ = original.SpawnNewVersion();

        // Assert
        original.Status.Should().Be(RevisionStatus.Applied);
        original.Version.Should().Be(1);
        original.FileReference.Should().Be("resumes/v1.pdf");
    }

    [Fact]
    public void SpawnNewVersion_WhenStatusIsDraft_ShouldThrowDomainException()
    {
        // Arrange
        var draft = ResumeRevision.CreateDraft(CreateValidPersonalInfo(), "Draft Summary");

        // Act
        var act = () => draft.SpawnNewVersion();

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("Only an Applied ResumeRevision can spawn a new version.");
    }
}
