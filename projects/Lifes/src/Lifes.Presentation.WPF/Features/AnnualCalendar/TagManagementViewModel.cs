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

public partial class TagManagementViewModel : ObservableObject
{
    private readonly ICalendarService _calendarService;
    
    public event Action? TagsUpdated;

    public ObservableCollection<SelectableTagViewModel> AvailableTags { get; } = new();

    [ObservableProperty] private string _editingTagName = string.Empty;
    [ObservableProperty] private string _editingTagColor = "#4CAF50";
    [ObservableProperty] private TagModel? _currentEditingTag;

    public string[] ColorPalette { get; } = new[]
    {
        "#FFFFFF", "#F2F2F2", "#D9D9D9", "#D9E1F2", "#FCE4D6", "#FDE9D9",
        "#FFF2CC", "#D9EAD3", "#E2EFDA", "#D9EBF7", "#DAE8FC", "#E1D5E7",
        "#BFBFBF", "#A5A5A5", "#7B7B7B", "#4472C4", "#ED7D31", "#FF0000",
        "#FFC000", "#70AD47", "#5B9BD5", "#255E91", "#44546A", "#262626"
    };

    public TagManagementViewModel(ICalendarService calendarService)
    {
        _calendarService = calendarService;
    }

    public async Task InitializeAsync()
    {
        await LoadTagsAsync();
    }

    [RelayCommand]
    private async Task LoadTagsAsync()
    {
        var tags = await _calendarService.GetTagsAsync();
        AvailableTags.Clear();
        foreach (var tag in tags)
        {
            AvailableTags.Add(new SelectableTagViewModel
            {
                Id = tag.Id,
                Name = tag.Name,
                Color = tag.Color
            });
        }
    }

    [RelayCommand]
    private void StartEditTag(SelectableTagViewModel selectableTag)
    {
        CurrentEditingTag = new TagModel 
        { 
            Id = selectableTag.Id, 
            Name = selectableTag.Name, 
            Color = selectableTag.Color 
        };
        EditingTagName = selectableTag.Name;
        EditingTagColor = selectableTag.Color;
    }

    [RelayCommand]
    private void SetTagColor(string color)
    {
        EditingTagColor = color;
    }

    [RelayCommand]
    private async Task SaveTag()
    {
        if (string.IsNullOrWhiteSpace(EditingTagName)) return;

        var tag = CurrentEditingTag ?? new TagModel();
        tag.Name = EditingTagName;
        tag.Color = EditingTagColor;

        await _calendarService.SaveTagAsync(tag);
        
        await LoadTagsAsync();
        TagsUpdated?.Invoke();
        
        // Reset form
        CurrentEditingTag = null;
        EditingTagName = string.Empty;
        EditingTagColor = "#4CAF50";
    }

    [RelayCommand]
    private async Task DeleteTag(SelectableTagViewModel selectableTag)
    {
        await _calendarService.DeleteTagAsync(selectableTag.Id);
        await LoadTagsAsync();
        TagsUpdated?.Invoke();
        
        if (CurrentEditingTag?.Id == selectableTag.Id)
        {
            CurrentEditingTag = null;
            EditingTagName = string.Empty;
        }
    }
}
