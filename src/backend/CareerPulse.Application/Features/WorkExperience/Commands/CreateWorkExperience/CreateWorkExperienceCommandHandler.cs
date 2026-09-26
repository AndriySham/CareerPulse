using CareerPulse.Application.Common.Mappings;
using CareerPulse.Application.DTOs.WorkExperiences;
using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.WorkExperience.Commands.CreateWorkExperience;

/// <summary>
/// Create handler for creation a WorkExperience entity.
/// </summary>
public sealed class CreateWorkExperienceCommandHandler
    : IRequestHandler<CreateWorkExperienceCommand, WorkExperienceDto>
{
    private readonly IApplicationDbContext _context;

    public CreateWorkExperienceCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<WorkExperienceDto> Handle(
        CreateWorkExperienceCommand request,
        CancellationToken cancellationToken)
    {
        var resumeRevisionExists = await _context.ResumeRevisions
            .AnyAsync(x => x.Id == request.Dto.ResumeRevisionId, cancellationToken);
        if (!resumeRevisionExists)
        {
            throw new ResourceNotFoundException($"ResumeRevision with ID '{request.Dto.ResumeRevisionId}' was not found.");
        }

        var workExperience = Domain.Entities.WorkExperience.Create(
           request.Dto.ResumeRevisionId,
           request.Dto.CompanyName,
           request.Dto.PositionTitle,
           request.Dto.StartMonth,
           request.Dto.StartYear,
           request.Dto.EndMonth,
           request.Dto.EndYear,
           request.Dto.IsCurrentJob,
           request.Dto.Description,
           request.Dto.Achievements,
           request.Dto.TechStack);

        _context.WorkExperiences.Add(workExperience);
        await _context.SaveChangesAsync(cancellationToken);

        return WorkExperienceMapping.MapToDto(workExperience);
    }
}
