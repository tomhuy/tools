using Lifes.Core.Interfaces;
using Lifes.Core.Models;
using Lifes.Presentation.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lifes.Presentation.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoodMetadataController : ControllerBase
{
    private readonly IMoodMetadataRepository _repository;

    public MoodMetadataController(IMoodMetadataRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<MoodMetadataDefinition>>>> GetAll()
    {
        var definitions = await _repository.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<MoodMetadataDefinition>>.Ok(definitions));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<MoodMetadataDefinition>>> Save([FromBody] MoodMetadataDefinition definition)
    {
        var result = await _repository.AddOrUpdateAsync(definition);
        return Ok(ApiResponse<MoodMetadataDefinition>.Ok(result));
    }

    [HttpDelete("{key}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(string key)
    {
        await _repository.DeleteAsync(key);
        return Ok(ApiResponse<bool>.Ok(true));
    }
}
