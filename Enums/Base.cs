namespace SharpAstrology.Enums;

public enum Base
{
    One = 1,
    Two = 2,
    Three = 3,
    Four = 4,
    Five = 5
}

public static class BaseExtensionMethods
{
    public static int ToNumber(this Base b) => b switch
    {
        Base.One => 1,
        Base.Two => 2,
        Base.Three => 3,
        Base.Four => 4,
        Base.Five => 5
    };
}