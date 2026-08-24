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

        lock (_lock)
        {
            if (!_sessions.TryGetValue(config.Name, out var session))
            {
                session = StartSession(config);
                _sessions[config.Name] = session;
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

    private PollingSession StartSession(InterfaceConfig config)
    {
        var driver = _driverFactory.Create(config.Driver);

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
            session.Config.ScanIntervalMs
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
                        $"Polling error for {session.Config.Name}: {ex.Message}"
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
                _sessions.Remove(session.Config.Name);
            }
        }
    }

    internal class PollingSession
    {
        public InterfaceConfig Config { get; }
        public IPlcDriver Driver { get; }

        public object? LastResult { get; set; }

        public CancellationTokenSource CancellationTokenSource { get; }
            = new();

        public Task? Task { get; set; }

        public List<Channel<object>> Clients { get; }
            = new();

        public PollingSession(
            InterfaceConfig config,
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