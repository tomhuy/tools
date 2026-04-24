using Lifes.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lifes.Core.Interfaces;

public interface ICalendarService
{
    Task<IEnumerable<MementoModel>> GetAnnualEventsAsync(int year, List<int>? tagIds = null, bool includeChildren = false);
    Task<IEnumerable<MementoModel>> GetMonthlyEventsAsync(int year, int month, List<int>? tagIds = null, bool includeChildren = false);
    Task<IEnumerable<MementoModel>> GetMementosAsync(MementoQueryModel query, bool includeChildren = false);
    Task<IEnumerable<TagModel>> GetTagsAsync();
    Task<MementoModel> SaveMementoAsync(MementoModel memento);
    Task DeleteMementoAsync(int id);
    Task<TagModel> SaveTagAsync(TagModel tag);
    Task DeleteTagAsync(int id);
}
