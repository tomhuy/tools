using CommunityToolkit.Mvvm.ComponentModel;
using Lifes.Presentation.WPF.Features.SprintBoard.Models;
using System.Collections.ObjectModel;

namespace Lifes.Presentation.WPF.Features.SprintBoard;

public partial class SprintBoardViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<BoardAssigneeModel> _assigneePills = new();

    [ObservableProperty]
    private ObservableCollection<BoardColumnModel> _boardColumns = new();

    [ObservableProperty]
    private ObservableCollection<FeatureRowViewModel> _featureRows = new();

    public SprintBoardViewModel()
    {
        InitializeMockData();
    }

    private void InitializeMockData()
    {
        // 1. Assignees (Pills at the top)
        var huy = new BoardAssigneeModel { Name = "Huy", Initials = "HY", ThemeColor = "#4D8AF0", BgColor = "#EBF4FF", DoneCount = 0, ActiveCount = 1 };
        var tuan = new BoardAssigneeModel { Name = "Tuấn", Initials = "TN", ThemeColor = "#27AE60", BgColor = "#E6F4EA", DoneCount = 0, ActiveCount = 2 };
        var bang = new BoardAssigneeModel { Name = "Bằng", Initials = "BG", ThemeColor = "#F39C12", BgColor = "#FEF5E7", DoneCount = 0, ActiveCount = 3 };
        var hoa = new BoardAssigneeModel { Name = "Hòa", Initials = "HÒ", ThemeColor = "#8E44AD", BgColor = "#F5EEF8", DoneCount = 1, ActiveCount = 0 };

        AssigneePills.Add(huy);
        AssigneePills.Add(tuan);
        AssigneePills.Add(bang);
        AssigneePills.Add(hoa);

        // 2. Board Columns (Table Headers)
        BoardColumns.Add(new BoardColumnModel { IsAssignee = false, Title = "LÀM TRƯỚC" });
        BoardColumns.Add(new BoardColumnModel { IsAssignee = true, Assignee = huy });
        BoardColumns.Add(new BoardColumnModel { IsAssignee = true, Assignee = tuan });
        BoardColumns.Add(new BoardColumnModel { IsAssignee = true, Assignee = bang });
        BoardColumns.Add(new BoardColumnModel { IsAssignee = true, Assignee = hoa });

        // Colors Reference
        string redBg = "#FDEDEC", redBorder = "#FADBD8", redTxt = "#E74C3C";
        string yelBg = "#FEF5E7", yelBorder = "#FDEBD0", yelTxt = "#F39C12";
        string grnBg = "#E6F4EA", grnBorder = "#D5F5E3", grnTxt = "#27AE60";
        string purBg = "#F5EEF8", purBorder = "#EBDEF0", purTxt = "#8E44AD";
        string bluBg = "#EBF4FF", bluBorder = "#D6EAF8", bluTxt = "#4D8AF0";

        // 3. Features and Matrix Task Rows
        FeatureRows.Add(CreateRow("Refactor PS Mode", 2, bluTxt, 
            (0, new BoardTaskModel { Id = "#01", Title = "lấy file cho design", StatusColor = redBg, BorderColor = redBorder, TextColor = redTxt }),
            (3, new BoardTaskModel { Id = "#02", Title = "investigate", StatusColor = yelBg, BorderColor = yelBorder, TextColor = yelTxt })
        ));

        FeatureRows.Add(CreateRow("Add validation dataloader", 1, grnTxt, 
            (0, new BoardTaskModel { Id = "#1.1", Title = "Check the feedback and split item", StatusColor = redBg, BorderColor = redBorder, TextColor = redTxt })
        ));

        FeatureRows.Add(CreateRow("Custom text", 3, yelTxt, 
            (4, new BoardTaskModel { Id = "#2.1", Title = "implement", StatusColor = purBg, BorderColor = purBorder, TextColor = purTxt, IsDone = true }),
            (2, new BoardTaskModel { Id = "#2.1", Title = "Code review", StatusColor = grnBg, BorderColor = grnBorder, TextColor = grnTxt }),
            (3, new BoardTaskModel { Id = "#2.2", Title = "Code review", StatusColor = yelBg, BorderColor = yelBorder, TextColor = yelTxt })
        ));

        FeatureRows.Add(CreateRow("IXL Time spent", 3, purTxt, 
            (1, new BoardTaskModel { Id = "#3.2", Title = "discuss with huy", StatusColor = bluBg, BorderColor = bluBorder, TextColor = bluTxt }),
            (2, new BoardTaskModel { Id = "#3.1", Title = "investigate", StatusColor = grnBg, BorderColor = grnBorder, TextColor = grnTxt }),
            (3, new BoardTaskModel { Id = "#3.3", Title = "implement", StatusColor = yelBg, BorderColor = yelBorder, TextColor = yelTxt })
        ));
    }

    private FeatureRowViewModel CreateRow(string title, int taskCount, string indicatorColor, params (int columnIndex, BoardTaskModel task)[] taskLines)
    {
        var row = new FeatureRowViewModel
        {
             Feature = new BoardFeatureModel { Title = title, TaskCount = taskCount, IndicatorColor = indicatorColor }
        };

        foreach (var lineData in taskLines)
        {
             var taskRow = new TaskRowModel();
             for (int i = 0; i < BoardColumns.Count; i++)
             {
                  var cell = new TaskCellModel 
                  { 
                      Task = (i == lineData.columnIndex) ? lineData.task : null,
                      Column = BoardColumns[i],
                      ParentRow = taskRow
                  };
                  taskRow.Cells.Add(cell);
             }
             row.TaskRows.Add(taskRow);
        }

        return row;
    }

    public void MoveTask(TaskCellModel sourceCell, TaskCellModel targetCell)
    {
        if (sourceCell == targetCell || targetCell.Task != null || sourceCell.Task == null) return;

        var task = sourceCell.Task;
        sourceCell.Task = null;
        targetCell.Task = task;
    }
}
