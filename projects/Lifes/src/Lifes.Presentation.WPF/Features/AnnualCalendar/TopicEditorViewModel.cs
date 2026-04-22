using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lifes.Core.Interfaces;
using Lifes.Core.Models;
using Lifes.Presentation.WPF.Constants;
using Lifes.Presentation.WPF.Models;

namespace Lifes.Presentation.WPF.Features.AnnualCalendar;

public partial class TopicEditorViewModel : ObservableObject
{
    private readonly ICalendarService _calendarService;

    public event Action? TopicAdded;
    public event Action? RequestClose;

    [ObservableProperty] private string _headerTitle = "Thêm Chủ đề mới";
    [ObservableProperty] private string _title = string.Empty;
    [ObservableProperty] private DateTime _startDate = DateTime.Today;
    [ObservableProperty] private DateTime _endDate = DateTime.Today;
    [ObservableProperty] private string _selectedColor = "#4472C4"; // Default blue
    [ObservableProperty] private bool _isSaving;
    [ObservableProperty] private int? _editingMementoId;
    [ObservableProperty] private int _order;

    public ObservableCollection<SelectableTagViewModel> AvailableTags { get; } = new();

    public string[] ColorPalette { get; } = UIConstants.StandardColorPalette;

    public TopicEditorViewModel(ICalendarService calendarService)
    {
        _calendarService = calendarService;
        _ = InitializeAsync();
    }

    public async Task InitializeAsync()
    {
        AvailableTags.Clear();
        var tags = await _calendarService.GetTagsAsync();
        foreach (var tag in tags)
        {
            AvailableTags.Add(new SelectableTagViewModel
            {
                Id = tag.Id,
                Name = tag.Name,
                Color = tag.Color,
                IsSelected = false
            });
        }
    }

    public void LoadMemento(MementoModel memento)
    {
        EditingMementoId = memento.Id;
        Title = memento.Title;
        StartDate = memento.StartDate;
        EndDate = memento.EndDate;
        SelectedColor = memento.Color ?? "#4472C4";
        HeaderTitle = "Sửa Chủ đề";
        Order = memento.Order;

        // Mark selected tags
        var tagIds = memento.TagIds?.ToHashSet() ?? new HashSet<int>();
        foreach (var tag in AvailableTags)
        {
            tag.IsSelected = tagIds.Contains(tag.Id);
        }
    }

    public void Reset()
    {
        EditingMementoId = null;
        Title = string.Empty;
        StartDate = DateTime.Today;
        EndDate = DateTime.Today;
        SelectedColor = "#4472C4";
        HeaderTitle = "Thêm Chủ đề mới";
        Order = 0;
        foreach (var tag in AvailableTags)
        {
            tag.IsSelected = false;
        }
    }

    [RelayCommand]
    private void SetColor(string color)
    {
        SelectedColor = color;
    }

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task Save()
    {
        if (string.IsNullOrWhiteSpace(Title)) return;

        IsSaving = true;
        try
        {
            var memento = new MementoModel
            {
                Id = EditingMementoId ?? 0,
                Title = Title,
                StartDate = StartDate,
                EndDate = EndDate,
                Color = SelectedColor,
                ParentId = null,
                CreatedDate = DateTime.Now,
                Order = Order,
                TagIds = AvailableTags.Where(t => t.IsSelected).Select(t => t.Id).ToList()
            };

            await _calendarService.SaveMementoAsync(memento);
            
            TopicAdded?.Invoke();
            RequestClose?.Invoke();
        }
        finally
        {
            IsSaving = false;
        }
    }

    private bool CanSave()
    {
        return !string.IsNullOrWhiteSpace(Title) && EndDate >= StartDate;
    }

    partial void OnTitleChanged(string value) => SaveCommand.NotifyCanExecuteChanged();
    partial void OnStartDateChanged(DateTime value) => SaveCommand.NotifyCanExecuteChanged();
    partial void OnEndDateChanged(DateTime value) => SaveCommand.NotifyCanExecuteChanged();

    [RelayCommand]
    private void Cancel()
    {
        RequestClose?.Invoke();
    }
}
