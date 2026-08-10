# Resume Ownership and Snapshot Semantics Design

# Resume Ownership and Snapshot Semantics Design

## 1. Resume Identity Model

### 1.1 Overview

The **`Resume`** aggregate root represents a developer's targeted career profile (positioning persona) for a specific job track and experience level. It acts as the stable parent identity container for a linear chain of versioned snapshots (**`ResumeRevision`**).

### 1.2 Identity Properties

A `Resume` profile is defined by four core properties:

- **`Name`** **(string):** User-facing label (e.g., `"C# Backend Developer Resume"`). Editable.
- **`Track`** **(`ResumeTrack` Enum):** Specialization area (`Backend`, `Frontend`, `FullStack`). **Immutable after creation.**
- **`CareerLevel`** **(`CareerLevel` Enum):** Targeted experience tier (`Intern`, `Junior`, `Middle`, `Senior`, `Lead`). **Immutable after creation.**
- **`TargetRole`** **(string):** Specific job title positioning (e.g., `".NET Backend Developer"`, `"React Frontend Engineer"`). **Immutable after creation.**

### 1.3 Immutability of Identity & Profile Separation

> **Rule:** Changing `Track`, `CareerLevel`, or `TargetRole` **MUST create a new** **`Resume`** **profile**, rather than spawning a new `ResumeRevision`.

#### Architectural Rationale:

A revision (`v1` → `v2` → `v3`) represents iterative refinement of the *same* candidate profile (e.g., tailoring summary or updating skills for a Junior .NET role).

A transition from `Junior` to `Middle`, or `Backend` to `FullStack`, represents a fundamentally different career positioning with distinct responsibilities, commercial history, and portfolio focus.

A resume may also be created specifically for a single vacancy or employer. Such a resume profile may contain a tailored `ProfessionalSummary`, selected skills, projects, or other content optimized for that particular vacancy. It can exist as an independent `Resume` profile and does not need to be related to another resume profile.

A user may therefore maintain both:

- standard resume profiles used repeatedly for similar vacancies;
- specialized resume profiles created for a specific vacancy or employer.

Within the same `Resume` profile, individual revisions may also be either **standard** or **tailored**. A tailored revision may be created for one specific application, while another revision of the same profile can remain the standard version used for other applications.

The specialized profile may be based conceptually on an existing resume, but it remains an independent `Resume` aggregate with its own revision chain.

The `Resume` identity (`Track`, `CareerLevel`, `TargetRole`) must not change across revisions. Tailoring a revision may change resume content, but it must not change the profile identity.

#### Example Profile Coexistence:

```text
Resume #1: Track = Backend, Level = Intern, TargetRole = ".NET Backend Intern"
  ├── Revision 1 (Draft)
  └── Revision 2 (Applied)

Resume #2: Track = Backend, Level = Junior, TargetRole = ".NET Backend Developer" [Independent Resume]
  ├── Revision 1 (Draft)
  └── Revision 2 (Applied)

Resume #3: Track = Backend, Level = Middle, TargetRole = ".NET Backend Developer" [Independent Resume]
  └── Revision 1 (Draft)

Resume #4: Track = Frontend, Level = Junior, TargetRole = "React Developer" [Independent Resume]
  └── Revision 1 (Draft)

Resume #5: Track = FullStack, Level = Junior, TargetRole = "Full-Stack .NET Developer" [Tailored for a specific vacancy]
  └── Revision 1 (Applied)
```

---

## 2. Resume vs ResumeRevision Responsibilities

