namespace SharpAstrology.Enums;
public enum Lines
{
    One = 1,
    Two = 2,
    Three = 3,
    Four = 4,
    Five = 5,
    Six = 6
}
public static class LinesExtensionMethods
{
    public static int ToNumber(this Lines line) => line switch
    {
        Lines.One => 1,
        Lines.Two => 2,
        Lines.Three => 3,
        Lines.Four => 4,
        Lines.Five => 5,
        Lines.Six => 6
    };
}