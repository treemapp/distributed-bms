namespace DistributedBms.Configuration;

public class InterfaceConfig
{
    public int Version { get; set; }
    public string Name { get; set; } = "";
    public string Driver { get; set; } = "";
    public int ScanIntervalMs { get; set; }
    public List<SourceConfig> Sources { get; set; } = new();
}
