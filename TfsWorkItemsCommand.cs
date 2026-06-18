using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using System.Threading.Tasks;

namespace ShadowPilot
{
    internal sealed class TfsWorkItemsCommand
    {
        public const int CommandId = 0x0301;

        public static readonly Guid CommandSet = new Guid("c2160fd4-9e6d-499c-8891-c9734a5d4ab2");

        private readonly AsyncPackage package;

        private TfsWorkItemsCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandId = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(Execute, menuCommandId);
            commandService.AddCommand(menuItem);
        }

        public static TfsWorkItemsCommand Instance { get; private set; }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new TfsWorkItemsCommand(package, commandService);
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
                var settings = ReadSettings();
                if (!settings.IsConfigured(out var validationMessage))
                {
                    throw new InvalidOperationException(validationMessage);
                }

                var client = new TfsWorkItemClient();
                var workItems = await client.GetAssignedWorkItemsAsync(settings).ConfigureAwait(true);

                using (var form = new TfsWorkItemsForm(settings, workItems))
                {
                    form.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                VsShellUtilities.ShowMessageBox(
                    package,
                    ex.Message,
                    "ShadowPilot TFS Work Items",
                    OLEMSGICON.OLEMSGICON_WARNING,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
            }
        }

        private TfsWorkItemSettings ReadSettings()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var optionsPage = package.GetDialogPage(typeof(ShadowPilotOptions)) as ShadowPilotOptions;
            return new TfsWorkItemSettings
            {
                CollectionUrl = optionsPage?.TfsCollectionUrl,
                Project = optionsPage?.TfsProject,
                AssignedTo = optionsPage?.TfsAssignedTo,
                PatEnvironmentVariable = optionsPage?.TfsPatEnvironmentVariable
            };
        }
    }
}