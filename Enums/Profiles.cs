namespace SharpAstrology.Enums;
public enum Profiles
{
    OneThree,
    OneFour,
    TwoFour,
    TwoFive,
    ThreeFive,
    ThreeSix,
    FourSix,
    FourOne,
    FiveOne,
    FiveTwo,
    SixTwo,
    SixThree
}
public static class ProfilesExtensionMethods
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

    public static (Lines Conscious, Lines Unconscious) ToLines(this Profiles profile) => profile switch
    {
        Profiles.OneThree => (Lines.One, Lines.Three),
        Profiles.OneFour => (Lines.One, Lines.Four),
        Profiles.TwoFour => (Lines.Two, Lines.Four),
        Profiles.TwoFive => (Lines.Two, Lines.Five),
        Profiles.ThreeFive => (Lines.Three, Lines.Five),
        Profiles.ThreeSix => (Lines.Three, Lines.Six),
        Profiles.FourSix => (Lines.Four, Lines.Six),
        Profiles.FourOne => (Lines.Four, Lines.One),
        Profiles.FiveOne => (Lines.Five, Lines.One),
        Profiles.FiveTwo => (Lines.Five, Lines.Two),
        Profiles.SixTwo => (Lines.Six, Lines.Two),
        Profiles.SixThree => (Lines.Six, Lines.Three),
        _ => throw new Exception($"No Human Design Profile defined for {profile}")
    };

    public static string GetAsString(this Profiles profiles) => profiles switch
    {
        Profiles.OneThree => "1 / 3",
        Profiles.OneFour => "1 / 4",
        Profiles.TwoFour => "2 / 4",
        Profiles.TwoFive => "2 / 5",
        Profiles.ThreeFive => "3 / 5",
        Profiles.ThreeSix => "3 / 6",
        Profiles.FourSix => "4 / 6",
        Profiles.FourOne => "4 / 1",
        Profiles.FiveOne => "5 / 1",
        Profiles.FiveTwo => "5 / 2",
        Profiles.SixTwo => "6 / 2",
        Profiles.SixThree => "6 / 3",
        _ => throw new Exception($"No Human Design Profile defined for {profiles}")
    };
}