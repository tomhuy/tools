using System.Collections.Generic;
using System.Threading.Tasks;
using Lifes.Core.Models;

namespace Lifes.Core.Interfaces;

public interface ISprintBoardRepository
{
    Task<Result<IEnumerable<Epic>>> GetEpicsAsync();
    Task<Result> SaveEpicsAsync(IEnumerable<Epic> epics);
}
