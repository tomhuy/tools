using Lifes.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lifes.Core.Interfaces;

public interface IMoodMetadataRepository
{
    Task<IEnumerable<MoodMetadataDefinition>> GetAllAsync();
    Task<MoodMetadataDefinition> AddOrUpdateAsync(MoodMetadataDefinition definition);
    Task DeleteAsync(string key);
}
