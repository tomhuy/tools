using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Lifes.Core.Interfaces;
using Lifes.Core.Models;

namespace Lifes.Infrastructure.Features.LaputaNotes.Repositories;

public class ObsidianNoteRepository : INoteRepository
{
    private readonly string _vaultPath = @"D:\Personal\ICloud\Huy Bui Local\Huy Bui Local\Inbox";

    public ObsidianNoteRepository()
    {
        if (!Directory.Exists(_vaultPath))
        {
            Directory.CreateDirectory(_vaultPath);
        }
    }

    public async Task<Result<IEnumerable<Note>>> GetNotesAsync(NoteQueryModel query)
    {
        try
        {
            var files = Directory.GetFiles(_vaultPath, "*.md");
            var notes = new List<Note>();

            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                var content = await File.ReadAllTextAsync(file);
                
                var note = new Note
                {
                    Id = Path.GetFileNameWithoutExtension(file),
                    Title = Path.GetFileNameWithoutExtension(file),
                    Content = content,
                    Created = fileInfo.CreationTime,
                    Modified = fileInfo.LastWriteTime,
                    Section = "Inbox" // Default for this path
                };
                
                // Basic tag parsing (looking for #tag in content)
                note.Tags = ParseTags(content);
                
                notes.Add(note);
            }

            // Apply basic filtering if needed (strategies will handle more complex logic)
            IEnumerable<Note> filteredNotes = notes;
            if (!string.IsNullOrEmpty(query.SearchQuery))
            {
                filteredNotes = filteredNotes.Where(n => 
                    n.Title.Contains(query.SearchQuery, StringComparison.OrdinalIgnoreCase) || 
                    n.Content.Contains(query.SearchQuery, StringComparison.OrdinalIgnoreCase));
            }

            // Sorting
            if (!string.IsNullOrEmpty(query.SortBy))
            {
                filteredNotes = query.SortBy.ToLower() switch
                {
                    "title" => query.SortAsc ? filteredNotes.OrderBy(n => n.Title) : filteredNotes.OrderByDescending(n => n.Title),
                    "modified" => query.SortAsc ? filteredNotes.OrderBy(n => n.Modified) : filteredNotes.OrderByDescending(n => n.Modified),
                    "created" => query.SortAsc ? filteredNotes.OrderBy(n => n.Created) : filteredNotes.OrderByDescending(n => n.Created),
                    _ => filteredNotes.OrderByDescending(n => n.Modified)
                };
            }
            else
            {
                filteredNotes = filteredNotes.OrderByDescending(n => n.Modified);
            }

            // Pagination
            var pagedNotes = filteredNotes
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToList();

            return Result<IEnumerable<Note>>.Success(pagedNotes);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<Note>>.Failure(ex.Message);
        }
    }

    public async Task<Result<Note>> GetByIdAsync(string id)
    {
        try
        {
            var filePath = Path.Combine(_vaultPath, $"{id}.md");
            if (!File.Exists(filePath))
            {
                return Result<Note>.Failure("Note not found");
            }

            var fileInfo = new FileInfo(filePath);
            var content = await File.ReadAllTextAsync(filePath);

            var note = new Note
            {
                Id = id,
                Title = id,
                Content = content,
                Created = fileInfo.CreationTime,
                Modified = fileInfo.LastWriteTime,
                Tags = ParseTags(content),
                Section = "Inbox"
            };

            return Result<Note>.Success(note);
        }
        catch (Exception ex)
        {
            return Result<Note>.Failure(ex.Message);
        }
    }

    public async Task<Result<Note>> SaveAsync(Note note)
    {
        try
        {
            var fileName = string.IsNullOrEmpty(note.Id) ? note.Title : note.Id;
            if (string.IsNullOrEmpty(fileName)) fileName = "Untitled";
            
            var filePath = Path.Combine(_vaultPath, $"{fileName}.md");
            
            // If ID changed (title change), handle rename?
            // For now, assume ID is fixed or Title is ID.
            
            await File.WriteAllTextAsync(filePath, note.Content);
            
            note.Id = fileName;
            note.Modified = DateTime.Now;
            
            return Result<Note>.Success(note);
        }
        catch (Exception ex)
        {
            return Result<Note>.Failure(ex.Message);
        }
    }

    public async Task<Result> DeleteAsync(string id)
    {
        try
        {
            var filePath = Path.Combine(_vaultPath, $"{id}.md");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            await Task.CompletedTask;
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }

    private List<string> ParseTags(string content)
    {
        // Simple regex or string splitting to find #tags
        // For now, just return empty list or basic implementation
        var tags = new List<string>();
        var words = content.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var word in words)
        {
            if (word.StartsWith("#") && word.Length > 1)
            {
                tags.Add(word.Substring(1).TrimEnd(',', '.', '!', '?', ';'));
            }
        }
        return tags.Distinct().ToList();
    }
}
