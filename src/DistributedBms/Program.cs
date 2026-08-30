using Microsoft.Extensions.FileProviders;
using DistributedBms.Api;
using DistributedBms.Configuration;
using DistributedBms.Drivers;
using DistributedBms.Polling;

var builder = WebApplication.CreateBuilder(args);

// Configuration
builder.Services.AddSingleton<ConfigurationLoader>();

// Drivers
builder.Services.AddSingleton<DriverFactory>();

// Core services
builder.Services.AddSingleton<PollingService>();

// HTTP API
builder.Services.AddControllers();

var app = builder.Build();

var staticPath = Path.GetFullPath(
    Path.Combine(
        builder.Environment.ContentRootPath,
        "..",
        "..",
        "static"
    )
);

app.UseDefaultFiles(new DefaultFilesOptions
{
    FileProvider = new PhysicalFileProvider(staticPath)
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(staticPath)
});

app.MapControllers();

app.Run();