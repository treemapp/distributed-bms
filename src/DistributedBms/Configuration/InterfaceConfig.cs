namespace DistributedBms.Configuration;

public class InterfaceConfig
{
    public string Name { get; set; } = "";
    public string? IpAddress { get; set; }
    public string Driver { get; set; } = "";
    public int ScanIntervalMs { get; set; }

}