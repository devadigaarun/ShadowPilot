## Overview

# Shadow Pilot - Visual Studio Copilot Extension  
**AI Workflow. One Click.**  
*A concise multi-stakeholder overview*  

---
## 📑 Table of Contents

- [What Is Shadow Pilot?](#what-is-shadow-pilot)
  - [Core Capabilities](#core-capabilities)
  - [Enterprise Architecture](#enterprise-architecture)
- [How Shadow Pilot Solves the Problem](#how-shadow-pilot-solves-the-problem)
- [Dynamic Agent Creation](#dynamic-agent-creation)
  - [Automatic Workspace Integration](#automatic-workspace-integration)
- [Key Advantages](#key-advantages)
- [Value for Different Stakeholders](#value-for-different-stakeholders)
- [Limitations (Transparent View)](#limitations-transparent-view)
- [Future Outlook](#future-outlook)
- [Auto Tagging for AI-Generated Code](#auto-tagging-for-ai-generated-code)
  - [Why Auto Tagging?](#why-auto-tagging)
  - [How It Works](#how-it-works)
  - [Configuration Steps](#configuration-steps)
  - [Examples](#examples)
  - [Language Support](#language-support)
  - [Supported Scenarios](#supported-scenarios)
  - [Best Practices](#best-practices)
  - [Troubleshooting](#troubleshooting)
- [Limitation on Visual Studio Side](#limitation-on-visual-studio-side)
- [Agent Configuration System](#agent-configuration-system)
  - [Configuration Architecture](#configuration-architecture)
  - [Directory Structure](#directory-structure)
- [Creating Agents: Step-by-Step Guide](#creating-agents-step-by-step-guide)
  - [Step 1: Create Global Configuration](#step-1-create-global-configuration)
  - [Step 2: Create Your First Agent](#step-2-create-your-first-agent)
  - [Step 3: Advanced Agent Configuration](#step-3-advanced-agent-configuration)
  - [Step 4: Understanding Instruction Layering](#step-4-understanding-instruction-layering)
  - [Step 5: Workspace Integration](#step-5-workspace-integration)
- [Agent Examples](#agent-examples)
- [Path Resolution Rules](#path-resolution-rules)
- [Best Practices](#best-practices)
- [Troubleshooting](#troubleshooting)
- [Configuring Agents Path](#configuring-agents-path)
- [Quick Start Checklist](#quick-start-checklist)
- [Example Workspace Directory Structure](#example-workspace-directory-structure)
- [Legacy Agent Configuration (Simple Mode)](#legacy-agent-configuration-simple-mode)
  - [When to Use Legacy Mode](#when-to-use-legacy-mode)
  - [Legacy Directory Structure](#legacy-directory-structure)
  - [How Legacy Mode Works](#how-legacy-mode-works)
  - [Setting Up Legacy Mode](#setting-up-legacy-mode)
  - [Legacy Mode Workspace Integration](#legacy-mode-workspace-integration)
  - [Legacy Mode Limitations](#legacy-mode-limitations)
  - [Migrating from Legacy to JSON Configuration](#migrating-from-legacy-to-json-configuration)
- [Additional Resources](#additional-resources)

---
## 1. What Is Shadow Pilot ?
Shadow Pilot is a **Visual Studio extension** that automates how developers interact with GitHub Copilot Chat while enabling **enterprise-wide governance** of AI interactions and **controlled context** injection. It automatically injects structured instruction templates directly into the Copilot Chat window and triggers execution, effectively empowering ***agent-style** workflows and functionality within Visual Studio.

### Core Capabilities

#### For Individual Developers:
- Streamlines and automates interactions with GitHub Copilot Chat directly within Visual Studio
- Injects structured instruction templates into the chat input buffer and triggers execution
- Enables agent-like workflows inside Visual Studio, bridging a key capability gap in the IDE
- **Auto-tags AI-generated code** for easy identification and compliance tracking

#### For Enterprise Teams:
- **Centralized Instruction Management** - Maintains AI instructions in a controlled, version-controlled repository accessible enterprise-wide
- **Layered Instruction Architecture** - Combines organization-level policies, project standards, and agent-specific instructions in a prioritized hierarchy
- **Automatic Propagation** - Ensures all developers always use the latest and greatest instructions without manual updates
- **Zero-Configuration Deployment** - Developers automatically receive current instructions on every agent invocation
- **Workspace Integration** - Auto-copies instruction files from the central repository to each solution's `.github` folder, ensuring GitHub Copilot has proper context
- **AI Code Tagging** - Automatically marks AI-generated code with configurable tags for audit, compliance, and code review purposes


### Enterprise Architecture

Shadow Pilot implements a **three-tier instruction layering system** from a centralized repository:

- **Organization Layer** (Highest Priority)
   - Company-wide guardrails (security, compliance, legal requirements)
   - Architecture standards and design principles
   - Non-negotiable rules that apply to all AI interactions

- **Standards Layer** (Mid Priority)
   - Department or team-level coding standards
   - Technology-specific best practices
   - Testing and quality requirements

- **Agent Layer** (Execution Layer)
   - Task-specific instructions for individual agents
   - Specialized prompts for code review, testing, refactoring, etc.
   - Custom templates and examples
[⬆ Back to Top](#table-of-contents)
---
## 2. How Shadow Pilot Solves the Problem

Shadow Pilot acts as a **bridge** between developers and Copilot Chat:

- Opens Copilot Chat automatically  
- Injects instruction templates into the chat input buffer  
- Sends the message to Copilot  
- **Auto-copies workspace context files** (`copilot-instructions.md` and instruction files) to solution's `.github` folder
- Emulates lightweight agent behavior until official APIs arrive
This enables **governed, repeatable, error-free AI workflows** in Visual Studio today.<br>
[⬆ Back to Top](#table-of-contents)
---
## 3. Dynamic Agent Creation

Shadow Pilot enables dynamic agent creation and execution through a configuration-driven approach—eliminating the need for code changes or redeployment.

**When Shadow Pilot → Import Agents is invoked:**

- Automatically discovers agent definitions from the configured Agents Path
- Dynamically generates menu entries for all available agents in Visual Studio
- Supports plug-and-play agent onboarding—simply add a new agent folder with configuration
- Combines global and agent-specific configurations to construct execution context
- Resolves and layers instruction files based on defined precedence (global → agent)
- Allows agents to either inherit global behavior or operate in isolation (ignoreGlobal)
- Enables instant availability of new agents without restarting Visual Studio

**Key Benefits:**
- Zero Code Changes → Create or modify agents using configuration only
- Rapid Extensibility → Add new capabilities by just dropping new agent folders
- Consistent Execution Model → All agents follow the same layered instruction architecture
- Scalable Design → Supports enterprise-scale agent ecosystems
- Future-Ready → Easily adaptable to native Copilot APIs when available
[⬆ Back to Top](#table-of-contents)
---
### Automatic Workspace Integration

Shadow Pilot goes beyond just injecting prompts - it **automatically prepares your solution** for GitHub Copilot. Configuration determines which files are included in dynamic agent instruction generation and which are copied into the workspace.

**When Shadow Pilot->Import Agents from Tools Menu is invoked:**
- Dynamically builds a clickable menu from agent configurations in the configured Agents Path
- Supports two configuration levels: global (shared) and agent-specific (overrides & customization)
- Based on the global and agent configurations, corresponding instruction files are copied into the workspace:
  - Global Instructions → copilot-instructions.md → [Solution]/.github/
  - Specialized Context → *.instructions.* → [Solution]/.github/instructions/
  - Each agent can specify its own set of files or rely on global defaults for copying.
- Zero Configuration → Developers get ready-to-use Copilot context without manual setup
- **Always Current - greatest & latest** - Every invocation refreshes files from the central repository.

**Key Benefits:**
- **Team Consistency** - Everyone gets identical Copilot context automatically
- **Solution-Specific** - Each solution maintains its own `.github` folder automatically updated by Shadow Pilot
- **Version Control Ready** - Files can be committed or gitignored per team policy
- **Centralized Management** - Update instructions in one plac under controlled environment, propagate to all developers seamlessly
- **Single Source of Truth** - No more outdated instructions floating around in chat histories or local files
- **Zero Maintenance** - No manual copying, no outdated instructions
[⬆ Back to Top](#table-of-contents)
---
## 4. Key Advantages

### Bring Agent-like Experience to Visual Studio
- Enables structured AI workflows inside Visual Studio  
- Mimics agent behavior without official APIs  
- No need to switch IDEs (Visual Studio -> Visual Code -> Visual Studio) 

**Impact:** Teams stay in their primary development environment

### One-Click AI Workflow Execution
- Click an agent → Opens Copilot Chat  
- Injects structured prompts automatically  
- Instantly triggers Agent execution with contextual instruction files automatically in place.

**Impact:** Reduces manual copy-paste effort • Standardizes AI usage across teams

### Centralized & Always latest & greatest Instructions
- Reads agent instructions from shared location  
- Ensures everyone uses the latest version  
- Eliminates the use of outdated agent prompts and contextual instruction files  

**Impact:** Governance + consistency across projects

### Multi-File Instruction Bundling
**Automatically loads:**
- Architecture guidelines  
- Coding standards  
- Test rules  
- Injects everything together  

**Impact:** Enterprise-grade prompt orchestration

### Instruction Layering with Precedence
**Supports ordered layers:**
1. Org-level rules  
2. Project-level rules  
3. Agent-level instructions  
4. User prompt  

Higher priority rules are never overridden.

**Impact:** Strong compliance + controlled AI behavior

### Enterprise Productivity Booster
- Reduces repetitive prompting  
- Encourages standardized Copilot usage  
- Improves onboarding for new developers  
- Enables scalable AI adoption  

**Impact:** Measurable productivity gains across the organization
[⬆ Back to Top](#table-of-contents)
---
## 5. Value for Different Stakeholders

### For Developers
- One-click execution of complex instructions  
- No need to copy/paste long prompts  
- Reproducible responses from Copilot  
- Faster automation and experimentation  
- **Automatic workspace setup** - Context files copied to `.github` folder without manual intervention

### For Architects
- Codified AI workflows  
- Standardized prompt templates  
- Reliable automation despite lack of official Copilot APIs  
- Future-safe approach: easily replaceable with native APIs later  
- **Centralized context management** - Single source of truth for Copilot instructions

### For Managers
- Consistency across teams  
- Reduced onboarding effort  
- Improved productivity with minimal training  
- Clear governance model for AI usage  
- **Guaranteed context uniformity** - All developers work with identical Copilot configurations

### For Leadership 
- Enables predictable productivity gains  
- Low-risk, high-impact solution  
- **Scalable knowledge distribution** - Best practices propagate automatically
[⬆ Back to Top](#table-of-contents)
---
## 6. Limitations (Transparent View)

Shadow Pilot operates as an **Agent Emulator**, not a full agent framework:  
 
- **Uses UI-driven customization:** Requires focus on Copilot chat window; user interaction may disrupt injection.
- **One-way automation:** Cannot read Copilot’s response or detect completion as of now.  
- **Includes temporary solution:** Once Microsoft exposes native APIs, this approach will be replaced—but Shadow Pilot can adopt them seamlessly.
[⬆ Back to Top](#table-of-contents)
---
## 7. Future Outlook

When Microsoft introduces a full **Copilot extensibility model for Visual Studio**, Shadow Pilot will transition from a Uses UI-driven customization to a native integration—preserving all workflows while gaining reliability and performance.
[⬆ Back to Top](#table-of-contents)
---
## 8. Auto Tagging for AI-Generated Code

Shadow Pilot includes an **Auto Tagging** feature that automatically identifies and marks code generated by GitHub Copilot. This helps teams track, audit, and manage AI-generated code within their codebase for compliance, code review, and governance purposes.

### Why Auto Tagging?

- **Compliance & Audit** - Track AI-generated code for regulatory and compliance requirements
- **Code Review** - Quickly identify sections that may need additional human review
- **Knowledge Transfer** - Help team members understand which code was AI-assisted
- **Quality Assurance** - Focus testing efforts on AI-generated sections
- **IP & Legal** - Maintain clear records of AI involvement in code creation

### How It Works

When enabled, Shadow Pilot monitors GitHub Copilot operations and automatically adds tags when:

1. **Inline Suggestions** - When you accept Copilot suggestions by pressing **Tab**
2. **Copilot Edits** - When you click **Keep** to accept entire file generation from Copilot

Tags are added based on code structure:
- **Multi-line code**: Start and end tags wrap the generated code block
- **Single-line code**: A single tag is placed above the line

### Configuration Steps

#### Step 1: Enable Auto Tagging

1. Open **Visual Studio**
2. Go to **Tools → Options**
3. Navigate to **Shadow Pilot → General**
4. Find the **Auto Tagging** section
5. Set **Enable Auto Tagging** to `True`
6. Click **OK** to save

#### Step 2: Customize Tags (Optional)

You can customize the tag text in **Tools → Options → Shadow Pilot → General**:

| Option | Default Value | Description |
|--------|---------------|-------------|
| **Enable Auto Tagging** | `False` | Enable or disable automatic tagging of AI-generated code |
| **Start Tag** | `// [AI-GENERATED CODE START]` | Tag placed before multi-line AI-generated code |
| **End Tag** | `// [AI-GENERATED CODE END]` | Tag placed after multi-line AI-generated code |
| **Single Line Tag** | `// [AI-GENERATED]` | Tag placed above single-line AI-generated code |

### Examples

**Multi-line code (method, class, etc.):**
```csharp
// [AI-GENERATED CODE START]
public void ProcessData(string input)
{
    if (string.IsNullOrEmpty(input))
        throw new ArgumentNullException(nameof(input));
    
    var result = input.Trim().ToUpper();
    Console.WriteLine(result);
}
// [AI-GENERATED CODE END]
```

**Single-line code:**
```csharp
// [AI-GENERATED]
var items = collection.Where(x => x.IsActive).OrderBy(x => x.Name).ToList();
```

**Entire file generated by Copilot Edits:**
```csharp
// [AI-GENERATED CODE START]
using System;
using System.Collections.Generic;

namespace MyApp
{
    public class GeneratedService
    {
        public void Execute()
        {
            // Implementation
        }
    }
}
// [AI-GENERATED CODE END]
```

### Language Support

The tagging feature automatically detects the programming language and uses the appropriate comment style:

| Language | Comment Style |
|----------|---------------|
| C#, C++, Java, JavaScript, TypeScript | `//` |
| Python, Ruby, Perl, PowerShell, Bash | `#` |
| SQL | `--` |
| VB.NET | `'` |
| HTML, XML, XAML | `<!-- -->` |
| CSS, LESS, SCSS | `/* */` |

### Supported Scenarios

| Scenario | Detection Method | Tag Placement |
|----------|------------------|---------------|
| **Inline suggestion (Tab)** | Tab key press with code insertion | Around inserted code |
| **Copilot Edits (Keep)** | Copilot command with file modification | Around entire generated content |
| **Copilot Chat suggestions** | Copilot command execution | Around inserted code |

### Best Practices

1. **Enable during active development** - Turn on auto-tagging when using Copilot for code generation
2. **Customize tags for your team** - Adjust tag text to match your organization's conventions
3. **Use for code reviews** - Tags help reviewers quickly identify AI-generated sections
4. **Consider compliance requirements** - Some industries require tracking of AI-generated code
5. **Version control friendly** - Tags are regular comments and work with all version control systems

### Troubleshooting

#### Tags not appearing

- Verify auto-tagging is enabled in **Tools → Options → Shadow Pilot → General**
- Ensure you're accepting Copilot suggestions (pressing Tab when suggestion is visible)
- For Copilot Edits, make sure you click "Keep" to accept the changes
- Check the VS Output window (**View → Output**, select **Debug**) for diagnostic messages

#### Tags in unexpected position

- The tagging uses line-based positioning for accuracy
- Ensure the document is not being modified by other extensions simultaneously
- Try undoing and re-accepting the Copilot suggestion

#### Wrong comment style

- The extension detects language from the file's content type
- For unsupported languages, C-style comments (`//`) are used by default
[⬆ Back to Top](#table-of-contents)
---
## 9. Limitation on Visual Studio side
- VSIX extensions cannot reach or control the Copilot chat UI.  
- No official APIs for Copilot automation or agent development.  
- Copilot Chat behaves as a **closed, non-extensible surface**.

➡️ Developers are forced to manually paste long instructions, leading to inconsistent workflows.<br>

[⬆ Back to Top](#table-of-contents)

---
## 10. Agent Configuration System

Shadow Pilot uses a **modular multi-file configuration architecture** that enables teams to create, manage, and share AI agents efficiently without external support.

### Configuration Architecture

Shadow Pilot supports two configuration layers:

1. **Global Configuration** (`global.config.json`) - Organization-wide settings
2. **Agent-Specific Configuration** (`agents/<agent-name>/agent.json`) - Individual agent definitions

### Directory Structure

```
<AgentsPath>/
├── global.config.json                    # Global configuration (required)
├── agents/                                # Agent definitions directory
│   ├── code-review/                       # Agent: Code Review
│   │   ├── agent.json                     # Agent configuration
│   │   ├── instruction.md                 # Main instruction
│   │   ├── guidelines.md                  # Supporting files
│   │   └── templates/
│   │       └── review-template.md
│   ├── clean-code/                        # Agent: Clean Code
│   │   ├── agent.json
│   │   ├── instruction.md
│   │   └── examples.md
│   └── security-audit/                    # Agent: Security Audit
│       ├── agent.json
│       ├── instruction.md
│       └── templates/
│           └── security-template.md
├── instructions/                          # Shared instruction files
│   ├── copilot-instructions.md
│   └── *.instructions.md
├── org/                                   # Organization-level files
│   ├── guardrails.instructions.md
│   ├── architecture.instructions.md
│   └── security.instructions.md
├── standards/                             # Coding standards
│   ├── testing.instructions.md
│   └── coding-standards.instructions.md
└── templates/                             # Shared templates
    └── default-output-template.md
```
[⬆ Back to Top](#table-of-contents)

---

## 11. Creating Agents: Step-by-Step Guide

### Step 1: Create Global Configuration

Create `global.config.json` in your AgentsPath directory:

**File:** `<AgentsPath>/global.config.json`

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
  
  "guardrails": [
    "org/guardrails.instructions.md"
  ],

  "prefix": [
    "org/architecture.instructions.md",
    "org/security.instructions.md"
  ],

  "suffix": [
    "standards/coding-standards.instructions.md"
  ],

  "outputTemplate": "templates/default-output-template.md"
}
```

**Configuration Properties:**

| Property | Type | Required | Description |
|----------|------|----------|-------------|
| `version` | string | Yes | Configuration schema version (use "1.0") |
| `workspaceIntegration` | object | No | Files to copy to solution `.github` folder |
| `guardrails` | array | No | Organization-level guardrails (highest priority) |
| `prefix` | array | No | Instructions prepended to all agents |
| `suffix` | array | No | Instructions appended to all agents |
| `outputTemplate` | string | No | Default output template for all agents |

**Path Resolution:** All paths in `global.config.json` are relative to `<AgentsPath>/`

---

### Step 2: Create Your First Agent

#### 2.1 Create Agent Directory

```bash
mkdir -p <AgentsPath>/agents/code-review
```

#### 2.2 Create Agent Configuration

**File:** `<AgentsPath>/agents/code-review/agent.json`

```json
{
  "name": "Code Review Agent",
  "instruction": "instruction.md",
  "ignoreGlobal": false
}
```

**Minimal Configuration Properties:**

| Property | Type | Required | Description |
|----------|------|----------|-------------|
| `name` | string | Yes | Agent display name (appears in VS menu) |
| `instruction` | string | Yes | Main instruction file (relative to agent directory) |
| `ignoreGlobal` | boolean | No | Set to `true` to ignore all global settings (default: `false`) |

**Even if you skip agent.json**, Shadow Pilot will create a menu entry using the folder name as the agent name and look for `instruction.md` by default.

#### 2.3 Create Agent Instruction

**File:** `<AgentsPath>/agents/code-review/instruction.md`

```markdown
# Code Review Agent

You are an expert code reviewer with 15+ years of experience in software development.

## Your Role
Perform thorough code reviews focusing on:
- Code quality and maintainability
- Security vulnerabilities
- Performance issues
- Best practices adherence

## Review Process
1. Analyze the provided code carefully
2. Identify issues by severity (Critical, High, Medium, Low)
3. Provide specific suggestions with code examples
4. Explain the reasoning behind each recommendation

## Output Format
For each issue found:
- **Severity:** [Critical/High/Medium/Low]
- **Location:** [File:Line]
- **Issue:** [Brief description]
- **Recommendation:** [Specific fix with code example]
- **Rationale:** [Why this matters]

## Guidelines
- Be constructive and professional
- Prioritize security and performance issues
- Suggest modern best practices
- Provide actionable feedback
```

#### 2.4 Test Your Agent

1. Open Visual Studio
2. Go to **Tools → Shadow Pilot → Import Agents**
3. Your agent should appear: **Tools → Shadow Pilot → Code Review Agent**
4. Click the agent to test execution

---

### Step 3: Advanced Agent Configuration

For more complex agents, you can add:

**File:** `<AgentsPath>/agents/code-review/agent.json`

```json
{
  "name": "Code Review Agent",
  "instruction": "instruction.md",

  "prefix": [
    "guidelines.md",
    "checklist.md"
  ],

  "suffix": [
    "examples.md"
  ],

  "guardrails": [
    "security-guardrails.md"
  ],

  "outputTemplate": "templates/code-review-template.md",

  "ignoreGlobal": false,
  
  "workspaceIntegration": {
    "enabled": true,
    "copilotInstructions": "instructions/code-review.instructions.md",
    "instructionFiles": [
      "templates:templates/review-template.md",
      "examples:examples/*.md"
    ]
  }
}
```

**Advanced Configuration Properties:**

| Property | Type | Description |
|----------|------|-------------|
| `prefix` | array | Instructions prepended to agent (before main instruction) |
| `suffix` | array | Instructions appended to agent (after main instruction) |
| `guardrails` | array | Agent-specific guardrails (strict rules) |
| `outputTemplate` | string | Agent-specific output format template |
| `workspaceIntegration` | object | Agent-specific workspace file copying (overrides global) |

---

### Step 4: Understanding Instruction Layering

When Shadow Pilot builds a prompt, instructions are layered in this **priority order**:

```
┌─────────────────────────────────────┐
│ 1. Global Guardrails (Highest)     │ ← Unless ignoreGlobal: true
├─────────────────────────────────────┤
│ 2. Agent Guardrails                 │
├─────────────────────────────────────┤
│ 3. Global Prefix                    │ ← Unless ignoreGlobal: true
├─────────────────────────────────────┤
│ 4. Agent Prefix                     │
├─────────────────────────────────────┤
│ 5. Agent Main Instruction           │ ← Your agent.instruction file
├─────────────────────────────────────┤
│ 6. Global Suffix                    │ ← Unless ignoreGlobal: true
├─────────────────────────────────────┤
│ 7. Agent Suffix                     │
├─────────────────────────────────────┤
│ 8. Output Template                  │ ← Agent-specific or global
└─────────────────────────────────────┘
```

**Example Layered Prompt:**

If you have:
- Global guardrails: `org/guardrails.instructions.md`
- Agent prefix: `guidelines.md`
- Agent instruction: `instruction.md`
- Agent suffix: `examples.md`
- Global suffix: `standards/coding-standards.instructions.md`

The final prompt will be:

```
[Content from org/guardrails.instructions.md]

[Content from guidelines.md]

[Content from instruction.md]

[Content from examples.md]

[Content from standards/coding-standards.instructions.md]
```

---
### Step 5: Workspace Integration

#### Global Workspace Integration

Configure files to copy to **all** solutions:

**In `global.config.json`:**

```json
"workspaceIntegration": {
  "enabled": true,
  "copilotInstructions": "instructions/copilot-instructions.md",
  "instructionFiles": [
    "instructions:instructions/*.md",
    "templates:templates/*.md"
    "standards/*.md"
  ]
}
```

**File Pattern Format:** `<destination-folder>:<source-path>` - Creates `<destination-folder>` folder in `.github` and copies files from the source path.

**File Pattern Format:** `<source-path>` - Copies files directly to `.github` root.


Examples:
- `"instructions:instructions/*.md"` → Copies to `.github/instructions/`
- `"templates/default:templates/template.md"` → Copies to `.github/templates/default/`
- `"*.md"` → Copies to `.github/` (root)

#### Agent-Specific Workspace Integration

Override global settings for specific agents:

**In `agent.json`:**

```json
"workspaceIntegration": {
  "enabled": true,
  "copilotInstructions": "instructions/code-review.instructions.md",
  "instructionFiles": [
    "templates:templates/review-template.md",
    "checklists:checklists/*.md"
  ]
}
```

Files are resolved from the **agent directory first**, then **global directory**.

**Result in Solution:**

```
YourSolution/.github/
├── copilot-instructions.md           (from agent or global)
├── instructions/                      (if configured)
├── templates/                       (if configured)
│   └── review-template.md
└── checklists/                      (if configured)
    └── *.md files
```

[⬆ Back to Top](#table-of-contents)

---

## 12. Agent Examples

### Example 1: Simple Agent (Minimal Configuration)

**Directory:** `agents/clean-code/`

**agent.json:**
```json
{
  "name": "Clean Code Agent",
  "instruction": "instruction.md",
  "ignoreGlobal": false
}
```

**instruction.md:**
```markdown
# Clean Code Agent

You are a clean code expert. Review and refactor code following:

## Principles
- Single Responsibility Principle
- DRY (Don't Repeat Yourself)
- KISS (Keep It Simple, Stupid)
- Meaningful naming conventions
- Small, focused functions

## Tasks
1. Identify code smells
2. Suggest refactoring opportunities
3. Improve readability
4. Add appropriate comments where needed
```

**Result:** Uses all global settings + agent instruction

---

### Example 2: Specialized Agent with Custom Instructions

**Directory:** `agents/security-audit/`

**agent.json:**
```json
{
  "name": "Security Audit Agent",
  "instruction": "instruction.md",
  
  "prefix": [
    "owasp-top10.md"
  ],
  
  "guardrails": [
    "security-rules.md"
  ],
  
  "outputTemplate": "templates/security-report.md",
  
  "ignoreGlobal": false
}
```

**Files in agent directory:**
- `instruction.md` - Main security audit instructions
- `owasp-top10.md` - OWASP Top 10 vulnerabilities reference
- `security-rules.md` - Non-negotiable security rules
- `templates/security-report.md` - Security report template

**Result:** Global settings + agent-specific security focus

---

### Example 3: Isolated Agent (No Global Settings)

**Directory:** `agents/experimental/`

**agent.json:**
```json
{
  "name": "Experimental Agent",
  "instruction": "instruction.md",
  "ignoreGlobal": true,
  
  "prefix": [
    "custom-context.md"
  ]
}
```

**Result:** Only uses agent-specific instructions (completely isolated)

---

## 13. Path Resolution Rules

### Global Configuration Paths

All paths in `global.config.json` are resolved from `<AgentsPath>/`

**Example:**
```json
"guardrails": ["org/guardrails.instructions.md"]
```
→ Resolves to: `<AgentsPath>/org/guardrails.instructions.md`

### Agent Configuration Paths

Paths in `agent.json` are resolved in this order:

1. **Agent directory first:** `<AgentsPath>/agents/<agent-name>/<path>`
2. **Global fallback:** `<AgentsPath>/<path>`

**Example:**

Agent directory: `agents/code-review/`

```json
"prefix": ["guidelines.md"]
```

Resolution:
1. Try: `<AgentsPath>/agents/code-review/guidelines.md` ✅
2. If not found, try: `<AgentsPath>/guidelines.md`

This allows agents to have **their own files** or **use shared files**.

[⬆ Back to Top](#table-of-contents)

---

## 13. Best Practices

### 1. **Organize Agent Files Logically**

```
agents/agent-name/
├── agent.json              # Configuration
├── instruction.md          # Main instruction
├── guidelines.md           # Supporting files
├── examples.md
├── templates/              # Agent-specific templates
│   └── template.md
└── instructions/           # Additional instructions
    └── extra.instructions.md
```

### 2. **Use Descriptive Agent Names**

✅ Good:
- `"name": "Code Review Agent"`
- `"name": "Security Audit Agent"`
- `"name": "BDD Test Generator"`

❌ Avoid:
- `"name": "Agent1"`
- `"name": "Test"`

### 3. **Share Common Resources**

Place organization-wide files in shared directories:
- `org/` - Organization-level rules and guardrails
- `standards/` - Coding standards and best practices
- `templates/` - Common templates
- `instructions/` - Shared instruction files

### 4. **Use Guardrails for Non-Negotiable Rules**

```json
"guardrails": [
  "org/security-rules.md",
  "org/compliance-requirements.md"
]
```

Guardrails are **highest priority** and always applied first.

### 5. **Version Control Your Configuration**

```bash
git add agents/
git add global.config.json
git commit -m "Add Code Review Agent"
```

Benefits:
- Track changes over time
- Review agent modifications via pull requests
- Rollback if needed

[⬆ Back to Top](#table-of-contents)

---
## 14. Troubleshooting

### Agent Not Appearing in Menu

**Problem:** Created agent doesn't show in Visual Studio

**Solutions:**
1. Ensure `agents/<agent-name>/agent.json` exists
2. Verify JSON is valid (use JSON validator)
3. Check `name` property is set in `agent.json`
4. Click **Tools → Shadow Pilot → Import Agents** to refresh
5. Check Visual Studio **Output → Debug** for error messages

### Files Not Found

**Problem:** Instruction files not loading

**Solutions:**
1. Check file paths are correct
2. Agent-specific files → relative to `agents/<name>/`
3. Shared files → relative to `<AgentsPath>/`
4. Use forward slashes `/` or backslashes `\` consistently
5. Check Visual Studio **Output → Debug** for file resolution messages

### Workspace Files Not Copying

**Problem:** Files not appearing in `.github` folder

**Solutions:**
1. Check `workspaceIntegration.enabled` is `true`
2. Verify file paths exist
3. Ensure solution is open in Visual Studio
4. Check pattern syntax (e.g., `"folder:path/*.md"`)
5. Review Visual Studio **Output → Debug** for copy operations

### Wrong Instructions Loading

**Problem:** Getting different instructions than expected

**Solutions:**
1. Verify `ignoreGlobal` setting
2. Check instruction layering order (see Section 10, Step 4)
3. Ensure agent-specific files exist in agent directory
4. Review **Output → Debug** for loaded instruction files

[⬆ Back to Top](#table-of-contents)

---
## 15. Configuring Agents Path

Shadow Pilot requires the **AgentsPath** setting to locate your agent configurations.

### Steps to Configure

1. Open **Visual Studio**
2. Go to **Tools → Options**
3. Navigate to **ShadowPilot → General**
4. Set **AgentsPath** to your configuration root directory
5. Click **OK**

**Example:**
- AgentsPath: `C:\ShadowPilot\Agents`
- Expected: `C:\ShadowPilot\Agents\global.config.json`
- Expected: `C:\ShadowPilot\Agents\agents\code-review\agent.json`

### Environment Variable (Optional)

You can also set via environment variable:

```cmd
setx AgentsPath "C:\ShadowPilot\Agents" /M
```

Priority order:
1. User environment variable
2. Machine environment variable
3. Visual Studio Options
[⬆ Back to Top](#table-of-contents)

---
## 16. Quick Start Checklist

- [ ] Create `<AgentsPath>/global.config.json`
- [ ] Create `<AgentsPath>/agents/` directory
- [ ] Create your first agent directory: `agents/my-agent/`
- [ ] Create `agents/my-agent/agent.json` with name and instruction
- [ ] Create `agents/my-agent/instruction.md` with agent prompts
- [ ] Configure AgentsPath in Visual Studio (Tools → Options → ShadowPilot)
- [ ] Click **Tools → Shadow Pilot → Import Agents**
- [ ] Test your agent: **Tools → Shadow Pilot → [Your Agent Name]**
- [ ] Verify files copied to solution `.github` folder
- [ ] (Optional) Enable Auto Tagging in Tools → Options → ShadowPilot → General
- [ ] Commit configuration to version control
[⬆ Back to Top](#table-of-contents)

---
## 17. Example Workspace Directory Structure

When you execute an agent, Shadow Pilot automatically creates:

```
D:\git\MyProject\.github\
├── copilot-instructions.md         (from global or agent config)
├── instructions\
│   ├── architecture.instructions.md
│   ├── coding-standards.instructions.md
│   ├── security.instructions.md
│   └── testing.instructions.md
└── templates\                       (if configured)
    └── output-template.md
```

**Note:** Files are refreshed on every agent execution, ensuring always up-to-date context.
[⬆ Back to Top](#table-of-contents)

---
## 18. Legacy Agent Configuration (Simple Mode)

Shadow Pilot also supports a **legacy configuration mode** for teams that prefer a simpler, file-based approach without JSON configuration files. This mode is ideal for quick setup, small teams, or when migrating from earlier versions.

### When to Use Legacy Mode

- **Quick Start** - Get agents running without creating JSON configuration files
- **Simple Teams** - Small teams with straightforward agent requirements
- **Migration** - Transitioning from earlier Shadow Pilot versions
- **Minimal Configuration** - When you don't need layered instructions or advanced features

### Legacy Directory Structure
#### Option 1: Flat Structure (Single Instruction File per Agent under instructions folder)
```
<AgentsPath>/
├── copilot-instructions.md              # Global Copilot instructions (optional)
├── instructions/                         # Shared instruction files
│   ├── agent-name.instructions.md       # Agent instruction file (required)
│   ├── code-review.instructions.md      # Example: Code Review Agent
│   ├── clean-code.instructions.md       # Example: Clean Code Agent
│   └── security-audit.instructions.md   # Example: Security Audit Agent
```

#### Option 2: Flat Structure (Single Instruction File per Agent under agents folder)
```
<AgentsPath>/
├── copilot-instructions.md              # Global Copilot instructions (optional)
├── agents/                              # Shared instruction files
│   ├── code-review.instructions.md      # Example: Code Review Agent
│   ├── clean-code.instructions.md       # Example: Clean Code Agent
│   └── security-audit.instructions.md   # Example: Security Audit Agent
```

#### Option 3: Flat Structure (Single Instruction File per Agent directly under agent root folder)
```
<AgentsPath>/
├── copilot-instructions.md          # Global Copilot instructions (optional)
├── agent-name.instructions.md       # Agent instruction file (required)
├── code-review.instructions.md      # Example: Code Review Agent
├── clean-code.instructions.md       # Example: Clean Code Agent
├── security-audit.instructions.md   # Example: Security Audit Agent
```

### How Legacy Mode Works

1. **Agent Discovery** - Shadow Pilot scans the `instructions/` folder for files matching `*.instructions.md`
2. **Menu Generation** - Each instruction file becomes a menu item in **Tools → Shadow Pilot**
3. **Agent Naming** - The filename (without `.instructions.md`) becomes the agent name
   - `code-review.instructions.md` → **Code Review** (spaces replace hyphens, title case)
   - `clean-code.instructions.md` → **Clean Code**
4. **Execution** - Clicking an agent injects the instruction file content into Copilot Chat

### Setting Up Legacy Mode

#### Step 1: Configure Agents Path

1. Open **Visual Studio**
2. Go to **Tools → Options**
3. Navigate to **Shadow Pilot → General**
4. Set **Agents Directory Path** to your agents folder
5. Click **OK**

#### Step 2: Create Instructions Folder

```bash
mkdir -p <AgentsPath>/instructions
```

#### Step 3: Create Agent Instruction Files

Create markdown files in the `instructions/` folder with the naming pattern `<agent-name>.instructions.md`:

**File:** `<AgentsPath>/instructions/code-review.instructions.md`

```markdown
# Code Review Agent

You are an expert code reviewer. Analyze the provided code for:

## Focus Areas
- Code quality and readability
- Potential bugs and edge cases
- Security vulnerabilities
- Performance issues
- Best practices adherence

## Output Format
Provide your review in this format:
1. **Summary** - Overall assessment
2. **Issues Found** - List each issue with severity
3. **Recommendations** - Specific improvements
4. **Positive Aspects** - What's done well
```

**File:** `<AgentsPath>/instructions/clean-code.instructions.md`

```markdown
# Clean Code Agent

You are a clean code expert. Review and refactor code following:

## Principles
- Single Responsibility Principle
- DRY (Don't Repeat Yourself)
- KISS (Keep It Simple, Stupid)
- Meaningful naming conventions
- Small, focused functions

## Tasks
1. Identify code smells
2. Suggest refactoring opportunities
3. Improve readability
4. Add appropriate comments where needed
```

#### Step 4: (Optional) Create Global Copilot Instructions

Create a `copilot-instructions.md` file in the root of your AgentsPath:

**File:** `<AgentsPath>/copilot-instructions.md`

```markdown
# Global Copilot Instructions

## Context
You are working on a .NET enterprise application.

## Standards
- Follow C# coding conventions
- Use async/await for I/O operations
- Include XML documentation comments
- Write unit tests for new code

## Security
- Never log sensitive data
- Validate all user inputs
- Use parameterized queries
```

This file is automatically copied to the solution's `.github/copilot-instructions.md` folder when any agent is invoked.

#### Step 5: Import and Test Agents

1. Open Visual Studio
2. Click **Tools → Shadow Pilot → Import Agents**
3. Your agents should appear in the menu:
   - **Tools → Shadow Pilot → Code Review**
   - **Tools → Shadow Pilot → Clean Code**
4. Click an agent to test

### Legacy Mode Workspace Integration

When an agent is invoked in legacy mode, Shadow Pilot automatically:

1. **Copies `copilot-instructions.md`** to `[Solution]/.github/copilot-instructions.md`
2. **Copies instruction files** from `instructions/` folder to `[Solution]/.github/instructions/`

**Result:**
```
YourSolution/.github/
├── copilot-instructions.md           (from AgentsPath root)
└── instructions/
    ├── code-review.instructions.md
    ├── clean-code.instructions.md
    └── security-audit.instructions.md
```

### Legacy Mode Limitations

| Feature | Legacy Mode | JSON Configuration |
|---------|-------------|-------------------|
| Agent discovery | ✅ Automatic from filenames | ✅ From `agent.json` |
| Custom agent names | ❌ Derived from filename | ✅ Configurable |
| Instruction layering | ❌ Single file only | ✅ Guardrails, prefix, suffix |
| Global guardrails | ❌ Not supported | ✅ Supported |
| Per-agent workspace files | ❌ Global only | ✅ Supported |
| Output templates | ❌ Not supported | ✅ Supported |
| ignoreGlobal option | ❌ Not applicable | ✅ Supported |

### Migrating from Legacy to JSON Configuration

To migrate from legacy mode to the full JSON configuration:

#### Step 1: Create Global Configuration

Create `global.config.json` in your AgentsPath:

```json
{
  "version": "1.0",
  "workspaceIntegration": {
    "enabled": true,
    "copilotInstructions": "copilot-instructions.md",
    "instructionFiles": [
      "instructions:instructions/*.instructions.md"
    ]
  }
}
```

#### Step 2: Create Agent Folders

For each instruction file, create an agent folder:

```bash
# For code-review.instructions.md
mkdir -p agents/code-review

# Move/copy instruction file
cp instructions/code-review.instructions.md agents/code-review/instruction.md
```

#### Step 3: Create Agent Configuration

**File:** `agents/code-review/agent.json`

```json
{
  "name": "Code Review Agent",
  "instruction": "instruction.md",
  "ignoreGlobal": false
}
```

#### Step 4: Test Migration

1. Click **Tools → Shadow Pilot → Import Agents**
2. Verify all agents appear with correct names
3. Test each agent to ensure functionality

### Legacy Mode Quick Reference

| Task | How To |
|------|--------|
| Add new agent | Create `<name>.instructions.md` in `instructions/` folder |
| Rename agent | Rename the instruction file |
| Remove agent | Delete the instruction file |
| Set global context | Edit `copilot-instructions.md` in AgentsPath root |
| Refresh agents | Click **Tools → Shadow Pilot → Import Agents** |

### Legacy Mode Example

**Complete Legacy Setup:**
```
C:\ShadowPilot\Agents\
├── copilot-instructions.md
└── instructions/
    ├── code-review.instructions.md
    ├── clean-code.instructions.md
    ├── unit-test-generator.instructions.md
    ├── documentation-writer.instructions.md
    └── security-audit.instructions.md
```

**Tools → Shadow Pilot Menu:**
- Import Agents
- Code Review
- Clean Code
- Unit Test Generator
- Documentation Writer
- Security Audit

[⬆ Back to Top](#table-of-contents)

---