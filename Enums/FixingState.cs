namespace SharpAstrology.Enums;

[Flags]
public enum FixingState
{
    None = 0,
    Exalted = 1,
    Detriment = 2,
    Juxtaposed = Exalted | Detriment
}