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
		/// Gets the latitude of the activation.
		/// </summary>
    public double Longitude { get; init; }

		/// <summary>
		/// Gets the color percentage of the activation.
		/// </summary>
		public double ColorPercentage { get; init; }

		/// <summary>
		/// Gets the tone percentage of the activation.
		/// </summary>
		public double TonePercentage { get; init; }

		/// <summary>
		/// Gets the base percentage of the activation.
		/// </summary>
		public double BasePercentage { get; init; }

		public override String ToString() => $"{Gate.ToNumber()}.{Line.ToNumber()}.{Color.ToNumber()}.{Tone.ToNumber()}.{Base.ToNumber()} C{Math.Round(ColorPercentage)}% T{Math.Round(TonePercentage)}% B{Math.Round(BasePercentage)}%";

    /// <summary>
    /// Determines whether the specified <see cref="Activation"/> object is equal to the current object,
    /// considering different levels of comparison depth specified by <see cref="ComparatorDepth"/>.
    /// </summary>
    /// <param name="other">The <see cref="Activation"/> object to compare with the current object.</param>
    /// <param name="depth">The depth of comparison, which determines how extensively the objects are compared.
    /// The comparison can include checks at various levels, such as Gate, Line, Color, Tone, and Base, depending on the specified depth.</param>
    /// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
    internal bool Equals(Activation? other, ComparatorDepth depth = ComparatorDepth.Line)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        if (ComparatorDepth.Gate <= depth)
        {
            if (Gate != other.Gate) return false;
        }
        
        if (ComparatorDepth.Line <= depth)
        {
            if (Line != other.Line) return false;
        }
        
        if (ComparatorDepth.Color <= depth)
        {
            if (Color != other.Color) return false;
        }
        
        if (ComparatorDepth.Tone <= depth)
        {
            if (Tone != other.Tone) return false;
        }
        
        if (ComparatorDepth.Base <= depth)
        {
            if (Base != other.Base) return false;
        }

        return true;
    }
}