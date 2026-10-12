# Reusable plans and versioning

Source: [PRD v1.2](../PRD_Doc/Training_Management_PRD.md), including the Create training plan update. All cases are **Not Run**. A listed automation reference indicates source coverage only, never a recorded execution result.

Prerequisites: Administrator; feedback templates V1/V2; isolated Draft and Published plans of 2, 4 and 10 days; two batches referencing Published plan.

Use the [fixture conventions and execution instructions](README.md) before changing dedicated records. The CSV is the authoritative execution register; these Markdown files present the initial case definitions.

## TC-PLAN-001 — Atomic ten-day plan creation publication

- Requirements: FR05; AC01
- Role: Administrator
- Priority: P0
- Test data: P10; ten numbered complete activities
- Steps: Create independent Draft P10; configure templates/topics; publish; count header/details and inspect schema.
- Expected result: Exactly one header, ten details numbered 1–10; no plan ownership or scheduled date columns; publication complete.
- Existing automation reference: DB: AC01 complete plan and no date columns (partial ownership check)
- Initial status: Not Run

## TC-PLAN-002 — Initial and final activities distinguished by day

- Requirements: FR06; AC02
- Role: Administrator
- Priority: P0
- Test data: P10
- Steps: Inspect plan detail, batch detail and boundary feedback links.
- Expected result: Days 1/10 are Feedback, presented as initial/final feedback; days 2–9 Classroom training; no separate assessment activity.
- Existing automation reference: DB: AC02 activity types (labels not covered)
- Initial status: Not Run

## TC-PLAN-003 — Minimum two-day plan

- Requirements: FR05; FR06; AC03
- Role: Administrator
- Priority: P0
- Test data: P2 with same template on both days
- Steps: Create/publish two-day plan; view details.
- Expected result: Days 1 and 2 Feedback; no intermediate classroom row.
- Existing automation reference: DB: AC03 two-day plan
- Initial status: Not Run

## TC-PLAN-004 — Invalid plan duration header and uniqueness

- Requirements: FR05; Data requirements / Trainingplan table
- Role: Administrator
- Priority: P1
- Test data: N=0,1,-1,2.5,blank; missing code/name; duplicate code/version
- Steps: Submit each malformed plan; inspect header/detail counts.
- Expected result: Minimum 2 integer enforced; required fields/version uniqueness enforced; no partial header/details.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-PLAN-005 — Missing duplicate and out-of-range day numbers

- Requirements: FR06; AC04
- Role: Administrator / DB verifier
- Priority: P0
- Test data: Draft P10; missing day3; duplicate day2; day0/day11
- Steps: Within rollback fixture attempt each invalid detail set and publication independently.
- Expected result: Publication rejected with invalid details; uniqueness/range enforcement prevents corrupt Published plan.
- Existing automation reference: DB: missing day publication only
- Initial status: Not Run

## TC-PLAN-006 — Invalid boundary and classroom configuration

- Requirements: FR06
- Role: Administrator
- Priority: P0
- Test data: Feedback without template; classroom without topics; classroom template; wrong type
- Steps: Try publish each invalid Draft; inspect validation and status.
- Expected result: Required templates/topics and allowed activity sequence enforced; no invalid publication.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-PLAN-007 — Draft duration growth preserves compatible content

- Requirements: FR07
- Role: Administrator
- Priority: P1
- Test data: Draft4 with titled/topic/instruction rows; change to6
- Steps: Grow duration; save; reopen numbered activities.
- Expected result: Exactly6 rows; compatible classroom content retained; old boundary converted consistently; new final feedback configured.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-PLAN-008 — Draft shrink warns before removing activities

- Requirements: FR07
- Role: Administrator
- Priority: P1
- Test data: Draft10 with topics/instructions; change to4
- Steps: Request shrink; inspect warning; cancel; repeat and confirm; save.
- Expected result: Removal warning shown; cancellation preserves original; confirmed save has rows1–4 and compatible content retained.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-PLAN-009 — Published plan immutable through direct mutation

