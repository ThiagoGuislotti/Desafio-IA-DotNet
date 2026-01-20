# Copilot Agents and Context Policy

# Agents
- Workspace: code-first agent for this repo. Reads/edits files, runs searches, and proposes patches.
- GitHub: PR/issue-centric agent. Summarizes, reviews, and changes GitHub artifacts.
- Profiler: performance agent. Benchmarks, profiles, and optimizes hot paths.
- VS: IDE helper. Settings, build/debug help, MSBuild/solution issues.

# Mandatory Context Files
- Always include BOTH of these files first when selecting context for Copilot Chat:
  1. AGENTS.md (this file)
  2. .github/copilot-instructions.md

Default workflow for all tasks: Static RAGs Routing (Route -> Execute)
- Route first (pick minimal context) when routing files exist:
  - `.github/instruction-routing.catalog.yml`
  - `.github/prompts/route-instructions.prompt.md`
- Execute next: use ONLY the files returned by the Context Pack.
- If routing files are missing, proceed with mandatory context + relevant `.github/instructions/*.md`.

# Prompt Phases and Code Generation Rules

## Prompt Phases
- Prompt #1 is a planning-only phase.
  - No code generation is allowed.
  - Outputs must be documentation only (architecture, planning, instructions).
- Prompt #2 and onwards may generate code.
  - Code generation must be restricted to the layer defined by the prompt.
  - Always follow the execution order: Domain -> Application -> Infrastructure -> API -> Worker -> Tests.

## Code Generation Scope
- When the prompt scope is documentation or planning, do not generate code.
- When the prompt scope is implementation, generate code only for the target layer.
- Do not anticipate responsibilities of outer layers.

## Unclear Requirements
- If a rule or requirement is not explicitly defined in the challenge documentation,
  register it as a pending item in PLANEJAMENTO.md instead of making assumptions.

# Project Constraints

## Target Architecture
- Reference: `ARQUITETURA.md`

## Execution Order
- Always start from Domain and move outward: Domain -> Application -> Infrastructure -> API -> Tests -> Docker -> Docs.

# Instruction Entry Point (Decision Flow)
Use this section as the quick "what do I load / follow first?" guide.

## Instruction Routing (If Available)
1) Load mandatory context files first
2) Select additional instruction files based on what you are changing:
   - Backend/C# code: `.github/instructions/dotnet-csharp.instructions.md`,
     `.github/instructions/clean-architecture-code.instructions.md`,
     `.github/instructions/backend.instructions.md`
   - Database/ORM: `.github/instructions/database.instructions.md`,
     `.github/instructions/orm.instructions.md`
   - Docker/infra: `.github/instructions/docker.instructions.md`
   - Testing: `.github/instructions/e2e-testing.instructions.md`
   - Docs/README: `.github/instructions/readme.instructions.md`
3) Precedence rules when instructions conflict
   - Follow the user prompt first
   - Then follow AGENTS.md + .github/copilot-instructions.md
   - Then prefer the most specific instruction file by scope/path

# Context Preservation & Execution Patterns

## Session Continuity
- Load previous context at session start: review recent changes and current state
- Maintain architectural patterns and decisions from earlier work
- Preserve Clean Architecture boundaries and established abstractions
- Respect previous technical choices unless explicitly changing approach

## Execution Flow for Development Tasks
1. Task Analysis: Load mandatory context files, identify domain, analyze scope
2. Planning: Create execution plan for non-trivial tasks, validate architecture
3. Implementation: Follow established templates and patterns, maintain standards
4. Validation: Verify compilation, run tests, confirm architectural compliance

### For Multi-Task Requests
- Apply Task-Based Execution Methodology (see below)
- Break complex requests into numbered, sequential tasks
- Validate each task completion before proceeding to next
- Maintain session continuity across task boundaries

## Quality Gates
Before generating code: Context loaded, patterns identified, approach validated
During implementation: Templates followed, naming conventions applied, boundaries respected
After changes: Code compiles, tests pass, architecture maintained, documentation updated

# Task-Based Execution Methodology

## Multi-Task Request Structure
- Break complex requests into numbered, sequential tasks
- Each task should have clear scope, dependencies, and success criteria
- Use format: "Tarefas: 1- [task], 2- [task], 3- [task]"
- Validate completion of each task before proceeding to next

## Task Execution Pattern
1. Task Analysis: Review all tasks, identify dependencies and execution order
2. Task Planning: Confirm approach and tools needed for each task
3. Sequential Execution: Complete tasks in order, validate each step
4. Progress Tracking: Report completion status and any blockers
5. Final Validation: Ensure all tasks completed successfully

## Task Documentation
- Document task completion in session context
- Reference specific files/changes made per task
- Note any deviations from original task specification
- Provide rollback information if tasks need to be undone

# Repository Guidelines

## Overview
Challenge repo for a customer registration platform with deduplication, search, messaging, and observability.

## Structure
- `docker/` infrastructure and observability files
- `src/` solution projects (Api, Application, Domain, Infrastructure)
- `tests/` unit and integration tests
- Root docs: README.md, DESAFIO.md, TEMPLATE_ENTREGA.md, DECISOES_TECNICAS.md, COMO_EXECUTAR.md

## Build, Test & Run
- `dotnet build CustomerPlatform.sln`
- `dotnet test`
- `dotnet run --project src/CustomerPlatform.Api`
- `docker compose -f docker/docker-compose.yaml up -d` (optional infra)

## Testing
- Unit tests in `tests/CustomerPlatform.UnitTests`
- Integration tests in `tests/CustomerPlatform.IntegrationTests`
- Remove `tests/CustomerPlatform.Tests` after replacements

## Documentation
- Keep `ARQUITETURA.md` and `PLANEJAMENTO.md` aligned with the target architecture and execution plan
- Use `TEMPLATE_ENTREGA.md` to create `PROMPTS_UTILIZADOS.md`

## Security
- No secrets in repo; use environment variables or secret stores

# Domain Instruction References
- `.github/instructions/dotnet-csharp.instructions.md`
- `.github/instructions/clean-architecture-code.instructions.md`
- `.github/instructions/backend.instructions.md`
- `.github/instructions/database.instructions.md`
- `.github/instructions/orm.instructions.md`
- `.github/instructions/docker.instructions.md`
- `.github/instructions/e2e-testing.instructions.md`
- `.github/instructions/readme.instructions.md`
