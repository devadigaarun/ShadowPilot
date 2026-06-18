using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ShadowPilot
{
    public class TfsWorkItemClient
    {
        private readonly HttpClient httpClient;

        public TfsWorkItemClient()
        {
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<IReadOnlyList<TfsWorkItemSummary>> GetAssignedWorkItemsAsync(TfsWorkItemSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (!settings.IsConfigured(out var validationMessage))
            {
                throw new InvalidOperationException(validationMessage);
            }

            ConfigureAuthentication(settings);

            var wiql = TfsWorkItemQueryBuilder.BuildAssignedToQuery(settings.Project, settings.AssignedTo);
            var wiqlBody = JsonConvert.SerializeObject(new { query = wiql });
            var wiqlResponse = await httpClient.PostAsync(
                TfsWorkItemQueryBuilder.BuildWiqlEndpoint(settings),
                new StringContent(wiqlBody, Encoding.UTF8, "application/json")).ConfigureAwait(false);

            var wiqlContent = await wiqlResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (!wiqlResponse.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"TFS work item query failed: {(int)wiqlResponse.StatusCode} {wiqlResponse.ReasonPhrase}. {wiqlContent}");
            }

            var workItemIds = ExtractWorkItemIds(wiqlContent).Take(100).ToList();
            if (workItemIds.Count == 0)
            {
                return new List<TfsWorkItemSummary>();
            }

            var detailsResponse = await httpClient.GetAsync(
                TfsWorkItemQueryBuilder.BuildWorkItemsEndpoint(settings, string.Join(",", workItemIds))).ConfigureAwait(false);

            var detailsContent = await detailsResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (!detailsResponse.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"TFS work item details retrieval failed: {(int)detailsResponse.StatusCode} {detailsResponse.ReasonPhrase}. {detailsContent}");
            }

            return ParseWorkItems(detailsContent);
        }

        private void ConfigureAuthentication(TfsWorkItemSettings settings)
        {
            var environmentVariable = string.IsNullOrWhiteSpace(settings.PatEnvironmentVariable) ? "TFS_PAT" : settings.PatEnvironmentVariable;
            var pat = Environment.GetEnvironmentVariable(environmentVariable);

            if (string.IsNullOrWhiteSpace(pat))
            {
                throw new InvalidOperationException($"TFS PAT environment variable '{environmentVariable}' is not set. Set it before loading TFS work items.");
            }

            var token = Convert.ToBase64String(Encoding.ASCII.GetBytes($":{pat}"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", token);
        }

        private static IEnumerable<int> ExtractWorkItemIds(string wiqlContent)
        {
            var json = JObject.Parse(wiqlContent);
            var workItems = json["workItems"] as JArray;

            if (workItems == null)
            {
                yield break;
            }

            foreach (var workItem in workItems)
            {
                var id = (int?)workItem["id"];
                if (id.HasValue)
                {
                    yield return id.Value;
                }
            }
        }

        private static IReadOnlyList<TfsWorkItemSummary> ParseWorkItems(string content)
        {
            var json = JObject.Parse(content);
            var items = json["value"] as JArray;
            var results = new List<TfsWorkItemSummary>();

            if (items == null)
            {
                return results;
            }

            foreach (var item in items)
            {
                var fields = item["fields"];
                results.Add(new TfsWorkItemSummary
                {
                    Id = (int)item["id"],
                    Title = (string)fields?["System.Title"],
                    State = (string)fields?["System.State"],
                    WorkItemType = (string)fields?["System.WorkItemType"],
                    AssignedTo = ReadIdentity(fields?["System.AssignedTo"]),
                    Url = (string)item["url"]
                });
            }

            return results;
        }

        private static string ReadIdentity(JToken token)
        {
            if (token == null)
            {
                return string.Empty;
            }

            if (token.Type == JTokenType.Object)
            {
                return (string)token["displayName"] ?? (string)token["uniqueName"] ?? token.ToString();
            }

            return token.ToString();
        }
    }
}