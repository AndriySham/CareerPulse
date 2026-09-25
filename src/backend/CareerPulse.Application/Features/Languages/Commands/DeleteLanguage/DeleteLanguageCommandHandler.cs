using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.Languages.Commands.DeleteLanguage;

/// <summary>
/// Command handlere to delete a Language entity.
/// </summary>
public sealed class DeleteLanguageCommandHandler
    : IRequestHandler<DeleteLanguageCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteLanguageCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(
        DeleteLanguageCommand request,
        CancellationToken cancellationToken)
    {
        var language = await _context.Languages
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (language == null)
        {
            throw new ResourceNotFoundException($"Language with ID '{request.Id}' was not found.");
        }

        var isRevisionUsed = await _context.Applications
            .AnyAsync(x => x.ResumeRevisionId == language.ResumeRevisionId, cancellationToken);
        if (isRevisionUsed)
        {
            throw new ConflictException("Language cannot be deleted because ins resume revision is already used in an application.");
        }

        _context.Languages.Remove(language);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
