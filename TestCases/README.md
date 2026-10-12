# Schoolnet project test cases

This suite contains **145 written test cases** for the whole Schoolnet Training Management MVC project. It is based on [PRD v1.2](../PRD_Doc/Training_Management_PRD.md), including the four-day **Pre form / Classroom training / Feedback** create-form update, and [repository guidance](../AGENTS.md). Every case starts **Not Run**. Creating these documents does not establish product acceptance or successful execution.

[test-cases.csv](test-cases.csv) is the machine-readable execution register. Each case includes an ID, role, priority, prerequisites, concrete data, steps, expected result, source requirement, existing automation reference, status, actual result, evidence and defect ID. Module Markdown files provide readable case definitions. [TRACEABILITY.md](TRACEABILITY.md) maps all **FR01–FR21** and **AC01–AC23**.

| Module | Cases | Document |
| --- | ---: | --- |
| Authentication and users | 10 | [auth.md](auth.md) |
| Projects, schools and locations | 11 | [master.md](master.md) |
| Trainee profiles and directory | 12 | [trainee.md](trainee.md) |
| Plans, versioning and four-day editor | 17 | [plan.md](plan.md) |
| Feedback templates | 7 | [template.md](template.md) |
| Batch setup and scheduling | 14 | [batch.md](batch.md) |
| Roster and eligibility | 10 | [roster.md](roster.md) |
| Attendance and concurrency | 12 | [att.md](att.md) |
| Initial, final and offline feedback | 11 | [fb.md](fb.md) |
| Lifecycle and dashboard | 10 | [life.md](life.md) |
| Reports and CSV exports | 9 | [report.md](report.md) |
| Permissions and request security | 8 | [sec.md](sec.md) |
| Responsive UI and accessibility | 6 | [ui.md](ui.md) |
| Integrity and operations | 8 | [ops.md](ops.md) |

## Execution conventions for a future authorized test run

Use an approved local/disposable test environment. The normal local URL is `http://localhost:5080`; database `Testdb`, schema `schoolnet`. Read credentials privately from ignored local configuration; never place passwords, connection strings, cookies or antiforgery tokens in evidence. `admin`, `coordinator`, `trainer` and `trainee` are demo usernames, not guaranteed fixture roles/scope for every scenario.

Create a unique test-run prefix such as `QA_20261012_<run-id>` for dedicated fixtures. P/Q are separate active projects, R is inactive; S/T are schools with a valid State → District → Block → Cluster chain. Coordinator P has only project P. Have assigned and unassigned Trainers, two independently logged-in Trainees, an Administrator and an inactive user. Trainee login identity is separate from profile information. P2/P4/P10 are complete two/four/ten-day plans; versioned templates cover required/optional rating, choice and text questions. A/B are independent batches that share the same Published plan. D/E are dedicated fixtures for Draft setup and overlap. Existing identifiers must be discovered, never hard-coded to production or demo records.

T0 means today's date in the configured application time zone. Prepare independent past/current/future fixtures rather than changing workstation time or production data. Use an isolated controllable-clock environment when a single trainee journey requires both boundary dates or midnight tests. Literal November/December 2026 dates in date-calculation cases demonstrate arithmetic only; they are not permission to edit existing records. Thirty trainees and ten days establish the 300-row acceptance fixture; performance data uses 100 trainees and 30 days.

Use `BEGIN`/`ROLLBACK` for direct SQL integrity fixtures. Operations such as invalid day numbering or forbidden histories may require rollback-only SQL fixtures because normal UI creation correctly disallows invalid states. Never reset Testdb, reseed existing data or delete user records. Preserve any persistent dedicated fixtures according to the approved environment cleanup policy; do not use broad prefix deletion. Reuse an existing server where possible and stop only a server process started by the tester.

