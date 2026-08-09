using Microsoft.EntityFrameworkCore;
using PlatformService.Data;
using PlatformService.Data.Helpers;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();
builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PlatformDbContext>(options => options.UseNpgsql(BuildConnectionString()));
builder.Services.AddScoped<IRepository<Platform>, PlatformRepository>();
builder.Services.AddScoped<DatabasePreparationHelper>();

builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();

var app = builder.Build();

await PrepareDb(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.UseHttpsRedirection();

app.Run();
return;

static string BuildConnectionString()
{
    var host = Environment.GetEnvironmentVariable("POSTGRES_HOST");
    var databaseName = Environment.GetEnvironmentVariable("POSTGRES_DB");
    var userName = Environment.GetEnvironmentVariable("POSTGRES_USER");
    var password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");

    var connectionString = $"Host={host};Port=5432;Database={databaseName};Username={userName};Password={password}";
    Console.WriteLine($"Connection string: {connectionString}");
    return connectionString;
}

static async Task PrepareDb(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var dbPreparation = scope.ServiceProvider.GetRequiredService<DatabasePreparationHelper>();
    await dbPreparation.Migrate();
    await dbPreparation.Populate();
}