using ETLTools.Application.Common.Commands;
using ETLTools.Application.Common.DTOs;
using ETLTools.Application.Features.VersionIncrease.Commands;
using ETLTools.Application.Features.VersionIncrease.DTOs;
using ETLTools.Core.Interfaces;
using ETLTools.Core.Models;
using ETLTools.Presentation.WPF.Constants;
using ETLTools.Presentation.WPF.Features.VersionIncrease;
using ETLTools.Presentation.WPF.Features.VersionIncrease.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace ETLTools.Presentation.WPF.Tests.Features.VersionIncrease;

public class VersionIncreaseViewModelTests
{
    // -------------------------------------------------------------------
    //  Shared mocks
    // -------------------------------------------------------------------

    private readonly Mock<IScanProjectsCommand> _mockScanCommand;
    private readonly Mock<IUpdateVersionsCommand> _mockUpdateCommand;
    private readonly Mock<ILoadSettingsCommand> _mockLoadSettingsCommand;
    private readonly Mock<ISaveSettingsCommand> _mockSaveSettingsCommand;
    private readonly Mock<IGitService> _mockGitService;
    private readonly Mock<INavigationService> _mockNavigationService;
    private readonly Mock<ILogger<VersionIncreaseViewModel>> _mockLogger;
    private readonly CommitChangesCommand _commitChangesCommand;

    private static readonly List<ToolDefinition> DefaultTools = new List<ToolDefinition>
    {
        new ToolDefinition
        {
            Id = ToolIds.VersionIncrease,
            Name = "Version Increase Tool",
            Description = "Increment version numbers for ETL projects"
        }
    };

    // -------------------------------------------------------------------
    //  Setup
    // -------------------------------------------------------------------

    public VersionIncreaseViewModelTests()
    {
        _mockScanCommand = new Mock<IScanProjectsCommand>();
        _mockUpdateCommand = new Mock<IUpdateVersionsCommand>();
        _mockLoadSettingsCommand = new Mock<ILoadSettingsCommand>();
        _mockSaveSettingsCommand = new Mock<ISaveSettingsCommand>();
        _mockGitService = new Mock<IGitService>();
        _mockNavigationService = new Mock<INavigationService>();
        _mockLogger = new Mock<ILogger<VersionIncreaseViewModel>>();

        _commitChangesCommand = new CommitChangesCommand(
            _mockGitService.Object,
            new Mock<ILogger<CommitChangesCommand>>().Object);

        _mockLoadSettingsCommand
            .Setup(x => x.ExecuteAsync())
            .ReturnsAsync(Result<AppSettingsDto>.Success(
                new AppSettingsDto { LastDirectory = "C:\\Test" }));

        _mockNavigationService
            .Setup(x => x.GetAllTools())
            .Returns(DefaultTools);
    }

    /// <summary>
    /// Creates a <see cref="VersionIncreaseViewModel"/> using the current mock state.
    /// Call this AFTER configuring any mock-specific behaviour for the test.
    /// </summary>
    private VersionIncreaseViewModel CreateViewModel() =>
        new(
            _mockScanCommand.Object,
            _mockUpdateCommand.Object,
            _mockLoadSettingsCommand.Object,
            _mockSaveSettingsCommand.Object,
            _commitChangesCommand,
            _mockGitService.Object,
            _mockNavigationService.Object,
            _mockLogger.Object);

    // -------------------------------------------------------------------
    //  Project scanning
    // -------------------------------------------------------------------

    [Fact]
    public async Task ScanProjectsCommand_WhenExecuted_ShouldUpdateProjectFiles()
    {
        // Arrange
        var projects = new List<ProjectFileDto>
        {
            new() { FileName = "Test1.ETL.csproj", CurrentVersion = "2026.2.3.1" },
            new() { FileName = "Test2.ETL.csproj", CurrentVersion = "2026.2.3.2" }
        };

        _mockScanCommand
            .Setup(x => x.ExecuteAsync(It.IsAny<string>()))
            .ReturnsAsync(Result<IEnumerable<ProjectFileDto>>.Success(projects));

        var viewModel = CreateViewModel();

        // Act
        await viewModel.ScanProjectsCommand.ExecuteAsync(null);

        // Assert
        viewModel.ProjectFiles.Should().HaveCount(2);
        viewModel.ProjectFiles[0].FileName.Should().Be("Test1.ETL.csproj");
        viewModel.FoundCount.Should().Be(2);
    }

