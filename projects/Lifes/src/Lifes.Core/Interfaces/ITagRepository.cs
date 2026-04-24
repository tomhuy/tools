using Lifes.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lifes.Core.Interfaces;

public interface ITagRepository
{
    Task<IEnumerable<TagModel>> GetAllAsync();
    Task<TagModel> SaveAsync(TagModel tag);
    Task DeleteAsync(int id);
}
