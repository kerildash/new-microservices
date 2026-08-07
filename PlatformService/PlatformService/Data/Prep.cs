using PlatformService.Models;

namespace PlatformService.Data;

public static class Prep
{
    public static void Populate(IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<PlatformDbContext>();

        Seed(context);
    }

    private static void Seed(PlatformDbContext context)
    {
        if (context.Platforms.Any())
            return;

        context.Platforms.AddRange(
            new Platform() { Name = ".NET", Publisher = "Microsoft" },
            new Platform() { Name = "Kubernetes", Publisher = "Cloud Native" }
        );
        
        context.SaveChanges();
    }
}