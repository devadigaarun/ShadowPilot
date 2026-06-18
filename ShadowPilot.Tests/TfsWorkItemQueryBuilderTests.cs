using ShadowPilot;
using Xunit;

namespace ShadowPilot.Tests;

public class TfsWorkItemQueryBuilderTests
{
    [Fact]
    public void BuildAssignedToQuery_FiltersByProjectAssignedToAndOpenState()
    {
        var query = TfsWorkItemQueryBuilder.BuildAssignedToQuery("CT", "User Name");

        Assert.Contains("[System.TeamProject] = 'CT'", query);
        Assert.Contains("[System.AssignedTo] = 'User Name'", query);
        Assert.Contains("[System.State] <> 'Closed'", query);
        Assert.Contains("ORDER BY [System.ChangedDate] DESC", query);
    }

    [Fact]
    public void BuildAssignedToQuery_EscapesSingleQuotes()
    {
        var query = TfsWorkItemQueryBuilder.BuildAssignedToQuery("C'T", "O'Connor");

        Assert.Contains("[System.TeamProject] = 'C''T'", query);
        Assert.Contains("[System.AssignedTo] = 'O''Connor'", query);
    }

    [Fact]
    public void BuildWiqlEndpoint_UsesCollectionAndEscapedProject()
    {
        var settings = new TfsWorkItemSettings
        {
            CollectionUrl = "https://apollo.siemens-healthineers.com/tfs/IKM.TPC.Projects/",
            Project = "CT Project"
        };

        var endpoint = TfsWorkItemQueryBuilder.BuildWiqlEndpoint(settings);

        Assert.Equal("https://apollo.siemens-healthineers.com/tfs/IKM.TPC.Projects/CT%20Project/_apis/wit/wiql?api-version=6.0", endpoint);
    }

    [Fact]
    public void BuildWorkItemsEndpoint_IncludesIdsAndFields()
    {
        var settings = new TfsWorkItemSettings
        {
            CollectionUrl = "https://apollo.siemens-healthineers.com/tfs/IKM.TPC.Projects",
            Project = "CT"
        };

        var endpoint = TfsWorkItemQueryBuilder.BuildWorkItemsEndpoint(settings, "1,2");

        Assert.Contains("ids=1,2", endpoint);
        Assert.Contains("fields=System.Id,System.Title,System.State,System.WorkItemType,System.AssignedTo", endpoint);
        Assert.EndsWith("api-version=6.0", endpoint);
    }
}