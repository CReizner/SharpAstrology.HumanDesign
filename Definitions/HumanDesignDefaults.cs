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
        Planets.Sun, 
				Planets.Earth, 
        Planets.NorthNode, 
				Planets.SouthNode,
				Planets.Moon, 
				Planets.Mercury,
        Planets.Uranus, 
        Planets.Venus, 
				Planets.Mars, 
				Planets.Neptune, 
				Planets.Saturn, 
				Planets.Jupiter, 
				Planets.Pluto, 
    ];
}