using Lifes.Core.Interfaces;
using Lifes.Core.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Lifes.Infrastructure.Features.SprintBoard.Repositories;

public class JsonSprintBoardRepository : ISprintBoardRepository
{
    private readonly ILogger<JsonSprintBoardRepository> _logger;
    private readonly string _filePath;
    private readonly List<Epic> _epics;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public JsonSprintBoardRepository(ILogger<JsonSprintBoardRepository> logger)
    {
        _logger = logger;
        
        var appDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var databaseDir = Path.Combine(appDirectory, "database");
        if (!Directory.Exists(databaseDir))
        {
            Directory.CreateDirectory(databaseDir);
        }
        
        _filePath = Path.Combine(databaseDir, "sprint_board.json");
        _epics = LoadData();
    }

    private List<Epic> LoadData()
    {
        try
        {
            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    return JsonSerializer.Deserialize<List<Epic>>(json, _jsonOptions) ?? new List<Epic>();
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load sprint board from {FilePath}", _filePath);
        }

        return new List<Epic>();
    }

    private void SaveData(List<Epic> data)
    {
        try
        {
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            File.WriteAllText(_filePath, json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save sprint board to {FilePath}", _filePath);
        }
    }

    public Task<Result<IEnumerable<Epic>>> GetEpicsAsync()
    {
        return Task.FromResult(Result<IEnumerable<Epic>>.Success(_epics.AsEnumerable()));
    }

    public async Task<Result> SaveEpicsAsync(IEnumerable<Epic> epics)
    {
        _epics.Clear();
        _epics.AddRange(epics);
        await Task.Run(() => SaveData(_epics));
        return Result.Success();
    }
}
