# Work Rules & Development Workflow

## 📋 Document Information

| Field | Value |
|-------|-------|
| **Document Name** | Development Workflow & Rules |
| **Version** | 1.0.0 |
| **Last Updated** | 2026-02-03 |
| **Purpose** | Define standard workflow for planning, implementing, and documenting features |
| **Target Audience** | Developers, AI Agents |

---

## 🎯 Overview

Tài liệu này định nghĩa quy trình làm việc chuẩn cho việc phát triển ETL Deployment Tools Suite, từ planning, implementation, đến documentation. Mỗi feature được phát triển theo 4 bước chính:

1. **Planning** - Phân tích requirement và tạo User Story
2. **Implementation** - Implement User Story theo Clean Architecture
3. **Review** - User review và feedback
4. **Documentation** - Update PRD và structure documents

---

## 📖 Table of Contents

1. [Step 1: Planning](#step-1-planning)
2. [Step 2: Implementation](#step-2-implementation)
3. [Step 3: Review](#step-3-review)
4. [Step 4: Documentation](#step-4-documentation)
5. [File Organization](#file-organization)
6. [Naming Conventions](#naming-conventions)
7. [Best Practices](#best-practices)
8. [Checklist](#checklist)

---

## Step 1: Planning

### 1.1 Input Requirements

User cung cấp:
- ✅ **Requirements**: Mô tả tính năng cần implement
- ✅ **Business context**: Mục đích, lợi ích của feature
- ✅ **Technical constraints**: Công nghệ, kiến trúc, limitations
- ✅ **Rule.md reference**: Reference đến coding rules và conventions
- ✅ **design_rule.md reference**: Tham khảo các quy tắc thiết kế UI/UX và logic Presentation (Event-driven vs Delegate)

### 1.2 Analysis & User Story Creation

AI Agent thực hiện:

1. **Phân tích requirements**
   - Hiểu rõ business problem
   - Xác định acceptance criteria
   - Identify technical challenges
   - Estimate complexity

2. **Tạo User Story**
   - Format: Chuẩn Agile User Story format
   - Bao gồm:
     - User Story ID (vd: US-1.1, US-2.3)
     - Title: Tên ngắn gọn của story
     - As a/I want to/So that format
     - Acceptance Criteria (chi tiết, measurable)
     - Technical Design (Clean Architecture layers)
     - Tasks breakdown
     - Definition of Done

3. **Generate User Story Document**
   - Location: `docs/user-stories/us-{story-id}/`
   - Filename: `user-story-{story-id}.md`

### 1.3 User Story Structure

```markdown
# User Story: US-{story-id}

## Story Information
- **ID**: US-{story-id}
- **Title**: [Tên feature]
- **Priority**: High/Medium/Low
- **Estimate**: [Story points hoặc hours]
- **Sprint**: [Sprint number nếu có]

## User Story
- **As a** [user role]
- **I want to** [action/feature]
- **So that** [business value/benefit]

## Acceptance Criteria
1. Given [precondition]
   When [action]
   Then [expected result]
2. ...

## Technical Design

### Clean Architecture Layers
- **Presentation**: [ViewModels, Views cần tạo]
- **Application**: [Commands, Queries, DTOs]
- **Domain**: [Entities, Value Objects, Business Rules]
- **Infrastructure**: [Services, External integrations]
- **Core**: [Interfaces, Shared models]

### Files to Create/Modify
- [ ] `Features/[Feature]/[FileName].cs`
- [ ] ...

## Tasks Breakdown
- [ ] Task 1: [Description]
- [ ] Task 2: [Description]
- [ ] ...

## Dependencies
- Depends on: [Other User Stories]
- Blocked by: [Issues or dependencies]

## Definition of Done
- [ ] Code implemented following Clean Architecture
- [ ] Unit tests written and passing
- [ ] Integration tests (if applicable)
- [ ] Code reviewed
- [ ] Documentation updated
- [ ] User Story marked as complete
```

### 1.4 Review & Approval

1. **AI Agent gửi User Story cho user review**
   - Giải thích approach và design decisions
   - Highlight potential risks hoặc challenges
   - Suggest alternatives nếu có

2. **User review và feedback**
   - User đọc và hiểu User Story
   - Provide feedback hoặc request changes
   - Approve hoặc reject approach

3. **Finalize User Story**
   - Incorporate feedback
   - Get final approval từ user
   - Ready to implement

### 1.5 Story ID Convention

**Format**: `US-{major}.{minor}`

**Examples:**
- `US-1.1`: Version Increase Tool - Load Projects
- `US-1.2`: Version Increase Tool - Update Versions
- `US-1.3`: Version Increase Tool - Git Commit
- `US-2.1`: Build Deploy Tool - Build Projects
- `US-2.2`: Build Deploy Tool - Deploy to Environment

**Rules:**
- **Major number**: Nhóm features liên quan (vd: 1 = Version Tool, 2 = Build Tool)
- **Minor number**: Sequential số thứ tự trong major group
- User cung cấp Story ID, nếu không có thì **phải hỏi lại user**

---

## Step 2: Implementation

### 2.1 Pre-Implementation Setup

1. **User cung cấp User Story muốn implement**
   - Reference: `docs/user-stories/us-{story-id}/us-{story-id}.md`

2. **AI Agent đọc và understand**
   - Đọc file `rule.md` (coding rules và conventions)
   - Đọc file `design_rule.md` (quy tắc thiết kế và xử lý logic UI)
   - Đọc file `PRD.md` (kiến trúc và design system)
   - Đọc User Story chi tiết
   - Identify all tasks cần làm

3. **Create working folder**
   - Location: `docs/user-stories/us-{story-id}/`
   - All generated files during implementation sẽ được log tại đây

### 2.2 Implementation Process

**Follow Clean Architecture principles:**

1. **Domain Layer First** (Core Business Logic)
   ```
   src/Lifes.Domain/Features/{Feature}/
   ├── Entities/
   ├── ValueObjects/
   └── Enums/
   ```

2. **Application Layer** (Use Cases)
   ```
   src/Lifes.Application/Features/{Feature}/
   ├── Commands/
   ├── Queries/
   └── DTOs/
   ```

3. **Infrastructure Layer** (External Services)
   ```
   src/Lifes.Infrastructure/Features/{Feature}/
   └── Services/
   ```

4. **Presentation Layer** (UI)
   ```
   src/Lifes.Presentation.WPF/Features/{Feature}/
   ├── [Feature]View.xaml
   ├── [Feature]ViewModel.cs
   └── Models/
   ```

5. **Core Layer** (Shared Interfaces)
   ```
   src/Lifes.Core/
   ├── Interfaces/
   └── Models/
   ```

**Implementation Guidelines:**

- ✅ Follow SOLID principles
- ✅ Use dependency injection
- ✅ Implement Result pattern for error handling
- ✅ Write clean, readable code with comments
- ✅ Follow naming conventions (see below)
- ✅ Add XML documentation for public APIs
- ✅ Create unit tests for business logic

### 2.3 Progress Tracking

Trong quá trình implement, update User Story document:

```markdown
## Implementation Progress

### Files Created
- [x] `Domain/Features/VersionIncrease/Entities/ProjectFile.cs`
- [x] `Application/Features/VersionIncrease/Commands/ScanProjectsCommand.cs`
- [ ] `Infrastructure/Features/VersionIncrease/Services/ProjectScanner.cs` (In Progress)

### Current Status
- **Status**: In Progress
- **Completed**: 60%
- **Blockers**: None
- **Notes**: Implementing ProjectScanner service
```

### 2.4 Code Quality Checks

Before marking task as complete:

- [ ] Code compiles without errors
- [ ] No linter warnings
- [ ] Unit tests pass
- [ ] Code follows Clean Architecture
- [ ] Dependencies point in correct direction (inward)
- [ ] No circular dependencies
- [ ] Proper error handling with Result pattern
- [ ] XML documentation added

---

## Step 3: Review

### 3.1 User Review Process

**User reviews:**

1. **Code Changes**
   - Review all modified/created files
   - Check code quality và readability
   - Verify Clean Architecture compliance
   - Check naming conventions

2. **Functionality**
   - Test feature manually
   - Verify acceptance criteria met
   - Check edge cases và error handling
   - Validate UI/UX (nếu có)

3. **Documentation**
   - Review User Story updates
   - Check implementation notes
   - Verify all tasks completed

### 3.2 Feedback & Iteration

**If issues found:**

1. User provides specific feedback
   - What's wrong
   - Expected behavior
   - Suggestions for improvement

2. AI Agent addresses feedback
   - Fix issues
   - Update code
   - Re-test

3. Repeat review process until approved

### 3.3 Approval & Commit

**When approved:**

1. **Mark User Story as Complete**
   ```markdown
   ## Final Status
   - **Status**: ✅ Completed
   - **Completed Date**: 2026-02-03
   - **Approved By**: [User name]
   ```

2. **Commit Changes**
   - User hoặc AI Agent commit code
   - Commit message format:
     ```
     feat(us-1.1): implement project scanning feature
     
     - Add ProjectScanner service
     - Add ScanProjectsCommand
     - Add ProjectFile entity
     - Add unit tests
     
     Closes US-1.1
     ```

---

## Step 4: Documentation

### 4.1 Trigger

**Command**: `"updoc"` hoặc `"update documentation"`

**When**: Sau khi User Story được approved và committed

### 4.2 Update PRD.md

Update Product Requirements Document với thông tin mới:

1. **Roadmap Section**
   - Mark completed features as ✅
   - Update progress percentages
   - Update status

2. **Implementation Section** (nếu cần)
   - Add implementation examples
   - Document new patterns
   - Add code snippets

3. **Known Issues** (nếu có)
   - Document limitations
   - Add workarounds

**Example:**
```markdown
### Phase 1: Version Increase Tool - v1.0.0 (Current Sprint)

**Tool 1: Version Increase Tool**
- ✅ Project scanning và filtering (US-1.1) - Completed 2026-02-03
- ⏳ Version parsing và increment logic (US-1.2) - In Progress
- 📋 Batch update multiple .csproj files (US-1.3)
- 📋 Git integration (US-1.4)
```

### 4.3 Update Structure Documents

#### 4.3.1 Feature Structure Document

**Location**: `docs/structures/fea-{feature}-structure.md`

**Purpose**: Document chi tiết về feature architecture và file responsibilities

**Format**:
```markdown
# Feature: {Feature Name}

## Overview
[Mô tả feature]

## Architecture

### Presentation Layer
- `{FileName}.xaml` - [Purpose]
- `{FileName}ViewModel.cs` - [Purpose]

### Application Layer
- `Commands/{CommandName}.cs` - [Purpose]
- `Queries/{QueryName}.cs` - [Purpose]
- `DTOs/{DtoName}.cs` - [Purpose]

### Domain Layer
- `Entities/{EntityName}.cs` - [Purpose]
- `ValueObjects/{VOName}.cs` - [Purpose]

### Infrastructure Layer
- `Services/{ServiceName}.cs` - [Purpose]

### Core Layer
- `Interfaces/I{InterfaceName}.cs` - [Purpose]

## Key Classes

### {ClassName}
**Location**: `{FilePath}`
**Purpose**: [What it does]
**Dependencies**: [What it depends on]
**Used by**: [What uses it]

## Data Flow
[Mô tả data flow qua các layers]

## Key Decisions
[Document important technical decisions]
```

**If file chưa có**: Tạo file mới `fea-{feature}-structure.md`

**If file đã có**: Append thông tin mới hoặc update existing sections

#### 4.3.2 Overall Backend Structure

**Location**: `docs/structures/be-all-structure.md`

**Purpose**: Overview toàn bộ backend structure của application

**Format**:
```markdown
# Backend Structure - Overall

## Project Structure

```
src/
├── Lifes.Presentation.WPF/
├── Lifes.Application/
├── Lifes.Domain/
├── Lifes.Infrastructure/
└── Lifes.Core/
```

## Features

### 1. Version Increase Tool
**Status**: ✅ Completed
**User Stories**: US-1.1, US-1.2, US-1.3, US-1.4
**Documentation**: [fea-version-increase-structure.md](./fea-version-increase-structure.md)
**Key Components**:
- ProjectScanner
- VersionService
- GitService

### 2. Build Deploy Tool
**Status**: 📋 Planned
**User Stories**: US-2.1, US-2.2
**Documentation**: TBD

## Shared Components

### Core Layer
- `IProjectScanner` - Interface for project scanning
- `Result<T>` - Result pattern for error handling

### Common Infrastructure
- `WpfListViewSink` - Logging to WPF ListView
- `FileSystemService` - File operations
```

### 4.4 Documentation Checklist

- [ ] PRD.md updated with completed features
- [ ] Feature structure document created/updated
- [ ] Overall structure document updated
- [ ] Code examples added (if needed)
- [ ] Diagrams updated (if architecture changed)
- [ ] Known issues documented
- [ ] All links working

---

## File Organization

### Directory Structure

```
Lifes/
├── src/                                    # Source code
│   ├── Lifes.Presentation.WPF/
│   ├── Lifes.Application/
│   ├── Lifes.Domain/
│   ├── Lifes.Infrastructure/
│   └── Lifes.Core/
│
├── tests/                                  # Test projects
│   ├── Lifes.Application.Tests/
│   ├── Lifes.Domain.Tests/
│   └── Lifes.Infrastructure.Tests/
│
├── docs/                                   # Documentation
│   ├── user-stories/                      # User stories
│   │   ├── us-1.1/
│   │   │   ├── user-story-1.1.md
│   │   │   └── implementation-log.md
│   │   ├── us-1.2/
│   │   └── ...
│   │
│   ├── structures/                         # Structure documents
│   │   ├── be-all-structure.md            # Overall structure
│   │   ├── fea-version-increase-structure.md
│   │   └── fea-build-deploy-structure.md
│   │
│   └── diagrams/                           # Architecture diagrams
│       ├── clean-architecture.png
│       └── version-increase-flow.png
│
├── PRD.md                                  # Product Requirements
├── work_rule.md                           # This file
├── rule.md                                # Coding rules
└── README.md                              # Project readme
```

### Feature Organization

Each feature follows Clean Architecture with feature-based folders:

```
src/Lifes.{Layer}/Features/{Feature}/
├── Commands/                   # Application layer
├── Queries/                    # Application layer
├── Entities/                   # Domain layer
├── Services/                   # Infrastructure layer
├── {Feature}View.xaml         # Presentation layer
└── {Feature}ViewModel.cs      # Presentation layer
```

---

## Naming Conventions

### User Story IDs
- Format: `US-{major}.{minor}`
- Example: `US-1.1`, `US-2.3`

### Folders
- `us-{story-id}` - User story folder (vd: `us-1.1`)
- `fea-{feature}` - Feature structure doc (vd: `fea-version-increase`)

### Files

**User Stories:**
- `user-story-{story-id}.md` - User story document
- `implementation-log.md` - Implementation notes

**Structure Documents:**
- `be-all-structure.md` - Overall backend structure
- `fea-{feature}-structure.md` - Feature structure detail

**Code Files:**
- `{Name}Command.cs` - Command (Application layer)
- `{Name}Query.cs` - Query (Application layer)
- `{Name}Dto.cs` - Data Transfer Object
- `{Name}Entity.cs` - Domain entity
- `{Name}Service.cs` - Service implementation
- `I{Name}.cs` - Interface (Core layer)
- `{Name}View.xaml` - WPF view
- `{Name}ViewModel.cs` - WPF view model

### Commit Messages

**Format**: `{type}({scope}): {subject}`

**Types:**
- `feat` - New feature
- `fix` - Bug fix
- `docs` - Documentation only
- `refactor` - Code refactoring
- `test` - Adding tests
- `chore` - Maintenance

**Examples:**
```
feat(us-1.1): implement project scanning
fix(us-1.2): correct version increment logic
docs(prd): update roadmap with completed features
refactor(infrastructure): extract git service interface
```

---

## Best Practices

### Planning Phase
- ✅ Thoroughly analyze requirements trước khi tạo User Story
- ✅ Hỏi rõ nếu requirements không clear
- ✅ Break down complex features thành smaller User Stories
- ✅ Get user approval trước khi implement

### Implementation Phase
- ✅ Start với Domain layer (core business logic)
- ✅ Follow dependency inversion (depend on abstractions)
- ✅ Use Result pattern cho error handling
- ✅ Write unit tests cho business logic
- ✅ Keep ViewModels thin (delegate to Application layer)
- ✅ Log progress trong User Story document

### Review Phase
- ✅ Test manually trước khi request review
- ✅ Check code quality tools (linter, formatter)
- ✅ Verify all acceptance criteria met
- ✅ Document any known limitations

### Documentation Phase
- ✅ Update documents ngay sau khi feature completed
- ✅ Keep structure documents up-to-date
- ✅ Add examples và code snippets
- ✅ Document design decisions

### General
- ✅ Consistent naming conventions
- ✅ Clear separation of concerns
- ✅ Feature-based organization
- ✅ Regular commits với meaningful messages
- ✅ Keep documentation in sync với code

---

## Checklist

### Planning Checklist
- [ ] Requirements received từ user
- [ ] Requirements analyzed và understood
- [ ] User Story ID assigned (ask user if not provided)
- [ ] User Story created với đầy đủ thông tin:
  - [ ] Story Information
  - [ ] User Story format (As a/I want/So that)
  - [ ] Acceptance Criteria
  - [ ] Technical Design
  - [ ] Tasks Breakdown
  - [ ] Definition of Done
- [ ] User Story folder created: `docs/user-stories/us-{story-id}/`
- [ ] User Story document saved: `user-story-{story-id}.md`
- [ ] User Story sent to user for review
- [ ] User feedback incorporated
- [ ] User Story approved

### Implementation Checklist
- [ ] User Story read và understood
- [ ] Coding rules (`rule.md`) read
- [ ] PRD architecture understood
- [ ] Domain layer implemented
- [ ] Application layer implemented
- [ ] Infrastructure layer implemented
- [ ] Presentation layer implemented
- [ ] Core interfaces defined
- [ ] Unit tests written
- [ ] Code quality checks passed
- [ ] User Story progress updated
- [ ] All tasks marked complete

### Review Checklist
- [ ] Code changes reviewed
- [ ] Functionality tested manually
- [ ] Acceptance criteria verified
- [ ] Edge cases tested
- [ ] Error handling verified
- [ ] Clean Architecture compliance checked
- [ ] Naming conventions followed
- [ ] User feedback received
- [ ] Issues addressed
- [ ] Final approval received

### Documentation Checklist
- [ ] PRD.md updated:
  - [ ] Roadmap section
  - [ ] Implementation examples (if needed)
  - [ ] Known issues (if any)
- [ ] Feature structure document created/updated:
  - [ ] `docs/structures/fea-{feature}-structure.md`
  - [ ] Architecture overview
  - [ ] File responsibilities documented
  - [ ] Key classes documented
  - [ ] Data flow explained
- [ ] Overall structure updated:
  - [ ] `docs/structures/be-all-structure.md`
  - [ ] New feature added
  - [ ] Status updated
  - [ ] Key components listed
- [ ] Code examples added (if needed)
- [ ] All links working
- [ ] Documentation reviewed

---

## Appendix

### Quick Reference

#### 📚 Core Documents

**User Story Location**: `docs/user-stories/us-{story-id}/user-story-{story-id}.md`

**Feature Structure**: `docs/structures/fea-{feature}-structure.md`

**Overall Structure**: `docs/structures/be-all-structure.md`

**Product Requirements**: `PRD.md`

**Coding Rules**: `rule.md`

**Design Rules**: `docs/guidelines/design_rule.md` (Quy tắc xử lý logic CRUD UI)

#### 🔄 Workflow Documents

**Coding Workflow**: `docs/work_flows/wf_coding.md`

**Testing Workflow**: `docs/work_flows/wf_write_test.md`

#### 🧪 Testing Documents

**Testing Guidelines**: `docs/guidelines/testing-guidelines.md`

**Testing Structure**: `docs/structures/testing-structure.md`

**Test Scripts**: `tasks/run-tests/` - PowerShell scripts để run tests

#### ⚙️ Automation & Tasks

**Task Overview**: `docs/structures/task_structured.md`
**Task Rules & Guidelines**: `docs/structures/tasks_rule.md`

**Purpose**: Khi làm việc với automation:
- **task_structured.md**: Overview về structure, implemented tasks, quick start
- **tasks_rule.md**: Chi tiết rules, guidelines, best practices, templates

**When to use**:
- Tạo automation tasks mới → follow `tasks_rule.md`
- Hiểu tổng thể automation → xem `task_structured.md`
- Follow script template và best practices
- Tạo documentation đầy đủ (README, QUICK-START)
- Ensure scripts work from anywhere (auto-navigate to project root)
- Implement proper error handling và user experience

**Example Tasks**:
- `tasks/run-tests/` - Test automation scripts (US-2.1) ✅
- `tasks/build-deploy/` - Build & deploy scripts (US-3.1) ✅
- `tasks/maintenance/` - Maintenance scripts (future) 📋

### Common Commands

#### Development Workflow

```bash
# Planning
"Create user story for [feature description]"

# Implementation
"Implement user story US-1.1"

# Review
"Review implementation for US-1.1"

# Documentation
"update document"
"update documentation for US-1.1"
```

#### Automation Tasks

```powershell
# Main menu (recommended)
.\run.ps1                                           # Launch automation menu

# Run tests
.\test-quick.ps1                                    # Quick test from root
.\test.ps1                                          # Full test + coverage
.\tasks\run-tests\run-tests-quick.ps1 -Watch       # Watch mode

# Build & Deploy
.\tasks\build-deploy\build.ps1                     # Build application
.\tasks\build-deploy\build-deploy.ps1              # Build & deploy
.\tasks\build-deploy\build-deploy.ps1 -Clean       # Clean build & deploy

# Create new automation task (follow tasks_rule.md)
# 1. Create folder: tasks/[task-name]/
# 2. Create main script with template
# 3. Add README.md documentation
# 4. Test from different locations
# 5. Update work_rule.md Quick Reference
```

### Example Workflow

```
User: "Tôi muốn implement tính năng scan .csproj files"

AI Agent:
1. Analyze requirement
2. Create User Story US-1.1 in docs/user-stories/us-1.1/
3. Send for review

User: "Approve"

AI Agent:
1. Implement feature theo Clean Architecture
2. Update progress trong user-story-{story-id}.md
3. Run tests
4. Request review

User: "Looks good, approve"

AI Agent:
1. Mark US-1.1 as completed
2. Commit code with message: feat(us-1.1): implement project scanning

User: "update document"

AI Agent:
1. Update PRD.md roadmap
2. Create docs/structures/fea-version-increase-structure.md
3. Update docs/structures/be-all-structure.md
```

---

**Document Version**: 1.3.0  
**Last Updated**: 2026-02-06  
**Status**: ✅ Active  
**Changes**:
- v1.1.0: Added Automation & Tasks section in Quick Reference
- v1.2.0: Added Build & Deploy tasks (US-3.1), updated Common Commands with main menu
- v1.3.0: Reorganized automation docs - split into task_structured.md (overview) and tasks_rule.md (rules)
