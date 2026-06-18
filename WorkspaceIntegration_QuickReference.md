# Workspace Integration - Quick Reference

## Add to agent.configuration.json

```json
{
  "version": "1.0",

  "global": {
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

  "agents": { }
}
```

## Properties

| Property | Type | Description | Default |
|----------|------|-------------|---------|
| `enabled` | boolean | Enable/disable workspace integration | `true` |
| `copilotInstructions` | string | Path to copilot instructions file | `"copilot-instructions.md"` |
| `instructionFiles` | array | File patterns to copy to `.github/` with optional folder prefix | `[]` |

## File Pattern Format

Supports two formats:
1. **Simple path**: `"instructions/*.instructions.md"` - copies to `.github/` preserving folder structure
2. **Prefixed path**: `"instructions:instructions/*.instructions.md"` - copies to `.github/instructions/`

## Wildcard Patterns

| Pattern | Matches |
|---------|---------|
| `*.md` | All `.md` files |
| `*.instructions.md` | All `.instructions.md` files |
| `instructions/*.md` | All `.md` files in `instructions/` |
| `org/*.instructions.md` | All `.instructions.md` in `org/` |

## Result

Files are copied to `[Solution]/.github/`:

```
.github/
??? copilot-instructions.md
??? instructions/
    ??? *.instructions.md files
```

## Examples

### Minimal
```json
"global": {
  "workspaceIntegration": {
    "enabled": true
  }
}
```

### Standard
```json
"global": {
  "workspaceIntegration": {
    "enabled": true,
    "instructionFiles": [
      "instructions:instructions/*.instructions.md"
    ]
  }
}
```

### Advanced
```json
"global": {
  "workspaceIntegration": {
    "enabled": true,
    "copilotInstructions": "global/instructions.md",
    "instructionFiles": [
      "instructions:instructions/*.instructions.md",
      "instructions:org/*.instructions.md",
      "instructions:standards/*.instructions.md",
      "templates:templates/*.instructions.md"
    ]
  }
}
```

## Disable Integration

```json
"global": {
  "workspaceIntegration": {
    "enabled": false
  }
}
```

Or omit the section entirely to use legacy behavior.

## Debug Output

View > Output > Debug

```
Using configuration-based workspace integration
Queued for copy: copilot-instructions.md -> copilot-instructions.md
Queued for copy: instructions/*.instructions.md matched architecture.instructions.md -> instructions/architecture.instructions.md
Created directory: D:\projects\MySolution\.github\instructions
Copied: architecture.instructions.md -> instructions/architecture.instructions.md
Workspace integration complete: 2 files copied
```

## Troubleshooting

**Files not copying?**
- Check `"enabled": true` inside `global.workspaceIntegration`
- Verify file paths exist
- Check debug output
- Ensure solution is open

**Pattern not matching?**
- Use correct wildcard syntax
- Check folder names
- Verify file extensions

**Legacy behavior?**
- Configuration might not be loaded
- Check `agent.configuration.json` exists in agents directory
- Verify JSON is valid

## See Also

- [WorkspaceIntegration_Configuration.md](WorkspaceIntegration_Configuration.md) - Full documentation
- [WorkspaceIntegration_Feature_Summary.md](WorkspaceIntegration_Feature_Summary.md) - Feature details
- [agent.configuration.example.json](agent.configuration.example.json) - Example configuration
