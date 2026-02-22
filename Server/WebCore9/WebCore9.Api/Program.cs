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

builder.Services.AddApiServices();
builder.Services.AddCoreServices();
builder.Services.AddInfrastructureServices();

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
