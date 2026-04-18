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

public partial class MonthlyCalendarViewModel : ObservableObject
{
    private readonly ICalendarService _calendarService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private int _currentYear;

    public ObservableCollection<SelectableMonthViewModel> AvailableMonths { get; } = new();
    public ObservableCollection<MonthDisplayViewModel> DisplayMonths { get; } = new();

    [ObservableProperty]
    private ObservableCollection<ToolMenuItem> _navigationMenuItems = new();

    public MonthlyCalendarViewModel(ICalendarService calendarService, INavigationService navigationService)
    {
        _calendarService = calendarService;
        _navigationService = navigationService;
        
        CurrentYear = 2026; // Match mockup year for consistency
        
        InitializeAvailableMonths();
        InitializeNavigationMenu();
    }

    private void InitializeAvailableMonths()
    {
        var today = DateTime.Today;
        // Calculate the start of the quarter (1-3, 4-6, 7-9, 10-12)
        int startMonth = ((today.Month - 1) / 3) * 3 + 1;

        for (int i = 1; i <= 12; i++)
        {
            var monthName = DateTimeFormatInfo.CurrentInfo.GetMonthName(i);
            var month = new SelectableMonthViewModel
            {
                Value = i,
                Name = monthName,
                IsSelected = i >= startMonth && i < startMonth + 3
            };
            month.SelectedChanged += (s, e) => _ = LoadDataAsync();
            AvailableMonths.Add(month);
        }

        // Trigger initial data load
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
                IsActive = t.Id == ToolIds.MonthlyCalendar,
                NavigateCommand = new RelayCommand(
                    () => _navigationService.NavigateTo(t.Id),
                    () => t.Id != ToolIds.MonthlyCalendar)
            })
        );
    }

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        var selectedMonths = AvailableMonths.Where(m => m.IsSelected).ToList();
        
        var newDisplayMonths = new List<MonthDisplayViewModel>();

        foreach (var monthInfo in selectedMonths)
        {
            var events = await _calendarService.GetMonthlyEventsAsync(CurrentYear, monthInfo.Value);
            var monthDisplay = BuildMonthDisplay(monthInfo.Value, events);
            newDisplayMonths.Add(monthDisplay);
        }

        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            DisplayMonths.Clear();
            foreach (var md in newDisplayMonths)
            {
                DisplayMonths.Add(md);
            }
        });
    }

    private MonthDisplayViewModel BuildMonthDisplay(int month, IEnumerable<CalendarEventModel> events)
    {
        var display = new MonthDisplayViewModel
        {
            MonthName = $"{DateTimeFormatInfo.CurrentInfo.GetMonthName(month)} {CurrentYear}"
        };

        // Build Headers
        int daysInMonth = DateTime.DaysInMonth(CurrentYear, month);
        var today = DateTime.Today;

        for (int i = 1; i <= daysInMonth; i++)
        {
            var date = new DateTime(CurrentYear, month, i);
            display.DayHeaders.Add(new DayHeaderViewModel
            {
                DayNumber = i,
                DayName = GetShortDayName(date.DayOfWeek),
                IsToday = date == today
            });
        }

        // Build Rows
        var monthStart = new DateTime(CurrentYear, month, 1);
        var monthEnd = new DateTime(CurrentYear, month, daysInMonth);

        foreach (var evt in events)
        {
            var row = new MonthlyEventRowViewModel
            {
                Title = evt.Title,
                Category = evt.Category,
                BgColor = GetSolidBgColor(evt.Category),
                FgColor = GetSolidFgColor(evt.Category)
            };

            // If event has phases, process each phase. Otherwise, process the main event as a single phase.
            if (evt.Phases != null && evt.Phases.Any())
            {
                foreach (var phase in evt.Phases)
                {
                    if (phase.StartDate <= monthEnd && phase.EndDate >= monthStart)
                    {
                        var bar = CreateBar(phase.Title, phase.StartDate, phase.EndDate, phase.Category ?? evt.Category, monthStart, monthEnd);
                        row.Bars.Add(bar);
                    }
                }
            }
            else
            {
                var bar = CreateBar(evt.Title, evt.StartDate, evt.EndDate, evt.Category, monthStart, monthEnd);
                row.Bars.Add(bar);
            }

            if (row.Bars.Any())
            {
                display.EventRows.Add(row);
            }
        }

        return display;
    }

    private MonthlyGanttBarViewModel CreateBar(string title, DateTime start, DateTime end, string category, DateTime monthStart, DateTime monthEnd)
    {
        DateTime effectiveStart = start < monthStart ? monthStart : start;
        DateTime effectiveEnd = end > monthEnd ? monthEnd : end;

        return new MonthlyGanttBarViewModel
        {
            Title = title,
            Category = category,
            StartColumn = effectiveStart.Day - 1,
            Duration = (int)(effectiveEnd.Date - effectiveStart.Date).TotalDays + 1,
            BgColor = GetSolidBgColor(category),
            FgColor = GetSolidFgColor(category)
        };
    }

    private string GetShortDayName(DayOfWeek day)
    {
        return day switch
        {
            DayOfWeek.Monday => "T2",
            DayOfWeek.Tuesday => "T3",
            DayOfWeek.Wednesday => "T4",
            DayOfWeek.Thursday => "T5",
            DayOfWeek.Friday => "T6",
            DayOfWeek.Saturday => "T7",
            DayOfWeek.Sunday => "CN",
            _ => ""
        };
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

public partial class SelectableMonthViewModel : ObservableObject
{
    public int Value { get; set; }
    public string Name { get; set; } = string.Empty;

    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (SetProperty(ref _isSelected, value))
            {
                SelectedChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public event EventHandler? SelectedChanged;
}

public partial class DayHeaderViewModel : ObservableObject
{
    [ObservableProperty] private int _dayNumber;
    [ObservableProperty] private string _dayName = string.Empty;
    [ObservableProperty] private bool _isToday;
}

public partial class MonthlyEventRowViewModel : ObservableObject
{
    [ObservableProperty] private string _title = string.Empty;
    [ObservableProperty] private string _category = string.Empty;
    [ObservableProperty] private string _bgColor = string.Empty;
    [ObservableProperty] private string _fgColor = string.Empty;
    public ObservableCollection<MonthlyGanttBarViewModel> Bars { get; } = new();
}

public partial class MonthlyGanttBarViewModel : ObservableObject
{
    [ObservableProperty] private string _title = string.Empty;
    [ObservableProperty] private string _category = string.Empty;
    [ObservableProperty] private int _startColumn;
    [ObservableProperty] private int _duration;
    [ObservableProperty] private string _bgColor = string.Empty;
    [ObservableProperty] private string _fgColor = string.Empty;
}

public class MonthDisplayViewModel
{
    public string MonthName { get; set; } = string.Empty;
    public ObservableCollection<DayHeaderViewModel> DayHeaders { get; } = new();
    public ObservableCollection<MonthlyEventRowViewModel> EventRows { get; } = new();
}
