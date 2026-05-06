using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Lifes.Core.Interfaces;
using Lifes.Core.Models;
using System.Collections.Generic;

namespace Lifes.Presentation.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SprintBoardController : ControllerBase
{
    private readonly ISprintBoardRepository _repository;

    public SprintBoardController(ISprintBoardRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> GetBoard()
    {
        var result = await _repository.GetEpicsAsync();
        if (result.IsSuccess)
            return Ok(new { success = true, data = result.Value });
            
        return BadRequest(new { success = false, error = result.Error });
    }

    [HttpPost]
    public async Task<IActionResult> SaveBoard([FromBody] List<Epic> epics)
    {
        var result = await _repository.SaveEpicsAsync(epics);
        if (result.IsSuccess)
            return Ok(new { success = true });
            
        return BadRequest(new { success = false, error = result.Error });
    }
}
