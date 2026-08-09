Read AGENTS.md, ADR.md, DOMAIN_MODEL.md, ARCHITECTURE.md, and MEMORY.md.

Execute 24.3: Domain & Application Unit Testing with xUnit and FluentAssertions.

Goal:
Use the @dotnet-tester subagent to create automated unit tests that verify the Domain invariants (ADR 005 State Machine, Draft Immutability, Copy-on-Write) and Application CQRS behavior (ADR 006 Skill Normalization) implemented in 24.1 and 24.2.

Important:
The production implementation from 24.1 and 24.2 is considered complete.
Do not refactor or redesign production code merely to make tests easier.
Only modify production code if a test reveals a genuine violation of an existing domain invariant.

Requirements:

1. Create Test Projects under src/tests/:
   - CareerPulse.Domain.Tests (.NET 9 xUnit test project referencing CareerPulse.Domain)
   - CareerPulse.Application.Tests (.NET 9 xUnit test project referencing CareerPulse.Application)

2. Add Test Dependencies to test projects:
   - xunit (2.9.x)
   - xunit.runner.visualstudio (2.8.x)
   - Microsoft.NET.Test.Sdk (17.11.x)
   - FluentAssertions (6.12.x)
   - EntityFrameworkCore.InMemory / MockQueryable for IApplicationDbContext testing

3. Add test projects to solution:
   - dotnet sln "src/backend/CareerPulse.sln" add "src/tests/CareerPulse.Domain.Tests/CareerPulse.Domain.Tests.csproj"
   - dotnet sln "src/backend/CareerPulse.sln" add "src/tests/CareerPulse.Application.Tests/CareerPulse.Application.Tests.csproj"

4. Implement Domain Unit Tests:

   ### ApplicationStatusMachineTests
   - Verify every valid transition defined by ApplicationStatusMachine succeeds.
   - Verify every invalid transition (e.g. Draft -> Offer) throws DomainException with descriptive message.
   - (Note: Do not test RFC 7807 here; that is handled by API middleware).

   ### ResumeRevisionTests
   - Verify CreateDraft() initializes Version = 1 and Status = Draft.
   - Verify MarkAsApplied() transitions Draft -> Applied.
   - Verify UpdateSummary(), UpdatePersonalInfo(), SetFileReference(), AddSkill(), RemoveSkill() throw DomainException when Status == Applied.

   ### Copy-on-Write Tests (ADR 005)
   - Verify Applied revision -> SpawnNewVersion() succeeds, producing Status = Draft, Version = parent.Version + 1, ParentRevisionId = parent.Id.
   - Verify spawned revision copies PersonalInfo, ProfessionalSummary, and Skills, but resets FileReference to null.
   - Verify original Applied revision remains untouched.
   - Verify Draft revision -> SpawnNewVersion() throws DomainException ("Only an Applied ResumeRevision can spawn a new version.").

5. Implement Application Unit Tests:

   ### ResolveSkillsQueryHandlerTests (ADR 006 & ADR 007)
   - Verify canonical skill names and aliases resolve correctly (case-insensitive & trimmed).
   - Verify unknown skills are categorized with status NeedsUserInput.
   - Verify auto-matched skills are categorized with status AutoResolved.
   - Verify handler is read-only and does not persist new MasterSkill records.

6. Verification:
   - Run: dotnet test "src/backend/CareerPulse.sln" -c Release
   - Run: dotnet build "src/backend/CareerPulse.sln" -c Release
   - All tests must PASS with 0 failures and 0 compilation errors.

7. Completion Summary:
   Provide a structured summary containing:
   - test projects created;
   - test files created;
   - total test count & execution results;
   - recommended Git commit message.

Stop after 24.3 is fully completed and verified. Do not proceed to 24.4.
