# ETL Deployment Tools Suite

A desktop application suite built with WPF and .NET 6.0 to automate deployment tasks for ETL projects.

## 🚀 Features

### Version Increase Tool (US-1.1 & US-1.2) ✅
- **Scan Projects**: Automatically scan for `.csproj` files ending with "ETL" (excludes files starting with "Share")
- **Version Management**: Read current versions from `AssemblyVersion` tags
- **Automatic Version Increment**: 
  - Format: `yyyy.M.d.{number}`
  - Same day: increment build number
  - New day: reset build number to 1
- **Batch Update**: Update multiple projects at once
- **Real-time Progress**: Progress bar and status updates during operations

## 🏗️ Architecture

Built using **Clean Architecture** with feature-based organization:

```
src/
├── Lifes.Core/               # Shared interfaces and models
│   ├── Interfaces/             # IProjectScanner, IProjectFileService, IVersionService
│   └── Models/                 # Result<T> pattern
│
├── Lifes.Domain/            # Domain logic and entities
│   └── Features/VersionIncrease/
│       ├── Entities/           # ProjectFile
│       ├── ValueObjects/       # VersionInfo (with increment logic)
│       └── Enums/              # ProjectStatus
│
├── Lifes.Infrastructure/    # External services implementation
│   └── Features/VersionIncrease/Services/
│       ├── ProjectScanner.cs   # File system scanning
│       ├── ProjectFileService.cs # XML file manipulation
│       └── VersionService.cs   # Version parsing and formatting
│
├── Lifes.Application/       # Use cases and commands
│   └── Features/VersionIncrease/
│       ├── Commands/           # ScanProjectsCommand, UpdateVersionsCommand
│       └── DTOs/               # Data transfer objects
│
└── Lifes.Presentation.WPF/ # UI layer
    ├── Features/VersionIncrease/
    │   ├── VersionIncreaseView.xaml
    │   ├── VersionIncreaseViewModel.cs
    │   └── Models/             # ProjectFileViewModel
    ├── App.xaml                # DI configuration
    └── MainWindow.xaml
```

### SOLID Principles
- ✅ **Single Responsibility**: Each class has one reason to change
- ✅ **Open/Closed**: Open for extension, closed for modification
- ✅ **Liskov Substitution**: Implementations can replace interfaces
- ✅ **Interface Segregation**: Small, focused interfaces
- ✅ **Dependency Inversion**: Depend on abstractions, not concretions

### Dependency Flow
```
Presentation → Application → Domain ← Infrastructure
           ↘              ↗
              Core (Shared)
```

## 🛠️ Technology Stack

| Component | Technology | Version |
|-----------|-----------|---------|
| Framework | .NET | 6.0 |
| UI | WPF (XAML) | 6.0 |
| MVVM | CommunityToolkit.Mvvm | 8.3.2 |
| DI | Microsoft.Extensions.DependencyInjection | 7.0.0 |
| Logging | Serilog | 4.3.0 |
| XML Processing | System.Xml.Linq | Built-in |

## 📦 Installation

### Prerequisites
- .NET 6.0 SDK or Runtime
- Windows 10/11

### Build from Source

```bash
# Clone the repository
git clone <repository-url>
cd Lifes

# Restore NuGet packages
dotnet restore

# Build the solution
dotnet build

# Run the application
dotnet run --project src/Lifes.Presentation.WPF/Lifes.Presentation.WPF.csproj
```

## 💻 Usage

### Version Increase Tool

1. **Select Base Directory**
   - Click "Browse..." to select the root directory containing your .csproj files
   - Or manually enter the path

2. **Scan for Projects**
   - Click "Scan Projects" to find all matching .csproj files
   - Filter: Files ending with "ETL" (excludes files starting with "Share")

3. **Select Projects**
   - Check individual projects or use "Select All" / "Deselect All"
   - View current version, status, and path for each project

4. **Increase Versions**
   - Click "Increase Version" to update selected projects
   - Confirm the operation
   - Watch real-time progress as versions are updated

### Version Format

**Format**: `yyyy.M.d.{number}`

**Examples**:
- `2026.2.4.1` - First build on February 4, 2026
- `2026.2.4.2` - Second build on February 4, 2026
- `2026.12.25.1` - First build on December 25, 2026

**Logic**:
- **Same day**: `2026.2.4.1` → `2026.2.4.2` (increment build number)
- **New day**: `2026.2.3.5` → `2026.2.4.1` (reset build number to 1)

## 📝 Project Files Updated

The tool updates both version tags in `.csproj` files:
- `<AssemblyVersion>` - Assembly version
- `<FileVersion>` - File version (created if not exists)

XML formatting and indentation are preserved.

## 🔍 Filter Rules

### ✅ Included:
- `MyProject.ETL.csproj`
- `DataSync.ETL.csproj`
- `TransformETL.csproj`

### ❌ Excluded:
- `ShareUtil.ETL.csproj` (starts with "Share")
- `SHAREDLIB.ETL.csproj` (starts with "share", case-insensitive)
- `MyProject.csproj` (doesn't end with ETL)
- `MyProject.API.csproj` (doesn't end with ETL)

## 📊 Logging

Logs are written to:
- **Console**: Real-time logging during development
- **File**: `logs/Lifes-{Date}.txt` (rolling daily, 30-day retention)

## 🧪 Testing

```bash
# Run all tests
dotnet test

# Run tests for specific project
dotnet test tests/Lifes.Domain.Tests
dotnet test tests/Lifes.Infrastructure.Tests
```

## 📚 Documentation

- [PRD.md](PRD.md) - Product Requirements Document
- [rule.md](rule.md) - Coding Rules and Standards
- [work_rule.md](work_rule.md) - Development Workflow
- [docs/user-stories/](docs/user-stories/) - User Stories
- [docs/structures/](docs/structures/) - Architecture Documentation

## 🗺️ Roadmap

### ✅ Completed
- US-1.1: Load và Hiển thị Danh Sách Projects
- US-1.2: Tăng Version Number Tự Động

### 🚧 In Progress
- US-1.3: Git Commit và Push Changes

### 📋 Planned
- US-1.4: Logging và Error Handling (Enhanced)
- US-1.5: Settings và Configuration
- Tool 2: Build & Deploy Tool
- Tool 3: Database Migration Tool

## 🤝 Contributing

1. Follow the coding standards in [rule.md](rule.md)
2. Follow the development workflow in [work_rule.md](work_rule.md)
3. Write unit tests for new features
4. Update documentation

## 📄 License

[License information]

## 👥 Authors

- Development Team

---

**Last Updated**: 2026-02-04  
**Version**: 1.0.0
