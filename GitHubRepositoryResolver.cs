using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.IO;

namespace ShadowPilot
{
    public class GitHubRepositoryResolver
    {
        private readonly AsyncPackage package;

        public GitHubRepositoryResolver(AsyncPackage package)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
        }

        public GitHubRepositoryInfo ResolveCurrentRepository()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var dte = (DTE2)package.GetServiceAsync(typeof(SDTE)).Result;
            var solutionDirectory = GetSolutionDirectory(dte);
            if (string.IsNullOrEmpty(solutionDirectory))
            {
                throw new InvalidOperationException("Open a solution connected to a GitHub repository before loading issues.");
            }

            var gitDirectory = FindGitDirectory(solutionDirectory);
            if (string.IsNullOrEmpty(gitDirectory))
            {
                throw new InvalidOperationException("The open solution is not inside a Git repository.");
            }

            var configPath = Path.Combine(gitDirectory, "config");
            if (!File.Exists(configPath))
            {
                throw new InvalidOperationException("The Git repository configuration file could not be found.");
            }

            var remoteUrl = ReadOriginRemoteUrl(configPath);
            if (string.IsNullOrWhiteSpace(remoteUrl))
            {
                throw new InvalidOperationException("The Git repository does not have an origin remote configured.");
            }

            var repository = GitHubRemoteParser.Parse(remoteUrl);
            repository.LocalPath = Directory.GetParent(gitDirectory)?.FullName;

            return repository;
        }

        private static string GetSolutionDirectory(DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (dte?.Solution == null || string.IsNullOrEmpty(dte.Solution.FullName))
            {
                return null;
            }

            return Path.GetDirectoryName(dte.Solution.FullName);
        }

        private static string FindGitDirectory(string startDirectory)
        {
            var directory = new DirectoryInfo(startDirectory);
            while (directory != null)
            {
                var gitPath = Path.Combine(directory.FullName, ".git");
                if (Directory.Exists(gitPath))
                {
                    return gitPath;
                }

                directory = directory.Parent;
            }

            return null;
        }

        private static string ReadOriginRemoteUrl(string configPath)
        {
            var inOriginSection = false;

            foreach (var line in File.ReadAllLines(configPath))
            {
                var trimmed = line.Trim();
                if (trimmed.StartsWith("[", StringComparison.Ordinal))
                {
                    inOriginSection = trimmed.Equals("[remote \"origin\"]", StringComparison.OrdinalIgnoreCase);
                    continue;
                }

                if (inOriginSection && trimmed.StartsWith("url", StringComparison.OrdinalIgnoreCase))
                {
                    var equalsIndex = trimmed.IndexOf('=');
                    if (equalsIndex >= 0 && equalsIndex < trimmed.Length - 1)
                    {
                        return trimmed.Substring(equalsIndex + 1).Trim();
                    }
                }
            }

            return null;
        }
    }
}