# Configuration Refactoring Summary

## What Changed

### Old Structure (Deprecated)
**Single file:** `agent.configuration.json`
- Contains global settings and all agents
- Agents defined as dictionary entries
- All paths relative to agents root directory

### New Structure (Current)
**Multiple files:**
- `global.config.json` - Global settings
- `agents/<agent-name>/agent.json` - Individual agent configurations

## Key Benefits

1. **Scalability** - Add agents without modifying global config
2. **Modularity** - Each agent has its own directory and resources
3. **Collaboration** - Teams can own specific agents
4. **Flexibility** - Agent-specific workspace integration
5. **Maintainability** - Smaller, focused configuration files

## File Structure

```
<root-folder>/
??? global.config.json              # NEW: Global configuration
??? agents/                          # NEW: Agents directory
?   ??? clean-code/
?   ?   ??? agent.json              # NEW: Agent configuration
?   ?   ??? instruction.md
?   ?   ??? ...
?   ??? code-review/
?       ??? agent.json
?       ??? instruction.md
?       ??? ...
??? [shared resources]
```

## Quick Start

### 1. Create Global Configuration
**File:** `<root-folder>/global.config.json`

```json
{
  "version": "1.0",
  "workspaceIntegration": {
    "enabled": true,
    "copilotInstructions": "instructions/copilot-instructions.md",
    "instructionFiles": [
      "instructions:instructions/*.instructions.md"
    ]
  },
  "guardrails": ["org/guardrails.instructions.md"],
  "prefix": ["org/architecture.instructions.md"],
  "suffix": ["standards/coding-standards.instructions.md"],
  "outputTemplate": "templates/default-output-template.md"
}
```

### 2. Create Agent Directory
```bash
mkdir -p agents/my-agent
```

### 3. Create Agent Configuration
**File:** `agents/my-agent/agent.json`

```json
{
  "name": "My Agent",
  "instruction": "instruction.md",
  "ignoreGlobal": false
}
```

### 4. Create Agent Instruction
**File:** `agents/my-agent/instruction.md`

```markdown
# My Agent Instructions

You are an expert in...
```

## Path Resolution

### Global Configuration Paths
All paths in `global.config.json` are relative to `<root-folder>/`

Example:
- `"org/guardrails.instructions.md"` ? `<root-folder>/org/guardrails.instructions.md`

### Agent Configuration Paths
Paths in `agent.json` are resolved:
1. **First:** Relative to agent directory: `<root-folder>/agents/<agent-name>/<path>`
2. **Fallback:** Relative to root: `<root-folder>/<path>`

Example (`agents/my-agent/agent.json`):
- `"instruction.md"` ? `agents/my-agent/instruction.md` (agent-specific)
- `"org/guardrails.md"` ? `<root-folder>/org/guardrails.md` (shared)

## Agent-Specific Workspace Integration

Agents can override global workspace integration:

```json
{
  "name": "Code Review Agent",
  "instruction": "instruction.md",
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

Files are copied from agent directory to solution `.github` folder.

## Instruction Layering

Prompt building order (top = highest priority):

1. Global Guardrails (unless `ignoreGlobal: true`)
2. Agent Guardrails
3. Global Prefix (unless `ignoreGlobal: true`)
4. Agent Prefix
5. **Agent Main Instruction**
6. Global Suffix (unless `ignoreGlobal: true`)
7. Agent Suffix
8. Output Template (agent-specific or global)

## Configuration Properties

### Global Configuration
| Property | Type | Description |
|----------|------|-------------|
| `version` | string | Config schema version |
| `workspaceIntegration` | object | Files to copy to `.github` |
| `guardrails` | array | Organization-level guardrails |
| `prefix` | array | Instructions prepended to all agents |
| `suffix` | array | Instructions appended to all agents |
| `outputTemplate` | string | Default output template |

### Agent Configuration
| Property | Type | Description |
|----------|------|-------------|
| `name` | string | Agent display name |
| `instruction` | string | Main instruction file |
| `prefix` | array | Agent-specific prefix instructions |
| `suffix` | array | Agent-specific suffix instructions |
| `guardrails` | array | Agent-specific guardrails |
| `outputTemplate` | string | Agent-specific output template |
| `ignoreGlobal` | boolean | Ignore all global settings |
| `workspaceIntegration` | object | Agent-specific workspace integration |

## Code Changes

### Modified Files
1. **AgentConfiguration.cs**
   - Separated `GlobalConfiguration` from root
   - Added `AgentConfig.AgentDirectory` property
   - Added `AgentConfig.WorkspaceIntegration` property

2. **AgentConfigurationReader.cs**
   - Reads `global.config.json` + multiple `agent.json` files
   - Enhanced path resolution (agent-first, then global)
   - Supports agent-specific workspace integration
   - Method signatures updated to support agent name parameter

### New Example Files
- `global.config.example.json`
- `agent.example.json`
- `CONFIGURATION_REFACTORING_GUIDE.md` (full documentation)
- `CONFIGURATION_REFACTORING_SUMMARY.md` (this file)

## Testing

### Verify Configuration Loading
1. Place `global.config.json` in agents directory
2. Create agent directories with `agent.json` files
3. Open Visual Studio ? View ? Output ? Debug
4. Execute an agent
5. Check for configuration loading messages

Expected output:
```
Global configuration loaded from: D:\Agents\global.config.json
Loaded agent: Clean Code Agent from D:\Agents\agents\clean-code
Loaded agent: Code Review Agent from D:\Agents\agents\code-review
Agents loaded: 2
```

### Verify File Copying
1. Execute an agent with workspace integration
2. Check solution `.github` folder
3. Verify files were copied

Expected structure:
```
.github/
??? copilot-instructions.md
??? instructions/
    ??? *.instructions.md
```

## Migration Checklist

- [ ] Create `global.config.json` in agents directory
- [ ] Create `agents/` subdirectory
- [ ] For each agent:
  - [ ] Create agent directory: `agents/<agent-name>/`
  - [ ] Create `agent.json` with configuration
  - [ ] Move/create `instruction.md`
  - [ ] Move agent-specific resources to agent directory
- [ ] Test configuration loading
- [ ] Test agent execution
- [ ] Test workspace integration
- [ ] Remove old `agent.configuration.json` (optional backup)

## Troubleshooting

### Configuration Not Loading
**Check:**
- `global.config.json` exists in agents directory
- JSON syntax is valid
- File encoding is UTF-8

### Agent Not Found
**Check:**
- Agent directory exists under `agents/`
- `agent.json` exists in agent directory
- `name` property is set in `agent.json`

### Files Not Copying
**Check:**
- Workspace integration is enabled
- File paths are correct (relative to agent or root)
- Files exist at specified paths
- Check debug output for error messages

## Support

For detailed information, see:
- **CONFIGURATION_REFACTORING_GUIDE.md** - Complete documentation
- **global.config.example.json** - Example global configuration
- **agent.example.json** - Example agent configuration

---

**Version:** 2.0.0  
**Last Updated:** 2026-01-XX
