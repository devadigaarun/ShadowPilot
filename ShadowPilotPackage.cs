using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using Task = System.Threading.Tasks.Task;

namespace ShadowPilot
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(ShadowPilotPackage.PackageGuidString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideOptionPage(typeof(ShadowPilotOptions), "Shadow Pilot", "General", 0, 0, true)]
    [ProvideAutoLoad(Microsoft.VisualStudio.Shell.Interop.UIContextGuids80.SolutionExists, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class ShadowPilotPackage : AsyncPackage
    {
        /// <summary>
        /// ShadowPilotPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "0a5ca1dc-bb62-4f03-b3c9-0ced54abc3b1";

        /// <summary>
        /// Auto-tagging service for AI-generated code
        /// </summary>
        private AutoTaggingService autoTaggingService;

        /// <summary>
        /// Text change monitor for detecting AI-generated code insertions
        /// </summary>
        private TextChangeMonitor textChangeMonitor;

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            System.Diagnostics.Debug.WriteLine("ShadowPilotPackage: InitializeAsync starting...");
            
            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            
            System.Diagnostics.Debug.WriteLine("ShadowPilotPackage: Switched to main thread");
            
            // Initialize existing commands
            await AgentCommand.InitializeAsync(this);
            await DynamicAgentCommand.InitializeAsync(this);

            System.Diagnostics.Debug.WriteLine("ShadowPilotPackage: Agent commands initialized");

            // Initialize auto-tagging services (works automatically based on Options setting)
            autoTaggingService = new AutoTaggingService(this);
            System.Diagnostics.Debug.WriteLine($"ShadowPilotPackage: AutoTaggingService created, IsEnabled = {autoTaggingService.IsEnabled}");
            
            textChangeMonitor = new TextChangeMonitor(this, autoTaggingService);
            await textChangeMonitor.InitializeAsync();

            System.Diagnostics.Debug.WriteLine("ShadowPilotPackage: Auto-tagging services initialized successfully");
        }

        /// <summary>
        /// Disposes of the package and its resources
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                textChangeMonitor?.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}
