using SharpAstrology.Enums;

namespace SharpAstrology.DataModels;

public sealed class Variables
{
    public Variable Digestion { get; init; } = default!;
    public Variable Perspective { get; init; }= default!;
    public Variable Environment { get; init; }= default!;
    public Variable Awareness { get; init; }= default!;
}

public sealed class Variable
{
    public Orientation Orientation { get; init; }
    public Color Color { get; init; }
    public Tone Tone{ get; init; }
    public Base Base { get; init; }
} 