using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dto;
using PlatformService.Extensions;
using PlatformService.Models;

namespace PlatformService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlatformController(IRepository<Platform> repository) : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<PlatformResponseDto>> GetAll()
    {
        var platforms = repository.GetAll().Select(p => p.ToResponseDto());
        return Ok(platforms);
    }

    [HttpGet("{id:int}")]
    public ActionResult<PlatformResponseDto> Get(int id)
    {
        var platform = repository.GetById(id)?.ToResponseDto();
        if (platform is null)
            return NotFound(id);

        return Ok(platform);
    }

    [HttpPost]
    public ActionResult<PlatformResponseDto> Create([FromBody] PlatformCreateDto dto)
    {
        var platform = Platform.ToPlatform(dto);
        repository.Add(platform);
        repository.SaveChanges();
        return CreatedAtAction(nameof(Get), new { id = platform.Id }, platform.ToResponseDto());
    }
}