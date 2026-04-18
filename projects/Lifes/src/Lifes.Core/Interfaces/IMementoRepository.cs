using Lifes.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lifes.Core.Interfaces;

public interface IMementoRepository
{
    Task<IEnumerable<MementoModel>> GetByQueryAsync(MementoQueryModel query);
    Task<IEnumerable<MementoModel>> GetChildrenAsync(IEnumerable<int> parentIds);
    Task SaveAsync(MementoModel memento);
    Task DeleteAsync(int id);
}
