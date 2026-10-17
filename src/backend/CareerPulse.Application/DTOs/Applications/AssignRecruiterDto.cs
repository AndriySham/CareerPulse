namespace CareerPulse.Application.DTOs.Applications;

/// <summary>
/// DTO for assigning or unassigning a recruiter to an existing application.
/// </summary>
public sealed class AssignRecruiterDto
{
    public Guid? RecruiterId { get; init; }
}
