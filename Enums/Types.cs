namespace SharpAstrology.Enums;
public enum Types
{
    Manifestor,
    ManifestingGenerator,
    Generator,
    Projector,
    Reflector
}

public static class TypesExtensionMethods
{
    public static string GetAsString(this Types type) => type switch
    {
        Types.Generator => "Generator",
        Types.Manifestor => "Manifestor",
        Types.Projector => "Projector",
        Types.Reflector => "Reflector",
        Types.ManifestingGenerator => "Manifesting Generator"
    };
}