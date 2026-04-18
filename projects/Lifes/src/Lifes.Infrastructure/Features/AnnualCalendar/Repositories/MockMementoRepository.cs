using Lifes.Core.Interfaces;
using Lifes.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lifes.Infrastructure.Features.AnnualCalendar.Repositories;

public class MockMementoRepository : IMementoRepository
{
    private readonly List<MementoModel> _mementos;

    public MockMementoRepository()
    {
        _mementos = GenerateMockData();
    }

    public Task<IEnumerable<MementoModel>> GetByQueryAsync(MementoQueryModel query)
    {
        var result = _mementos.AsEnumerable();

        if (query.StartDate.HasValue)
            result = result.Where(m => m.EndDate >= query.StartDate.Value);

        if (query.EndDate.HasValue)
            result = result.Where(m => m.StartDate <= query.EndDate.Value);

        if (query.TagIds != null && query.TagIds.Any())
            result = result.Where(m => m.TagIds.Any(t => query.TagIds.Contains(t)));

        return Task.FromResult(result);
    }

    public Task<IEnumerable<MementoModel>> GetChildrenAsync(IEnumerable<int> parentIds)
    {
        var result = _mementos.Where(m => m.ParentId.HasValue && parentIds.Contains(m.ParentId.Value));
        return Task.FromResult<IEnumerable<MementoModel>>(result);
    }

