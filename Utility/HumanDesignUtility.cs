using SharpAstrology.Enums;
using SharpAstrology.DataModels;

namespace SharpAstrology.Utility;

/// <summary>
/// Utility class for calculating Human Design-related stuff.
/// </summary>
public static class HumanDesignUtility
{
    private const double HdOffsetToZodiac = 3.875;
    private const double DegreePerGate = 5.626;
    private const double DegreePerLine = 0.9375;
    private const double DegreePerColor = 0.15625;
    private const double DegreePerTone = DegreePerColor / 6;
    private const double DegreePerBase = DegreePerTone / 5;
    
    /// <summary>
    /// Determines the set of active gates based on the planet positions for personality and design.
    /// </summary>
    /// <param name="personalityActivation">Activation values for personality planets.</param>
    /// <param name="designActivation">Activation values for design planets.</param>
    /// <returns>A HashSet containing the active gates.</returns>
    public static HashSet<Gates> ActiveGates(
        Dictionary<Planets, Activation> personalityActivation,
        Dictionary<Planets, Activation> designActivation)
    {
        var activeGates = personalityActivation.Select(pair => pair.Value.Gate).ToHashSet();
        activeGates.UnionWith(designActivation.Select(pair => pair.Value.Gate).ToHashSet());
        return activeGates;
    }
    
    /// <summary>
    /// Determines the set of active channels based on the provided set of active gates.
    /// </summary>
    /// <param name="activeGates">The set of active gates.</param>
    /// <returns>A HashSet containing the active channels.</returns>
    public static HashSet<Channels> ActiveChannels(HashSet<Gates> activeGates)
    {
        var activeChannels = new HashSet<Channels>();
        foreach (var channel in Enum.GetValues<Channels>())
        {
            var (key1, key2) = channel.ToGates();
            if(activeGates.Contains(key1) && activeGates.Contains(key2)) activeChannels.Add(channel);
        }
    
        return activeChannels;
    }
    
    /// <summary>
    /// Determines the activation values (Gate, Line, Color, Tone, Base) based on the provided longitude.
    /// </summary>
    /// <param name="longitude">The longitude used for activation calculation.</param>
    /// <returns>An <see cref="DataModels.Activation"/> object containing the calculated values.</returns>
    public static Activation ActivationOf(double longitude)
    {
        var x = longitude - HdOffsetToZodiac;
        if (x < 0) x += 360;
            
        return new Activation()
        {
            Gate = (Gates)Math.Floor(x / DegreePerGate),
            Line = (Lines)Math.Floor((x % DegreePerGate) / DegreePerLine) + 1,
            Color = (Color)Math.Floor((x % DegreePerLine) / DegreePerColor) + 1,
            Tone = (Tone)Math.Floor((x % DegreePerColor) / DegreePerTone) + 1,
            Base = (Base)Math.Floor((x % DegreePerTone) / DegreePerBase) + 1
        };
    }

}