namespace SharpAstrology.Enums;

public enum Color
{
    One = 1,
    Two = 2,
    Three = 3,
    Four = 4,
    Five = 5,
    Six = 6
}

public static class ColorExtensionMethods
{
    public static int ToNumber(this Color color) => color switch
    {
        Color.One => 1,
        Color.Two => 2,
        Color.Three => 3,
        Color.Four => 4,
        Color.Five => 5,
        Color.Six => 6,
        _ => throw new NotImplementedException($"{color} not implemented for extension method ToNumber()")
    };
}