# Training Management Application Product Requirements Document

Version: 1.2  
Date: 12 October 2026  
Status: Draft for product review

## Product purpose

The application manages projects, schools, trainees, reusable training plans, and batches. Each batch references one training plan, and any number of batches can share the same plan. Plans describe Day 1, Day 2, through Day N using day numbers rather than calendar dates. Day 1 and Day N contain feedback activities; intermediate days contain classroom training.

Trainees are assigned to batches. Attendance and feedback submissions belong to the trainee's batch assignment, so batches sharing a plan keep their training records separate.

## Scope

### Required capabilities

- Maintain projects and schools.
- Maintain trainees with username, name, mobile number, email address, project, state, district, block, cluster, and school.
- Maintain batches with project, school, number of days, start date, end date, and training plan.
- Maintain training plans using two tables: `trainingplan` for the plan header and `trainingplandetails` for its numbered daily activities.
- Allow one training plan to be shared by any number of batches, including batches from different projects or schools.
- Assign trainees to batches and record attendance for every trainee on every batch day.
- Capture initial feedback on Day 1 and final feedback on Day N; conduct classroom training on Days 2 through N minus 1.

### Proposed supporting capabilities

Role permissions, location master data, feedback templates, lifecycle controls, audit history, dashboards, and CSV reports support these workflows. Their details below are proposed defaults.

### Outside the first release

Online training delivery, video hosting, payments, certificates, payroll, biometric attendance, and disconnected application operation are outside scope. Separate pre-assessment and post-assessment activities are outside the current scope; both boundary days use feedback.

## Assumptions and decisions

| Topic | Proposed default |
| --- | --- |
| Counting batch days | Consecutive calendar days, including weekends and holidays. |
| Minimum duration | At least two days. A two-day plan has initial and final feedback with no intermediate classroom training. |
| Batch dates | Enter start date and number of days; calculate end date. If all three are supplied, validate agreement. |
| Plan duration | The selected plan's number of days must equal the batch's number of days. |
| Feedback content | Initial and final feedback may use the same or different templates. Configure a template separately for each boundary day. |
| Project membership | Each trainee belongs to one project; schools can link to multiple projects. |
| School eligibility | Assigned trainees must match the batch project and school. |
| Batch membership | Trainees may join multiple non-overlapping batches; duplicate assignment is prohibited. |
| Joining a batch | Complete assignments before scheduling. Late joining and withdrawal during training are outside the initial workflow. |
| Attendance values | Present, Absent, and Not marked. Not marked is never silently treated as Absent. |
| Form delivery | Trainees submit forms online; authorized staff can enter offline responses with attribution. |
| Time zone | A configured application time zone governs day restrictions. Batch dates are stored as dates. |

## Users and permissions

| Role | Permissions |
| --- | --- |
| Administrator | Manage users, projects, schools, locations, shared plans, and feedback templates; review all reports and audit history. |
| Coordinator | Manage trainees and batches within assigned projects; select published shared plans; assign trainees; schedule batches; review completion and make authorized corrections. |
| Trainer | View assigned batches, plans, and rosters; record daily attendance and authorized offline responses. |
| Trainee | View their own profile, batch plan, and attendance; submit their initial and final feedback. |

Published shared plans are available for selection across projects and schools. This does not grant access to another project's batches, trainee details, attendance, or submissions.

## Data requirements

All records have a system-generated ID, creation time, update time, and responsible user. Preserve records referenced by training history; deactivate eligible master records rather than deleting them.

### Project

| Field | Requirement |
| --- | --- |
| Project ID | Unique identifier. |
| Project code | Required and unique. |
| Project name | Required. |
| Description | Optional. |
| Status | Active or Inactive. Inactive projects cannot receive new batches or trainees. |

### School

| Field | Requirement |
| --- | --- |
| School ID | Unique identifier. |
| School code | Required and unique. |
| School name | Required. |
| State, district, block, cluster | Required references to the location hierarchy. |
| Projects | One or more project links through a project-school relationship. |
| Status | Active or Inactive. |

### Trainee

