using System.Text.Json;
using CommandService.Dto;
using CommandService.Mapping;
using CommandService.Models;
using CommandService.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CommandService.Integration;

public class MessageHandler(IServiceScopeFactory scopeFactory, ILogger<MessageHandler> logger)
{
    public async Task ProcessMessage(string message, string? messageType)
    {
        if (messageType is "PlatformCreated")
        {
            await AddPlatform(message);
            return;
        }

        logger.LogInformation("Incoming message wasn't processed. Unrecognized message type: {Type}", messageType);
    }

    private async Task AddPlatform(string message)
    {
        var dto = JsonSerializer.Deserialize<PlatformReceivedDto>(message);
        if (dto is null)
        {
            logger.LogInformation("Could not deserialize a platform from message: \"{Message}\".", message);
            throw new InvalidOperationException("Could not deserialize a platform from message.");
        }

        await using var context = scopeFactory.CreateScope().ServiceProvider.GetRequiredService<CommandDbContext>();
        if (await context.Platforms.AnyAsync(p => p.Id == dto.Id))
        {
            logger.LogInformation("Cannot add platform with ID {ID}. Platform with that id already exists.", dto.Id);
            throw new InvalidOperationException("Could not deserialize a platform from message.");
        }

        await context.Platforms.AddAsync(Platform.FromReceivedDto(dto));
        await context.SaveChangesAsync();
        logger.LogInformation("Platform {ID} was successfully added.", dto.Id);
    }
}