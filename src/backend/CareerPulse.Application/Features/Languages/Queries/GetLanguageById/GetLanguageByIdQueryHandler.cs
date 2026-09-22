using CareerPulse.Application.Common.Mappings;
using CareerPulse.Application.DTOs.Languages;
using CareerPulse.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.Languages.Queries.GetLanguageById;

/// <summary>
/// Query handler for retrieving a single Language by ID with AsNoTracking().
/// </summary>
public sealed class GetLanguageByIdQueryHandler
    : IRequestHandler<GetLanguageByIdQuery, LanguageDto?>
{
    private readonly IApplicationDbContext _context;

    public GetLanguageByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<LanguageDto?> Handle(
        GetLanguageByIdQuery request,
        CancellationToken cancellationToken)
    {
        var language = await _context.Languages
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == request.Id, cancellationToken);

        return language is null ? null : LanguageMapping.MapToDto(language);
    }
}
