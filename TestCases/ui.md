# Responsive accessible end-to-end screens

Source: [PRD v1.2](../PRD_Doc/Training_Management_PRD.md), including the Create training plan update. All cases are **Not Run**. A listed automation reference indicates source coverage only, never a recorded execution result.

Prerequisites: Desktop1440×1000/mobile390×844 plus current supported browsers; role sessions; real keyboard and dedicated save fixtures; official local logo.

Use the [fixture conventions and execution instructions](README.md) before changing dedicated records. The CSV is the authoritative execution register; these Markdown files present the initial case definitions.

## TC-UI-001 — Branding readable screens and navigation

- Requirements: Core screens and workflow; UI conventions
- Role: All roles
- Priority: P1
- Test data: Login/dashboard/masters/plans/batches/attendance/feedback/reports
- Steps: Visit permitted screens; inspect logo/blue-green palette/sidebar/metric/filter/table consistency.
- Expected result: Schoolnet branding/local official logo retained; readable headings/tables; active navigation clear; no implementation jargon.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-UI-002 — Keyboard labels focus and errors

- Requirements: Quality and operational requirements
- Role: All roles
- Priority: P1
- Test data: Forms/modals/navigation/filter controls
- Steps: Tab/Shift-Tab through screens; activate via keyboard; trigger required-field errors; inspect labels/focus.
- Expected result: All controls reachable and labelled; visible focus and readable field-level errors; error recovery possible without mouse.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-UI-003 — Mobile attendance and feedback workflow

- Requirements: Quality and operational requirements
- Role: Trainer/Trainee
- Priority: P1
- Test data: Phone390×844; current-day dedicated fixture
- Steps: Open nav; select batch/day; mark exception and save; submit feedback; rotate.
- Expected result: Essential controls readable/touch usable; no clipped submit/status; tables scroll appropriately; values persist.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-UI-004 — Desktop and mobile browser compatibility

- Requirements: Quality and operational requirements
- Role: All roles
- Priority: P1
- Test data: Agreed current Edge/Chrome/Firefox/Safari and mobile OS matrix
- Steps: Execute key create/schedule/attendance/feedback/export smoke journey in each agreed browser.
- Expected result: Consistent functional results without client errors; unsupported environment explicitly documented.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-UI-005 — Empty loading validation and failure states

- Requirements: UI conventions; FR21
- Role: All roles
- Priority: P1
- Test data: No matches; no batches; invalid form; simulated save failure
- Steps: Filter to zero rows; submit invalid save; interrupt disposable test connection and retry.
- Expected result: Honest empty state; failures clear; no false success; entered compatible values retained for recovery.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-UI-006 — Full ten-day role journey

- Requirements: Core screens and workflow
- Role: Administrator/Coordinator/Trainer/Trainee
- Priority: P0
- Test data: Dedicated P/S P10, roster, trainer; controlled dateT0
- Steps: Create/publish plan; create batch/select/assign/schedule; run daily attendance and boundary feedback; complete/export.
- Expected result: All role handoffs work; numbered days/date derivation/isolation preserved; final report matches saved records.
- Existing automation reference: Manual
- Initial status: Not Run
