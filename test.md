Read AGENTS.md, ADR.md, DOMAIN_MODEL.md, ARCHITECTURE.md, and MEMORY.md.

Task: Improve the existing Resume Editor content sections.

## Feature

Improve the existing Resume Editor implementation by refining the already migrated Personal Information, Professional Summary, and Skills functionality.

The Resume Editor page, routing, Resume metadata section, and section navigation already exist and must be preserved.

The implementation must follow the existing frontend architecture, conventions, and design system defined in AGENTS.md and ADR 009.

## Before Implementation

* Inspect the existing Resume Editor implementation under `src/frontend/src/`.
* Inspect the current `ResumeEditorPage.tsx` and all related Resume API hooks, types, and components.
* Inspect the corresponding backend DTOs and endpoints to ensure the existing frontend behavior matches the current API contract.
* Identify the existing implementation of:

  * Personal Information;
  * Professional Summary;
  * MasterSkill selection;
  * Skill proficiency;
  * Attached skills list.
* Reuse the existing components, hooks, types, utilities, design tokens, and UI patterns.
* Do not recreate functionality that already exists.

## Implementation

Improve the existing Resume Editor content sections.

### Personal Information

Keep the existing fields:

* Full Name
* Email
* Phone
* Location
* LinkedIn
* GitHub

Ensure they remain properly integrated with the existing `PersonalInfo` model and existing update/create mutations.

Do not change the backend contract.

### Professional Summary

Keep the existing Professional Summary textarea.

Improve its presentation within the Resume Editor if appropriate, while preserving the existing form behavior and validation.

Do not introduce a rich-text editor or another new UI library.

### Skills

Keep the existing MasterSkill normalization workflow:

```text
MasterSkill catalog
        ↓
CustomSelect
        ↓
Proficiency level
        ↓
Attached skills
```

Improve the presentation and usability of the attached skills where appropriate.

The UI should make it clear:

* which skills are attached;
* which proficiency level each skill has;
* how a skill can be removed;
* how another skill can be attached.

Reuse the existing `MasterSkill` API and `CustomSelect`.

Do not replace the normalized MasterSkill architecture with free-text skills.

### Resume Editor Structure

Preserve the existing structure created by the previous Resume Editor implementation:

```text
Resume Editor
│
├── Resume Information
│
├── General
│   ├── Personal Information
│   └── Professional Summary
│
├── Skills
│
├── Experience      (future)
├── Education       (future)
├── Projects        (future)
└── Languages       (future)
```

Do not implement Experience, Education, Projects, or Languages in this task.

Do not redesign the Resume Editor page or replace its navigation.

## API & State

* Use the existing Axios client.
* Use the existing TanStack Query hooks.
* Preserve the existing create and update mutation behavior.
* Preserve existing cache invalidation patterns.
* Preserve loading, empty, success, and RFC 7807 ProblemDetails error handling.
* Do not modify backend code.
* Do not create new API endpoints.
* Do not introduce a new state-management solution.

## UI

* Preserve the existing App Shell and Resume Editor page layout.
* Preserve the existing Resume Information section.
* Preserve the existing top page header and navigation.
* Preserve the existing responsive behavior.
* Reuse existing Shadcn UI components, Lucide icons, CustomSelect, and design tokens.
* Improve only the Personal Information, Professional Summary, and Skills sections where there is a clear UX benefit.
* Do not introduce new UI, routing, API, or state-management libraries.
* Do not turn the Resume Editor back into a modal.

## Scope

Implement only the improvement of the existing Resume Editor content sections:

1. Personal Information
2. Professional Summary
3. Skills

The following are explicitly OUT OF SCOPE:

* Resume routing changes;
* Resume List redesign;
* Resume metadata redesign;
* new revision workflow;
* revision history;
* Experience;
* Education;
* Projects;
* Languages;
* backend changes;
* new API endpoints;
* authentication changes;
* AI resume import;
* PDF storage/import;
* tests.

Do not expand the task into unrelated features.

Do not introduce a new architectural pattern when an existing project pattern already applies.

## Testing Boundary

Testing is a separate task and is explicitly OUT OF SCOPE.

Do NOT:

* create or modify test files;
* add test projects;
* create test components or test utilities;
* generate unit, integration, or E2E tests;
* delegate testing work to `@react-tester` or another testing subagent.

You may only run the existing frontend test suite if appropriate, without modifying test-related files.

## Verification

Run:

* `cd src/frontend && npm run build`
* `cd src/frontend && npm run lint`

Verify that the frontend builds successfully and linting passes.

## Completion Report

Report:

* files created or modified;
* changes made to Personal Information;
* changes made to Professional Summary;
* changes made to Skills;
* API/state integration preserved;
* verification result;
* architectural or implementation issues discovered;
* recommended Git commit message.

Do not modify backend files.

Do not implement future Resume sections.

Stop after the existing Resume Editor content sections have been improved and verified.
