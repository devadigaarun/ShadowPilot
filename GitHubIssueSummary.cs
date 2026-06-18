using System;
using System.Collections.Generic;

namespace ShadowPilot
{
    public class GitHubIssueSummary
    {
        public int Number { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public string State { get; set; }

        public string HtmlUrl { get; set; }

        public DateTimeOffset? UpdatedAt { get; set; }

        public List<string> Labels { get; set; }

        public GitHubIssueSummary()
        {
            Labels = new List<string>();
        }

        public override string ToString()
        {
            return $"#{Number} {Title}";
        }
    }
}