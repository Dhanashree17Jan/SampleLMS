# Modify Trainee: name, mobile number and email ID

This supplemental manual suite covers changing an **existing** trainee's name, mobile number and email address. It extends [trainee.md](trainee.md) without changing the existing 153-case CSV register. All **26 cases are Not Run**; actual result, evidence and defect ID are blank. This document records test design, not successful application execution.

Sources: [PRD](../PRD_Doc/Training_Management_PRD.md), Data requirements / Trainee, FR03, FR04, FR21, Users and permissions, AC20; [AGENTS.md](../AGENTS.md). Administrator and project-scoped Coordinator can edit profiles. Trainer and Trainee have no profile-edit permission under the current role contract.

## Preconditions and execution contract

Follow the [existing fixture and evidence conventions](README.md). Use dedicated local QA profiles, not existing user records. Prepare active projects P/Q, eligible schools S/T, Coordinator P with access only to P, Administrator, Trainer, and two linked Trainee logins. Record discovered IDs; never assume demo IDs or passwords. Read credentials privately. Use a unique run prefix in profile usernames and email local parts.

- **A:** P/S profile, name `QA Teacher One`, mobile `9123456789`, email `qa.<run>.one@example.com`, active, linked to Trainee login U1. Record original `id`, `username`, `project_id`, `school_id`, `user_id`, `active`, timestamps and contact values.
- **B:** another P/S profile with different username and contacts; **C:** Q profile outside Coordinator P's scope. Use unique contacts except for explicit duplicate cases.
- Prepare dedicated Scheduled, In progress and Completed assignments for A when required. Record assignment IDs, assignment-time project/school, attendance IDs/status/revisions, feedback IDs/answers and batch/plan references before editing.
- Open `/Masters?kind=trainee`, locate A, then select **Modify**. Direct route is `/Masters/ModifyTrainee?id=<A-id>` (directory link uses `/Masters/ModifyTrainee/<A-id>`). Submit using **Save changes**. Cancel and **Back to directory** navigate to `/Masters?kind=trainee`.
- For direct POST tests, copy the valid Modify Trainee payload and alter only the named test inputs. POST `/Masters/ModifyTrainee` with A's `id`, `name`, `mobile`, `email` and required `reason=QA contact correction <run>`. Username, project, school, login and status are not normal submitted fields. Include a valid antiforgery token for the requesting session unless the case explicitly tests tokens. Omitting unchanged fields could test an unrelated failure.
- Each invalid variant is a separate execution. Inspect persistent data after each attempt; a rendered error alone does not establish that nothing saved. Use fresh fixtures or approved exact-record restoration between successful mutations. Direct database fixture setup must use `BEGIN`/`ROLLBACK`; never reset or reseed Testdb.
- Record `Pass`, `Fail`, `Blocked` or `Not Applicable` only after execution, along with actual result, redacted evidence and defect ID where relevant. Save artifacts under ignored `artifacts` or `.local`. Default role is Coordinator P and default priority is P1 unless a case states otherwise.

## Requirement and implementation boundaries

The PRD requires a nonblank name allowing spaces/common punctuation, required text mobile validated for the configured country, valid required email syntax, duplicate-contact warnings rather than uniqueness rejection, scoped editing, and separate login credentials. It does not specify alphabet-only names, OTP/email verification, mandatory uniqueness of contact details, or a snapshot of historical trainee names.

Current UI limits are name **200**, mobile **16** characters (optional `+`, **10–15 digits**) and email **254** characters. Mobile and email format checks also exist in the database. The PRD does not define these length limits or a specific country pattern; tests below label UI limits and proposed server parity explicitly.

## TC-MODTRAINEE-001 — Open existing trainee for modification

- Requirements: FR03; Data requirements / Trainee.
- Steps: Open A through the directory and direct edit route; inspect name, mobile, email and unchanged identity/scope fields.
- Expected result: Correct A values are prepopulated in labelled editable fields; name/mobile/email show required indicators; A's ID is retained; no record is changed merely by opening the form.

