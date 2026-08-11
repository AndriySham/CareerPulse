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
        // Arrange
        var resumeId = Guid.NewGuid();
        var personalInfo = CreateValidPersonalInfo();

        // Act
        var revision = ResumeRevision.CreateDraft(
            resumeId,
            personalInfo,
            "Software Developer");

        // Assert
        revision.Id.Should().NotBeEmpty();
        revision.ResumeId.Should().Be(resumeId);
        revision.Status.Should().Be(RevisionStatus.Draft);
        revision.Version.Should().Be(1);
        revision.ParentRevisionId.Should().BeNull();
        revision.ProfessionalSummary.Should().Be("Software Developer");
        revision.Skills.Should().BeEmpty();
        revision.FileReference.Should().BeNull();
    }

    [Fact]
    public void CreateDraft_WhenResumeIdIsEmpty_ShouldThrowDomainException()
    {
        // Arrange & Act
        var act = () => ResumeRevision.CreateDraft(Guid.Empty, CreateValidPersonalInfo(), "Summary");

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("ResumeId is required for ResumeRevision.");
    }

    [Fact]
    public void MarkAsApplied_WhenStatusIsDraft_ShouldTransitionToApplied()
    {
        // Arrange
        var revision = ResumeRevision.CreateDraft(Guid.NewGuid(), CreateValidPersonalInfo(), "Summary");

        // Act
        revision.MarkAsApplied();

        // Assert
        revision.Status.Should().Be(RevisionStatus.Applied);
    }

    [Fact]
    public void MarkAsApplied_WhenStatusIsAlreadyApplied_ShouldThrowDomainException()
    {
        // Arrange
        var revision = ResumeRevision.CreateDraft(Guid.NewGuid(), CreateValidPersonalInfo(), "Summary");
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
        var revision = ResumeRevision.CreateDraft(Guid.NewGuid(), CreateValidPersonalInfo(), "Summary");
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
        var revision = ResumeRevision.CreateDraft(Guid.NewGuid(), CreateValidPersonalInfo(), "Summary");
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
        var revision = ResumeRevision.CreateDraft(Guid.NewGuid(), CreateValidPersonalInfo(), "Summary");
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
        var revision = ResumeRevision.CreateDraft(Guid.NewGuid(), CreateValidPersonalInfo(), "Summary");
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
        var revision = ResumeRevision.CreateDraft(Guid.NewGuid(), CreateValidPersonalInfo(), "Summary");
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
        var revision = ResumeRevision.CreateDraft(Guid.NewGuid(), CreateValidPersonalInfo(), "Summary");
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
        var revision = ResumeRevision.CreateDraft(Guid.NewGuid(), CreateValidPersonalInfo(), "Summary");
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
        var resumeId = Guid.NewGuid();
        var original = ResumeRevision.CreateDraft(resumeId, CreateValidPersonalInfo(), "Original Summary");
        var skillId = Guid.NewGuid();
        original.AddSkill(skillId, 4);
        original.SetFileReference("resumes/v1.pdf");
        original.MarkAsApplied();

        // Act
        var spawned = original.SpawnNewVersion();

        // Assert
        spawned.Id.Should().NotBeEmpty();
        spawned.Id.Should().NotBe(original.Id);
        spawned.ResumeId.Should().Be(resumeId);
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
        var original = ResumeRevision.CreateDraft(Guid.NewGuid(), CreateValidPersonalInfo(), "Original Summary");
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
        var draft = ResumeRevision.CreateDraft(Guid.NewGuid(), CreateValidPersonalInfo(), "Draft Summary");

        // Act
        var act = () => draft.SpawnNewVersion();

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("Only an Applied ResumeRevision can spawn a new version.");
    }

    [Fact]
    public void Resume_CreateFirstRevision_ShouldCreateDraftWithResumeIdAndAddToRevisions()
    {
        // Arrange
        var resume = Resume.Create("John's Resume", ResumeTrack.FullStack, CareerLevel.Senior, "Software Developer");
        var personalInfo = CreateValidPersonalInfo();

        // Act
        var revision = resume.CreateFirstRevision("Software Developer", personalInfo);

        // Assert
        revision.ResumeId.Should().Be(resume.Id);
        revision.Version.Should().Be(1);
        revision.Status.Should().Be(RevisionStatus.Draft);
        resume.Revisions.Should().ContainSingle(r => r.Id == revision.Id);
    }

    [Fact]
    public void Resume_CreateFirstRevision_WhenFirstRevisionAlreadyExists_ShouldThrowDomainException()
    {
        // Arrange
        var resume = Resume.Create("John's Resume", ResumeTrack.FullStack, CareerLevel.Senior, "Software Developer");
        resume.CreateFirstRevision("First Summary", CreateValidPersonalInfo());

        // Act
        var act = () => resume.CreateFirstRevision("Second Summary", CreateValidPersonalInfo());

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("First revision already exists. Use SpawnNewVersion() on an existing revision.");
    }

    [Fact]
    public void Resume_SpawnRevision_WhenRevisionBelongsToDifferentResume_ShouldThrowDomainException()
    {
        // Arrange
        var resume1 = Resume.Create("Resume 1", ResumeTrack.Backend, CareerLevel.Senior, "Dev 1");
        var resume2 = Resume.Create("Resume 2", ResumeTrack.Frontend, CareerLevel.Middle, "Dev 2");

        var revision1 = resume1.CreateFirstRevision("Summary 1", CreateValidPersonalInfo());
        revision1.MarkAsApplied();

        // Act
        var act = () => resume2.SpawnRevision(revision1);

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage($"ResumeRevision {revision1.Id} does not belong to Resume {resume2.Id}.");
    }

    [Fact]
    public void Resume_SpawnRevision_WhenRevisionBelongsToSameResume_ShouldSpawnNewVersionAndAddToRevisions()
    {
        // Arrange
        var resume = Resume.Create("Resume 1", ResumeTrack.Backend, CareerLevel.Senior, "Dev 1");
        var revision1 = resume.CreateFirstRevision("Summary 1", CreateValidPersonalInfo());
        revision1.MarkAsApplied();

        // Act
        var revision2 = resume.SpawnRevision(revision1);

        // Assert
        revision2.ResumeId.Should().Be(resume.Id);
        revision2.Version.Should().Be(2);
        revision2.ParentRevisionId.Should().Be(revision1.Id);
        revision2.Status.Should().Be(RevisionStatus.Draft);
        resume.Revisions.Should().HaveCount(2);
        resume.Revisions.Should().Contain(revision2);
    }
}