| Responsibility / Property | Owner Aggregate / Entity | Rationale |
| :--- | :--- | :--- |
| **Profile Identity (`Track`, `CareerLevel`, `TargetRole`)** | **`Resume`** (Aggregate Root) | Defines the stable career persona. Shared across all revisions of this profile. |
| **Profile Display Name (`Name`)** | **`Resume`** (Aggregate Root) | Allows the user to label the profile container in the UI. |
| **Revision History Collection (`Revisions`)** | **`Resume`** (Aggregate Root) | `Resume` owns the lifecycle and version sequence of its child `ResumeRevision` snapshots. |
| **Personal Contacts (`PersonalInfo`)** | **`ResumeRevision`** | Historical snapshot: Developer phone/location/email at the time of snapshot creation. |
| **Professional Summary (`ProfessionalSummary`)** | **`ResumeRevision`** | Content tailored per revision. |
| **Skills & Proficiency Levels (`Skills`)** | **`ResumeRevision`** | Content snapshot: Proficiency levels and skill attachments evolve per revision. |
| **Work Experience (`WorkExperiences`)** | **`ResumeRevision`** | Content snapshot: Responsibilities, achievements, and tech stacks tailored per revision. |
| **Education & Certifications (`Educations`)** | **`ResumeRevision`** | Content snapshot of academic credentials. |
| **Projects & Portfolio (`Projects`)** | **`ResumeRevision`** | Content snapshot: Relevant pet projects or team projects tailored per revision. |
| **Languages (`Languages`)** | **`ResumeRevision`** | Content snapshot of spoken languages and fluency levels. |
| **PDF Storage Attachment (`FileReference`)** | **`ResumeRevision`** | Key to the generated or uploaded PDF file for *this specific revision*. |
| **Version & Parent Pointer (`Version`, `ParentRevisionId`)** | **`ResumeRevision`** | Copy-on-Write lineage tracking. |
| **Draft Immutability Lock (`Status`)** | **`ResumeRevision`** | Indicates whether this specific revision is an editable `Draft` or a locked `Applied` snapshot. |

---

## 3. Snapshot Ownership Table

| Concept | Owner | Snapshot Data? | Mutable? | Notes |
| :--- | :--- | :---: | :--- | :--- |
| **`Resume`** | Root Aggregate | No | `Name` only | Parent aggregate container. Identity (`Track`, `Level`, `TargetRole`) is immutable. |
| **`ResumeRevision`** | `Resume` | Yes | `Draft` only | Child entity. Once `Applied`, becomes 100% read-only. |
| **`PersonalInfo`** | `ResumeRevision` | Yes | `Draft` only | Value Object owned by `ResumeRevision`. |
| **`ProfessionalSummary`** | `ResumeRevision` | Yes | `Draft` only | Text string property. |
| **`WorkExperience`** | `ResumeRevision` | Yes | `Draft` only | Owned Value Object / Snapshot Entity deep-copied per revision. |
| **`Education`** | `ResumeRevision` | Yes | `Draft` only | Owned Value Object / Snapshot Entity deep-copied per revision. |
| **`Project`** | `ResumeRevision` | Yes | `Draft` only | Owned Value Object / Snapshot Entity deep-copied per revision. |
| **`Language`** | `ResumeRevision` | Yes | `Draft` only | Owned Value Object / Snapshot Entity deep-copied per revision. |
| **`ResumeRevisionSkill`** | `ResumeRevision` | Yes | `Draft` only | Join entity storing `MasterSkillId` + `ProficiencyLevel`. |
| **`MasterSkill`** | Catalog Aggregate | No | Yes | Canonical reference catalog (`C#`, `PostgreSQL`). Shared across all resumes. |
| **`FileReference`** | `ResumeRevision` | Yes | `Draft` only | `IFileStorage` key pointing to PDF artifact (ADR 003). |
| **`Application`** | Root Aggregate | No | Yes (`Status`) | Central domain anchor. Holds immutable `ResumeRevisionId` foreign key. |

---

## 4. Copy-on-Write Lifecycle

```text
A. Create First Revision ──► B. Edit Draft ──► C. Link to Application & Apply
                                                     │
                                                     ▼
                                        D. Attempt Direct Edit ──► [BLOCKED: DomainException]
                                                     │
                                                     ▼
                                        E. Call SpawnNewVersion()
                                                     │
                                                     ▼
                                        Creates New Revision v2 (Draft)
```

### 4.1 Lifecycle Operations

* **A. Creating the First Revision:**
  * When a new `Resume` is created, `ResumeRevision v1` is automatically initialized with `Version = 1`, `Status = RevisionStatus.Draft`, and `ParentRevisionId = null`.
* **B. Editing a Draft Revision:**
  * While `Status == Draft`, in-place updates to `Summary`, `PersonalInfo`, `WorkExperiences`, `Educations`, `Projects`, `Languages`, `Skills`, and `FileReference` are permitted.
* **C. Applying a Revision to an Application:**
  * When an `Application` linked to `ResumeRevision v1` transitions to `Applied`, `v1.MarkAsApplied()` is invoked. `v1.Status` becomes `Applied` (Terminal Read-Only).
