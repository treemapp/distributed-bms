namespace DistributedBms.BeckhoffInspector;

public record BeckhoffSymbol(
    string Name,
    string Type,
    string Comment
    /*uint IndexGroup,
    uint IndexOffset*/
);