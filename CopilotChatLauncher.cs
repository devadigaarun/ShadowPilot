using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShadowPilot
{
    public class CopilotChatLauncher
    {
        private readonly AsyncPackage package;

        public CopilotChatLauncher(AsyncPackage package)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
        }

        public async Task LaunchAsync(string prompt)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            if (string.IsNullOrWhiteSpace(prompt))
            {
                throw new ArgumentException("Prompt cannot be empty.", nameof(prompt));
            }

            if (!await TryInjectAndSendAsync(prompt))
            {
                Clipboard.SetText(prompt, TextDataFormat.UnicodeText);

                VsShellUtilities.ShowMessageBox(
                    package,
                    "The selected GitHub issue workflow prompt was copied to the clipboard. Open GitHub Copilot Chat, paste it, and send it to start the autonomous workflow.",
                    "ShadowPilot GitHub Issue Workflow",
                    OLEMSGICON.OLEMSGICON_WARNING,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
            }
        }

        private async Task<bool> TryInjectAndSendAsync(string prompt)
        {
            string previousClipboard = null;

            try
            {
                try
                {
                    previousClipboard = Clipboard.GetText();
                }
                catch
                {
                    previousClipboard = null;
                }

                Clipboard.SetText(prompt, TextDataFormat.UnicodeText);

                var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
                var copilotWindowCommandId = new CommandID(new Guid("39B0DEDE-D931-4A92-9AA2-3447BC4998DC"), 512);
                commandService?.GlobalInvoke(copilotWindowCommandId);
                await Task.Delay(800);

                var copilotNewThreadCommandId = new CommandID(new Guid("39B0DEDE-D931-4A92-9AA2-3447BC4998DC"), 14336);
                commandService?.GlobalInvoke(copilotNewThreadCommandId);
                await Task.Delay(300);

                SendKeys.SendWait("{TAB}");
                await Task.Delay(200);
                SendKeys.SendWait("^a");
                await Task.Delay(200);
                SendKeys.SendWait("^v");
                await Task.Delay(800);
                SendKeys.SendWait("{ENTER}");

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GitHub issue workflow prompt injection failed: {ex.Message}");
                return false;
            }
            finally
            {
                await Task.Delay(500);

                try
                {
                    if (!string.IsNullOrEmpty(previousClipboard))
                    {
                        Clipboard.SetText(previousClipboard);
                    }
                }
                catch
                {
                }
            }
        }
    }
}