- Requirements: FR07; AC12
- Role: Administrator / DB verifier
- Priority: P0
- Test data: Published10 referenced A/B
- Steps: Attempt header/detail edit through UI, direct POST and rollback SQL.
- Expected result: Content edits denied; both batches and their history unchanged.
- Existing automation reference: DB: Published detail UPDATE only
- Initial status: Not Run

## TC-PLAN-010 — Revision creates separate Draft version

- Requirements: FR07; AC12
- Role: Administrator
- Priority: P0
- Test data: Published P10 v1 used A/B
- Steps: Choose revise; compare new header/details/source/version; edit new Draft; inspect A/B.
- Expected result: New Draft v2 copies detail content with new IDs; source points v1; A/B retain v1/history.
- Existing automation reference: DB: AC12 version/status/reference (partial copy checks)
- Initial status: Not Run

## TC-PLAN-011 — Concurrent revisions allocate distinct versions

- Requirements: FR07
- Role: Administrator
- Priority: P1
- Test data: Two sessions revising same v1
- Steps: Trigger revision concurrently; inspect resulting headers/details and errors.
- Expected result: Version allocation cannot duplicate logical code/version; successful revisions complete; failures explicit and atomic.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-PLAN-012 — Archive used shared plan

- Requirements: FR07; AC21
- Role: Administrator/Coordinator
- Priority: P0
- Test data: Published P10 used A/B
- Steps: Archive; open A/B details; try select archived ID for new batch via POST.
- Expected result: Existing references remain readable; new selection rejected; history retained.
- Existing automation reference: DB: AC21 archived selection/reference
- Initial status: Not Run

## TC-PLAN-013 — Non-admin plan mutation denied

- Requirements: Users and permissions; FR05; FR07
- Role: Coordinator/Trainer/Trainee
- Priority: P0
- Test data: Draft and Published plan IDs
- Steps: GET /Plans/Edit; POST /Plans/Edit and /Plans/Action publish/revise/archive with valid token.
- Expected result: Every mutation denied; no header/detail/version/status changed.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-PLAN-014 — Shared selection across projects without data leakage

- Requirements: FR08; Users and permissions
- Role: Coordinator
- Priority: P0
- Test data: Published plan used both P/Q; Coordinator P
- Steps: Open plan listing/detail and select in P batch; inspect linked batches and rosters.
- Expected result: Shared plan available independent of project; only permitted usage counts/batches shown; no Q trainee data.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-PLAN-015 — Four-day default Pre form classroom Feedback editor

- Requirements: Create training plan update; FR05; FR06
- Role: Administrator
- Priority: P0
- Test data: New /Plans/Edit
- Steps: Open create form; inspect default duration, legends, inputs and submitted data.
- Expected result: Default4; exactly4 rows: Day1 Pre form, Days2/3 Classroom training, Day4 Feedback; both boundaries stored Feedback with corresponding templates.
- Existing automation reference: HTTP: default4; Browser: legends/visible controls; DB: default titles
- Initial status: Not Run

## TC-PLAN-016 — Automatic exact row resizing and roundtrip input preservation

- Requirements: Create training plan update; FR07
- Role: Administrator
- Priority: P0
- Test data: Draft input topics/titles/templates; N=2,4,5
- Steps: Fill row values; change4→2→4→5→4; inspect visible rows and FormData; save.
- Expected result: ExactlyN numbered rows visible and submitted; compatible inputs retained; boundary templates/classroom controls follow day position; persisted row count equalsN.
- Existing automation reference: Browser: resizing/input preservation/FormData counts; DB: resize counts
- Initial status: Not Run

## TC-PLAN-017 — Reject mismatched submitted row count

- Requirements: Create training plan update; FR05; FR07
- Role: Administrator
- Priority: P0
- Test data: N4 with one/three/five submitted day rows
- Steps: POST tampered activity arrays with valid token; reopen saved Draft.
- Expected result: Server rejects mismatch clearly; no partial changes or extra detail rows.
- Existing automation reference: HTTP/DB: one-row mismatch
- Initial status: Not Run
