# Projects schools and locations

Source: [PRD v1.2](../PRD_Doc/Training_Management_PRD.md), including the Create training plan update. All cases are **Not Run**. A listed automation reference indicates source coverage only, never a recorded execution result.

Prerequisites: Administrator and scoped Coordinator; isolated active projects P/Q and inactive R; two schools S/T; State → District → Block → Cluster hierarchy.

Use the [fixture conventions and execution instructions](README.md) before changing dedicated records. The CSV is the authoritative execution register; these Markdown files present the initial case definitions.

## TC-MASTER-001 — Project create edit view search deactivate

- Requirements: FR01
- Role: Administrator
- Priority: P1
- Test data: Unique QA project code/name; optional description
- Steps: Create project; find by code/name; edit description; deactivate; reload.
- Expected result: Required fields persist; code remains unique; inactive status visible; audit timestamps and actor present.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-MASTER-002 — Project required and unique fields

- Requirements: FR01; Data requirements / Project
- Role: Administrator
- Priority: P1
- Test data: Empty/whitespace code/name; duplicate existing code
- Steps: Submit each invalid project; inspect response and stored row count.
- Expected result: Field-level error; no invalid/duplicate row created.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-MASTER-003 — School create edit search deactivate

- Requirements: FR01
- Role: Administrator
- Priority: P1
- Test data: Unique code/name; valid hierarchy; links to P/Q
- Steps: Create school linked to P/Q; edit name; search; deactivate; inspect historical references.
- Expected result: Valid values persist; school linked to both projects; history retained on deactivation.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-MASTER-004 — School hierarchy consistency

- Requirements: Data requirements / School; FR03
- Role: Administrator
- Priority: P1
- Test data: District from different state; cluster from different block
- Steps: Tamper school form combinations and submit valid-token request.
- Expected result: Inconsistent hierarchy rejected on server; existing school unchanged.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-MASTER-005 — School required code location and project links

- Requirements: Data requirements / School
- Role: Administrator
- Priority: P1
- Test data: Blank code/name; missing location; no project; duplicate code
- Steps: Submit one invalid combination at a time.
- Expected result: Validation identifies invalid field/link; no partial school or relationship created.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-MASTER-006 — Project-school links control selectors

- Requirements: FR02
- Role: Administrator/Coordinator
- Priority: P0
- Test data: S linked P; T linked Q
- Steps: Open trainee/batch form for P; select S; tamper school_id=T and save.
- Expected result: Only eligible schools offered; server rejects T regardless of selector manipulation.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-MASTER-007 — Cannot remove links in active use

- Requirements: FR02
- Role: Administrator
- Priority: P0
- Test data: Active trainee and Draft/Scheduled batch using P/S
- Steps: Attempt removal of P/S link; reload association and dependent records.
- Expected result: Invalidating link removal blocked; dependent profiles/batches remain valid.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-MASTER-008 — Inactive project school block new associations

- Requirements: Data requirements / Project; FR08
- Role: Administrator/Coordinator
- Priority: P1
- Test data: Inactive R and inactive S
- Steps: Try create trainee/batch using inactive records via UI and POST.
- Expected result: New references rejected; existing historical records remain viewable.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-MASTER-009 — Scoped Coordinator reads master records

- Requirements: FR01; Users and permissions
- Role: Coordinator
- Priority: P0
- Test data: Assigned P only; Q records
- Steps: Open master listings; request Q master detail ID; attempt project/school mutation.
- Expected result: Only permitted records visible; unauthorized mutations denied; shared-plan selection does not broaden scope.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-MASTER-010 — Location hierarchy administration

- Requirements: Users and permissions; Core screens and workflow
- Role: Administrator
- Priority: P1
- Test data: New QA State/District/Block/Cluster
- Steps: Create valid parent-child chain; try invalid/missing parent; search/list; edit used location.
- Expected result: Valid chain usable in forms; invalid parent/level rejected; references remain consistent.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-MASTER-011 — Referenced master history preserved

- Requirements: Data requirements
- Role: Administrator
- Priority: P1
- Test data: Completed batch uses P/S/location/trainee
- Steps: Attempt destructive removal/deactivation using supported UI/routine; inspect completed reports.
- Expected result: Referenced records not deleted; historical names/associations remain available; eligible deactivation audited.
- Existing automation reference: Manual
- Initial status: Not Run
