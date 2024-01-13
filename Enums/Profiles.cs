#pragma warning disable CS8524

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
        Profiles.SixThree => (Lines.Six, Lines.Three)
    };

    public static string ToText(this Profiles profile) => profile switch
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
        Profiles.SixThree => "6 / 3"
    };

    public static Angles ToAngle(this Profiles profile) => profile switch
    {
        Profiles.OneThree => Angles.Right,
        Profiles.OneFour => Angles.Right,
        Profiles.TwoFour => Angles.Right,
        Profiles.TwoFive => Angles.Right,
        Profiles.ThreeFive => Angles.Right,
        Profiles.ThreeSix => Angles.Right,
        Profiles.FourSix => Angles.Right,
        Profiles.FourOne => Angles.Juxtaposition,
        Profiles.FiveOne => Angles.Left,
        Profiles.FiveTwo => Angles.Left,
        Profiles.SixTwo => Angles.Left,
        Profiles.SixThree => Angles.Left,
    };
}