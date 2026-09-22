using CareerPulse.Application.Common.Mappings;
using CareerPulse.Application.DTOs.Languages;
using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Interfaces;
using CareerPulse.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.Languages.Commands.CreateLanguage;

/// <summary>
/// Create handler for creation a Language entity.
/// </summary>
public sealed class CreateLanguageCommandHandler
    : IRequestHandler<CreateLanguageCommand, LanguageDto>
{
    private readonly IApplicationDbContext _context;

    public CreateLanguageCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<LanguageDto> Handle(
        CreateLanguageCommand request,
        CancellationToken cancellationToken)
    {
        var resumeRevisionExists = await _context.ResumeRevisions
            .AnyAsync(r => r.Id == request.Dto.ResumeRevisionId, cancellationToken);
        if (!resumeRevisionExists)
        {
            throw new ResourceNotFoundException($"ResumeRevision with ID '{request.Dto.ResumeRevisionId}' was not found.");
        }

        var language = Language.Create(
            request.Dto.ResumeRevisionId,
            request.Dto.LanguageName,
            request.Dto.Proficiency);

        _context.Languages.Add(language);
        await _context.SaveChangesAsync(cancellationToken);

        return LanguageMapping.MapToDto(language);
    }
}
