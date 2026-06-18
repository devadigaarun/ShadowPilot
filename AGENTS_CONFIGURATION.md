# Shadow Pilot Configuration Guide

## Overview
Shadow Pilot now provides a user-friendly way to configure the Agents directory path through Visual Studio's Tools > Options menu.

## How to Configure

### Method 1: Using Tools > Options (Recommended for UI)

1. **Open Visual Studio**
2. **Go to Tools > Options** (or press `Ctrl+,`)
3. **Navigate to Shadow Pilot > General**
4. **Enter the Agents Directory Path** in the text field
5. **Click OK** to save the changes

### Method 2: Using Environment Variables (Recommended for Automation)

You can also set the agents path using environment variables, which take priority over the Tools > Options setting:

- **User Environment Variable**: `AgentsPath` (specific to current user)
- **Machine Environment Variable**: `AgentsPath` (available to all users)
- **Process Environment Variable**: `AgentsPath` (current process only)

To set an environment variable:

**Windows Command Prompt:**
```cmd
setx AgentsPath "C:\path\to\agents"
```

**Windows PowerShell:**
```powershell
[Environment]::SetEnvironmentVariable("AgentsPath", "C:\path\to\agents", "User")
```

**Windows Registry:**
```
HKEY_CURRENT_USER\Environment\AgentsPath
```

## Configuration Priority (Highest to Lowest)

The Shadow Pilot extension checks for the agents path in the following order:

1. **User Environment Variable** - `AgentsPath`
2. **Machine Environment Variable** - `AgentsPath`
3. **Process Environment Variable** - `AgentsPath`
4. **Visual Studio Options** - Tools > Options > Shadow Pilot > General
5. **Default Path** - `\\inblrgh781dat.ad005.onehc.net\CTShare\Agents`

## Examples

### Example 1: Local Directory
```
C:\Agents
```

### Example 2: Network Share
```
\\server\share\agents
```

### Example 3: Network Path with Credentials
```
\\inblrgh781dat.ad005.onehc.net\CTShare\Agents
```

## Troubleshooting

If agents are not loading:

1. **Check the path exists** - Ensure the directory path is valid and accessible
2. **Check permissions** - Verify you have read access to the directory
3. **Verify file format** - Agents should be stored as `.md` (Markdown) files
4. **Check Debug Output** - Look at Visual Studio's Debug Output window for diagnostic messages
5. **Verify configuration** - Go to Tools > Options > Shadow Pilot > General and confirm the path

## Debug Output

You can monitor the agent loading process by checking the Debug Output window:

- Open View > Output (or press `Ctrl+Alt+O`)
- Select "Debug" from the dropdown
- Look for messages like:
  - `Found X .md files in [path]`
  - `Using AgentsPath from Tools > Options: [path]`
  - `Total agents loaded: X`

## Notes

- After changing the configuration, agents are automatically reloaded when you next open the Shadow Pilot menu
- Environment variables take precedence over the Tools > Options setting
- The default path is used only if no other configuration is found
- Reserved agent files (`.agent-header.md` and `.agent-footer.md`) are not displayed as individual agents but are used to wrap agent prompts
