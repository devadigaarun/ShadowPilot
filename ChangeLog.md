# Changelog

All notable changes to the **ShadowPilot** extension will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [1.0.0] - 2025-01-XX

### 🎉 Initial Release

#### Added
- **Dynamic Agent Menu System**
  - Automatically discovers and loads AI agent definitions from `D:\Agents\*.md`
  - Generates menu items dynamically under `Extensions → Copilot Agents`
  - Supports unlimited number of custom agents

- **GitHub Copilot Integration**
  - Programmatically opens GitHub Copilot Chat window
  - Injects agent instructions directly into chat input
  - Maintains full Copilot context and functionality

- **Real-Time Agent Updates**
  - Menu automatically refreshes when agent files are added or modified
  - No need to restart Visual Studio
  - `BeforeQueryStatus` event ensures menu is always up-to-date

- **File-Based Agent Storage**
  - Agents defined as simple markdown files
  - Easy to create, edit, and version control
  - Supports team collaboration through shared agent definitions

- **Error Handling**
  - Graceful fallback when agents folder doesn't exist
  - User-friendly error messages for missing agent files
  - Debug logging for troubleshooting

#### Technical Details
- Built for Visual Studio 2017+ (targeting Visual Studio 2022)
- Targets .NET Framework 4.7.2
- Utilizes VS SDK for editor and text manager services
- Implements `AsyncPackage` for non-blocking initialization

#### Components
- `ShadowPilotPackage.cs` - Main package and initialization
- `DynamicAgentCommand.cs` - Dynamic menu generation and agent loading
- `AgentCommand.cs` - Command infrastructure

---

## [Unreleased]

### Planned Features
- [ ] Configurable agent folder location (user preferences)
- [ ] Agent templates library
- [ ] Agent categories and grouping in menu
- [ ] Agent validation and linting
- [ ] Cross-platform path support (macOS, Linux)
- [ ] UI for agent management and creation
- [ ] Agent usage analytics
- [ ] Multi-language agent support
- [ ] Agent inheritance/composition
- [ ] Quick access keyboard shortcuts
- [ ] Agent search functionality

### Under Consideration
- Integration with other AI services
- Agent marketplace/sharing platform
- Visual agent editor
- Agent testing framework
- Performance monitoring
- Agent recommendation system based on context

---

## Version History

### Version Numbering Convention
- **Major (X.0.0)** - Breaking changes or major feature additions
- **Minor (1.X.0)** - New features, backward compatible
- **Patch (1.0.X)** - Bug fixes and minor improvements

---

## Contributing

Found a bug or have a feature request? Please check the project repository for contribution guidelines.

---

## Support

For issues and support, please refer to the project's GitHub repository or contact the development team.

---

**Last Updated:** January 2025
