namespace SharpAstrology.Enums;

[Flags]
public enum FixingState
{
    Exalted = 1,
    Detriment = 2,
    Juxtaposed = Exalted | Detriment,
    None = 0
}