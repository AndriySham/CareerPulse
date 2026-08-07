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



fix

I reviewed the Lesson 24.2 implementation, specifically SpawnResumeVersionCommandHandler and the ResumeRevision domain entity.

The Handler implementation is correct and should NOT be rewritten.

However, there is one domain-level invariant that should be strengthened.

Current behavior:
ResumeRevision.SpawnNewVersion() can currently be called regardless of the current RevisionStatus.

According to ADR 005 and the intended Copy-on-Write workflow, a new version should only be spawned from an Applied (read-only) ResumeRevision.

Please make ONLY this minimal correction now:

1. Update ResumeRevision.SpawnNewVersion() in the Domain layer.

Add a domain guard:

- If Status != RevisionStatus.Applied, throw DomainException.
- The exception message should clearly state that only an Applied ResumeRevision can spawn a new version.

Keep the existing Copy-on-Write behavior unchanged:
- new Guid
- Status = Draft
- Version = current Version + 1
- ParentRevisionId = current Id
- copy PersonalInfo
- copy ProfessionalSummary
- FileReference = null
- new CreatedAt / UpdatedAt timestamps

2. Do NOT move this business rule into SpawnResumeVersionCommandHandler.

The invariant belongs inside the Domain Entity because ResumeRevision itself must protect this business rule regardless of which Application use case calls SpawnNewVersion().

3. Do NOT create or modify unit tests at this stage.

Unit testing is a separate planned phase: Lesson 24.3.

We will verify this invariant properly during Lesson 24.3 using xUnit.

4. Do NOT change SpawnResumeVersionCommandHandler unless the Domain change causes a compilation issue.

5. Do NOT change ADR.md or ARCHITECTURE.md unless the existing documentation contradicts this invariant. If the documentation is already consistent, leave it unchanged.

6. After the change, run:

dotnet build "src/backend/CareerPulse.sln" -c Release

Fix only compilation errors caused by this change.

7. At completion, report:
- files modified;
- exact domain rule added;
- build verification result;
- whether any other files were changed.

Do not implement any other "24.2" functionality.
Do not start "".
Do not create tests yet.
