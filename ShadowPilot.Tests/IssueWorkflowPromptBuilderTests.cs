using ShadowPilot;
using Xunit;

namespace ShadowPilot.Tests;

public class IssueWorkflowPromptBuilderTests
{
    [Fact]
    public void BuildPrompt_WhenIssueIsSelected_IncludesTraceableIssueContext()
    {
        var repository = new GitHubRepositoryInfo
        {
            Owner = "devadigaarun",
            Name = "ShadowPilot",
            RemoteUrl = "https://github.com/devadigaarun/ShadowPilot.git"
        };
        var issue = new GitHubIssueSummary
        {
            Number = 1,
            Title = "Add UI to list open GitHub issues",
            Body = "List non-closed issues and start autonomous workflow on click.",
            State = "open",
            HtmlUrl = "https://github.com/devadigaarun/ShadowPilot/issues/1"
        };
        issue.Labels.Add("ready");

        var prompt = new IssueWorkflowPromptBuilder().BuildPrompt(repository, issue);

        Assert.Contains("run Orchestrator Agent", prompt);
        Assert.Contains("Repository: https://github.com/devadigaarun/ShadowPilot", prompt);
        Assert.Contains("Issue ID: 1", prompt);
        Assert.Contains("Issue URL: https://github.com/devadigaarun/ShadowPilot/issues/1", prompt);
        Assert.Contains("Issue labels: ready", prompt);
        Assert.Contains("Issue title: Add UI to list open GitHub issues", prompt);
        Assert.Contains("List non-closed issues and start autonomous workflow on click.", prompt);
    }

    [Fact]
    public void BuildPrompt_WhenIssueIsSelected_IncludesWorkflowAcceptanceCriteria()
    {
        var repository = new GitHubRepositoryInfo
        {
            Owner = "devadigaarun",
            Name = "ShadowPilot",
            RemoteUrl = "https://github.com/devadigaarun/ShadowPilot.git"
        };
        var issue = new GitHubIssueSummary
        {
            Number = 1,
            Title = "Autonomous issue workflow",
            State = "open",
            HtmlUrl = "https://github.com/devadigaarun/ShadowPilot/issues/1"
        };

        var prompt = new IssueWorkflowPromptBuilder().BuildPrompt(repository, issue);

        Assert.Contains("List GitHub issues that are not closed", prompt);
        Assert.Contains("Start the autonomous implementation workflow", prompt);
        Assert.Contains("Implement, test, review, and check in", prompt);
        Assert.Contains("close the implemented GitHub issue with test evidence", prompt);
    }
}