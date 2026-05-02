using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lifes.Core.Interfaces;
using Lifes.Core.Models;

namespace Lifes.Application.Features.LaputaNotes.Strategies;

public class InboxQueryStrategy : INoteQueryStrategy
{
    private readonly INoteRepository _repository;
    public InboxQueryStrategy(INoteRepository repository) => _repository = repository;

    public async Task<Result<IEnumerable<Note>>> ExecuteAsync(NoteQueryModel query)
    {
        query.Section = "Inbox";
        return await _repository.GetNotesAsync(query);
    }
}

public class AllNotesQueryStrategy : INoteQueryStrategy
{
    private readonly INoteRepository _repository;
    public AllNotesQueryStrategy(INoteRepository repository) => _repository = repository;

    public async Task<Result<IEnumerable<Note>>> ExecuteAsync(NoteQueryModel query)
    {
        query.Section = null;
        return await _repository.GetNotesAsync(query);
    }
}

public class CategoryQueryStrategy : INoteQueryStrategy
{
    private readonly INoteRepository _repository;
    public CategoryQueryStrategy(INoteRepository repository) => _repository = repository;

    public async Task<Result<IEnumerable<Note>>> ExecuteAsync(NoteQueryModel query)
    {
        return await _repository.GetNotesAsync(query);
    }
}

public class NullQueryStrategy : INoteQueryStrategy
{
    private readonly INoteRepository _repository;
    public NullQueryStrategy(INoteRepository repository) => _repository = repository;

    public async Task<Result<IEnumerable<Note>>> ExecuteAsync(NoteQueryModel query)
    {
        return await _repository.GetNotesAsync(query);
    }
}
