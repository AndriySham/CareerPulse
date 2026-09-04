using CareerPulse.Application.DTOs.Resumes;
using CareerPulse.Domain.Entities;

namespace CareerPulse.Application.Common.Mappings;

public class ResumeRevisionMapping
{
    public static ResumeRevisionDto MapToDto(ResumeRevision revision)
    {
        return new ResumeRevisionDto
        {
            Id = revision.Id,
            ResumeId = revision.ResumeId,
            ResumeName = revision.Resume?.Name ?? string.Empty,
            Track = revision.Resume?.Track ?? Domain.Enums.ResumeTrack.Backend,
            CareerLevel = revision.Resume?.CareerLevel ?? Domain.Enums.CareerLevel.Middle,
            TargetRole = revision.Resume?.TargetRole ?? string.Empty,
            Status = revision.Status,
            PersonalInfo = revision.PersonalInfo,
            ProfessionalSummary = revision.ProfessionalSummary,
            FileReference = revision.FileReference,
            Version = revision.Version,
            ParentRevisionId = revision.ParentRevisionId,
            CreatedAt = revision.CreatedAt,
            UpdatedAt = revision.UpdatedAt,
            Skills = revision.Skills.Select(s => new ResumeRevisionSkillDto
            {
                MasterSkillId = s.MasterSkillId,
                SkillName = s.MasterSkill?.Name ?? string.Empty,
                Category = s.MasterSkill?.Category ?? Domain.Enums.SkillCategory.Other,
                ProficiencyLevel = s.ProficiencyLevel
            }).ToList()
        };
    }
}
