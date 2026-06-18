# Configuration Refactoring Guide: New Structure

## Overview
The ShadowPilot configuration has been refactored to support a more modular, scalable structure. Instead of a single `agent.configuration.json` file, the new system uses:
- **One global configuration file** (`global.config.json`)
- **Multiple agent-specific configuration files** (`agents/<agent-name>/agent.json`)

## Benefits of New Structure

### ? **Better Organization**
- Each agent has its own directory
- Agent files are self-contained
- Easier to manage large numbers of agents

### ? **Improved Scalability**
- Add new agents without modifying global config
- Agents can be versioned independently
- Easier to share agents across teams

### ? **Agent-Specific Resources**
- Each agent can have its own instruction files
- Workspace integration can be customized per agent
- Templates and examples stay with the agent

### ? **Simplified Collaboration**
- Teams can own specific agents
- Merge conflicts reduced
- Better Git history per agent

## New File Structure

```
<root-folder>/
??? global.config.json                    # Global configuration
??? agents/                                # All agents directory
?   ??? clean-code/                        # Agent: Clean Code
?   ?   ??? agent.json                     # Agent configuration
?   ?   ??? instruction.md                 # Main instruction
?   ?   ??? guidelines.md                  # Additional files
?   ?   ??? examples.md
?   ?   ??? templates/
?   ?       ??? output-template.md
?   ??? code-review/                       # Agent: Code Review
?   ?   ??? agent.json
?   ?   ??? instruction.md
?   ?   ??? guidelines.md
?   ?   ??? instructions/
?   ?       ??? code-review.instructions.md
?   ??? security-audit/                    # Agent: Security Audit
?       ??? agent.json
?       ??? instruction.md
?       ??? templates/
?           ??? security-template.md
??? instructions/                          # Shared instructions
?   ??? copilot-instructions.md
?   ??? agent.instructions.md
??? org/                                   # Organization-level files
?   ??? guardrails.instructions.md
?   ??? architecture.instructions.md
?   ??? security.instructions.md
??? standards/                             # Coding standards
?   ??? testing.instructions.md
?   ??? coding-standards.instructions.md
??? templates/                             # Shared templates
    ??? default-output-template.md
```

## Configuration Files

### 1. Global Configuration (`global.config.json`)

Located at `<root-folder>/global.config.json`

```json
{
  "version": "1.0",
  
  "workspaceIntegration": {
    "enabled": true,
    "copilotInstructions": "instructions/copilot-instructions.md",
    "instructionFiles": [
      "instructions:instructions/*.instructions.md",
      "templates/default:templates/default-output-template.md"
    ]
  },
  
  "guardrails": [
    "org/guardrails.instructions.md"
  ],

  "prefix": [
    "org/architecture.instructions.md",
    "org/security.instructions.md",
    "standards/testing.instructions.md"
  ],

  "suffix": [
    "standards/coding-standards.instructions.md"
  ],

  "outputTemplate": "templates/default-output-template.md"
}
```

**Properties:**
- `version` - Configuration schema version
- `workspaceIntegration` - Files to copy to solution `.github` folder
- `guardrails` - Organization-level guardrails (highest priority)
- `prefix` - Instructions prepended to all agents
- `suffix` - Instructions appended to all agents
- `outputTemplate` - Default output template for all agents

**Path Resolution:**
All paths in `global.config.json` are relative to `<root-folder>/`

### 2. Agent Configuration (`agents/<agent-name>/agent.json`)

Located at `<root-folder>/agents/<agent-name>/agent.json`

```json
{
  "name": "Code Review Agent",
  "instruction": "instruction.md",

  "prefix": [
    "guidelines.md"
  ],

  "suffix": [
    "examples.md"
  ],

  "guardrails": [
    "org/guardrails/security.instructions.md"
  ],

  "outputTemplate": "templates/code-review-template.md",

  "ignoreGlobal": false,
  
  "workspaceIntegration": {
    "enabled": true,
    "copilotInstructions": "instructions/code-review.instructions.md",
    "instructionFiles": [
      "templates:templates/code-review-template.md",
      "examples:examples/*.md"
    ]
  }
}
```

