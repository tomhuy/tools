using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lifes.Application.Features.DashboardChart.Interfaces;
using Lifes.Domain.Features.DashboardChart.Entities;
using Lifes.Core.Interfaces;
using Lifes.Presentation.WPF.Models;
using Lifes.Presentation.WPF.Constants;

namespace Lifes.Presentation.WPF.Features.DashboardChart;

public partial class DashboardChartViewModel : ObservableObject
{
    private readonly IDashboardDataService _dataService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private ObservableCollection<DashboardBlock> _blocks = new();

    [ObservableProperty]
    private DashboardCenterInfo? _centerInfo;

    [ObservableProperty]
    private ObservableCollection<ToolMenuItem> _navigationMenuItems = new();

    public DashboardChartViewModel(IDashboardDataService dataService, INavigationService navigationService)
    {
        _dataService = dataService;
        _navigationService = navigationService;
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
                IsActive = t.Id == ToolIds.DashboardChart,
                NavigateCommand = new RelayCommand(
                    () => _navigationService.NavigateTo(t.Id),
                    () => t.Id != ToolIds.DashboardChart)
            })
        );
    }

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        var blocksResult = await _dataService.GetBlocksAsync();
        if (blocksResult.IsSuccess)
        {
            Blocks = new ObservableCollection<DashboardBlock>(blocksResult.Value ?? Enumerable.Empty<DashboardBlock>());
        }

        var centerResult = await _dataService.GetCenterInfoAsync();
        if (centerResult.IsSuccess)
        {
            CenterInfo = centerResult.Value;
        }
    }
}
