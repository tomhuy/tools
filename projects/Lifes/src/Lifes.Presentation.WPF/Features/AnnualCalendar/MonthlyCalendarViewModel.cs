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

public enum CalendarDisplayMode
{
    Gantt,
    Dot,
    PureDot
}

public partial class MonthlyCalendarViewModel : ObservableObject
{
    private readonly ICalendarService _calendarService;
    private readonly INavigationService _navigationService;
    private List<MementoModel> _allMementos = new();

    [ObservableProperty]
    private CalendarDisplayMode _displayMode = CalendarDisplayMode.Gantt;

    public CalendarDisplayMode[] AvailableDisplayModes { get; } = (CalendarDisplayMode[])Enum.GetValues(typeof(CalendarDisplayMode));

    [ObservableProperty]
    private int _currentYear;

    public ObservableCollection<SelectableMonthViewModel> AvailableMonths { get; } = new();
    public ObservableCollection<MonthDisplayViewModel> DisplayMonths { get; } = new();

    [ObservableProperty]
    private ObservableCollection<ToolMenuItem> _navigationMenuItems = new();

    public ObservableCollection<SelectableTagViewModel> AvailableTags { get; } = new();

    [ObservableProperty]
    private bool _includeChildrenOfTaggedParents = true;

    [ObservableProperty] private bool _isEditPopupOpen;
    [ObservableProperty] private bool _isColorPopupOpen;
    [ObservableProperty] private string _editingTitle = string.Empty;
    [ObservableProperty] private MementoModel? _currentEditingMemento;
    [ObservableProperty] private MonthlyEventRowViewModel? _editingRow;
    [ObservableProperty] private int _editingDay;
    [ObservableProperty] private int _editingMonth;
    [ObservableProperty] private bool _isShowEditingTag;
    public ObservableCollection<SelectableTagViewModel> EditingMementoTags { get; } = new();

    [ObservableProperty] private bool _isTagManagerOpen;
    public TagManagementViewModel TagManager { get; }

    [ObservableProperty] private bool _isAddTopicOpen;
    public AddTopicViewModel AddTopicManager { get; }

    public string[] ColorPalette { get; } = UIConstants.StandardColorPalette;

    public MonthlyCalendarViewModel(ICalendarService calendarService, INavigationService navigationService)
    {
        _calendarService = calendarService;
        _navigationService = navigationService;
        
        TagManager = new TagManagementViewModel(calendarService);
        TagManager.TagsUpdated += async () => await RefreshAvailableTagsAsync();

        AddTopicManager = new AddTopicViewModel(calendarService);
        AddTopicManager.TopicAdded += async () => await LoadDataAsync();
        AddTopicManager.RequestClose += () => IsAddTopicOpen = false;

        CurrentYear = 2026; // Match mockup year for consistency
        
        InitializeAvailableMonths();
        _ = InitializeAvailableTagsAsync();
        InitializeNavigationMenu();
    }

