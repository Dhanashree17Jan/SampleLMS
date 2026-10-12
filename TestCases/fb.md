# Initial final and offline feedback

Source: [PRD v1.2](../PRD_Doc/Training_Management_PRD.md), including the Create training plan update. All cases are **Not Run**. A listed automation reference indicates source coverage only, never a recorded execution result.

Prerequisites: Dedicated assigned trainee; P2/P10 with required rating/choice/text; A/B sharing plan; available dayT0 and unavailable variants; separate staff accounts.

Use the [fixture conventions and execution instructions](README.md) before changing dedicated records. The CSV is the authoritative execution register; these Markdown files present the initial case definitions.

## TC-FB-001 — Initial and final submission separation

- Requirements: FR17; AC11
- Role: Trainee
- Priority: P0
- Test data: Own A assignment; boundary day fixtures
- Steps: Submit valid initial on its date; submit valid final on its date; inspect IDs/answers.
- Expected result: Two separate final submissions attached to own assignment and correct numbered details/template versions.
- Existing automation reference: DB: AC11 offline administrator submissions (partial trainee timing)
- Initial status: Not Run

## TC-FB-002 — Wrong date and classroom feedback denied

- Requirements: FR17; AC23
- Role: Trainee
- Priority: P0
- Test data: Future/past boundary; day2 classroom inP10; day0/N+1
- Steps: Request and POST forms on each unavailable day.
- Expected result: Submission unavailable; clear date/day denial; no submission created.
- Existing automation reference: DB: not covered by future attendance assertion
- Initial status: Not Run

## TC-FB-003 — Required rating choice text validation

- Requirements: FR16; FR18
- Role: Trainee/Coordinator
- Priority: P0
- Test data: Blank required answer; rating0/6/nonnumber; choice outside list
- Steps: Submit each invalid answer with proper reason/token; submit optional blanks.
- Expected result: Invalid required/range/choice rejected server-side; optional blank allowed; existing final response not damaged.
- Existing automation reference: DB: required/rating6 only
- Initial status: Not Run

## TC-FB-004 — Offline late feedback reason and attribution

- Requirements: FR18
- Role: Coordinator/Trainer
- Priority: P0
- Test data: Past boundary; assigned staff/trainee
- Steps: Submit blank reason; repeat valid reason; inspect submitted_by/time, trainee and audit.
- Expected result: Only authorized staff exception allowed; mandatory reason and attribution stored; does not impersonate trainee.
- Existing automation reference: DB: administrator offline reason/audit only
- Initial status: Not Run

## TC-FB-005 — Final submission locking and correction audit

- Requirements: FR18
- Role: Trainee/Coordinator
- Priority: P0
- Test data: Existing final submission
- Steps: Attempt trainee overwrite; attempt authorized staff correction without/with reason; inspect history.
- Expected result: Final trainee submission locked; authorized correction retains original and before/after audit with actor/reason.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-FB-006 — Another trainee feedback direct GET and POST denied

- Requirements: FR17; AC20
- Role: Trainee
- Priority: P0
- Test data: Own assignmentA; other learner assignmentA/B
- Steps: GET /Feedback?assignment=other&day=1; POST /Feedback/Save with valid token.
- Expected result: No another learner answers/form content; no submission/overwrite; denied or not found.
- Existing automation reference: HTTP: other-assignment GET; DB: learner foreign submission
- Initial status: Not Run

## TC-FB-007 — Feedback independent of attendance

- Requirements: FR17; AC18
- Role: Trainee/Coordinator
- Priority: P0
- Test data: Not marked initial/final attendance
- Steps: Submit boundary feedback; inspect attendance counts/status/revision.
- Expected result: Feedback completion increases; all unmarked attendance stays unchanged.
- Existing automation reference: DB: AC18 final-day status unchanged
- Initial status: Not Run

## TC-FB-008 — Shared-plan feedback isolated by assignment

- Requirements: FR17; AC10; AC11
- Role: Trainee/Coordinator
- Priority: P1
- Test data: Same trainee non-overlap A/B sameP10/template
- Steps: Submit A initial; view B feedback completion and records.
- Expected result: B has no submission; A answer not reused by plan detail or trainee identity.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-FB-009 — Cancelled batch feedback denied

- Requirements: FR20
- Role: Trainee/Coordinator
- Priority: P0
- Test data: Cancelled A with/without earlier feedback
- Steps: Request form; POST new answer and correction.
- Expected result: Existing history retained; no new submission/correction on Cancelled batch.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-FB-010 — Wrong plan or template feedback FK integrity

- Requirements: Data relationships and integrity
- Role: DB verifier
- Priority: P0
- Test data: Own assignment; detail/template from another plan/version
- Steps: In rollback transaction attempt mismatched insert/update and duplicate final submission.
- Expected result: Assignment/detail/template association and uniqueness enforced; existing submissions remain valid.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-FB-011 — Concurrent final submissions atomic uniqueness

- Requirements: FR18; Quality and operational requirements
- Role: Trainee
- Priority: P1
- Test data: Two sessions same assignment/boundary
- Steps: Submit two different valid answers concurrently; inspect rows and error.
- Expected result: Exactly one final submission; loser has clear locked/conflict error; no partial answers/audit.
- Existing automation reference: Manual
- Initial status: Not Run
