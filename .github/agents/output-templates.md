# Shared Output Templates

Use these templates to keep agent outputs consistent, traceable, and automation-friendly.

## Concept Intake Summary

```yaml
workflow_run_id: RUN-0001
concept_document:
  title: ""
  source: ""
  version: ""
  received_at_utc: ""
stakeholders:
  - name: ""
    role: ""
business_goal: ""
problem_statement: ""
target_users:
  - ""
in_scope:
  - ""
out_of_scope:
  - ""
constraints:
  - ""
assumptions:
  - id: ASSUMPTION-001
    text: ""
    status: pending | approved | rejected
risks:
  - id: RISK-001
    description: ""
    impact: low | medium | high
    mitigation: ""
open_questions:
  - id: QUESTION-001
    question: ""
    blocking: true
```

## GitHub Issue Intake Summary

```yaml
github_issue_intake_id: GH-INTAKE-0001
workflow_run_id: RUN-0001
repository_url: https://github.com/devadigaarun/ShadowPilot.git
issue:
  id: 123
  url: https://github.com/devadigaarun/ShadowPilot/issues/123
  title: ""
  state: open | closed
  labels:
    - ""
  assignees:
    - ""
  body_summary: ""
  source_comments_used:
    - comment_url: ""
      summary: ""
derived_acceptance_criteria:
  - id: AC-001
    given: ""
    when: ""
    then: ""
open_questions:
  - id: QUESTION-001
    question: ""
    blocking: true
```

## Requirement Elicitation Output

```yaml
requirement_specification_id: REQ-SPEC-0001
workflow_run_id: RUN-0001
functional_requirements:
  - id: REQ-001
    title: ""
    description: ""
    source_reference: ""
    priority: must | should | could | wont
    acceptance_criteria:
      - id: AC-001
        given: ""
        when: ""
        then: ""
non_functional_requirements:
  - id: NFR-001
    category: security | performance | reliability | usability | observability | maintainability | compliance
    description: ""
    measurable_target: ""
data_requirements:
  - id: DATA-001
    entity: ""
    fields:
      - ""
external_dependencies:
  - id: DEP-001
    name: ""
    type: system | service | team | vendor
    notes: ""
clarification_log:
  - question_id: QUESTION-001
    answer: ""
    answered_by: ""
    timestamp_utc: ""
```

## Requirement Slicing Output

```yaml
slicing_result_id: SLICE-PLAN-0001
workflow_run_id: RUN-0001
slices:
  - slice_id: SLICE-001
    title: ""
    business_value: ""
    included_requirements:
      - REQ-001
    acceptance_criteria:
      - AC-001
    dependencies:
      - ""
    suggested_tfs_type: User Story
    suggested_parent_type: Work Package
    priority: 1
    estimated_size: XS | S | M | L | XL
    notes: ""
```

## TFS Work Item Dry-Run Plan

```yaml
tfs_plan_id: TFS-PLAN-0001
workflow_run_id: RUN-0001
parent_work_item_id: "REQUIRED_BEFORE_CREATE"
parent_validation:
  validated: false
  title: ""
  type: ""
proposed_work_items:
  - temp_id: TEMP-EPIC-001
    work_item_type: Epic
    title: ""
    description: ""
    parent: "PARENT_WORK_ITEM_ID"
    acceptance_criteria: []
    tags:
      - multi-agent-generated
    links:
      requirements:
        - REQ-001
  - temp_id: TEMP-FUNC-001
    work_item_type: Functionality
    title: ""
    description: ""
    parent: TEMP-EPIC-001
  - temp_id: TEMP-IMP-001
    work_item_type: Improvement
    title: ""
    description: ""
    parent: TEMP-FUNC-001
  - temp_id: TEMP-WP-001
    work_item_type: Work Package
    title: ""
    description: ""
    parent: TEMP-IMP-001
  - temp_id: TEMP-US-001
    work_item_type: User Story
    title: "As a <user>, I want <capability>, so that <benefit>"
    description: ""
    parent: TEMP-WP-001
    acceptance_criteria:
      - "Given <context> when <event> then <outcome>"
  - temp_id: TEMP-TASK-001
    work_item_type: Task
    title: ""
    description: ""
    parent: TEMP-US-001
    implementation_notes:
      - ""
approval_required: true
```

## TFS Creation Result

```yaml
tfs_creation_result_id: TFS-CREATE-0001
workflow_run_id: RUN-0001
created_by: tfs-work-item-agent
created_at_utc: ""
items:
  - temp_id: TEMP-US-001
    tfs_work_item_id: 12345
    work_item_type: User Story
    title: ""
    url: ""
    parent_tfs_work_item_id: 12344
failures:
  - temp_id: ""
    reason: ""
```

## Architecture and Design Output

