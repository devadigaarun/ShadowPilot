using ShadowPilot;
using Xunit;

namespace ShadowPilot.Tests;

public class TfsWorkItemSettingsTests
{
    [Fact]
    public void IsConfigured_WhenRequiredValuesArePresent_ReturnsTrue()
    {
        var settings = new TfsWorkItemSettings
        {
            CollectionUrl = "https://apollo.siemens-healthineers.com/tfs/IKM.TPC.Projects/",
            Project = "CT",
            AssignedTo = "User Name",
            PatEnvironmentVariable = "TFS_PAT"
        };

        var isConfigured = settings.IsConfigured(out var message);

        Assert.True(isConfigured);
        Assert.Equal(string.Empty, message);
        Assert.Equal("https://apollo.siemens-healthineers.com/tfs/IKM.TPC.Projects", settings.NormalizedCollectionUrl);
    }

    [Theory]
    [InlineData("", "CT", "User Name", "TFS collection URL")]
    [InlineData("https://server/tfs/Collection", "", "User Name", "TFS project")]
    [InlineData("https://server/tfs/Collection", "CT", "", "TFS assigned-to identity")]
    public void IsConfigured_WhenRequiredValueIsMissing_ReturnsFalse(string collectionUrl, string project, string assignedTo, string expectedMessagePart)
    {
        var settings = new TfsWorkItemSettings
        {
            CollectionUrl = collectionUrl,
            Project = project,
            AssignedTo = assignedTo
        };

        var isConfigured = settings.IsConfigured(out var message);

        Assert.False(isConfigured);
        Assert.Contains(expectedMessagePart, message);
    }

    [Fact]
    public void IsConfigured_WhenCollectionUrlIsInvalid_ReturnsFalse()
    {
        var settings = new TfsWorkItemSettings
        {
            CollectionUrl = "not-a-url",
            Project = "CT",
            AssignedTo = "User Name"
        };

        var isConfigured = settings.IsConfigured(out var message);

        Assert.False(isConfigured);
        Assert.Contains("absolute HTTP or HTTPS URL", message);
    }
}