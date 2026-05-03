using Lifes.Core.Interfaces;
using Lifes.Core.Models;
using Lifes.Presentation.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lifes.Presentation.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoodController : ControllerBase
{
    private readonly IMoodEntryRepository _repository;

    public MoodController(IMoodEntryRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<MoodEntry>>>> GetAll()
    {
        var entries = await _repository.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<MoodEntry>>.Ok(entries));
    }

    [HttpGet("range")]
    public async Task<ActionResult<ApiResponse<IEnumerable<MoodEntry>>>> GetByRange([FromQuery] DateTime start, [FromQuery] DateTime end)
    {
        var entries = await _repository.GetByRangeAsync(start, end);
        return Ok(ApiResponse<IEnumerable<MoodEntry>>.Ok(entries));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<MoodEntry>>> Save([FromBody] MoodEntry entry)
    {
        var result = await _repository.AddOrUpdateAsync(entry);
        return Ok(ApiResponse<MoodEntry>.Ok(result));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(string id)
    {
        await _repository.DeleteAsync(id);
        return Ok(ApiResponse<bool>.Ok(true));
    }
}
