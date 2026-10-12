# Batch setup dates and plan selection

Source: [PRD v1.2](../PRD_Doc/Training_Management_PRD.md), including the Create training plan update. All cases are **Not Run**. A listed automation reference indicates source coverage only, never a recorded execution result.

Prerequisites: Scoped Coordinator; P/S active linked; Published P2/P10; Draft D with eligible roster and unmarked attendance; shared batch B.

Use the [fixture conventions and execution instructions](README.md) before changing dedicated records. The CSV is the authoritative execution register; these Markdown files present the initial case definitions.

## TC-BATCH-001 — Plan selector duration and end-date calculation

- Requirements: FR08; AC05
- Role: Coordinator
- Priority: P0
- Test data: P10; start2026-11-02
- Steps: Open batch create; select P10; set start; save/reopen.
- Expected result: Selector shows name/version/duration; duration10; end2026-11-11; existing plan referenced.
- Existing automation reference: Browser: five-day duration/date calculation (different data)
- Initial status: Not Run

## TC-BATCH-002 — Shared plan different dates without duplication

- Requirements: FR08; AC06; AC07
- Role: Coordinator
- Priority: P0
- Test data: A starts2026-11-02; B starts2026-12-01; same P10
- Steps: Save both; compare plan/detail counts and batch derived dates.
- Expected result: Single shared header10 details; A ends11Nov; B ends10Dec; independent day dates.
- Existing automation reference: DB: AC05–AC07 shared references (partial dates)
- Initial status: Not Run

## TC-BATCH-003 — Reject tampered duration and dates

- Requirements: FR08; AC08
- Role: Coordinator
- Priority: P0
- Test data: P10 plus duration9; end=start+10; end<start
- Steps: POST each mismatch with valid token; inspect row count and errors.
- Expected result: Clear mismatch error; original/new batch unchanged; no partial roster or attendance.
- Existing automation reference: DB: AC08 duration/end mismatch
- Initial status: Not Run

## TC-BATCH-004 — Batch required unique and active fields

- Requirements: FR08; Data requirements / Batch
- Role: Coordinator
- Priority: P1
- Test data: Missing code/name/project/school/plan/start; duplicate code; inactive references
- Steps: Submit each invalid batch including Draft/Archived plan ID.
- Expected result: Required unique active linked references enforced server-side; no invalid batch.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-BATCH-005 — Draft start-date change is local

- Requirements: FR09; AC13
- Role: Coordinator
- Priority: P0
- Test data: D=P10, roster2; B shares P10
- Steps: Move D start by7 days; save; inspect dates, attendance/detail IDs and B.
- Expected result: D end/derived dates shift7; same roster/plan/details; B dates/history unchanged.
- Existing automation reference: DB: AC13 Draft and other-batch start only
- Initial status: Not Run

## TC-BATCH-006 — Draft plan switch reconciles unmarked roster atomically

- Requirements: FR09; AC14
- Role: Coordinator
- Priority: P0
- Test data: D=P10 roster2; switchP2
- Steps: Save P2 selection; inspect assignment IDs and attendance pairs.
- Expected result: Roster/assignment history preserved; exactly4 Not marked rows for P2; old unmarked rows removed; no mixed plan rows.
- Existing automation reference: DB: AC14 roster/attendance counts
- Initial status: Not Run

## TC-BATCH-007 — Plan switch blocked by attendance or feedback

- Requirements: FR09; AC15
- Role: Coordinator / DB verifier
- Priority: P0
- Test data: Separate Draft-state fixtures containing marked attendance or feedback history
- Steps: Attempt switch for each via supported workflow or rollback fixture.
- Expected result: Replacement denied whenever records exist; plan/reference/roster/history unchanged.
- Existing automation reference: DB: AC15 Scheduled lock only (partial scenario)
- Initial status: Not Run

## TC-BATCH-008 — Date change rechecks roster overlap

- Requirements: FR09; FR11
- Role: Coordinator
- Priority: P0
- Test data: Draft D shares trainee with future Scheduled E
- Steps: Move D dates into E then away; save each.
- Expected result: Overlap rejected including equal endpoints; valid non-overlap succeeds; failed save preserves all dates.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-BATCH-009 — Consecutive dates across calendar boundaries

- Requirements: Assumptions and decisions; AC05
- Role: Coordinator
- Priority: P1
- Test data: Start month/year/leap boundary; includes weekends/holidays
- Steps: Create P10 batches on2028-02-25,2026-12-28; review each day.
- Expected result: Calendar arithmetic includes all consecutive dates; leap/month/year transitions correct.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-BATCH-010 — Trainer assignment validated

- Requirements: Data requirements / Batch; FR10
- Role: Coordinator
- Priority: P1
- Test data: Active Trainer; inactive user; Trainee role; other project where scoped
- Steps: Select valid trainer; tamper role/inactive trainer_id; schedule.
- Expected result: Valid authorized trainer accepted; invalid role/status/scope rejected before scheduling.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-BATCH-011 — Scheduling gates complete prerequisites

- Requirements: FR10
- Role: Coordinator
- Priority: P0
- Test data: Draft variants missing trainer/roster/Published complete plan/valid project school
- Steps: Attempt schedule each variant; then fully valid D.
- Expected result: Each missing gate identified; valid D schedules once; no partial status/roster mutation.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-BATCH-012 — Scheduled setup and roster locked

- Requirements: FR10
- Role: Coordinator
- Priority: P0
- Test data: Scheduled D before start
- Steps: Try changing project/school/start/end/duration/plan/trainer and adding/removing roster via POST.
- Expected result: Locked fields and roster cannot change; existing training setup preserved.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-BATCH-013 — Return Scheduled to Draft before start without history

- Requirements: FR10
- Role: Coordinator
- Priority: P1
- Test data: Future Scheduled D empty records; past/history variants
- Steps: Request return to Draft for future clean D; repeat variants.
- Expected result: Only future clean Scheduled can return; forbidden cases remain locked; audit records transition.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-BATCH-014 — Outside-scope batch edit and creation denied

- Requirements: Users and permissions
- Role: Coordinator/Trainer/Trainee
- Priority: P0
- Test data: Q batch ID; P-only Coordinator
- Steps: GET edit Q; POST modified Q; create batch with Q project; repeat Trainer/Trainee.
- Expected result: No unauthorized batch or roster changes; denied response without private fields.
- Existing automation reference: HTTP: Coordinator other-project detail only
- Initial status: Not Run
