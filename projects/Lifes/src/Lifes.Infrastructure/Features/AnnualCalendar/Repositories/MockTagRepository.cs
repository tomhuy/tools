using Lifes.Core.Interfaces;
using Lifes.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lifes.Infrastructure.Features.AnnualCalendar.Repositories;

public class MockTagRepository : ITagRepository
{
    private readonly List<TagModel> _tags;

    public MockTagRepository()
    {
        _tags = new List<TagModel>
        {
            new TagModel { Id = 1, Name = "Work", Color = "#4CAF50" },
            new TagModel { Id = 2, Name = "Personal", Color = "#2196F3" },
            new TagModel { Id = 3, Name = "Health", Color = "#F44336" },
            new TagModel { Id = 4, Name = "Learning", Color = "#9C27B0" },
            new TagModel { Id = 5, Name = "Review", Color = "#FFC107" },
            new TagModel { Id = 6, Name = "Psychology", Color = "#4CAF50" }
        };
    }

    public Task<IEnumerable<TagModel>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<TagModel>>(_tags);
    }

    public Task<TagModel> SaveAsync(TagModel tag) => Task.FromResult(tag);
    public Task DeleteAsync(int id) => Task.CompletedTask;
}
