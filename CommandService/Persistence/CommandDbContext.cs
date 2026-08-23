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

        modelBuilder.Entity<Command>(e =>
        {
            e.Property(c => c.Line).IsRequired().HasMaxLength(250);
            e.Property(c => c.Description).IsRequired().HasMaxLength(250);
            e.HasIndex(c => c.PlatformId);
        });
        
        base.OnModelCreating(modelBuilder);
    }
}