# Architecture Decision Records (ADR) - CareerPulse

## ADR Index

| ADR | Title | Status |
|------|-------|--------|
| [ADR 001](#adr-001-database-choice--postgresql-in-docker) | Database Choice — PostgreSQL in Docker | Accepted |
| [ADR 002](#adr-002-single-source-of-truth--database-vs-json-files-on-disk) | Single Source of Truth — Database vs JSON Files | Accepted |
| [ADR 003](#adr-003-storage-abstraction-pattern--ifilestorage-provider) | Storage Abstraction Pattern — `IFileStorage` | Accepted |
| [ADR 004](#adr-004-backup--recovery-strategy--native-pg_dump-via-ibackupservice) | Backup & Recovery Strategy — Native `pg_dump` | Accepted |
| [ADR 005](#adr-005-resume-revision-lifecycle-immutability--dual-mode-creation) | Resume Revision Lifecycle, Immutability & Dual-Mode Creation | Accepted |
| [ADR 006](#adr-006-skill-normalization-pipeline-categorization--aliasing) | Skill Normalization Pipeline, Categorization & Aliasing | Accepted |
| [ADR 007](#adr-007-temporary-ai-import-dtos--field-level-confidence-scoring) | Temporary AI Import DTOs & Confidence Scoring | Accepted |
| [ADR 008](#adr-008-ai-responsibility-boundary--human-in-the-loop-enforcement) | AI Responsibility Boundary & Human-in-the-Loop | Accepted |
| [ADR 009](#adr-009-technology-stack-selection--backend-net-9--frontend-ecosystem) | Technology Stack Selection — Backend & Frontend Ecosystem | Accepted |

---

## ADR 001: Database Choice — PostgreSQL in Docker

**Status:** Accepted

### Context
The application requires a robust relational database for tracking complex domain relationships (`Application` → `Company` → `Vacancy` → `ResumeRevision` → `Skills`).

### Decision
Use PostgreSQL running inside a Docker container with persistent volumes for local development instead of SQLite.

### Consequences
- Full compatibility with production-grade Entity Framework Core features.
- Development environment closely matches production containers.
- Enhanced portfolio value demonstrating Docker Compose and PostgreSQL setup.

---

## ADR 002: Single Source of Truth — Database vs JSON Files on Disk

**Status:** Accepted

### Context
AI models require structured resume data. Storing duplicated `.json` files alongside `.pdf` files on disk introduces Data Drift and synchronization bugs when skills are updated.

### Decision
PostgreSQL is the sole Single Source of Truth (SSOT). PDF files are binary attachments and are never treated as primary business data. No `.json` resume files are stored on disk.

### Consequences
- Resume data exists in only one place (PostgreSQL).
- C# ASP.NET Core backend dynamically projects database entities into in-memory `ResumeRevisionDto` instances for Gemini API.
- Eliminates Data Drift and simplifies maintenance.

---

## ADR 003: Storage Abstraction Pattern — `IFileStorage` Provider

**Status:** Accepted

### Context
Binary files (PDF resumes, cover letters, certificates) must be stored independently from domain logic and support future migration to cloud storage.

### Decision
Introduce an abstraction interface in C# .NET:

```csharp
public interface IFileStorage
{
    Task<string> SaveFileAsync(Stream stream, string fileName, string contentType);
    Task<Stream> GetFileAsync(string fileId);
    Task DeleteFileAsync(string fileId);
}
```

Implementations:
- **Phase 1 (MVP):** `LocalFileStorage` writing to `storage/resumes/`.
- **Phase 2 (Cloud):** `GoogleDriveStorage` integrating with Google Drive API via OAuth 2.0.

### Consequences
- Domain services remain completely independent of physical file storage.
- Storage providers are fully replaceable via Dependency Injection.
- Future cloud migration requires zero changes to domain services.

---

## ADR 004: Backup & Recovery Strategy — Native `pg_dump` via `IBackupService`

**Status:** Accepted

### Context
Users must not lose career application history because of local hardware failures or OS reinstallations.

### Decision
Use PostgreSQL's native `pg_dump` utility wrapped inside an `IBackupService`. Generated `backup.dump` snapshots are uploaded to a dedicated `/CareerPulse_Backup/` folder on Google Drive.

### Consequences
- Reliable disaster recovery using official PostgreSQL binary format (`pg_restore`).
- Backups are intended strictly for recovery and are never read during normal application execution.
- Backup logic is isolated from domain services.

---

## ADR 005: Resume Revision Lifecycle, Immutability & Dual-Mode Creation

**Status:** Accepted

### Context
Every submitted resume represents historical evidence of what was sent to an employer, requiring strict integrity while allowing manual or AI-assisted resume creation.

### Decision
1. **Dual Creation Modes:** Support **Mode 1 (Manual Entry - MVP)** and **Mode 2 (AI-Assisted PDF Import)**.
2. **Revision Immutability:** A `ResumeRevision` becomes strictly **Immutable (Read-Only)** once linked to an `Application`. Modifications are allowed ONLY in `Draft` status. Edits to an applied revision spawn a new version (Copy-on-Write).
3. **PDF Attachment Persistence:** In AI-Assisted Import mode, the uploaded PDF is automatically persisted via `IFileStorage` as a `FileReference` attached to the generated revision.

### Consequences
- Guarantees 100% historical accuracy when reviewing past job applications.
- Ensures original PDF files remain accessible for sending to recruiters.

---

## ADR 006: Skill Normalization Pipeline, Categorization & Aliasing

**Status:** Accepted

### Context
Skills extracted from resumes often contain textual variations (e.g., `EF Core`, `Entity Framework`, `EntityFramework`), breaking search, filtering, and vacancy matching.

### Decision
1. Maintain a centralized `MasterSkill` catalog in PostgreSQL.
2. Categorize `MasterSkill` from the start: `Programming Language`, `Framework`, `ORM`, `Database`, `Cloud`, `DevOps`, `Messaging`, `Testing`, `Tools`.
3. Introduce `MasterSkillAlias` entity to resolve variations (`Entity Framework` → `EF Core`). Aliases are internal implementation details not exposed in UI.
4. Unrecognized skills trigger a User Choice Workflow (*Create New Skill*, *Map to Existing*, *Ignore*). Never create new master skills automatically without user approval.

### Consequences
- Reliable searching, accurate filtering, and precise vacancy matching.
- Eliminates duplicated skill names across the system.

---

## ADR 007: Temporary AI Import DTOs & Field-Level Confidence Scoring

**Status:** Accepted

### Context
AI cannot guarantee 100% extraction accuracy from every PDF format (e.g. Canva/Figma graphic layouts).

### Decision
1. Gemini API returns a temporary `ResumeImportResultDto` containing extracted field values and field-level confidence scores.
2. UI visually highlights fields below confidence threshold (<70% marked as "Needs Review").
3. **Human-in-the-Loop Constraint:** Confidence metadata exists ONLY during the import workflow and is NEVER saved to PostgreSQL. Only final user-confirmed values are persisted.

### Consequences
- Domain entities remain clean of temporary AI metadata.
- Users maintain full control over imported data quality.

---

## ADR 008: AI Responsibility Boundary & Human-in-the-Loop Enforcement

**Status:** Accepted

### Context
AI is integrated into multiple features (PDF import, description polish, cover letters, vacancy matching). Without explicit boundaries, AI might autonomously mutate domain data or make business decisions.

### Decision
AI is strictly an advisory component.

AI **may**:
- Extract structured data from uploaded PDFs into temporary DTOs.
- Suggest resume description improvements.
- Generate cover letter drafts.
- Calculate vacancy match scores and skill-gap recommendations.

AI **must NEVER**:
- Write directly to PostgreSQL without user review.
- Create or update `ResumeRevision` automatically.
- Create new `MasterSkill` entities automatically.
- Submit `Applications` automatically.
- Perform irreversible domain operations.

Every business action requires explicit user review and confirmation.

### Consequences
- Humans remain 100% responsible for all business decisions.
- Business logic stays deterministic and predictable.
- AI providers remain replaceable without changing business rules.

---

## ADR 009: Technology Stack Selection — Backend (.NET 9) & Frontend Ecosystem

**Status:** Accepted

### Context
To build a production-grade Personal Career CRM that reflects modern 2026 enterprise standards and aligns with active market hiring requirements, we must select explicit, industry-standard frameworks and libraries.

### Decision

#### 1. Backend Stack (.NET 9 Ecosystem)
- **Framework:** C# .NET 9 Web API (ASP.NET Core).
- **ORM & Database:** Entity Framework Core 9 (`Npgsql.EntityFrameworkCore.PostgreSQL`).
- **Architecture Pattern:** Clean Architecture + CQRS via `MediatR`.
- **Validation:** `FluentValidation` for input DTO verification.
- **Error Format:** RFC 7807 `ProblemDetails` via global middleware.

#### 2. Frontend Stack (Modern React 19 Ecosystem)
- **Core:** React 19 + TypeScript + Vite.
- **UI & Styling:** **Tailwind CSS + Shadcn UI** (Utility-first CSS with accessible, dark-mode ready component primitives).
- **Data Fetching & Caching:** **TanStack Query (React Query)** with **Axios** (automates caching, loading states, error handling, and RFC 7807 interceptors).
- **Client State Management:** **Zustand** (lightweight state manager for UI modals, active filters, and client preferences).

### Consequences
- Aligns 100% with current commercial market expectations for Senior Full-Stack React + .NET developers.
- Eliminates legacy boilerplate (no outdated Redux or manual fetch state loops).
- High UI aesthetics and rapid development velocity using Shadcn UI + Tailwind CSS primitives.

