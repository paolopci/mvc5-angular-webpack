using WebCore9.Core;
using WebCore9.Infrastructure;
using WebCore9.Api;

const string DevSpaCorsPolicy = "DevSpaClient";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(DevSpaCorsPolicy, policy =>
    {
        policy.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var defaultHeroesStoragePath = Path.Combine(builder.Environment.ContentRootPath, "App_Data", "heroes-store.json");
var configuredHeroesStoragePath = builder.Configuration["Heroes:StorageFilePath"];
var resolvedHeroesStoragePath = string.IsNullOrWhiteSpace(configuredHeroesStoragePath)
    ? defaultHeroesStoragePath
    : (Path.IsPathRooted(configuredHeroesStoragePath)
        ? configuredHeroesStoragePath
        : Path.Combine(builder.Environment.ContentRootPath, configuredHeroesStoragePath));

builder.Services.AddApiServices();
builder.Services.AddCoreServices();
builder.Services.AddInfrastructureServices(resolvedHeroesStoragePath);

var app = builder.Build();

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseCors(DevSpaCorsPolicy);
}

app.MapApiEndpoints();

app.Run();

public partial class Program
{
}
