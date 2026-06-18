# Workspace Integration Configuration

## Overview
The workspace integration feature automatically copies context files from your agents repository to the solution's `.github` folder. This ensures that GitHub Copilot has access to project-specific instructions, coding standards, and guidelines.

## Configuration Section

Add a `workspaceIntegration` section to your `agent.configuration.json`:

```json
{
  "version": "1.0",
  
  "workspaceIntegration": {
    "enabled": true,
    "copilotInstructions": "copilot-instructions.md",
    "instructionFiles": [
      "instructions/*.instructions.md"
    ],
    "additionalFiles": {
      "source/path/file.md": "destination/path/file.md"
    }
  }
}
```

## Configuration Properties

### `enabled` (boolean, default: true)
Controls whether workspace integration is active.

**Example:**
```json
"enabled": true
```

### `copilotInstructions` (string, default: "copilot-instructions.md")
Path to the main copilot instructions file (relative to agents directory).

This file is copied to: `[Solution]/.github/copilot-instructions.md`

**Example:**
```json
"copilotInstructions": "copilot-instructions.md"
```

**Alternative custom path:**
```json
"copilotInstructions": "global/my-copilot-instructions.md"
```

### `instructionFiles` (array of strings)
List of file patterns to copy to the `.github/instructions/` folder.

Supports wildcards (`*`) for pattern matching.

**Examples:**

Copy all `.instructions.md` files from instructions folder:
```json
"instructionFiles": [
  "instructions/*.instructions.md"
]
```

Copy from multiple folders:
```json
"instructionFiles": [
  "instructions/*.instructions.md",
  "org/*.instructions.md",
  "standards/*.instructions.md",
  "templates/*.instructions.md"
]
```

Copy specific files:
```json
"instructionFiles": [
  "instructions/architecture.instructions.md",
  "instructions/security.instructions.md"
]
```

### `additionalFiles` (dictionary)
Custom file mappings for copying additional files to `.github` folder.

**Key**: Source file path (relative to agents directory)  
**Value**: Destination path (relative to .github folder)

**Examples:**

Copy templates:
```json
"additionalFiles": {
  "templates/default-output-template.md": "templates/default-output-template.md",
  "templates/code-review-template.md": "templates/code-review-template.md"
}
```

Copy documentation:
```json
"additionalFiles": {
  "docs/agent-guide.md": "docs/agent-guide.md",
  "README.md": "agent-readme.md"
}
```

Custom destination paths:
```json
"additionalFiles": {
  "company/standards.md": "company-standards.md",
  "policies/security-policy.md": "policies/security.md"
}
```

## Complete Example

```json
{
  "version": "1.0",

  "workspaceIntegration": {
    "enabled": true,
    "copilotInstructions": "copilot-instructions.md",
    "instructionFiles": [
      "instructions/*.instructions.md",
      "org/*.instructions.md",
      "standards/*.instructions.md"
    ],
    "additionalFiles": {
      "templates/default-output-template.md": "templates/default-output-template.md",
      "templates/code-review-template.md": "templates/code-review.md",
      "policies/security-guidelines.md": "policies/security.md",
      "README.md": "agent-readme.md"
    }
  },

  "global": {
    "guardrails": ["org/guardrails.instructions.md"],
    "prefix": ["org/architecture.instructions.md"],
    "suffix": ["standards/coding-standards.instructions.md"]
  },

  "agents": {
    "Code Review Agent": {
      "instruction": "agents/Code Review Agent.md"
    }
  }
}
```

## Resulting .github Folder Structure

With the configuration above, when you execute any agent, Shadow Pilot will create:

```
[Solution]/.github/
??? copilot-instructions.md
??? instructions/
?   ??? architecture.instructions.md
?   ??? coding-standards.instructions.md
?   ??? security.instructions.md
?   ??? testing.instructions.md
?   ??? *.instructions.md (all matching files)
??? templates/
?   ??? default-output-template.md
?   ??? code-review.md
??? policies/
?   ??? security.md
??? agent-readme.md
```

## How It Works

### 1. **Agent Invocation Triggers Copy**
When you click any agent from the Shadow Pilot menu, the workspace integration runs automatically.

### 2. **Configuration-Based Approach**
If `agent.configuration.json` exists with `workspaceIntegration` section:
- Reads the configuration
- Resolves file patterns (wildcards supported)
- Copies all specified files to `.github` folder
- Creates destination directories as needed

### 3. **Legacy Fallback**
If no configuration exists or workspace integration is disabled:
- Copies `copilot-instructions.md` if it exists
- Copies all `*.instructions.md` files from `instructions/` folder
- Uses hard-coded legacy behavior

## Use Cases

### Enterprise Standards Distribution
```json
"workspaceIntegration": {
  "enabled": true,
  "copilotInstructions": "enterprise/copilot-instructions.md",
  "instructionFiles": [
    "enterprise/*.instructions.md",
    "compliance/*.instructions.md",
    "security/*.instructions.md"
  ],
  "additionalFiles": {
    "enterprise/coding-standards.md": "standards/enterprise-coding.md",
    "enterprise/architecture-guidelines.md": "architecture.md"
  }
}
```

