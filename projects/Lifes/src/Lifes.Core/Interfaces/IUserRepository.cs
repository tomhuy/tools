using System.Collections.Generic;
using System.Threading.Tasks;
using Lifes.Core.Models;

namespace Lifes.Core.Interfaces;

public interface IUserRepository
{
    Task<Result<IEnumerable<User>>> GetAllAsync();
    Task<Result<User>> GetByIdAsync(string id);
    Task<Result<User>> SaveAsync(User user);
    Task<Result> DeleteAsync(string id);
    Task<Result> SaveAllAsync(IEnumerable<User> users);
}
