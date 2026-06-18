# 🚀 ShadowPilot - AI Agent Orchestration for Visual Studio

**Transform GitHub Copilot into a team of specialized AI agents** — instantly accessible from your Visual Studio IDE.

> 📖 **Want to brief overview?** Read the [Breif Overview](overview.md).
> 📖 **Want to dive deeper?** Read the [Conceptual Design Document](concept.md) for detailed architecture, design philosophy, and future vision.

---

## 📑 Topics

- [Why ShadowPilot?](#-why-shadowpilot)
  - [The Problem](#the-problem)
  - [The Solution](#the-solution-shadowpilot)
- [Key Benefits](#-key-benefits)
  - [Productivity Multiplier](#-productivity-multiplier)
  - [Specialized AI Agents](#-specialized-ai-agents)
  - [Seamless Integration](#-seamless-integration)
  - [Team Enablement](#-team-enablement)
- [How It Works](#️-how-it-works)
  - [Architecture](#architecture)
  - [Technical Approach](#technical-approach)
- [Features](#-features)
- [Getting Started](#-getting-started)
  - [Installation](#installation)
  - [Setup](#setup)
  - [Creating Your First Agent](#creating-your-first-agent)
- [Use Cases](#-use-cases)
- [Comparison](#-comparison)
- [Best Practices](#-best-practices)
- [Technical Details](#-technical-details)
- [Contributing](#-contributing)
- [License](#-license)
- [Acknowledgments](#-acknowledgments)

---

## 🎯 Why ShadowPilot?

### The Problem
While GitHub Copilot is powerful, it has significant limitations that hamper developer productivity:

#### **GitHub Copilot Limitations:**
- **No Persistent Context** - Every chat session starts fresh; you lose your carefully crafted instructions
- **Repetitive Prompting** - You have to retype the same role definitions and guidelines repeatedly
- **No Agent Specialization** - Single generic assistant that tries to do everything
- **Manual Context Setting** - No quick way to switch between different AI personas or workflows
- **Limited Workflow Automation** - Can't predefine complex instruction sets for recurring tasks

#### **Microsoft Visual Studio Limitations:**
- **No Built-in Agent System** - VS lacks native support for multiple AI agent profiles
- **No Template Management** - No mechanism to save and reuse AI instruction templates
- **Limited Extensibility** - Copilot integration doesn't expose agent customization APIs
- **One-Size-Fits-All Approach** - Cannot adapt to different development scenarios automatically

### 😫 Pain Points: Using Agent Mode Without ShadowPilot

#### **1. The Prompt Fatigue Problem**
**Scenario**: You want to use Copilot as a code reviewer every day.

**Without ShadowPilot**:
```
Day 1: Type 300-word prompt defining reviewer role, standards, focus areas...
Day 2: Realize you forgot to save it. Type it again (slightly different).
Day 3: Copy from yesterday's chat. Paste. Copilot says "Session expired."
Day 4: Keep prompt in notepad. Copy-paste. Format breaks. Fix format.
Day 5: Team member asks "What prompt do you use?" Send via email.
Day 6: Prompt evolves. Now you have 3 versions floating around.
```

**Result**: 5-10 minutes wasted EVERY session. Inconsistent results. Team fragmentation.

**With ShadowPilot**: Click **Extensions → Code Reviewer**. Done in 2 seconds.

---

#### **2. The Context Switching Nightmare**
**Scenario**: Working on multiple tasks requiring different AI expertise.

**Without ShadowPilot**:
```
Morning:   Writing unit tests → Type test expert prompt
Afternoon: Code review → Type reviewer prompt  
Evening:   Documentation → Type docs writer prompt
Next Day:  What were those exact prompts again? 🤔
```

**Reality Check**:
- Lost prompts in chat history
- Inconsistent prompt variations
- Mental overhead remembering what worked
- Can't quickly switch between "modes"

**With ShadowPilot**: 
- **Extensions → Test Expert** (morning)
- **Extensions → Code Reviewer** (afternoon)
- **Extensions → Docs Writer** (evening)
- Same prompts, every time, forever.

---

#### **3. The Team Alignment Crisis**
**Scenario**: 10 developers need consistent AI-assisted code reviews.

**Without ShadowPilot**:
```
Developer A: Uses brief 50-word prompt (misses security checks)
Developer B: Uses detailed 400-word prompt (checks everything)
Developer C: Uses outdated prompt (old coding standards)
Developer D: Doesn't use Copilot for reviews at all
Developer E: Invents their own prompt daily

Code Review Results:
✅ Inconsistent quality
✅ Different standards applied
✅ Knowledge silos
✅ Onboarding chaos for new team members
✅ No way to enforce team practices
```

**With ShadowPilot**:
- Tech lead creates **TeamCodeReviewer.md** once
- Commits to repo: `git add Agents/TeamCodeReviewer.md`
- Everyone clones, gets instant access
- Updates propagate automatically
- Uniform quality across entire team

---

#### **4. The Lost Expertise Problem**
**Scenario**: Your senior developer created the perfect security audit prompt.

**Without ShadowPilot**:
```
Week 1: Senior dev crafts amazing security audit prompt (2 hours of work)
Week 2: Junior dev asks "How do I check for SQL injection?"
         Senior: "I have a great prompt for that!" 
         *Searches through 50 Copilot chat sessions*
         "Uh... I'll get back to you"
Week 3: Senior dev leaves company
Week 4: Prompt knowledge = GONE FOREVER 💨
```

**With ShadowPilot**:
- Prompt saved as `SecurityAuditor.md`
- Version controlled in Git
- Accessible to entire team forever
- New team members benefit from day 1
- Expertise preserved as organizational knowledge

---

#### **5. The Workflow Interruption Issue**
**Scenario**: You're in the zone, need to switch AI context.

**Without ShadowPilot - The 11-Step Dance**:
1. Stop coding
2. Open Copilot Chat
3. Find your prompt notes
4. Copy the right prompt
5. Paste into chat
6. Fix formatting issues
7. Send prompt
8. Wait for response
9. Realize you used the wrong prompt
10. Start over
11. Lost your train of thought 🧠💥

**Time Lost**: 3-7 minutes per switch  
**Focus Lost**: Every single time  
**Frustration**: Maximum

**With ShadowPilot - The 1-Step Flow**:
1. Click **Extensions → [Agent Name]**

**Time Lost**: 2 seconds  
**Focus Lost**: None  
**Frustration**: Zero

---

#### **6. The Version Control Gap**
**Scenario**: Your team's AI prompts need to evolve with your coding standards.

**Without ShadowPilot**:
```
❌ Prompts stored in: Slack, Email, OneNote, Sticky notes, Developer's memory
❌ No change history
❌ No code reviews for prompts
❌ No rollback capability
❌ No branching strategy
❌ No CI/CD integration
❌ Can't see who changed what or why
```

**With ShadowPilot**:
```bash
✅ git log Agents/CodeReviewer.md
✅ git diff Agents/CodeReviewer.md
✅ git blame Agents/CodeReviewer.md
✅ Pull request reviews for prompt changes
✅ Branch-specific agents (feature/new-standards)
✅ Tag releases (v1.0-prompts)
✅ Full audit trail
```

---

#### **7. The Discoverability Problem**
**Scenario**: New team member wants to use AI effectively.

**Without ShadowPilot**:
```
New Dev: "What AI prompts does the team use?"
Mentor:  "Oh, we have some good ones somewhere..."
         "Check the wiki" (outdated)
         "Ask Sarah" (she's on vacation)
         "Look in Slack" (1000+ messages)
         "I'll send you mine" (sends 1 of 15)

Result: New dev reinvents the wheel, poorly.
```

**With ShadowPilot**:
```
New Dev: Opens Visual Studio
         Extensions → Copilot Agents
         *Sees entire library instantly*
         CodeReviewer, TestExpert, SecurityAuditor, 
         DocumentationWriter, RefactoringHelper...

Result: Productive from day 1, learns team practices organically.
```

---

#### **8. The Prompt Maintenance Burden**
**Scenario**: Your coding standards change (new .NET version, new security requirements).

**Without ShadowPilot**:
```
Old Standard: ".NET Framework 4.7.2"
New Standard: ".NET 8 with modern patterns"

Update Process:
1. Notify team via email/Slack
2. Hope everyone updates their personal prompts
3. Some do, some don't
4. No way to verify
5. Mixed results for months
6. Inconsistent AI guidance across team
```

**With ShadowPilot**:
```bash
git pull origin main  # Everyone gets updated agents
```

Single source of truth. Instant propagation. Guaranteed consistency.

### The Solution: ShadowPilot

**ShadowPilot bridges the gap** between GitHub Copilot's capabilities and real-world development needs by introducing:

✅ **Persistent Agent Definitions** - Store your specialized AI agents as markdown files  
✅ **One-Click Agent Switching** - Instantly activate pre-configured AI experts  
✅ **Workflow Automation** - Launch complex instruction sets with a single command  
✅ **Team Collaboration** - Share agent definitions across your development team  
✅ **Infinite Customization** - Create unlimited specialized agents for any scenario  

**Bottom Line**: Stop fighting your tools. Start using AI the way it should work.

---

## ✨ Key Benefits

### 🚀 **Productivity Multiplier**
- **Save 5-10 minutes per session** by eliminating repetitive prompt setup
- **Switch contexts instantly** instead of manually retyping instructions
- **Maintain consistency** across your team with shared agent definitions

### 🎭 **Specialized AI Agents**
Transform Copilot into a team of experts:
- 🔍 **Code Reviewer** - Enforces your team's coding standards
- 🏗️ **Architecture Advisor** - Provides design pattern guidance
- 📝 **Documentation Writer** - Generates consistent documentation
- 🧪 **Test Case Generator** - Creates comprehensive test scenarios
- 🔧 **Refactoring Assistant** - Modernizes legacy code
- 🐛 **Debug Expert** - Analyzes complex issues systematically
- 🔐 **Security Auditor** - Reviews code for vulnerabilities

### 🔄 **Seamless Integration**
- **Zero Learning Curve** - Works exactly like native Copilot
- **Non-Intrusive** - Doesn't modify VS or Copilot's core functionality
- **Lightweight** - Simple extension with minimal overhead
- **File-Based Configuration** - Easy to version control and share

### 👥 **Team Enablement**
- **Standardize AI Interactions** - Everyone uses the same proven prompts
- **Knowledge Sharing** - Best practices encoded in agent files
- **Onboarding Acceleration** - New team members get instant access to expert agents
- **Quality Consistency** - Uniform code review and documentation standards

---

## 🛠️ How It Works

### Architecture

```
Visual Studio Extension
       ↓
   ShadowPilot
       ↓
   Loads Agents from AgentsPath environment variable
   (Default: \\inblrgh781dat.ad005.onehc.net\CTShare\Agents)
       ↓
   Scans for *.md files (excluding .agent-header.md and .agent-footer.md)
       ↓
   Generates dynamic menu under Tools → Shadow Pilot
       ↓
   User clicks Import Agents (refreshes agent list)
   OR
   User selects an agent
       ↓
   Auto-copies copilot-instructions.md to solution/.github/
   Auto-copies *.instructions.* files to solution/.github/instructions/
       ↓
   Builds final prompt: [Header] + [Agent Instructions] + [Footer]
       ↓
   Opens Copilot Chat and injects instructions
       ↓
   GitHub Copilot responds with specialized context

```

### Technical Approach

**Key Components:**
- **Agent Manifest** (`*.md` files): Define your AI agents
- **Header/Footer Templates** (`.agent-header.md`, `.agent-footer.md`): Global instructions
- **ShadowPilot Extension**: Integrates with Visual Studio and manages agents

**Agent Manifest Structure:**
```markdown
# You are a [Agent Role]

You are an expert [Agent Specialty] with [X]+ years of experience.

## Your responsibilities:
- [Responsibility 1]
- [Responsibility 2]
- [Responsibility 3]

## Guidelines:
- [Guideline 1]
- [Guideline 2]
- [Guideline 3]
```

**Creating Agents:**
1. Define your agent in a new `*.md` file
2. Include **role**, **responsibilities**, and **guidelines**
3. Save the file in your agents directory
4. In Visual Studio, click **Tools → Shadow Pilot → Import Agents**
5. Your agent is now available in the Shadow Pilot menu

**Using Header and Footer Templates:**
- Create global instructions that apply to all agents using special files:

#### `.agent-header.md` (Optional)
Content prepended to all agent instructions:

```markdown
# General Context
You are assisting a developer working in Visual Studio on a .NET project.

## Always:
- Provide clear, actionable responses
- Use markdown formatting
- Include code examples where relevant
- Follow C# and .NET best practices
```

#### `.agent-footer.md` (Optional)
Content appended to all agent instructions:

```markdown
# Additional Guidelines
- Use latest C# language features when applicable
- Follow Microsoft coding conventions
- Consider performance and security
- Suggest appropriate design patterns

## Code Style
- Use PascalCase for public members
- Use camelCase for private fields
- Add XML documentation comments
```

**Final Prompt Structure:**
```
[.agent-header.md content]

[Your agent-specific instructions from CodeReviewer.md]

[.agent-footer.md content]
```

This allows you to maintain consistent global guidelines while each agent provides specialized instructions.

**The "Import Agents" Menu Item**
- The first item in the Shadow Pilot menu is always **Import Agents**. This special item:

- ✅ **Refreshes the agent list** from your configured directory
- ✅ **Shows a confirmation message** with the count of loaded agents
- ✅ **Does NOT invoke Copilot Chat** - it's purely for management
- ✅ **Reloads modified agent files** without restarting Visual Studio

**When to use Import Agents:**
- After adding new `.md` files to your agents folder
- After modifying existing agent instructions
- To verify your agents directory is accessible
- To see how many agents are currently available

**Example workflow:**
```
1. Edit CodeReviewer.md (add new guideline)
2. Tools → Shadow Pilot → Import Agents
3. See: "Agents refreshed successfully. 12 agent(s) loaded."
4. Tools → Shadow Pilot → CodeReviewer (now uses updated instructions)


```

## 🔧 Technical Details

### Requirements
- Visual Studio 2019 or later
- GitHub Copilot extension installed and signed in
- .NET Framework 4.7.2 or later
- Windows OS

### Supported Configurations
- **Local Drives**: `C:\Agents`, `D:\Agents`, etc.
- **Network Shares**: `\\server\share\Agents` (UNC paths)
- **Mapped Drives**: Any accessible drive letter

### Environment Variables
- **`AgentsPath`** (User/Machine/Process scope)
  - Specifies the directory containing agent `.md` files
  - Checked in order: User → Machine → Process
  - Falls back to default network path if not set

### Reserved Filenames
The following `.md` files have special purposes:
- **`.agent-header.md`** - Prepended to all agent instructions
- **`.agent-footer.md`** - Appended to all agent instructions
- **`copilot-instructions.md`** - Auto-copied to solution `.github/` folder

These files won't appear as menu items.

### Automatic Workspace Integration
When you invoke an agent, ShadowPilot automatically prepares your solution for GitHub Copilot:

#### **copilot-instructions.md Auto-Copy**
- **Source**: `AgentsPath/copilot-instructions.md`
- **Destination**: `[Solution]/.github/copilot-instructions.md`
- **Purpose**: Provides global context to GitHub Copilot for your entire solution
- **Behavior**: Copied automatically when any agent is invoked (overwrites existing)

#### **Instruction Files Auto-Copy**
- **Source**: `AgentsPath/instructions/*.instructions.*` (case-insensitive)
- **Destination**: `[Solution]/.github/instructions/`
- **Purpose**: Additional context files for specialized Copilot behaviors
- **Behavior**: All files containing `.instructions` in their name are copied
- **Examples**:
  - `coding.instructions.md` → `.github/instructions/coding.instructions.md`
  - `testing.Instructions.txt` → `.github/instructions/testing.Instructions.txt`
  - `security.INSTRUCTIONS.json` → `.github/instructions/security.INSTRUCTIONS.json`

#### **Benefits**
✅ **Zero Manual Setup** - No need to manually copy files to each solution  
✅ **Always Up-to-Date** - Latest instructions from central repository  
✅ **Team Consistency** - Everyone gets the same Copilot context  
✅ **Solution-Specific** - Each open solution gets its own `.github` folder  
✅ **Git-Friendly** - `.github` folder can be committed or gitignored as needed

#### **File Structure Example**
```
YourSolution/
├── .github/
│   ├── copilot-instructions.md          (auto-copied)
│   └── instructions/
│       ├── coding.instructions.md       (auto-copied)
│       ├── testing.instructions.md      (auto-copied)
│       └── security.instructions.md     (auto-copied)
├── YourProject/
└── YourSolution.sln
```

#### **Debug Logging for File Operations**
```
Agents directory: C:\MyAgents
Solution directory: C:\Projects\MyApp
Copied copilot-instructions.md to: C:\Projects\MyApp\.github\copilot-instructions.md
Created instructions folder: C:\Projects\MyApp\.github\instructions
Copied instruction file: coding.instructions.md
Copied instruction file: testing.instructions.md
Copied 2 instruction files to: C:\Projects\MyApp\.github\instructions
```

### Menu Capacity
- **Maximum Agents**: 19 (20 menu items total, minus 1 for "Import Agents")
- Configurable via `MaxMenuItems` constant in `DynamicAgentCommand.cs`

### Extension Architecture
- **ShadowPilotPackage.cs** - Main VS package initialization and service provider
- **DynamicAgentCommand.cs** - Core logic:
  - Agent loading from file system
  - Dynamic menu generation
  - Copilot Chat integration
  - Header/footer template support
  - Automatic workspace file management (copilot-instructions.md and instruction files)
- **AgentCommand.cs** - Parent menu command handler
- **ShadowPilotPackage.vsct** - Menu structure definition (XML)

### Auto-Refresh Behavior
- Agents are automatically reloaded when the Shadow Pilot menu is opened
- No need to restart Visual Studio after modifying agent files
- "Import Agents" provides manual refresh with confirmation

### Debug Logging
To view detailed operation logs:
1. Open **View → Output** in Visual Studio
2. Select **Debug** from "Show output from:" dropdown
3. Open Shadow Pilot menu to see:
   ```
   Found 15 .md files in C:\MyAgents
   Processing: CodeReviewer
     Added to Agents list
   Processing: .agent-header
     Skipped (reserved name)
   Total agents loaded: 13
   UpdateAllMenuItems: 13 agents loaded, 20 menu commands registered
   ```

### Error Handling
- **Directory Not Found**: Shows error dialog with configured path
- **File Read Errors**: Logs to debug output, continues loading other agents
- **Copilot Integration Errors**: Falls back to clipboard copy with instructions
- **Network Path Issues**: Graceful degradation with user notification
- **File Copy Operations**: Logged to debug output, non-blocking if files don't exist


---

## 🔍 Troubleshooting

### Agents Not Appearing in Menu

**Problem:** Shadow Pilot menu is empty or missing agents.

**Solutions:**
1. **Verify AgentsPath environment variable**
   ```cmd
   echo %AgentsPath%
   ```
   Should show your configured path. If empty, set it:
   ```cmd
   setx AgentsPath "C:\MyAgents"
   ```

2. **Restart Visual Studio** after setting environment variable

3. **Check directory exists and is accessible**
   ```cmd
   dir "%AgentsPath%"
   ```

4. **Verify .md files exist** (excluding `.agent-header.md` and `.agent-footer.md`)

5. **Click Import Agents** to manually refresh

6. **Check debug output**:
   - View → Output → Select "Debug"
   - Look for: "Found X .md files" and "Total agents loaded: X"

### Import Agents Shows Zero Agents

**Problem:** "Agents refreshed successfully. 0 agent(s) loaded."

**Causes:**
- Only `.agent-header.md` and `.agent-footer.md` exist (these are reserved)
- All `.md` files have duplicate names
- Directory path is incorrect

**Solutions:**
- Add at least one agent file (e.g., `MyAgent.md`)
- Check debug output for "Skipped (duplicate or reserved name)" messages
- Verify AgentsPath points to correct directory

### Copilot Chat Not Opening

**Problem:** Clicking an agent doesn't open Copilot Chat.

**Solutions:**
1. **Ensure GitHub Copilot extension is installed**
   - Extensions → Manage Extensions → Search "GitHub Copilot"

2. **Sign in to GitHub Copilot**
   - Tools → Options → GitHub → Copilot

3. **Try manual open first**
   - Alt+Backslash or View → GitHub Copilot Chat

4. **Check debug output** for error messages

5. **Fallback mode**: If Copilot fails to open, instructions are copied to clipboard

### Agent Instructions Not Working

**Problem:** Wrong or incomplete instructions sent to Copilot.

**Solutions:**
1. **Verify file encoding is UTF-8**
   - Open in notepad, Save As → Encoding: UTF-8

2. **Check for special characters** that might break formatting

3. **Verify header/footer files** (if used):
   - `.agent-header.md` should exist and be readable
   - `.agent-footer.md` should exist and be readable

4. **Test without header/footer** by temporarily renaming them

### Network Path Issues

**Problem:** Error accessing network share path.

**Solutions:**
1. **Test network connectivity**
   ```cmd
   ping inblrgh781dat.ad005.onehc.net
   ```

2. **Verify UNC path access**
   ```cmd
   dir \\inblrgh781dat.ad005.onehc.net\CTShare\Agents
   ```

3. **Check permissions** - ensure read access to the share

4. **Consider mapping the drive**:
   ```cmd
   net use Z: \\inblrgh781dat.ad005.onehc.net\CTShare
   setx AgentsPath "Z:\Agents"
   ```

5. **Use local path as alternative**:
   ```cmd
   setx AgentsPath "C:\LocalAgents"
   ```

### Visual Studio Performance Impact

**Problem:** Menu opens slowly with many agents.

**Solutions:**
- Currently supports up to 19 agents efficiently
- Agents are cached and only reloaded when menu opens
- Consider organizing agents into categories if needed
- Remove unused agent files

### Debug Mode

**Enable detailed logging:**
1. Open Output window: **View → Output**
2. Select **Debug** from dropdown
3. Open Shadow Pilot menu
4. Look for diagnostic messages:
   ```
   AgentsPath environment variable not found, using default
   Found 15 .md files in C:\MyAgents
   Processing: CodeReviewer
     Added to Agents list
   Processing: TestGenerator  
     Added to Agents list
   Total agents loaded: 13
   UpdateAllMenuItems: 13 agents loaded, 20 menu commands registered
   ```

This shows exactly what's happening during agent loading.

---

