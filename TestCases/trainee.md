# Trainee profiles and directory

Source: [PRD v1.2](../PRD_Doc/Training_Management_PRD.md), including the Create training plan update. All cases are **Not Run**. A listed automation reference indicates source coverage only, never a recorded execution result.

Prerequisites: Scoped Coordinator P; S/T schools linked to P; complete hierarchy; active and inactive dedicated trainees; Scheduled and Completed fixtures.

Use the [fixture conventions and execution instructions](README.md) before changing dedicated records. The CSV is the authoritative execution register; these Markdown files present the initial case definitions.

## TC-TRAINEE-001 — Create complete trainee profile

- Requirements: FR03; Data requirements / Trainee
- Role: Coordinator
- Priority: P1
- Test data: QA username; 'Anne-Marie O’Neil'; valid mobile/email; P/S
- Steps: Create trainee with required fields; reopen profile and directory.
- Expected result: All values persist; common punctuation/spaces supported; responsible actor and timestamps set.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-TRAINEE-002 — Required profile fields

- Requirements: FR03; Data requirements / Trainee
- Role: Coordinator
- Priority: P1
- Test data: Omit username/name/mobile/email/project/school/location separately
- Steps: Save each omission using valid antiforgery token.
- Expected result: Specific validation errors; no incomplete trainee created.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-TRAINEE-003 — Case-trim username uniqueness

- Requirements: Data requirements / Trainee
- Role: Coordinator
- Priority: P0
- Test data: ' qa.teacher ' followed by 'QA.TEACHER'
- Steps: Save first profile; attempt second; inspect normalized stored username.
- Expected result: Username lowercased/trimmed; duplicate rejected globally, including another project.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-TRAINEE-004 — Mobile text and configured country format

- Requirements: Data requirements / Trainee
- Role: Coordinator
- Priority: P1
- Test data: Valid configured-format mobile; alphabetic/short/too-long values
- Steps: Save valid number; reopen; save invalid variants.
- Expected result: Mobile stored as text without numeric conversion; invalid country-format inputs rejected.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-TRAINEE-005 — Email syntax validation

- Requirements: Data requirements / Trainee
- Role: Coordinator
- Priority: P1
- Test data: teacher@example.com; teacher@; missing @
- Steps: Submit valid and invalid email values.
- Expected result: Valid email accepted; malformed values rejected with field message.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-TRAINEE-006 — Repeated contact warning

- Requirements: Data requirements / Trainee
- Role: Coordinator
- Priority: P1
- Test data: Two distinct usernames with same mobile/email
- Steps: Create first; submit second sharing contact; review warning and confirmation flow.
- Expected result: Repeated contact produces warning; contact reuse is not treated as identity uniqueness.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-TRAINEE-007 — School selection populates hierarchy

- Requirements: FR03
- Role: Coordinator
- Priority: P1
- Test data: S with known State/District/Block/Cluster
- Steps: Select S; inspect hierarchy values; tamper one location before save.
- Expected result: Displayed hierarchy follows S; server rejects inconsistent manual combination.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-TRAINEE-008 — Search and combined directory filters

- Requirements: FR03
- Role: Coordinator
- Priority: P1
- Test data: Profiles vary username/name/project/location/school/status
- Steps: Search partial username/name; combine each location and status filter; clear filters.
- Expected result: Matching authorized rows only; filters persist; empty result honest; no cross-project contact leakage.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-TRAINEE-009 — Block membership changes with active assignments

- Requirements: FR04
- Role: Coordinator
- Priority: P0
- Test data: Trainee assigned Scheduled or In progress P/S batch
- Steps: Change trainee project to Q or school to T; submit; inspect batch and profile.
- Expected result: Change rejected; current project/school preserved; conflict identified.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-TRAINEE-010 — Completed history retains assignment association

- Requirements: FR04
- Role: Coordinator
- Priority: P1
- Test data: Completed P/S assignment; eligible profile transfer to P/T
- Steps: Change current profile school; view old batch/export and new directory.
- Expected result: Permitted change affects current profile; old report retains association at assignment time.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-TRAINEE-011 — Other-project profile direct access and mutation

- Requirements: FR03; Users and permissions
- Role: Coordinator
- Priority: P0
- Test data: Q trainee ID; valid P session token
- Steps: Request edit directly; POST contact/project changes to Q ID.
- Expected result: Denied without private fields; no profile, user link or audit mutation.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-TRAINEE-012 — Trainee own profile only

- Requirements: Users and permissions; Core screens and workflow
- Role: Trainee
- Priority: P0
- Test data: Own and another trainee profile IDs
- Steps: Open personal profile and attendance; request other profile via query/direct ID.
- Expected result: Only own data returned; another trainee's personal contacts absent from page/export.
- Existing automation reference: Manual
- Initial status: Not Run
