namespace SharpAstrology.Enums;

public enum Tone
{
    One = 1,
    Two = 2,
    Three = 3,
    Four = 4,
    Five = 5,
    Six = 6
}

public static class ToneExtensionMethods
{
    public static int ToNumber(this Tone line) => line switch
    {
        Tone.One => 1,
        Tone.Two => 2,
        Tone.Three => 3,
        Tone.Four => 4,
        Tone.Five => 5,
        Tone.Six => 6
    };

    public static Orientation ToVOrientation(this Tone line) => line switch
    {
        Tone.One => Orientation.Left,
        Tone.Two => Orientation.Left,
        Tone.Three => Orientation.Left,
        Tone.Four => Orientation.Right,
        Tone.Five => Orientation.Right,
        Tone.Six => Orientation.Right
    };
}