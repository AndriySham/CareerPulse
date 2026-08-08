Read AGENTS.md, ADR.md, DOMAIN_MODEL.md, ARCHITECTURE.md, and MEMORY.md.

Execute unit testing for [FEATURE_NAME].

## Feature Under Test

[SHORT_DESCRIPTION_OF_FEATURE]

The implementation already exists in the workspace.

Relevant source location(s):

- [SOURCE_PATH_1]
- [SOURCE_PATH_2]
- [SOURCE_PATH_3]

Relevant documentation, if applicable:

- [DOCUMENTATION_FILE_1]
- [DOCUMENTATION_FILE_2]

Inspect the actual implementation before creating tests. The source code in the workspace is the primary source of truth.

## Test Project

Use the existing test project:

[TEST_PROJECT_PATH]

Before creating files, inspect the existing test structure and reuse its test infrastructure, helpers, fixtures, namespaces, and conventions where appropriate.

Do not create a duplicate test project or duplicate test infrastructure.

## Testing Task

Use `@dotnet-tester` to analyze [FEATURE_NAME] and create the appropriate unit tests for the existing implementation.

Relevant components may include:

- [DTOs]
- [Commands]
- [Queries]
- [Handlers]
- [Validators]
- [Domain Entities]

Determine the actual test coverage from the implementation rather than from this prompt.

Do not prescribe individual test cases unless a specific business requirement must be explicitly verified.

## Scope

Test only:

[FEATURE_NAME / FEATURE_SCOPE]

Do not expand the task to unrelated features.

## Testing Type

This task is limited to unit tests.

[OPTIONAL: ADD SPECIFIC TESTING BOUNDARY IF NEEDED]

## Verification

Complete and verify the resulting test suite according to the existing `@dotnet-tester` testing workflow.

## Completion

Return the standard `@dotnet-tester` test report.

Stop after [FEATURE_NAME] unit testing is fully completed and verified.