| Field | Requirement |
| --- | --- |
| Trainee ID | Unique identifier. |
| Username | Required; globally unique after trimming and case normalization. |
| Name | Required; allow spaces and common name punctuation. |
| Mobile number | Required; store as text and validate using the configured country format. |
| Email address | Required; validate email syntax. |
| Project | Required active project reference. |
| State | Required location reference. |
| District | Required; belongs to the selected state. |
| Block | Required; belongs to the selected district. |
| Cluster | Required; belongs to the selected block. |
| School | Required; matches the selected location hierarchy and is linked to the selected project. |
| Status | Active or Inactive. |

Selecting a school populates its location fields. Prevent inconsistent combinations. Warn about repeated mobile numbers or email addresses without assuming they uniquely identify a person. Login credentials are separate from profile data.

### Batch

| Field | Requirement |
| --- | --- |
| Batch ID | Unique identifier. |
| Batch code | Required and unique. |
| Batch name | Required display name. |
| Project | Required active project reference. |
| School | Required active school linked to the selected project. |
| Number of days | Required integer of at least 2; must equal the selected training plan duration. |
| Start date | Required calendar date. |
| End date | Required; start date plus number of days minus 1. |
| Training plan | Required `training_plan_id` foreign key referencing `trainingplan.training_plan_id`. Multiple batches may reference the same plan; this field is not unique across batches. |
| Status | Draft, Scheduled, In progress, Completed, or Cancelled. |
| Assigned trainer | Proposed field; required before scheduling. |

### Trainingplan table

The `trainingplan` table contains reusable plan headers. A plan has no batch, project, school, start date, or end date ownership fields. The proposed versioning fields preserve shared plan history.

| Field | Requirement |
| --- | --- |
| `training_plan_id` | Primary key. Referenced by batches and plan details. |
| `plan_code` | Required logical plan code; unique together with version number. |
| `plan_name` | Required display name. |
| `number_of_days` | Required integer of at least 2. |
| `description` | Optional purpose or summary. |
| `version_number` | Required positive integer; starts at 1. |
| `source_training_plan_id` | Optional reference to the previous version when a published plan is copied for revision. |
| `status` | Draft, Published, or Archived. Only Published plans can be newly selected for batches. |

### Trainingplandetails table

The `trainingplandetails` table contains one numbered activity entry per day of a training plan. It has no calendar date field. Audit timestamps record changes and do not represent scheduled training dates.

| Field | Requirement |
| --- | --- |
| `training_plan_detail_id` | Primary key. |
| `training_plan_id` | Required foreign key to `trainingplan`. |
| `day_number` | Required integer from 1 through the parent plan's `number_of_days`. Unique within the plan. |
| `activity_type` | Feedback for Day 1 and Day N; Classroom training for every intermediate day. |
| `title` | Required activity title, such as Initial feedback, Classroom training, or Final feedback. |
| `topics` | Training topics for classroom days. |
| `instructions` | Optional activity guidance. |
| `feedback_template_version_id` | Required for feedback days; empty for classroom days. References the chosen versioned feedback form. |
| `duration_minutes` | Optional positive activity duration. |

Day 1 is initial feedback and Day N is final feedback, distinguished by their day numbers even though both have activity type Feedback. A valid plan has exactly N detail rows numbered 1 through N without duplicates or gaps.

Actual trainers, venue, and calendar scheduling belong to the batch context. Viewing or selecting a plan shows numbered days. Attendance screens may display a derived batch date using:

`batch day date = batch start date + (day_number - 1)`

This derived date is not stored in either plan table. Batches sharing a plan can therefore have different start and end dates.

### Assignment attendance and feedback records

| Record | Required data and constraints |
| --- | --- |
| Batch assignment | Batch, trainee, assignment timestamp, assigned by; unique batch-trainee pair. |
| Daily attendance | Batch assignment, `training_plan_detail_id`, status, remarks, marked by, marked at; unique assignment-plan-detail pair. |
| Feedback submission | Batch assignment, `training_plan_detail_id`, template version, answers, submitted by, submitted at; one final submission per assignment and feedback day. |

Attendance and feedback completion are independent. Submitting feedback does not mark attendance. Records from batches sharing a plan remain separate because each record references its own batch assignment.

## Functional requirements

### Master data and trainees

**FR01:** Administrators can create, view, edit, search, and deactivate projects and schools. Coordinators can view records within their scope.

**FR02:** Project-school links control school eligibility. Prevent changes that invalidate active trainee or batch associations.

