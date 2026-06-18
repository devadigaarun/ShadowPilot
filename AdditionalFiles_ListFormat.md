# Additional Files - List Format Support

## Overview
The `additionalFiles` property now supports **two formats**: dictionary (key-value mapping) and list (array of file paths). This provides flexibility based on your needs.

## Format 1: Dictionary (Original)

### Syntax
```json
"additionalFiles": {
  "source/path/file.md": "destination/path/file.md",
  "another/source.md": "custom/destination.md"
}
```

### Use Case
Use dictionary format when you need **custom destination paths** different from source paths.

### Example
```json
{
  "workspaceIntegration": {
    "additionalFiles": {
      "templates/default-output-template.md": "templates/output.md",
      "README.md": "agent-readme.md",
      "company/standards.md": "standards.md"
    }
  }
}
```

### Result
| Source | Destination in .github |
|--------|------------------------|
| `templates/default-output-template.md` | `templates/output.md` |
| `README.md` | `agent-readme.md` |
| `company/standards.md` | `standards.md` |

---

## Format 2: List (New)

### Syntax
```json
"additionalFiles": [
  "path/to/file.md",
  "another/file.md",
  "templates/*.md"
]
```

### Use Case
Use list format when you want to **preserve relative path structure** - source and destination paths are the same.

### Example
```json
{
  "workspaceIntegration": {
    "additionalFiles": [
      "templates/default-output-template.md",
      "templates/code-review-template.md",
      "docs/agent-guide.md",
      "policies/security.md"
    ]
  }
}
```

### Result
| Source | Destination in .github |
|--------|------------------------|
| `templates/default-output-template.md` | `templates/default-output-template.md` |
| `templates/code-review-template.md` | `templates/code-review-template.md` |
| `docs/agent-guide.md` | `docs/agent-guide.md` |
| `policies/security.md` | `policies/security.md` |

---

## Comparison

| Feature | Dictionary Format | List Format |
|---------|------------------|-------------|
| **Syntax** | `{ "src": "dest" }` | `[ "path" ]` |
| **Custom Destinations** | ? Yes | ? No (preserves structure) |
| **Simpler Configuration** | ? More verbose | ? Concise |
| **Wildcards** | ? Supported | ? Supported |
| **Use When** | Custom mapping needed | Preserving structure |

---

## Wildcard Support

Both formats support wildcard patterns:

### Dictionary with Wildcards
```json
"additionalFiles": {
  "templates/*.md": "templates/*.md",
  "docs/*.md": "documentation/*.md"
}
```

### List with Wildcards
```json
"additionalFiles": [
  "templates/*.md",
  "docs/*.md",
  "policies/*.md"
]
```

**Note**: With list format, wildcard matches preserve their relative paths automatically.

---

## Complete Examples

### Example 1: Simple List
```json
{
  "workspaceIntegration": {
    "enabled": true,
    "additionalFiles": [
      "templates/output-template.md",
      "docs/guide.md",
      "policies/security-policy.md"
    ]
  }
}
```

**Result**:
```
.github/
??? templates/
?   ??? output-template.md
??? docs/
?   ??? guide.md
??? policies/
    ??? security-policy.md
```

### Example 2: List with Wildcards
```json
{
  "workspaceIntegration": {
    "enabled": true,
    "additionalFiles": [
      "templates/*.md",
      "docs/*.md"
    ]
  }
}
```

**Result**:
```
.github/
??? templates/
?   ??? template1.md
?   ??? template2.md
?   ??? template3.md
??? docs/
    ??? guide1.md
    ??? guide2.md
```

### Example 3: Dictionary with Custom Mapping
```json
{
  "workspaceIntegration": {
    "enabled": true,
    "additionalFiles": {
      "templates/default-output.md": "templates/output.md",
      "company-readme.md": "README.md",
      "internal/standards.md": "standards.md"
    }
  }
}
```

**Result**:
```
.github/
??? templates/
?   ??? output.md           (from default-output.md)
??? README.md               (from company-readme.md)
??? standards.md            (from internal/standards.md)
```

