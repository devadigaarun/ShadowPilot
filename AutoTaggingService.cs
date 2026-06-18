/* -------------------------------------------------------------------------------------------------
   Restricted. Copyright (C) Siemens Healthineers AG, 2026. All rights reserved.
   ------------------------------------------------------------------------------------------------- */

using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Linq;

namespace ShadowPilot
{
    /// <summary>
    /// Service responsible for adding tags to AI-generated code
    /// </summary>
    public class AutoTaggingService
    {
        private readonly AsyncPackage package;

        public AutoTaggingService(AsyncPackage package)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            System.Diagnostics.Debug.WriteLine("AutoTaggingService: Constructor called");
        }

        private ShadowPilotOptions GetOptions()
        {
            try
            {
                return (ShadowPilotOptions)package.GetDialogPage(typeof(ShadowPilotOptions));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AutoTaggingService: Error getting options - {ex.Message}");
                return null;
            }
        }

        public bool IsEnabled
        {
            get
            {
                var options = GetOptions();
                return options?.EnableAutoTagging ?? false;
            }
        }

        public void TagInsertedCode(IWpfTextView textView, string insertedText, int insertPosition)
        {
            System.Diagnostics.Debug.WriteLine($"AutoTaggingService.TagInsertedCode - Length: {insertedText?.Length ?? 0}, Position: {insertPosition}");
            
            if (textView == null || string.IsNullOrEmpty(insertedText))
            {
                System.Diagnostics.Debug.WriteLine("AutoTaggingService: Invalid parameters, aborting");
                return;
            }

            var options = GetOptions();
            if (options == null || !options.EnableAutoTagging)
            {
                System.Diagnostics.Debug.WriteLine("AutoTaggingService: Auto-tagging disabled, aborting");
                return;
            }

            try
            {
                var textBuffer = textView.TextBuffer;
                var snapshot = textBuffer.CurrentSnapshot;

                bool isSingleLine = IsSingleLineCode(insertedText);
                
                int codeStartPosition = Math.Max(0, Math.Min(insertPosition, snapshot.Length - 1));
                
                var startLine = snapshot.GetLineFromPosition(codeStartPosition);
                string indentation = GetLineIndentation(snapshot, startLine.Start.Position);
                string commentPrefix = GetCommentPrefix(textView);

                string startTag = BuildTag(options.AICodeTagStart, indentation, commentPrefix);
                string endTag = BuildTag(options.AICodeTagEnd, indentation, commentPrefix);
                string singleLineTag = BuildTag(options.AICodeTagSingleLine, indentation, commentPrefix);

                int codeEndPosition = Math.Min(codeStartPosition + insertedText.Length, snapshot.Length);
                
                int startLineNumber = startLine.LineNumber;
                int endLineNumber = startLineNumber;
                
                if (codeEndPosition > 0 && codeEndPosition <= snapshot.Length)
                {
                    var endLine = snapshot.GetLineFromPosition(Math.Max(0, codeEndPosition - 1));
                    endLineNumber = endLine.LineNumber;
                }

                string trimmedText = insertedText.TrimEnd('\r', '\n');
                if (trimmedText.Length < insertedText.Length && endLineNumber > startLineNumber)
                {
                    int trimmedEndPosition = codeStartPosition + trimmedText.Length;
                    if (trimmedEndPosition > 0 && trimmedEndPosition <= snapshot.Length)
                    {
                        var adjustedEndLine = snapshot.GetLineFromPosition(trimmedEndPosition - 1);
                        endLineNumber = adjustedEndLine.LineNumber;
                    }
                }

                var finalStartLine = snapshot.GetLineFromLineNumber(startLineNumber);
                var finalEndLine = snapshot.GetLineFromLineNumber(Math.Min(endLineNumber, snapshot.LineCount - 1));

                System.Diagnostics.Debug.WriteLine($"AutoTaggingService: Tagging lines {startLineNumber} to {endLineNumber}");

                using (var edit = textBuffer.CreateEdit())
                {
                    if (isSingleLine)
                    {
                        edit.Insert(finalStartLine.Start.Position, singleLineTag + Environment.NewLine);
                    }
                    else
                    {
                        edit.Insert(finalEndLine.End.Position, Environment.NewLine + endTag);
                        edit.Insert(finalStartLine.Start.Position, startTag + Environment.NewLine);
                    }
                    edit.Apply();
                }

                System.Diagnostics.Debug.WriteLine($"AutoTaggingService: Successfully tagged {(isSingleLine ? "single-line" : "multi-line")} code");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AutoTaggingService: Error - {ex.Message}\n{ex.StackTrace}");
            }
        }

