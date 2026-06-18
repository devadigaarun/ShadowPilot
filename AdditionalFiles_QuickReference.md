# Additional Files - Quick Reference

## Two Formats Supported

### Format 1: Dictionary (Custom Mapping)
```json
"additionalFiles": {
  "source/path.md": "destination/path.md"
}
```
**Use when**: Source and destination paths are different

### Format 2: List (Preserve Structure)
```json
"additionalFiles": [
  "path/to/file.md"
]
```
**Use when**: Source and destination paths are the same

---

## Examples

### List Format (Simple)
```json
{
  "workspaceIntegration": {
    "additionalFiles": [
      "templates/output-template.md",
      "docs/guide.md",
      "policies/security.md"
    ]
  }
}
```

**Result**:
```
.github/
??? templates/output-template.md
??? docs/guide.md
??? policies/security.md
```

### Dictionary Format (Custom Paths)
```json
{
  "workspaceIntegration": {
    "additionalFiles": {
      "templates/default.md": "templates/output.md",
      "README.md": "agent-readme.md"
    }
  }
}
```

**Result**:
```
.github/
??? templates/output.md        (from default.md)
??? agent-readme.md             (from README.md)
```

### List with Wildcards
```json
{
  "workspaceIntegration": {
    "additionalFiles": [
      "templates/*.md",
      "docs/*.md"
    ]
  }
}
```

**Result**: All matching files copied preserving structure

---

## Quick Decision Guide

**Preserve folder structure?**
- ? Yes ? Use **List format**
- ? No ? Use **Dictionary format**

**Custom file names/paths?**
- ? Yes ? Use **Dictionary format**
- ? No ? Use **List format**

**Simple configuration?**
- ? Yes ? Use **List format**
- ? No ? Either format works

---

## Your Current Configuration

```json
"additionalFiles": {
  "templates/default-output-template.md": "templates/default-output-template.md"
}
```

### Can Be Simplified To:
```json
"additionalFiles": [
  "templates/default-output-template.md"
]
```

**Same result, simpler syntax!**

---

## Common Patterns

### Copy All Templates
```json
"additionalFiles": [
  "templates/*.md"
]
```

### Copy Specific Files
```json
"additionalFiles": [
  "templates/output.md",
  "docs/guide.md",
  "policies/security.md"
]
```

### Copy with Renaming
```json
"additionalFiles": {
  "company-readme.md": "README.md",
  "internal-guide.md": "guide.md"
}
```

### Copy Multiple Categories
```json
"additionalFiles": [
  "templates/*.md",
  "docs/*.md",
  "policies/*.md"
]
```

---

## See Also
- [AdditionalFiles_ListFormat.md](AdditionalFiles_ListFormat.md) - Complete documentation
- [WorkspaceIntegration_Configuration.md](WorkspaceIntegration_Configuration.md) - Full workspace integration guide
