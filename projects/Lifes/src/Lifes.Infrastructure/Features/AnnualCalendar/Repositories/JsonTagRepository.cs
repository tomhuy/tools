using Lifes.Core.Interfaces;
using Lifes.Core.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Lifes.Infrastructure.Features.AnnualCalendar.Repositories;

public class JsonTagRepository : ITagRepository
{
    private readonly ILogger<JsonTagRepository> _logger;
    private readonly string _filePath;
    private readonly List<TagModel> _tags;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public JsonTagRepository(ILogger<JsonTagRepository> logger)
    {
        _logger = logger;
        
        var appDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var databaseDir = Path.Combine(appDirectory, "database");
        if (!Directory.Exists(databaseDir))
        {
            Directory.CreateDirectory(databaseDir);
        }
        
        _filePath = Path.Combine(databaseDir, "tags.json");
        _tags = LoadData();
    }

    private List<TagModel> LoadData()
    {
        try
        {
            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    return JsonSerializer.Deserialize<List<TagModel>>(json, _jsonOptions) ?? new List<TagModel>();
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load tags from {FilePath}", _filePath);
        }

        // Seed if file not found or corrupted
        var seedData = GenerateSeedData();
        SaveData(seedData);
        return seedData;
    }

    private void SaveData(List<TagModel> data)
    {
        try
        {
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            File.WriteAllText(_filePath, json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save tags to {FilePath}", _filePath);
        }
    }

    public Task<IEnumerable<TagModel>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<TagModel>>(_tags);
    }
    
    public Task SaveAsync(TagModel tag)
    {
        if (tag.Id == 0)
        {
            tag.Id = _tags.Any() ? _tags.Max(t => t.Id) + 1 : 1;
            _tags.Add(tag);
        }
        else
        {
            var existing = _tags.FirstOrDefault(t => t.Id == tag.Id);
            if (existing != null)
            {
                existing.Name = tag.Name;
                existing.Color = tag.Color;
            }
        }
        
        SaveData(_tags);
        return Task.CompletedTask;
    }
    
    public Task DeleteAsync(int id)
    {
        var tag = _tags.FirstOrDefault(t => t.Id == id);
        if (tag != null)
        {
            _tags.Remove(tag);
            SaveData(_tags);
        }
        return Task.CompletedTask;
    }

    private List<TagModel> GenerateSeedData()
    {
        return new List<TagModel>
        {
            new TagModel { Id = 1, Name = "Work", Color = "#4CAF50" },
            new TagModel { Id = 2, Name = "Personal", Color = "#2196F3" },
            new TagModel { Id = 3, Name = "Health", Color = "#F44336" },
            new TagModel { Id = 4, Name = "Learning", Color = "#9C27B0" },
            new TagModel { Id = 5, Name = "Review", Color = "#FFC107" },
            new TagModel { Id = 6, Name = "Psychology", Color = "#4CAF50" }
        };
    }
}