**FR03:** Authorized users can maintain trainee profiles, search by username or name, and filter by project, location, school, and status. Validate required fields and location relationships.

**FR04:** Block trainee project or school changes while they have Scheduled or In progress assignments. Completed reports retain the association at assignment time.

### Reusable training plans

**FR05:** Administrators create a Draft `trainingplan` header with duration N and generate N `trainingplandetails` rows numbered 1 through N. Creation is atomic and does not require a batch.

**FR06:** Set Day 1 and Day N to Feedback and intermediate days to Classroom training. Configure initial and final feedback templates and classroom topics before publication. Reject missing, repeated, or out-of-range day numbers and invalid activity types.

**FR07:** Draft plan duration changes update its detail rows. Preserve compatible topics and instructions, and warn before removing rows. Publishing requires a complete valid plan. Published or referenced plan content is immutable; revisions create a new Draft version and copied detail rows in the same two tables, leaving existing references unchanged. Archived plans cannot be newly selected but remain accessible through existing batches.

### Batch setup and plan selection

**FR08:** The batch form includes a required Training plan selector showing Published plans with their name, version, and duration. Multiple batches may select the same plan. Selecting a plan sets the batch duration to the plan duration and calculates end date from start date. Reject manually supplied duration mismatches. Saving a batch references the existing plan without copying its header or details.

**FR09:** Changing a Draft batch start date recalculates its end date and derived attendance dates without modifying the shared plan. Changing its duration requires selection of another matching Published plan. A Draft batch can switch plans before attendance has been marked or feedback submitted; reconcile its unmarked attendance rows atomically and preserve the roster. Block plan replacement once training records exist.

**FR10:** Scheduling requires a valid project and school, assigned trainer, at least one trainee, and a complete selected Published plan. Lock project, school, dates, duration, plan reference, and roster after scheduling. A Scheduled batch may return to Draft before its start date if no attendance has been marked and no feedback submitted. Plan selection never changes another batch.

### Trainee assignment

**FR11:** Coordinators can assign eligible active trainees individually or through multi-select in Draft batches. Reject duplicate assignments, project or school mismatches, and overlaps with another Scheduled or In progress batch. Recheck overlaps on scheduling and Draft date changes; sharing a plan itself does not create a conflict.

**FR12:** Removing a Draft assignment removes only its unmarked attendance rows. Preserve assignments with training history. Initialize one Not marked attendance entry per assigned trainee per selected plan detail. Expected entry count equals assigned trainees multiplied by batch duration.

### Daily attendance

**FR13:** Trainers and coordinators select a batch and day number, view its derived date and roster, mark Present or Absent, add remarks, and save. Bulk marking Present allows adjustment of exceptions before saving.

**FR14:** Repeated saves update the existing attendance record. Detect concurrent changes and show save failures clearly. Do not combine attendance across batches that share a plan.

**FR15:** Prevent attendance entry before scheduling, for future derived dates, or for Cancelled batches. Coordinators may correct past entries with a mandatory reason. Completed batches require administrator-authorized corrections with audit history.

### Initial and final feedback

**FR16:** Administrators define versioned feedback templates with ratings, choices, required questions, and text responses. A plan can use the same template on both boundary days or different templates. Template versions remain stable for recorded responses.

**FR17:** Initial feedback is available on the derived date for Day 1; final feedback is available on the derived date for Day N. Only trainees assigned to the relevant batch can submit. Store the two submissions separately even when they use the same template.

**FR18:** Authorized coordinators can enter late or offline feedback with a reason and attribution. Lock final submissions; corrections retain audit history. Feedback is identified by trainee under the proposed default; anonymity requires a product decision.

### Lifecycle and reporting

**FR19:** Scheduled batches become In progress on their start date. After the end date, coordinators can complete a batch only when every attendance entry is Present or Absent. Show missing initial and final feedback; these do not block completion under the proposed default.

**FR20:** Allow cancellation with a reason. Preserve records, disable new attendance and submissions, and exclude Cancelled batches from overlap checks and active totals.

**FR21:** Provide batch summaries and CSV exports with project, school, dates, duration, plan name and version, numbered activities, roster, attendance, and initial and final feedback completion. Filter by project, school, batch, plan, date range, and trainee within access permissions.

Use these definitions consistently:

