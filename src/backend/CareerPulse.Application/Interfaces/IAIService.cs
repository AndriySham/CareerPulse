using CareerPulse.Application.DTOs.AI;

namespace CareerPulse.Application.Interfaces;

/// <summary>
/// Advisory AI service interface.
/// ADR 008: AI is strictly advisory — never writes to the database.
/// Returns temporary DTOs for HITL (Human-in-the-Loop) user review.
/// </summary>
public interface IAIService
{
    /// <summary>
    /// Extracts structured resume data from a PDF stream.
    /// Returns an ephemeral DTO — never persisted directly.
    /// ADR 007: Field-level confidence scoring for HITL review.
    /// </summary>
    Task<ResumeImportResultDto> ExtractResumeFromPdfAsync(
        Stream pdfStream,
        CancellationToken ct = default);

    /// <summary>
    /// Generates a cover letter draft for the given vacancy description.
    /// Returns plain text — never persisted without explicit user confirmation.
    /// </summary>
    Task<string> GenerateCoverLetterDraftAsync(
        string resumeSummary,
        string vacancyDescription,
        CancellationToken ct = default);
}
