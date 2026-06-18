/* -------------------------------------------------------------------------------------------------
   Restricted. Copyright (C) Siemens Healthineers AG, 2026. All rights reserved.
   ------------------------------------------------------------------------------------------------- */

using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace ShadowPilot
{
    /// <summary>
    /// Monitors GitHub Copilot suggestion acceptance and automatically tags the inserted code.
    /// When auto-tagging is enabled in Tools -> Options -> Shadow Pilot -> General,
    /// this monitor intercepts Copilot's accept command and adds tags to the inserted code.
    /// 
    /// Handles both:
    /// 1. Inline suggestions (Tab to accept)
    /// 2. Copilot Edits - entire file generation (Keep button)
    /// </summary>
    public class TextChangeMonitor : IDisposable
    {
        private readonly AsyncPackage package;
        private readonly AutoTaggingService taggingService;
        private readonly object lockObject = new object();
        
        private IVsEditorAdaptersFactoryService editorAdaptersFactory;
        private IVsTextManager textManager;
        private DTE2 dte;
        private CommandEvents commandEvents;
        private DocumentEvents documentEvents;
        private bool isDisposed;
        private bool isInitialized;

        // Track state before Copilot acceptance (inline suggestions)
        private bool isExpectingCopilotInsertion;
        private int preAcceptPosition;
        private int preAcceptLength;
        private ITextBuffer preAcceptBuffer;
        private IWpfTextView preAcceptTextView;
        private bool wasDirectCopilotCommand;

        // Track state for Copilot Edits (entire file changes)
        private bool isExpectingCopilotFileChange;
        private string preChangeDocumentPath;
        private string preChangeDocumentContent;
        private Dictionary<string, string> pendingFileContents = new Dictionary<string, string>();

        // GitHub Copilot command GUIDs and known command IDs
        private static readonly Guid CopilotCommandSet = new Guid("39B0DEDE-D931-4A92-9AA2-3447BC4998DC");
        
        // Known Copilot Edits command IDs (Keep, Discard, etc.)
        private const int CopilotKeepChanges = 256;
        private const int CopilotDiscardChanges = 257;
        private const int CopilotAcceptAllEdits = 512;
        private const int CopilotApplyEdit = 1024;

        // VS Standard command sets
        private static readonly string VSStd2KCmdSet = "{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}";
        private const int ECMD_TAB = 4;

        // Minimum characters to be considered Copilot-generated code
        private const int MinCopilotInsertionLength = 3;
        private const int MinCopilotFileChangeLength = 50;

        /// <summary>
        /// Initializes a new instance of TextChangeMonitor
        /// </summary>
        public TextChangeMonitor(AsyncPackage package, AutoTaggingService taggingService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            this.taggingService = taggingService ?? throw new ArgumentNullException(nameof(taggingService));
            System.Diagnostics.Debug.WriteLine("TextChangeMonitor: Constructor called");
        }

        /// <summary>
        /// Initializes the monitor and starts listening for Copilot commands
        /// </summary>
        public async Task InitializeAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            try
            {
                System.Diagnostics.Debug.WriteLine("TextChangeMonitor: InitializeAsync starting...");
                
                var componentModel = await package.GetServiceAsync(typeof(SComponentModel)) as IComponentModel;
                if (componentModel != null)
                {
                    editorAdaptersFactory = componentModel.GetService<IVsEditorAdaptersFactoryService>();
                    System.Diagnostics.Debug.WriteLine($"TextChangeMonitor: EditorAdaptersFactory = {(editorAdaptersFactory != null ? "OK" : "NULL")}");
                }

                textManager = await package.GetServiceAsync(typeof(SVsTextManager)) as IVsTextManager;
                System.Diagnostics.Debug.WriteLine($"TextChangeMonitor: TextManager = {(textManager != null ? "OK" : "NULL")}");
                
                dte = await package.GetServiceAsync(typeof(SDTE)) as DTE2;
                System.Diagnostics.Debug.WriteLine($"TextChangeMonitor: DTE = {(dte != null ? "OK" : "NULL")}");

                if (dte != null)
                {
                    // Subscribe to DTE command events to intercept Copilot commands
                    commandEvents = dte.Events.CommandEvents;
                    commandEvents.BeforeExecute += OnBeforeCommandExecute;
                    commandEvents.AfterExecute += OnAfterCommandExecute;

                    // Subscribe to document events for file-level changes
                    documentEvents = dte.Events.DocumentEvents;
                    documentEvents.DocumentOpened += OnDocumentOpened;
                    documentEvents.DocumentSaved += OnDocumentSaved;

                    isInitialized = true;
                    System.Diagnostics.Debug.WriteLine("TextChangeMonitor: Initialized successfully - monitoring for Copilot commands");
                    System.Diagnostics.Debug.WriteLine($"TextChangeMonitor: AutoTagging Enabled = {taggingService.IsEnabled}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("TextChangeMonitor: DTE not available, command monitoring disabled");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"TextChangeMonitor: Initialization error - {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Called when a document is opened
        /// </summary>
        private void OnDocumentOpened(Document document)
        {
            if (!taggingService.IsEnabled)
                return;

            ThreadHelper.ThrowIfNotOnUIThread();

            try
            {
                if (document != null && !string.IsNullOrEmpty(document.FullName))
                {
                    string content = GetDocumentContent(document);
                    if (content != null)
                    {
                        lock (lockObject)
                        {
                            pendingFileContents[document.FullName] = content;
                        }
                        System.Diagnostics.Debug.WriteLine($"TextChangeMonitor: Tracking document opened - {Path.GetFileName(document.FullName)}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"TextChangeMonitor: Error in DocumentOpened - {ex.Message}");
            }
        }

        /// <summary>
        /// Called when a document is saved
        /// </summary>
        private void OnDocumentSaved(Document document)
        {
            if (!taggingService.IsEnabled)
                return;

            ThreadHelper.ThrowIfNotOnUIThread();

            try
            {
                if (document != null && !string.IsNullOrEmpty(document.FullName) && isExpectingCopilotFileChange)
                {
                    isExpectingCopilotFileChange = false;
                    ProcessCopilotFileChange(document);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"TextChangeMonitor: Error in DocumentSaved - {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the text content of a document
        /// </summary>
        private string GetDocumentContent(Document document)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            try
            {
                if (document == null)
                    return null;

                var textDocument = document.Object("TextDocument") as TextDocument;
                if (textDocument != null)
                {
                    var editPoint = textDocument.StartPoint.CreateEditPoint();
                    return editPoint.GetText(textDocument.EndPoint);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"TextChangeMonitor: Error getting document content - {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Called before a VS command is executed
        /// </summary>
        private void OnBeforeCommandExecute(string Guid, int ID, object CustomIn, object CustomOut, ref bool CancelDefault)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            // Log all commands for debugging (only when auto-tagging is enabled)
            if (taggingService.IsEnabled)
            {
                // Only log Copilot-related commands or Tab
                if (Guid.Contains("39B0DEDE") || (Guid.Contains("1496A755") && ID == ECMD_TAB))
                {
                    System.Diagnostics.Debug.WriteLine($"TextChangeMonitor: [BeforeExecute] Guid={Guid}, ID={ID}");
                }
            }

            if (!taggingService.IsEnabled)
            {
                return;
            }

            try
            {
                var commandType = ClassifyCommand(Guid, ID);
                
                if (commandType != CommandType.None)
                {
                    System.Diagnostics.Debug.WriteLine($"TextChangeMonitor: Detected {commandType} command - Guid: {Guid}, ID: {ID}");
                    
                    if (commandType == CommandType.CopilotFileEdit)
                    {
                        CapturePreFileChangeState();
                    }
                    else
                    {
                        CapturePreAcceptState();
                        wasDirectCopilotCommand = (commandType == CommandType.DirectCopilot);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"TextChangeMonitor: Error in BeforeExecute - {ex.Message}");
            }
        }

        /// <summary>
        /// Called after a VS command is executed
        /// </summary>
        private void OnAfterCommandExecute(string Guid, int ID, object CustomIn, object CustomOut)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (!taggingService.IsEnabled)
                return;

            try
            {
                if (isExpectingCopilotInsertion)
                {
                    System.Diagnostics.Debug.WriteLine($"TextChangeMonitor: [AfterExecute] Processing potential Copilot insertion");
                    isExpectingCopilotInsertion = false;
                    ProcessPotentialCopilotInsertion();
                }

                if (isExpectingCopilotFileChange)
                {
                    System.Diagnostics.Debug.WriteLine($"TextChangeMonitor: [AfterExecute] Processing potential Copilot file change");
                    _ = ProcessCopilotFileChangeDelayedAsync();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"TextChangeMonitor: Error in AfterExecute - {ex.Message}");
            }
        }

        private enum CommandType
        {
            None,
            DirectCopilot,
            PotentialCopilot,
            CopilotFileEdit
        }

        private CommandType ClassifyCommand(string guidString, int commandId)
        {
            // Check for GitHub Copilot specific commands
            if (System.Guid.TryParse(guidString, out System.Guid commandGuid))
            {
                if (commandGuid == CopilotCommandSet)
                {
                    System.Diagnostics.Debug.WriteLine($"TextChangeMonitor: Copilot command detected - ID: {commandId}");
                    
                    if (IsCopilotFileEditCommand(commandId))
                    {
                        return CommandType.CopilotFileEdit;
                    }
                    return CommandType.DirectCopilot;
                }
            }

            // Check for Tab command
            if (string.Equals(guidString, VSStd2KCmdSet, StringComparison.OrdinalIgnoreCase))
            {
                if (commandId == ECMD_TAB)
                {
                    if (HasActiveTextEditor())
                    {
                        System.Diagnostics.Debug.WriteLine("TextChangeMonitor: Tab command in editor detected");
                        return CommandType.PotentialCopilot;
                    }
                }
            }

            return CommandType.None;
        }

        private bool IsCopilotFileEditCommand(int commandId)
        {
            return commandId >= 256;
        }

        private bool HasActiveTextEditor()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            try
            {
                if (textManager == null)
                    return false;

                int result = textManager.GetActiveView(1, null, out IVsTextView vsTextView);
                return result == VSConstants.S_OK && vsTextView != null;
            }
            catch
            {
                return false;
            }
        }

        private void CapturePreAcceptState()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            try
            {
                if (textManager == null || editorAdaptersFactory == null)
                {
                    System.Diagnostics.Debug.WriteLine("TextChangeMonitor: Cannot capture state - services not available");
                    return;
                }

                int result = textManager.GetActiveView(1, null, out IVsTextView vsTextView);
                if (result != VSConstants.S_OK || vsTextView == null)
                {
                    System.Diagnostics.Debug.WriteLine("TextChangeMonitor: Cannot capture state - no active view");
                    return;
                }

                var wpfTextView = editorAdaptersFactory.GetWpfTextView(vsTextView);
                if (wpfTextView == null)
                {
                    System.Diagnostics.Debug.WriteLine("TextChangeMonitor: Cannot capture state - WPF text view is null");
                    return;
                }

                var textBuffer = wpfTextView.TextBuffer;
                if (textBuffer == null)
                {
                    System.Diagnostics.Debug.WriteLine("TextChangeMonitor: Cannot capture state - text buffer is null");
                    return;
                }

                lock (lockObject)
                {
                    preAcceptBuffer = textBuffer;
                    preAcceptTextView = wpfTextView;
                    preAcceptLength = textBuffer.CurrentSnapshot.Length;
                    preAcceptPosition = wpfTextView.Caret.Position.BufferPosition.Position;
                    isExpectingCopilotInsertion = true;
                }

                System.Diagnostics.Debug.WriteLine($"TextChangeMonitor: Captured pre-accept state - Position: {preAcceptPosition}, Length: {preAcceptLength}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"TextChangeMonitor: Error capturing pre-accept state - {ex.Message}");
            }
        }

        private void CapturePreFileChangeState()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            try
            {
                var activeDocument = dte?.ActiveDocument;
                if (activeDocument != null && !string.IsNullOrEmpty(activeDocument.FullName))
                {
                    string content = GetDocumentContent(activeDocument);
                    
                    lock (lockObject)
                    {
                        preChangeDocumentPath = activeDocument.FullName;
                        preChangeDocumentContent = content ?? string.Empty;
                        isExpectingCopilotFileChange = true;
                    }

                    System.Diagnostics.Debug.WriteLine($"TextChangeMonitor: Captured pre-file-change state - {Path.GetFileName(activeDocument.FullName)}, Length: {preChangeDocumentContent.Length}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"TextChangeMonitor: Error capturing pre-file-change state - {ex.Message}");
            }
        }

        private void ProcessPotentialCopilotInsertion()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            try
            {
                ITextBuffer buffer;
                IWpfTextView textView;
                int oldLength;
                int oldPosition;
                bool directCopilot;

                lock (lockObject)
                {
                    buffer = preAcceptBuffer;
                    textView = preAcceptTextView;
                    oldLength = preAcceptLength;
                    oldPosition = preAcceptPosition;
                    directCopilot = wasDirectCopilotCommand;

                    preAcceptBuffer = null;
                    preAcceptTextView = null;
                    wasDirectCopilotCommand = false;
                }

                if (buffer == null || textView == null)
                {
                    System.Diagnostics.Debug.WriteLine("TextChangeMonitor: Cannot process - buffer or textView is null");
                    return;
                }

                var currentSnapshot = buffer.CurrentSnapshot;
                int newLength = currentSnapshot.Length;
                int insertedLength = newLength - oldLength;

                System.Diagnostics.Debug.WriteLine($"TextChangeMonitor: Processing insertion - OldLength: {oldLength}, NewLength: {newLength}, InsertedLength: {insertedLength}");

                if (insertedLength > 0)
                {
                    int insertStart = oldPosition;
                    int insertEnd = oldPosition + insertedLength;

                    if (insertEnd > currentSnapshot.Length)
                        insertEnd = currentSnapshot.Length;
                    if (insertStart < 0)
                        insertStart = 0;

                    if (insertEnd > insertStart)
                    {
                        string insertedText = currentSnapshot.GetText(insertStart, insertEnd - insertStart);

                        System.Diagnostics.Debug.WriteLine($"TextChangeMonitor: Inserted text length: {insertedText.Length}, DirectCopilot: {directCopilot}");

                        if (IsLikelyCopilotInsertion(insertedText, directCopilot))
                        {
                            string preview = insertedText.Length > 50 
                                ? insertedText.Substring(0, 50) + "..." 
                                : insertedText;
                            System.Diagnostics.Debug.WriteLine($"TextChangeMonitor: Confirmed Copilot insertion - {insertedLength} chars: '{preview}'");

                            _ = TagInsertedCodeAsync(textView, insertedText, insertStart);
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"TextChangeMonitor: Insertion rejected as non-Copilot");
                        }
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("TextChangeMonitor: No text inserted (insertedLength <= 0)");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"TextChangeMonitor: Error processing insertion - {ex.Message}");
            }
        }

        private async Task ProcessCopilotFileChangeDelayedAsync()
        {
            await Task.Delay(500);

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            try
            {
                if (!isExpectingCopilotFileChange)
                    return;

                isExpectingCopilotFileChange = false;

                var activeDocument = dte?.ActiveDocument;
                if (activeDocument != null)
                {
                    ProcessCopilotFileChange(activeDocument);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"TextChangeMonitor: Error in delayed file change processing - {ex.Message}");
            }
        }

        private void ProcessCopilotFileChange(Document document)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            try
            {
                string documentPath;
                string oldContent;

                lock (lockObject)
                {
                    documentPath = preChangeDocumentPath;
                    oldContent = preChangeDocumentContent;
                    preChangeDocumentPath = null;
                    preChangeDocumentContent = null;
                }

                if (string.IsNullOrEmpty(documentPath))
                    return;

                string newContent = GetDocumentContent(document);
                if (string.IsNullOrEmpty(newContent))
                    return;

                int contentChange = newContent.Length - (oldContent?.Length ?? 0);

                System.Diagnostics.Debug.WriteLine($"TextChangeMonitor: File change - Old: {oldContent?.Length ?? 0}, New: {newContent.Length}, Change: {contentChange}");

                bool isSignificantChange = Math.Abs(contentChange) >= MinCopilotFileChangeLength ||
                                           (oldContent != null && oldContent.Length < 100 && newContent.Length >= MinCopilotFileChangeLength);

                if (isSignificantChange || (string.IsNullOrWhiteSpace(oldContent) && newContent.Length >= MinCopilotFileChangeLength))
                {
                    System.Diagnostics.Debug.WriteLine($"TextChangeMonitor: Detected Copilot Edits file generation - {Path.GetFileName(documentPath)}");
                    _ = TagEntireFileAsync(document, newContent);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"TextChangeMonitor: Error processing file change - {ex.Message}");
            }
        }

        private async Task TagEntireFileAsync(Document document, string content)
        {
            await Task.Delay(200);

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            try
            {
                if (!taggingService.IsEnabled)
                    return;

                if (textManager == null || editorAdaptersFactory == null)
                    return;

                int result = textManager.GetActiveView(1, null, out IVsTextView vsTextView);
                if (result != VSConstants.S_OK || vsTextView == null)
                    return;

                var wpfTextView = editorAdaptersFactory.GetWpfTextView(vsTextView);
                if (wpfTextView == null)
                    return;

                var snapshot = wpfTextView.TextBuffer.CurrentSnapshot;
                string currentContent = snapshot.GetText();
                
                System.Diagnostics.Debug.WriteLine($"TextChangeMonitor: Tagging entire file ({currentContent.Length} chars, {snapshot.LineCount} lines)");
                
                taggingService.TagInsertedCode(wpfTextView, currentContent, 0);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"TextChangeMonitor: Error tagging entire file - {ex.Message}");
            }
        }

        private bool IsLikelyCopilotInsertion(string insertedText, bool wasDirectCopilotCommand)
        {
            if (string.IsNullOrEmpty(insertedText))
                return false;

            // If it was a direct Copilot command, trust it
            if (wasDirectCopilotCommand && insertedText.Length >= 1)
            {
                System.Diagnostics.Debug.WriteLine("TextChangeMonitor: Direct Copilot command - accepting");
                return true;
            }

            // Filter out non-Copilot insertions
            if (insertedText == "\t")
                return false;

            if (insertedText.Length <= 8 && string.IsNullOrWhiteSpace(insertedText))
                return false;

            if (insertedText == "\r" || insertedText == "\n" || insertedText == "\r\n")
                return false;

            if (insertedText.Length < MinCopilotInsertionLength)
                return false;

            bool hasCodeIndicators = 
                insertedText.Contains(";") ||
                insertedText.Contains("{") ||
                insertedText.Contains("}") ||
                insertedText.Contains("(") ||
                insertedText.Contains(")") ||
                insertedText.Contains("=") ||
                insertedText.Contains("//") ||
                insertedText.Contains("/*") ||
                insertedText.Contains("#") ||
                insertedText.Contains("return") ||
                insertedText.Contains("if") ||
                insertedText.Contains("for") ||
                insertedText.Contains("while") ||
                insertedText.Contains("var ") ||
                insertedText.Contains("let ") ||
                insertedText.Contains("const ") ||
                insertedText.Contains("public") ||
                insertedText.Contains("private") ||
                insertedText.Contains("class") ||
                insertedText.Contains("function") ||
                insertedText.Contains("def ") ||
                insertedText.Contains("=>") ||
                insertedText.Contains("->") ||
                (insertedText.Contains("\n") && insertedText.Length > 10);

            System.Diagnostics.Debug.WriteLine($"TextChangeMonitor: Code indicators check = {hasCodeIndicators}");
            return hasCodeIndicators;
        }

        private async Task TagInsertedCodeAsync(IWpfTextView textView, string insertedText, int insertPosition)
        {
            await Task.Delay(100);

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            try
            {
                if (!taggingService.IsEnabled)
                {
                    System.Diagnostics.Debug.WriteLine("TextChangeMonitor: Auto-tagging disabled, skipping");
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"TextChangeMonitor: Calling TagInsertedCode - {insertedText.Length} chars at position {insertPosition}");
                taggingService.TagInsertedCode(textView, insertedText, insertPosition);
                System.Diagnostics.Debug.WriteLine("TextChangeMonitor: TagInsertedCode completed");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"TextChangeMonitor: Error tagging code - {ex.Message}\n{ex.StackTrace}");
            }
        }

        public void Dispose()
        {
            if (isDisposed)
                return;

            isDisposed = true;

            try
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                if (commandEvents != null)
                {
                    commandEvents.BeforeExecute -= OnBeforeCommandExecute;
                    commandEvents.AfterExecute -= OnAfterCommandExecute;
                    commandEvents = null;
                }

                if (documentEvents != null)
                {
                    documentEvents.DocumentOpened -= OnDocumentOpened;
                    documentEvents.DocumentSaved -= OnDocumentSaved;
                    documentEvents = null;
                }

                lock (lockObject)
                {
                    preAcceptBuffer = null;
                    preAcceptTextView = null;
                    preChangeDocumentPath = null;
                    preChangeDocumentContent = null;
                    pendingFileContents.Clear();
                }

                System.Diagnostics.Debug.WriteLine("TextChangeMonitor: Disposed");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"TextChangeMonitor: Error during dispose - {ex.Message}");
            }
        }
    }
}
