namespace DistributedBms.Drivers;

public interface IPlcDriver
{
    Task<object> ReadAsync();//    Task<IReadOnlyList<string>> GetSourcesAsync();

    Task WriteAsync(
        string sourceId,
        object value);
}