using System;

namespace ShadowPilot
{
    public static class TfsWorkItemQueryBuilder
    {
        public static string BuildAssignedToQuery(string project, string assignedTo)
        {
            if (string.IsNullOrWhiteSpace(project))
            {
                throw new ArgumentException("Project is required.", nameof(project));
            }

            if (string.IsNullOrWhiteSpace(assignedTo))
            {
                throw new ArgumentException("Assigned-to identity is required.", nameof(assignedTo));
            }

            return "SELECT [System.Id], [System.Title], [System.State], [System.WorkItemType], [System.AssignedTo] " +
                   "FROM WorkItems " +
                   $"WHERE [System.TeamProject] = '{EscapeWiqlValue(project)}' " +
                   $"AND [System.AssignedTo] = '{EscapeWiqlValue(assignedTo)}' " +
                   "AND [System.State] <> 'Closed' " +
                   "ORDER BY [System.ChangedDate] DESC";
        }

        public static string BuildWiqlEndpoint(TfsWorkItemSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            return $"{settings.NormalizedCollectionUrl}/{Uri.EscapeDataString(settings.Project)}/_apis/wit/wiql?api-version=6.0";
        }

        public static string BuildWorkItemsEndpoint(TfsWorkItemSettings settings, string commaSeparatedIds)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            return $"{settings.NormalizedCollectionUrl}/{Uri.EscapeDataString(settings.Project)}/_apis/wit/workitems?ids={commaSeparatedIds}&fields=System.Id,System.Title,System.State,System.WorkItemType,System.AssignedTo&api-version=6.0";
        }

        private static string EscapeWiqlValue(string value)
        {
            return value.Replace("'", "''");
        }
    }
}