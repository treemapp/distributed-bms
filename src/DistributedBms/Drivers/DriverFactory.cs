namespace DistributedBms.Drivers;

public class DriverFactory
{
    public IPlcDriver Create(string driverName, Dictionary<string, object?> config)
    {
        return driverName.ToLowerInvariant() switch
        {
            "simulator" => new SimulatorDriver(config),
            "beckhoff-ads" => new BeckhoffDriver(config),

            _ => throw new InvalidOperationException(
                $"Unknown driver: {driverName}"
            )
        };
    }
}