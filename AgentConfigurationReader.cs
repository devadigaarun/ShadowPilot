/* -------------------------------------------------------------------------------------------------
   Restricted. Copyright (C) Siemens Healthineers AG, 2026. All rights reserved.
   ------------------------------------------------------------------------------------------------- */

using EnvDTE;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ShadowPilot
{
    /// <summary>
    /// Reads and manages agent configuration from global.config.json and individual agent.json files
    /// </summary>
    public class AgentConfigurationReader
    {
        private const string GlobalConfigFileName = "global.config.json";
        private const string AgentConfigFileName = "agent.json";
        private const string AgentInstructionFileName = "instruction.md";
        private const string AgentsSubDirectory = "agents";
        
        private GlobalConfiguration globalConfiguration;
        private Dictionary<string, AgentConfig> agentConfigurations;
        private string myAgentsRootDirectory;
        private AgentPathRetriever myAgentPathRetriever;
        public bool IsLegacyMode;

        /// <summary>
        /// Initializes a new instance of AgentConfigurationReader
        /// </summary>
        /// <param name="agentPathRetriever">Agent path retriever</param>
        public AgentConfigurationReader(AgentPathRetriever agentPathRetriever)
        {
            myAgentPathRetriever = agentPathRetriever ?? throw new ArgumentNullException(nameof(agentPathRetriever));
            agentConfigurations = new Dictionary<string, AgentConfig>();
        }

        /// <summary>
        /// Loads the global configuration from global.config.json and all agent configurations
        /// </summary>
        /// <returns>True if configuration was loaded successfully, false otherwise</returns>
        public bool LoadConfiguration()
        {
            try
            {
                myAgentsRootDirectory = myAgentPathRetriever.GetAgentsDirectory();

                // Load global configuration
                if (!LoadGlobalConfiguration())
                {
                    System.Diagnostics.Debug.WriteLine("Failed to load global configuration");
                }

                // Load all agent configurations
                if (LoadAgentConfigurations())
                {
                    System.Diagnostics.Debug.WriteLine($"Agent Configuration loaded successfully");
                    System.Diagnostics.Debug.WriteLine($"Global configuration version: {globalConfiguration?.Version}");
                    System.Diagnostics.Debug.WriteLine($"Agents loaded: {agentConfigurations.Count}");

                    return true;
                }
                else                
                {
                    System.Diagnostics.Debug.WriteLine("Failed to load agent configurations");
                    
                    return false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading configuration: {ex.Message}");

                return false;
            }
        }

        /// <summary>
        /// Loads the global configuration from global.config.json
        /// </summary>
        /// <returns>True if loaded successfully</returns>
        private bool LoadGlobalConfiguration()
        {
            try
            {
                string globalConfigPath = Path.Combine(myAgentsRootDirectory, GlobalConfigFileName);

                if (!File.Exists(globalConfigPath))
                {
                    System.Diagnostics.Debug.WriteLine($"Global configuration file not found: {globalConfigPath}");
                    return false;
                }

                string jsonContent = File.ReadAllText(globalConfigPath);
                globalConfiguration = JsonConvert.DeserializeObject<GlobalConfiguration>(jsonContent);

                if (globalConfiguration == null)
                {
                    System.Diagnostics.Debug.WriteLine("Failed to deserialize global configuration");
                    return false;
                }

                System.Diagnostics.Debug.WriteLine($"Global configuration loaded from: {globalConfigPath}");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading global configuration: {ex.Message}");
                return false;
            }
        }

        public bool IsAgentDirectoryChanged()
        {
            var currentDir = myAgentPathRetriever.GetAgentsDirectory();
            return !myAgentsRootDirectory.Equals(currentDir, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Loads all agent configurations from agents subdirectory
        /// </summary>
        private bool LoadAgentConfigurations()
        {
            agentConfigurations.Clear();

            string agentsSubDir = Path.Combine(myAgentsRootDirectory, AgentsSubDirectory);

            if (!Directory.Exists(agentsSubDir))
            {                
                if(Directory.Exists(Path.Combine(myAgentsRootDirectory, "instructions")))
                {
                    string instructionSubDir = Path.Combine(myAgentsRootDirectory, "instructions");

                    System.Diagnostics.Debug.WriteLine($"Agents directory not found: {agentsSubDir}");
                    System.Diagnostics.Debug.WriteLine("Checking legacy agents in <agentroot foloder>\\instructions folder");

                    LoadAgentNamesFromFiles(instructionSubDir);

                    if (agentConfigurations.Count == 0)
                    {
                        System.Diagnostics.Debug.WriteLine("No agents found in legacy mode either");

                        return false;
                    }

                    IsLegacyMode = true;
                }
                else
                if (Directory.Exists(myAgentsRootDirectory))
                {
                    System.Diagnostics.Debug.WriteLine($"Agents directory not found: {agentsSubDir}");
                    System.Diagnostics.Debug.WriteLine("Checking legacy agents in Agent Root folder ");

                    LoadAgentNamesFromFiles(myAgentsRootDirectory);

                    if (agentConfigurations.Count == 0)
                    {
                        System.Diagnostics.Debug.WriteLine("No agents found in legacy mode either");

                        return false;
                    }

                    IsLegacyMode = true;
                }

                return true;
            }

            // Get all subdirectories in the agents folder
            var agentDirectories = Directory.GetDirectories(agentsSubDir);

            foreach (var agentDir in agentDirectories)
            {
                string agentConfigPath = Path.Combine(agentDir, AgentConfigFileName);

                if (File.Exists(agentConfigPath))
                {
                    try
                    {
                        string jsonContent = File.ReadAllText(agentConfigPath);
                        var agentConfig = JsonConvert.DeserializeObject<AgentConfig>(jsonContent);

                        if (agentConfig != null)
                        {
                            // Set the agent directory for relative path resolution
                            agentConfig.AgentDirectory = agentDir;

                            // Use agent name from config, or directory name as fallback
                            string agentName = !string.IsNullOrEmpty(agentConfig.Name) 
                                ? agentConfig.Name 
                                : Path.GetFileName(agentDir);

                            agentConfigurations[agentName] = agentConfig;
                            System.Diagnostics.Debug.WriteLine($"Loaded agent: {agentName} from {agentDir}");
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error loading agent config from {agentConfigPath}: {ex.Message}");
                    }
                }
                else
                {
                    string agentInstruction = Path.Combine(agentDir, AgentInstructionFileName);

                    if (File.Exists(agentInstruction))
                    {
                        string agentName = Path.GetFileName(agentDir);
                        var agentConfig = new AgentConfig
                        {
                            Name = agentName,
                            Instruction = AgentInstructionFileName,
                            AgentDirectory = agentDir
                        };

                        agentConfigurations[agentName] = agentConfig;
                        
                        System.Diagnostics.Debug.WriteLine($"Loaded legacy agent: {agentName} from {agentDir}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"No agent.json or instruction.md found in {agentDir}");
                    }
                }
            }

            if (agentConfigurations.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("No agent configurations loaded");
                System.Diagnostics.Debug.WriteLine("Checking legacy agents ");

                LoadAgentNamesFromFiles(agentsSubDir);

                if (agentConfigurations.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("No agents found in legacy mode either");

                    return false;
                }

                IsLegacyMode = true;
            }

            return true;
        }

        private void LoadAgentNamesFromFiles(string agentsDirectory)
        {
            if (Directory.Exists(agentsDirectory))
            {
                var mdFiles = Directory.GetFiles(agentsDirectory, "*.md");
                System.Diagnostics.Debug.WriteLine($"Found {mdFiles.Length} .md files in {agentsDirectory}");

                foreach (var file in mdFiles)
                {
                    string agentName = Path.GetFileNameWithoutExtension(file);

                    if(agentName.Equals("copilot-instructions", StringComparison.OrdinalIgnoreCase) 
                        || agentName.Equals(".agent-header", StringComparison.OrdinalIgnoreCase) 
                        || agentName.Equals(".agent-footer", StringComparison.OrdinalIgnoreCase)) // Skip workspace integration instruction file
                    {
                        continue;
                    }

                    System.Diagnostics.Debug.WriteLine($"  Processing: {agentName}");

                    var agentConfig = new AgentConfig
                    {
                        Name = agentName,
                        Instruction = Path.GetFileName(file),
                        AgentDirectory = agentsDirectory
                    };

                    agentConfigurations[agentName] = agentConfig;
                }
            }
        }

        /// <summary>
        /// Gets the configuration for a specific agent
        /// </summary>
        /// <param name="agentName">Name of the agent</param>
        /// <returns>Agent configuration or null if not found</returns>
        public AgentConfig GetAgentConfig(string agentName)
        {
            if (agentConfigurations == null)
            {
                return null;
            }

            if (agentConfigurations.TryGetValue(agentName, out AgentConfig agentConfig))
            {
                return agentConfig;
            }

            return null;
        }

        /// <summary>
        /// Gets the global configuration
        /// </summary>
        /// <returns>Global configuration or null if not loaded</returns>
        public GlobalConfiguration GetGlobalConfiguration()
        {
            return globalConfiguration;
        }

        /// <summary>
        /// Checks if configuration is loaded
        /// </summary>
        /// <returns>True if configuration is loaded</returns>
        public bool IsGlobalConfigurationLoaded()
        {
            return globalConfiguration != null;
        }

        /// <summary>
        /// Checks if configuration is loaded
        /// </summary>
        /// <returns>True if configuration is loaded</returns>
        public bool IsAgentConfigurationLoaded()
        {
            return (agentConfigurations.Count != 0);
        }

        /// <summary>
        /// Gets all configured agent names
        /// </summary>
        /// <returns>List of agent names</returns>
        public List<string> GetConfiguredAgentNames()
        {
            if (agentConfigurations == null)
            {
                return new List<string>();
            }

            return agentConfigurations.Keys.ToList();
        }

        /// <summary>
        /// Builds the complete prompt for an agent by combining global and agent-specific instructions
        /// </summary>
        /// <param name="agentName">Name of the agent</param>
        /// <returns>Complete prompt with all layered instructions</returns>
        public string BuildAgentPrompt(string agentName)
        {
            if (!IsGlobalConfigurationLoaded() && !IsAgentConfigurationLoaded())
            {
                return string.Empty;
            }

            var agentConfig = GetAgentConfig(agentName);

            if (agentConfig == null)
            {
                System.Diagnostics.Debug.WriteLine($"Agent configuration not found: {agentName}");
                return string.Empty;
            }

            var parts = new List<string>();

            // Add global guardrails (highest priority)
            if (globalConfiguration?.Guardrails != null
                && globalConfiguration.Guardrails.Any()
                && !agentConfig.IgnoreGlobal)
            {
                foreach (var guardrail in globalConfiguration.Guardrails)
                {
                    var content = ReadInstructionFile(guardrail, myAgentsRootDirectory);
                    if (!string.IsNullOrEmpty(content))
                    {
                        parts.Add(content);
                    }
                }
            }

            // Add agent guardrails (next highest priority)
            if (agentConfig.Guardrails != null && agentConfig.Guardrails.Any())
            {
                foreach (var guardrail in agentConfig.Guardrails)
                {
                    var content = ReadInstructionFile(guardrail, agentConfig.AgentDirectory);
                    if (!string.IsNullOrEmpty(content))
                    {
                        parts.Add(content);
                    }
                }
            }

            // Add global prefix instructions
            if (globalConfiguration?.Prefix != null 
                && globalConfiguration.Prefix.Any() 
                && !agentConfig.IgnoreGlobal)
            {
                foreach (var prefix in globalConfiguration.Prefix)
                {
                    var content = ReadInstructionFile(prefix, myAgentsRootDirectory);
                    if (!string.IsNullOrEmpty(content))
                    {
                        parts.Add(content);
                    }
                }
            }

            // Add agent prefix instructions
            if (agentConfig.Prefix != null && agentConfig.Prefix.Any())
            {
                foreach (var prefix in agentConfig.Prefix)
                {
                    var content = ReadInstructionFile(prefix, agentConfig.AgentDirectory);
                    if (!string.IsNullOrEmpty(content))
                    {
                        parts.Add(content);
                    }
                }
            }

            // Add main agent instruction
            if (!string.IsNullOrEmpty(agentConfig.Instruction))
            {
                var agentInstruction = ReadInstructionFile(agentConfig.Instruction, agentConfig.AgentDirectory);
                if (!string.IsNullOrEmpty(agentInstruction))
                {
                    parts.Add(agentInstruction);
                }
            }

            // Add global suffix instructions
            if (globalConfiguration?.Suffix != null 
                && globalConfiguration.Suffix.Any() 
                && !agentConfig.IgnoreGlobal)
            {
                foreach (var suffix in globalConfiguration.Suffix)
                {
                    var content = ReadInstructionFile(suffix, myAgentsRootDirectory);
                    if (!string.IsNullOrEmpty(content))
                    {
                        parts.Add(content);
                    }
                }
            }

            // Add agent suffix instructions
            if (agentConfig.Suffix != null && agentConfig.Suffix.Any())
            {
                foreach (var suffix in agentConfig.Suffix)
                {
                    var content = ReadInstructionFile(suffix, agentConfig.AgentDirectory);
                    if (!string.IsNullOrEmpty(content))
                    {
                        parts.Add(content);
                    }
                }
            }

            // Add output template if configured
            string outputTemplate = null;
            string templateBaseDir = null;

            if (agentConfig != null && !string.IsNullOrEmpty(agentConfig.OutputTemplate))
            {
                outputTemplate = agentConfig.OutputTemplate;
                templateBaseDir = agentConfig.AgentDirectory;
            }
            else if (globalConfiguration != null 
                     && !string.IsNullOrEmpty(globalConfiguration.OutputTemplate) 
                     && !agentConfig.IgnoreGlobal)
            {
                outputTemplate = globalConfiguration.OutputTemplate;
                templateBaseDir = myAgentsRootDirectory;
            }

            if (!string.IsNullOrEmpty(outputTemplate))
            {
                var content = ReadInstructionFile(outputTemplate, templateBaseDir);
                if (!string.IsNullOrEmpty(content))
                {
                    parts.Add(content);
                }
            }

            return string.Join("\n\n", parts);
        }

        /// <summary>
        /// Reads an instruction file from the specified base directory
        /// </summary>
        /// <param name="relativePath">Relative path to the instruction file</param>
        /// <param name="baseDirectory">Base directory to resolve relative path from</param>
        /// <returns>File content or empty string if not found</returns>
        private string ReadInstructionFile(string relativePath, string baseDirectory)
        {
            try
            {
                if (string.IsNullOrEmpty(relativePath))
                {
                    return string.Empty;
                }

                if(relativePath.ToLower().Contains("root:"))
                {
                    // Handle root: prefix for absolute paths
                    string rootPath = relativePath.Replace("root:", myAgentsRootDirectory + "/").TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                    
                    if (File.Exists(rootPath))
                    {
                        return File.ReadAllText(rootPath);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"Instruction file not found at root path: {rootPath}");
                        return string.Empty;
                    }
                }

                // First try relative to base directory
                string fullPath = Path.Combine(baseDirectory, relativePath);

                if (File.Exists(fullPath))
                {
                    return File.ReadAllText(fullPath);
                }

                // If not found and base directory is agent directory, try global agents directory
                if (baseDirectory != myAgentsRootDirectory)
                {
                    fullPath = Path.Combine(myAgentsRootDirectory, relativePath);
                    if (File.Exists(fullPath))
                    {
                        return File.ReadAllText(fullPath);
                    }
                }

                System.Diagnostics.Debug.WriteLine($"Instruction file not found: {relativePath} (base: {baseDirectory})");
                return string.Empty;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error reading instruction file '{relativePath}': {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the instruction file path for a specific agent
        /// </summary>
        /// <param name="agentName">Name of the agent</param>
        /// <returns>Full path to the instruction file or null if not configured</returns>
        public string GetAgentInstructionPath(string agentName)
        {
            var agentConfig = GetAgentConfig(agentName);
            if (agentConfig != null && !string.IsNullOrEmpty(agentConfig.Instruction))
            {
                return Path.Combine(agentConfig.AgentDirectory, agentConfig.Instruction);
            }

            return null;
        }

        /// <summary>
        /// Gets the workspace integration configuration for a specific agent
        /// Merges agent-specific and global workspace integration settings
        /// </summary>
        /// <param name="agentName">Name of the agent (null for global only)</param>
        /// <returns>Workspace integration configuration or null if not configured</returns>
        public WorkspaceIntegrationConfig GetWorkspaceIntegrationConfig(string agentName = null)
        {
            // If no agent specified, return global config only
            if (string.IsNullOrEmpty(agentName))
            {
                return globalConfiguration?.WorkspaceIntegration;
            }

            var agentConfig = GetAgentConfig(agentName);
            
            // If agent has workspace integration, use it
            if (agentConfig?.WorkspaceIntegration != null)
            {
                return agentConfig.WorkspaceIntegration;
            }

            // Otherwise, use global configuration
            return globalConfiguration?.WorkspaceIntegration;
        }

        /// <summary>
        /// Gets all files that should be copied to the solution .github folder
        /// </summary>
        /// <param name="agentName">Name of the agent (null for global only)</param>
        /// <returns>Dictionary where key is source path and value is destination path (relative to .github)</returns>
        public Dictionary<string, string> GetFilesToCopy(string agentName = null)
        {
            var filesToCopy = new Dictionary<string, string>();
            var workspaceConfig = GetWorkspaceIntegrationConfig(agentName);

            if (workspaceConfig == null || !workspaceConfig.Enabled)
            {
                System.Diagnostics.Debug.WriteLine("Workspace integration is disabled or not configured");
                return filesToCopy;
            }

            // Determine base directory for file resolution
            string baseDirectory = myAgentsRootDirectory;
            var agentRelativePath = string.Empty;

            if (!string.IsNullOrEmpty(agentName))
            {
                var agentConfig = GetAgentConfig(agentName);
                if (agentConfig?.WorkspaceIntegration != null)
                {
                    baseDirectory = agentConfig.AgentDirectory;
                }

                agentRelativePath = agentConfig != null ? agentConfig.GetRelativePath(myAgentsRootDirectory) : string.Empty;
            }
   
            // Add copilot-instructions.md
            if (!string.IsNullOrEmpty(workspaceConfig.CopilotInstructions))
            {
                string sourceFile = Path.Combine(baseDirectory, workspaceConfig.CopilotInstructions);
                if (File.Exists(sourceFile))
                {
                    if(!string.IsNullOrEmpty(agentRelativePath))
                    {
                        filesToCopy[sourceFile] = agentRelativePath + "/copilot-instructions.md";
                    }
                    else
                    {
                        filesToCopy[sourceFile] = "copilot-instructions.md";
                    }


                    System.Diagnostics.Debug.WriteLine($"Queued for copy: {workspaceConfig.CopilotInstructions} -> copilot-instructions.md");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Copilot instructions file not found: {sourceFile}");
                }
            }

            // Add instruction files
            if (workspaceConfig.InstructionFiles != null && workspaceConfig.InstructionFiles.Any())
            {
                foreach (var pattern in workspaceConfig.InstructionFiles)
                {
                    InstructionFileEntry entry;

                    if(string.IsNullOrEmpty(agentName))
                    {
                        entry = InstructionFileEntry.Parse(pattern);
                    }
                    else
                    {
                        entry = InstructionFileEntry.Parse(myAgentsRootDirectory, baseDirectory, pattern);
                    }

                    var matchedFiles = GetMatchingFiles(entry.SourcePath, baseDirectory);

                    foreach (var file in matchedFiles)
                    {
                        string fileName = Path.GetFileName(file);
                        string destPath;

                        if (!string.IsNullOrEmpty(entry.WorkspaceFolder))
                        {
                            destPath = Path.Combine(entry.WorkspaceFolder, fileName);
                        }
                        else
                        {
                            destPath = fileName;
                        }

                        filesToCopy[file] = destPath;
                        System.Diagnostics.Debug.WriteLine($"Queued for copy: {pattern} matched {fileName} -> {destPath}");
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine($"Total files queued for copy: {filesToCopy.Count}");
            return filesToCopy;
        }

        /// <summary>
        /// Gets files matching a pattern (supports wildcards and specific files)
        /// </summary>
        /// <param name="pattern">File pattern (e.g., "instructions/*.instructions.md" or "instructions/agent.instructions.md")</param>
        /// <param name="baseDirectory">Base directory to resolve pattern from</param>
        /// <returns>List of matching file paths</returns>
        private List<string> GetMatchingFiles(string pattern, string baseDirectory)
        {
            var matchedFiles = new List<string>();

            try
            {
                string fullPattern = Path.Combine(baseDirectory, pattern);
                
                // Check if this is a specific file (no wildcards)
                if (!pattern.Contains("*") && !pattern.Contains("?"))
                {
                    if (File.Exists(fullPattern))
                    {
                        matchedFiles.Add(fullPattern);
                        System.Diagnostics.Debug.WriteLine($"Matched specific file: {pattern}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"Specific file not found: {fullPattern}");
                    }
                }
                else
                {
                    // Handle wildcard patterns
                    string directory = Path.GetDirectoryName(fullPattern);
                    string searchPattern = Path.GetFileName(fullPattern);

                    if (Directory.Exists(directory))
                    {
                        var files = Directory.GetFiles(directory, searchPattern, SearchOption.TopDirectoryOnly);
                        matchedFiles.AddRange(files);
                        System.Diagnostics.Debug.WriteLine($"Pattern '{pattern}' matched {files.Length} file(s)");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"Directory not found for pattern: {pattern}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error matching files for pattern '{pattern}': {ex.Message}");
            }

            return matchedFiles;
        }
    }
}
