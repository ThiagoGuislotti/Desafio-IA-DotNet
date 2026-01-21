# Global Instructions

Language: pt-BR for chat, READMEs, auxiliary docs, and code comments; EN for code (identifiers/APIs), commits, UI keys/structure, and database schema; pt-BR i18n output.

# Language Policy
- Chat/Conversation: pt-BR (Portuguese) - all responses to user in chat
- Code: EN (English) - identifiers and public APIs
- Code comments: pt-BR (Portuguese)
- Docs/README: pt-BR (Portuguese) - README and auxiliary docs
- Commits: EN (English) - imperative, short
- UI: EN (English) keys/structure; pt-BR translations via i18n for end users
- Database: EN (English) - schema, table names, column names

# Hierarchy and Scope
- Global rules live here and are always applied.
- Domain instruction files extend these rules; do not duplicate globals.
- Prefer the most specific domain rule when conflicts occur.
- Map and reference new instruction files here.

# Context Selection

## Hard rule
- Always load `AGENTS.md` first, then this file.
- If the workspace is available, do not proceed unless both files are loaded.

# Static RAGs Routing
Preferred default workflow: Route -> Execute (always route first to generate a minimal Context Pack).

Use static routing when you want consistent instruction selection without running any external service.

Flow (two-stage):
1) Route: Use .github/instruction-routing.catalog.yml + .github/prompts/route-instructions.prompt.md to produce a Context Pack (mandatory + minimal domain files).
2) Execute: Perform the actual task using ONLY the Context Pack files as context.

Rules:
- Always include mandatory context (AGENTS.md + this file).
- Prefer 2-5 domain instruction files per task.
- If routing files are missing, proceed with mandatory context + relevant .github/instructions/*.md.

## Decision Quickstart (Instruction Hierarchy)

Follow this order of operations on every task:

1) Read the user request and identify the target area
- `.github/**` (policies, prompts, instructions)
- Code workspace (C#/.NET)
- Build/CI/CD/infra (Docker)

2) Apply instructions in this precedence order
- User prompt (explicit constraints)
- AGENTS.md + this file
- Domain instruction files under `.github/instructions/` (pick by language/folder)

3) Resolve conflicts
- More specific scope wins (narrower `applyTo` beats broader)
- Prefer safer/minimal changes when ambiguous, and ask 1-3 clarifying questions if needed

# Workflow

## How to use
- Start with AGENTS.md for repo-specific details (stack, folders, commands).
- Use this file for global rules and technology mappings.
- Follow domain-specific files in .github/instructions/*.md for technical details.

# Instruction Files
- `.github/instructions/dotnet-csharp.instructions.md`
- `.github/instructions/clean-architecture-code.instructions.md`
- `.github/instructions/backend.instructions.md`
- `.github/instructions/database.instructions.md`
- `.github/instructions/orm.instructions.md`
- `.github/instructions/docker.instructions.md`
- `.github/instructions/e2e-testing.instructions.md`
- `.github/instructions/readme.instructions.md`

# Transparency

## Pragmatic use
- List applied instructions only when there are relevant actions (plans, command executions, patches/file changes).
- Use a short preamble to indicate key instructions before tool/command calls; omit in purely informational answers.
- For auditing, consolidate the full list of instructions in PR/commit body or CHANGELOG.md.
- When requested, include an Applied instructions section with the actually used set.

# Security
- No secrets in repo; use environment variables or secret stores.

# Changelog
- No project changelog is required unless the user asks; do not create one by default.

# STYLE (EOF and whitespace)
- Do not leave a trailing blank line at the end of files.
- For files under `.github/instructions/*.md` and Copilot/Codex instruction outputs: do NOT include a final newline (consistent with AGENTS.md).
- For other files, follow `.editorconfig` rules (final newline usually enforced); always avoid trailing whitespace.

# Coding Conventions

## If statements
- Single-line `if` statements that contain only `return` or `throw` may omit curly braces.
- Use curly braces `{}` whenever:
  - there is an `else`
  - the condition is complex
  - the block may reasonably grow in the future

## XML Documentation
- All public classes, methods and properties must have XML documentation.
- Constructors must have XML documentation with a short description (e.g. "Construtor").
- Private or internal members must be documented only when they contain non-obvious business rules.