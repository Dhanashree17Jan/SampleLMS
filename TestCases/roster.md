# Assignment eligibility and isolation

Source: [PRD v1.2](../PRD_Doc/Training_Management_PRD.md), including the Create training plan update. All cases are **Not Run**. A listed automation reference indicates source coverage only, never a recorded execution result.

Prerequisites: Draft A/B using shared P10; active eligible P/S trainees including30 dedicated fixtures; wrong-project/school, inactive and overlapping trainees.

Use the [fixture conventions and execution instructions](README.md) before changing dedicated records. The CSV is the authoritative execution register; these Markdown files present the initial case definitions.

## TC-ROSTER-001 — Single and multi-select assignment initializes exact rows

- Requirements: FR11; FR12; AC09
- Role: Coordinator
- Priority: P0
- Test data: 30 eligible trainees; ten-day A
- Steps: Assign one then29 by multi-select; count assignments/attendance and statuses.
- Expected result: 30 assignments; exactly300 Not marked attendance entries against A plan details; no silent Absent values.
- Existing automation reference: DB: AC09 counts (initial statuses partially asserted later)
- Initial status: Not Run

## TC-ROSTER-002 — Duplicate assignment rejected

- Requirements: FR11; AC16
- Role: Coordinator
- Priority: P0
- Test data: Trainee already assigned A
- Steps: Assign same trainee individually and in repeated-ID array.
- Expected result: Duplicate rejected; unique pair remains; no duplicate attendance.
- Existing automation reference: DB: AC16 individual duplicate
- Initial status: Not Run

## TC-ROSTER-003 — Inactive project and school mismatch

- Requirements: FR11; AC17
- Role: Coordinator
- Priority: P0
- Test data: Inactive trainee; Q/S trainee; P/T trainee
- Steps: Attempt each assignment to P/S A via UI and direct POST.
- Expected result: All ineligible assignments rejected; mismatch explained; roster/count unchanged.
- Existing automation reference: DB: AC17 project mismatch only
- Initial status: Not Run

## TC-ROSTER-004 — Scheduled and In progress overlap inclusive endpoints

- Requirements: FR11; AC17
- Role: Coordinator
- Priority: P0
- Test data: Same trainee in E; A starts on E end; then day after
- Steps: Assign/schedule overlapping A; then non-overlapping A.
- Expected result: Inclusive date overlap rejected; adjacent non-overlap allowed; cancelled batches excluded.
- Existing automation reference: DB: AC17 Scheduled overlap only
- Initial status: Not Run

## TC-ROSTER-005 — Scheduling rechecks concurrent Draft overlap

- Requirements: FR11
- Role: Coordinator
- Priority: P0
- Test data: A/B Draft share trainee and overlap
- Steps: Assign trainee to both; schedule A; attempt schedule B; run concurrent schedule variant.
- Expected result: At most one conflicting batch scheduled; conflicts clearly identified; losing transaction unchanged.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-ROSTER-006 — Remove Draft assignment with no history

- Requirements: FR12
- Role: Coordinator
- Priority: P1
- Test data: Draft A with one trainee10 unmarked rows
- Steps: Remove assignment; compare rows in A and B.
- Expected result: Only selected A assignment and its10 unmarked rows removed; B unaffected; audit records roster change.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-ROSTER-007 — Preserve assignment with training history

- Requirements: FR12
- Role: Coordinator
- Priority: P0
- Test data: Assignment with attendance/feedback; Scheduled/Completed batch
- Steps: Attempt removal via direct POST for each history/status variant.
- Expected result: Removal denied; assignment and attendance/feedback history remain.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-ROSTER-008 — Mixed-validity bulk assignment atomicity

- Requirements: FR11; FR12; Quality and operational requirements
- Role: Coordinator
- Priority: P0
- Test data: Array containing eligible and ineligible/duplicate IDs
- Steps: Submit mixed array; inspect every assignment/attendance count.
- Expected result: Entire operation fails; no partial eligible additions or initialized rows.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-ROSTER-009 — Shared plan alone creates no roster conflict

- Requirements: FR11; AC06; AC10
- Role: Coordinator
- Priority: P1
- Test data: Same trainee; A/B sameP10 non-overlapping dates
- Steps: Assign and schedule both; inspect assignments/attendance IDs.
- Expected result: Both succeed; separate batch-assignment IDs; records remain isolated.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-ROSTER-010 — Roster POST uses batch scope and assignment ownership

- Requirements: Users and permissions
- Role: Coordinator
- Priority: P0
- Test data: P batchA; Q batchB; B assignment ID
- Steps: POST Assign to B and Remove A with B assignment ID using P session.
- Expected result: No cross-project mutation or cross-batch removal; original rosters unchanged.
- Existing automation reference: Manual
- Initial status: Not Run
