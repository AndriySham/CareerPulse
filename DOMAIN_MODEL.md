# Domain Model Specification: CareerPulse

## 1. Domain Overview & Philosophy
The domain of **CareerPulse** centers around managing a software developer's career progression, tracking job applications, and evaluating decision-making for re-applying based on acquired skills over time.

Following **Domain-Driven Design (DDD)** principles, the domain logic is strictly separated from database persistence, HTTP APIs, and AI integrations. Business rules are deterministic, explicit, and immune to external infrastructure shifts.

The specific architectural pattern, project structure, and component boundaries will be formally designed and detailed by the `@architect` subagent in `ARCHITECTURE.md`.

---

## 2. Ubiquitous Language

| Term | Definition |
|------|------------|
| **CareerProfile** | The root profile representing a developer's global master identity, skills, projects, and experiences. |
| **MasterSkill** | A normalized, categorized technical skill (e.g. `C#`, `PostgreSQL`, `Docker`). |
| **MasterSkillAlias** | An alternative text representation pointing to a `MasterSkill` (e.g., `EF` → `EF Core`). |
| **ResumeProfile** | A specific target role/persona variant (e.g., `Backend Junior`, `Frontend Intern`). Formerly called ResumeTemplate. |
| **ResumeRevision** | An immutable snapshot of structured resume data created at a specific point in time. |
| **PersonalInfo** | A C# Value Object containing developer contact details (Name, Email, Phone, LinkedIn, GitHub, Location). |
| **Company** | An employer organization offering job vacancies and maintaining contact history. |
| **Vacancy** | A job opportunity post associated with a `Company` defining required skills and requirements. |
| **Application** | The central domain entity representing an active or historical job submission. |
| **ApplicationStatus** | A state machine representing the lifecycle of an `Application` (`Draft` → `Applied` → `HR Interview` → `Technical Interview` → `Offer` / `Rejected`). |

---

## 3. Aggregate Roots & Entity Relationships

```text
┌────────────────────────────────┐                 ┌─────────────────────────┐
│     CareerProfile (Root)       │                 │     Company (Root)      │
│  - MasterSkills Catalog        │                 │  - Historical Contacts  │
│  - MasterSkillAliases          │                 │  - Vacancies            │
└───────────────┬────────────────┘                 └────────────┬────────────┘
                │ 1:N                                           │ 1:N
                ▼                                               ▼
┌────────────────────────────────┐                 ┌─────────────────────────┐
│         ResumeTemplate         │                 │         Vacancy         │
└───────────────┬────────────────┘                 └────────────┬────────────┘
                │ 1:N                                           │
                ▼                                               │
┌────────────────────────────────┐                              │
│  ResumeRevision (Snapshot)     │                              │
└───────────────┬────────────────┘                              │
                │                                               │
                └───────────────────────┐   ┌───────────────────┘
                                        │   │
                                        ▼   ▼
                        ┌────────────────────────────────┐
                        │       Application (Root)       │
                        │  - ApplicationStatus (Machine) │
                        │  - Submission Date & Source    │
                        └────────────────────────────────┘
```

---

## 4. Domain Entities & Value Objects

### Aggregate Root 1: `CareerProfile`
- **Responsibilities:** Manages developer identity and owns the global `MasterSkill` catalog and alias mappings.
- **Child Entities:** `MasterSkill`, `MasterSkillAlias`, `ResumeProfile`.
- **Value Objects:** `PersonalInfo`, `Education`, `Certification`, `Project`.

### Aggregate Root 2: `Company`
- **Responsibilities:** Manages employer profile information and owns job vacancies.
- **Child Entities:** `Vacancy`, `CompanyContact`.
- **Value Objects:** `CompanyMetadata`, `WebsiteUrl`.

### Aggregate Root 3: `Application` (Central Domain Anchor)
- **Responsibilities:** Owns the lifecycle state machine of a job application.
- **Relationships:** References a `Company`, a `Vacancy`, and a specific `ResumeRevision`.
- **Value Objects:** `ApplicationStatusHistory`, `JobSource`.

---

## 5. Domain Invariants & Business Constraints

### Invariant 1: Draft Immutability Rule
- A `ResumeRevision` is editable ONLY while its status is `Draft` (unlinked to any `Application`).
- The moment a `ResumeRevision` is attached to an `Application`, it becomes **Immutable (Read-Only)**.
- Any subsequent modifications spawn a new `ResumeRevision` version (Copy-on-Write).

### Invariant 2: Skill Normalization
- All skills in a `ResumeRevision` must resolve to a valid `MasterSkill`.
- Textual variations are resolved via `MasterSkillAlias`.
- Unrecognized skills cannot be added to `MasterSkill` without explicit user review.

### Invariant 3: Single Source of Truth
- Structured data inside PostgreSQL is the sole Source of Truth.
- PDF files are `FileReference` attachments; they are never treated as domain models.

### Invariant 4: AI Responsibility Boundary
- AI tools function as advisory services only.
- AI cannot mutate `ResumeRevision`, `MasterSkill`, or `Application` entities autonomously.
- Every domain mutation originating from AI extractions requires user confirmation.

---

## 6. Domain Isolation Principle
- Business rules and domain entities must remain pure C# models, isolated from HTTP Controllers, Entity Framework infrastructure details, and UI concerns.
- The detailed architectural project structure, project/namespace layout, and DTO mapping strategy are delegated to the `@architect` subagent to design in `ARCHITECTURE.md`.
