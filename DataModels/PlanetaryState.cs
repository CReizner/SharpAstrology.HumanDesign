using SharpAstrology.Enums;

namespace SharpAstrology.DataModels;

public sealed class PlanetaryState
{
    public Planets Planet { get; set; }
    public FixingState State { get; set; }
    public bool ChangedByComparer { get; set; }
}