    [Fact]
    public async Task ScanProjectsCommand_WhenScanFails_ShouldSetErrorStatusMessage()
    {
        // Arrange
        _mockScanCommand
            .Setup(x => x.ExecuteAsync(It.IsAny<string>()))
            .ReturnsAsync(Result<IEnumerable<ProjectFileDto>>.Failure("Scan error"));

        var viewModel = CreateViewModel();

        // Act
        await viewModel.ScanProjectsCommand.ExecuteAsync(null);

        // Assert
        viewModel.StatusMessage.Should().Contain("Error");
        viewModel.ProjectFiles.Should().BeEmpty();
    }

    // -------------------------------------------------------------------
    //  Search / filter
    // -------------------------------------------------------------------

    [Fact]
    public void SearchText_WhenChanged_ShouldFilterProjects()
    {
        // Arrange
        var viewModel = CreateViewModel();
        viewModel.ProjectFiles.Add(new ProjectFileViewModel { FileName = "ProjectA.ETL.csproj" });
        viewModel.ProjectFiles.Add(new ProjectFileViewModel { FileName = "ProjectB.ETL.csproj" });

        // Act
        viewModel.SearchText = "ProjectA";

        // Assert
        viewModel.FilteredProjectFiles.Should().HaveCount(1);
        viewModel.FilteredProjectFiles[0].FileName.Should().Be("ProjectA.ETL.csproj");
    }

    [Fact]
    public void SearchText_WhenCleared_ShouldShowAllProjects()
    {
        // Arrange
        var viewModel = CreateViewModel();
        viewModel.ProjectFiles.Add(new ProjectFileViewModel { FileName = "ProjectA.ETL.csproj" });
        viewModel.ProjectFiles.Add(new ProjectFileViewModel { FileName = "ProjectB.ETL.csproj" });
        viewModel.SearchText = "ProjectA";

        // Act
        viewModel.SearchText = string.Empty;

        // Assert
        viewModel.FilteredProjectFiles.Should().HaveCount(2);
    }

    // -------------------------------------------------------------------
    //  Selection
    // -------------------------------------------------------------------

    [Fact]
    public void SelectedCount_WhenProjectsSelected_ShouldUpdate()
    {
        // Arrange
        var viewModel = CreateViewModel();
        var project = new ProjectFileViewModel { FileName = "Test.ETL.csproj", IsSelected = false };
        viewModel.ProjectFiles.Add(project);

        // Act
        project.IsSelected = true;
        viewModel.UpdateSelectedCount();

        // Assert
        viewModel.SelectedCount.Should().Be(1);
    }

    [Fact]
    public void SelectAllCommand_ShouldSelectOnlyFilteredProjects()
    {
        // Arrange
        var viewModel = CreateViewModel();
        viewModel.ProjectFiles.Add(new ProjectFileViewModel { FileName = "ProjectA.ETL.csproj" });
        viewModel.ProjectFiles.Add(new ProjectFileViewModel { FileName = "ProjectB.ETL.csproj" });
        viewModel.SearchText = "ProjectA";

        // Act
        viewModel.SelectAllCommand.Execute(null);

        // Assert
        viewModel.SelectedCount.Should().Be(1);
        viewModel.ProjectFiles.First(p => p.FileName == "ProjectA.ETL.csproj").IsSelected.Should().BeTrue();
        viewModel.ProjectFiles.First(p => p.FileName == "ProjectB.ETL.csproj").IsSelected.Should().BeFalse();
    }

    [Fact]
    public void DeselectAllCommand_ShouldDeselectOnlyFilteredProjects()
    {
        // Arrange
        var viewModel = CreateViewModel();
        viewModel.ProjectFiles.Add(new ProjectFileViewModel { FileName = "ProjectA.ETL.csproj", IsSelected = true });
        viewModel.ProjectFiles.Add(new ProjectFileViewModel { FileName = "ProjectB.ETL.csproj", IsSelected = true });
        viewModel.SearchText = "ProjectA";

        // Act
        viewModel.DeselectAllCommand.Execute(null);

        // Assert
        viewModel.ProjectFiles.First(p => p.FileName == "ProjectA.ETL.csproj").IsSelected.Should().BeFalse();
        viewModel.ProjectFiles.First(p => p.FileName == "ProjectB.ETL.csproj").IsSelected.Should().BeTrue();
    }

    // -------------------------------------------------------------------
    //  Navigation menu — initialisation
    // -------------------------------------------------------------------

    [Fact]
    public void NavigationMenuItems_WhenInitialized_ShouldContainAllRegisteredTools()
    {
        var viewModel = CreateViewModel();

        viewModel.NavigationMenuItems.Should().HaveCount(1);
        viewModel.NavigationMenuItems[0].Id.Should().Be(ToolIds.VersionIncrease);
        viewModel.NavigationMenuItems[0].Name.Should().Be("Version Increase Tool");
    }