* **D. Attempting to Edit an Applied Revision:**
  * All mutation methods on `v1` throw `DomainException("Cannot modify an Applied (Read-Only) ResumeRevision.")`.
* **E. Creating a New Revision (`SpawnNewVersion`):**
  * Invoking `v1.SpawnNewVersion()` creates `ResumeRevision v2` under the **same parent `ResumeId`**.
  * **Copied Content:** Deep copies of `PersonalInfo`, `ProfessionalSummary`, `WorkExperiences`, `Educations`, `Projects`, `Languages`, and `Skills`.
  * **New Identifiers:** `v2.Id = Guid.NewGuid()`, `v2.Version = v1.Version + 1`, `v2.ParentRevisionId = v1.Id`, `v2.Status = RevisionStatus.Draft`.
  * **Reset Fields:** `v2.FileReference = null` (requires uploading or generating a new PDF for `v2`).

---

## 5. Application Relationship

```text
Application (Aggregate Root)
 ├── Id: Guid
 ├── CompanyId: Guid
 ├── VacancyId: Guid?
 ├── ApplicationStatus: Applied
 └── ResumeRevisionId: Guid ────────────────► ResumeRevision v1 (Status = Applied)
                                               ├── TargetRole: ".NET Developer"
                                               ├── PersonalInfo, Summary
                                               └── FileReference: "storage/pdfs/v1.pdf"
```

### 5.1 Requirement
The `Application` aggregate root MUST hold a foreign key `ResumeRevisionId` pointing directly to a specific **`ResumeRevision`**, NOT to the parent `Resume`.

### 5.2 Rationale
An `Application` represents a historical job submission at a specific point in time. Pointing to `Resume` would make the application dynamic, meaning future edits to the resume would corrupt the historical record of what was actually submitted to the employer. Pointing to `ResumeRevision` guarantees absolute auditability and snapshot immutability.

---

## 6. PDF Ownership Decision

### 6.1 Ownership Location
`FileReference` belongs strictly to **`ResumeRevision`**.

### 6.2 Rationale
Each `ResumeRevision` snapshot represents a distinct version of candidate data, which may be rendered into or uploaded as a unique PDF document. 
When `v2` is spawned from `v1`, `v2.FileReference` is set to `null` so that `v1` retains its specific PDF artifact key in `IFileStorage` without risk of overwrite.

---

## 7. Work Experience, Education, Project, and Language Modeling Decision

### 7.1 Decision: Owned Snapshot Child Entities

`WorkExperience`, `Education`, `Project`, and `Language` MUST be modeled as **child entities owned by `ResumeRevision`**.

They do not exist independently outside a `ResumeRevision` and are included in the immutable snapshot of that revision.

Each revision owns its own copies of these entities. When `SpawnNewVersion()` creates a new revision, these entities are deep-copied so that modifications to the new `Draft` revision cannot affect any historical `Applied` revision.

### 7.2 Rationale against Shared Global Entities:
If `WorkExperience` were a shared entity referenced by multiple revisions, modifying an achievement in `WorkExperience` for a new `v2` revision would silently alter historical `Applied v1` revisions linked to existing `Applications`. 
Modeling them as owned snapshot collections deep-copied during `SpawnNewVersion()` ensures 100% self-contained snapshot immutability.

---

## 8. Company Relationship Decision

### 8.1 Decision: Store Historical `CompanyName` String Snapshot
`WorkExperience` MUST store `CompanyName` as a plain `string` property, NOT as a foreign key to the `Company` domain aggregate root.

### 8.2 Rationale:
1. **Domain Boundary Isolation:** The `Company` aggregate root in CareerPulse represents prospective employers offering job vacancies for applications. Work experience in a resume includes past employers, freelance projects, or companies never tracked as application targets.
2. **Snapshot Immutability:** If a target `Company` in CareerPulse is renamed or deleted, historical work experience entries in past resume revisions must remain completely unchanged.

---

## 9. Work Experience Date Representation

### 9.1 Decision

Work experience dates MUST be stored with **month and year precision only**.

The model does not require a specific day for employment history in a resume.

---

## 10. Explicit Invariants

