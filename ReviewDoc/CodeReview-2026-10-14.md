# Schoolnet application code review

Reviewed on 14 October 2026 against `PRD_Doc/Training_Management_PRD.md` version 1.2 and repository guidance. Scope includes MVC controllers, authentication, domain accessors, repository queries, PostgreSQL workflows, Razor forms, and the current verification scripts. The reviewed workspace is based on commit `f41fef2` and includes uncommitted Modify Trainee and attendance changes. Only this review document is committed; application changes remain in the workspace.

Four findings need attention: trainee login reassignment can expose existing history, the older edit route bypasses correction requirements, attendance revision checks accept missing values, and trainee edits can race with scheduling. Passing acceptance checks do not cover these cases.

## Findings

### R1 Restrict changes to trainee login ownership

**Priority:** P1 high. **Evidence:** reproduced in a database transaction that was rolled back.

**Locations:** `database/001_schema.sql:151-153`, `database/001_schema.sql:74-75`, `Schoolnet.Repository/TrainingRepository.cs:59`, `Schoolnet.WEBUI/Views/Masters/Edit.cshtml:12`.

The general trainee editor lets a scoped coordinator replace `user_id`, including for a trainee with Scheduled or In progress assignments. Validation checks only that the selected login is active and has the Trainee role. The login choices include every active trainee login, without project scope. Personal access checks resolve ownership from the trainee's current `user_id`, so changing this link also transfers access to existing batches, reports, and feedback. The unique constraint prevents using an already linked login but does not prevent reassignment to an unrelated unlinked login.

**Reproduction:** Create an unrelated active, unlinked Trainee login. Confirm `can_batch(new_login, existing_batch)` is false. As a coordinator scoped to the trainee's project, call `save_master('trainee', trainee_id, payload_with_new_user_id, coordinator_id)` while retaining the profile's other fields. The save succeeds and `can_batch(new_login, existing_batch)` becomes true. The application submits this same field through `/Masters/Edit?kind=trainee`.

**Impact:** A coordinator can change which person receives access to an existing trainee's training history, without the administrator user-management workflow or a correction reason. This affects the server-side record and login permission requirements and AC20.

**Suggested fix:** Separate login binding from ordinary profile maintenance. Require an explicit administrator-authorized operation with a reason, validate the intended person's identity, and define how existing history should behave when a login is replaced. Scope the choices accordingly and enforce the rule in the database routine. Add a coordinator POST regression check and verify that an unrelated login cannot gain historical access.

### R2 Enforce contact correction rules on the existing edit route

**Priority:** P2 medium. **Evidence:** reproduced in a database transaction that was rolled back.

**Locations:** `Schoolnet.WEBUI/Controllers/MastersController.cs:78-99`, `database/001_schema.sql:146-153`, `database/003_modify_trainee.sql:10-17`.

`ModifyTrainee` requires a correction reason and bounds name and email lengths, but the existing `/Masters/Edit?kind=trainee` POST remains available to Administrators and Coordinators. That action calls `save_master`, which updates the same contact fields without requiring a reason or applying the new length limits. `save_master` calls `set_actor(p_actor)` without a reason, so even a supplied `reason` does not become the audit reason. Replacing the directory link does not remove this direct route.

**Reproduction:** Send the existing trainee edit payload with valid project, school, username, mobile, email, status, and login values; change the name and omit `reason`. The update succeeds, including on a scheduled trainee, and the trainee UPDATE audit entry has an empty reason. The rollback probe confirmed this as a scoped coordinator.

**Impact:** Users can bypass the correction policy through an older endpoint, producing contact history without an explanation. Validation differs depending on the route used. This violates the repository requirement to require correction reasons.

**Suggested fix:** Apply the same contact validation and audit reason requirements to every update path, preferably in shared database validation. Keep creation separate from correction semantics. Test valid-token POSTs to both endpoints with blank reasons and oversized contact values; rejected updates must preserve the record and audit history.

