using System;
using System.Linq;
using System.Text;

namespace ShadowPilot
{
    public class IssueWorkflowPromptBuilder
    {
        public string BuildPrompt(GitHubRepositoryInfo repository, GitHubIssueSummary issue)
        {
            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            if (issue == null)
            {
                throw new ArgumentNullException(nameof(issue));
            }

            var labels = issue.Labels != null && issue.Labels.Count > 0 ? string.Join(", ", issue.Labels) : "none";
            var body = string.IsNullOrWhiteSpace(issue.Body) ? "No issue body was provided." : issue.Body.Trim();

            var builder = new StringBuilder();
            builder.AppendLine("run Orchestrator Agent");
            builder.AppendLine();
            builder.AppendLine("Use GitHub issue intake with the following approved source item.");
            builder.AppendLine();
            builder.AppendLine($"Repository: {repository.WebUrl}");
            builder.AppendLine($"Repository remote: {repository.RemoteUrl}");
            builder.AppendLine($"Issue ID: {issue.Number}");
            builder.AppendLine($"Issue URL: {issue.HtmlUrl}");
            builder.AppendLine($"Issue state: {issue.State}");
            builder.AppendLine($"Issue labels: {labels}");
            builder.AppendLine($"Issue title: {issue.Title}");
            builder.AppendLine();
            builder.AppendLine("Issue body:");
            builder.AppendLine(body);
            builder.AppendLine();
            builder.AppendLine("Derived acceptance criteria:");
            builder.AppendLine("- List GitHub issues that are not closed from the currently connected GitHub repository in Visual Studio.");
            builder.AppendLine("- Start the autonomous implementation workflow when the user clicks an issue.");
            builder.AppendLine("- Implement, test, review, and check in the selected issue through the configured agent workflow.");
            builder.AppendLine("- After a successful push, close the implemented GitHub issue with test evidence in the closing comment.");
            builder.AppendLine();
            builder.AppendLine("Continue through the required workflow phases. Stop only for required human approval gates, missing credentials, failed tests, or repository remote mismatch.");

            return builder.ToString();
        }
    }
}