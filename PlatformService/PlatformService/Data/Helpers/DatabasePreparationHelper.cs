using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data.Helpers;

public class DatabasePreparationHelper(PlatformDbContext context)
{
    public async Task Migrate() => await context.Database.MigrateAsync();

    public async Task Populate()
    {
        if (await context.Platforms.AnyAsync())
            return;

        await context.Platforms.AddRangeAsync(
            new Platform() { Name = ".NET", Publisher = "Microsoft" },
            new Platform() { Name = "Kubernetes", Publisher = "Cloud Native" }
        );

        await context.SaveChangesAsync();
    }
}