using Microsoft.EntityFrameworkCore;
using PlatformService.Data;
using PlatformService.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PlatformDbContext>(options => options.UseInMemoryDatabase("InMemory"));
builder.Services.AddScoped<IRepository<Platform>, PlatformRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    Console.WriteLine("dev environment");
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
    Prep.Populate(app);
}

app.MapControllers();

app.UseHttpsRedirection();

app.Run();