1. **Profile Identity Lock:** A `Resume` profile's `Track`, `CareerLevel`, and `TargetRole` are immutable after creation. Changing them requires creating a new `Resume` aggregate root.
2. **Copy-on-Write Profile Boundary:** `ResumeRevision.SpawnNewVersion()` creates a new revision under the same parent `ResumeId`. It cannot change profile identity.
3. **Draft Immutability Guard:** A `ResumeRevision` is editable ONLY while `Status == RevisionStatus.Draft`.
4. **Applied Lock:** When an `Application` transitions to `Applied`, the linked `ResumeRevision` is locked as `Applied` (Read-Only).
5. **Snapshot Self-Containment:** Modifying a `Draft` revision (`v2`) can never mutate historical `Applied` revisions (`v1`).
6. **MasterSkill Canonical Sharing:** `MasterSkill` is shared catalog data. Skill proficiency levels belong to `ResumeRevisionSkill` within a specific `ResumeRevision`.
7. **Application Immutability:** `Application.ResumeRevisionId` is immutable after application creation.
8. **Standard / Tailored Revisions:** A revision may be standard and reused across similar applications, or tailored for a specific vacancy/application. Tailoring cannot change the parent `Resume` identity.

---

## 11. Documentation Impact & Contradiction Analysis

### 11.1 Discovered Contradictions in Existing Docs

* **`DOMAIN_MODEL.md` Contradiction:**
  * `DOMAIN_MODEL.md` currently references `CareerProfile` as an Aggregate Root owning `ResumeTemplate` and `MasterSkill`. 
  * *Correction:* `CareerProfile` is an overarching concept. `Resume` is the actual aggregate root for resume profiles, while `MasterSkill` is a standalone shared catalog aggregate root. `ResumeTemplate` term must be replaced with `Resume`.
* **`ARCHITECTURE.md` Contradiction:**
  * `ARCHITECTURE.md` describes `ResumeRevision` as a direct child of `ResumeTemplate`. 
  * *Correction:* Update references from `ResumeTemplate` to `Resume` and document `Track`/`CareerLevel` identity rules.

### 11.2 Documentation Action Plan (Post-Approval)

| Document | Planned Updates |
| :--- | :--- |
| **`DOMAIN_MODEL.md`** | Add `Resume` aggregate root, `ResumeTrack`, `CareerLevel`, `WorkExperienceEntry`, `EducationEntry`, `ProjectEntry`, `LanguageEntry`. Clarify `MasterSkill` catalog independence. |
| **`ARCHITECTURE.md`** | Update aggregate root boundaries diagram (`Resume` → `ResumeRevision`, `Application` → `ResumeRevisionId`). Document Copy-on-Write mechanics. |
| **`ADR.md`** | Add **`ADR 010 — Resume Profile and Revision Architecture`** documenting identity immutability and owned snapshot collections. |
| **`VISION_AND_SCOPE.md`** | Update Resume Management feature descriptions to reflect multi-profile support (Track/Level) and full Resume Builder. |
| **`MEMORY.md`** | Update active architecture context with `Resume` profile identity rules. |
| **`AGENTS.md`** | Add rule: *"Resume identity (Track, CareerLevel, TargetRole) is immutable across revisions. Changing identity requires creating a new Resume profile."* |

---

## 12. Resolved Decisions

* **DECIDED 1: WorkExperience Company Reference vs Plain String**
  * Store `CompanyName` as a string snapshot in `WorkExperienceEntry` rather than a foreign key to `Company`.
* **DECIDED 2: Profile Identity Immutability**
  * `Track`, `CareerLevel`, and `TargetRole` are immutable on `Resume`. Changing them requires creating a new `Resume` profile rather than spawning a revision.
* **DECIDED 3: Scope of Owned Collections on ResumeRevision**
  * `WorkExperiences`, `Educations`, `Projects`, and `Languages` are owned snapshot collections on `ResumeRevision`, deep-copied during `SpawnNewVersion()`.
* **DECIDED 4: Resume Tracks**
  * `ResumeTrack` currently contains only `Backend`, `Frontend`, and `FullStack`. Additional tracks such as `Mobile` or `DevOps` may be added later if the product requires them.
* **DECIDED 5: Standard / Tailored Revisions**
  * A `Resume` profile may contain both reusable standard revisions and revisions tailored for specific applications. Tailoring does not change `Track`, `CareerLevel`, or `TargetRole`.
* **DECIDED 6: Work Experience Date Precision**
  * Work experience dates use month and year precision only; a specific day is not required.

---

A2 DESIGN COMPLETE — DECISIONS FINALIZED
