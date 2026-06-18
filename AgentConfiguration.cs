/* -------------------------------------------------------------------------------------------------
   Restricted. Copyright (C) Siemens Healthineers AG, 2026. All rights reserved.
   ------------------------------------------------------------------------------------------------- */

using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace ShadowPilot
{
    /// <summary>
    /// Represents the global configuration from global.config.json
    /// </summary>
    public class GlobalConfiguration
    {
        /// <summary>
        /// Configuration version
        /// </summary>
        [JsonProperty("version")]
        public string Version { get; set; }

        /// <summary>
        /// Workspace integration settings for copying files to solution .github folder
        /// </summary>
        [JsonProperty("workspaceIntegration")]
        public WorkspaceIntegrationConfig WorkspaceIntegration { get; set; }

        /// <summary>
        /// List of guardrail instruction file paths
        /// </summary>
        [JsonProperty("guardrails")]
        public List<string> Guardrails { get; set; }

        /// <summary>
        /// List of instruction files to prepend to agent prompts
        /// </summary>
        [JsonProperty("prefix")]
        public List<string> Prefix { get; set; }

        /// <summary>
        /// List of instruction files to append to agent prompts
        /// </summary>
        [JsonProperty("suffix")]
        public List<string> Suffix { get; set; }

        /// <summary>
        /// Path to the output template file
        /// </summary>
        [JsonProperty("outputTemplate")]
        public string OutputTemplate { get; set; }

        public GlobalConfiguration()
        {
            Guardrails = new List<string>();
            Prefix = new List<string>();
            Suffix = new List<string>();
        }
    }

    /// <summary>
    /// Configuration for workspace integration (copying files to solution .github folder)
    /// </summary>
    public class WorkspaceIntegrationConfig
    {
        /// <summary>
        /// Path to copilot-instructions.md file to copy to [Solution]/.github/
        /// </summary>
        [JsonProperty("copilotInstructions")]
        public string CopilotInstructions { get; set; }

        /// <summary>
        /// List of instruction files to copy to [Solution]/.github/
        /// Supports two formats:
        /// 1. Simple path: "instructions/file.md" - copies to .github/ preserving relative path
        /// 2. Prefixed path: "folder:instructions/file.md" - copies to .github/folder/ preserving source relative path
        /// Supports wildcards (e.g., "templates:templates/*.md")
        /// </summary>
        [JsonProperty("instructionFiles")]
        public List<string> InstructionFiles { get; set; }

        /// <summary>
        /// Whether to enable workspace integration (default: true)
        /// </summary>
        [JsonProperty("enabled")]
        public bool Enabled { get; set; }

        public WorkspaceIntegrationConfig()
        {
            InstructionFiles = new List<string>();
            Enabled = true;
            CopilotInstructions = "copilot-instructions.md";
        }
    }

    /// <summary>
    /// Agent-specific configuration from agent.json
    /// </summary>
    public class AgentConfig
    {
        /// <summary>
        /// Name of the agent
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Path to the agent's instruction file (relative to agent folder)
        /// </summary>
        [JsonProperty("instruction")]
        public string Instruction { get; set; }

        /// <summary>
        /// Flag to ignore global configuration for this agent
        /// </summary>
        [JsonProperty("ignoreGlobal")]
        public bool IgnoreGlobal { get; set; }

        /// <summary>
        /// Agent-specific guardrail instruction file paths
        /// </summary>
        [JsonProperty("guardrails")]
        public List<string> Guardrails { get; set; }

        /// <summary>
        /// Agent-specific instruction files to prepend
        /// </summary>
        [JsonProperty("prefix")]
        public List<string> Prefix { get; set; }

        /// <summary>
        /// Agent-specific instruction files to append
        /// </summary>
        [JsonProperty("suffix")]
        public List<string> Suffix { get; set; }

        /// <summary>
        /// Agent-specific output template file path
        /// </summary>
        [JsonProperty("outputTemplate")]
        public string OutputTemplate { get; set; }

        /// <summary>
        /// Agent-specific workspace integration settings
        /// </summary>
        [JsonProperty("workspaceIntegration")]
        public WorkspaceIntegrationConfig WorkspaceIntegration { get; set; }

        /// <summary>
        /// Directory path where this agent's files are located (set at runtime)
        /// </summary>
        [JsonIgnore]
        public string AgentDirectory { get; set; }

        public AgentConfig()
        {
            Guardrails = new List<string>();
            Prefix = new List<string>();
            Suffix = new List<string>();
            IgnoreGlobal = false;
        }

        public string GetRelativePath(string rootFolder)
        {
            return AgentDirectory.Replace(rootFolder, "").Trim(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }
    }

    /// <summary>
    /// Represents a parsed instruction file entry with optional folder prefix
    /// </summary>
    public class InstructionFileEntry
    {
        /// <summary>
        /// The workspace folder to copy files to (null means use default "instructions" folder for .instructions.* files, or preserve path for others)
        /// </summary>
        public string WorkspaceFolder { get; set; }

        /// <summary>
        /// The source file path pattern (may contain wildcards)
        /// </summary>
        public string SourcePath { get; set; }

        /// <summary>
        /// Parses an instruction file entry in the format "folder:path" or "path"
        /// </summary>
        /// <param name="entry">The instruction file entry string</param>
        /// <returns>Parsed InstructionFileEntry</returns>
        public static InstructionFileEntry Parse(string entry)
        {
            if (string.IsNullOrWhiteSpace(entry))
            {
                return new InstructionFileEntry { SourcePath = entry };
            }

            var colonIndex = entry.IndexOf(':');
            if (colonIndex > 0 && colonIndex < entry.Length - 1)
            {
                return new InstructionFileEntry
                {
                    WorkspaceFolder = entry.Substring(0, colonIndex),
                    SourcePath = entry.Substring(colonIndex + 1)
                };
            }

            return new InstructionFileEntry
            {
                WorkspaceFolder = null,
                SourcePath = entry
            };
        }

        public static InstructionFileEntry Parse(string rootFolder, string agentFolder, string entry)
        {
            if (string.IsNullOrWhiteSpace(entry))
            {
                return new InstructionFileEntry { SourcePath = entry };
            }

            var colonIndex = entry.IndexOf(':');
            if (colonIndex > 0 && colonIndex < entry.Length - 1)
            {
                return new InstructionFileEntry
                {
                    WorkspaceFolder = entry.Substring(0, colonIndex),
                    SourcePath = entry.Substring(colonIndex + 1)
                };
            }

            //assign relative folder of the agent (after removing rootFolder from this path) + entry path, so that it can be copied to the same relative path under .github
            var workspaceFolder = agentFolder.StartsWith(rootFolder) ? agentFolder.Substring(rootFolder.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) : null;
            //append path in entry without file name
            workspaceFolder = string.IsNullOrEmpty(workspaceFolder) ? null : Path.Combine(workspaceFolder, Path.GetDirectoryName(entry) ?? string.Empty);

            return new InstructionFileEntry
            {
                WorkspaceFolder = workspaceFolder,
                SourcePath = entry
            };
        }
    }

    /// <summary>
    /// Legacy configuration structure (deprecated - for backward compatibility only)
    /// </summary>
    public class AgentConfiguration
    {
        /// <summary>
        /// Configuration version
        /// </summary>
        [JsonProperty("version")]
        public string Version { get; set; }

        /// <summary>
        /// Global configuration settings applied to all agents unless overridden
        /// </summary>
        [JsonProperty("global")]
        public GlobalConfiguration Global { get; set; }

        /// <summary>
        /// Dictionary of agent-specific configurations
        /// </summary>
        [JsonProperty("agents")]
        public Dictionary<string, AgentConfig> Agents { get; set; }

        public AgentConfiguration()
        {
            Agents = new Dictionary<string, AgentConfig>();
        }
    }
}
