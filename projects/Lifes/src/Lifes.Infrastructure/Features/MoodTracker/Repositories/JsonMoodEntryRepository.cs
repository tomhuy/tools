using Lifes.Core.Interfaces;
using Lifes.Core.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Lifes.Infrastructure.Features.MoodTracker.Repositories;

public class JsonMoodEntryRepository : IMoodEntryRepository
{
    private readonly ILogger<JsonMoodEntryRepository> _logger;
    private readonly string _filePath;
    private readonly List<MoodEntry> _entries;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public JsonMoodEntryRepository(ILogger<JsonMoodEntryRepository> logger)
    {
        _logger = logger;
        
        var appDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var databaseDir = Path.Combine(appDirectory, "database");
        if (!Directory.Exists(databaseDir))
        {
            Directory.CreateDirectory(databaseDir);
        }
        
        _filePath = Path.Combine(databaseDir, "mood_entries.json");
        _entries = LoadData();
    }

    private List<MoodEntry> LoadData()
    {
        try
        {
            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    return JsonSerializer.Deserialize<List<MoodEntry>>(json, _jsonOptions) ?? new List<MoodEntry>();
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load mood entries from {FilePath}", _filePath);
        }

        return new List<MoodEntry>();
    }

    private void SaveData(List<MoodEntry> data)
    {
        try
        {
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            File.WriteAllText(_filePath, json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save mood entries to {FilePath}", _filePath);
        }
    }

    public Task<IEnumerable<MoodEntry>> GetAllAsync()
    {
        return Task.FromResult(_entries.AsEnumerable());
    }

    public Task<IEnumerable<MoodEntry>> GetByRangeAsync(DateTime start, DateTime end)
    {
        var result = _entries.Where(e => e.Date >= start && e.Date <= end);
        return Task.FromResult(result);
    }

    public async Task<MoodEntry> AddOrUpdateAsync(MoodEntry entry)
    {
        if (string.IsNullOrEmpty(entry.Id))
        {
            entry.Id = Guid.NewGuid().ToString();
            _entries.Add(entry);
        }
        else
        {
            var existing = _entries.FirstOrDefault(e => e.Id == entry.Id);
            if (existing != null)
            {
                existing.Date = entry.Date;
                existing.MoodId = entry.MoodId;
                existing.Tags = entry.Tags;
                existing.Note = entry.Note;
                existing.Reason = entry.Reason;
            }
            else
            {
                _entries.Add(entry);
            }
        }
        
        await Task.Run(() => SaveData(_entries));
        return entry;
    }

    public async Task DeleteAsync(string id)
    {
        var existing = _entries.FirstOrDefault(e => e.Id == id);
        if (existing != null)
        {
            _entries.Remove(existing);
            await Task.Run(() => SaveData(_entries));
        }
    }
}
