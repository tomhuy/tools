using Lifes.Core.Models;
using Lifes.Domain.Features.DashboardChart.Entities;

namespace Lifes.Application.Features.DashboardChart.Interfaces;

public interface IDashboardDataService
{
    Task<Result<IEnumerable<DashboardBlock>>> GetBlocksAsync();
    Task<Result<DashboardCenterInfo>> GetCenterInfoAsync();
}
