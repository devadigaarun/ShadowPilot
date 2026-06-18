using System;
using System.Text.RegularExpressions;

namespace ShadowPilot
{
    public static class GitHubRemoteParser
    {
        public static GitHubRepositoryInfo Parse(string remoteUrl)
        {
            if (TryParse(remoteUrl, out var repository))
            {
                return repository;
            }

            throw new InvalidOperationException($"The origin remote is not a supported GitHub URL: {remoteUrl}");
        }

        public static bool TryParse(string remoteUrl, out GitHubRepositoryInfo repository)
        {
            repository = null;

            if (string.IsNullOrWhiteSpace(remoteUrl))
            {
                return false;
            }

            var httpsMatch = Regex.Match(remoteUrl, @"^https://github\.com/(?<owner>[^/]+)/(?<repo>[^/]+?)(?:\.git)?/?$", RegexOptions.IgnoreCase);
            if (httpsMatch.Success)
            {
                repository = CreateRepositoryInfo(httpsMatch, remoteUrl);
                return true;
            }

            var sshMatch = Regex.Match(remoteUrl, @"^(?:git@github\.com:|ssh://git@github\.com/)(?<owner>[^/]+)/(?<repo>[^/]+?)(?:\.git)?/?$", RegexOptions.IgnoreCase);
            if (sshMatch.Success)
            {
                repository = CreateRepositoryInfo(sshMatch, remoteUrl);
                return true;
            }

            return false;
        }

        private static GitHubRepositoryInfo CreateRepositoryInfo(Match match, string remoteUrl)
        {
            return new GitHubRepositoryInfo
            {
                Owner = match.Groups["owner"].Value,
                Name = match.Groups["repo"].Value,
                RemoteUrl = remoteUrl
            };
        }
    }
}