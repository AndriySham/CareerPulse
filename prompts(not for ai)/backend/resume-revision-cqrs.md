Read AGENTS.md, ADR.md, DOMAIN_MODEL.md, ARCHITECTURE.md, and MEMORY.md.

Execute Lesson 24.2: ResumeRevision CQRS Use Cases & Copy-on-Write Versioning.

Goal:
Implement CQRS use cases for ResumeRevision management adhering to ADR 005 (Draft Immutability & Copy-on-Write) and ADR 006 (Skill Normalization).

Requirements:

1. DTOs in CareerPulse.Application/DTOs/Resumes/:
   - ResumeRevisionDto (Id, Status, PersonalInfo, ProfessionalSummary, FileReference, Version, ParentRevisionId, CreatedAt, Skills)
   - CreateResumeDraftDto & UpdateResumeDraftDto

2. Use Cases in CareerPulse.Application/Features/Resumes/:
   - CreateResumeDraftCommand & Handler & FluentValidation Validator (Creates a new ResumeRevision with Status = Draft, Version = 1).
   - UpdateResumeDraftCommand & Handler & FluentValidation Validator (Updates summary & skills of an existing Draft; enforces EnsureDraft()).
   - SpawnResumeVersionCommand & Handler (ADR 005: Creates a new Draft revision version = parent.Version + 1 from an existing Applied revision via SpawnNewVersion()).
   - GetResumeRevisionsQuery & Handler (Retrieves all revisions or by ID with AsNoTracking()).

3. Follow all architectural constraints:
   - Clean Architecture via IApplicationDbContext
   - CQRS with MediatR & FluentValidation
   - Domain Exception mapping via RFC 7807

4. Do NOT implement Application, Interview, Company, or Vacancy features in this step.

5. After implementation:
   - run dotnet build "src/backend/CareerPulse.sln" -c Release;
   - fix all compilation errors.

6. At completion provide a summary containing:
   - files created & modified;
   - architectural decisions made;
   - recommended git commit message.

Stop after "name-task" fully completed and verified.
