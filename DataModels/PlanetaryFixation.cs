using SharpAstrology.Enums;

namespace SharpAstrology.DataModels;

public sealed class PlanetaryFixation
{
    public FixingState State { get; set; }
    public bool FixingStateChangedByComparer { get; set; }
}