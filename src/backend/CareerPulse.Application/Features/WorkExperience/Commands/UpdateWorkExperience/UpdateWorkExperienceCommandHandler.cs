using CareerPulse.Application.Common.Mappings;
using CareerPulse.Application.DTOs.WorkExperiences;
using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.WorkExperience.Commands.UpdateWorkExperience;

/// <summary>
/// Command handler for updating an existing WorkExperience Entity.
/// </summary>
public sealed class UpdateWorkExperienceCommandHandler
    : IRequestHandler<UpdateWorkExperienceCommand, WorkExperienceDto>
{
    private readonly IApplicationDbContext _context;

    public UpdateWorkExperienceCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<WorkExperienceDto> Handle(
        UpdateWorkExperienceCommand request,
        CancellationToken cancellationToken)
    {
        var workExperience = await _context.WorkExperiences
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (workExperience == null)
        {
            throw new ResourceNotFoundException($"WorkExperience with ID '{request.Id}' was not found.");
        }

        var isRevisionUsed = await _context.Applications
            .AnyAsync(x => x.ResumeRevisionId == workExperience.ResumeRevisionId, cancellationToken);
        if (isRevisionUsed)
        {
            throw new ConflictException("WorkExperience cannot be updated because its resume revision is already used in an application.");
        }

        workExperience.Update(
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

        await _context.SaveChangesAsync(cancellationToken);

        return WorkExperienceMapping.MapToDto(workExperience);
    }
}
