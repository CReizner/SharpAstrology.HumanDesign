using SharpAstrology.Enums;

namespace SharpAstrology.Definitions;

/// <summary>
/// Contains static lists defaults for astrological related subjects.
/// </summary>
public static class HumanDesignDefaults
{
    /// <summary>
    /// The major and minor celestial bodies used by the default Human Design system.
    /// </summary>
    public static readonly Planets[] HumanDesignPlanets =
    [
        Planets.Sun, Planets.Earth, Planets.Mars, Planets.Mercury,
        Planets.Venus, Planets.Moon, Planets.Jupiter, Planets.Saturn, 
        Planets.Uranus, Planets.Neptune, Planets.Pluto, 
        Planets.NorthNode, Planets.SouthNode
    ];
}