P0 means critical permission, integrity or acceptance behavior; P1 means ordinary functional/usability behavior; P2 means proposed operational targets or product-decision checks. Execute P0 first, then P1/P2. Mark `Pass`, `Fail`, `Blocked` or `Not Applicable` only after recording actual result and evidence; retain `Not Run` for unexecuted cases. Blocked is appropriate for absent credentials/fixtures/environment or an unconfirmed product decision. Record browser/build/database version, role, date/time/time zone, fixture IDs and precise request/response or screenshot evidence with secrets redacted. Store temporary logs/screenshots under ignored `artifacts` or `.local`. On failure, report exact reproduction steps, expected/actual behavior, unaffected/affected records and a defect link. A suite command does not prove every mapped manual case passed.

## Existing automation references

These files were inspected for mapping. They are independent existing application checks, not created or executed by this written-case suite. No new manual case is marked executed based on their output.

- [verify-database.ps1](../tests/verify-database.ps1) invokes [verify-database.sql](../tests/verify-database.sql), requires initialized fixtures and PostgreSQL tools, and rolls back fixtures. Coverage includes numbered plans, boundaries, reuse, mismatch validation, attendance counts/isolation, stale revisions, feedback separation, plan revisions/switches, permissions and lifecycle. A mapping labelled partial means additional written steps/assertions are absent from that script.
- [verify-http.mjs](../tests/verify-http.mjs) covers demo role login/screen loads, selected validation, missing antiforgery, scoped batch/feedback access and personal CSV content. A successful HTTP 200 alone does not prove save correctness or every permission edge case.
- [verify-browser.mjs](../tests/verify-browser.mjs) covers editor rows/resizing, date calculation, dashboard filtering and mobile navigation. It **saves DEMO-B04**; do not run without authorization for that fixture mutation. It uses local Playwright and Edge. It does not establish all-browser compatibility, accessibility, mobile attendance saving or performance.

If these source files are absent from a checkout, the manual cases still stand; automation references describe the local repository reviewed for this task. Publishing this TestCases folder does not publish or initialize the application, its database or its existing test scripts. Do not install tools or run application tests as part of merely reviewing these documents.

## Direct-request inventory for security cases

Repeat forbidden mutations with a **valid token for the attacking session** so role/record denial is evaluated independently of antiforgery. Separately repeat all POST actions with missing, invalid and cross-session tokens. For denied writes compare the full affected record set and audit history. A login redirect, forbidden result or not-found result may be appropriate; the contract is no unauthorized data or mutation.

| Area | Read routes | POST mutation routes |
| --- | --- | --- |
| Account | `/Account/Login`, protected dashboard | `/Account/Login`, `/Account/Logout` |
| Masters | `/Masters?kind=project/school/trainee/location/user/template`, `/Masters/Edit?kind=...&id=...` | `/Masters/Edit` with each supported kind |
| Plans | `/Plans`, `/Plans/Detail/{id}`, `/Plans/Edit/{id}` | `/Plans/Edit`, `/Plans/Action` publish/revise/archive |
| Batches | `/Batches`, `/Batches/Detail/{id}`, `/Batches/Edit/{id}` | `/Batches/Edit`, `/Batches/Assign`, `/Batches/Remove`, `/Batches/Transition` |
| Attendance | `/Attendance?id=...&day=...` | `/Attendance/Save` |
| Feedback | `/Feedback?assignment=...&day=...` | `/Feedback/Save` |
| Reports | `/Reports`, `/Reports?export=true`, `/Reports/Audit` | None |

Administrator can maintain master/users/plans/templates and view all reports/audit. Coordinator can manage scoped trainee/batch/roster/lifecycle and authorized corrections, while Published shared-plan selection crosses project boundaries. Trainer can read assigned batches/rosters and enter permitted attendance/offline responses. Trainee can read their own profile/batch/attendance and submit only their assigned boundary feedback. Sharing a plan never expands batch/trainee permissions. Test both same-project different-record and outside-project access; test another trainee even within the same batch.

## Document integrity check

`node TestCases/validate.mjs` checks CSV structure, required fields, unique case IDs, status defaults, module counts and all numbered PRD references. It validates documents only and does not start the app, connect to PostgreSQL, run application tests or change records. Product acceptance, load/backup evidence and manual executions remain untested until separately performed.
