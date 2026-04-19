using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lifes.Core.Interfaces;
using Lifes.Core.Models;
using Lifes.Presentation.WPF.Models;

namespace Lifes.Presentation.WPF.Features.AnnualCalendar;

public partial class AddTopicViewModel : ObservableObject
{
    private readonly ICalendarService _calendarService;

    public event Action? TopicAdded;
    public event Action? RequestClose;

    [ObservableProperty] private string _title = string.Empty;
    [ObservableProperty] private DateTime _startDate = DateTime.Today;
    [ObservableProperty] private DateTime _endDate = DateTime.Today;
    [ObservableProperty] private string _selectedColor = "#4472C4"; // Default blue
    [ObservableProperty] private bool _isSaving;

    public ObservableCollection<SelectableTagViewModel> AvailableTags { get; } = new();

    public string[] ColorPalette { get; } = new[]
    {
        "#FFFFFF", "#F2F2F2", "#D9D9D9", "#D9E1F2", "#FCE4D6", "#FDE9D9",
        "#FFF2CC", "#D9EAD3", "#E2EFDA", "#D9EBF7", "#DAE8FC", "#E1D5E7",
        "#BFBFBF", "#A5A5A5", "#7B7B7B", "#4472C4", "#ED7D31", "#FF0000",
        "#FFC000", "#70AD47", "#5B9BD5", "#255E91", "#44546A", "#262626"
    };

    public AddTopicViewModel(ICalendarService calendarService)
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
            var newTopic = new MementoModel
            {
                Title = Title,
                StartDate = StartDate,
                EndDate = EndDate,
                Color = SelectedColor,
                ParentId = null,
                CreatedDate = DateTime.Now,
                Order = 0,
                TagIds = AvailableTags.Where(t => t.IsSelected).Select(t => t.Id).ToList()
            };

            await _calendarService.SaveMementoAsync(newTopic);
            
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
