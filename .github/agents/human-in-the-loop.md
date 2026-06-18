# Human-in-the-Loop Policy

Human oversight is mandatory at key decision points to keep the delivery workflow safe, auditable, and aligned with business intent.

## Required Human Roles

- Product Owner or Business Reviewer: validates requirements, slices, and acceptance criteria.
- Technical Reviewer or Architect: validates architecture and design outputs.
- Code Reviewer: accepts, rejects, or defers review comments.
- TFS Operator or Delivery Lead: approves TFS work item creation and hierarchy.

## Approval Gates

| Gate | Trigger | Required Human Decision | Agent Allowed After Approval |
| --- | --- | --- | --- |
| Requirements Validation | Elicitation complete | Approve requirements or request clarification | Slice requirements |
| TFS Parent Confirmation | Before work item creation | Provide parent work item ID | Prepare dry-run work item tree |
| TFS Creation Approval | Dry-run tree complete | Approve, edit, or reject proposed work items | Create approved work items |
| Design Approval | Architecture/design complete | Approve design or request changes | Start development |
| Review Decision | Review comments complete | Accept, reject, defer, or request clarification per comment | Implement accepted comments only |
| Automated Check-in Safety | Pipeline readiness complete but repository or credential state is unsafe | Correct repository, authenticate Git, or stop automation | Pipeline Agent resumes automated check-in only after the unsafe state is resolved |
| GitHub Issue Safety | Issue details are missing, ambiguous, already closed, or cannot be updated | Clarify scope, reopen/replace issue, authenticate GitHub, or stop automation | Orchestrator or Pipeline Agent resumes only after the unsafe state is resolved |

## Review Comment Decision Model

Each review comment must be assigned one status:

- Accepted: Developer Agent must implement it.
- Rejected: No implementation; rejection rationale is recorded.
- Deferred: Create or link a future work item.
- Needs clarification: Route back to Review Agent or relevant author.

## Approval Record Template

```yaml
approval_id: APPROVAL-0001
workflow_run_id: RUN-0001
gate: TFS Creation Approval
requested_by_agent: tfs-work-item-agent
approver_name: ""
approver_role: ""
decision: approved | rejected | changes_requested
approved_items:
  - item_id_or_temp_id: ""
notes: ""
timestamp_utc: ""
```

## Escalation Rules

Escalate immediately when:

- Requirements conflict with legal, privacy, security, or compliance constraints.
- Agents disagree on scope, architecture, tests, or review severity.
- TFS or repository operations may affect unrelated work.
- GitHub issue closure would affect the wrong issue or cannot include required test evidence.
- The requested action requires credentials or elevated permissions.
- A pipeline failure indicates a systemic or environment issue.

## Audit Requirements

Keep these records for each workflow run:

- Questions asked and answers received.
- Assumptions accepted by humans.
- Approval decisions and approver identity.
- TFS item creation plan and resulting IDs.
- GitHub issue source details and close result when GitHub issue intake is used.
- Review comments and final decisions.
- Test commands and results.
- Check-in comment prepared by Pipeline Agent.
- Automated check-in result, including commit hash and push status.
