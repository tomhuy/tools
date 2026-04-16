namespace Lifes.Presentation.WPF.Features.DocumentManagement;

using CommunityToolkit.Mvvm.ComponentModel;
using Lifes.Core.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.Input;
using Lifes.Core.Interfaces;
using Lifes.Presentation.WPF.Models;
using Lifes.Presentation.WPF.Constants;

public partial class DocumentManagementViewModel : ObservableObject
{
    private readonly IDocumentService _documentService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private int _currentYear;

    [ObservableProperty]
    private int _currentMonth;

    [ObservableProperty]
    private string _monthName = string.Empty;

    public ObservableCollection<DayHeaderViewModel> DayHeaders { get; } = new();
    public ObservableCollection<DocumentRowViewModel> DocumentRows { get; } = new();

    [ObservableProperty]
    private ObservableCollection<ToolMenuItem> _navigationMenuItems = new();

    public DocumentManagementViewModel(IDocumentService documentService, INavigationService navigationService)
    {
        _documentService = documentService;
        _navigationService = navigationService;
        
        var today = DateTime.Today;
        CurrentYear = today.Year;
        CurrentMonth = today.Month;
        MonthName = $"{CurrentMonth}/{CurrentYear}";
        
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
                IsActive = t.Id == ToolIds.DocumentManagement,
                NavigateCommand = new RelayCommand(
                    () => _navigationService.NavigateTo(t.Id),
                    () => t.Id != ToolIds.DocumentManagement)
            })
        );
    }

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        var documents = await _documentService.GetDocumentsAsync(CurrentYear, CurrentMonth);
        
        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            BuildHeaders();
            BuildRows(documents);
        });
    }

    private void BuildHeaders()
    {
        DayHeaders.Clear();
        int daysInMonth = DateTime.DaysInMonth(CurrentYear, CurrentMonth);
        var today = DateTime.Today;

        for (int i = 1; i <= daysInMonth; i++)
        {
            var date = new DateTime(CurrentYear, CurrentMonth, i);
            bool isToday = date == today;
            string dayOfWeek = date.DayOfWeek switch
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

            DayHeaders.Add(new DayHeaderViewModel
            {
                DayNumber = i,
                DayName = dayOfWeek,
                IsToday = isToday
            });
        }
    }

    private void BuildRows(System.Collections.Generic.IEnumerable<DocumentModel> documents)
    {
        DocumentRows.Clear();
        int daysInMonth = DateTime.DaysInMonth(CurrentYear, CurrentMonth);

        foreach (var doc in documents)
        {
            var row = new DocumentRowViewModel
            {
                Title = doc.Title,
                IsSubtask = doc.IsTask,
            };

            for (int i = 1; i <= daysInMonth; i++)
            {
                var date = new DateTime(CurrentYear, CurrentMonth, i);
                
                // Active if StartDate/EndDate covers this date
                bool isActive = false;
                if (doc.StartDate.HasValue && doc.EndDate.HasValue)
                {
                     isActive = doc.StartDate.Value.Date <= date.Date && doc.EndDate.Value.Date >= date.Date;
                }
                
                bool isToday = date == DateTime.Today;

                row.Cells.Add(new DayCellViewModel
                {
                    DayNumber = i,
                    IsActive = isActive,
                    IsToday = isToday
                });
            }

            DocumentRows.Add(row);
        }
    }
}

public partial class DayHeaderViewModel : ObservableObject
{
    [ObservableProperty] private int _dayNumber;
    [ObservableProperty] private string _dayName = string.Empty;
    [ObservableProperty] private bool _isToday;
}

public partial class DocumentRowViewModel : ObservableObject
{
    [ObservableProperty] private string _title = string.Empty;
    [ObservableProperty] private bool _isSubtask;
    public ObservableCollection<DayCellViewModel> Cells { get; } = new();
}

public partial class DayCellViewModel : ObservableObject
{
    [ObservableProperty] private int _dayNumber;
    [ObservableProperty] private bool _isActive;
    [ObservableProperty] private bool _isToday;
}
