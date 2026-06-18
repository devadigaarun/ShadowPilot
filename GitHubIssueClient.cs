using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ShadowPilot
{
    public class GitHubIssueClient
    {
        private readonly HttpClient httpClient;

        public GitHubIssueClient()
        {
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("ShadowPilot-VisualStudio-Extension");
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));

            var token = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
            if (!string.IsNullOrWhiteSpace(token))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<IReadOnlyList<GitHubIssueSummary>> GetOpenIssuesAsync(GitHubRepositoryInfo repository)
        {
            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            var requestUrl = $"https://api.github.com/repos/{repository.ApiRepositoryPath}/issues?state=open&per_page=100";
            var response = await httpClient.GetAsync(requestUrl).ConfigureAwait(false);
            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"GitHub issue retrieval failed: {(int)response.StatusCode} {response.ReasonPhrase}. {content}");
            }

            var items = JArray.Parse(content);
            return items
                .Where(item => item["pull_request"] == null)
                .Select(ParseIssue)
                .OrderByDescending(issue => issue.UpdatedAt)
                .ToList();
        }

        private static GitHubIssueSummary ParseIssue(JToken item)
        {
            var labels = item["labels"] is JArray labelArray
                ? labelArray.Select(label => (string)label["name"]).Where(name => !string.IsNullOrWhiteSpace(name)).ToList()
                : new List<string>();

            DateTimeOffset updatedAt;
            DateTimeOffset? parsedUpdatedAt = DateTimeOffset.TryParse((string)item["updated_at"], out updatedAt) ? updatedAt : (DateTimeOffset?)null;

            return new GitHubIssueSummary
            {
                Number = (int)item["number"],
                Title = (string)item["title"],
                Body = (string)item["body"],
                State = (string)item["state"],
                HtmlUrl = (string)item["html_url"],
                UpdatedAt = parsedUpdatedAt,
                Labels = labels
            };
        }
    }
}