## TC-MODTRAINEE-002 — Modify name only

- Requirements: FR03; Data requirements / Trainee.
- Test data: `Anne-Marie O'Neil`.
- Steps: Replace A's name only; save; inspect success notice and directory; reopen the edit page and compare persisted A/B records.
- Expected result: New name persists including spaces, hyphen and apostrophe. Mobile/email and A's ID, username, scope, login link and active status remain unchanged; B is unchanged; no new trainee is created.

## TC-MODTRAINEE-003 — Modify mobile number only

- Requirements: FR03; Data requirements / Trainee.
- Test data: `+919123456780`.
- Steps: Replace only A's mobile; save; reopen and inspect stored mobile type/value.
- Expected result: Mobile persists as text with its `+` prefix; name/email and all identity/scope fields remain unchanged. Substitute a valid configured-country example if the agreed format differs.

## TC-MODTRAINEE-004 — Modify email ID only

- Requirements: FR03; Data requirements / Trainee.
- Test data: `qa.<run>.updated+training@example.com`.
- Steps: Replace only A's email; save; reopen.
- Expected result: Valid email with a plus-tag persists exactly; name/mobile and identity/scope fields remain unchanged. Changing profile email does not change login credentials.

## TC-MODTRAINEE-005 — Modify all three fields together as each authorized role

- Requirements: FR03; Users and permissions.
- Role: Coordinator P, then Administrator on an independent fixture.
- Test data: `QA Teacher Updated`, `9198765432`, `qa.<run>.all@example.com`.
- Steps: Change name/mobile/email in one form; save; refresh, sign out/in and reopen; compare A/B and record count.
- Expected result: All three values persist together after a fresh session; success is shown; only intended profile values change; no duplicate trainee or modification to B occurs.

## TC-MODTRAINEE-006 — Cancel and Back discard unsaved edits

- Requirements: FR03; UI conventions.
- Steps: Change all three values and click Cancel; reopen. Repeat using Back to directory.
- Expected result: Directory navigation succeeds; original values persist; neither action sends a save or creates a profile-update audit entry.

## TC-MODTRAINEE-007 — Required and whitespace-only values

- Requirements: FR03; Data requirements / Trainee.
- Test data: Empty string and three spaces, separately for name/mobile/email.
- Steps: Submit each variant through the UI; then bypass browser checks with a valid-token POST retaining the other fields.
- Expected result: Every blank/whitespace-only field is rejected on the server with a visible validation error; all three original stored values remain intact; no successful profile-update audit entry is created.

## TC-MODTRAINEE-008 — Common names, Unicode and name trimming

- Requirements: Data requirements / Trainee; implementation trimming behavior.
- Test data: `Dr. Asha Rao`, `José D'Souza`, `आशा शर्मा`, `  QA Teacher Updated  `.
- Steps: Save each name; reopen and compare stored text.
- Expected result: Nonblank names with spaces/common punctuation persist; Unicode round-trips correctly; outer whitespace is removed by the current save routine without removing internal spaces. Do not impose an undocumented alphabet-only restriction.

## TC-MODTRAINEE-009 — Name length boundary

- Requirements: Implemented Modify Trainee name limit; FR03.
- Test data: Nonblank names of 200 and 201 characters.
- Steps: Save 200 characters; attempt typing/pasting 201 in the UI; send 201 directly with a valid token.
- Expected result: UI allows 200 and prevents longer input. Controller and database reject direct POST of 201 with a name-length error without saving.

## TC-MODTRAINEE-010 — Mobile text preservation and format boundaries

- Requirements: Data requirements / Trainee; current mobile-format rule.
- Test data: `0123456789`, `123456789012345`, `+123456789012345`.
- Steps: Save each value, reopen and compare the exact stored string.
- Expected result: Under the current 10–15-digit optional-plus rule, boundary values are accepted without losing a leading zero or `+`. Configured-country acceptance remains subject to confirmation; these are format test strings, not assertions that each is a real phone number.

## TC-MODTRAINEE-011 — Reject invalid mobile numbers

