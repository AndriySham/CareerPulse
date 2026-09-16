using CareerPulse.Application.Common.Mappings;
using CareerPulse.Application.DTOs.Educations;
using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.Educations.Commands.UpdateEducation;

/// <summary>
/// Command handler for updating an existing Education entity.
/// </summary>
public sealed class UpdateEducationCommandHandler
    : IRequestHandler<UpdateEducationCommand, EducationDto>
{
    private readonly IApplicationDbContext _context;

    public UpdateEducationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<EducationDto> Handle(
        UpdateEducationCommand request,
        CancellationToken cancellationToken)
    {
        var education = await _context.Educations
            .FirstOrDefaultAsync(x => x.Id == request.Id &&
                x.ResumeRevisionId == request.Dto.ResumeRevisionId
            , cancellationToken);
        
        if (education == null)
        {
            throw new ResourceNotFoundException($"Education with ID '{request.Id}' was not found.");
        }

        education.Update(
            request.Dto.InstitutionName,
            request.Dto.Description,
            request.Dto.StartYear,
            request.Dto.EndYear);

        await _context.SaveChangesAsync(cancellationToken);

        return EducationMapping.MapToDto(education);
    }
}
