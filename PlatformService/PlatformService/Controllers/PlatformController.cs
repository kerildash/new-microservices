using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dto;
using PlatformService.Extensions;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlatformController(
    IRepository<Platform> repository,
    ICommandDataClient commandClient,
    ILogger<PlatformController> logger) : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<PlatformResponseDto>> GetAll()
    {
        var platforms = repository.GetAll().Select(p => p.ToResponseDto());
        logger.LogInformation("Returning {Count} platforms.", platforms.Count());
        return Ok(platforms);
    }

    [HttpGet("{id:int}")]
    public ActionResult<PlatformResponseDto> Get(int id)
    {
        var platform = repository.GetById(id)?.ToResponseDto();
        if (platform is null)
        {
            logger.LogInformation("No platform found with id {Id}.", id);
            return NotFound(id);
        }

        logger.LogInformation("Returning platform with id {Id}.", id);
        return Ok(platform);
    }

    [HttpPost]
    public async Task<ActionResult<PlatformResponseDto>> Create([FromBody] PlatformCreateDto dto)
    {
        var platform = Platform.ToPlatform(dto);
        repository.Add(platform);
        repository.SaveChanges();

        var response = platform.ToResponseDto();

        try
        {
            await commandClient.SendPlatformToCommandService(response);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while sending the platform.");
        }

        logger.LogInformation("New platform created with id {Id}.", platform.Id);
        return CreatedAtAction(nameof(Get), new { id = platform.Id }, response);
    }
}