- Requirements: FR03; Data requirements / Trainee.
- Test data: `123456789` (9 digits), `1234567890123456` (16 digits), `91234abc89`, `91 23456789`, `912-345-6789`, `++919123456789`, `919123456789+`.
- Steps: Submit every variant through UI and valid-token direct POST; retain valid name/email.
- Expected result: All values failing the current pattern are rejected by browser/server as applicable; no name/mobile/email changes persist. Country-specific formatting requirements must be confirmed separately.

## TC-MODTRAINEE-012 — Email syntax acceptance and rejection

- Requirements: FR03; Data requirements / Trainee.
- Test data: Valid `qa.<run>+tag@example.co.in`; invalid `teacher`, `teacher@`, `@example.com`, `teacher@example`, `teacher name@example.com`, `teacher@@example.com`.
- Steps: Save valid email; attempt each invalid email using UI and a direct POST with browser checks bypassed.
- Expected result: Valid syntax persists; malformed emails are rejected server-side; no partial field update occurs. Record browser/server disagreements for investigation; the database's simple pattern is not proof of full email-syntax validation.

## TC-MODTRAINEE-013 — Email length boundary

- Requirements: Implemented Modify Trainee email limit; FR03.
- Test data: Generate syntactically valid 254- and 255-character emails using local part `q` repeated 64 times, `@`, domain labels `a` repeated 63, `b` repeated 63, `c` repeated 57 (58 for the longer case), separated by dots, ending `.com`.
- Steps: Confirm calculated lengths; save 254; attempt 255 in UI; send 255 by valid-token POST.
- Expected result: UI accepts the configured 254-character maximum and prevents longer input. Controller and database reject 255 without saving and show the email maximum in the error.

## TC-MODTRAINEE-014 — Duplicate contacts warn without rejecting the person

- Requirements: Data requirements / Trainee.
- Test data: B's mobile only; B's email only; both B contacts; B's email with case changed; A's original unique contacts as control.
- Steps: Update A with each duplicate variant while retaining A's username/login; inspect save result/warning. Save unchanged unique contacts as control.
- Expected result: Contact reuse saves with a visible repeated-contact warning; no identity merge, username conflict or record replacement occurs; email case does not avoid the warning. Saving only A's own unique contacts does not produce a duplicate warning.

## TC-MODTRAINEE-015 — Duplicate contact outside project scope

- Requirements: Data requirements / Trainee; Users and permissions.
- Role: Coordinator P, then Administrator.
- Steps: Give A a mobile/email shared only by Q's C profile; save as Coordinator P and inspect response; repeat as Administrator on a dedicated fixture.
- Expected result: Save is allowed; response never exposes C's personal details or project membership to Coordinator P. Administrator sees a repeated-contact warning. Current Coordinator warning lookup is scoped to accessible projects; whether to issue a generic cross-project warning without revealing existence is a product decision, not an assumed permission expansion.

## TC-MODTRAINEE-016 — Validation failure retains entered values and prevents partial save

- Requirements: FR03; UI conventions; atomicity guidance.
- Priority: P0.
- Steps: Submit a changed valid name/mobile with invalid email by valid-token POST; inspect returned form, persistent profile and audit. Correct the email on that form and save once.
- Expected result: Initial failure shows an error and retains entered values for correction; no initial name/mobile/email changes or profile-update audit persist. Corrected save commits all three intended values together.

## TC-MODTRAINEE-017 — Cross-project direct access and ID tampering

- Requirements: FR03; Users and permissions; server-side record permissions.
- Priority: P0.
- Steps: As Coordinator P request C's edit URL; obtain a valid token from an allowed form and POST C's ID with modified contacts and a valid reason, first using the ordinary contact-only payload, then adding forged project/school=P/S fields.
- Expected result: C's details are not disclosed; all forbidden saves are rejected regardless of payload scope; A/C and audit remain unchanged. A success-status page alone is not evidence of permission correctness.

## TC-MODTRAINEE-018 — Trainer, Trainee and anonymous edit attempts denied

