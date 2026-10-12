# Repository guidance

## Purpose and architecture

Build and maintain the Schoolnet Training Management MVC application according to `PRD_Doc/Training_Management_PRD.md`.

- Solution: `Schoolnet.slnx`, .NET 10 / ASP.NET Core MVC.
- Domain models and repository interface: `Schoolnet.Domain`.
- Parameterized Npgsql data access: `Schoolnet.Repository`.
- Controllers, Razor `.cshtml` views and static assets: `Schoolnet.WEBUI`.
- PostgreSQL schema, views, triggers, functions and procedures: `database`.
- Local database: `Testdb`, application schema `schoolnet`.

## Build and run

Run commands from the repository root. On restricted Windows sessions, set `DOTNET_CLI_HOME` to `.dotnet` inside the workspace and use `.nuget/packages` for package restore.

```powershell
$env:DOTNET_CLI_HOME = Join-Path $PWD '.dotnet'
$env:NUGET_PACKAGES = Join-Path $PWD '.nuget/packages'
dotnet restore Schoolnet.slnx --packages .nuget/packages
dotnet build Schoolnet.slnx --no-restore
./run.ps1 -Initialize -Seed
./run.ps1
```

Local URL: `http://localhost:5080`. Initialization is explicit; normal startup does not overwrite database records. Use the existing ignored `Schoolnet.WEBUI/appsettings.Local.json`; never print its secrets. Administrator username is `admin`; its initial password is stored under `Bootstrap:Password`. Demo usernames are `coordinator`, `trainer`, and `trainee`.

## Required invariants

- Training plans are reusable, versioned, and numbered by day. Neither plan table stores calendar dates.
- New plans label Day 1 as Pre form and Day N as Feedback; both use the boundary feedback form engine. Intermediate days are Classroom training. Minimum duration is two days.
- Create training plan defaults to four days and must show/save exactly N activities for a selected duration N; changing duration synchronizes the rows automatically.
- Batches reference one existing Published plan; sharing a plan must never share attendance or feedback.
- Published/referenced plan content is immutable. Revision creates a new Draft version.
- Attendance and feedback reference batch assignments and the selected plan's detail rows.
- Attendance starts as Not marked. Feedback never marks attendance.
- Enforce project, school, login-role and record permissions on the server, including direct URLs and exports.
- Preserve history, audit changes, require correction reasons, and keep roster/date/lifecycle operations atomic.
- Parameterize every request value. Dynamic SQL/routine names must come from fixed code whitelists.
- Keep secrets, build artifacts, `.vs`, local configuration and encryption keys out of Git.
- Preserve PRD and existing user documentation; do not push application changes or change Azure DevOps task status unless requested.

## Validation

```powershell
./tests/verify-database.ps1
node tests/verify-http.mjs
node tests/verify-browser.mjs
```

Database checks require initialized demo data and roll back all fixtures. HTTP/browser checks require a running app. Browser checks use local Playwright under `.local/browser` and installed Edge; screenshots go to ignored `artifacts`. Test meaningful database constraints, access boundaries, form saves and responsive behavior for relevant changes. Verify build and inspect failures before handoff.

If the browser helper is missing, install it with `npm install --prefix .local/browser playwright --cache .local/npm-cache --no-audit --no-fund`. The database test script expects PostgreSQL tools under `C:/Program Files/PostgreSQL/18/bin`; adjust that path when using a different installation.

## UI conventions

Preserve Schoolnet branding, local official logo, blue/green palette, sidebar, metric cards, filter bar, and readable tables. Keep forms labelled, keyboard accessible and responsive. Do not show implementation details in ordinary product screens. Show explicit validation errors and honest empty states.

## Project agents

Project-specific agents are defined in `.codex/agents/`:

| Agent | Responsibility | Primary files |
| --- | --- | --- |
| `csharp_developer` | C# backend, MVC authorization, repositories and PostgreSQL workflows | Domain, Repository, Controllers, Program.cs, database |
| `html_developer` | Razor views, Schoolnet styling, accessibility and client interactions | Views, wwwroot |
| `tester` | PRD acceptance, database integrity, HTTP permissions and browser verification | tests, ignored artifacts |
| `code_reviewer` | Read-only review of correctness, security, regressions and test gaps | Assigned application changes |

When the user requests these agents or team work, delegate bounded tasks to the matching role. The parent agent owns integration and the final response. Agree on file ownership before parallel edits; coordinate changes that cross controller/view contracts. Run dependent verification after implementation, then review the final changes. Respect the session's concurrency limit and queue additional roles as needed. Agent models and reasoning settings inherit from the parent session.

Example: `Use csharp_developer and html_developer to implement this feature, then have tester verify it and code_reviewer review the changes.`
