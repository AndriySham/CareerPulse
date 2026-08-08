using CareerPulse.Application.DTOs.Vacancies;
using CareerPulse.Application.Interfaces;
using CareerPulse.Domain.Entities;
using CareerPulse.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.Vacancies.Commands.CreateVacancy;

/// <summary>
/// Command handler for creating a Vacancy entity.
/// </summary>
public sealed class CreateVacancyCommandHandler
    : IRequestHandler<CreateVacancyCommand, VacancyDto>
{
    private readonly IApplicationDbContext _context;

    public CreateVacancyCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<VacancyDto> Handle(
        CreateVacancyCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var trimmedTitle = dto.Title?.Trim() ?? string.Empty;

        var companyExists = await _context.Companies
            .AnyAsync(c => c.Id == dto.CompanyId, cancellationToken);

        if (!companyExists)
        {
            throw new DomainException($"Company with ID '{dto.CompanyId}' was not found.");
        }

        var vacancy = Vacancy.Create(dto.CompanyId, trimmedTitle, dto.Description, dto.Url, dto.PostedAt);

        _context.Vacancies.Add(vacancy);
        await _context.SaveChangesAsync(cancellationToken);

        return MapToDto(vacancy);
    }

    public static VacancyDto MapToDto(Vacancy vacancy) => new()
    {
        Id = vacancy.Id,
        CompanyId = vacancy.CompanyId,
        Title = vacancy.Title,
        Description = vacancy.Description,
        Url = vacancy.Url,
        PostedAt = vacancy.PostedAt,
        CreatedAt = vacancy.CreatedAt,
        UpdatedAt = vacancy.UpdatedAt
    };
}
