using SharpAstrology.Enums;

namespace SharpAstrology.DataModels;

public sealed class Variables
{
    public Variable Digestion { get; set; }
    public Variable Perspective { get; set; }
    public Variable Environment { get; set; }
    public Variable Awareness { get; set; }
}

public sealed class Variable
{
    public Orientation Orientation { get; set; }
    public Color Color { get; set; }
    public Tone Tone{ get; set; }
    public Base Base { get; set; }
} 