@GH-1
Feature: GitHub issue workflow launcher
  In order to start approved issue work from Visual Studio
  As a Visual Studio user
  I want ShadowPilot to list open GitHub issues and launch the autonomous workflow for a selected issue

  Background:
    Given a solution is open in Visual Studio
    And the solution repository origin points to a GitHub repository

  @AC-001
  Scenario: List non-closed GitHub issues from the connected repository
    Given the connected GitHub repository has open issues
    When the user opens the ShadowPilot GitHub Issues command
    Then the issue picker shows the open GitHub issues for that repository

  @AC-002
  Scenario: Start the workflow for a selected issue
    Given the issue picker shows issue 1
    When the user selects issue 1 and starts the workflow
    Then ShadowPilot builds an orchestrator prompt using issue 1 as the approved source item
    And ShadowPilot sends the prompt to GitHub Copilot Chat when possible

  @AC-003
  Scenario: Include workflow acceptance criteria in the launched prompt
    Given issue 1 is selected
    When ShadowPilot builds the workflow prompt
    Then the prompt includes implementation, testing, review, check-in, and issue closure expectations

  @AC-001
  Scenario: Reject unsupported connected repository remotes
    Given the solution repository origin is not a GitHub remote
    When the user opens the ShadowPilot GitHub Issues command
    Then ShadowPilot does not request issues from GitHub
    And ShadowPilot shows a repository validation message

  @AC-001
  Scenario: Handle an empty issue list
    Given the connected GitHub repository has no open issues
    When the user opens the ShadowPilot GitHub Issues command
    Then the issue picker shows that no open issues were found
    And the workflow cannot be started without a selected issue