**Properties:**
- `name` - Agent display name (used in menu)
- `instruction` - Main instruction file (relative to agent directory)
- `prefix` - Agent-specific prefix instructions
- `suffix` - Agent-specific suffix instructions
- `guardrails` - Agent-specific guardrails
- `outputTemplate` - Agent-specific output template
- `ignoreGlobal` - Set to `true` to ignore all global settings
- `workspaceIntegration` - Agent-specific workspace integration (overrides global)

**Path Resolution:**
- Paths in `agent.json` are resolved in this order:
  1. **Agent directory first**: `<root-folder>/agents/<agent-name>/<path>`
  2. **Global fallback**: `<root-folder>/<path>`

This allows agents to have their own files or use shared files.

## Instruction Layering (Priority Order)

When building an agent prompt, instructions are layered in this order:

1. **Global Guardrails** (highest priority, unless `ignoreGlobal: true`)
2. **Agent Guardrails**
3. **Global Prefix** (unless `ignoreGlobal: true`)
4. **Agent Prefix**
5. **Agent Main Instruction** (from `instruction` property)
6. **Global Suffix** (unless `ignoreGlobal: true`)
7. **Agent Suffix**
8. **Output Template** (agent-specific or global)

## Workspace Integration

### Global Workspace Integration
Defined in `global.config.json`, applies to all agents:

```json
"workspaceIntegration": {
  "enabled": true,
  "copilotInstructions": "instructions/copilot-instructions.md",
  "instructionFiles": [
    "instructions:instructions/*.instructions.md"
  ]
}
```

Files are resolved from `<root-folder>/`

### Agent-Specific Workspace Integration
Defined in `agent.json`, overrides global:

```json
"workspaceIntegration": {
  "enabled": true,
  "copilotInstructions": "instructions/code-review.instructions.md",
  "instructionFiles": [
    "templates:templates/code-review-template.md",
    "examples:examples/*.md"
  ]
}
```

Files are resolved from agent directory first, then global.

### File Path Prefix Format

Pattern: `<destination-folder>:<source-path>`

Examples:
- `"instructions:instructions/*.md"` ? copies to `.github/instructions/`
- `"templates/default:templates/template.md"` ? copies to `.github/templates/default/`
- `"*.md"` ? copies to `.github/` (root)

## Migration from Old Structure

### Step 1: Create Global Configuration

Create `global.config.json` in your agents directory:

**Old (agent.configuration.json):**
```json
{
  "version": "1.0",
  "global": {
    "guardrails": [...],
    "prefix": [...],
    "suffix": [...],
    "outputTemplate": "...",
    "workspaceIntegration": {...}
  }
}
```

**New (global.config.json):**
```json
{
  "version": "1.0",
  "guardrails": [...],
  "prefix": [...],
  "suffix": [...],
  "outputTemplate": "...",
  "workspaceIntegration": {...}
}
```

### Step 2: Create Agent Directories

For each agent in the old `agents` section:

**Old:**
```json
"agents": {
  "Clean Code Agent": {
    "instruction": "agents/Clean Code Agent.md",
    "prefix": [...]
  }
}
```

**New:**
1. Create directory: `agents/clean-code/`
2. Create `agent.json`:
```json
{
  "name": "Clean Code Agent",
  "instruction": "instruction.md",
  "prefix": [...]
}
```
3. Move `agents/Clean Code Agent.md` to `agents/clean-code/instruction.md`

### Step 3: Migrate Agent Resources

Move agent-specific files to agent directories:

**Before:**
```
/
??? agent.configuration.json
??? agents/
?   ??? Clean Code Agent.md
??? templates/
    ??? clean-code-template.md
```

**After:**
```
/
??? global.config.json
??? agents/
?   ??? clean-code/
?       ??? agent.json
?       ??? instruction.md
?       ??? templates/
?           ??? clean-code-template.md
```

### Step 4: Update Paths

Update file paths in agent configurations:

**Old (absolute from root):**
```json
"prefix": ["templates/clean-code-template.md"]
```

**New (relative to agent directory):**
```json
"prefix": ["templates/clean-code-template.md"]
```

Or use shared files (resolved from root):
```json
"prefix": ["standards/coding-standards.instructions.md"]
```

## Examples

### Example 1: Simple Agent (Uses Global Settings)

