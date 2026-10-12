# Attendance and concurrency

Source: [PRD v1.2](../PRD_Doc/Training_Management_PRD.md), including the Create training plan update. All cases are **Not Run**. A listed automation reference indicates source coverage only, never a recorded execution result.

Prerequisites: Scheduled/In progress A/B share plan; derived past/current/future days relative to configured application date T0; Trainer assigned A; two sessions; recorded revisions.

Use the [fixture conventions and execution instructions](README.md) before changing dedicated records. The CSV is the authoritative execution register; these Markdown files present the initial case definitions.

## TC-ATT-001 — Day roster and derived date display

- Requirements: FR13
- Role: Trainer/Coordinator
- Priority: P0
- Test data: A startT0-2; day3
- Steps: Open /Attendance?id=A&day=3; inspect roster/date; navigate boundary days.
- Expected result: Day3 dateT0; relevant A roster only; boundary dates start/end; current statuses shown.
- Existing automation reference: HTTP: current-day page loads (partial assertions)
- Initial status: Not Run

## TC-ATT-002 — Bulk Present with exception remarks

- Requirements: FR13
- Role: Trainer
- Priority: P1
- Test data: Current A day; three trainees
- Steps: Bulk mark Present; change one to Absent; add remarks; save/reload.
- Expected result: Exact statuses/remarks persisted; exception not overwritten; actor/time stored.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-ATT-003 — Repeated save updates existing records

- Requirements: FR14; AC16
- Role: Trainer/Coordinator
- Priority: P0
- Test data: Existing A day revision
- Steps: Save; reload latest revision; change status; save again; count pairs.
- Expected result: One record per assignment/detail; revision advances; status updated without duplicates.
- Existing automation reference: DB: AC16 repeated attendance count
- Initial status: Not Run

## TC-ATT-004 — Stale concurrent attendance revision rejected

- Requirements: FR14
- Role: Trainer/Coordinator
- Priority: P0
- Test data: Two sessions load same day/revision
- Steps: Session1 saves; session2 submits stale payload; compare stored records.
- Expected result: Session2 receives clear conflict; first save retained; no stale overwrite.
- Existing automation reference: DB: stale attendance payload
- Initial status: Not Run

## TC-ATT-005 — Bulk stale or invalid row rollback

- Requirements: FR14; Quality and operational requirements
- Role: Trainer
- Priority: P0
- Test data: Multi-row payload; one stale/invalid row
- Steps: Change valid row followed by stale/foreign row in same save; inspect all rows.
- Expected result: Whole save rolled back; no partial status/revision/remark changes.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-ATT-006 — Shared-batch attendance isolation

- Requirements: FR14; AC10
- Role: Trainer/Coordinator
- Priority: P0
- Test data: A/B share P10
- Steps: Mark A day1; compare B same day and trainee.
- Expected result: B remains Not marked with unchanged revisions/remarks; only A audit entries.
- Existing automation reference: DB: AC10 shared attendance untouched
- Initial status: Not Run

## TC-ATT-007 — Future and Draft attendance rejected

- Requirements: FR15; AC23
- Role: Trainer/Coordinator
- Priority: P0
- Test data: DraftA; future day in ScheduledB
- Steps: Open/tamper save with valid IDs and tokens for each.
- Expected result: Entry before scheduling and future date rejected server-side; statuses unchanged.
- Existing automation reference: DB: AC23 future attendance only
- Initial status: Not Run

## TC-ATT-008 — Cancelled attendance read-only

- Requirements: FR15; FR20
- Role: Trainer/Coordinator
- Priority: P0
- Test data: Cancelled A with history
- Steps: Open attendance; POST new status/remarks.
- Expected result: History viewable as permitted; no entry or modification allowed.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-ATT-009 — Past correction requires Coordinator reason

- Requirements: FR15
- Role: Coordinator/Trainer
- Priority: P0
- Test data: Past marked entry
- Steps: Submit correction blank/whitespace reason; submit with reason as Coordinator; repeat Trainer.
- Expected result: Missing reason rejected; authorized correction records actor/reason/before-after; unauthorized Trainer correction denied.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-ATT-010 — Completed attendance administrator authorization

- Requirements: FR15
- Role: Administrator/Coordinator/Trainer
- Priority: P0
- Test data: Completed A marked entry
- Steps: Attempt correction as Coordinator/Trainer; perform administrator-authorized correction with reason.
- Expected result: Only administrator-authorized correction permitted; reason/audit preserved; unauthorized requests fail.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-ATT-011 — Foreign assignment detail and day tampering

- Requirements: AC22; Data relationships and integrity
- Role: Trainer / DB verifier
- Priority: P0
- Test data: A row; B assignment; detail from other plan; day0/N+1
- Steps: POST tampered IDs/day/status; rollback SQL wrong-plan detail insert.
- Expected result: No wrong-plan/cross-batch write; day/status validation enforced; unchanged valid rows.
- Existing automation reference: DB: AC22 foreign-plan detail
- Initial status: Not Run

## TC-ATT-012 — Unassigned Trainer and Trainee mutation denied

- Requirements: Users and permissions
- Role: Trainer/Trainee
- Priority: P0
- Test data: Trainer unassignedB; own traineeA
- Steps: Read/write B attendance as Trainer; write own/other attendance as Trainee.
- Expected result: Trainer cannot access unassigned records; Trainee read-only own data; all denied mutations unchanged.
- Existing automation reference: Manual
- Initial status: Not Run
