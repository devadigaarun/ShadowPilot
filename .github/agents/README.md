# Multi-Agent Delivery System

This folder defines reusable instructions, guardrails, templates, and test strategy for a multi-agent delivery workflow that turns a concept document or GitHub issue into reviewed, tested, automatically checked-in work.

## Fixed Repository

All repository operations target:

```text
https://github.com/devadigaarun/ShadowPilot.git
```

## Primary Input

- Concept document describing product intent, target users, constraints, business rules, and expected outcomes.
- GitHub issue ID from `https://github.com/devadigaarun/ShadowPilot/issues`.

## Delivery Flow

At workflow start, Orchestrator Agent asks for either a concept document or a GitHub issue ID.

Concept document path:

1. Requirement Elicitation Agent clarifies goals, gaps, assumptions, risks, constraints, and acceptance criteria.
2. Requirement Slicing Agent decomposes validated requirements into independently deliverable slices.
3. TFS Work Item Agent asks for the parent work item number, proposes the work item tree, and creates approved work items.
4. Architecture and Design Agent prepares design outputs for a selected work item.
5. Developer Agent implements the selected work item.
6. BDD Test Agent creates Gherkin scenarios and unit tests for the implemented behavior.
7. Review Agent reviews product code and test code, then prepares review comments.
8. Human Reviewer accepts or rejects review comments.
9. Developer Agent implements accepted review comments.
10. Pipeline Agent prepares check-in readiness evidence, writes meaningful check-in comments, commits the approved change set, and pushes it to the fixed GitHub repository.

GitHub issue ID path:

1. Orchestrator Agent fetches issue details from `https://github.com/devadigaarun/ShadowPilot/issues`.
2. Architecture and Design Agent designs the issue when architecture/design is needed. If design is not needed, Orchestrator records the rationale and routes directly to Developer Agent.
3. Developer Agent implements the issue scope.
4. BDD Test Agent creates or updates Gherkin scenarios and unit tests for the issue behavior.
5. Review Agent reviews product code, test code, and issue coverage.
6. Developer Agent implements accepted review comments.
7. Pipeline Agent commits, pushes, comments on the GitHub issue with test evidence, and closes the issue automatically.

## TFS Work Item Hierarchy

Use only the following work item types unless a human explicitly overrides the process:

```text
Epic
└── Functionality
    └── Improvement
        └── Work Package
            └── User Story
                └── Task
```

## Agent Instruction Files

- [Orchestrator Agent](instructions/00-orchestrator-agent.md)
- [Requirement Elicitation Agent](instructions/01-requirement-elicitation-agent.md)
- [Requirement Slicing Agent](instructions/02-requirement-slicing-agent.md)
- [TFS Work Item Agent](instructions/03-tfs-work-item-agent.md)
- [Architecture and Design Agent](instructions/04-architecture-design-agent.md)
- [Developer Agent](instructions/05-developer-agent.md)
- [BDD Test Agent](instructions/06-bdd-test-agent.md)
- [Review Agent](instructions/07-review-agent.md)
- [Pipeline Agent](instructions/08-pipeline-agent.md)

## Shared Governance

- [Guardrails](guardrails.md)
- [Human-in-the-loop Policy](human-in-the-loop.md)
- [Output Templates](output-templates.md)
- [Test Strategy](test-strategy.md)

## Required State Artifacts

Each workflow run should persist these artifacts in the work tracking system or a run folder:

- Original concept document reference.
- Elicitation questions and answers.
- Validated requirement specification.
- Requirement slicing matrix.
- Parent work item ID and created TFS IDs.
- GitHub issue ID, title, URL, labels, comments used as source evidence, and close result when GitHub issue intake is used.
- Architecture/design package.
- Implementation summary.
- BDD feature files and unit test results.
- Review comments with accepted/rejected decisions.
- Check-in readiness report, automated check-in comment, commit hash, and push result.

## Approval Gates and Safety Stops

Human approval is mandatory before:

- Creating or modifying TFS work items.
- Starting implementation for a work item.
- Applying review comments.
- Running destructive operations.

Automation must stop for human action when:

- Repository credentials are missing.
- The configured remote does not match the fixed GitHub repository.
- GitHub issue details cannot be fetched or updated.
