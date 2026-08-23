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

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

app.Run();