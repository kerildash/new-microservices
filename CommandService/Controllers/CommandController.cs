using CommandService.Dto;
using CommandService.Mapping;
using CommandService.Models;
using CommandService.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CommandService.Controllers;

[Route("api/c/platform/{platformId:int}/[controller]")]
[ApiController]
public class CommandController(CommandDbContext context, ILogger<CommandController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CommandResponseDto>>> GetAllByPlatform([FromRoute] int platformId)
    {
        if (!await context.Platforms.AnyAsync(p => p.Id == platformId))
        {
            logger.LogInformation(
                "Could not return commands for platform. No platform found with ID {ID}.", platformId);

            return NotFound();
        }

        var commands = await context
            .Commands
            .Where(c => c.PlatformId == platformId)
            .Select(c => c.ToResponseDto())
            .ToListAsync();

        logger.LogInformation(
            "Returning {Commands} commands for platform with ID {ID}.", commands.Count, platformId);

        return Ok(commands);
    }

    [HttpGet("{commandId:int}")]
    public async Task<ActionResult<IEnumerable<CommandResponseDto>>> Get(
        [FromRoute] int platformId,
        [FromRoute] int commandId)
    {
        var command = await context
            .Commands
            .FirstOrDefaultAsync(c => c.PlatformId == platformId && c.Id == commandId);

        if (command is not null)
        {
            logger.LogInformation("Returning command with Id {ID} and PlatformId {PlatformID}.",
                commandId,
                platformId);

            return Ok(command.ToResponseDto());
        }

        if (!await context.Platforms.AnyAsync(p => p.Id == platformId))
        {
            logger.LogInformation("Could not return command. There is no platform with ID {ID}.", platformId);
            return NotFound();
        }

        logger.LogInformation("Could not return command with Id {ID} and PlatformId {PlatformID}.",
            commandId,
            platformId);

        return NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<CommandResponseDto>> Create(
        [FromRoute] int platformId,
        [FromBody] CommandCreateDto commandCreateDto)
    {
        if (!await context.Platforms.AnyAsync(p => p.Id == platformId))
        {
            logger.LogInformation(
                "Could not create command for platform with ID {ID}. No such platform exists.",
                platformId);

            return NotFound();
        }

        var command = Command.FromCreateDto(commandCreateDto);
        command.PlatformId = platformId;

        await context.Commands.AddAsync(command);
        await context.SaveChangesAsync();

        logger.LogInformation("New command created.");
        return CreatedAtAction(
            nameof(Get),
            new { platformId, commandId = command.Id },
            command.ToResponseDto());
    }
}