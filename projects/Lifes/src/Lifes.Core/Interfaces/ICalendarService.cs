using Lifes.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lifes.Core.Interfaces;

public interface ICalendarService
{
    Task<IEnumerable<MementoModel>> GetAnnualEventsAsync(int year, List<int>? tagIds = null, bool includeChildren = false);
    Task<IEnumerable<MementoModel>> GetMonthlyEventsAsync(int year, int month, List<int>? tagIds = null, bool includeChildren = false);
    Task<IEnumerable<TagModel>> GetTagsAsync();
    Task SaveMementoAsync(MementoModel memento);
    Task DeleteMementoAsync(int id);
    Task SaveTagAsync(TagModel tag);
    Task DeleteTagAsync(int id);
}
