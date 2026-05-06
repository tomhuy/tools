using Lifes.Core.Interfaces;
using Lifes.Core.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Lifes.Infrastructure.Features.Users.Repositories;

public class JsonUserRepository : IUserRepository
{
    private readonly ILogger<JsonUserRepository> _logger;
    private readonly string _filePath;
    private readonly List<User> _users;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public JsonUserRepository(ILogger<JsonUserRepository> logger)
    {
        _logger = logger;
        
        var appDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var databaseDir = Path.Combine(appDirectory, "database");
        if (!Directory.Exists(databaseDir))
        {
            Directory.CreateDirectory(databaseDir);
        }
        
        _filePath = Path.Combine(databaseDir, "users.json");
        _users = LoadData();

        // Seed with default user if empty
        if (!_users.Any())
        {
            _users.Add(new User { Id = "u1", Name = "Huy", Initials = "HY", Color = "blue" });
            SaveData(_users);
        }
    }

    private List<User> LoadData()
    {
        try
        {
            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    return JsonSerializer.Deserialize<List<User>>(json, _jsonOptions) ?? new List<User>();
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load users from {FilePath}", _filePath);
        }

        return new List<User>();
    }

    private void SaveData(List<User> data)
    {
        try
        {
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            File.WriteAllText(_filePath, json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save users to {FilePath}", _filePath);
        }
    }

    public Task<Result<IEnumerable<User>>> GetAllAsync()
    {
        return Task.FromResult(Result<IEnumerable<User>>.Success(_users.AsEnumerable()));
    }

    public Task<Result<User>> GetByIdAsync(string id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        if (user == null) return Task.FromResult(Result<User>.Failure($"User with ID {id} not found."));
        return Task.FromResult(Result<User>.Success(user));
    }

    public async Task<Result<User>> SaveAsync(User user)
    {
        var existing = _users.FirstOrDefault(u => u.Id == user.Id);
        if (existing != null)
        {
            existing.Name = user.Name;
            existing.Initials = user.Initials;
            existing.Color = user.Color;
        }
        else
        {
            _users.Add(user);
        }

        await Task.Run(() => SaveData(_users));
        return Result<User>.Success(user);
    }

    public async Task<Result> DeleteAsync(string id)
    {
        var existing = _users.FirstOrDefault(u => u.Id == id);
        if (existing != null)
        {
            _users.Remove(existing);
            await Task.Run(() => SaveData(_users));
            return Result.Success();
        }
        return Result.Failure($"User with ID {id} not found.");
    }

    public async Task<Result> SaveAllAsync(IEnumerable<User> users)
    {
        _users.Clear();
        _users.AddRange(users);
        await Task.Run(() => SaveData(_users));
        return Result.Success();
    }
}
