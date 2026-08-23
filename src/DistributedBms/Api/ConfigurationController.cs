using DistributedBms.Configuration;
using DistributedBms.Drivers;
using DistributedBms.Polling;
using Microsoft.AspNetCore.Mvc;

namespace DistributedBms.Api;

[ApiController]
[Route("api")]
public class ConfigController : ControllerBase
{
    private readonly ConfigurationLoader _configurationLoader;
    private readonly DriverFactory _driverFactory;
    private readonly PollingService _pollingService;

    public ConfigController(
        ConfigurationLoader configurationLoader,
        DriverFactory driverFactory,
        PollingService pollingService)
    {
        _configurationLoader = configurationLoader;
        _driverFactory = driverFactory;
        _pollingService = pollingService;
    }

    [HttpGet("interfaces/{name}")]
    public IActionResult GetInterface(string name)
    {
        try
        {
            return Ok(_configurationLoader.GetInterface(name));
        }
        catch (FileNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("systems/{name}")]
    public IActionResult GetSystem(string name)
    {
        try
        {
            return Ok(_configurationLoader.GetSystem(name));
        }
        catch (FileNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("interfaces/{name}/get-sources")]
    public async Task GetSources(string name, CancellationToken cancellationToken)
    {
        Response.ContentType = "text/event-stream";

        try
        {
            using var subscription = _pollingService.Subscribe(name);

            await foreach (var result in subscription.Reader.ReadAllAsync(cancellationToken))
            {
                var json = System.Text.Json.JsonSerializer.Serialize(result);

                await Response.WriteAsync(
                    $"data: {json}\n\n",
                    cancellationToken
                );

                await Response.Body.FlushAsync(cancellationToken);
            }
        }
        catch (FileNotFoundException)
        {
            Response.StatusCode = StatusCodes.Status404NotFound;
        }
        catch (OperationCanceledException)
        {
        // Client disconnected.
        }
    }
}