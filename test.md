Read AGENTS.md, ADR.md, DOMAIN_MODEL.md, ARCHITECTURE.md, and MEMORY.md.

Task: Implement the Resume Editor page frontend feature.

## Feature

Replace the current Resume create/edit modal workflow with a dedicated Resume Editor page.

The current Resume UI uses `ResumeFormModal.tsx` as the main Resume editor. Replace this workflow with:

* `/resumes/new` — create a new Resume;
* `/resumes/:resumeId` — edit an existing Resume.

The Resume Editor must clearly separate:

1. Resume-level metadata:

   * Name
   * Target Role
   * Track
   * Career Level

2. Current ResumeRevision content:

   * Personal Information
   * Professional Summary
   * Skills

Reuse the existing Resume functionality from `ResumeFormModal.tsx`; do not rewrite the feature from scratch.

The implementation must follow the existing frontend architecture, conventions, and design system defined in AGENTS.md and ADR 009.

## Before Implementation

* Inspect the existing Resume List page.
* Inspect the current Resume routing.
* Inspect `ResumeFormModal.tsx`.
* Inspect existing Resume API functions/hooks.
* Inspect existing Resume TypeScript DTOs.
* Inspect the corresponding backend controllers, DTOs, commands, handlers, and queries to understand the existing API contract.
* Reuse existing components, hooks, types, utilities, and patterns wherever applicable.

## Implementation

### Resume Editor

Create the minimum page/component structure required for:

`/resumes/new`

and:

`/resumes/:resumeId`

The page must contain:

### Resume Metadata

* Resume Name
* Target Role
* Track
* Career Level

### Current Revision

Move the existing functionality from `ResumeFormModal.tsx` into the page:

* Full Name
* Email
* Phone
* Location
* LinkedIn
* GitHub
* Professional Summary
* MasterSkill selection
* Skill proficiency
* Attached skills

Do not embed `ResumeFormModal` inside the new page.

### Create

For `/resumes/new`:

* use the existing `CreateResumeDraftDto`;
* create the Resume using the existing backend API;
* after successful creation, navigate to `/resumes/:resumeId` using the identifier returned by the existing API response;
* use existing TanStack Query patterns.

### Edit

For `/resumes/:resumeId`:

* load the existing Resume using the existing API;
* display its metadata and current revision;
* update the current draft revision using the existing update API;
* do not create a new revision when editing the current draft.

### Resume List

Update the existing Resume List so that:

* Create Resume → `/resumes/new`;
* Edit/View Resume → `/resumes/:resumeId`.

Once the new workflow is working, remove the old Resume create/edit modal usage.

Delete `ResumeFormModal.tsx` only if it is no longer used anywhere.

## API & State

* Use the existing Axios client.
* Use existing TanStack Query hooks.
* Create new hooks only when an existing hook does not provide an existing backend operation.
* Follow existing query-key and cache-invalidation patterns.
* Handle loading, empty/not-found, success, and RFC 7807 ProblemDetails error states.
* Do not introduce a new global state-management solution.

## UI

* Follow the existing App Shell.
* Follow the existing responsive layout.
* Follow the existing light/dark theme.
* Reuse existing Shadcn UI components.
* Reuse existing Lucide icons.
* Reuse existing design tokens and form patterns.
* Reuse the existing MasterSkill/CustomSelect functionality.
* Do not introduce new UI or state-management libraries.

The Resume Editor should look like a normal CareerPulse page, not like a large modal stretched across the screen.

## Backend Boundary — STRICT

This is a FRONTEND-ONLY task.

The entire `src/backend/` directory is READ-ONLY.

You may inspect backend files to understand the API contract.

You MUST NOT modify:

* backend controllers;
* backend DTOs;
* application handlers;
* domain entities;
* EF Core configurations;
* database configuration;
* migrations;
* any other file under `src/backend/`.

If you discover a backend issue or missing API capability:

1. do NOT modify the backend;
2. do NOT create a workaround by changing backend code;
3. report the exact issue in the Completion Report.

A backend change is NEVER part of this task.

## Scope

Implement ONLY the migration from Resume modal editing to the dedicated Resume Editor page.

Do NOT implement:

* Experience;
* Education;
* Projects;
* Languages;
* Revision History;
* revision comparison;
* Create New Version;
* PDF generation;
* PDF import;
* AI resume import;
* Resume templates;
* application/job features;
* backend changes.

Do not refactor unrelated frontend code.

## Testing Boundary

Testing is a separate task and is explicitly OUT OF SCOPE.

Do NOT:

* create or modify test files;
* add test projects;
* create test utilities;
* generate unit, integration, or E2E tests;
* delegate testing to `@react-tester`.

You may only run the existing frontend test suite if appropriate, without modifying test-related files.

## Verification

Run:

* `cd src/frontend && npm run build`
* `cd src/frontend && npm run lint`

Verify the following:

* `/resumes/new` opens the new Resume Editor;
* Resume metadata can be entered;
* current revision fields can be entered;
* existing MasterSkill functionality still works;
* creating a Resume succeeds;
* successful creation navigates to `/resumes/:resumeId`;
* existing Resume data loads on `/resumes/:resumeId`;
* the current draft revision can be edited;
* Resume List uses the new routes;
* the old Resume create/edit modal is no longer used.

## Completion Report

Report:

* files created or modified;
* routes added or changed;
* Resume Editor functionality implemented;
* functionality migrated from `ResumeFormModal.tsx`;
* API/hooks used;
* whether `ResumeFormModal.tsx` was removed;
* build and lint results;
* any backend/API limitation discovered;
* recommended Git commit message.

Stop after this feature is fully implemented and verified.
