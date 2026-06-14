# Coding & Repo Conventions

These are agent-facing guidelines collected from repository instruction files.

- Never migrate git to reftable. (from git.instructions.md)
- Follow project-specific AGENTS.md guidance for editing backend and library projects.
- Prefer minimal, targeted edits that preserve existing style and public APIs.

When creating or editing files:

- Add necessary imports and update dependency manifests where appropriate.
- For C# code, follow existing DI (dependency injection) patterns in `backend/` projects.
- For front-end work, prefer local package scripts and the monorepo tooling in `frontend/turborepo-zizibot-console`.

