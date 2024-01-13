#pragma warning disable CS8524

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
    public static Profiles ToProfile(this (Lines, Lines) line) => line.Item1 switch
    {
        Lines.One when line.Item2 == Lines.Three => Profiles.OneThree,
        Lines.One when line.Item2 == Lines.Four => Profiles.OneFour,
        Lines.Two when line.Item2 == Lines.Four => Profiles.TwoFour,
        Lines.Two when line.Item2 == Lines.Five => Profiles.TwoFive,
        Lines.Three when line.Item2 == Lines.Five => Profiles.ThreeFive,
        Lines.Three when line.Item2 == Lines.Six => Profiles.ThreeSix,
        Lines.Four when line.Item2 == Lines.Six => Profiles.FourSix,
        Lines.Four when line.Item2 == Lines.One => Profiles.FourOne,
        Lines.Five when line.Item2 == Lines.One => Profiles.FiveOne,
        Lines.Five when line.Item2 == Lines.Two => Profiles.FiveTwo,
        Lines.Six when line.Item2 == Lines.Two => Profiles.SixTwo,
        Lines.Six when line.Item2 == Lines.Three => Profiles.SixThree,
        _ => throw new Exception($"No Human Design Profile defined for {line.Item1} / {line.Item2}")
    };
    
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