    private List<MementoModel> GenerateMockData()
    {
        var mementos = new List<MementoModel>();
        int idCounter = 1;

        // Years to generate
        int[] years = { 2025, 2026 };

        foreach (var year in years)
        {
            mementos.Add(new MementoModel { Id = idCounter++, Title = "Annual Plan", StartDate = new DateTime(year, 2, 16), EndDate = new DateTime(year, 2, 17), Color = "Planning", TagIds = new List<int> { 1 } });
            
            mementos.Add(new MementoModel { Id = idCounter++, Title = "ST1-T...", StartDate = new DateTime(year, 3, 2), EndDate = new DateTime(year, 3, 3), Color = "Learning", TagIds = new List<int> { 4 } });
            mementos.Add(new MementoModel { Id = idCounter++, Title = "CVS-...", StartDate = new DateTime(year, 3, 4), EndDate = new DateTime(year, 3, 5), Color = "Learning", TagIds = new List<int> { 4 } });
            mementos.Add(new MementoModel { Id = idCounter++, Title = "ST1-T...", StartDate = new DateTime(year, 3, 6), EndDate = new DateTime(year, 3, 7), Color = "Learning", TagIds = new List<int> { 4 } });
            mementos.Add(new MementoModel { Id = idCounter++, Title = "RC-12345", StartDate = new DateTime(year, 3, 9), EndDate = new DateTime(year, 3, 11), Color = "Release", TagIds = new List<int> { 1 } });
            mementos.Add(new MementoModel { Id = idCounter++, Title = "di au...", StartDate = new DateTime(year, 3, 9), EndDate = new DateTime(year, 3, 10), Color = "Competition", TagIds = new List<int> { 1, 2 } });
            mementos.Add(new MementoModel { Id = idCounter++, Title = "ST1-T...", StartDate = new DateTime(year, 3, 11), EndDate = new DateTime(year, 3, 12), Color = "Learning", TagIds = new List<int> { 4 } });

            mementos.Add(new MementoModel { Id = idCounter++, Title = "RC-6", StartDate = new DateTime(year, 3, 9), EndDate = new DateTime(year, 3, 9), Color = "Release", TagIds = new List<int> { 1 } });
            mementos.Add(new MementoModel { Id = idCounter++, Title = "RC-7", StartDate = new DateTime(year, 3, 10), EndDate = new DateTime(year, 3, 11), Color = "Release", TagIds = new List<int> { 1 } });
            mementos.Add(new MementoModel { Id = idCounter++, Title = "Suy ngẫm sau Q1", StartDate = new DateTime(year, 3, 20), EndDate = new DateTime(year, 3, 28), Color = "Review", TagIds = new List<int> { 5 } });
            mementos.Add(new MementoModel { Id = idCounter++, Title = "Hàn cướp", StartDate = new DateTime(year, 3, 24), EndDate = new DateTime(year, 3, 29), Color = "Travel", TagIds = new List<int> { 2 } });
            
            mementos.Add(new MementoModel { Id = idCounter++, Title = "Health Check", StartDate = new DateTime(year, 4, 3), EndDate = new DateTime(year, 4, 4), Color = "Health", TagIds = new List<int> { 3 } });
            mementos.Add(new MementoModel { Id = idCounter++, Title = "Project Alpha Review", StartDate = new DateTime(year, 4, 6), EndDate = new DateTime(year, 4, 8), Color = "Work", TagIds = new List<int> { 1 } });
            
            mementos.Add(new MementoModel { Id = idCounter++, Title = "Spring Conference", StartDate = new DateTime(year, 4, 13), EndDate = new DateTime(year, 4, 17), Color = "Conference", TagIds = new List<int> { 1, 4 } });
            mementos.Add(new MementoModel { Id = idCounter++, Title = "Family Weekend", StartDate = new DateTime(year, 4, 18), EndDate = new DateTime(year, 4, 19), Color = "Personal", TagIds = new List<int> { 2 } });
            mementos.Add(new MementoModel { Id = idCounter++, Title = "Security Audit", StartDate = new DateTime(year, 4, 21), EndDate = new DateTime(year, 4, 23), Color = "Work", TagIds = new List<int> { 1 } });
            mementos.Add(new MementoModel { Id = idCounter++, Title = "New Tech Research", StartDate = new DateTime(year, 4, 25), EndDate = new DateTime(year, 4, 30), Color = "Learning", TagIds = new List<int> { 4 } });
            
            mementos.Add(new MementoModel { Id = idCounter++, Title = "Hackathon", StartDate = new DateTime(year, 5, 8), EndDate = new DateTime(year, 5, 14), Color = "Work", TagIds = new List<int> { 1, 4 } });
            
            mementos.Add(new MementoModel { Id = idCounter++, Title = "Data Release", StartDate = new DateTime(year, 6, 1), EndDate = new DateTime(year, 6, 2), Color = "Release", TagIds = new List<int> { 1 } });
            
            mementos.Add(new MementoModel { Id = idCounter++, Title = "Mid-Year Review", StartDate = new DateTime(year, 7, 1), EndDate = new DateTime(year, 7, 4), Color = "Review", TagIds = new List<int> { 5 } });
            
            mementos.Add(new MementoModel { Id = idCounter++, Title = "Summer Vacation", StartDate = new DateTime(year, 8, 8), EndDate = new DateTime(year, 8, 28), Color = "Travel", TagIds = new List<int> { 2 } });
            
            mementos.Add(new MementoModel { Id = idCounter++, Title = "Product Launch Prep", StartDate = new DateTime(year, 9, 1), EndDate = new DateTime(year, 9, 5), Color = "Work", TagIds = new List<int> { 1 } });
            mementos.Add(new MementoModel { Id = idCounter++, Title = "Award Ceremony", StartDate = new DateTime(year, 9, 19), EndDate = new DateTime(year, 9, 21), Color = "Event", TagIds = new List<int> { 2 } });
            
            mementos.Add(new MementoModel { Id = idCounter++, Title = "Skill Development Program", StartDate = new DateTime(year, 10, 1), EndDate = new DateTime(year, 11, 14), Color = "Learning", TagIds = new List<int> { 4 } });
            
            mementos.Add(new MementoModel { Id = idCounter++, Title = "Product Launch", StartDate = new DateTime(year, 11, 18), EndDate = new DateTime(year, 11, 20), Color = "Release", TagIds = new List<int> { 1 } });
            mementos.Add(new MementoModel { Id = idCounter++, Title = "Holiday Travel", StartDate = new DateTime(year, 11, 19), EndDate = new DateTime(year, 11, 28), Color = "Travel", TagIds = new List<int> { 2 } });
            
            mementos.Add(new MementoModel { Id = idCounter++, Title = "Year End Review", StartDate = new DateTime(year, 12, 5), EndDate = new DateTime(year, 12, 17), Color = "Review", TagIds = new List<int> { 5 } });
            mementos.Add(new MementoModel { Id = idCounter++, Title = "Personal Reflection", StartDate = new DateTime(year, 12, 22), EndDate = new DateTime(year, 12, 29), Color = "Personal", TagIds = new List<int> { 2 } });

            // Product Launch Cycle
            int launchCycleId = idCounter++;
            mementos.Add(new MementoModel { Id = launchCycleId, Title = "Product Launch Cycle", StartDate = new DateTime(year, 4, 10), EndDate = new DateTime(year, 4, 28), Color = "Work", TagIds = new List<int> { 1 } });
            mementos.Add(new MementoModel { Id = idCounter++, Title = "Preparation", StartDate = new DateTime(year, 4, 10), EndDate = new DateTime(year, 4, 14), Color = "Work", ParentId = launchCycleId });
            mementos.Add(new MementoModel { Id = idCounter++, Title = "Main Event", StartDate = new DateTime(year, 4, 18), EndDate = new DateTime(year, 4, 22), Color = "Work", ParentId = launchCycleId });
            mementos.Add(new MementoModel { Id = idCounter++, Title = "Wrap-up", StartDate = new DateTime(year, 4, 25), EndDate = new DateTime(year, 4, 28), Color = "Work", ParentId = launchCycleId });
        }

        // Psychology Events
        int psychologyTopicId = 1000;
        mementos.Add(new MementoModel { Id = psychologyTopicId, Title = "Học tâm lý học", StartDate = new DateTime(2025, 9, 11), EndDate = new DateTime(2026, 4, 13), Color = "Psychology", TagIds = new List<int> { 4, 6 } });

        var psychDates = new[] {
            (2025,9,11), (2025,9,12), (2025,9,13), (2025,9,14), (2025,9,16), (2025,9,20), (2025,9,22), (2025,9,24), (2025,9,29),
            (2025,10,1), (2025,10,2), (2025,10,6), (2025,10,8), (2025,10,10), (2025,10,13), (2025,10,15), (2025,10,17), (2025,10,20), (2025,10,22), (2025,10,24), (2025,10,27), (2025,10,29),
            (2025,11,1), (2025,11,3), (2025,11,8), (2025,11,13), (2025,11,14), (2025,11,17), (2025,11,19),
            (2025,12,1), (2025,12,4), (2025,12,5), (2025,12,9), (2025,12,17), (2025,12,24),
            (2026,1,5), (2026,1,7), (2026,1,9), (2026,1,12), (2026,1,14), (2026,1,16), (2026,1,19), (2026,1,23), (2026,1,26), (2026,1,29),
            (2026,2,4), (2026,2,10), (2026,2,12), (2026,2,17), (2026,2,19), (2026,2,22), (2026,2,24), (2026,2,26),
            (2026,3,2), (2026,3,4), (2026,3,6), (2026,3,9), (2026,3,11), (2026,3,13), (2026,3,16), (2026,3,20), (2026,3,30),
            (2026,4,1), (2026,4,3), (2026,4,6), (2026,4,8), (2026,4,13)
        };

        foreach (var d in psychDates)
        {
            mementos.Add(new MementoModel { Id = idCounter++, Title = "Học", StartDate = new DateTime(d.Item1, d.Item2, d.Item3), EndDate = new DateTime(d.Item1, d.Item2, d.Item3), Color = "Psychology", ParentId = psychologyTopicId });
        }

        return mementos;
    }
}
