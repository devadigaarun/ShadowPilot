using ShadowPilot;
using Xunit;

namespace ShadowPilot.Tests;

public class GitHubRemoteParserTests
{
    [Theory]
    [InlineData("https://github.com/devadigaarun/ShadowPilot.git", "devadigaarun", "ShadowPilot")]
    [InlineData("https://github.com/devadigaarun/ShadowPilot", "devadigaarun", "ShadowPilot")]
    [InlineData("git@github.com:devadigaarun/ShadowPilot.git", "devadigaarun", "ShadowPilot")]
    [InlineData("ssh://git@github.com/devadigaarun/ShadowPilot.git", "devadigaarun", "ShadowPilot")]
    public void TryParse_WhenRemoteIsGitHubRemote_ReturnsRepositoryInfo(string remoteUrl, string expectedOwner, string expectedName)
    {
        var parsed = GitHubRemoteParser.TryParse(remoteUrl, out var repository);

        Assert.True(parsed);
        Assert.NotNull(repository);
        Assert.Equal(expectedOwner, repository.Owner);
        Assert.Equal(expectedName, repository.Name);
        Assert.Equal(remoteUrl, repository.RemoteUrl);
        Assert.Equal($"{expectedOwner}/{expectedName}", repository.ApiRepositoryPath);
        Assert.Equal($"https://github.com/{expectedOwner}/{expectedName}", repository.WebUrl);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("https://example.com/devadigaarun/ShadowPilot.git")]
    [InlineData("git@example.com:devadigaarun/ShadowPilot.git")]
    public void TryParse_WhenRemoteIsNotSupported_ReturnsFalse(string remoteUrl)
    {
        var parsed = GitHubRemoteParser.TryParse(remoteUrl!, out var repository);

        Assert.False(parsed);
        Assert.Null(repository);
    }
}