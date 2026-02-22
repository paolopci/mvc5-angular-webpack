using WebCore9.Core;
using WebCore9.Infrastructure;
using WebCore9.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiServices();
builder.Services.AddCoreServices();
builder.Services.AddInfrastructureServices();

var app = builder.Build();

app.UseHttpsRedirection();
app.MapApiEndpoints();

app.Run();
