using CommandService.Integration;
using CommandService.Persistence;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace CommandService;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Host.UseSerilog((context, configuration) =>
            configuration.ReadFrom.Configuration(context.Configuration));

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddDbContext<CommandDbContext>(options => options.UseNpgsql(BuildConnectionString()));
        builder.Services.AddSingleton<MessageHandler>();
        builder.Services.AddHostedService<MessageBusSubscriber>();

        builder.Services.AddAuthorization();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var app = builder.Build();

        MigrateDatabase(app);
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }

    private static string BuildConnectionString()
    {
        var host = Environment.GetEnvironmentVariable("POSTGRES_HOST");
        var databaseName = Environment.GetEnvironmentVariable("POSTGRES_DB");
        var userName = Environment.GetEnvironmentVariable("POSTGRES_USER");
        var password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");

        var connectionString =
            $"Host={host};Port=5432;Database={databaseName};Username={userName};Password={password}";

        return connectionString;
    }

    private static void MigrateDatabase(WebApplication app)
    {
        var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CommandDbContext>();
        dbContext.Database.Migrate();
    }
}