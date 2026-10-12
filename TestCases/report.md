# Reports exports and calculated metrics

Source: [PRD v1.2](../PRD_Doc/Training_Management_PRD.md), including the Create training plan update. All cases are **Not Run**. A listed automation reference indicates source coverage only, never a recorded execution result.

Prerequisites: Permitted P/S A with3 trainees/4 days; one trainee Present2 Absent1 Not marked1; daily present2 of3; initial2 final1; zero-roster Draft; Q data.

Use the [fixture conventions and execution instructions](README.md) before changing dedicated records. The CSV is the authoritative execution register; these Markdown files present the initial case definitions.

## TC-REPORT-001 — Report fields shared plan and numbered activities

- Requirements: FR21
- Role: Administrator/Coordinator
- Priority: P0
- Test data: A/B share versioned plan
- Steps: Open reports/batch summary; export; compare project/school/dates/duration/plan/version/activities/roster/status/feedback.
- Expected result: All required batch summary/export fields accurate; day numbers and batch-derived dates distinct from shared plan content.
- Existing automation reference: HTTP: CSV content contains plan only (partial fields)
- Initial status: Not Run

## TC-REPORT-002 — Combined report filters and export parity

- Requirements: FR21
- Role: Coordinator
- Priority: P1
- Test data: Project/school/batch/plan/date range/trainee variants
- Steps: Apply filters individually and combined; export each; clear; try reversed/invalid dates.
- Expected result: Display/export same permitted filter set; invalid ranges validated; no unrelated rows or leakage.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-REPORT-003 — Trainee attendance denominator includes unmarked

- Requirements: FR21
- Role: Coordinator/Trainee
- Priority: P0
- Test data: 4 days: Present2 Absent1 Not marked1
- Steps: View trainee summary and CSV.
- Expected result: Attendance50%; Absent1 and Not marked1 shown; provisional indication; unmarked not collapsed into Absent.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-REPORT-004 — Daily attendance percentage and feedback denominators

- Requirements: FR21
- Role: Coordinator
- Priority: P1
- Test data: Roster3; Present2; initial2; final1
- Steps: View each daily/feedback summary and compare calculations.
- Expected result: Daily/initial66.67% and final33.33% at documented rounding; denominators all3 assignments; attendance independent.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-REPORT-005 — Zero denominator Not applicable

- Requirements: FR21
- Role: Coordinator
- Priority: P1
- Test data: Zero-roster Draft A
- Steps: Open summary/dashboard/report/export.
- Expected result: Zero-denominator percentages show Not applicable; no divide-by-zero, misleading0% or server failure.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-REPORT-006 — Scoped direct export and filter tampering

- Requirements: FR21; AC20
- Role: Coordinator/Trainee
- Priority: P0
- Test data: Q IDs and other trainee username/ID
- Steps: Export with Q project/batch and other trainee filters; inspect rows and contacts.
- Expected result: No outside-scope records; Trainee export only own records regardless query IDs.
- Existing automation reference: HTTP: personal export own/other name; Coordinator listing/detail partial
- Initial status: Not Run

## TC-REPORT-007 — CSV escaping Unicode and formula-safe cells

- Requirements: Quality and operational requirements
- Role: Coordinator
- Priority: P1
- Test data: Names/remarks with comma quote newline Unicode and = + - @ prefixes
- Steps: Save dedicated safe test data; export; open text and spreadsheet.
- Expected result: Proper column/quote/newline encoding; Unicode readable; cells do not execute spreadsheet formulas; data meaning preserved.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-REPORT-008 — Historical report survives profile and plan revision

- Requirements: FR04; FR07; FR21
- Role: Coordinator
- Priority: P0
- Test data: Completed A; later trainee transfer; new plan/template version
- Steps: Compare original snapshot to current report/export.
- Expected result: Assignment-time project/school and original plan/template version retained; response interpretation unchanged.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-REPORT-009 — Trainer reports limited to assigned batches

- Requirements: FR21; Users and permissions
- Role: Trainer
- Priority: P0
- Test data: AssignedA unassignedB same project
- Steps: View report/export; force B ID/project filters.
- Expected result: Only assigned authorized batch records; no unassigned rosters/contact details.
- Existing automation reference: Manual
- Initial status: Not Run