**agents/clean-code/agent.json:**
```json
{
  "name": "Clean Code Agent",
  "instruction": "instruction.md",
  "ignoreGlobal": false
}
```

This agent:
- Uses all global guardrails, prefix, suffix
- Uses global workspace integration
- Only adds its main instruction

### Example 2: Customized Agent

**agents/code-review/agent.json:**
```json
{
  "name": "Code Review Agent",
  "instruction": "instruction.md",
  "prefix": ["guidelines.md"],
  "suffix": ["examples.md"],
  "guardrails": ["security-guardrails.md"],
  "outputTemplate": "templates/code-review-template.md",
  "ignoreGlobal": false,
  "workspaceIntegration": {
    "enabled": true,
    "copilotInstructions": "instructions/code-review.instructions.md",
    "instructionFiles": [
      "templates:templates/*.md",
      "examples:examples/*.md"
    ]
  }
}
```

This agent:
- Uses global settings PLUS agent-specific settings
- Overrides workspace integration
- Adds custom templates and examples

### Example 3: Isolated Agent

**agents/experimental/agent.json:**
```json
{
  "name": "Experimental Agent",
  "instruction": "instruction.md",
  "prefix": ["custom-prefix.md"],
  "ignoreGlobal": true
}
```

This agent:
- Ignores ALL global settings
- Only uses agent-specific instructions
- Completely isolated configuration

## Troubleshooting

### Files Not Found

**Problem:** Instruction files not loading

**Solution:**
1. Check file paths are relative to correct directory
2. Agent-specific files ? relative to `agents/<name>/`
3. Shared files ? can use absolute from root or relative

**Debug:** Check Visual Studio Output > Debug window for file resolution messages

### Agent Not Appearing

**Problem:** Agent doesn't show in menu

**Solution:**
1. Ensure `agents/<agent-name>/agent.json` exists
2. Check JSON is valid
3. Verify `name` property is set

### Wrong Instructions Loading

**Problem:** Getting global instead of agent instructions

**Solution:**
1. Check `ignoreGlobal` is set correctly
2. Verify agent-specific files exist
3. Check path resolution (agent dir first, then global)

## Code Changes

### Files Modified
1. **AgentConfiguration.cs**
   - Restructured classes
   - Added `AgentConfig.AgentDirectory` property
   - Added `AgentConfig.WorkspaceIntegration` property
   - Separated `GlobalConfiguration` from root

2. **AgentConfigurationReader.cs**
   - Changed from single file to multi-file loading
   - Added `LoadGlobalConfiguration()`
   - Added `LoadAgentConfigurations()`
   - Updated path resolution logic
   - Enhanced `GetWorkspaceIntegrationConfig()` to support agent-specific configs

### New Files
1. **global.config.example.json** - Example global configuration
2. **agent.example.json** - Example agent configuration
3. **CONFIGURATION_REFACTORING_GUIDE.md** - This document

## Best Practices

### 1. Organize Agent Files
```
agents/
??? agent-name/
    ??? agent.json              # Configuration
    ??? instruction.md          # Main instruction
    ??? guidelines.md           # Supporting files
    ??? examples.md
    ??? templates/              # Agent-specific templates
    ?   ??? template.md
    ??? instructions/           # Additional instructions
        ??? extra.instructions.md
```

### 2. Use Descriptive Names
- Use kebab-case for directory names: `code-review`, `clean-code`
- Use clear, descriptive agent names in `name` property
- Match directory name to agent purpose

### 3. Share Common Resources
- Put organization-wide files in `org/`
- Put shared standards in `standards/`
- Put common templates in `templates/`

### 4. Version Control
- Each agent directory can be a Git submodule
- Teams can own specific agent directories
- Global config versioned separately

### 5. Documentation
- Add README.md in each agent directory
- Document agent purpose and usage
- Include examples

## Summary

The new configuration structure provides:
- ? Better organization with agent-specific directories
- ? Improved scalability for managing many agents
- ? Agent-specific workspace integration
- ? Flexible path resolution (agent-first, then global)
- ? Better collaboration with isolated agent files
- ? Backward compatible (legacy structure still supported)

The refactored system maintains all existing functionality while providing a more maintainable and scalable architecture for growing agent libraries.