- Requirements: Users and permissions; AC20; server-side record permissions.
- Priority: P0.
- Role: Trainer; linked Trainee U1; another Trainee U2; anonymous.
- Steps: Request A's edit URL as each role; try contact-change POSTs. For logged-in roles use a valid token from a form they may access so role authorization is tested independently; for Trainees test both own and another profile.
- Expected result: No role listed can access the edit form or modify profile contacts. Anonymous requests require login; forbidden reads do not expose contacts. A and its audit remain unchanged. Own-profile view permission does not imply self-edit permission.

## TC-MODTRAINEE-019 — Missing, invalid and cross-session antiforgery tokens

- Requirements: Request security; existing global antiforgery filter.
- Priority: P0.
- Steps: As authorized Coordinator, POST otherwise valid modifications with token absent, corrupted, then from a different authenticated session; compare profile/audit. Submit with a matching session token as control on a dedicated fixture.
- Expected result: Invalid-token requests are rejected without contact changes; matching-session control may save. Do not interpret token rejection as proof of project/role authorization.

## TC-MODTRAINEE-020 — Unknown ID cannot create or modify a different profile

- Requirements: FR03; server-side record permissions.
- Priority: P0.
- Steps: Discover a positive ID confirmed absent from trainee records; GET its edit route and POST a complete valid edit payload with that absent ID. Also try ID 0 and a negative ID; ModifyTrainee never creates profiles.
- Expected result: GET returns not found; POST rejects the nonexistent target and creates no trainee or success notice. Existing profiles and audit remain unchanged.

## TC-MODTRAINEE-021 — Contact edits during active assignments preserve training history

- Requirements: FR03; FR04; FR21; AGENTS.md history/isolation invariants.
- Priority: P0.
- Steps: For dedicated Scheduled, In progress and Completed fixtures, modify A's name/mobile/email only while keeping project/school/active/login unchanged; compare assignment, attendance, feedback, plan and batch snapshots; inspect roster/report/export.
- Expected result: Authorized contact edits are allowed without treating them as membership transfer. Assignment IDs and assignment-time scope, attendance status/revisions and feedback IDs/answers remain unchanged, with no new attendance/feedback records. Current names appear where reports resolve the live profile; the PRD requires historical association preservation, not frozen historical names. Unrelated trainees/batches remain unchanged.

## TC-MODTRAINEE-022 — Separate login identity and audit attribution

- Requirements: Data requirements / Trainee; Data requirements / all records; AGENTS.md audit guidance.
- Priority: P0.
- Steps: Update all three contacts, then sign in using U1's existing credentials; inspect own assigned records. As Administrator inspect the trainee UPDATE audit and timestamps; compare app_user without printing passwords/hashes.
- Expected result: Same trainee ID/login link and credentials remain valid; another user's records remain inaccessible. Audit records old/new profile values and editing actor/time; creation time is unchanged and update time advances. No linked app_user mutation is caused by contact edits. The mandatory correction reason is preserved in the profile-update audit and attributed to the actor.

## TC-MODTRAINEE-023 — Safe rendering, parameterization and updated-name search

- Requirements: FR03; server-side security; parameterization guidance.
- Priority: P0.
- Test data: Name `QA O'Neil`; then literal `QA <script>alert('qa')</script>` on a dedicated test profile.
- Steps: Save each value; search directory by updated name/unchanged username; inspect directory/edit/roster rendering and another authorized fresh session.
- Expected result: Apostrophe saves without SQL errors or unrelated data changes; any accepted markup is encoded as text and executes no script. New name/username searches locate the same record with preserved filters/scope. Do not require mobile/email search: FR03 specifies username/name search.

## TC-MODTRAINEE-024 — Keyboard and narrow-screen modification

