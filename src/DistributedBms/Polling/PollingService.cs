using System.Threading.Channels;
using DistributedBms.Configuration;
using DistributedBms.Drivers;

namespace DistributedBms.Polling;

public class PollingService
{
    private readonly ConfigurationLoader _configurationLoader;
    private readonly DriverFactory _driverFactory;

    private readonly Dictionary<string, PollingSession> _sessions = new();
    private readonly object _lock = new();

    public PollingService(
        ConfigurationLoader configurationLoader,
        DriverFactory driverFactory)
    {
        _configurationLoader = configurationLoader;
        _driverFactory = driverFactory;
    }

    public Subscription Subscribe(string interfaceName)
    {
        var config = _configurationLoader.GetInterface(interfaceName);

        var name = GetRequiredString(config, "name");

        lock (_lock)
        {
            if (!_sessions.TryGetValue(name, out var session))
            {
                session = StartSession(config);
                _sessions[name] = session;
            }

            var channel = Channel.CreateUnbounded<object>();

            session.Clients.Add(channel);

            // Send the last result immediately, if one exists.
            if (session.LastResult != null)
                channel.Writer.TryWrite(session.LastResult);

            return new Subscription(
                this,
                session,
                channel
            );
        }
    }
	
	public async Task WriteAsync(
	    string interfaceName,
        string sourceId,
        object value)
    {
        var config = _configurationLoader.GetInterface(interfaceName);

        var name = GetRequiredString(config, "name");

        PollingSession session;

        lock (_lock)
        {
            if (!_sessions.TryGetValue(name, out session!))
            {
                throw new InvalidOperationException(
                    $"Interface '{interfaceName}' is not currently active"
                );
            }
        }

        await session.Driver.WriteAsync(
            sourceId,
            value
        );
    }

    private PollingSession StartSession(
        Dictionary<string, object?> config)
    {
        var driverName = GetRequiredString(config, "driver");

        var driver = _driverFactory.Create(
            driverName,
            config
        );

        var session = new PollingSession(
            config,
            driver
        );

        session.Task = Task.Run(
            () => PollLoopAsync(session)
        );

        return session;
    }

    private async Task PollLoopAsync(PollingSession session)
    {
        var interval = TimeSpan.FromMilliseconds(
            GetScanIntervalMs(session.Config)
        );

        var name = GetRequiredString(
            session.Config,
            "name"
        );

        try
        {
            while (!session.CancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    var result = await session.Driver.ReadAsync();

                    session.LastResult = result;

                    lock (_lock)
                    {
                        foreach (var client in session.Clients)
                        {
                            client.Writer.TryWrite(result);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        $"Polling error for {name}: {ex.Message}"
                    );
                }

                await Task.Delay(
                    interval,
                    session.CancellationTokenSource.Token
                );
            }
        }
        catch (OperationCanceledException)
        {
            // Normal shutdown when the last client disconnects.
        }
    }

    private void Unsubscribe(
        PollingSession session,
        Channel<object> channel)
    {
        lock (_lock)
        {
            session.Clients.Remove(channel);

            channel.Writer.TryComplete();

            if (session.Clients.Count == 0)
            {
                session.CancellationTokenSource.Cancel();

                var name = GetRequiredString(
                    session.Config,
                    "name"
                );

                _sessions.Remove(name);
            }
        }
    }

    private static string GetRequiredString(
        Dictionary<string, object?> config,
        string key)
    {
        return config[key]?.ToString()
            ?? throw new InvalidOperationException(
                $"Configuration is missing required field '{key}'"
            );
    }

    private static int GetScanIntervalMs(
        Dictionary<string, object?> config)
    {
        if (!config.TryGetValue(
                "scan-interval-ms",
                out var value) ||
            value == null)
        {
            throw new InvalidOperationException(
                "Configuration is missing required field 'scan-interval-ms'"
            );
        }

        return Convert.ToInt32(value);
    }

    internal class PollingSession
    {
        public Dictionary<string, object?> Config { get; }
        public IPlcDriver Driver { get; }

        public object? LastResult { get; set; }

        public CancellationTokenSource CancellationTokenSource { get; }
            = new();

        public Task? Task { get; set; }

        public List<Channel<object>> Clients { get; }
            = new();

        public PollingSession(
            Dictionary<string, object?> config,
            IPlcDriver driver)
        {
            Config = config;
            Driver = driver;
        }
    }

    public class Subscription : IDisposable
    {
        private readonly PollingService _service;
        private readonly PollingSession _session;

        public ChannelReader<object> Reader { get; }

        private readonly Channel<object> _channel;

        internal Subscription(
            PollingService service,
            PollingSession session,
            Channel<object> channel)
        {
            _service = service;
            _session = session;
            _channel = channel;
            Reader = channel.Reader;
        }

        public void Dispose()
        {
            _service.Unsubscribe(
                _session,
                _channel
            );
        }
    }
}