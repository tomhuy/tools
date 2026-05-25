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

public class JsonMoodMetadataRepository : IMoodMetadataRepository
{
    private readonly ILogger<JsonMoodMetadataRepository> _logger;
    private readonly string _filePath;
    private readonly List<MoodMetadataDefinition> _definitions;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public JsonMoodMetadataRepository(ILogger<JsonMoodMetadataRepository> logger)
    {
        _logger = logger;
        
        var appDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var databaseDir = Path.Combine(appDirectory, "database");
        if (!Directory.Exists(databaseDir))
        {
            Directory.CreateDirectory(databaseDir);
        }
        
        _filePath = Path.Combine(databaseDir, "mood_metadata_definitions.json");
        _definitions = LoadData();
    }

    private List<MoodMetadataDefinition> LoadData()
    {
        try
        {
            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    return JsonSerializer.Deserialize<List<MoodMetadataDefinition>>(json, _jsonOptions) ?? new List<MoodMetadataDefinition>();
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load mood metadata definitions from {FilePath}", _filePath);
        }

        return new List<MoodMetadataDefinition>();
    }

    private void SaveData(List<MoodMetadataDefinition> data)
    {
        try
        {
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            File.WriteAllText(_filePath, json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save mood metadata definitions to {FilePath}", _filePath);
        }
    }

    public Task<IEnumerable<MoodMetadataDefinition>> GetAllAsync()
    {
        lock (_definitions)
        {
            return Task.FromResult(_definitions.AsEnumerable());
        }
    }

    public async Task<MoodMetadataDefinition> AddOrUpdateAsync(MoodMetadataDefinition definition)
    {
        if (string.IsNullOrWhiteSpace(definition.Key))
        {
            throw new ArgumentException("Key cannot be empty", nameof(definition));
        }

        lock (_definitions)
        {
            var existing = _definitions.FirstOrDefault(d => d.Key.Equals(definition.Key, StringComparison.OrdinalIgnoreCase));
            if (existing != null)
            {
                existing.LabelDisplay = definition.LabelDisplay;
                existing.Description = definition.Description;
                existing.InputType = definition.InputType;
                existing.Options = definition.Options ?? new List<string>();
                existing.Enabled = definition.Enabled;
            }
            else
            {
                _definitions.Add(definition);
            }
        }

        await Task.Run(() => SaveData(_definitions));
        return definition;
    }

    public async Task DeleteAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key)) return;

        lock (_definitions)
        {
            var existing = _definitions.FirstOrDefault(d => d.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
            if (existing != null)
            {
                _definitions.Remove(existing);
            }
        }

        await Task.Run(() => SaveData(_definitions));
    }
}
