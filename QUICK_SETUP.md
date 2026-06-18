# Shadow Pilot Quick Setup

## For End Users

### To Configure Agents Path in Visual Studio:

1. **Tools ? Options**
2. **Search for or navigate to: Shadow Pilot ? General**
3. **Enter your Agents Directory Path**
4. **Click OK**

### Default Path:
```
\\inblrgh781dat.ad005.onehc.net\CTShare\Agents
```

### Supported Path Types:
- ? Local directories: `C:\Agents`
- ? Network shares: `\\server\share\agents`
- ? UNC paths: `\\hostname\share\path`

---

## For Administrators / Automation

### Setting via Environment Variables (Preferred):

**PowerShell (All Users):**
```powershell
[Environment]::SetEnvironmentVariable("AgentsPath", "YOUR_PATH_HERE", "Machine")
```

**PowerShell (Current User Only):**
```powershell
[Environment]::SetEnvironmentVariable("AgentsPath", "YOUR_PATH_HERE", "User")
```

**Command Prompt:**
```cmd
setx AgentsPath "YOUR_PATH_HERE"
setx /M AgentsPath "YOUR_PATH_HERE"  (for all users)
```

### Agent File Format:
- Store agent definitions as `.md` files
- Filename becomes the agent name (e.g., `SecurityReview.md` = "SecurityReview" agent)
- Reserved files: `.agent-header.md`, `.agent-footer.md` (not shown as individual agents)

---

## Verification

Check the Visual Studio **Debug Output** window (View ? Output) to verify:
- Agent files found: `Found X .md files in [path]`
- Total agents loaded: `Total agents loaded: X`