```yaml
design_package_id: DESIGN-0001
workflow_run_id: RUN-0001
tfs_work_item_id: 12345
github_issue_id: 123
summary: ""
context:
  current_state: ""
  target_state: ""
  impacted_components:
    - ""
diagrams:
  use_case_diagram:
    format: mermaid
    content: ""
  domain_diagram:
    format: mermaid
    content: ""
  architecture_big_picture:
    format: mermaid
    content: ""
  critical_sequence_diagrams:
    - title: ""
      format: mermaid
      content: ""
  class_diagrams:
    - title: ""
      format: mermaid
      content: ""
design_decisions:
  - id: ADR-001
    decision: ""
    rationale: ""
    alternatives_considered:
      - ""
    consequences:
      - ""
security_considerations:
  - ""
observability_considerations:
  - ""
open_questions:
  - ""
```

## Implementation Summary

```yaml
implementation_summary_id: IMPL-0001
workflow_run_id: RUN-0001
tfs_work_item_id: 12345
github_issue_id: 123
implemented_by: developer-agent
files_changed:
  - path: ""
    change_type: added | modified | deleted
    summary: ""
requirements_covered:
  - REQ-001
acceptance_criteria_covered:
  - AC-001
build:
  command: ""
  result: passed | failed | not_run
  notes: ""
known_limitations:
  - ""
handoff_notes: ""
```

## BDD Feature Template

```gherkin
Feature: <feature name>
  In order to <business outcome>
  As a <user or system actor>
  I want <capability>

  Background:
    Given <shared context>

  Scenario: <primary behavior>
    Given <initial context>
    When <action or event>
    Then <expected outcome>

  Scenario: <validation or failure behavior>
    Given <invalid or boundary context>
    When <action or event>
    Then <expected validation or failure outcome>
```

## Test Summary

```yaml
test_summary_id: TEST-0001
workflow_run_id: RUN-0001
tfs_work_item_id: 12345
github_issue_id: 123
feature_files:
  - path: ""
    scenarios:
      - name: ""
        maps_to_acceptance_criteria:
          - AC-001
unit_tests:
  - test_name: ""
    file_path: ""
    maps_to_scenario: ""
test_execution:
  command: ""
  result: passed | failed | not_run
  failures:
    - test_name: ""
      message: ""
coverage_notes: ""
```

## Review Comment Template

```yaml
review_id: REVIEW-0001
workflow_run_id: RUN-0001
tfs_work_item_id: 12345
github_issue_id: 123
reviewed_artifacts:
  - path: ""
comments:
  - review_comment_id: RC-001
    severity: blocking | major | minor | suggestion
    category: correctness | security | performance | maintainability | testability | style | requirements | testing
    location:
      file: ""
      line: ""
    requirement_reference: REQ-001
    comment: ""
    recommended_change: ""
    status: pending_human_decision
```

## Review Decision Template

```yaml
review_decision_batch_id: REVIEW-DECISION-0001
workflow_run_id: RUN-0001
decisions:
  - review_comment_id: RC-001
    decision: accepted | rejected | deferred | needs_clarification
    rationale: ""
    decided_by: ""
    timestamp_utc: ""
    follow_up_tfs_work_item_id: ""
```

## Pipeline Readiness Report

```yaml
pipeline_readiness_id: PIPELINE-0001
workflow_run_id: RUN-0001
repository_url: https://github.com/devadigaarun/ShadowPilot.git
tfs_work_item_ids:
  - 12345
github_issue_ids:
  - 123
branch: ""
target_branch: ""
build_status:
  command: ""
  result: passed | failed | not_run
test_status:
  command: ""
  result: passed | failed | not_run
review_status:
  pending_blocking_comments: 0
  accepted_comments_implemented: true
risk_summary: ""
check_in_ready: false
blocking_items:
  - ""
```

## Check-in Comment Template

```text
TFS: #<work-item-id>, #<work-item-id>
GitHub Issue: #<issue-id>
Summary: <short description of implemented change>
Requirements: <REQ IDs or acceptance criteria covered>
Design: <design package or ADR references>
Tests: <commands run and results>
Review: <review ID; accepted comments implemented; rejected/deferred notes if any; concise meaningful review notes>
Review Comments:
- <meaningful review comment or verification note tied to the change>
- <meaningful review comment or verification note tied to tests, risk, or maintainability>
Risk: <low/medium/high and mitigation>
Notes: <migration, config, deployment, or operational notes>
```

## Automated Check-in Result

```yaml
automated_check_in_result_id: CHECKIN-0001
workflow_run_id: RUN-0001
repository_url: https://github.com/devadigaarun/ShadowPilot.git
github_issue_ids:
  - 123
branch: ""
target_branch: ""
commit_sha: ""
commit_message: ""
push_result: pushed | failed | not_run
review_comment_summary:
  - ""
blocking_items:
  - ""
```

## GitHub Issue Close Result

```yaml
github_issue_close_id: GH-CLOSE-0001
workflow_run_id: RUN-0001
repository_url: https://github.com/devadigaarun/ShadowPilot.git
issue_id: 123
issue_url: https://github.com/devadigaarun/ShadowPilot/issues/123
closing_comment_url: ""
closing_comment_summary:
  implementation: ""
  commit_sha: ""
  build: ""
  test_command: ""
  test_result: ""
  bdd_features: ""
  unit_tests: ""
  review: ""
  coverage_gaps: ""
  risk: ""
close_result: closed | failed | not_run
blocking_items:
  - ""
```