### R3 Reject missing attendance revisions before comparing them

**Priority:** P2 medium. **Evidence:** reproduced in a database transaction that was rolled back.

**Locations:** `Schoolnet.WEBUI/Controllers/AttendanceController.cs:28`, `database/001_schema.sql:272`.

The controller allows an attendance row to reach the procedure with a missing revision via `ElementAtOrDefault`. The procedure compares `old.revision <> (r->>'revision')::integer`. An omitted or JSON-null revision produces SQL NULL, and the PL/pgSQL `IF` condition does not raise the stale-write error. The update then increments the revision normally.

**Reproduction:** Submit an otherwise authorized attendance change with an ID, valid status, and any required correction reason, but omit `revision`. The rollback probe verified that `save_attendance` accepts the update and increments the stored revision. The existing stale-revision test supplies an integer and does not exercise this case.

**Impact:** A malformed or deliberately changed POST can overwrite another session's attendance update without passing the required concurrency check. This conflicts with FR14.

**Suggested fix:** Require a non-null, valid integer revision for every submitted row and use `IS DISTINCT FROM` when comparing it to the stored revision. Validate matching form-array counts in the controller. Add missing-revision, null-revision, and stale-revision checks and confirm that each failure leaves all submitted rows unchanged.

### R4 Serialize trainee eligibility edits with batch scheduling

**Priority:** P2 medium. **Evidence:** static analysis of transaction and lock ordering; a concurrent reproduction was not run.

**Locations:** `database/001_schema.sql:147-153`, `database/001_schema.sql:229`, `database/001_schema.sql:243-249`.

Assignment and scheduling lock trainee rows with `FOR UPDATE`, but the general trainee edit checks Scheduled/In progress assignments before acquiring a trainee row lock. Its later UPDATE obtains the lock only after the permission, association, and active-assignment checks have already passed.

**Failure sequence:** Session A starts scheduling a Draft batch and locks its trainees. Session B checks the trainee while the batch still appears Draft, so the active-assignment check passes; its UPDATE then waits for A's trainee lock. A validates the original trainee, schedules the batch, and commits. B resumes the already validated UPDATE and can change the project, school, or active status without rechecking the newly scheduled assignment.

**Impact:** A batch can become Scheduled with a roster member who is no longer active or matches a different project or school. FR04 and FR10 require these associations to remain valid and scheduling to be atomic.

**Suggested fix:** Lock the existing trainee before reading its current project and checking assignments. Coordinate lock ordering with scheduling and assignment routines. Add a test using two database connections that races scheduling against project/school changes and deactivation; one operation must reject the conflicting final state.

## Verification results

| Check | Result |
| --- | --- |
| `dotnet build Schoolnet.slnx --no-restore` | Passed with zero warnings and zero errors. |
| `./tests/verify-database.ps1` | Passed; acceptance fixtures rolled back. |
| `node tests/verify-http.mjs` | All 52 HTTP checks passed. |
| `node tests/verify-browser.mjs` | Passed, including mobile layout, form saves, plan resizing, and attendance availability. Screenshots are in ignored `artifacts`. |
| `dotnet run --project tests/AttendancePolicyChecks --no-restore` | All 14 attendance policy checks passed. |
| Targeted review probes | R1, R2, and R3 reproduced; fixture rows and updates rolled back. |

The browser suite performs saves against demo records and can create or publish its demo plan. The existing checks establish baseline behavior, but do not test the bypasses above or concurrent scheduling against a trainee edit. No application source files were changed as part of this review.

## Product clarification

The PRD role table permits Trainers to enter authorized offline responses, while FR18 specifies authorized Coordinators. The current feedback query and submission routine use `can_project`, which excludes Trainers, so even the assigned trainer cannot open another trainee's feedback form. Clarify the intended role before treating this as a defect. If Trainers should enter offline responses, both the read query and submission procedure need a batch-scoped permission with a required reason and attribution; trainer corrections should remain a separately defined permission.
