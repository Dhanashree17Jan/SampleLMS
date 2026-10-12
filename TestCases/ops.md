# Database integrity atomicity and operations

Source: [PRD v1.2](../PRD_Doc/Training_Management_PRD.md), including the Create training plan update. All cases are **Not Run**. A listed automation reference indicates source coverage only, never a recorded execution result.

Prerequisites: DB fixtures inside BEGIN/ROLLBACK; disposable production-like restore/load environment for operational checks; agreed volume and retention decisions.

Use the [fixture conventions and execution instructions](README.md) before changing dedicated records. The CSV is the authoritative execution register; these Markdown files present the initial case definitions.

## TC-OPS-001 — Required foreign keys unique keys and audit metadata

- Requirements: Data requirements; Data relationships and integrity
- Role: DB verifier
- Priority: P0
- Test data: Plan/day, assignment, attendance and feedback fixture tables
- Steps: Attempt orphan references/duplicate keys/missing required data in rollback fixtures; inspect timestamps/actor.
- Expected result: Invalid relations rejected; unique pairs enforced; IDs/time/actor present on required records.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-OPS-002 — Failed related save preserves existing data

- Requirements: Quality and operational requirements; FR05; FR09; FR12
- Role: DB verifier
- Priority: P0
- Test data: Plan create/resize, switch/assign, attendance/feedback invalid payload
- Steps: Force one validation failure midway through each rollback workflow; compare before/after row sets.
- Expected result: Each operation atomic; no partial header/detail/roster/attendance/answer/audit changes.
- Existing automation reference: DB: bad plan row-count preserves Draft only (partial)
- Initial status: Not Run

## TC-OPS-003 — Referenced plan detail template cannot be deleted

- Requirements: Data relationships and integrity; FR07
- Role: DB verifier
- Priority: P0
- Test data: Published/reference plan and recorded history
- Steps: Attempt direct DELETE/UPDATE of referenced header/detail/template in rollback transaction.
- Expected result: Foreign keys/immutability block deletion/content mutation; history unchanged.
- Existing automation reference: DB: Published detail UPDATE only
- Initial status: Not Run

## TC-OPS-004 — Configured time zone governs date restrictions

- Requirements: Assumptions and decisions; AC23
- Role: Trainer/Trainee/Operator
- Priority: P0
- Test data: Server UTC vs application zone; fixture boundary around midnight
- Steps: In isolated controllable-clock environment compare availability immediately before/after local midnight.
- Expected result: Start/status/attendance/feedback gates follow configured application date, not browser or server UTC date.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-OPS-005 — Performance proposed target under representative load

- Requirements: Quality and operational requirements
- Role: Performance tester
- Priority: P2
- Test data: 100 trainees×30days;50 concurrent users; agreed environment
- Steps: Measure warm/cold page and representative saves over agreed duration; report p95, failures and test config.
- Expected result: Proposed p95 within3seconds for pages/saves; no lost updates or cross-user records; product/environment agreement recorded.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-OPS-006 — Daily backup and verified restoration

- Requirements: Quality and operational requirements
- Role: Operator
- Priority: P0
- Test data: Disposable restore instance; approved sample backup
- Steps: Check daily schedule; restore backup into isolated instance; reconcile plans/rosters/attendance/feedback/audit and protected credentials.
- Expected result: Daily backup demonstrable; restored data internally consistent and recoverable before launch; production untouched.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-OPS-007 — Retention deletion policy decision and rollout gate

- Requirements: Quality and operational requirements
- Role: Product owner/Operator
- Priority: P2
- Test data: Agreed policy or unresolved decision
- Steps: Review approved retention/deletion/hosting policy; map history constraints; verify only on disposable fixtures.
- Expected result: Policy confirmed before production rollout; referenced training history preserved; unresolved decisions logged.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-OPS-008 — Normal startup preserves data and secrets excluded from Git

- Requirements: Repository guidance / startup
- Role: Operator / reviewer
- Priority: P1
- Test data: Dedicated sentinel row; existing config; repository tracked-file list
- Steps: Review startup for explicit initialization; restart only owned isolated server without Initialize/Seed; inspect tracked files.
- Expected result: Normal start leaves sentinel/history unchanged; local config/keys/build artifacts/secrets absent from version control.
- Existing automation reference: Manual
- Initial status: Not Run
