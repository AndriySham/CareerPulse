# Domain Model Specification: CareerPulse

## 1. Domain Overview & Philosophy
The domain of **CareerPulse** centers around managing a software developer's career progression, tracking job applications, and evaluating decision-making for re-applying based on acquired skills over time.

Following **Domain-Driven Design (DDD)** principles, the domain logic is strictly separated from database persistence, HTTP APIs, and AI integrations. Business rules are deterministic, explicit, and immune to external infrastructure shifts.

The specific architectural pattern, project structure, and component boundaries will be formally designed and detailed in `ARCHITECTURE.md`.

---

## 2. Ubiquitous Language

| Term | Definition |
|------|------------|
| **Resume** | The root aggregate representing a stable candidate career profile (positioning persona) for a specific job track and level. |
| **ResumeTrack** | A domain enum defining the candidate's specialization area (`Backend`, `Frontend`, `FullStack`). |
| **CareerLevel** | A domain enum defining the targeted experience tier (`Intern`, `Junior`, `Middle`, `Senior`, `Lead`). |
| **TargetRole** | A domain string property defining specific job title positioning (e.g. `".NET Backend Developer"`). |
| **ResumeRevision** | An immutable snapshot entity belonging to a `Resume`, representing structured resume content at a specific point in time. |
| **WorkExperience** | An owned snapshot collection in `ResumeRevision` representing employment history with month/year precision. |
| **Education** | An owned snapshot collection in `ResumeRevision` representing academic credentials and certifications. |
| **Project** | An owned snapshot collection in `ResumeRevision` representing portfolio projects, pet projects, and repositories. |
| **Language** | An owned snapshot collection in `ResumeRevision` representing spoken languages and fluency levels. |
| **PersonalInfo** | A C# Value Object containing developer contact details (FullName, Email, Phone, LinkedIn, GitHub, Location). |
| **MasterSkill** | A standalone aggregate root representing a normalized, categorized technical skill (e.g. `C#`, `PostgreSQL`, `Docker`). |
| **MasterSkillAlias** | An alternative text representation pointing to a `MasterSkill` (e.g., `EF` → `EF Core`). |
| **Company** | An employer organization offering job vacancies and maintaining application history. |
| **Vacancy** | A job opportunity post associated with a `Company` defining required skills and requirements. |
| **Application** | The central domain anchor representing an active or historical job submission to a company. |
| **ApplicationStatus** | A state machine representing the lifecycle of an `Application` (`Draft` → `Applied` → `HR Interview` → `Technical Interview` → `Offer` / `Rejected`). |

---

## 3. Aggregate Roots & Entity Relationships

```text
┌────────────────────────────────┐                 ┌─────────────────────────┐
│         Resume (Root)          │                 │     MasterSkill (Root)  │
│  - Name, Track, CareerLevel    │                 │  - Normalized Skill     │
│  - TargetRole                  │                 │  - SkillCategory        │
└───────────────┬────────────────┘                 └────────────┬────────────┘
                │ 1:N                                           │ 1:N
                ▼                                               ▼
┌────────────────────────────────┐                 ┌─────────────────────────┐
│   ResumeRevision (Snapshot)    │                 │    MasterSkillAlias     │
│  - PersonalInfo, Summary       │                 └─────────────────────────┘
│  - WorkExperiences, Educations │
│  - Projects, Languages         │                 ┌─────────────────────────┐
│  - Skills (MasterSkillId)      │                 │     Company (Root)      │
│  - FileReference (PDF key)     │                 │  - Historical Contacts  │
└───────────────┬────────────────┘                 │  - Vacancies            │
                │                                  └────────────┬────────────┘
                └───────────────────────┐                       │ 1:N
                                        │                       ▼
                                        │          ┌─────────────────────────┐
                                        │          │         Vacancy         │
                                        │          └────────────┬────────────┘
                                        │                       │
                                        ▼                       ▼
                        ┌────────────────────────────────────────┐
                        │          Application (Root)            │
                        │  - References ResumeRevisionId (FK)    │
                        │  - ApplicationStatus (Machine)         │
                        │  - Submission Date & JobSource         │
                        └────────────────────────────────────────┘
```

---

## 4. Domain Entities & Value Objects

