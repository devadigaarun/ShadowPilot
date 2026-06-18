# Agent Configuration Reader

## Overview
The `AgentConfigurationReader` class provides functionality to read and manage agent configurations from `agent.configuration.json` file.

## Usage Examples

### 1. Initialize the Reader

```csharp
string agentsDirectory = @"C:\ShadowPilot\Agents";
var configReader = new AgentConfigurationReader(agentsDirectory);

// Load the configuration
if (configReader.LoadConfiguration())
{
    // Configuration loaded successfully
}
else
{
    // Configuration file not found or invalid
}
```

### 2. Get Agent Configuration

```csharp
// Get specific agent configuration
AgentConfig codeReviewConfig = configReader.GetAgentConfig("Code Review Agent");

if (codeReviewConfig != null)
{
    string instructionPath = codeReviewConfig.Instruction;
    bool ignoreGlobal = codeReviewConfig.IgnoreGlobal;
    List<string> prefixes = codeReviewConfig.Prefix;
}
```

### 3. Get Global Configuration

```csharp
GlobalConfiguration globalConfig = configReader.GetGlobalConfiguration();

if (globalConfig != null)
{
    List<string> guardrails = globalConfig.Guardrails;
    List<string> prefixes = globalConfig.Prefix;
    List<string> suffixes = globalConfig.Suffix;
    string outputTemplate = globalConfig.OutputTemplate;
}
```

### 4. Get All Configured Agents

```csharp
List<string> agentNames = configReader.GetConfiguredAgentNames();

foreach (string agentName in agentNames)
{
    Console.WriteLine($"Agent: {agentName}");
}
```

### 5. Build Complete Agent Prompt

```csharp
// Read the main agent instruction file
string agentInstructionContent = File.ReadAllText(@"C:\ShadowPilot\Agents\Code Review Agent.md");

// Build the complete prompt with all layered instructions
string completePrompt = configReader.BuildAgentPrompt("Code Review Agent", agentInstructionContent);

// The completePrompt now contains:
// 1. Guardrails (if configured)
// 2. Prefix instructions (global + agent-specific)
// 3. Main agent instruction
// 4. Suffix instructions (global + agent-specific)
// 5. Output template (if configured)
```

### 6. Get Agent Instruction Path

```csharp
string instructionPath = configReader.GetAgentInstructionPath("Clean Code Agent");
// Returns: "C:\ShadowPilot\Agents\Clean Code Agent.md"
```

## Integration with DynamicAgentCommand

The configuration reader can be integrated into the existing `DynamicAgentCommand` class to enhance prompt building:

```csharp
private string BuildAgentPrompt(string agentName, string instructionContent)
{
    // Try to use configuration-based prompt building
    var configReader = new AgentConfigurationReader(GetAgentsDirectory());
    
    if (configReader.LoadConfiguration())
    {
        return configReader.BuildAgentPrompt(agentName, instructionContent);
    }
    
    // Fallback to existing logic if no configuration exists
    var generalAgentHeaderInstructions = ReadGeneralInstructions(".agent-header.md");
    var generalAgentFooterInstructions = ReadGeneralInstructions(".agent-footer.md");
    
    // ... existing code ...
}
```

## Configuration File Structure

### Minimal Configuration
```json
{
  "version": "1.0",
  "agents": {
    "Simple Agent": {
      "instruction": "Simple Agent.md"
    }
  }
}
```

### Full Configuration with Global Settings
```json
{
  "version": "1.0",
  "global": {
    "guardrails": [
      "org/guardrails.instructions.md"
    ],
    "prefix": [
      "org/architecture.instructions.md"
    ],
    "suffix": [
      "standards/coding-standards.instructions.md"
    ],
    "outputTemplate": "templates/default-output-template.md"
  },
  "agents": {
    "Code Review Agent": {
      "instruction": "Code Review Agent.md",
      "prefix": [
        "standards/code-review-guidelines.md"
      ]
    }
  }
}
```

### Agent with Custom Settings (Override Global)
```json
{
  "version": "1.0",
  "global": {
    "prefix": ["global-prefix.md"]
  },
  "agents": {
    "Custom Agent": {
      "instruction": "Custom Agent.md",
      "ignoreGlobal": false,
      "guardrails": ["custom-guardrails.md"],
      "prefix": ["custom-prefix.md"],
      "suffix": ["custom-suffix.md"],
      "outputTemplate": "templates/custom-template.md"
    }
  }
}
```

## Notes

- All file paths in the configuration are relative to the agents directory
- If `ignoreGlobal` is `true`, global settings are not applied to that agent
- Agent-specific settings override global settings when both are present
- Files are loaded in this order: guardrails ? prefix ? instruction ? suffix ? outputTemplate
