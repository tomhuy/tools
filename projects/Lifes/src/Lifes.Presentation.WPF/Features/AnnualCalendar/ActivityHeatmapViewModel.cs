using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lifes.Core.Interfaces;
using Lifes.Core.Models;
using Lifes.Presentation.WPF.Constants;
using Lifes.Presentation.WPF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Lifes.Presentation.WPF.Features.AnnualCalendar;

public partial class ActivityHeatmapViewModel : ObservableObject
{
    private readonly ICalendarService _calendarService;
    private readonly INavigationService _navigationService;

    public ObservableCollection<EventGroupViewModel> EventGroups { get; } = new();
    public ObservableCollection<LegendItemViewModel> Legends { get; } = new();

    [ObservableProperty]
    private ObservableCollection<ToolMenuItem> _navigationMenuItems = new();

    public ActivityHeatmapViewModel(ICalendarService calendarService, INavigationService navigationService)
    {
        _calendarService = calendarService;
        _navigationService = navigationService;

        InitializeLegends();
        InitializeNavigationMenu();
        _ = LoadDataAsync();
    }

    private void InitializeNavigationMenu()
    {
        NavigationMenuItems = new ObservableCollection<ToolMenuItem>(
            _navigationService.GetAllTools().Select(t => new ToolMenuItem
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                IsActive = t.Id == ToolIds.ActivityHeatmap,
                NavigateCommand = new RelayCommand(
                    () => _navigationService.NavigateTo(t.Id),
                    () => t.Id != ToolIds.ActivityHeatmap)
            })
        );
    }

    private void InitializeLegends()
    {
        Legends.Add(new LegendItemViewModel { Category = "Work", BgColor = "#4CAF50", FgColor = "#FFFFFF" });
        Legends.Add(new LegendItemViewModel { Category = "Personal", BgColor = "#2196F3", FgColor = "#FFFFFF" });
        Legends.Add(new LegendItemViewModel { Category = "Health", BgColor = "#F44336", FgColor = "#FFFFFF" });
        Legends.Add(new LegendItemViewModel { Category = "Learning", BgColor = "#9C27B0", FgColor = "#FFFFFF" });
        Legends.Add(new LegendItemViewModel { Category = "Travel", BgColor = "#FF9800", FgColor = "#FFFFFF" });
        Legends.Add(new LegendItemViewModel { Category = "Event", BgColor = "#009688", FgColor = "#FFFFFF" });
    }

    private async Task LoadDataAsync()
    {
        var allEvents = new List<CalendarEventModel>();
        for (int y = 2025; y <= 2026; y++)
        {
            var yearEvents = await _calendarService.GetAnnualEventsAsync(y);
            allEvents.AddRange(yearEvents);
        }

        if (!allEvents.Any()) return;

        // Group by Event Title
        var groups = allEvents.GroupBy(e => e.Title).OrderBy(g => g.Key);
        var newEventGroups = new List<EventGroupViewModel>();

        foreach (var group in groups)
        {
            var eventGroupVM = new EventGroupViewModel
            {
                EventTitle = $"hành động: {group.Key.ToLower()}"
            };

            // Numeric Header (1-31)
            for (int i = 1; i <= 31; i++) eventGroupVM.DayNumbers.Add(i);

            // Find all active months for this event title
            var minDate = group.Min(e => e.StartDate);
            var maxDate = group.Max(e => e.EndDate);
            var startMonth = new DateTime(minDate.Year, minDate.Month, 1);
            var endMonth = new DateTime(maxDate.Year, maxDate.Month, 1);

            for (var mStart = startMonth; mStart <= endMonth; mStart = mStart.AddMonths(1))
            {
                var rowVM = new MonthRowViewModel
                {
                    MonthLabel = $"{mStart.Month}/{mStart.Year}"
                };

                int daysInMonth = DateTime.DaysInMonth(mStart.Year, mStart.Month);

                for (int d = 1; d <= 31; d++)
                {
                    var cell = new TrackerCellViewModel();
                    if (d > daysInMonth)
                    {
                        cell.IsHidden = true;
                    }
                    else
                    {
                        var date = new DateTime(mStart.Year, mStart.Month, d);
                        var activeEvent = group.FirstOrDefault(e => IsActiveOnDay(e, date));
                        if (activeEvent != null)
                        {
                            cell.IsActive = true;
                            cell.BgColor = GetSolidBgColor(activeEvent.Category);
                            cell.Text = GetPhaseTitleAt(activeEvent, date);
                            cell.Tooltip = activeEvent.Title;
                        }
                    }
                    rowVM.Cells.Add(cell);
                }
                eventGroupVM.Months.Add(rowVM);
            }

            newEventGroups.Add(eventGroupVM);
        }

        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            EventGroups.Clear();
            foreach (var eg in newEventGroups) EventGroups.Add(eg);
        });
    }

    private bool IsActiveOnDay(CalendarEventModel evt, DateTime date)
    {
        if (evt.Phases != null && evt.Phases.Any())
        {
            return evt.Phases.Any(p => p.StartDate.Date <= date && p.EndDate.Date >= date);
        }
        return evt.StartDate.Date <= date && evt.EndDate.Date >= date;
    }

    private string GetPhaseTitleAt(CalendarEventModel evt, DateTime date)
    {
        if (evt.Phases != null && evt.Phases.Any())
        {
            var phase = evt.Phases.FirstOrDefault(p => p.StartDate.Date <= date && p.EndDate.Date >= date);
            return phase?.Title ?? "";
        }
        return evt.Title;
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
            // "Psychology" => "#F44336",
            "Psychology" => "#4CAF50",
            _ => "#9E9E9E"
        };
    }
}

public class EventGroupViewModel
{
    public string EventTitle { get; set; } = string.Empty;
    public ObservableCollection<int> DayNumbers { get; } = new();
    public ObservableCollection<MonthRowViewModel> Months { get; } = new();
}

public class MonthRowViewModel
{
    public string MonthLabel { get; set; } = string.Empty;
    public ObservableCollection<TrackerCellViewModel> Cells { get; } = new();
}

public partial class TrackerCellViewModel : ObservableObject
{
    [ObservableProperty] private bool _isActive;
    [ObservableProperty] private bool _isHidden;
    [ObservableProperty] private string _text = string.Empty;
    [ObservableProperty] private string _bgColor = "Transparent";
    [ObservableProperty] private string _tooltip = string.Empty;
}
