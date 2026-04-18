using Lifes.Core.Interfaces;
using Lifes.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lifes.Infrastructure.Features.AnnualCalendar.Services;

public class CalendarService : ICalendarService
{
    private readonly IMementoRepository _mementoRepository;
    private readonly ITagRepository _tagRepository;

    public CalendarService(IMementoRepository mementoRepository, ITagRepository tagRepository)
    {
        _mementoRepository = mementoRepository;
        _tagRepository = tagRepository;
    }

    public async Task<IEnumerable<TagModel>> GetTagsAsync()
    {
        return await _tagRepository.GetAllAsync();
    }

    public async Task<IEnumerable<MementoModel>> GetAnnualEventsAsync(int year, List<int>? tagIds = null, bool includeChildren = false)
    {
        var query = new MementoQueryModel
        {
            StartDate = new DateTime(year, 1, 1),
            EndDate = new DateTime(year, 12, 31),
            TagIds = tagIds
        };

        var initialResults = await _mementoRepository.GetByQueryAsync(query);
        var mementos = initialResults.ToList();

        if (includeChildren)
        {
            await FetchChildrenRecursiveAsync(mementos);
        }

        return mementos;
    }

    public async Task<IEnumerable<MementoModel>> GetMonthlyEventsAsync(int year, int month, List<int>? tagIds = null, bool includeChildren = false)
    {
        var monthStart = new DateTime(year, month, 1);
        var monthEnd = new DateTime(year, month, DateTime.DaysInMonth(year, month));

        var query = new MementoQueryModel
        {
            StartDate = monthStart,
            EndDate = monthEnd,
            TagIds = tagIds
        };

        var initialResults = await _mementoRepository.GetByQueryAsync(query);
        var mementos = initialResults.ToList();

        if (includeChildren)
        {
            await FetchChildrenRecursiveAsync(mementos);
        }

        return mementos;
    }

    private async Task FetchChildrenRecursiveAsync(List<MementoModel> mementos)
    {
        var currentLevelIds = mementos.Select(m => m.Id).Distinct().ToList();
        var allIds = new HashSet<int>(currentLevelIds);

        while (currentLevelIds.Any())
        {
            var children = await _mementoRepository.GetChildrenAsync(currentLevelIds);
            var newChildren = children.Where(c => !allIds.Contains(c.Id)).ToList();

            if (!newChildren.Any()) break;

            mementos.AddRange(newChildren);
            currentLevelIds = newChildren.Select(c => c.Id).Distinct().ToList();
            foreach (var id in currentLevelIds) allIds.Add(id);
        }
    }
}
