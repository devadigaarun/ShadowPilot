using System;
using System.Collections.Generic;

namespace ShadowPilot
{
    public class TfsWorkItemSettings
    {
        public string CollectionUrl { get; set; }

        public string Project { get; set; }

        public string AssignedTo { get; set; }

        public string PatEnvironmentVariable { get; set; }

        public string NormalizedCollectionUrl => string.IsNullOrWhiteSpace(CollectionUrl) ? CollectionUrl : CollectionUrl.Trim().TrimEnd('/');

        public bool IsConfigured(out string message)
        {
            var missingValues = new List<string>();

            if (string.IsNullOrWhiteSpace(CollectionUrl))
            {
                missingValues.Add("TFS collection URL");
            }

            if (string.IsNullOrWhiteSpace(Project))
            {
                missingValues.Add("TFS project");
            }

            if (string.IsNullOrWhiteSpace(AssignedTo))
            {
                missingValues.Add("TFS assigned-to identity");
            }

            if (missingValues.Count > 0)
            {
                message = "Configure " + string.Join(", ", missingValues) + " in Tools > Options > Shadow Pilot > General.";
                return false;
            }

            if (!Uri.TryCreate(NormalizedCollectionUrl, UriKind.Absolute, out var uri) || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                message = "TFS collection URL must be an absolute HTTP or HTTPS URL.";
                return false;
            }

            message = string.Empty;
            return true;
        }
    }
}