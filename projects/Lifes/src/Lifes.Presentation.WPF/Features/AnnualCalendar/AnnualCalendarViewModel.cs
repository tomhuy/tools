using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lifes.Core.Interfaces;
using Lifes.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Lifes.Presentation.WPF.Constants;
using Lifes.Presentation.WPF.Models;

namespace Lifes.Presentation.WPF.Features.AnnualCalendar;

public partial class AnnualCalendarViewModel : ObservableObject
{
    private readonly ICalendarService _calendarService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private int _currentYear;

    public ObservableCollection<string> TopDaysOfWeek { get; } = new();
    public ObservableCollection<AnnualMonthViewModel> Months { get; } = new();
    public ObservableCollection<LegendItemViewModel> Legends { get; } = new();
    
    [ObservableProperty]
    private ObservableCollection<ToolMenuItem> _navigationMenuItems = new();

    public AnnualCalendarViewModel(ICalendarService calendarService, INavigationService navigationService)
    {
        _calendarService = calendarService;
        _navigationService = navigationService;
        CurrentYear = 2026; // Fixed for now based on mockup

        InitializeTopHeader();
        InitializeLegends();
        InitializeNavigationMenu();
    }

    private void InitializeNavigationMenu()
    {
        NavigationMenuItems = new ObservableCollection<ToolMenuItem>(
            _navigationService.GetAllTools().Select(t => new ToolMenuItem
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                IsActive = t.Id == ToolIds.AnnualCalendar,
                NavigateCommand = new RelayCommand(
                    () => _navigationService.NavigateTo(t.Id),
                    () => t.Id != ToolIds.AnnualCalendar)
            })
        );
    }

    private void InitializeTopHeader()
    {
        // 37 columns: Su Mo Tu We Th Fr Sa repeated 5 times + Su Mo
        var days = new[] { "Su", "Mo", "Tu", "We", "Th", "Fr", "Sa" };
        for (int i = 0; i < 37; i++)
        {
            TopDaysOfWeek.Add(days[i % 7]);
        }
    }

    private void InitializeLegends()
    {
        Legends.Add(new LegendItemViewModel { Category = "Work", BgColor = "#E8F5E9", FgColor = "#2E7D32" });
        Legends.Add(new LegendItemViewModel { Category = "Personal", BgColor = "#E3F2FD", FgColor = "#1565C0" });
        Legends.Add(new LegendItemViewModel { Category = "Health", BgColor = "#FFEBEE", FgColor = "#C62828" });
        Legends.Add(new LegendItemViewModel { Category = "Learning", BgColor = "#F3E5F5", FgColor = "#6A1B9A" });
        Legends.Add(new LegendItemViewModel { Category = "Travel", BgColor = "#FFF3E0", FgColor = "#EF6C00" });
        Legends.Add(new LegendItemViewModel { Category = "Event", BgColor = "#E0F2F1", FgColor = "#00695C" });
        Legends.Add(new LegendItemViewModel { Category = "Review", BgColor = "#FFFDE7", FgColor = "#F57F17" });
        Legends.Add(new LegendItemViewModel { Category = "Planning", BgColor = "#E8EAF6", FgColor = "#283593" });
        Legends.Add(new LegendItemViewModel { Category = "Conference", BgColor = "#ECEFF1", FgColor = "#455A64" });
        Legends.Add(new LegendItemViewModel { Category = "Competition", BgColor = "#E0F7FA", FgColor = "#00838F" });
        Legends.Add(new LegendItemViewModel { Category = "Release", BgColor = "#FCE4EC", FgColor = "#AD1457" });
        Legends.Add(new LegendItemViewModel { Category = "Học tâm lý học", BgColor = "#FFEBEE", FgColor = "#C62828" });
    }

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        var mementos = (await _calendarService.GetAnnualEventsAsync(CurrentYear)).ToList();

        Months.Clear();

        var today = DateTime.Today;

        for (int month = 1; month <= 12; month++)
        {
            var monthStart = new DateTime(CurrentYear, month, 1);
            int daysInMonth = DateTime.DaysInMonth(CurrentYear, month);
            var monthEnd = new DateTime(CurrentYear, month, daysInMonth);
            
            // Calculate starting column based on DayOfWeek (Sunday = 0)
            int startColumn = (int)monthStart.DayOfWeek;

            var monthViewModel = new AnnualMonthViewModel
            {
                MonthName = monthStart.ToString("MMM"), // Jan, Feb
            };

            // Setup day columns (01, 02.. 31)
            for (int i = 0; i < 37; i++)
            {
                var dayCell = new AnnualDayViewModel { ColumnIndex = i };
                
                int dayNumber = i - startColumn + 1;
                if (dayNumber >= 1 && dayNumber <= daysInMonth)
                {
                    dayCell.DayText = dayNumber.ToString("D2");
                    var date = new DateTime(CurrentYear, month, dayNumber);
                    dayCell.IsToday = date == today;
                }
                
                monthViewModel.Days.Add(dayCell);
            }

            // Find overlapping tasks for THIS month
            var monthTasks = new List<AnnualTaskViewModel>();
            
            // Separate Topics and Supplemental Concepts
            var topics = mementos.Where(m => m.ParentId == null).ToList();
            var children = mementos.Where(m => m.ParentId != null).ToList();

            foreach (var topic in topics)
            {
                var topicChildren = children.Where(c => c.ParentId == topic.Id).ToList();

                if (topicChildren.Any())
                {
                    foreach (var child in topicChildren)
                    {
                        if (child.StartDate <= monthEnd && child.EndDate >= monthStart)
                        {
                            DateTime effectiveStart = child.StartDate < monthStart ? monthStart : child.StartDate;
                            DateTime effectiveEnd = child.EndDate > monthEnd ? monthEnd : child.EndDate;

                            monthTasks.Add(new AnnualTaskViewModel
                            {
                                Title = child.Title,
                                Category = child.Color,
                                StartColumn = startColumn + (effectiveStart.Day - 1),
                                Duration = (int)(effectiveEnd.Date - effectiveStart.Date).TotalDays + 1,
                                BgColor = GetSolidBgColor(child.Color),
                                FgColor = GetSolidFgColor(child.Color)
                            });
                        }
                    }
                }
                else
                {
                    if (topic.StartDate <= monthEnd && topic.EndDate >= monthStart)
                    {
                        DateTime effectiveStart = topic.StartDate < monthStart ? monthStart : topic.StartDate;
                        DateTime effectiveEnd = topic.EndDate > monthEnd ? monthEnd : topic.EndDate;

                        monthTasks.Add(new AnnualTaskViewModel
                        {
                            Title = topic.Title,
                            Category = topic.Color,
                            StartColumn = startColumn + (effectiveStart.Day - 1),
                            Duration = (int)(effectiveEnd.Date - effectiveStart.Date).TotalDays + 1,
                            BgColor = GetSolidBgColor(topic.Color),
                            FgColor = GetSolidFgColor(topic.Color)
                        });
                    }
                }
            }

            // Pack tasks into rows using a greedy approach
            monthTasks = monthTasks.OrderBy(t => t.StartColumn).ToList();
            
            var rows = new List<AnnualTaskRowViewModel>();

            foreach (var task in monthTasks)
            {
                bool placed = false;
                foreach (var row in rows)
                {
                    // Check if task can fit in this row without overlapping
                    bool overlaps = row.Tasks.Any(existing => 
                        (task.StartColumn >= existing.StartColumn && task.StartColumn < existing.StartColumn + existing.Duration) ||
                        (existing.StartColumn >= task.StartColumn && existing.StartColumn < task.StartColumn + task.Duration));
                        
                    if (!overlaps)
                    {
                        row.Tasks.Add(task);
                        placed = true;
                        break;
                    }
                }

                if (!placed)
                {
                    // Create new row
                    var newRow = new AnnualTaskRowViewModel();
                    newRow.Tasks.Add(task);
                    rows.Add(newRow);
                }
            }

            foreach (var row in rows)
            {
                monthViewModel.TaskRows.Add(row);
            }

            Months.Add(monthViewModel);
        }
    }

    private string GetSolidBgColor(string category)
    {
        return category switch
        {
            "Work" => "#4CAF50",
            "Personal" => "#2196F3",
            "Health" => "#F44336",
            "Learning" => "#9C27B0",
            "Travel" => "#FF9800",
            "Event" => "#009688",
            "Review" => "#FFC107", 
            "Planning" => "#3F51B5",
            "Conference" => "#607D8B",
            "Competition" => "#00BCD4",
            "Release" => "#E91E63",
            "Psychology" => "#F44336",
            _ => "#9E9E9E"
        };
    }

    private string GetSolidFgColor(string category)
    {
        return category == "Review" ? "#000000" : "#FFFFFF";
    }
}

public class AnnualMonthViewModel
{
    public string MonthName { get; set; } = string.Empty;
    public ObservableCollection<AnnualDayViewModel> Days { get; } = new();
    public ObservableCollection<AnnualTaskRowViewModel> TaskRows { get; } = new();
}

public partial class AnnualDayViewModel : ObservableObject
{
    public int ColumnIndex { get; set; }
    [ObservableProperty] private string _dayText = string.Empty;
    [ObservableProperty] private bool _isToday;
}

public class AnnualTaskRowViewModel
{
    public ObservableCollection<AnnualTaskViewModel> Tasks { get; } = new();
}

public class AnnualTaskViewModel
{
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int StartColumn { get; set; }
    public int Duration { get; set; }
    public string BgColor { get; set; } = string.Empty;
    public string FgColor { get; set; } = string.Empty;
}

public class LegendItemViewModel
{
    public string Category { get; set; } = string.Empty;
    public string BgColor { get; set; } = string.Empty;
    public string FgColor { get; set; } = string.Empty;
}