- Requirements: UI conventions; Quality and operational requirements.
- Steps: At desktop and 390-pixel mobile width, use keyboard navigation to reach labelled name/mobile/email, edit, trigger invalid email feedback, correct it and save; inspect error and success visibility and Cancel navigation.
- Expected result: Labels, required indicators, input values, errors, Save and Cancel remain readable/reachable; no essential controls are clipped; keyboard focus and mobile input modes support completing the edit. Stored values match the corrected inputs.

## TC-MODTRAINEE-025 - Correction reason validation and trimming

- Requirements: AGENTS.md correction/audit guidance; implemented reason limits.
- Priority: P0.
- Test data: Missing, empty, spaces-only, 1 character, 1000 characters, 1001 characters; valid reason with outer spaces.
- Steps: Modify valid contacts and submit each reason variant through UI and valid-token POST; inspect profile and audit after every attempt.
- Expected result: Missing/blank/whitespace-only/over-1000 reasons reject the save with an explicit error and no profile/audit changes; 1 and 1000 trimmed characters save; outer whitespace is removed and audit retains the trimmed reason and actor.

## TC-MODTRAINEE-026 - Forged identity and scope fields cannot change associations

- Requirements: Contact-only modification contract; FR04; server-side permissions.
- Priority: P0.
- Steps: POST valid contacts/reason for allowed A with extra changed username, project_id, school_id, user_id, active=false and location fields. Inspect stored fields and training history; inspect UI for editable association controls. Repeat with forbidden C ID and forged P/S scope.
- Expected result: Only allowed contacts/timestamp change; injected fields are ignored or rejected. Identity, project/school, active status, login link, assignments and history remain intact; C access is denied regardless of forged scope. UI exposes only contacts and reason for editing.

## Source-review observations and untested decisions

These are read-only inspection findings, not executed defect reproductions:

- [MastersController.cs](../Schoolnet.WEBUI/Controllers/MastersController.cs): new GET/POST ModifyTrainee loads the scoped profile first, accepts only name/mobile/email/reason, trims values and provides explicit validation errors. General Edit remains a separate flow.
- [ModifyTrainee.cshtml](../Schoolnet.WEBUI/Views/Masters/ModifyTrainee.cshtml): only contacts and mandatory correction reason are editable; username/project/school are display context. Save changes and Cancel are the applicable controls.
- [003_modify_trainee.sql](../database/003_modify_trainee.sql): save_trainee_contact locks the row, checks project permissions and all four inputs, sets actor/reason and changes only contacts/timestamp. Unknown ID raises an error; controller rejects unknown/scoped-out IDs before saving. Name/email server limits are present for this route. Confirm migration 003 is installed before testing saves.
- Duplicate-contact warning lookup remains in general Edit but is absent from the new Modify action as inspected. Cases 014/015 retain PRD duplicate-warning expectations for accessible duplicates and may identify a gap when executed. Cross-project existence must not be disclosed to Coordinator P.
- [Program.cs](../Schoolnet.WEBUI/Program.cs) applies global antiforgery validation; [TrainingRepository.cs](../Schoolnet.Repository/TrainingRepository.cs) uses parameterized save calls.
- [ReportsController.cs](../Schoolnet.WEBUI/Controllers/ReportsController.cs) resolves current names by ID; historical name snapshots are not required by FR04.
- Existing [HTTP](../tests/verify-http.mjs), [browser](../tests/verify-browser.mjs) and [database](../tests/verify-database.sql) scripts have related coverage; concurrent script changes are not evidence of execution. No application build, manual save, browser test or database mutation was run by this document-writing task.
- Stale-profile conflict policy and country-specific mobile rules remain unconfirmed. Row locks serialize writes but do not detect stale forms; attendance FR14 is not automatically a profile policy. Contact-only corrections for inactive project/school contexts can be assessed separately; the new routine has no active-project/school gate.

## Execution record template

Use one row per case/variant when this supplemental suite is executed. It is not yet included in `test-cases.csv` or the existing validator's 153-case count.

| Case / variant | Status | Actual result | Evidence | Defect ID |
| --- | --- | --- | --- | --- |
| TC-MODTRAINEE-001 through TC-MODTRAINEE-026 (each initially) | Not Run | | | |
