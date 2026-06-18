# Orchestration Workflow

## Objective

Coordinate specialized agents so a concept document or GitHub issue becomes a traceable, designed, implemented, tested, reviewed, and automatically checked-in delivery increment.

## Fixed Repository

All repository operations target this GitHub repository:

```text
https://github.com/devadigaarun/ShadowPilot.git
```

## Workflow Contract

Every agent must receive:

- Workflow run ID.
- Source concept document, GitHub issue details, or prior agent output.
- Target work item ID or GitHub issue ID when applicable.
- Repository branch or workspace context for `https://github.com/devadigaarun/ShadowPilot.git` when applicable.
- Current approval state.
- Known assumptions, decisions, risks, and open questions.

Every agent must produce:

- Structured output using [output templates](output-templates.md).
- Traceability back to source requirements, TFS IDs, and GitHub issue IDs when applicable.
- Clear handoff notes for the next agent.
- A decision log entry for any material decision.

## Routing Rules

### Start

Input: concept document or GitHub issue ID.

Orchestrator Agent asks the user to provide one of:

- Concept document.
- GitHub issue ID from `https://github.com/devadigaarun/ShadowPilot/issues`.

For concept document intake, route to Requirement Elicitation Agent.

For GitHub issue ID intake:

- Fetch issue title, body, labels, state, assignees, comments, and linked references from `https://github.com/devadigaarun/ShadowPilot/issues`.
- Treat the GitHub issue as the approved source item.
- Derive acceptance criteria and open questions from the issue details.
- Skip Requirement Elicitation Agent, Requirement Slicing Agent, and TFS Work Item Agent unless the issue explicitly asks for new TFS work items.
- Route to Architecture and Design Agent when architecture/design is needed. If design is not needed, record why and route to Developer Agent.

### Requirement Elicitation Complete

Proceed only when:

- Ambiguities are resolved or explicitly marked as accepted assumptions.
- Acceptance criteria are testable.
- Scope boundaries are documented.

Route to Requirement Slicing Agent.

### Requirement Slicing Complete

Proceed only when:

- Slices are independently valuable and testable.
- Dependencies are identified.
- Suggested TFS hierarchy is available.

Route to TFS Work Item Agent.

### Before TFS Creation

The TFS Work Item Agent must ask:

> What is the parent TFS work item number under which these work items should be created?

No TFS creation may occur until a human supplies the parent ID and approves the proposed work item tree.

### TFS Work Items Created

For each approved work item, route to Architecture and Design Agent.

### Architecture and Design Complete

Proceed only when:

- Use-case diagram is prepared when user/system interactions exist.
- Domain diagram is prepared when business concepts or relationships exist.
- Architecture big picture is prepared for system/component changes.
- Critical sequence diagrams are prepared for important runtime interactions.
- Important class diagrams are prepared for object model changes.
- Design decisions and trade-offs are recorded.

Route to Developer Agent.

### Implementation Complete

Proceed only when:

- Product code is implemented.
- Acceptance criteria traceability is updated.
- Build succeeds or failure is explicitly escalated.

Route to BDD Test Agent.

### Testing Complete

Proceed only when:

- Gherkin scenarios cover primary, alternate, boundary, and failure cases.
- Unit tests are implemented and runnable.
- Test results are captured.

Route to Review Agent.

### Review Complete

Route review comments to a human reviewer.

Human reviewer must mark each comment as:

- Accepted.
- Rejected.
- Deferred.
- Needs clarification.

Accepted comments route back to Developer Agent for implementation. Rejected comments are logged with rationale. Deferred comments require a target work item or backlog note.

### Review Comments Implemented

Run affected tests and route back to Review Agent if changes are material. Otherwise route to Pipeline Agent.

### Pipeline Ready

Pipeline Agent prepares check-in evidence, generates a meaningful check-in comment, commits the approved change set, and pushes it to `https://github.com/devadigaarun/ShadowPilot.git`.

For GitHub issue ID intake, Pipeline Agent then posts a closing issue comment containing implementation summary, commit SHA, build results, test results, review status, known coverage gaps, and risk notes, then closes the GitHub issue automatically.

## Failure Handling

- If required input is missing, stop and ask for the missing information.
- If an agent detects unsafe, destructive, or irreversible operations, escalate to human approval.
- If outputs conflict, route to Orchestrator Agent for reconciliation.
- If tests fail, route to Developer Agent with failure evidence.
- If the Git remote does not match `https://github.com/devadigaarun/ShadowPilot.git`, stop and request repository correction.
- If Git credentials are required, stop and ask the user to authenticate through Git or the terminal without exposing secrets.
- If GitHub issue details cannot be fetched or the issue is already closed, stop and request input.
- If GitHub issue update or close permissions are missing, stop and ask the user to authenticate through GitHub CLI, Git credential manager, browser, or device login without exposing secrets.
- If requirements change after implementation starts, route to Requirement Slicing Agent for impact analysis.

## Traceability Keys

Use these IDs consistently across all artifacts:

- `workflow_run_id`
- `concept_document_id`
- `requirement_id`
- `slice_id`
- `tfs_work_item_id`
- `github_issue_id`
- `design_decision_id`
- `test_case_id`
- `review_comment_id`
- `check_in_candidate_id`
- `github_issue_close_id`
