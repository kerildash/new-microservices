using Microsoft.EntityFrameworkCore;
using PlatformService.Data;
using PlatformService.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();
builder.Services.AddDbContext<PlatformDbContext>(options => options.UseInMemoryDatabase("InMemory"));
builder.Services.AddScoped<IRepository<Platform>, PlatformRepository>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
    Prep.Populate(app);
}

app.UseHttpsRedirection();
