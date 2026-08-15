using CommandService.Models;
using Microsoft.EntityFrameworkCore;

namespace CommandService.Persistence;

public class CommandDbContext(DbContextOptions<CommandDbContext> options) : DbContext(options)
{
    public DbSet<Command> Commands { get; set; }
    public DbSet<Platform> Platforms { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Platform>()
            .HasMany(p => p.Commands)
            .WithOne(c => c.Platform)
            .HasForeignKey(c => c.PlatformId);

        base.OnModelCreating(modelBuilder);
    }
}