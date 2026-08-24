namespace DistributedBms.Drivers;

public class SimulatorDriver : IPlcDriver
{
    private readonly Dictionary<string, object?> _values = new();

    public SimulatorDriver(Dictionary<string, object?> config)
    {
        if (!config.TryGetValue("sources", out var sources))
        {
            throw new InvalidOperationException(
                "Simulator configuration is missing 'sources'"
            );
        }

        if (sources is not IEnumerable<object> sourceList)
        {
            throw new InvalidOperationException(
                "Simulator 'sources' must be a list"
            );
        }

        foreach (var source in sourceList)
        {
            if (source is not Dictionary<object, object?> sourceConfig)
            {
                throw new InvalidOperationException(
                    "Invalid simulator source configuration"
                );
            }

            if (!sourceConfig.TryGetValue("id", out var idValue) ||
                idValue is null)
            {
                throw new InvalidOperationException(
                    "Simulator source is missing 'id'"
                );
            }

            var id = idValue.ToString()!;

            sourceConfig.TryGetValue(
                "initial",
                out var initialValue
            );

            _values[id] = initialValue;
        }
    }

    public Task<object> ReadAsync()
    {
        return Task.FromResult<object>(
            new Dictionary<string, object?>(_values)
        );
    }
}