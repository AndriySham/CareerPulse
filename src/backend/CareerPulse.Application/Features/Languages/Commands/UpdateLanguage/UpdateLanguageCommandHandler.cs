using CareerPulse.Application.Common.Mappings;
using CareerPulse.Application.DTOs.Languages;
using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.Languages.Commands.UpdateLanguage;

/// <summary>
/// Command handler for an existing Language Entity.
/// </summary>
public sealed class UpdateLanguageCommandHandler
    : IRequestHandler<UpdateLanguageCommand, LanguageDto>
{
    private readonly IApplicationDbContext _context;

    public UpdateLanguageCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<LanguageDto> Handle(
        UpdateLanguageCommand request,
        CancellationToken cancellationToken)
    {
        var language = await _context.Languages
            .FirstOrDefaultAsync(l => l.Id == request.Id, cancellationToken);
        if (language == null)
        {
            throw new ResourceNotFoundException($"Language with ID '{request.Id}' was not found.");
        }

        var isRevisionUsed = await _context.Applications
            .AnyAsync(r => r.ResumeRevisionId == language.ResumeRevisionId, cancellationToken);
        if (isRevisionUsed)
        {
            throw new ConflictException("Language cannot be updated because its resume revision is already used in an application.");
        }

        language.Update(request.Dto.LanguageName, request.Dto.Proficiency);
        await _context.SaveChangesAsync(cancellationToken);

        return LanguageMapping.MapToDto(language);
    }
}
