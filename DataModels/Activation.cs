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

    /// <summary>
    /// Determines whether the specified <see cref="Activation"/> object is equal to the current object,
    /// considering different levels of comparison depth specified by <see cref="ComparerDepth"/>.
    /// </summary>
    /// <param name="other">The <see cref="Activation"/> object to compare with the current object.</param>
    /// <param name="depth">The depth of comparison, which determines how extensively the objects are compared.
    /// The comparison can include checks at various levels, such as Gate, Line, Color, Tone, and Base, depending on the specified depth.</param>
    /// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
    internal bool Equals(Activation? other, ComparerDepth depth = ComparerDepth.Line)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        if (ComparerDepth.Gate <= depth)
        {
            if (Gate != other.Gate) return false;
        }
        
        if (ComparerDepth.Line <= depth)
        {
            if (Line != other.Line) return false;
        }
        
        if (ComparerDepth.Color <= depth)
        {
            if (Color != other.Color) return false;
        }
        
        if (ComparerDepth.Tone <= depth)
        {
            if (Tone != other.Tone) return false;
        }
        
        if (ComparerDepth.Base <= depth)
        {
            if (Base != other.Base) return false;
        }

        return true;
    }
}