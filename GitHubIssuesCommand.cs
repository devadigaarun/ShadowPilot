using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using System.Threading.Tasks;

namespace ShadowPilot
{
    internal sealed class GitHubIssuesCommand
    {
        public const int CommandId = 0x0300;

        public static readonly Guid CommandSet = new Guid("c2160fd4-9e6d-499c-8891-c9734a5d4ab2");

        private readonly AsyncPackage package;

        private GitHubIssuesCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandId = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(Execute, menuCommandId);
            commandService.AddCommand(menuItem);
        }

        public static GitHubIssuesCommand Instance { get; private set; }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new GitHubIssuesCommand(package, commandService);
        }

        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _ = package.JoinableTaskFactory.RunAsync(ExecuteAsync);
        }

        private async Task ExecuteAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            try
            {
                var repositoryResolver = new GitHubRepositoryResolver(package);
                var repository = repositoryResolver.ResolveCurrentRepository();

                var issueClient = new GitHubIssueClient();
                var issues = await issueClient.GetOpenIssuesAsync(repository).ConfigureAwait(true);

                using (var form = new GitHubIssuesForm(repository, issues))
                {
                    var result = form.ShowDialog();
                    if (result != System.Windows.Forms.DialogResult.OK || form.SelectedIssue == null)
                    {
                        return;
                    }

                    var promptBuilder = new IssueWorkflowPromptBuilder();
                    var prompt = promptBuilder.BuildPrompt(repository, form.SelectedIssue);
                    var launcher = new CopilotChatLauncher(package);
                    await launcher.LaunchAsync(prompt).ConfigureAwait(true);
                }
            }
            catch (Exception ex)
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                VsShellUtilities.ShowMessageBox(
                    package,
                    ex.Message,
                    "ShadowPilot GitHub Issues",
                    OLEMSGICON.OLEMSGICON_WARNING,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
            }
        }
    }
}