### Aggregate Root 1: `Resume`
- **Responsibilities:** Represents a stable candidate positioning persona for a target career track and experience level. Owns the version lineage of its child `ResumeRevision` snapshots.
- **Identity Properties (Immutable after creation):** `Track` (`ResumeTrack`), `CareerLevel` (`CareerLevel`), `TargetRole` (string).
- **Mutable Properties:** `Name` (string).
- **Child Entities:** `ResumeRevision`.

### Aggregate Root 2: `MasterSkill` (Catalog Aggregate)
- **Responsibilities:** Standalone SSOT catalog of normalized technical skills and categories.
- **Child Entities:** `MasterSkillAlias`.

### Aggregate Root 3: `Company`
- **Responsibilities:** Manages target employer profiles and job vacancies.
- **Child Entities:** `Vacancy`, `CompanyContact`.
- **Value Objects:** `WebsiteUrl`.

### Aggregate Root 4: `Application` (Central Domain Anchor)
- **Responsibilities:** Owns the application lifecycle state machine.
- **Relationships:** Holds an immutable foreign key `ResumeRevisionId` pointing to a specific `ResumeRevision` snapshot, alongside references to `Company` and optional `Vacancy`.

### Owned Snapshot Collections & Value Objects (Owned by `ResumeRevision`)
- **`PersonalInfo` (Value Object):** `FullName`, `Email`, `Phone`, `LinkedIn`, `GitHub`, `Location`.
- **`WorkExperience` (Owned Collection):** `CompanyName` (string snapshot), `PositionTitle`, `StartDate` (Month/Year), `EndDate` (Month/Year), `IsCurrentJob`, `Description`, `Achievements`, `TechStack`.
- **`Education` (Owned Collection):** `InstitutionName`, `Degree`, `FieldOfStudy`, `StartYear`, `EndYear`.
- **`Project` (Owned Collection):** `ProjectName`, `Description`, `Role`, `RepositoryUrl`, `LiveDemoUrl`, `TechStack`.
- **`Language` (Owned Collection):** `LanguageName`, `Proficiency`.
- **`ResumeRevisionSkill` (Join Entity):** `ResumeRevisionId`, `MasterSkillId`, `ProficiencyLevel` (1–5).

---

## 5. Domain Invariants & Business Constraints

### Invariant 1: Profile Identity Immutability
- A `Resume` profile's `Track`, `CareerLevel`, and `TargetRole` are **immutable after creation**.
- Changing career track or level requires creating a **new `Resume` aggregate root**, rather than spawning a new `ResumeRevision`.

### Invariant 2: Draft Immutability & Copy-on-Write
- A `ResumeRevision` is editable ONLY while its status is `Draft`.
- Attaching a `ResumeRevision` to an `Application` and transitioning to `Applied` locks the revision as **Applied (Terminal Read-Only)**.
- Subsequent edits spawn a new `ResumeRevision` version (`v2`) under the same parent `Resume` profile (`SpawnNewVersion()`).
- Spawning deep-copies `PersonalInfo`, `Summary`, `WorkExperiences`, `Educations`, `Projects`, `Languages`, and `Skills`. `FileReference` is reset to `null`.

### Invariant 3: Application Snapshot Immutability
- `Application.ResumeRevisionId` is set at submission time and is **immutable**.
- An `Application` forever references the exact historical `ResumeRevision` snapshot submitted to the employer.

### Invariant 4: Work Experience Company Snapshot Isolation
- `WorkExperience` stores `CompanyName` as a plain string snapshot.
- Past employment history is completely isolated from mutations or deletions of `Company` entities in the CRM.
- Work experience employment dates are stored with Month and Year precision only.

### Invariant 5: Skill Normalization & Catalog Independence
- All skills in a `ResumeRevision` resolve to a `MasterSkillId`.
- Proficiency levels belong to `ResumeRevisionSkill` within a specific `ResumeRevision` snapshot, keeping `MasterSkill` pure and shared.

### Invariant 6: Single Source of Truth & PDF Attachments
- Structured PostgreSQL data is the sole Source of Truth.
- PDF files are referenced by `FileReference` keys managed by `IFileStorage` (ADR 003).

---

## 6. Domain Isolation Principle
- Business rules and domain entities remain pure C# models, isolated from HTTP Controllers, Entity Framework infrastructure details, and UI concerns.
- Project layout, namespace organization, and persistence mappings are detailed in `ARCHITECTURE.md`.
