using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lifes.Core.Interfaces;
using Lifes.Core.Models;
using Lifes.Presentation.WPF.Constants;
using Lifes.Presentation.WPF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Lifes.Presentation.WPF.Features.AnnualCalendar;

public partial class MementoManagementViewModel : ObservableObject
{
    private readonly ICalendarService _calendarService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private bool _includeChildren;

    [ObservableProperty]
    private bool _parentOnly = true;

    [ObservableProperty]
    private string _searchKeyword = string.Empty;

    [ObservableProperty]
    private bool _isTopicEditorOpen;

    public TopicEditorViewModel TopicEditor { get; }
    public ObservableCollection<MementoModel> Mementos { get; } = new();
    public ObservableCollection<SelectableTagViewModel> AvailableTags { get; } = new();
    public ObservableCollection<ToolMenuItem> NavigationMenuItems { get; } = new();

    public MementoManagementViewModel(ICalendarService calendarService, INavigationService navigationService)
    {
        _calendarService = calendarService;
        _navigationService = navigationService;

        TopicEditor = new TopicEditorViewModel(calendarService);
        TopicEditor.TopicAdded += async () => await LoadDataAsync();
        TopicEditor.RequestClose += () => IsTopicEditorOpen = false;

        InitializeNavigationMenu();
        _ = InitializeTagsAsync();
        _ = LoadDataAsync();
    }

    private async Task InitializeTagsAsync()
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

    private void InitializeNavigationMenu()
    {
        var tools = _navigationService.GetAllTools();
        foreach (var t in tools)
        {
            NavigationMenuItems.Add(new ToolMenuItem
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                IsActive = t.Id == ToolIds.MementoManagement,
                NavigateCommand = new RelayCommand(
                    () => _navigationService.NavigateTo(t.Id),
                    () => t.Id != ToolIds.MementoManagement)
            });
        }
    }

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        var selectedTagIds = AvailableTags.Where(t => t.IsSelected).Select(t => t.Id).ToList();
        
        var query = new MementoQueryModel
        {
            TagIds = selectedTagIds.Any() ? selectedTagIds : null,
            ParentOnly = ParentOnly,
            Keyword = SearchKeyword
        };

        var results = await _calendarService.GetMementosAsync(query, IncludeChildren);
        
        Mementos.Clear();
        // Sort by Order ASC, then by Title
        var sorted = results.OrderBy(m => m.Order).ThenBy(m => m.Title);
        foreach (var m in sorted)
        {
            Mementos.Add(m);
        }
    }

    [RelayCommand]
    private async Task SaveChangesAsync()
    {
        foreach (var memento in Mementos)
        {
            await _calendarService.SaveMementoAsync(memento);
        }
        
        // Refresh to show sorted results
        await LoadDataAsync();
    }

    [RelayCommand]
    private void EditMemento(MementoModel memento)
    {
        if (memento == null) return;
        
        TopicEditor.LoadMemento(memento);
        IsTopicEditorOpen = true;
    }

    partial void OnIncludeChildrenChanged(bool value) => _ = LoadDataAsync();
    partial void OnParentOnlyChanged(bool value) => _ = LoadDataAsync();
    partial void OnSearchKeywordChanged(string value) => _ = LoadDataAsync();
}
