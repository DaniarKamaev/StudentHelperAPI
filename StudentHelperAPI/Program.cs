using Microsoft.EntityFrameworkCore;
using StudentHelperAPI.Core.Abstractions;
using StudentHelperAPI.Features.AI.Send;
using StudentHelperAPI.Infrastructure.Services;
using StudentHelperAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();

// Логирование
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

// Database
builder.Services.AddDbContext<HelperDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 0))
    ));

// MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// AI Service - GigaChat
builder.Services.AddScoped<IAiService, GigaChatService>();
builder.Services.Configure<GigaChatSettings>(
    builder.Configuration.GetSection("GigaChat"));

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Регистрируем endpoints
app.SendMessageMap();

// Health check endpoint
app.MapGet("/", () => "Student Helper API with GigaChat is running!");
app.MapGet("/health", () => "Healthy");

app.Run();