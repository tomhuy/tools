using System.Collections.Generic;
using System.Threading.Tasks;
using Lifes.Core.Models;

namespace Lifes.Core.Interfaces;

public interface INoteRepository
{
    Task<Result<IEnumerable<Note>>> GetNotesAsync(NoteQueryModel query);
    Task<Result<Note>> GetByIdAsync(string id);
    Task<Result<Note>> SaveAsync(Note note);
    Task<Result> DeleteAsync(string id);
}

public class NoteQueryModel
{
    public string? QueryType { get; set; } // inbox, all, category
    public string? SearchQuery { get; set; }
    public string? Section { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; }
    public bool SortAsc { get; set; } = false;
}
