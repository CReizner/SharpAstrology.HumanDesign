using SharpAstrology.Enums;

namespace SharpAstrology.DataModels;

/// <summary>
/// Represents detailed information about the activation of a gate.
/// </summary>
public sealed class Activation
{
    /// <summary>
    /// Gets the gate that is activated.
    /// </summary>
    public Gates Gate { get; init; }
    
    /// <summary>
    /// Gets the specific line of the gate that is activated. Each gate is divided into six lines. 
    /// </summary>
    public Lines Line { get; init; }
    
    /// <summary>
    /// Gets the color associated with the activation. The color is a subdivision of a line.
    /// Each line is divided into six colors.
    /// </summary>
    public Color Color { get; init; }
    
    /// <summary>
    /// Gets the tone that's related to the particular activation. The tone is a subdivision of a color.
    /// Each color is divided into six tones.
    /// </summary>
    public Tone Tone { get; init; }
    
    /// <summary>
    /// Gets the base that's related to the particular activation. The base is a subdivision of a tone.
    /// Each tone is divided into five bases.
    /// </summary>
    public Base Base { get; init; }

    /// <summary>
    /// Gets the planets fixing state of the activation. Depending on the gate and line where the planet is situated, it can
    /// be in exaltation, detriment or both or no special state.
    /// </summary>
    public FixingState FixingState { get; set; }
}