    private async Task InitializeAvailableTagsAsync()
    {
        var tags = await _calendarService.GetTagsAsync();
        foreach (var tag in tags)
        {
            var selectableTag = new SelectableTagViewModel
            {
                Id = tag.Id,
                Name = tag.Name,
                Color = tag.Color,
                IsSelected = false
            };
            selectableTag.SelectedChanged += (s, e) => _ = LoadDataAsync();
            AvailableTags.Add(selectableTag);
        }
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
        _allMementos.Clear();
        var selectedMonths = AvailableMonths.Where(m => m.IsSelected).ToList();
        var selectedTagIds = AvailableTags.Where(t => t.IsSelected).Select(t => t.Id).ToList();
        
        var newDisplayMonths = new List<MonthDisplayViewModel>();

        foreach (var monthInfo in selectedMonths)
        {
            var mementos = await _calendarService.GetMonthlyEventsAsync(
                CurrentYear, 
                monthInfo.Value, 
                selectedTagIds.Any() ? selectedTagIds : null, 
                IncludeChildrenOfTaggedParents);
                
            // Collect all mementos for lookup
            _allMementos.AddRange(mementos);
                 
            var monthDisplay = BuildMonthDisplay(monthInfo.Value, mementos);
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

    private MonthDisplayViewModel BuildMonthDisplay(int month, IEnumerable<MementoModel> mementos)
    {
        var display = new MonthDisplayViewModel
        {
            MonthValue = month,
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

        // Separate Topics and Supplemental Concepts
        var flatList = mementos.ToList();
        var topics = flatList.Where(m => m.ParentId == null).OrderBy(m => m.Order).ThenBy(m => m.StartDate).ToList();
        var supplemental = flatList.Where(m => m.ParentId != null).ToList();

        foreach (var topic in topics)
        {
            var row = new MonthlyEventRowViewModel
            {
                MementoId = topic.Id,
                Month = month,
                Title = topic.Title,
                Category = topic.Color,
                BgColor = GetSolidBgColor(topic.Color),
                FgColor = GetSolidFgColor(topic.Color)
            };

            // Find supplemental notes for this topic
            var children = supplemental.Where(s => s.ParentId == topic.Id).OrderBy(s => s.Order).ThenBy(s => s.StartDate).ToList();

            if (children.Any())
            {
                foreach (var child in children)
                {
                    if (child.StartDate <= monthEnd && child.EndDate >= monthStart)
                    {
                        var bar = CreateBar(child.Id, child.Title, child.StartDate, child.EndDate, child.Color, monthStart, monthEnd);
                        row.Bars.Add(bar);
                    }
                }
            }
            else
            {
                // If no children, treat topic itself as a bar if it overlaps this month
                if (topic.StartDate <= monthEnd && topic.EndDate >= monthStart)
                {
                    var bar = CreateBar(topic.Id, topic.Title, topic.StartDate, topic.EndDate, topic.Color, monthStart, monthEnd);
                    row.Bars.Add(bar);
                }
            }

            if (row.Bars.Any())
            {
                display.EventRows.Add(row);
            }
        }

        return display;
    }

    private MonthlyGanttBarViewModel CreateBar(int id, string title, DateTime start, DateTime end, string color, DateTime monthStart, DateTime monthEnd)
    {
        DateTime effectiveStart = start < monthStart ? monthStart : start;
        DateTime effectiveEnd = end > monthEnd ? monthEnd : end;

        return new MonthlyGanttBarViewModel
        {
            MementoId = id,
            Title = title,
            Category = color,
            StartColumn = effectiveStart.Day - 1,
            Duration = (int)(effectiveEnd.Date - effectiveStart.Date).TotalDays + 1,
            BgColor = GetSolidBgColor(color),
            FgColor = GetSolidFgColor(color)
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
        if (string.IsNullOrEmpty(category)) return "#9E9E9E";
        
        // If it's a hex color from our palette, return it directly
        if (category.StartsWith("#")) return category;

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
            "Psychology" => "#4CAF50",
            _ => "#9E9E9E"
        };
    }

    private string GetSolidFgColor(string color)
    {
        if (string.IsNullOrEmpty(color)) return "#FFFFFF";

        if (color.StartsWith("#"))
        {
            return GetContrastColor(color);
        }

        return color == "Review" ? "#000000" : "#FFFFFF";
    }

    private string GetContrastColor(string hexColor)
    {
        // Use centralized light colors list
        return UIConstants.LightColors.Contains(hexColor.ToUpper()) ? "#2D3748" : "#FFFFFF";
    }

    [RelayCommand]
    private void OpenEditPopup(EditCellArgs args)
    {
        EditingRow = args.Row;
        EditingDay = args.Day;
        EditingMonth = args.Month;

        if (args.ExistingMementoId.HasValue)
        {
            CurrentEditingMemento = _allMementos.FirstOrDefault(m => m.Id == args.ExistingMementoId.Value);
        }
        else if (EditingDay >= 1)
        {
            var targetDate = new DateTime(CurrentYear, EditingMonth, EditingDay);
            // Find if there's an existing child memento for this topic on this day
            CurrentEditingMemento = _allMementos.FirstOrDefault(m =>
                m.ParentId == args.Row.MementoId &&
                m.StartDate.Date == targetDate.Date);
        }
        else
        {
            CurrentEditingMemento = null;
        }

        EditingTitle = CurrentEditingMemento?.Title ?? "X";
        
        // Tag Selection logic
        IsShowEditingTag = args.Day == -1;
        EditingMementoTags.Clear();
        foreach (var tag in AvailableTags)
        {
            EditingMementoTags.Add(new SelectableTagViewModel
            {
                Id = tag.Id,
                Name = tag.Name,
                Color = tag.Color,
                IsSelected = CurrentEditingMemento?.TagIds?.Contains(tag.Id) ?? false
            });
        }

        IsEditPopupOpen = true;
    }

    [RelayCommand]
    private async Task SaveEdit()
    {
        if (EditingRow == null) return;

        var memento = CurrentEditingMemento;
        if (memento == null)
        {
            var targetDate = new DateTime(CurrentYear, EditingMonth, EditingDay);
            memento = new MementoModel
            {
                ParentId = EditingRow.MementoId,
                StartDate = targetDate,
                EndDate = targetDate,
                Color = EditingRow.Category, // Inherit from parent
                TagIds = new List<int>()
            };
        }

        memento.Title = EditingTitle;

        if (IsShowEditingTag)
        {
            memento.TagIds = EditingMementoTags.Where(t => t.IsSelected).Select(t => t.Id).ToList();
        }

        await _calendarService.SaveMementoAsync(memento);
        IsEditPopupOpen = false;
        await LoadDataAsync();
    }

    [RelayCommand]
    private async Task DeleteMemento()
    {
        if (CurrentEditingMemento == null) return;

        // In a real app, we might ask for confirmation here
        // For now, satisfy requirement: Trash icon deletes it.
        await _calendarService.DeleteMementoAsync(CurrentEditingMemento.Id);
        IsEditPopupOpen = false;
        await LoadDataAsync();
    }

    [RelayCommand]
    private void OpenColorPicker(EditCellArgs args)
    {
        EditingRow = args.Row;
        EditingDay = args.Day;
        EditingMonth = args.Month;

        if (args.ExistingMementoId.HasValue)
        {
            CurrentEditingMemento = _allMementos.FirstOrDefault(m => m.Id == args.ExistingMementoId.Value);
        }
        else
        {
            var targetDate = new DateTime(CurrentYear, EditingMonth, EditingDay);
            CurrentEditingMemento = _allMementos.FirstOrDefault(m =>
                m.ParentId == args.Row.MementoId &&
                m.StartDate.Date == targetDate.Date);
        }

        if (CurrentEditingMemento != null)
        {
            IsColorPopupOpen = true;
        }
    }

    [RelayCommand]
    private async Task SetColor(string color)
    {
        if (CurrentEditingMemento == null) return;

        CurrentEditingMemento.Color = color;
        await _calendarService.SaveMementoAsync(CurrentEditingMemento);
        IsColorPopupOpen = false;
        await LoadDataAsync();
    }

    [RelayCommand]
    private async Task OpenTagManager()
    {
        await TagManager.InitializeAsync();
        IsTagManagerOpen = true;
    }

    [RelayCommand]
    private void CloseTagManager()
    {
        IsTagManagerOpen = false;
    }

    [RelayCommand]
    private void OpenAddTopic()
    {
        AddTopicManager.Title = string.Empty;
        IsAddTopicOpen = true;
    }

    private async Task RefreshAvailableTagsAsync()
    {
        var selectedIds = AvailableTags.Where(t => t.IsSelected).Select(t => t.Id).ToHashSet();
        
        var tags = await _calendarService.GetTagsAsync();
        
        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            AvailableTags.Clear();
            foreach (var tag in tags)
            {
                var selectableTag = new SelectableTagViewModel
                {
                    Id = tag.Id,
                    Name = tag.Name,
                    Color = tag.Color,
                    IsSelected = selectedIds.Contains(tag.Id)
                };
                selectableTag.SelectedChanged += (s, e) => _ = LoadDataAsync();
                AvailableTags.Add(selectableTag);
            }
        });
        
        await LoadDataAsync();
    }
}

public class EditCellArgs
{
    public int? ExistingMementoId { get; set; }
    public MonthlyEventRowViewModel Row { get; set; } = null!;
    public int Month { get; set; }
    public int Day { get; set; }
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
    [ObservableProperty] private int _mementoId; // Topic ID
    [ObservableProperty] private int _month;
    [ObservableProperty] private string _title = string.Empty;
    [ObservableProperty] private string _category = string.Empty;
    [ObservableProperty] private string _bgColor = string.Empty;
    [ObservableProperty] private string _fgColor = string.Empty;
    public ObservableCollection<MonthlyGanttBarViewModel> Bars { get; } = new();
}

public partial class MonthlyGanttBarViewModel : ObservableObject
{
    [ObservableProperty] private int _mementoId;
    [ObservableProperty] private string _title = string.Empty;
    [ObservableProperty] private string _category = string.Empty;
    [ObservableProperty] private int _startColumn;
    [ObservableProperty] private int _duration;
    [ObservableProperty] private string _bgColor = string.Empty;
    [ObservableProperty] private string _fgColor = string.Empty;

    public IEnumerable<int> DayIndices => Enumerable.Range(0, Duration);
}

public class MonthDisplayViewModel
{
    public int MonthValue { get; set; }
    public string MonthName { get; set; } = string.Empty;
    public ObservableCollection<DayHeaderViewModel> DayHeaders { get; } = new();
    public ObservableCollection<MonthlyEventRowViewModel> EventRows { get; } = new();
}
