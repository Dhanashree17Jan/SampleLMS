# Authentication and users

Source: [PRD v1.2](../PRD_Doc/Training_Management_PRD.md), including the Create training plan update. All cases are **Not Run**. A listed automation reference indicates source coverage only, never a recorded execution result.

Prerequisites: Dedicated active accounts for all four roles; disabled account; two independent browser sessions. Read passwords privately from ignored configuration.

Use the [fixture conventions and execution instructions](README.md) before changing dedicated records. The CSV is the authoritative execution register; these Markdown files present the initial case definitions.

## TC-AUTH-001 — Valid role sign-in

- Requirements: Users and permissions; Quality and operational requirements
- Role: All roles
- Priority: P0
- Test data: admin/coordinator/trainer/trainee
- Steps: Sign in separately using each dedicated account; open dashboard; inspect menu and identity.
- Expected result: Authentication succeeds; identity and actions match role; no contact data outside scope.
- Existing automation reference: HTTP: role login
- Initial status: Not Run

## TC-AUTH-002 — Invalid and missing credentials

- Requirements: Users and permissions
- Role: Anonymous
- Priority: P1
- Test data: Unknown username; wrong password; blank fields
- Steps: Submit each variant on /Account/Login; revisit protected dashboard.
- Expected result: No authenticated session is created; generic readable error; no user enumeration or password disclosure.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-AUTH-003 — Normalized login identity

- Requirements: Data requirements / Trainee
- Role: Administrator
- Priority: P1
- Test data: ' QA.Learner ' and 'qa.learner'
- Steps: Create first dedicated username; attempt second normalized duplicate; sign in using supported normalization.
- Expected result: Globally normalized duplicate rejected; original identity and profile unchanged.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-AUTH-004 — Inactive user cannot authenticate

- Requirements: Users and permissions
- Role: Anonymous
- Priority: P1
- Test data: Inactive dedicated login
- Steps: Attempt login; try protected URL with previously issued session after deactivation.
- Expected result: No new login; disabled user cannot perform protected actions; any session invalidation policy is recorded.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-AUTH-005 — Logout and cookie expiry

- Requirements: Users and permissions
- Role: All roles
- Priority: P1
- Test data: Existing session
- Steps: Sign out using UI; navigate back; reload protected URL; replay expired session.
- Expected result: Authentication required after logout/expiry; protected data cannot be fetched from server.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-AUTH-006 — Local return URL enforcement

- Requirements: Quality and operational requirements
- Role: Anonymous
- Priority: P1
- Test data: returnUrl=/Reports and https://example.invalid/
- Steps: Sign in with each returnUrl supplied to /Account/Login.
- Expected result: Local return succeeds; external URL cannot cause open redirect.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-AUTH-007 — Administrator manages user roles and project scope

- Requirements: Users and permissions
- Role: Administrator
- Priority: P1
- Test data: Dedicated Coordinator; project P/Q
- Steps: Create user; assign only P; edit role/project scope; sign in as user.
- Expected result: Role and scoped access follow saved assignment; user changes audited with responsible actor.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-AUTH-008 — Non-admin user management denied

- Requirements: Users and permissions
- Role: Coordinator/Trainer/Trainee
- Priority: P0
- Test data: /Masters?kind=user; /Masters/Edit?kind=user
- Steps: Request listing/edit directly; replay valid user-edit POST with current antiforgery token.
- Expected result: No user data or mutation; denied response; database unchanged.
- Existing automation reference: HTTP: Trainer user listing only
- Initial status: Not Run

## TC-AUTH-009 — Password and credential protection

- Requirements: Quality and operational requirements
- Role: Administrator
- Priority: P1
- Test data: Dedicated login and repository index
- Steps: Inspect rendered HTML, logs, errors, configuration tracking and stored password format without copying secrets.
- Expected result: No plaintext password displayed/logged/tracked; credentials stored using password hashing; secret configuration ignored.
- Existing automation reference: Manual
- Initial status: Not Run

## TC-AUTH-010 — Profile changes do not alter login credentials

- Requirements: Users and permissions
- Role: Coordinator
- Priority: P1
- Test data: Dedicated trainee and its separate login
- Steps: Update trainee name/contact fields; sign in with existing login; inspect role.
- Expected result: Profile saved independently; password and authorization unchanged.
- Existing automation reference: Manual
- Initial status: Not Run
