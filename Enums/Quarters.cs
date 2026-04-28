namespace SharpAstrology.Enums;

public enum Quarters
{
    Initiation = 1,
    Civilization = 2,
    Duality = 3,
    Mutation = 4
}

public static class QuartersExtensionMethods
{
    public static int ToNumber(this Quarters quarter) => (int)quarter;
}