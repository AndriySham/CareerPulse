using CareerPulse.Application.Common.Mappings;
using CareerPulse.Application.DTOs.Languages;
using CareerPulse.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.Languages.Queries.GetLanguages;

/// <summary>
/// Query handler for retrieving languages with AsNoTraking().
/// </summary>
public sealed class GetLanguagesQueryHandler
    : IRequestHandler<GetLanguagesQuery, List<LanguageDto>>
{
    private readonly IApplicationDbContext _context;

    public GetLanguagesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<LanguageDto>> Handle(
        GetLanguagesQuery request,
        CancellationToken cancellationToken)
    {
        var languages =  await _context.Languages
            .AsNoTracking()
            .Where(l => l.ResumeRevisionId == request.ResumeRevisionId)
            .ToListAsync(cancellationToken);

        return languages
            .Select(LanguageMapping.MapToDto)
            .ToList();
    }
}
