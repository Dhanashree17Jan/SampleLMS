# Feedback templates and stable versions

Source: [PRD v1.2](../PRD_Doc/Training_Management_PRD.md), including the Create training plan update. All cases are **Not Run**. A listed automation reference indicates source coverage only, never a recorded execution result.

Prerequisites: Administrator; dedicated templates with Rating, Choice and Text questions; Published plan and submitted response referencing original template.

Use the [fixture conventions and execution instructions](README.md) before changing dedicated records. The CSV is the authoritative execution register; these Markdown files present the initial case definitions.

## TC-TEMPLATE-001 — Create question types and mandatory fields

- Requirements: FR16
- Role: Administrator
- Priority: P1
- Test data: Rating, choices A/B, required text, optional text
- Steps: Create template; configure questions; save; open feedback form.
- Expected result: All supported controls and labels rendered; requiredness and valid choices preserved.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-TEMPLATE-002 — Invalid template configuration

- Requirements: FR16
- Role: Administrator
- Priority: P1
- Test data: Blank name/label; duplicate question IDs; empty choice options; unsupported type
- Steps: Submit each invalid template directly and via form.
- Expected result: Invalid form cannot become usable version; clear validation; existing template unchanged.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-TEMPLATE-003 — Editing referenced template preserves old version

- Requirements: FR16
- Role: Administrator
- Priority: P0
- Test data: TemplateV1 used P10 and response; edit question/choices
- Steps: Save edited template as version; inspect old plan/form/response and new selector.
- Expected result: V1 unchanged and interpretable; new version separately selectable; old answer meaning retained.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-TEMPLATE-004 — Same template supports both boundary submissions

- Requirements: FR16; FR17; AC11
- Role: Administrator/Trainee
- Priority: P0
- Test data: P2 with same V1 on both days
- Steps: Submit initial/final on available fixture dates; compare template/detail/assignment IDs.
- Expected result: Two distinct submissions tied to respective details despite same version; no overwrite.
- Existing automation reference: DB: AC11 boundary submissions same template
- Initial status: Not Run

## TC-TEMPLATE-005 — Different initial final templates

- Requirements: FR16; FR17
- Role: Administrator/Trainee
- Priority: P1
- Test data: P2 uses V1 initial V2 final
- Steps: Open boundary forms; submit valid answers for each.
- Expected result: Correct separate template version and questions used on each day.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-TEMPLATE-006 — Template deletion protection

- Requirements: FR16; Quality and operational requirements
- Role: Administrator / DB verifier
- Priority: P0
- Test data: Version referenced by Published plan/feedback
- Steps: Attempt supported deletion and rollback DB deletion.
- Expected result: Foreign keys/history protection prevent deletion of referenced version.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-TEMPLATE-007 — Non-admin template changes denied

- Requirements: Users and permissions; FR16
- Role: Coordinator/Trainer/Trainee
- Priority: P0
- Test data: Existing template ID
- Steps: Direct GET/POST /Masters/Edit?kind=template with valid role token.
- Expected result: No unauthorized template mutation or private administration data.
- Existing automation reference: Manual
- Initial status: Not Run
