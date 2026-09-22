using CareerPulse.Application.Common.Mappings;
using CareerPulse.Application.DTOs.Educations;
using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Interfaces;
using CareerPulse.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.Educations.Commands.CreateEducation;

/// <summary>
/// Command handler for creating an Education entity.
/// </summary>
public sealed class CreateEducationCommandHandler
    : IRequestHandler<CreateEducationCommand, EducationDto>
{
    private readonly IApplicationDbContext _context;

    public CreateEducationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<EducationDto> Handle(
        CreateEducationCommand request,
        CancellationToken cancellationToken)
    {
        var resumeRevisionExist = await _context.ResumeRevisions
            .AnyAsync(x => x.Id == request.Dto.ResumeRevisionId, cancellationToken);

        if (!resumeRevisionExist)
        {
            throw new ResourceNotFoundException($"ResumeRevision with ID '{request.Dto.ResumeRevisionId}' was not found.");
        }

        var education = Education.Create(
            request.Dto.ResumeRevisionId,
            request.Dto.InstitutionName,
            request.Dto.Description,
            request.Dto.StartYear,
            request.Dto.EndYear);

        _context.Educations.Add(education);
        await _context.SaveChangesAsync(cancellationToken);

        return EducationMapping.MapToDto(education);
    }
}
