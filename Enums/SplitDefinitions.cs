#pragma warning disable CS8524

namespace SharpAstrology.Enums;

public enum SplitDefinitions
{
    Empty = 0,
    SingleDefinition = 1,
    SplitDefinition = 2,
    TripleSplit = 3,
    QuadrupleSplit = 4
}
public static class HdSplitDefinitionExtensionMethods
{
    public static int ToNumberOfComponent(this SplitDefinitions splitDefinition) => (int)splitDefinition;

    public static string ToText(this SplitDefinitions splitDefinition) => splitDefinition switch
    {
        SplitDefinitions.Empty => "Empty",
        SplitDefinitions.SingleDefinition => "Single",
        SplitDefinitions.SplitDefinition => "Split",
        SplitDefinitions.TripleSplit => "Triple Split",
        SplitDefinitions.QuadrupleSplit => "Quadruple Split",
    };
}