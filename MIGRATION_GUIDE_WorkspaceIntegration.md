# Migration Guide: WorkspaceIntegration Moved to Global

## Overview
The `workspaceIntegration` configuration has been moved from the root level to inside the `global` configuration section for better organization and consistency.

## What Changed

### Before (Old Structure)
```json
{
  "version": "1.0",
  
  "workspaceIntegration": {
    "enabled": true,
    "copilotInstructions": "copilot-instructions.md",
    "instructionFiles": [
      "instructions:instructions/*.instructions.md"
    ]
  },
  
  "global": {
    "guardrails": [...],
    "prefix": [...],
    "suffix": [...]
  },
  
  "agents": {...}
}
```

### After (New Structure)
```json
{
  "version": "1.0",
  
  "global": {
    "guardrails": [...],
    "prefix": [...],
    "suffix": [...],
    
    "workspaceIntegration": {
      "enabled": true,
      "copilotInstructions": "copilot-instructions.md",
      "instructionFiles": [
        "instructions:instructions/*.instructions.md"
      ]
    }
  },
  
  "agents": {...}
}
```

## Why This Change?

1. **Logical Organization** - Workspace integration is a global setting that applies to all agents, so it belongs in the `global` section
2. **Consistency** - All global settings (guardrails, prefix, suffix, outputTemplate) are now in one place
3. **Cleaner Structure** - Separates root-level metadata (version) from configuration settings
4. **Better Semantics** - Makes it clear that workspace integration is a global behavior

## Migration Steps

### Step 1: Update Your agent.configuration.json

**Move the entire `workspaceIntegration` section** from root level into the `global` section:

**Old:**
```json
{
  "workspaceIntegration": { ... },
  "global": { ... }
}
```

**New:**
```json
{
  "global": {
    "workspaceIntegration": { ... }
  }
}
```

### Step 2: Verify Configuration

Check that your configuration is valid JSON:
- Ensure proper comma placement
- Check bracket nesting
- Validate with a JSON validator

### Step 3: Test

1. Open Visual Studio
2. Execute an agent
3. Check debug output (View ? Output ? Debug)
4. Verify files are copied to `.github` folder

## Code Changes

### Files Modified

1. **AgentConfiguration.cs**
   - Removed `WorkspaceIntegration` property from `AgentConfiguration` class
   - Added `WorkspaceIntegration` property to `GlobalConfiguration` class

2. **AgentConfigurationReader.cs**
   - Updated `GetWorkspaceIntegrationConfig()` to access via `configuration?.Global?.WorkspaceIntegration`

3. **agent.configuration.example.json**
   - Updated example to show new structure

4. **WorkspaceIntegration_QuickReference.md**
   - Updated all examples to show new structure

## Backward Compatibility

?? **Breaking Change** - This is a breaking change. Old configuration files will need to be updated.

**Old configurations will NOT work** until migrated to the new structure.

## Example Configurations

### Minimal Configuration
```json
{
  "version": "1.0",
  "global": {
    "workspaceIntegration": {
      "enabled": true
    }
  },
  "agents": {}
}
```

### Standard Configuration
```json
{
  "version": "1.0",
  "global": {
    "prefix": ["org/architecture.instructions.md"],
    "workspaceIntegration": {
      "enabled": true,
      "copilotInstructions": "copilot-instructions.md",
      "instructionFiles": [
        "instructions:instructions/*.instructions.md"
      ]
    }
  },
  "agents": {}
}
```

### Full Configuration
```json
{
  "version": "1.0",
  "global": {
    "guardrails": ["org/guardrails.instructions.md"],
    "prefix": [
      "org/architecture.instructions.md",
      "org/security.instructions.md"
    ],
    "suffix": ["standards/coding-standards.instructions.md"],
    "outputTemplate": "templates/default-output-template.md",
    "workspaceIntegration": {
      "enabled": true,
      "copilotInstructions": "copilot-instructions.md",
      "instructionFiles": [
        "instructions:instructions/*.instructions.md",
        "instructions:org/*.instructions.md",
        "instructions:standards/*.instructions.md"
      ]
    }
  },
  "agents": {
    "Clean Code Agent": {
      "instruction": "agents/Clean Code Agent.md"
    }
  }
}
```

## Troubleshooting

### Files Not Copying?

**Check configuration location:**
```json
// ? Wrong - at root level
{
  "workspaceIntegration": { ... }
}

// ? Correct - inside global
{
  "global": {
    "workspaceIntegration": { ... }
  }
}
```

### Debug Output Shows "Workspace integration is disabled or not configured"

This means:
1. `workspaceIntegration` is still at root level (old structure)
2. Configuration file wasn't loaded
3. `enabled` is set to `false`

**Solution:** Move `workspaceIntegration` into `global` section

### JSON Parse Errors

Common mistakes:
- Missing comma after previous property
- Extra comma before closing brace
- Incorrect bracket nesting

**Use a JSON validator** to check syntax.

## Benefits of New Structure

? **Better Organization** - All global settings in one place  
? **Clearer Semantics** - Obvious that workspace integration is global  
? **Easier to Navigate** - Logical grouping of related settings  
? **Consistent Pattern** - Matches other global configuration properties  

## Support

If you encounter issues during migration:
1. Check debug output (View ? Output ? Debug)
2. Validate JSON syntax
3. Compare with `agent.configuration.example.json`
4. Review this migration guide

## Summary

- **Action Required:** Move `workspaceIntegration` from root to inside `global`
- **Breaking Change:** Old configurations won't work
- **Migration Time:** ~2 minutes
- **Benefit:** Better organized, more logical configuration structure

---

*Last Updated: 2026-01-XX*
