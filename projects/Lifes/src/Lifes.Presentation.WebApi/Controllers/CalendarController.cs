using Lifes.Core.Interfaces;
using Lifes.Core.Models;
using Lifes.Presentation.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lifes.Presentation.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CalendarController : ControllerBase
{
    private readonly ICalendarService _svc;
    private readonly ILogger<CalendarController> _logger;

    public CalendarController(ICalendarService svc, ILogger<CalendarController> logger)
    {
        _svc = svc;
        _logger = logger;
    }

    [HttpGet("mementos")]
    public async Task<ActionResult<ApiResponse<IEnumerable<MementoModel>>>> GetMementos(
        [FromQuery] int year,
        [FromQuery] int? month,
        [FromQuery] string? tagIds,
        [FromQuery] bool parentOnly = false,
        [FromQuery] bool includeChildren = false)
    {
        try
        {
            var query = new MementoQueryModel
            {
                StartDate = new DateTime(year, month ?? 1, 1),
                EndDate = month.HasValue 
                    ? new DateTime(year, month.Value, DateTime.DaysInMonth(year, month.Value)) 
                    : new DateTime(year, 12, 31),
                TagIds = string.IsNullOrEmpty(tagIds) 
                    ? null 
                    : tagIds.Split(',').Select(int.Parse).ToList(),
                ParentOnly = parentOnly
            };
            var data = await _svc.GetMementosAsync(query, includeChildren);
            return Ok(ApiResponse<IEnumerable<MementoModel>>.Ok(data));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting mementos");
            return StatusCode(500, ApiResponse<IEnumerable<MementoModel>>.Fail(ex.Message));
        }
    }

    [HttpGet("tags")]
    public async Task<ActionResult<ApiResponse<IEnumerable<TagModel>>>> GetTags()
    {
        try
        {
            var data = await _svc.GetTagsAsync();
            return Ok(ApiResponse<IEnumerable<TagModel>>.Ok(data));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tags");
            return StatusCode(500, ApiResponse<IEnumerable<TagModel>>.Fail(ex.Message));
        }
    }

    [HttpPost("mementos")]
    public async Task<ActionResult<ApiResponse<MementoModel>>> SaveMemento([FromBody] MementoModel m)
    {
        try
        {
            var saved = await _svc.SaveMementoAsync(m);
            return Ok(ApiResponse<MementoModel>.Ok(saved));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving memento");
            return StatusCode(500, ApiResponse<MementoModel>.Fail(ex.Message));
        }
    }

    [HttpDelete("mementos/{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteMemento(int id)
    {
        try
        {
            await _svc.DeleteMementoAsync(id);
            return StatusCode(204, ApiResponse<object>.Ok(null!));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting memento {Id}", id);
            return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpPost("tags")]
    public async Task<ActionResult<ApiResponse<TagModel>>> SaveTag([FromBody] TagModel t)
    {
        try
        {
            var saved = await _svc.SaveTagAsync(t);
            return Ok(ApiResponse<TagModel>.Ok(saved));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving tag");
            return StatusCode(500, ApiResponse<TagModel>.Fail(ex.Message));
        }
    }

    [HttpDelete("tags/{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteTag(int id)
    {
        try
        {
            await _svc.DeleteTagAsync(id);
            return StatusCode(204, ApiResponse<object>.Ok(null!));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tag {Id}", id);
            return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
        }
    }
}
