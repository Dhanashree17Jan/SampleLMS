# Server permissions and request hardening

Source: [PRD v1.2](../PRD_Doc/Training_Management_PRD.md), including the Create training plan update. All cases are **Not Run**. A listed automation reference indicates source coverage only, never a recorded execution result.

Prerequisites: Separate sessions for all roles; P/Q and own/other trainee fixtures; capture form tokens privately; dedicated records only; use authorized test environment.

Use the [fixture conventions and execution instructions](README.md) before changing dedicated records. The CSV is the authoritative execution register; these Markdown files present the initial case definitions.

## TC-SEC-001 — Anonymous direct pages and exports

- Requirements: Users and permissions; AC20
- Role: Anonymous
- Priority: P0
- Test data: /Masters,/Plans,/Batches/Detail,/Attendance,/Feedback,/Reports?export=true
- Steps: Request all without session; attempt mutation without authentication.
- Expected result: No protected HTML/CSV/contact data; login/denied result; no mutation.
- Existing automation reference: HTTP: anonymous dashboard only
- Initial status: Not Run

## TC-SEC-002 — Missing invalid cross-session antiforgery tokens

- Requirements: Quality and operational requirements
- Role: All roles
- Priority: P0
- Test data: Every POST endpoint in route inventory
- Steps: Submit each with no token, invalid token and token from another session; compare state.
- Expected result: Each rejected before mutation; no save/lifecycle/user change; clear safe error.
- Existing automation reference: HTTP: missing token on Batches/Transition only
- Initial status: Not Run

## TC-SEC-003 — Role and record ID mutation matrix

- Requirements: Users and permissions; AC20
- Role: All roles
- Priority: P0
- Test data: Other-project IDs; other trainee assignment; unassigned batch; forged actor/role fields
- Steps: For each route in README repeat allowed/denied role calls with valid own token; forge IDs/actor fields.
- Expected result: Server derives identity from session; all forbidden record/role changes denied without leaking values.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-SEC-004 — SQL injection and routine whitelist

- Requirements: Quality and operational requirements
- Role: All roles / reviewer
- Priority: P0
- Test data: Search/name/kind/id filters: quote, SQL comments, kind unsupported
- Steps: Submit harmless injection-shaped inputs to listings/forms; inspect parameterized repository/routine name construction.
- Expected result: Inputs treated as data; no broader rows/SQL execution; dynamic identifiers only fixed whitelist; unsupported kinds rejected.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-SEC-005 — Stored reflected script injection

- Requirements: Quality and operational requirements
- Role: Coordinator
- Priority: P0
- Test data: Harmless script-shaped title/remark/name in dedicated fixture
- Steps: Save values; view lists/details/forms/audit; search with same value.
- Expected result: Text HTML-encoded; no script execution or DOM event injection; inputs safely preserved.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-SEC-006 — Sensitive error handling

- Requirements: Quality and operational requirements
- Role: Anonymous/All roles
- Priority: P1
- Test data: Malformed ID, nonexistent ID, unavailable DB in disposable environment
- Steps: Request invalid routes; simulate failure; inspect response and retained state.
- Expected result: Readable safe error/404; no stack trace, connection string, credentials or another record disclosed.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-SEC-007 — Encrypted deployment transport and secure cookie policy

- Requirements: Quality and operational requirements
- Role: Operator
- Priority: P0
- Test data: Production-like HTTPS deployment
- Steps: Inspect HTTP→HTTPS redirects, auth cookie Secure/HttpOnly/SameSite and login behavior.
- Expected result: Credentials transported encrypted; cookies restricted as configured; no downgrade disclosure; local HTTP not used as production proof.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-SEC-008 — School and contact confidentiality

- Requirements: Users and permissions; Quality and operational requirements
- Role: Coordinator/Trainer/Trainee
- Priority: P0
- Test data: Same project different school; own/other record
- Steps: Use direct school/trainee/batch URLs and exports with out-of-record IDs.
- Expected result: Permissions enforce assigned project, school and record boundaries; private contact details not leaked.
- Existing automation reference: Manual
- Initial status: Not Run
