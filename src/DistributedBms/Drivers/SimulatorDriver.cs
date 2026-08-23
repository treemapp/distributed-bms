namespace DistributedBms.Drivers;

public class SimulatorDriver : IPlcDriver
{
    public Task<object> ReadAsync()
    {
        var result = new
        {
            Temperature = 21.5,
            Humidity = 45.0,
            SetPoint = 21.0
        };

        return Task.FromResult<object>(result);
    }
}