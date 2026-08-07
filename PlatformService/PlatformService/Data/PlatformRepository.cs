using PlatformService.Models;

namespace PlatformService.Data;

public class PlatformRepository(PlatformDbContext context) : IRepository<Platform>
{
    public bool SaveChanges() => context.SaveChanges() >= 0;
    public IEnumerable<Platform> GetAll() => [.. context.Platforms];
    public Platform? GetById(int id) => context.Platforms.FirstOrDefault(p => p.Id == id);
    public void Add(Platform platform) => context.Platforms.Add(platform);
}