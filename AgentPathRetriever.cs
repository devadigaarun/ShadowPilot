using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;

namespace ShadowPilot
{
    /// <summary>
    /// Retrieves the agents directory path from Visual Studio options
    /// </summary>
    public class AgentPathRetriever
    {
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of myAgentPathRetriever
        /// </summary>
        /// <param name="package">The Visual Studio package instance</param>
        public AgentPathRetriever(AsyncPackage package)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
        }

        /// <summary>
        /// Gets the agents directory path from Visual Studio options
        /// </summary>
        /// <param name="showErrorDialog">Whether to show error dialog if path is not configured</param>
        /// <returns>Agents directory path or empty string if not configured</returns>
        public string GetAgentsDirectory(bool showErrorDialog = true)
        {
            var agentsPath = string.Empty;
            
            try
            {
                var optionsPage = this.package.GetDialogPage(typeof(ShadowPilotOptions)) as ShadowPilotOptions;
                if (optionsPage != null && !string.IsNullOrEmpty(optionsPage.AgentsPath))
                {
                    agentsPath = optionsPage.AgentsPath;
                    System.Diagnostics.Debug.WriteLine($"Using AgentsPath from Tools > Options: {agentsPath}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error reading AgentsPath from options: {ex.Message}");
            }
            
            if (string.IsNullOrEmpty(agentsPath) && showErrorDialog)
            {
                string message = $"AgentsPath is not configured. Please configure using Tools->Options->ShadowPilot->General->AgentsPath";
                string title = "Copilot Agents - Instruction files not found!";

                VsShellUtilities.ShowMessageBox(
                    this.package,
                    message,
                    title,
                    OLEMSGICON.OLEMSGICON_WARNING,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

                System.Diagnostics.Debug.WriteLine($"AgentsPath not configured");
            }

            return agentsPath;
        }
    }
}
