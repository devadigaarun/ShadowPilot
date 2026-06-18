@GH-2
Feature: TFS work item listing
  In order to see my assigned TFS work from Visual Studio
  As a Visual Studio user
  I want ShadowPilot to list TFS work items assigned to me under the Shadow Pilot menu

  Background:
    Given ShadowPilot is installed in Visual Studio
    And TFS work item settings are configured

  @AC-001 @AC-003
  Scenario: Open TFS work items from the Shadow Pilot menu
    Given the TFS collection URL and project are configured
    And the assigned-to identity is configured
    When the user opens Tools > Shadow Pilot > TFS Work Items
    Then ShadowPilot queries TFS for assigned work items
    And ShadowPilot displays the assigned work items in Visual Studio

  @AC-002
  Scenario: Query work items assigned to the configured user
    Given the assigned-to identity is configured as the current user
    When ShadowPilot builds the work item query
    Then the query filters by the configured project
    And the query filters by the configured assigned-to identity
    And the query excludes closed work items

  @AC-004
  Scenario: Missing TFS configuration blocks retrieval
    Given the TFS collection URL is missing
    When the user opens Tools > Shadow Pilot > TFS Work Items
    Then ShadowPilot does not call TFS
    And ShadowPilot shows a configuration message

  @AC-004
  Scenario: Missing TFS token blocks retrieval
    Given the TFS PAT environment variable is not set
    When the user opens Tools > Shadow Pilot > TFS Work Items
    Then ShadowPilot does not call TFS
    And ShadowPilot shows an authentication configuration message

  @AC-003
  Scenario: Empty assigned work item result
    Given TFS returns no active work items assigned to the configured user
    When ShadowPilot displays the result
    Then the work item dialog shows that no assigned TFS work items were found