        public void TagCodeRange(IWpfTextView textView, int startLine, int endLine)
        {
            if (textView == null) return;

            var options = GetOptions();
            if (options == null || !options.EnableAutoTagging) return;

            try
            {
                var textBuffer = textView.TextBuffer;
                var snapshot = textBuffer.CurrentSnapshot;

                if (startLine < 0 || endLine >= snapshot.LineCount || startLine > endLine) return;

                var firstLine = snapshot.GetLineFromLineNumber(startLine);
                var lastLine = snapshot.GetLineFromLineNumber(endLine);

                bool isSingleLine = startLine == endLine;
                string indentation = GetLineIndentation(snapshot, firstLine.Start.Position);
                string commentPrefix = GetCommentPrefix(textView);

                string startTag = BuildTag(options.AICodeTagStart, indentation, commentPrefix);
                string endTag = BuildTag(options.AICodeTagEnd, indentation, commentPrefix);
                string singleLineTag = BuildTag(options.AICodeTagSingleLine, indentation, commentPrefix);

                using (var edit = textBuffer.CreateEdit())
                {
                    if (isSingleLine)
                    {
                        edit.Insert(firstLine.Start.Position, singleLineTag + Environment.NewLine);
                    }
                    else
                    {
                        edit.Insert(lastLine.End.Position, Environment.NewLine + endTag);
                        edit.Insert(firstLine.Start.Position, startTag + Environment.NewLine);
                    }
                    edit.Apply();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AutoTaggingService: Error tagging range - {ex.Message}");
            }
        }

        private bool IsSingleLineCode(string text)
        {
            if (string.IsNullOrEmpty(text)) return true;
            var lines = text.Trim().Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            return lines.Count(line => !string.IsNullOrWhiteSpace(line)) <= 1;
        }

        private string GetLineIndentation(ITextSnapshot snapshot, int position)
        {
            try
            {
                var line = snapshot.GetLineFromPosition(position);
                string lineText = line.GetText();
                int indent = 0;
                foreach (char c in lineText)
                {
                    if (c == ' ' || c == '\t') indent++;
                    else break;
                }
                return lineText.Substring(0, indent);
            }
            catch { return string.Empty; }
        }

        private string GetCommentPrefix(IWpfTextView textView)
        {
            try
            {
                string contentType = textView.TextBuffer.ContentType.TypeName.ToLowerInvariant();
                switch (contentType)
                {
                    case "python": return "#";
                    case "html": case "xml": case "xaml": return "<!--";
                    case "css": case "less": case "scss": return "/*";
                    case "sql": return "--";
                    case "vb": case "basic": return "'";
                    case "powershell": return "#";
                    case "fsharp": return "//";
                    case "ruby": case "perl": return "#";
                    case "bash": case "shellscript": return "#";
                    default: return "//";
                }
            }
            catch { return "//"; }
        }

        private string GetCommentSuffix(string commentPrefix)
        {
            switch (commentPrefix)
            {
                case "<!--": return " -->";
                case "/*": return " */";
                default: return string.Empty;
            }
        }

        private string BuildTag(string tagTemplate, string indentation, string commentPrefix)
        {
            string trimmedTag = tagTemplate.TrimStart();
            if (trimmedTag.StartsWith("//") || trimmedTag.StartsWith("#") || 
                trimmedTag.StartsWith("'") || trimmedTag.StartsWith("--") ||
                trimmedTag.StartsWith("<!--") || trimmedTag.StartsWith("/*"))
            {
                return indentation + trimmedTag;
            }

            string suffix = GetCommentSuffix(commentPrefix);
            string tagContent = tagTemplate.Trim()
                .TrimStart('/', '*', '#', '-', '<', '!', '\'', ' ')
                .TrimEnd('/', '*', '>', '-', ' ')
                .Trim();

            return $"{indentation}{commentPrefix} {tagContent}{suffix}";
        }
    }
}
