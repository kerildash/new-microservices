using CommandService.Dto;
using CommandService.Mapping;
using CommandService.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CommandService.Controllers;

[ApiController]
[Route("api/c/[controller]")]
public class PlatformController(CommandDbContext context, ILogger<PlatformController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlatformResponseDto>>> GetAll()
    {
        var platforms =
            await context.Platforms.Select(p => p.ToResponseDto()).ToListAsync();

        logger.LogInformation("Returning {Count} platforms", platforms.Count);
        return Ok(platforms);
    }
}