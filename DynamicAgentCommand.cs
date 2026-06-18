using EnvDTE;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShadowPilot
{
    /// <summary>
    /// Command handler for dynamic sub menus
    /// /// </summary>
    internal sealed class DynamicAgentCommand
    {
        /// <summary>
        /// Command ID for the dynamic menu item
        /// /// </summary>
        public const int DynamicMenuId = 0x0200;
        private const int MaxMenuItems = 20;
        private static bool FirstInvocation = true;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("c2160fd4-9e6d-499c-8891-c9734a5d4ab2");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        private readonly OleMenuCommandService commandService;
        private AgentConfigurationReader myConfigReader;
        private AgentPathRetriever agentPathRetriever;

        /// <summary>
        /// List of registered menu commands
        /// /// </summary>
        private List<OleMenuCommand> menuCommands = new List<OleMenuCommand>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicAgentCommand"/> class.
        /// </summary>
        private DynamicAgentCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            this.commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            // Register multiple OleMenuCommand instances dynamically
            RegisterDynamicMenuItems();

            // Initialize agent path retriever
            agentPathRetriever = new AgentPathRetriever(package);
            myConfigReader = new AgentConfigurationReader(agentPathRetriever);

            if (!myConfigReader.LoadConfiguration())
            {
                System.Diagnostics.Debug.WriteLine($"Failed to read agent.configuration.json");

                var agentsPath = agentPathRetriever.GetAgentsDirectory(showErrorDialog: false);
                var message = $"GitHubCopilot Agents Repository: {agentsPath} is not found! \n\nPlease check configuration, Tools->Options->Shadow Pilot->General->Agents Directory Path";

                var title = "Error";

                VsShellUtilities.ShowMessageBox(
                    this.package,
                    message,
                    title,
                    OLEMSGICON.OLEMSGICON_CRITICAL,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static DynamicAgentCommand Instance { get; private set; }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IAsyncServiceProvider ServiceProvider => this.package;

        /// <summary>
        /// Registers dynamic menu items based on the loaded Agents
        /// /// </summary>
        private void RegisterDynamicMenuItems()
        {
            for (int i = 0; i < MaxMenuItems; i++)
            {
                var commandID = new CommandID(CommandSet, DynamicMenuId + i);
                var menuCmd = new OleMenuCommand(MenuItemCallback, commandID);
                menuCmd.BeforeQueryStatus += MenuItem_BeforeQueryStatus;

                commandService.AddCommand(menuCmd);
                menuCommands.Add(menuCmd);
            }
        }

        /// <summary>
        /// Updates the menu item's text based on the loaded Agents
        /// /// </summary>
        private void MenuItem_BeforeQueryStatus(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var menuCommand = sender as OleMenuCommand;
            if (menuCommand != null)
            {
                int index = menuCommand.CommandID.ID - DynamicMenuId;

                // Always reload agents on every menu opening (first item queried)
                // This ensures fresh data and proper visibility for all items
                if (index == 0)
                {
                    if (!UpdateAllMenuItems())
                    {
                        return;
                    }                   
                }

                var agents = myConfigReader.GetConfiguredAgentNames();

                // Ensure the current item is properly set
                if (index >= 0 && index < agents.Count)
                {
                    menuCommand.Text = agents[index];
                    menuCommand.Visible = true;
                }
                else
                {
                    menuCommand.Visible = false;
                }
            }
        }

        /// <summary>
        /// Updates all menu items visibility and text based on loaded agents
        /// /// </summary>
        private bool UpdateAllMenuItems()
        {           
            var agents = myConfigReader.GetConfiguredAgentNames();

            if(agents.Count == 0)
            {
                if (!myConfigReader.LoadConfiguration())
                {
                    return false;
                }
            }

            // Only update items that have corresponding agents
            // Items without agents will remain invisible (not shown in menu)
            for (int i = 0; i < menuCommands.Count && i < agents.Count; i++)
            {
                var menuCmd = menuCommands[i];
                menuCmd.Text = agents[i];
                menuCmd.Visible = true;
                System.Diagnostics.Debug.WriteLine($"  Item {i}: '{agents[i]}' - Visible");
            }

            // Hide items beyond the number of available agents
            for (int i = agents.Count; i < menuCommands.Count; i++)
            {
                menuCommands[i].Visible = false;
                System.Diagnostics.Debug.WriteLine($"  Item {i}: Hidden");
            }

            return true;
        }

        /// <summary>
        /// Called when a menu item is clicked
        /// /// </summary>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            if (FirstInvocation)
            {
                // Copy copilot-instructions.md to workspace .github folder
                CopyCopilotInstructionsToWorkspace();

                FirstInvocation = false;
                return;
            }
            else
            {
                if((!myConfigReader.IsGlobalConfigurationLoaded() && !myConfigReader.IsAgentConfigurationLoaded()) || myConfigReader.IsAgentDirectoryChanged())
                {
                    myConfigReader.LoadConfiguration();

                    if (!myConfigReader.LoadConfiguration())
                    {
                        System.Diagnostics.Debug.WriteLine($"Failed to read agent.configuration.json");

                        var agentsPath = agentPathRetriever.GetAgentsDirectory(showErrorDialog: false);
                        var message = $"GitHubCopilot Agents Repository: {agentsPath} is not found! \n\nPlease check configuration, Tools->Options->Shadow Pilot->General->Agents Directory Path";
                        var title = "Error";

                        VsShellUtilities.ShowMessageBox(
                            this.package,
                            message,
                            title,
                            OLEMSGICON.OLEMSGICON_CRITICAL,
                            OLEMSGBUTTON.OLEMSGBUTTON_OK,
                            OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

                        return;
                    }

                    // Copy copilot-instructions.md to workspace .github folder
                    CopyCopilotInstructionsToWorkspace();
                }
            }

            ThreadHelper.ThrowIfNotOnUIThread();

            var menuCommand = sender as OleMenuCommand;
            if (menuCommand != null)
            {
                int index = menuCommand.CommandID.ID - DynamicMenuId;

                var agents = myConfigReader.GetConfiguredAgentNames();

                if (index >= 0 && index < agents.Count)
                {
                    string agentName = agents[index];

                    // Build full prompt with configuration-based or legacy approach
                    var finalPrompt = BuildAgentPrompt(agentName);

                    // Invoke GitHub Copilot in chat mode with the instruction content
                    _ = InvokeCopilotChat(finalPrompt);
                }
                else
                {
                    string message = $"Could not find instruction file for agent index: {index}";
                    string title = "Copilot Agents - File Not Found";

                    VsShellUtilities.ShowMessageBox(
                        this.package,
                        message,
                        title,
                        OLEMSGICON.OLEMSGICON_WARNING,
                        OLEMSGBUTTON.OLEMSGBUTTON_OK,
                        OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
                }
            }
        }

        /// <summary>
        /// Gets the Visual Studio version
        /// /// </summary>
        private int GetVisualStudioVersion()
        {
            try
            {
                var dte = (EnvDTE80.DTE2)ServiceProvider.GetServiceAsync(typeof(SDTE)).Result;
                if (dte != null)
                {
                    string version = dte.Version;
                    System.Diagnostics.Debug.WriteLine($"Visual Studio Version: {version}");

                    // Parse version string (e.g., "17.0" for VS 2022, "18.0" for VS 2026)
                    if (int.TryParse(version.Split('.')[0], out int majorVersion))
                    {
                        return majorVersion;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting VS version: {ex.Message}");
            }

            return 0;
        }

        /// <summary>
        /// Checks if running on Visual Studio 2026 or later
        /// /// </summary>
        private bool IsVisualStudio2026OrLater()
        {
            int version = GetVisualStudioVersion();
            return version >= 18; // VS 2026 is version 18.x
        }

        private async Task InvokeCopilotChat(string instructionContent)
        {
            try
            {
                // Ensures all UI operations happen on the main thread
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                // VS 2026+ requires clipboard and keyboard injection
                if (IsVisualStudio2026OrLater())
                {
                    System.Diagnostics.Debug.WriteLine("Visual Studio 2026+ detected, using clipboard/keyboard injection");
                    bool success = await InjectViaClipboardAndKeyboard(instructionContent);

                    if (success)
                    {
                        System.Diagnostics.Debug.WriteLine("Successfully injected content into Copilot Chat");
                        return;
                    }
                }
                else
                {
                    // For VS 2022 and earlier, try the alternative method
                    System.Diagnostics.Debug.WriteLine("Visual Studio 2022 or earlier detected");
                    bool success = await InjectViaTextBuffer(instructionContent);

                    if (success)
                    {
                        System.Diagnostics.Debug.WriteLine("Successfully injected content into Copilot Chat");
                        return;
                    }
                }

                // Fallback - show clipboard message
                System.Diagnostics.Debug.WriteLine("Text injection failed, copying to clipboard as last resort");
                System.Windows.Forms.Clipboard.SetText(instructionContent);

                VsShellUtilities.ShowMessageBox(
                    this.package,
                    "Could not automatically send to Copilot Chat.\n\n" +
                    "The agent instructions have been copied to your clipboard.\n\n" +
                    "Please paste them into Copilot Chat (Ctrl+V) and send.",
                    "Copilot Agents",
                    OLEMSGICON.OLEMSGICON_WARNING,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error invoking Copilot chat: {ex.Message}\n{ex.StackTrace}");

                try
                {
                    System.Windows.Forms.Clipboard.SetText(instructionContent);
                    VsShellUtilities.ShowMessageBox(
                        this.package,
                        $"Error: {ex.Message}\n\n" +
                        "The agent instructions have been copied to your clipboard.\n\n" +
                        "Please open Copilot Chat and paste the instructions.",
                        "Copilot Agents - Error",
                        OLEMSGICON.OLEMSGICON_WARNING,
                        OLEMSGBUTTON.OLEMSGBUTTON_OK,
                        OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
                }
                catch { }
            }
        }

        private async Task<bool> InjectViaTextBuffer(string instructionContent)
        {
            try
            {
                // Ensures all UI operations happen on the main thread
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                var dte = (EnvDTE80.DTE2)await this.ServiceProvider.GetServiceAsync(typeof(SDTE));

                Document activeDocument = dte.ActiveDocument;
                string documentContent = string.Empty;

                var mcs = await this.package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;

                // Open GitHub Copilot Chat window
                CommandID copilotCommandId = new CommandID(new Guid("39B0DEDE-D931-4A92-9AA2-3447BC4998DC"), 512);
                mcs.GlobalInvoke(copilotCommandId);

                // Wait for window to open and become active
                await Task.Delay(800);

                // Get the component model for MEF services
                var componentModel = await this.package.GetServiceAsync(typeof(SComponentModel)) as IComponentModel;
                if (componentModel == null)
                {
                    throw new InvalidOperationException("Could not get component model");
                }

                var editorAdapterFactoryService = componentModel.GetService<Microsoft.VisualStudio.Editor.IVsEditorAdaptersFactoryService>();

                // Get the text manager to access the active view
                var txtMgr = await this.package.GetServiceAsync(typeof(SVsTextManager)) as IVsTextManager;
                if (txtMgr == null)
                {
                    throw new InvalidOperationException("Could not get text manager");
                }

                // Get the active text view (should be Copilot Chat input after opening)
                IVsTextView vsTextView = null;

                //CommandID copilotNewThreadCommandId = new CommandID(new Guid("39B0DEDE-D931-4A92-9AA2-3447BC4998DC"), 14336);
                //mcs.GlobalInvoke(copilotNewThreadCommandId);
                //await Task.Delay(300);

                int result = txtMgr.GetActiveView(1, null, out vsTextView);

                if (result == 0 && vsTextView != null)
                {
                    // Convert to WPF text view
                    var wpfTextView = editorAdapterFactoryService?.GetWpfTextView(vsTextView);

                    if (wpfTextView != null)
                    {
                        var prompt = instructionContent;

                        var textBuffer = wpfTextView.TextBuffer;
                        var currentSnapshot = textBuffer.CurrentSnapshot;

                        // Clear existing text and insert new prompt
                        using (var edit = textBuffer.CreateEdit())
                        {
                            // Delete all existing text
                            edit.Delete(0, currentSnapshot.Length);
                            // Insert the new prompt
                            edit.Insert(0, prompt);
                            edit.Apply();
                        }

                        // Move cursor to the end
                        var newSnapshot = textBuffer.CurrentSnapshot;
                        wpfTextView.Caret.MoveTo(new SnapshotPoint(newSnapshot, newSnapshot.Length));

                        // Wait a moment for text to be processed
                        await Task.Delay(800);

                        mcs.GlobalInvoke(copilotCommandId);
                        await Task.Delay(100);

                        // Try to send using Enter key as fallback since no send command exists
                        
                        SendKeys.SendWait("{ENTER}");
                        await Task.Delay(500);

                        System.Diagnostics.Debug.WriteLine("Successfully sent instructions to Copilot Chat");

                        return true;
                    }
                }

                // Fallback: If we couldn't access the text buffer, use SendKeys
                System.Diagnostics.Debug.WriteLine("Could not access text buffer, using SendKeys fallback");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error invoking Copilot chat: {ex.Message}");

                // Error handling - copy to clipboard as fallback
                try
                {
                    System.Windows.Forms.Clipboard.SetText(instructionContent);
                    VsShellUtilities.ShowMessageBox(
                        this.package,
                        $"Error: {ex.Message}\n\n" +
                        "The agent instructions have been copied to your clipboard.\n\n" +
                        "Please open Copilot Chat manually and paste the instructions:\n" +
                        "• Press Ctrl+/ then type 'copilot chat' or\n" +
                        "• Use View menu > GitHub Copilot Chat",
                        "Copilot Agents - Error",
                        OLEMSGICON.OLEMSGICON_WARNING,
                        OLEMSGBUTTON.OLEMSGBUTTON_OK,
                        OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
                }
                catch { }
            }

            return false;
        }

        private async Task<bool> InjectViaClipboardAndKeyboard(string content)
        {
            try
            {
                // Save current clipboard
                string previousClipboard = null;

                try
                {
                    previousClipboard = System.Windows.Forms.Clipboard.GetText();
                }
                catch { }

                try
                {
                    // Clear and set clipboard with content
                    System.Windows.Forms.Clipboard.Clear();
                    await Task.Delay(200);
                    System.Windows.Forms.Clipboard.SetText(content, System.Windows.Forms.TextDataFormat.UnicodeText);
                    await Task.Delay(300);

                    System.Diagnostics.Debug.WriteLine("Content copied to clipboard successfully");

                    var mcs = await this.package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;

                    CommandID copilotWindowCommandId = new CommandID(new Guid("39B0DEDE-D931-4A92-9AA2-3447BC4998DC"), 512);
                    mcs.GlobalInvoke(copilotWindowCommandId);
                    await Task.Delay(800);

                    CommandID copilotNewThreadCommandId = new CommandID(new Guid("39B0DEDE-D931-4A92-9AA2-3447BC4998DC"), 14336);
                    mcs.GlobalInvoke(copilotNewThreadCommandId);
                    await Task.Delay(300);

                    // Click in the input field area - Send Tab to move focus to input area if needed
                        SendKeys.SendWait("{TAB}");
                    await Task.Delay(200);

                    // Select all any existing text
                    SendKeys.SendWait("^a");
                    await Task.Delay(200);

                    // Paste the content
                    SendKeys.SendWait("^v");
                    await Task.Delay(800);

                    System.Diagnostics.Debug.WriteLine("Pasted content via keyboard");

                    // Send the message with Enter
                    SendKeys.SendWait("{ENTER}");
                    await Task.Delay(500);

                    System.Diagnostics.Debug.WriteLine("Sent message to Copilot Chat");

                    return true;
                }
                finally
                {
                    // Restore previous clipboard content after a delay
                    await Task.Delay(500);
                    if (!string.IsNullOrEmpty(previousClipboard))
                    {
                        try
                        {
                            System.Windows.Forms.Clipboard.SetText(previousClipboard);
                            System.Diagnostics.Debug.WriteLine("Restored clipboard content");
                        }
                        catch { }
                    }
                    else
                    {
                        try
                        {
                            System.Windows.Forms.Clipboard.Clear();
                        }
                        catch { }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in clipboard/keyboard injection: {ex.Message}");
                return false;
            }
        }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new DynamicAgentCommand(package, commandService);
        }

        /// <summary>
        /// Builds the complete agent prompt using configuration or legacy approach
        /// </summary>
        /// <param name="agentName">Name of the agent</param>
        /// <param name="instructionContent">Main agent instruction content</param>
        /// <returns>Complete prompt with all layered instructions</returns>
        private string BuildAgentPrompt(string agentName)
        {
            // Try to use configuration-based prompt building
            if(myConfigReader.IsGlobalConfigurationLoaded() || myConfigReader.IsAgentConfigurationLoaded())
            {
                System.Diagnostics.Debug.WriteLine($"Using configuration-based prompt building for agent: {agentName}");
                return myConfigReader.BuildAgentPrompt(agentName);
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the solution directory path
        /// /// </summary>
        private string GetSolutionDirectory()
        {
            try
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                var dte = (EnvDTE80.DTE2)ServiceProvider.GetServiceAsync(typeof(SDTE)).Result;
                
                if (dte?.Solution != null && !string.IsNullOrEmpty(dte.Solution.FullName))
                {
                    string solutionPath = dte.Solution.FullName;
                    string solutionDirectory = Path.GetDirectoryName(solutionPath);
                    System.Diagnostics.Debug.WriteLine($"Solution directory: {solutionDirectory}");
                    return solutionDirectory;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("No solution is currently open");
                    return null;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting solution directory: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Copies copilot-instructions.md from Agent Directory to workspace .github folder
        /// /// </summary>
        private void CopyCopilotInstructionsToWorkspace()
        {
            try
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                string agentsDirectory = agentPathRetriever.GetAgentsDirectory(showErrorDialog: false);
                if (string.IsNullOrEmpty(agentsDirectory))
                {
                    System.Diagnostics.Debug.WriteLine("Agents directory is not configured, skipping workspace integration");
                    return;
                }

                string solutionDirectory = GetSolutionDirectory();
                if (string.IsNullOrEmpty(solutionDirectory))
                {
                    System.Diagnostics.Debug.WriteLine("No solution is open, skipping workspace integration");
                    return;
                }

                string githubFolder = Path.Combine(solutionDirectory, ".github");
                if (!Directory.Exists(githubFolder))
                {
                    Directory.CreateDirectory(githubFolder);
                    System.Diagnostics.Debug.WriteLine($"Created .github folder: {githubFolder}");
                }

                // Use configuration-based approach if available
                if (myConfigReader != null && (myConfigReader.IsGlobalConfigurationLoaded() || (myConfigReader.IsAgentConfigurationLoaded() && !myConfigReader.IsLegacyMode)))
                {
                    System.Diagnostics.Debug.WriteLine("Using configuration-based workspace integration");
                    CopyFilesFromConfiguration(githubFolder);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Using legacy workspace integration");
                    CopyFilesLegacyApproach(agentsDirectory, githubFolder);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error during workspace integration: {ex.Message}");
            }
        }

        /// <summary>
        /// Copies files to workspace using configuration-based approach
        /// /// </summary>
        private void CopyFilesFromConfiguration(string githubFolder)
        {
            var filesToCopy = myConfigReader.GetFilesToCopy();

            var agents = myConfigReader.GetConfiguredAgentNames();

            foreach (var agent in agents)
            {
                var agentfilesToCopy = myConfigReader.GetFilesToCopy(agent);
                
                // Add agent-specific files to the main collection
                foreach (var kvp in agentfilesToCopy)
                {
                    filesToCopy[kvp.Key] = kvp.Value;
                }
            }

            if (filesToCopy.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("No files configured for workspace integration");
                return;
            }

            int copiedCount = 0;
            foreach (var kvp in filesToCopy)
            {
                try
                {
                    string sourceFile = kvp.Key;
                    string destRelativePath = kvp.Value;
                    string destFile = Path.Combine(githubFolder, destRelativePath);

                    // Create destination directory if needed
                    string destDir = Path.GetDirectoryName(destFile);
                    if (!Directory.Exists(destDir))
                    {
                        Directory.CreateDirectory(destDir);
                        System.Diagnostics.Debug.WriteLine($"Created directory: {destDir}");
                    }

                    // Copy file
                    File.Copy(sourceFile, destFile, overwrite: true);
                    copiedCount++;
                    System.Diagnostics.Debug.WriteLine($"Copied: {Path.GetFileName(sourceFile)} -> {destRelativePath}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error copying file: {ex.Message}");
                }
            }

            System.Diagnostics.Debug.WriteLine($"Workspace integration complete: {copiedCount} files copied");
        }

        /// <summary>
        /// Copies files using legacy approach (fallback when no configuration exists)
        /// /// </summary>
        private void CopyFilesLegacyApproach(string agentsDirectory, string githubFolder)
        {
            // Copy copilot-instructions.md
            string sourceFile = Path.Combine(agentsDirectory, "copilot-instructions.md");
            if (File.Exists(sourceFile))
            {
                string destinationFile = Path.Combine(githubFolder, "copilot-instructions.md");
                File.Copy(sourceFile, destinationFile, overwrite: true);
                System.Diagnostics.Debug.WriteLine($"Copied copilot-instructions.md to: {destinationFile}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"copilot-instructions.md not found in agents directory: {sourceFile}");
            }

            // Copy instruction files from instructions folder
            string instructionsSourceFolder = Path.Combine(agentsDirectory, "instructions");
            if (Directory.Exists(instructionsSourceFolder))
            {
                string instructionsDestFolder = Path.Combine(githubFolder, "instructions");
                if (!Directory.Exists(instructionsDestFolder))
                {
                    Directory.CreateDirectory(instructionsDestFolder);
                    System.Diagnostics.Debug.WriteLine($"Created instructions folder: {instructionsDestFolder}");
                }

                var allFiles = Directory.GetFiles(instructionsSourceFolder, "*.*");
                var instructionFiles = new List<string>();
                
                foreach (var file in allFiles)
                {
                    string fileName = Path.GetFileName(file);
                    if (fileName.IndexOf(".instructions.md", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        instructionFiles.Add(file);
                    }
                }

                foreach (var file in instructionFiles)
                {
                    string fileName = Path.GetFileName(file);
                    string destFile = Path.Combine(instructionsDestFolder, fileName);
                    File.Copy(file, destFile, overwrite: true);
                    System.Diagnostics.Debug.WriteLine($"Copied instruction file: {fileName}");
                }

                System.Diagnostics.Debug.WriteLine($"Copied {instructionFiles.Count} instruction files to: {instructionsDestFolder}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"instructions folder not found in agents directory: {instructionsSourceFolder}");
            }
        }
    }
}