    [Fact]
    public void NavigationMenuItems_WhenMultipleToolsRegistered_ShouldListAll()
    {
        // Arrange
        _mockNavigationService.Setup(x => x.GetAllTools()).Returns(new List<ToolDefinition>
        {
            new ToolDefinition { Id = ToolIds.VersionIncrease, Name = "Version Increase Tool" },
            new ToolDefinition { Id = "build-deploy",          Name = "Build Deploy Tool" }
        });

        var viewModel = CreateViewModel();

        // Assert
        viewModel.NavigationMenuItems.Should().HaveCount(2);
        viewModel.NavigationMenuItems.Should().ContainSingle(m => m.Id == "build-deploy");
    }

    [Fact]
    public void NavigationMenuItems_WhenNoToolsRegistered_ShouldBeEmpty()
    {
        _mockNavigationService.Setup(x => x.GetAllTools())
            .Returns(new List<ToolDefinition>());

        var viewModel = CreateViewModel();

        viewModel.NavigationMenuItems.Should().BeEmpty();
    }

    // -------------------------------------------------------------------
    //  Navigation menu — active state
    // -------------------------------------------------------------------

    [Fact]
    public void NavigationMenuItems_CurrentTool_ShouldBeMarkedAsActive()
    {
        var viewModel = CreateViewModel();

        var item = viewModel.NavigationMenuItems.Single(m => m.Id == ToolIds.VersionIncrease);

        item.IsActive.Should().BeTrue();
    }

    [Fact]
    public void NavigationMenuItems_OtherTools_ShouldNotBeMarkedAsActive()
    {
        // Arrange
        _mockNavigationService.Setup(x => x.GetAllTools()).Returns(new List<ToolDefinition>
        {
            new ToolDefinition { Id = ToolIds.VersionIncrease, Name = "Version Increase Tool" },
            new ToolDefinition { Id = "build-deploy",          Name = "Build Deploy Tool" }
        });

        var viewModel = CreateViewModel();

        // Assert
        viewModel.NavigationMenuItems
            .Where(m => m.Id != ToolIds.VersionIncrease)
            .Should().AllSatisfy(m => m.IsActive.Should().BeFalse());
    }

    // -------------------------------------------------------------------
    //  Navigation menu — commands
    // -------------------------------------------------------------------

    [Fact]
    public void NavigationMenuItems_CurrentTool_NavigateCommand_ShouldBeDisabled()
    {
        var viewModel = CreateViewModel();

        var item = viewModel.NavigationMenuItems.Single(m => m.Id == ToolIds.VersionIncrease);

        item.NavigateCommand.Should().NotBeNull();
        item.NavigateCommand!.CanExecute(null).Should().BeFalse();
    }

    [Fact]
    public void NavigationMenuItems_OtherTool_NavigateCommand_ShouldBeEnabled()
    {
        // Arrange
        _mockNavigationService.Setup(x => x.GetAllTools()).Returns(new List<ToolDefinition>
        {
            new ToolDefinition { Id = ToolIds.VersionIncrease, Name = "Version Increase Tool" },
            new ToolDefinition { Id = "build-deploy",          Name = "Build Deploy Tool" }
        });

        var viewModel = CreateViewModel();

        var item = viewModel.NavigationMenuItems.Single(m => m.Id == "build-deploy");

        // Assert
        item.NavigateCommand!.CanExecute(null).Should().BeTrue();
    }

    [Fact]
    public void NavigationMenuItems_OtherTool_WhenCommandExecuted_ShouldCallNavigateTo()
    {
        // Arrange
        _mockNavigationService.Setup(x => x.GetAllTools()).Returns(new List<ToolDefinition>
        {
            new ToolDefinition { Id = ToolIds.VersionIncrease, Name = "Version Increase Tool" },
            new ToolDefinition { Id = "build-deploy",          Name = "Build Deploy Tool" }
        });

        var viewModel = CreateViewModel();
        var item = viewModel.NavigationMenuItems.Single(m => m.Id == "build-deploy");

        // Act
        item.NavigateCommand!.Execute(null);

        // Assert
        _mockNavigationService.Verify(x => x.NavigateTo("build-deploy"), Times.Once);
    }

    [Fact]
    public void NavigationMenuItems_CurrentTool_WhenCommandExecuted_ShouldNotCallNavigateTo()
    {
        var viewModel = CreateViewModel();
        var item = viewModel.NavigationMenuItems.Single(m => m.Id == ToolIds.VersionIncrease);

        // CanExecute = false so Execute is a no-op, but verify NavigateTo is never called
        item.NavigateCommand!.Execute(null);

        _mockNavigationService.Verify(x => x.NavigateTo(It.IsAny<string>()), Times.Never);
    }
}
