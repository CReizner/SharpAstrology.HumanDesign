namespace SharpAstrology.Enums;

[Flags]
public enum ActivationTypes
{
    Personality = 1,
    Design = 2,
    Mixed = 4,
    Any = Personality | Design | Mixed,
    None = 16
}