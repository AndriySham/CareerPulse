# Product Vision & Scope: CareerPulse (Personal Career CRM)

## 1. Product Summary
CareerPulse is a Personal Career CRM that helps software developers manage resumes, job applications, interviews, career progress, and AI-assisted job search.

It enables users to manage multiple resume templates and immutable resume revisions, track the entire job application lifecycle, prevent duplicate applications, analyze application outcomes across different job platforms, and make informed re-application decisions based on newly acquired skills and experience.

---

## 2. Product Vision
CareerPulse aims to become a centralized personal career management platform for software developers.

The system should help users make informed career decisions, maintain a complete history of their job search, analyze long-term application results, and leverage AI as an assistant without replacing user decisions.

---

## 3. Core Architectural Principles (Per ADR 001 - 008)
The following principles define the foundation of the entire system and guide every architectural and implementation decision:

- **PostgreSQL in Docker** is the Single Source of Truth for all structured business data and entity relationships.
- **PDF resumes are attachments**, referenced via `FileReference`, not primary business data.
- **ResumeRevisions are immutable snapshots.** Once linked to an Application, a revision becomes Read-Only; subsequent edits spawn a new revision version.
- **AI Responsibility Boundary (ADR 008):** AI is strictly advisory. AI assists the user but never makes business decisions, mutates PostgreSQL, or creates entities automatically.
- **Human-in-the-Loop Authority:** Every AI-assisted action (PDF import, cover letter, skill creation) requires explicit user review and confirmation before database persistence.
- **Skills are normalized** through the categorized `MasterSkill` catalog and `MasterSkillAlias` mapping.
- **Abstract File Storage (`IFileStorage`)** decouples domain logic from physical storage (`LocalFileStorage` -> `GoogleDriveStorage`).
- **Native Backup (`IBackupService`)** uses PostgreSQL `pg_dump` snapshots uploaded to Google Drive.

---

## 4. Core Business Entities & Domain Hierarchy

### CareerProfile (Root Profile)
Contains reusable career information shared across resume revisions:
- Personal Information
- Master Skill Catalog
- Projects, Work Experience, Education, Languages, Certifications, Knowledge Base.

---

### MasterSkill & MasterSkillAlias (Skill Catalog)
- **Categorized Skills:** `Programming Language`, `Framework`, `ORM`, `Database`, `Cloud`, `DevOps`, `Messaging`, `Testing`, `Tools`.
- **MasterSkillAlias** resolves string variations (`Entity Framework`, `EF`, `EntityFramework` -> `EF Core`).

---

### ResumeTemplate (Resume Variant)
Examples: `Backend Intern`, `Backend Junior`, `Frontend Intern`, `Frontend Junior`, `Fullstack Intern`, `Fullstack Junior`.
- Belongs to `CareerProfile`.

---

### ResumeRevision (Snapshot – Source of Truth)
Represents a complete immutable snapshot of a resume stored in PostgreSQL:
- Personal Information, Professional Summary, Master Skills (via N:M `ResumeRevisionSkill`), Work Experience, Projects, Education, Languages, Certifications, Contacts, attached PDF (`FileReference`).

#### Immutability Rule
A `ResumeRevision` becomes **Read-Only** once linked to an `Application`. Edits are allowed only while in `Draft` state; any further changes to an applied revision spawn a new `ResumeRevision` version (Copy-on-Write).

---

### Company
- Name, Website, Industry, Notes, Historical Contact Log.

---

### Vacancy
Contains: Title, Grade, Specialization, Required Master Skills, Job Description.
- Belongs to `Company`.

---

### Application (Central Domain Entity)
Represents one specific job application.
Links together: `Company` + `Vacancy` + `ResumeRevision` + `JobSource` (Djinni, DOU, LinkedIn, etc.) + `SubmissionDate` + `Status`.

---

### ApplicationStatus (State Machine)
```
Draft
  ↓
Applied
  ↓
Viewed
  ↓
HR Interview
  ↓
Technical Interview
  ↓
Offer / Rejected / No Response
```

---

## 5. Resume Creation Workflows

### Mode 1 — Manual Resume Creation (MVP Core)
The user creates a `ResumeRevision` manually through the UI:
- Enters personal info, selects skills from `MasterSkill` catalog, adds experience/projects, and optionally attaches a PDF resume.
- 100% independent of AI; available at all times.

---

### Mode 2 — AI-Assisted PDF Import (HITL Feature)
Workflow for uploading an existing resume PDF (Canva, Word, Figma, LaTeX, etc.):
1. Store PDF via `IFileStorage` (`storage/resumes/`) and create `FileReference`.
2. Send PDF to Gemini API SDK.
3. Extract structured data into temporary `ResumeImportResultDto` with field-level **Confidence Scores**.
4. Pass extracted skills through Skill Normalizer (`MasterSkill` & `MasterSkillAlias`).
5. Pre-fill the UI form, highlighting fields with confidence <70% as "Needs Review".
6. User reviews every field and resolves unrecognized skills (*Create New*, *Map to Existing*, *Ignore*).
7. User explicitly confirms the imported data.
8. `ResumeRevision` is saved into PostgreSQL.

#### Human-in-the-Loop Rule (ADR 008)
AI never writes directly to PostgreSQL. The imported `ResumeRevision` is persisted ONLY after explicit user confirmation.

---

### Future Resume Generation
Because every `ResumeRevision` is stored as structured domain data in PostgreSQL, future versions of CareerPulse will generate professional PDF resumes directly from templates.

---

## 6. Architecture & Storage Strategy

### Database
PostgreSQL running inside Docker stores resume data, Master Skills, companies, vacancies, applications, relationships, and statuses.

### Binary File Storage (`IFileStorage`)
- **Phase 1 (MVP):** `LocalFileStorage` writing to `storage/resumes/`.
- **Phase 2 (Cloud):** `GoogleDriveStorage` using OAuth 2.0.

### AI Responsibilities & Boundaries (ADR 008)
AI assists users by extracting structured data, polishing descriptions, generating cover letters, and matching resumes against vacancies. AI communicates using in-memory `ResumeRevisionDto` instances rather than parsing stored PDF files. AI never performs domain mutations autonomously.

### Backup Strategy (`IBackupService`)
PostgreSQL backups are created using native `pg_dump` (`backup.dump`) and uploaded to Google Drive for 1-click cloud recovery.

---

## 7. Key Decision-Making Scenarios

### Scenario 1 — Duplicate Prevention
Search for a company before applying. See previous applications, submitted `ResumeRevision`, and application outcome.

### Scenario 2 — Re-application Analysis
Compare the current `ResumeRevision` with previously submitted versions to determine whether enough new skills or experience have been gained before applying again.

### Scenario 3 — Channel Analytics
Analyze response rates from different job platforms (LinkedIn vs Djinni vs DOU).

### Scenario 4 — Resume Evolution
Track how resumes evolve over time. Compare `ResumeRevisions` to identify newly acquired skills, new projects, additional certifications, and accumulated experience for career growth planning.
