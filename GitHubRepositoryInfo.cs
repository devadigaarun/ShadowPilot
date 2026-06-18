namespace ShadowPilot
{
    public class GitHubRepositoryInfo
    {
        public string Owner { get; set; }

        public string Name { get; set; }

        public string RemoteUrl { get; set; }

        public string LocalPath { get; set; }

        public string ApiRepositoryPath => $"{Owner}/{Name}";

        public string WebUrl => $"https://github.com/{Owner}/{Name}";
    }
}