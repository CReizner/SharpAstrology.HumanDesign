using SharpAstrology.Enums;

namespace SharpAstrology.DataModels;

public sealed class Variables
{
    public (Orientation Orientation, Color Color, Tone Tone, Base Base) Digestion { get; set; }
    public (Orientation Orientation, Color Color, Tone Tone, Base Base) Perspective { get; set; }
    public (Orientation Orientation, Color Color, Tone Tone, Base Base) Environment { get; set; }
    public (Orientation Orientation, Color Color, Tone Tone, Base Base) Awareness { get; set; }
}