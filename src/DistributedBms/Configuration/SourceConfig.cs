namespace DistributedBms.Configuration;

public class SourceConfig
{
    public string Id { get; set; } = "";
    public string? Description { get; set; }
    public bool Writable { get; set; }
}