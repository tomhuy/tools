using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Lifes.Core.Interfaces;
using Lifes.Core.Models;
using System.Collections.Generic;

namespace Lifes.Presentation.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _repository;

    public UsersController(IUserRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var result = await _repository.GetAllAsync();
        if (result.IsSuccess)
            return Ok(new { success = true, data = result.Value });
            
        return BadRequest(new { success = false, error = result.Error });
    }

    [HttpPost]
    public async Task<IActionResult> Save([FromBody] User user)
    {
        var result = await _repository.SaveAsync(user);
        if (result.IsSuccess)
            return Ok(new { success = true, data = result.Value });
            
        return BadRequest(new { success = false, error = result.Error });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _repository.DeleteAsync(id);
        if (result.IsSuccess)
            return Ok(new { success = true });
            
        return BadRequest(new { success = false, error = result.Error });
    }
    
    [HttpPost("bulk")]
    public async Task<IActionResult> SaveAll([FromBody] List<User> users)
    {
        var result = await _repository.SaveAllAsync(users);
        if (result.IsSuccess)
            return Ok(new { success = true });
            
        return BadRequest(new { success = false, error = result.Error });
    }
}
