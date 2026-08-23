namespace DistributedBms.Drivers;

public class DriverFactory
{
    public IPlcDriver Create(string driverName)
    {
        return driverName.ToLowerInvariant() switch
        {
            "simulator" => new SimulatorDriver(),

            _ => throw new InvalidOperationException(
                $"Unknown driver: {driverName}"
            )
        };
    }
}