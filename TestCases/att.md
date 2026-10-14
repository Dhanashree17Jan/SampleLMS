# Attendance and concurrency

Source: [PRD v1.2](../PRD_Doc/Training_Management_PRD.md), including the Create training plan update. Future-date cases also reflect the current future-attendance guard and availability notice; FR15/AC23 prohibit future marking for all staff roles. All cases are **Not Run**. A listed automation reference indicates source coverage only, never a recorded execution result.

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

## TC-ATT-007 — Draft attendance rejected before scheduling

- Requirements: FR15
- Role: Administrator/Coordinator/Trainer
- Priority: P0
- Test data: Draft A with valid plan day and unmarked roster; derived date T0
- Steps: Open Draft A attendance as each authorized staff role; submit valid IDs, revisions, Present/Absent and a valid session token to /Attendance/Save.
- Expected result: Draft roster is read-only even when the derived date is today; bulk/save controls absent and status/remarks disabled; direct save rejected before scheduling; no attendance, revision or marking-history changes.
- Existing automation reference: Manual
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

## TC-ATT-013 — Future attendance screen is read-only

- Requirements: FR13; FR15; AC23
- Role: Administrator/Coordinator/Trainer
- Priority: P0
- Test data: Scheduled A starts T0+1; In progress B starts T0-1 with day3=T0+1; P4 roster
- Steps: Open A days1 through4 and B day3 as each permitted staff role; inspect selected day/date, notice, status and remarks controls, bulk action and save controls; attempt keyboard editing.
- Expected result: Every future day shows the correct derived date and a clear future-date notice with the availability date in India time; status and remarks are disabled; Mark all present, correction input and Save attendance controls absent; roster remains Not marked.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-ATT-014 — Future attendance POST denied for every staff role

- Requirements: FR15; AC23
- Role: Administrator/Coordinator/Trainer
- Priority: P0
- Test data: Accessible future A day; valid attendance IDs/revisions and session tokens; Present/Absent variants; reasons blank and nonblank
- Steps: For each staff role POST /Attendance/Save directly with valid token and each status/reason variant; follow response redirect; reload roster and compare statuses, remarks, revisions, marked_by, marked_at and attendance audit history.
- Expected result: All future requests rejected, including Administrator with a correction reason; redirect preserves the requested batch/day and displays future-date rejection without success; no attendance, revision, marker metadata or attendance-change audit writes.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-ATT-015 — Today is editable while tomorrow stays blocked

- Requirements: FR13; FR15; AC23
- Role: Administrator/Coordinator/Trainer
- Priority: P0
- Test data: In progress P4 A starts T0-1; day2=T0, day3=T0+1; separate rows initialized Not marked
- Steps: Open today and tomorrow in separate sessions for each staff role; save today using latest revisions with Present/Absent and remarks; attempt tomorrow with equivalent valid payload; reload both dates.
- Expected result: Today permits authorized bulk/save and persists exact statuses/remarks once; tomorrow remains read-only and rejects the save; today writes never update tomorrow rows; no off-by-one allowance at T0+1.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-ATT-016 — India midnight and client clock permit no bypass

- Requirements: FR15; AC23; Assumptions and decisions
- Role: Trainer/Operator
- Priority: P0
- Test data: Isolated controllable-clock fixture: target date D; UTC time D-1 at18:29:59 then18:30:00; browser time zones UTC and Asia/Kolkata
- Steps: With server/application test clock controlled, request target day immediately before and at India midnight; alter browser time zone/date and inject client date fields while server instant unchanged; repeat future POST with valid token.
- Expected result: Before India midnight target day is future and blocked; at India midnight it becomes available if lifecycle/permissions permit; browser clock, browser time zone and forged client dates do not change derived date or server eligibility; no pre-midnight mutation.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-ATT-017 — Day and record tampering cannot evade future guard

- Requirements: FR15; AC23; AC22; Users and permissions
- Role: Trainer/Coordinator
- Priority: P0
- Test data: Accessible In progress A today day2/future day3; another batch B inaccessible; foreign plan detail; day0, dayN+1 and invalid batch IDs
- Steps: From a current-day form change day to future day3 with matching future rows; repeat with current-day or B row IDs, forged derived date and batch ID; submit day0/N+1 and nonexistent IDs separately with valid token.
- Expected result: Matching future payload denied by date; foreign/current-day row substitution cannot write future records; invalid days/IDs denied; inaccessible batch reveals no roster; all targeted and unrelated rows/revisions/history unchanged.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-ATT-018 — Empty or malformed future payload remains denied

- Requirements: FR15; AC23
- Role: Administrator/Coordinator/Trainer
- Priority: P0
- Test data: Accessible future A day; variants: no rows, mismatched row arrays, invalid IDs/revisions, duplicate rows, invalid status; valid session token
- Steps: POST each payload variant directly to future A day as each staff role with valid token; follow redirect and inspect error, roster and revisions; separately POST valid future payload without or with invalid token.
- Expected result: Authenticated valid-token requests display future-date denial before row parsing or persistence, including empty/malformed payloads; no success, server error or partial writes; missing/invalid antiforgery requests rejected independently with no mutation.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-ATT-019 — Future denial preserves sibling batch and feedback

- Requirements: FR14; FR15; AC10; AC23
- Role: Administrator/Coordinator/Trainer
- Priority: P0
- Test data: A/B share Published P4 with independent assignments, attendance and feedback; A future day; current B day; capture both snapshots
- Steps: Submit future A bulk Present/Absent with remarks and reason; compare every A/B attendance field, revisions and audit rows plus feedback submissions/completion and lifecycle; save B current day and reload A future day.
- Expected result: Rejected A save changes no A or B attendance/history, no feedback/completion or lifecycle; valid current B save affects only B assignment/day; shared plan grants no future allowance and A future rows remain Not marked.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-ATT-020 — Future denial cannot be overridden by lifecycle or correction

- Requirements: FR15; FR20; AC23
- Role: Administrator/Coordinator/Trainer / DB verifier
- Priority: P0
- Test data: Scheduled and In progress future days; Draft/Cancelled fixtures; rollback-only Completed fixture whose selected derived day is future; reasons blank/nonblank
- Steps: Attempt UI and valid-token direct saves in every applicable lifecycle; include Administrator with reason on Completed future fixture; in BEGIN/ROLLBACK invoke save_attendance for accessible future day with valid JSON and reason.
- Expected result: Future day remains blocked regardless of lifecycle, prior status or correction reason; Draft/Cancelled never permit attendance; Completed Administrator authority cannot override future gate; direct routine also rejects and preserves rows/revisions/history; rollback all SQL fixtures.
- Existing automation reference: DB: AC23 future attendance denial (partial; lifecycle/reason matrix manual)
- Initial status: Not Run
