using System.Collections.Generic;
using System.Threading.Tasks;
using Lifes.Core.Interfaces;
using Lifes.Core.Models;

namespace Lifes.Application.Features.LaputaNotes.Strategies;

public interface INoteQueryStrategy
{
    Task<Result<IEnumerable<Note>>> ExecuteAsync(NoteQueryModel query);
}
