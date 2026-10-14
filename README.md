# Schoolnet Training Management

ASP.NET Core 10 MVC application with C# controllers, Razor `.cshtml` views and PostgreSQL. The dashboard follows the supplied layout with Schoolnet's blue/green branding and official logo. Requirements: [PRD](PRD_Doc/Training_Management_PRD.md).

## Run locally

1. Install .NET SDK 10 and PostgreSQL 18 (PostgreSQL 14+ supports the SQL used here).
2. Create a database named `Testdb`. The application uses its own `schoolnet` schema and does not overwrite other schemas.
3. Copy `Schoolnet.WEBUI/appsettings.Local.example.json` to `Schoolnet.WEBUI/appsettings.Local.json`, set the PostgreSQL credentials and choose a strong bootstrap password. Local configuration is ignored by Git. For this workstation it has already been configured.
4. Run `./run.ps1 -Initialize` to install the schema and first administrator. Optionally add `-Seed` to create local demo projects, schools, plans, trainees and batches. Seed is idempotent and intended for local development.
5. Run `./run.ps1`, then visit http://localhost:5080. Sign in as `admin` using `Bootstrap:Password` from the local configuration.

Optional demo users `coordinator`, `trainer`, and `trainee` use the same bootstrap password **only in local sample data**. Replace their passwords through Users & permissions before other use. Administrator can create users, assign coordinator project permissions, and link Trainee logins to profiles. Profile usernames are separate from login credentials.

## Structure

- `Schoolnet.Domain`: read models and repository contract.
- `Schoolnet.Repository`: Npgsql pooled data access and parameterized routine calls.
- `Schoolnet.WEBUI/Controllers`: MVC request handling, authorization and validation.
- `Schoolnet.WEBUI/Views`: Razor layouts, dashboard, directories, plans, batches, attendance, feedback and reports.
- `Schoolnet.WEBUI/wwwroot`: responsive Schoolnet styling, local logo and client interactions.
- `database/001_schema.sql`: tables, views, constraints, triggers, functions and stored procedures.
- `database/002_demo.sql`: optional local sample data.
- `database/003_modify_trainee.sql`: non-destructive installation of audited trainee contact corrections; included by `./run.ps1 -Initialize`. Existing installations can apply this SQL file directly to `Testdb` without seeding or resetting data. Normal startup does not apply migrations.
- `tests`: database integrity and HTTP workflow checks.

## Database operations

The repository uses functions `save_master`, `save_plan`, `revise_plan`, and `save_batch` when a new identifier must be returned. PostgreSQL procedures `publish_plan`, `archive_plan`, `assign_trainees`, `remove_assignment`, `transition_batch`, `save_attendance`, and `submit_feedback` handle transactional workflows. Every submitted value is parameterized. Permissions are checked both in controllers and routines. Application access uses the locally supplied database account; a production deployment should use a dedicated database role.

Shared plan headers and details have no calendar dates. Batches reference existing Published versions. Published or referenced content is immutable; revisions create Draft versions. Roster changes initialize Not marked attendance per assignment/day. Attendance has revision checks, batch-specific keys, future-date restrictions and correction reasons. Feedback retains versioned questions, separate initial/final submissions, attribution, locked final responses and audited corrections. Batch completion requires all attendance to be marked and an elapsed end date; feedback is independent.

Create training plan defaults to **four days**: **Pre form**, **Classroom training**, **Classroom training**, and **Feedback**. Changing the duration automatically shows exactly that many activities; the server rejects inconsistent submitted row counts. Boundary forms use the existing versioned feedback engine, and existing Published plans keep their original saved activities.

Scheduling serializes access to trainee rows before rechecking overlaps. Scheduled dates and roster lock. Scheduled batches display In progress once the start date is reached; operations use the application day in Asia/Kolkata. CSV exports respect record permissions and escape spreadsheet formula prefixes.

In the Trainees directory, **Modify** lets administrators and authorized project coordinators correct a trainee's name, mobile number and email address. A correction reason is required and retained with the old and new values in the audit history. Contact corrections are available during scheduled training and for existing inactive project/school associations; username, login, status, associations and training records stay unchanged. Use the existing **Edit** action for other permitted profile changes.

## Build and verify

`dotnet build Schoolnet.slnx`

`dotnet run --project tests/AttendancePolicyChecks` checks attendance date availability at the India-midnight boundary and preserves role/lifecycle restrictions. Future dates are read-only in the page and rejected by both the MVC save action and PostgreSQL, including for administrators.

`tests/verify-database.ps1` runs transactional database acceptance checks and rolls back its fixtures. `node tests/verify-http.mjs` checks authenticated screens, role scope, anti-forgery protection, exports and form workflows against the running local server. Local credentials are read from ignored configuration and never printed.

For browser checks, install the workspace-local helper once with `npm install --prefix .local/browser playwright --cache .local/npm-cache --no-audit --no-fund`, then run `node tests/verify-browser.mjs`. It uses installed Microsoft Edge and saves desktop/mobile screenshots in ignored `artifacts`.

## Operational notes

The configured product time zone is Asia/Kolkata; database routine date gates use this zone. Demo records are labeled and use example.com emails. Passwords use ASP.NET Core PasswordHasher; cookie sessions are revalidated against active users and user changes. Login is rate-limited and all POST forms require anti-forgery tokens. Production mode uses HTTPS redirection/HSTS, generic error pages and bounded hosts. Encryption keys are stored in ignored `.keys`; protect and persist these in deployment. Configure TLS, a dedicated database account, backups/restoration, retention policy and load testing before production. The PRD's 50-user performance target requires a separate load test.

Schoolnet brand reference: https://www.schoolnetindia.com/ . The logo is used for the requested internal application; no third-party CDN is needed at runtime.
