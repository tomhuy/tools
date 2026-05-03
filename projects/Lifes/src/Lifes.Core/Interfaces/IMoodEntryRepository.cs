using Lifes.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lifes.Core.Interfaces;

public interface IMoodEntryRepository
{
    Task<IEnumerable<MoodEntry>> GetAllAsync();
    Task<IEnumerable<MoodEntry>> GetByRangeAsync(DateTime start, DateTime end);
    Task<MoodEntry> AddOrUpdateAsync(MoodEntry entry);
    Task DeleteAsync(string id);
}