- Trainee attendance percentage = Present days / total batch days x 100; show Absent and Not marked counts and indicate provisional results when days remain unmarked.
- Daily attendance percentage = Present trainees / assigned trainees x 100.
- Initial feedback completion percentage = Day 1 final submissions / assigned trainees x 100.
- Final feedback completion percentage = Day N final submissions / assigned trainees x 100.
- Display Not applicable when a denominator is zero.

## Core screens and workflow

### Create training plan update

The create form defaults to four training days. A four-day plan contains exactly four activities: Day 1 **Pre form**, Days 2 and 3 **Classroom training**, and Day 4 **Feedback**. Choosing another supported duration N automatically shows exactly N numbered rows before saving. Pre form uses the initial boundary feedback template and submission workflow; the final Feedback uses the final boundary workflow. Both remain stored as Feedback activity types. Existing Published plans and recorded training history retain their saved content.

| Screen | Main actions |
| --- | --- |
| Dashboard | View batch status, upcoming activities, missing attendance, and feedback completion. |
| Projects and schools | Maintain master records and project-school links. |
| Trainees | Maintain profiles and search or filter the directory. |
| Training plans | Create plan headers; maintain Day 1 through Day N details; publish, version, archive, and view batches using each plan. |
| Batches | Select a Published plan and review project, school, dates, duration, and status. |
| Batch detail | View referenced plan name, version, numbered daily activities, roster, attendance, and initial and final feedback. Shared plan content is read-only here. |
| Daily attendance | Select a batch and day number; show its derived date and record roster attendance. |
| Trainee view | View personal batch information, numbered plan, attendance, and available feedback forms. |
| Reports | Filter and export permitted records. |

An administrator creates and publishes a reusable numbered plan. A coordinator creates a batch, selects that plan, enters the start date, and reviews the calculated end date. The coordinator assigns eligible trainees and a trainer and schedules the batch. Initial feedback occurs on Day 1, classroom training on intermediate days, and final feedback on Day N. Record attendance every day. After training ends, resolve unmarked attendance and complete the batch.

## Example shared ten day plan

The following rows belong to one `trainingplan` header with `number_of_days = 10`. The `trainingplandetails` rows contain day numbers and activities without dates.

| Day number | Activity type | Title |
| --- | --- | --- |
| 1 | Feedback | Initial feedback |
| 2 | Classroom training | Classroom training day 2 |
| 3 | Classroom training | Classroom training day 3 |
| 4 | Classroom training | Classroom training day 4 |
| 5 | Classroom training | Classroom training day 5 |
| 6 | Classroom training | Classroom training day 6 |
| 7 | Classroom training | Classroom training day 7 |
| 8 | Classroom training | Classroom training day 8 |
| 9 | Classroom training | Classroom training day 9 |
| 10 | Feedback | Final feedback |

Batch A can use this plan from 2 through 11 November 2026, and Batch B can use the same plan from 1 through 10 December 2026. Both reference the same plan ID and ten detail rows. The plan stores neither date range.

With 30 trainees in each batch, each batch has 30 assignments and 300 attendance entries. Together they have 600 attendance entries, but still only one shared plan header and ten plan detail rows. Feedback submissions are also separate for each batch.

## Data relationships and integrity

- Project to batch and project to trainee: one to many under the proposed membership default.
- Project to school: many to many through project-school links.
- School to batch and school to trainee: one to many.
- `trainingplan` to batch: one to many. Each batch has exactly one required `training_plan_id`; the reference is not unique across batches.
- `trainingplan` to `trainingplandetails`: one to many, with unique `(training_plan_id, day_number)` and exactly N detail rows for N days.
- Batch to trainee: many to many through assignments.
- Assignment to attendance: one record per selected plan detail.
- Assignment to feedback submission: at most one final submission for Day 1 and one for Day N.

Validate that attendance and feedback reference a detail belonging to the plan selected by the assignment's batch. Enforce foreign keys and uniqueness in the database. Validate duration, consecutive day numbers, and activity sequence during plan publication and batch selection. Never delete a referenced plan, detail, or template version. Shared plan edits cannot alter historical batch records.

## Quality and operational requirements

