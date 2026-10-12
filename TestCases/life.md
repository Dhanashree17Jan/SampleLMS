# Lifecycle controls and dashboard

Source: [PRD v1.2](../PRD_Doc/Training_Management_PRD.md), including the Create training plan update. All cases are **Not Run**. A listed automation reference indicates source coverage only, never a recorded execution result.

Prerequisites: Dedicated Draft/Scheduled/In progress/Completed/Cancelled batches; configured dateT0; missing/fully marked attendance and incomplete feedback variants.

Use the [fixture conventions and execution instructions](README.md) before changing dedicated records. The CSV is the authoritative execution register; these Markdown files present the initial case definitions.

## TC-LIFE-001 — Scheduled automatically becomes In progress

- Requirements: FR19
- Role: Coordinator
- Priority: P1
- Test data: Batch startsT0; future startsT0+1
- Steps: View dashboard/detail before/on start in controlled environment; inspect effective status.
- Expected result: On start date batch is In progress; future remains Scheduled; no premature activation.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-LIFE-002 — Completion blocked by Not marked attendance

- Requirements: FR19; AC19
- Role: Coordinator
- Priority: P0
- Test data: Past-ended A with one unmarked entry
- Steps: Attempt complete; inspect error and missing attendance navigation.
- Expected result: Completion denied; missing entries identified; Not marked never treated Absent.
- Existing automation reference: DB: AC19 completion block
- Initial status: Not Run

## TC-LIFE-003 — Completion after end with attendance despite missing feedback

- Requirements: FR19
- Role: Coordinator
- Priority: P0
- Test data: Past-ended A all marked; missing initial/final feedback
- Steps: Complete; inspect status and missing feedback indicators.
- Expected result: Completes after end; absent feedback displayed but does not block under proposed default.
- Existing automation reference: DB: completion after attendance (feedback partially missing)
- Initial status: Not Run

## TC-LIFE-004 — Premature completion prohibited

- Requirements: FR19
- Role: Coordinator
- Priority: P1
- Test data: Before/on end-date A all marked
- Steps: Attempt complete before end and on end; repeat after end.
- Expected result: Cannot complete before end date has elapsed; after-end valid completion permitted.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-LIFE-005 — Cancellation reason and preserved history

- Requirements: FR20
- Role: Coordinator
- Priority: P0
- Test data: A with roster/attendance/feedback
- Steps: Cancel with blank/whitespace reason; retry valid reason; inspect history/audit.
- Expected result: Reason required; valid cancellation preserves all records and audit; entry disabled.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-LIFE-006 — Cancelled exclusion from overlap and active totals

- Requirements: FR20; FR11
- Role: Coordinator
- Priority: P1
- Test data: Cancelled A overlapping proposed B; same trainee
- Steps: Schedule eligible B; compare dashboard active/Cancelled totals.
- Expected result: A no longer conflicts; excluded from active totals; historical Cancelled count/report retained.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-LIFE-007 — Invalid repeated lifecycle transitions

- Requirements: FR10; FR19; FR20
- Role: Coordinator
- Priority: P1
- Test data: All five status fixtures
- Steps: Tamper transition status values; repeat complete/cancel; try Completed→Draft.
- Expected result: Illegal/repeated changes rejected without corrupting dates/history; only documented transitions permitted.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-LIFE-008 — Dashboard metrics filters upcoming and missing records

- Requirements: Core screens and workflow; FR19; FR21
- Role: All roles
- Priority: P1
- Test data: P/Q batches; missing attendance and feedback; upcoming day
- Steps: View metrics/activity cards; filter project/state/school; compare source records and own role scope.
- Expected result: Counts/statuses/upcoming activities correct; missing records visible; filters honor scope; no hidden Q data.
- Existing automation reference: Browser: metrics count and project filter only
- Initial status: Not Run

## TC-LIFE-009 — Lifecycle and roster audit trace

- Requirements: Quality and operational requirements
- Role: Administrator
- Priority: P1
- Test data: Dedicated created/published/revised plan; batch/roster/attendance/feedback lifecycle
- Steps: Perform authorized fixture changes; open /Reports/Audit; compare actor/entity/time/reason/history.
- Expected result: Each required action recorded with actor/time and correction reason; before/after changes reconstructable.
- Existing automation reference: DB: feedback reason/actor only; HTTP: audit screen load
- Initial status: Not Run

## TC-LIFE-010 — Trainer Trainee lifecycle and audit access denied

- Requirements: Users and permissions
- Role: Trainer/Trainee
- Priority: P0
- Test data: A status; /Reports/Audit
- Steps: Direct POST schedule/complete/cancel with valid token; GET audit URL.
- Expected result: No status mutation; audit administration denied; existing records preserved.
- Existing automation reference: Manual
- Initial status: Not Run
