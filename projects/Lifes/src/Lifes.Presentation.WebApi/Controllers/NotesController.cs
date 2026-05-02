using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Lifes.Core.Interfaces;
using Lifes.Application.Features.LaputaNotes.Strategies;
using Lifes.Core.Models;
using System.Collections.Generic;

namespace Lifes.Presentation.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotesController : ControllerBase
{
    private readonly INoteRepository _repository;
    private readonly NoteQueryStrategyFactory _strategyFactory;

    public NotesController(INoteRepository repository, NoteQueryStrategyFactory strategyFactory)
    {
        _repository = repository;
        _strategyFactory = strategyFactory;
    }

    [HttpGet]
    public async Task<IActionResult> GetNotes([FromQuery] NoteQueryModel query)
    {
        var strategy = _strategyFactory.GetStrategy(query.QueryType);
        var result = await strategy.ExecuteAsync(query);
        
        if (result.IsSuccess)
            return Ok(new { success = true, data = result.Value });
            
        return BadRequest(new { success = false, error = result.Error });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _repository.GetByIdAsync(id);
        if (result.IsSuccess)
            return Ok(new { success = true, data = result.Value });
            
        return NotFound(new { success = false, error = result.Error });
    }

    [HttpPost]
    public async Task<IActionResult> Save([FromBody] Note note)
    {
        var result = await _repository.SaveAsync(note);
        if (result.IsSuccess)
            return Ok(new { success = true, data = result.Value });
            
        return BadRequest(new { success = false, error = result.Error });
    }

    [HttpPost("{id}/duplicate")]
    public async Task<IActionResult> Duplicate(string id)
    {
        var result = await _repository.GetByIdAsync(id);
        if (!result.IsSuccess)
            return NotFound(new { success = false, error = result.Error });

        var note = result.Value!;
        var duplicate = new Note
        {
            Id = $"{note.Id}_copy_{DateTime.Now:yyyyMMddHHmmss}",
            Title = $"{note.Title} (Copy)",
            Content = note.Content,
            Tags = new List<string>(note.Tags),
            Section = note.Section,
            IsStarred = note.IsStarred
        };

        var saveResult = await _repository.SaveAsync(duplicate);
        if (saveResult.IsSuccess)
            return Ok(new { success = true, data = saveResult.Value });

        return BadRequest(new { success = false, error = saveResult.Error });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _repository.DeleteAsync(id);
        if (result.IsSuccess)
            return Ok(new { success = true });
            
        return BadRequest(new { success = false, error = result.Error });
    }
}