- Support current desktop and mobile browsers, including attendance entry from a phone. Classroom training describes delivery mode; application access requires connectivity.
- Provide labelled fields, keyboard navigation, readable contrast, and field-level validation messages.
- Enforce server-side permissions for records, exports, and changes. Protect credentials, use encrypted transport, and restrict access to personal contact details.
- Audit plan publication and revisions, batch plan selection, roster changes, attendance corrections, feedback corrections, and lifecycle changes.
- Save related changes atomically; failed saves preserve existing data.
- Proposed performance target: 95th-percentile page and save responses within 3 seconds for batches of up to 100 trainees and 30 days with 50 concurrent users in the agreed test environment.
- Back up production data daily and verify restoration before launch. Confirm retention and deletion policy before production rollout.

## Acceptance criteria

| ID | Scenario | Expected result |
| --- | --- | --- |
| AC01 | Create and publish a ten-day plan | One `trainingplan` row and ten `trainingplandetails` rows numbered 1 through 10; no scheduled date fields in either table. |
| AC02 | Review its activities | Days 1 and 10 are Feedback; Days 2 through 9 are Classroom training. |
| AC03 | Create a two-day plan | Both days are Feedback; no intermediate classroom day. |
| AC04 | Publish a plan with missing or duplicate day numbers | Publication is rejected with the invalid details identified. |
| AC05 | Select a ten-day plan for a batch | Duration is 10, end date is start date plus 9 days, and the batch references the existing plan. |
| AC06 | Select the same plan for another batch | Save succeeds without duplicating the plan or its details. |
| AC07 | Use one plan in batches with different start dates | Each batch derives its own day dates; shared plan rows remain unchanged. |
| AC08 | Supply duration or dates inconsistent with the selected plan | Save is rejected with a clear validation message. |
| AC09 | Assign 30 trainees to a ten-day batch | Exactly 300 separate Not marked attendance entries are initialized. |
| AC10 | Record attendance in Batch A sharing a plan with Batch B | Batch B's attendance remains unchanged. |
| AC11 | Submit feedback on both boundary days | Two distinct submissions are stored against the trainee's batch assignment and the respective plan details. |
| AC12 | Revise a Published shared plan | A new Draft version is created; existing batches keep their original plan and history. |
| AC13 | Change only a Draft batch start date | Derived dates and end date change; plan details and other batches remain unchanged. |
| AC14 | Switch a Draft batch to another plan before records exist | Plan reference, duration, end date, and unmarked attendance are reconciled atomically. |
| AC15 | Replace a plan after attendance or feedback exists | Replacement is blocked. |
| AC16 | Repeat an assignment or attendance save | Duplicate assignment is rejected; attendance is updated without creating duplicates. |
| AC17 | Assign an ineligible trainee or schedule overlapping trainee batches | Action is rejected with the mismatch or conflict shown. |
| AC18 | Submit feedback without marking attendance | Feedback completion changes; attendance remains Not marked. |
| AC19 | Complete a batch with unmarked attendance | Completion is blocked and missing entries are identified. |
| AC20 | Access another trainee's records | Access is denied, including direct requests and exports. |
| AC21 | Archive a plan already used by batches | Existing batches retain access; new plan selection is blocked. |
| AC22 | Reference a detail from a different plan in batch attendance | Save is rejected. |
| AC23 | Mark future attendance or submit feedback on an unavailable day | Action is rejected; authorized feedback exceptions require a reason. |

## Delivery sequence

1. Build projects, schools, locations, trainee profiles, and role permissions.
2. Build `trainingplan`, `trainingplandetails`, feedback templates, and plan publication and versioning.
3. Build batch plan selection, date calculation, eligibility checks, trainee assignments, attendance, and feedback capture.
4. Add lifecycle controls, reports, audit history, operational checks, and user acceptance testing.

## Product decisions to confirm

- Should weekends and holidays be excluded from batch scheduling? If so, use a batch-specific training calendar while retaining numbered plan details.
- Are one-day plans required? If so, define how initial and final feedback share one day and one attendance entry.
- What questions should initial and final feedback contain, and should either form be anonymous?
- Can a batch include trainees from multiple schools?
- Can trainees belong to multiple projects or attend overlapping batches?
- Are late joining, withdrawal, partial-day attendance, or excused absence required?
- Will trainees log in or will staff enter all responses?
- What user volumes, retention period, and hosting environment are expected?
