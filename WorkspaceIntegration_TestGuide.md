# Workspace Integration - Configuration Test Guide

## Current Configuration

Your `agent.configuration.json` is configured as follows:

```json
{
  "workspaceIntegration": {
    "enabled": true,
    "copilotInstructions": "instructions/copilot-instructions.md",
    "instructionFiles": [
      "instructions/agent.instructions.md"
    ],
    "additionalFiles": {
      "templates/default-output-template.md": "templates/default-output-template.md"
    }
  }
}
```

## What Files Will Be Copied

### Agents Repository Structure (Source)
```
D:\agent-repository\
??? instructions/
?   ??? copilot-instructions.md        ? Source for copilotInstructions
?   ??? agent.instructions.md          ? Source for instructionFiles
??? templates/
?   ??? default-output-template.md     ? Source for additionalFiles
??? agent.configuration.json
```

### Solution .github Folder Structure (Destination)

When you execute any agent, these files will be copied to your solution:

```
[YourSolution]/.github/
??? copilot-instructions.md            ? From instructions/copilot-instructions.md
??? instructions/
?   ??? agent.instructions.md          ? From instructions/agent.instructions.md
??? templates/
    ??? default-output-template.md     ? From templates/default-output-template.md
```

## File Mapping Details

| Source Path (Agents Repository) | Destination Path (Solution .github) | Type |
|----------------------------------|--------------------------------------|------|
| `instructions/copilot-instructions.md` | `copilot-instructions.md` | Copilot Instructions |
| `instructions/agent.instructions.md` | `instructions/agent.instructions.md` | Instruction File |
| `templates/default-output-template.md` | `templates/default-output-template.md` | Additional File |

## How It Works

### 1. **copilotInstructions**
```json
"copilotInstructions": "instructions/copilot-instructions.md"
```
- **Source**: `D:\agent-repository\instructions\copilot-instructions.md`
- **Destination**: `[Solution]\.github\copilot-instructions.md`
- **Note**: Always copied to root of `.github` folder (filename preserved as `copilot-instructions.md`)

### 2. **instructionFiles**
```json
"instructionFiles": [
  "instructions/agent.instructions.md"
]
```
- **Source**: `D:\agent-repository\instructions\agent.instructions.md`
- **Destination**: `[Solution]\.github\instructions\agent.instructions.md`
- **Note**: Copied to `instructions/` subfolder in `.github`

### 3. **additionalFiles**
```json
"additionalFiles": {
  "templates/default-output-template.md": "templates/default-output-template.md"
}
```
- **Source**: `D:\agent-repository\templates\default-output-template.md`
- **Destination**: `[Solution]\.github\templates\default-output-template.md`
- **Note**: Uses custom source ? destination mapping

## Testing Steps

### 1. Verify Source Files Exist

Check that these files exist in your agents repository:
```
D:\agent-repository\instructions\copilot-instructions.md
D:\agent-repository\instructions\agent.instructions.md
D:\agent-repository\templates\default-output-template.md
```

### 2. Open a Solution in Visual Studio

### 3. Execute Any Agent
- Click on any agent from the Shadow Pilot menu (e.g., "Clean Code Agent")

### 4. Check Debug Output
View > Output > Select "Debug" from dropdown

Expected output:
```
Using configuration-based workspace integration
Queued for copy: instructions/copilot-instructions.md -> copilot-instructions.md
Matched specific file: instructions/agent.instructions.md
Queued for copy: instructions/agent.instructions.md matched agent.instructions.md -> instructions/agent.instructions.md
Queued for copy: templates/default-output-template.md -> templates/default-output-template.md
Total files queued for copy: 3
Created directory: [YourSolution]\.github
Copied: copilot-instructions.md -> copilot-instructions.md
Created directory: [YourSolution]\.github\instructions
Copied: agent.instructions.md -> instructions/agent.instructions.md
Created directory: [YourSolution]\.github\templates
Copied: default-output-template.md -> templates/default-output-template.md
Workspace integration complete: 3 files copied
```

### 5. Verify Files Were Copied

Navigate to your solution's `.github` folder and verify:
- `copilot-instructions.md` exists
- `instructions/agent.instructions.md` exists
- `templates/default-output-template.md` exists

## Code Changes Summary

### Enhanced `GetMatchingFiles()` Method

**Before**: Only handled wildcard patterns
**After**: Handles both specific files AND wildcard patterns

```csharp
private List<string> GetMatchingFiles(string pattern)
{
    // Check if this is a specific file (no wildcards)
    if (!pattern.Contains("*") && !pattern.Contains("?"))
    {
        if (File.Exists(fullPattern))
        {
            matchedFiles.Add(fullPattern);
        }
    }
    else
    {
        // Handle wildcard patterns
        var files = Directory.GetFiles(directory, searchPattern);
        matchedFiles.AddRange(files);
    }
}
```

**Benefits**:
- ? Supports specific files: `instructions/agent.instructions.md`
- ? Supports wildcards: `instructions/*.instructions.md`
- ? Supports both in same configuration
- ? Better debug logging

## Alternative Configuration Examples

### Example 1: Using Wildcards
```json
"instructionFiles": [
  "instructions/*.instructions.md"
]
```
**Result**: Copies ALL `.instructions.md` files from `instructions/` folder

### Example 2: Multiple Specific Files
```json
"instructionFiles": [
  "instructions/agent.instructions.md",
  "instructions/architecture.instructions.md",
  "instructions/security.instructions.md"
]
```
**Result**: Copies only the 3 specified files

### Example 3: Mix of Specific and Wildcards
```json
"instructionFiles": [
  "instructions/agent.instructions.md",
  "org/*.instructions.md",
  "standards/*.instructions.md"
]
```
**Result**: Copies the specific file PLUS all wildcard matches

## Troubleshooting

### File Not Copied

**Symptom**: Expected file missing from `.github` folder

**Check**:
1. Source file exists: `D:\agent-repository\instructions\copilot-instructions.md`
2. Configuration path is correct (no typos)
3. Debug output shows "Queued for copy" message
4. Debug output shows "Copied" confirmation

### Directory Not Created

**Symptom**: Destination directory missing

**Check**:
1. Debug output for "Created directory" messages
2. File permissions on solution directory
3. Solution is open in Visual Studio

### Wrong File Content

**Symptom**: File copied but contains unexpected content

**Check**:
1. Source file in agents repository
2. File hasn't been cached or locked
3. No intermediate file modifications

## Success Criteria

? All 3 files copied successfully  
? Correct folder structure created in `.github`  
? Debug output shows all operations  
? No error messages in Debug output  
? Files contain expected content  

## Next Steps

1. **Test the configuration** - Execute any agent and verify files are copied
2. **Add more files** - Extend `instructionFiles` or `additionalFiles` as needed
3. **Use wildcards** - Replace specific file with `*.instructions.md` to copy all files automatically
4. **Deploy to team** - Share the configuration file with your team

## Support

If you encounter issues:
1. Check Debug Output (View > Output > Debug)
2. Verify source file paths
3. Check file permissions
4. Review [WorkspaceIntegration_Configuration.md](WorkspaceIntegration_Configuration.md) for detailed documentation
