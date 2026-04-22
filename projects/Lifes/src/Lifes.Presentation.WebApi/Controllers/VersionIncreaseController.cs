using Lifes.Application.Features.VersionIncrease.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Lifes.Presentation.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VersionIncreaseController : ControllerBase
{
    private readonly IScanProjectsCommand _scanProjectsCommand;

    public VersionIncreaseController(IScanProjectsCommand scanProjectsCommand)
    {
        _scanProjectsCommand = scanProjectsCommand;
    }

    [HttpGet("scan")]
    public async Task<IActionResult> ScanProjects([FromQuery] string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return BadRequest("Path is required.");

        var result = await _scanProjectsCommand.ExecuteAsync(path);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }
}
