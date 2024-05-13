using SharpAstrology.Enums;

namespace SharpAstrology.DataModels;

/// <summary>
/// Represents the planets fixing state that is dependent on the activations of all planets in the related harmonic gates.
/// </summary>
public sealed class PlanetaryFixation
{
    /// <summary>
    /// Gets the planets fixing state of the activation. Depending on the gate and line where the planet is situated, it can
    /// be in exaltation, detriment or both or no special state.
    /// </summary>
    public FixingState FixingState { get; set; }
    
    /// <summary>
    /// If a fixing state changes due to a comparison set of celestial objects, such as transits or a partner chart, then this value is true.
    /// In the case of an individual chart, this means that if a fixing state of a personal planet is changed by a design planet (or vice versa), then this value is true.
    /// </summary>
    public bool FixingStateChangedByComparer { get; set; }
}