### Team-Specific Context
```json
"workspaceIntegration": {
  "enabled": true,
  "instructionFiles": [
    "team/backend/*.instructions.md",
    "team/frontend/*.instructions.md"
  ],
  "additionalFiles": {
    "team/onboarding.md": "team-onboarding.md",
    "team/conventions.md": "team-conventions.md"
  }
}
```

### Project-Specific Templates
```json
"workspaceIntegration": {
  "enabled": true,
  "instructionFiles": [
    "projects/microservices/*.instructions.md"
  ],
  "additionalFiles": {
    "templates/service-template.md": "templates/microservice.md",
    "templates/api-template.md": "templates/api-design.md",
    "templates/test-template.md": "templates/testing.md"
  }
}
```

## Disable Workspace Integration

To disable workspace integration (use legacy approach):

```json
"workspaceIntegration": {
  "enabled": false
}
```

Or completely omit the `workspaceIntegration` section.

## Benefits

### ? **Centralized Management**
- Single source of truth for all Copilot context files
- Easy to update across all developers and projects
- Version-controlled in agents repository

### ? **Automatic Synchronization**
- Files copied on every agent invocation
- Always up-to-date with latest instructions
- No manual file management

### ? **Flexible Organization**
- Organize files by team, department, or purpose
- Use wildcards for dynamic file selection
- Custom destination paths for any structure

### ? **Team Consistency**
- Everyone gets identical Copilot context
- Eliminates "works on my machine" issues
- Standardized AI assistance across organization

### ? **Git Integration Ready**
- `.github` folder can be gitignored or committed
- Per-project customization possible
- Easy rollback via git history

## Troubleshooting

### Files Not Copying

**Check Debug Output:**
```
View > Output > Select "Debug" from dropdown
Look for messages like:
- "Queued for copy: ..."
- "Workspace integration complete: X files copied"
```

**Common Issues:**
1. **Workspace integration disabled** - Check `"enabled": true`
2. **File not found** - Verify source paths are correct relative to agents directory
3. **Invalid patterns** - Check wildcard syntax (e.g., `*.instructions.md`)
4. **No solution open** - Workspace integration requires an open solution

### Verify Configuration
```json
{
  "workspaceIntegration": {
    "enabled": true  // Must be true
  }
}
```

### Test Individual Files
Start with a simple configuration:
```json
"workspaceIntegration": {
  "enabled": true,
  "copilotInstructions": "copilot-instructions.md",
  "instructionFiles": [
    "instructions/test.instructions.md"
  ]
}
```

Gradually add more files once basic copying works.

## Migration from Legacy Approach

### Before (No Configuration)
Files automatically copied:
- `copilot-instructions.md` ? `.github/copilot-instructions.md`
- `instructions/*.instructions.md` ? `.github/instructions/*.instructions.md`

### After (With Configuration)
Explicitly configure what to copy:
```json
"workspaceIntegration": {
  "enabled": true,
  "copilotInstructions": "copilot-instructions.md",
  "instructionFiles": [
    "instructions/*.instructions.md"
  ]
}
```

**Advantage**: Full control over which files are copied and where they go.

## Best Practices

### 1. **Use Wildcards for Scalability**
```json
"instructionFiles": [
  "instructions/*.instructions.md",  // Picks up new files automatically
  "org/*.instructions.md"
]
```

### 2. **Organize by Category**
```
AgentsRepository/
??? org/              (organization-wide)
??? standards/        (coding standards)
??? templates/        (output templates)
??? instructions/     (general instructions)
??? agents/           (agent definitions)
```

### 3. **Version Your Configuration**
Keep `agent.configuration.json` in version control to track changes.

### 4. **Test Changes Locally**
Test configuration changes on a test solution before deploying team-wide.

### 5. **Document Your Structure**
Add comments in README explaining your folder organization and file naming conventions.

## Advanced Scenarios

### Conditional File Copying Based on Project Type
While not directly supported, you can maintain multiple configuration files:
- `agent.configuration.backend.json`
- `agent.configuration.frontend.json`
- `agent.configuration.fullstack.json`

And rename/swap them as needed for different project types.

### Dynamic Content Generation
Files copied are static. For dynamic content, use instruction layering in agent definitions instead.

### Environment-Specific Instructions
```json
"additionalFiles": {
  "environments/dev.instructions.md": "environment-specific.md",
  "environments/prod.instructions.md": "production-guidelines.md"
}
```

Choose which to include based on your deployment target.

## Related Documentation

- [AgentConfigurationReader.md](AgentConfigurationReader.md) - Configuration reader usage
- [AgentConfigurationIntegration.md](AgentConfigurationIntegration.md) - Integration details
- [Overview.md](overview.md) - Shadow Pilot overview and features
