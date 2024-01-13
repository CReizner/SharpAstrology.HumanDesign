#pragma warning disable CS8524

namespace SharpAstrology.Enums;

public enum SplitDefinitions
{
    Empty,
    SingleDefinition,
    SplitDefinition,
    TripleSplit,
    QuadrupleSplit
}
public static class HdSplitDefinitionExtensionMethods
{
    public static int GetNumberOfComponent(this SplitDefinitions splitDefinition) => splitDefinition switch
    {
        SplitDefinitions.Empty => 0,
        SplitDefinitions.SingleDefinition => 1,
        SplitDefinitions.SplitDefinition => 2,
        SplitDefinitions.TripleSplit => 3,
        SplitDefinitions.QuadrupleSplit => 4
    };

    public static string GetAsString(this SplitDefinitions splitDefinition) => splitDefinition switch
    {
        SplitDefinitions.Empty => "Empty",
        SplitDefinitions.SingleDefinition => "Single",
        SplitDefinitions.SplitDefinition => "Split",
        SplitDefinitions.TripleSplit => "Triple Split",
        SplitDefinitions.QuadrupleSplit => "Quadruple Split",
    };
}