### Example 4: Mixed Content (Dictionary)
```json
{
  "workspaceIntegration": {
    "enabled": true,
    "additionalFiles": {
      "templates/*.md": "templates/*.md",
      "README.md": "agent-info.md",
      "docs/guide.md": "documentation/guide.md"
    }
  }
}
```

---

## Migration Guide

### From Dictionary to List

**Before** (Dictionary):
```json
"additionalFiles": {
  "templates/output.md": "templates/output.md",
  "docs/guide.md": "docs/guide.md",
  "policies/security.md": "policies/security.md"
}
```

**After** (List):
```json
"additionalFiles": [
  "templates/output.md",
  "docs/guide.md",
  "policies/security.md"
]
```

**Benefit**: Simpler, more concise when paths don't need remapping.

---

## When to Use Each Format

### Use Dictionary Format When:
- ? You need to rename files during copy
- ? You need to reorganize folder structure
- ? Source and destination paths are different
- ? You want explicit control over destinations

**Example Scenario**: Copy company README as agent-readme, reorganize docs

### Use List Format When:
- ? You want to preserve folder structure
- ? Source and destination paths are the same
- ? Configuration should be simple and readable
- ? You're copying multiple files from same category

**Example Scenario**: Copy all templates maintaining their structure

---

## Real-World Examples

### Enterprise Setup (List Format)
```json
{
  "workspaceIntegration": {
    "additionalFiles": [
      "templates/enterprise-template.md",
      "templates/compliance-template.md",
      "policies/security-policy.md",
      "policies/data-privacy.md",
      "standards/enterprise-coding.md"
    ]
  }
}
```

### Team Setup (Dictionary Format)
```json
{
  "workspaceIntegration": {
    "additionalFiles": {
      "team-backend/README.md": "backend-readme.md",
      "team-backend/standards.md": "backend-standards.md",
      "templates/api-template.md": "templates/api.md"
    }
  }
}
```

### Hybrid Approach
You can use dictionary format with matching source/destination if you prefer explicit mapping:

```json
{
  "workspaceIntegration": {
    "additionalFiles": {
      "templates/template1.md": "templates/template1.md",
      "templates/template2.md": "templates/template2.md"
    }
  }
}
```

But list format is simpler for this case:
```json
{
  "workspaceIntegration": {
    "additionalFiles": [
      "templates/template1.md",
      "templates/template2.md"
    ]
  }
}
```

---

## Debug Output

### Dictionary Format
```
Queued for copy (dict): templates/output.md -> templates/custom-output.md
Queued for copy (dict): README.md -> agent-readme.md
```

### List Format
```
Queued for copy (list): templates/output.md -> templates/output.md
Queued for copy (list): docs/guide.md -> docs/guide.md
```

---

## Configuration Validation

Both formats are validated during loading:

### Valid Configurations ?
```json
// List format
"additionalFiles": [
  "file1.md",
  "file2.md"
]

// Dictionary format
"additionalFiles": {
  "source.md": "destination.md"
}

// Empty (valid)
"additionalFiles": []
"additionalFiles": {}
```

### Invalid Configurations ?
```json
// Mixed format (not supported)
"additionalFiles": {
  "file1.md": "dest1.md",
  "file2.md"  // ? Error: value required
}
```

---

## Best Practices

### 1. **Choose Based on Need**
- Same paths ? Use list
- Different paths ? Use dictionary

### 2. **Use Wildcards for Scalability**
```json
"additionalFiles": [
  "templates/*.md",
  "docs/*.md"
]
```

### 3. **Group Related Files**
```json
"additionalFiles": [
  "templates/template1.md",
  "templates/template2.md",
  "templates/template3.md"
]
```

### 4. **Document Your Choices**
Add comments in your configuration (note: JSON doesn't support comments, but you can use `_comment_` keys):

```json
{
  "_comment": "Using list format to preserve folder structure",
  "additionalFiles": [
    "templates/*.md"
  ]
}
```

---

## Summary

| Aspect | Dictionary | List |
|--------|-----------|------|
| **Format** | Object with key-value pairs | Array of strings |
| **Complexity** | More complex | Simpler |
| **Flexibility** | High (custom paths) | Medium (preserves structure) |
| **Verbosity** | Higher | Lower |
| **Best For** | Custom reorganization | Direct copying |

Choose the format that best matches your use case!
