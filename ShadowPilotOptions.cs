/* -------------------------------------------------------------------------------------------------
   Restricted. Copyright (C) Siemens Healthineers AG, 2026. All rights reserved.
   ------------------------------------------------------------------------------------------------- */
   

using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel;

namespace ShadowPilot
{
    /// <summary>
    /// Shadow Pilot configuration options page
    /// </summary>
    public class ShadowPilotOptions : DialogPage
    {
        private string agentsPath = @"";
        private bool enableAutoTagging = false;
        private string aiCodeTagStart = "// [AI-GENERATED CODE START]";
        private string aiCodeTagEnd = "// [AI-GENERATED CODE END]";
        private string aiCodeTagSingleLine = "// [AI-GENERATED]";

        /// <summary>
        /// Gets or sets the agents directory path
        /// </summary>
        [Category("Shadow Pilot")]
        [DisplayName("Agents Directory Path")]
        [Description("The network or local path where agent definition files (.md) are stored. Environment variable AgentsPath will override this setting.")]
        public string AgentsPath
        {
            get { return agentsPath; }
            set { agentsPath = value; }
        }

        /// <summary>
        /// Gets or sets whether auto-tagging is enabled for AI-generated code
        /// </summary>
        [Category("Auto Tagging")]
        [DisplayName("Enable Auto Tagging")]
        [Description("When enabled, automatically adds tags before and after GitHub Copilot generated code. For single-line code, only one tag is added above the line.")]
        [DefaultValue(false)]
        public bool EnableAutoTagging
        {
            get { return enableAutoTagging; }
            set { enableAutoTagging = value; }
        }

        /// <summary>
        /// Gets or sets the tag added at the start of AI-generated code blocks
        /// </summary>
        [Category("Auto Tagging")]
        [DisplayName("Start Tag")]
        [Description("The comment tag added before multi-line AI-generated code blocks.")]
        [DefaultValue("// [AI-GENERATED CODE START]")]
        public string AICodeTagStart
        {
            get { return aiCodeTagStart; }
            set { aiCodeTagStart = value; }
        }

        /// <summary>
        /// Gets or sets the tag added at the end of AI-generated code blocks
        /// </summary>
        [Category("Auto Tagging")]
        [DisplayName("End Tag")]
        [Description("The comment tag added after multi-line AI-generated code blocks.")]
        [DefaultValue("// [AI-GENERATED CODE END]")]
        public string AICodeTagEnd
        {
            get { return aiCodeTagEnd; }
            set { aiCodeTagEnd = value; }
        }

        /// <summary>
        /// Gets or sets the tag added for single-line AI-generated code
        /// </summary>
        [Category("Auto Tagging")]
        [DisplayName("Single Line Tag")]
        [Description("The comment tag added above single-line AI-generated code. Only this tag is used when the generated code is a single line.")]
        [DefaultValue("// [AI-GENERATED]")]
        public string AICodeTagSingleLine
        {
            get { return aiCodeTagSingleLine; }
            set { aiCodeTagSingleLine = value; }
        }

        /// <summary>
        /// Override OnApply to save settings
        /// </summary>
        protected override void OnApply(PageApplyEventArgs e)
        {
            if (e.ApplyBehavior == ApplyKind.Apply)
            {
                System.Diagnostics.Debug.WriteLine($"ShadowPilotOptions: Saving AgentsPath = {agentsPath}");
                System.Diagnostics.Debug.WriteLine($"ShadowPilotOptions: Saving EnableAutoTagging = {enableAutoTagging}");
            }
            base.OnApply(e);
        }
    }
}
