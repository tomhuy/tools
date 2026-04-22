namespace Lifes.Presentation.WPF.Features.SprintBoard.Models;
using CommunityToolkit.Mvvm.ComponentModel;

public class BoardAssigneeModel
{
    public string Name { get; set; } = string.Empty;
    public string Initials { get; set; } = string.Empty;
    public string ThemeColor { get; set; } = "#E0E0E0";
    public string BgColor { get; set; } = "#FFFFFF";
    public int DoneCount { get; set; }
    public int ActiveCount { get; set; }
    public int TotalCount => DoneCount + ActiveCount;
    public string SummaryText => $"{DoneCount} done - {ActiveCount} active";
}

public class BoardColumnModel
{
    public bool IsAssignee { get; set; }
    public bool IsNotAssignee => !IsAssignee;
    public string Title { get; set; } = string.Empty;
    public BoardAssigneeModel? Assignee { get; set; }
}

public class BoardTaskModel
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string StatusColor { get; set; } = "#FFFFFF";
    public string BorderColor { get; set; } = "#E0E0E0";
    public string TextColor { get; set; } = "#424242";
    public bool IsDone { get; set; }
}

public partial class TaskCellModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasTask))]
    private BoardTaskModel? _task;

    public bool HasTask => Task != null;

    public BoardColumnModel Column { get; set; } = null!;
    public TaskRowModel ParentRow { get; set; } = null!;
}

public class TaskRowModel
{
    public System.Collections.ObjectModel.ObservableCollection<TaskCellModel> Cells { get; set; } = new();
}

public class BoardFeatureModel
{
    public string Title { get; set; } = string.Empty;
    public int TaskCount { get; set; }
    public string IndicatorColor { get; set; } = "#3ECF8E";
    public string SummaryText => $"{TaskCount} task{(TaskCount > 1 ? "s" : "")}";
}

public class FeatureRowViewModel
{
    public BoardFeatureModel Feature { get; set; } = new();
    public System.Collections.ObjectModel.ObservableCollection<TaskRowModel> TaskRows { get; set; } = new();
}
