using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Lifes.Presentation.WPF.Features.SprintBoard.Models;

namespace Lifes.Presentation.WPF.Features.SprintBoard;

/// <summary>
/// Interaction logic for SprintBoardView.xaml
/// </summary>
public partial class SprintBoardView : System.Windows.Controls.UserControl
{
    private System.Windows.Point _startPoint;

    public SprintBoardView()
    {
        InitializeComponent();
    }

    private void TaskCard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _startPoint = e.GetPosition(null);
    }

    private void TaskCard_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed && sender is FrameworkElement element && element.DataContext is TaskCellModel sourceCell)
        {
            System.Windows.Point mousePos = e.GetPosition(null);
            System.Windows.Vector diff = _startPoint - mousePos;

            if (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
            {
                // Start drag drop
                System.Windows.DataObject dragData = new System.Windows.DataObject("TaskCellFormat", sourceCell);
                System.Windows.DragDrop.DoDragDrop(element, dragData, System.Windows.DragDropEffects.Move);
            }
        }
    }

    private void TaskCell_DragOver(object sender, System.Windows.DragEventArgs e)
    {
        if (!e.Data.GetDataPresent("TaskCellFormat"))
        {
            e.Effects = System.Windows.DragDropEffects.None;
            e.Handled = true;
            return;
        }

        if (sender is FrameworkElement element && element.DataContext is TaskCellModel targetCell)
        {
            if (targetCell.HasTask)
            {
                e.Effects = System.Windows.DragDropEffects.None; // Cannot drop on already occupied cell
            }
            else
            {
                e.Effects = System.Windows.DragDropEffects.Move;
            }
        }
        e.Handled = true;
    }

    private void TaskCell_Drop(object sender, System.Windows.DragEventArgs e)
    {
        if (e.Data.GetDataPresent("TaskCellFormat"))
        {
            var sourceCell = e.Data.GetData("TaskCellFormat") as TaskCellModel;
            if (sender is FrameworkElement element && element.DataContext is TaskCellModel targetCell)
            {
                if (sourceCell != null && sourceCell != targetCell && !targetCell.HasTask)
                {
                    if (this.DataContext is SprintBoardViewModel viewModel)
                    {
                        viewModel.MoveTask(sourceCell, targetCell);
                    }
                }
            }
        }
    }
}
