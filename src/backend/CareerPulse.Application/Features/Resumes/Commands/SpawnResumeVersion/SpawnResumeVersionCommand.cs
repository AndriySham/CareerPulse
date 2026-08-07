using CareerPulse.Application.DTOs.Resumes;
using MediatR;

namespace CareerPulse.Application.Features.Resumes.Commands.SpawnResumeVersion;

/// <summary>
/// Command to create a new Draft revision version = parent.Version + 1 from an existing revision.
/// Implements Copy-on-Write pattern per ADR 005.
/// </summary>
public sealed record SpawnResumeVersionCommand(Guid ParentRevisionId) : IRequest<ResumeRevisionDto>;
