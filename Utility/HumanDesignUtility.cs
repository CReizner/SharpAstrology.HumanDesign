using SharpAstrology.Enums;
using SharpAstrology.DataModels;
using SharpAstrology.Definitions;

namespace SharpAstrology.Utility;

/// <summary>
/// Utility class for calculating Human Design-related stuff.
/// </summary>
public static class HumanDesignUtility
{
    private const double HdOffsetToZodiac = 3.875;
    private const double DegreePerGate = 5.625;
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
            Base = (Base)Math.Floor((x % DegreePerTone) / DegreePerBase) + 1,
						ColorPercentage = ((x % DegreePerColor) / DegreePerColor) * 100.0,
						TonePercentage = ((x % DegreePerTone) / DegreePerTone) * 100.0,
						BasePercentage = ((x % DegreePerBase) / DegreePerBase) * 100.0,
						Longitude = longitude,
        };
    }
    
    /// <summary>
    /// Determines the type of split definition based on the number of splits provided.
    /// </summary>
    /// <param name="splits">The number of splits.</param>
    /// <returns>The corresponding SplitDefinitions enum.</returns>
    /// <exception cref="ArgumentException">Thrown when the number of splits is not within the expected range (0-4).</exception>
    public static SplitDefinitions SplitDefinition(int splits)
    {
        return splits switch
        {
            0 => SplitDefinitions.Empty,
            1 => SplitDefinitions.SingleDefinition,
            2 => SplitDefinitions.SplitDefinition,
            3 => SplitDefinitions.TripleSplit,
            4 => SplitDefinitions.QuadrupleSplit,
            _ => throw new ArgumentException($"To much splits: {splits}")
        };
    }

    private static FixingState _getStateFromStatesTable(Planets planet, Gates gate, Lines line)
    {
        return planet switch
        {
            Planets.Moon => (gate, line) switch
            {
                (Gates.Key1, Lines.One) => FixingState.Exalted,
                (Gates.Key4, Lines.One) => FixingState.Exalted,
                (Gates.Key4, Lines.Two) => FixingState.Exalted,
                (Gates.Key5, Lines.Three) => FixingState.Detriment,
                (Gates.Key6, Lines.Five) => FixingState.Detriment,
                (Gates.Key7, Lines.Three) => FixingState.Exalted,
                (Gates.Key8, Lines.Three) => FixingState.Exalted,
                (Gates.Key9, Lines.Four) => FixingState.Exalted,
                (Gates.Key9, Lines.Six) => FixingState.Exalted,
                (Gates.Key10, Lines.One) => FixingState.Detriment,
                (Gates.Key10, Lines.Three) => FixingState.Detriment,
                (Gates.Key11, Lines.One) => FixingState.Exalted,
                (Gates.Key11, Lines.Four) => FixingState.Exalted,
                (Gates.Key11, Lines.Five) => FixingState.Exalted,
                (Gates.Key13, Lines.One) => FixingState.Detriment,
                (Gates.Key13, Lines.Two) => FixingState.Exalted,
                (Gates.Key14, Lines.Four) => FixingState.Exalted,
                (Gates.Key16, Lines.Three) => FixingState.Exalted,
                (Gates.Key16, Lines.Five) => FixingState.Detriment,
                (Gates.Key17, Lines.Two) => FixingState.Detriment,
                (Gates.Key17, Lines.Six) => FixingState.Exalted,
                (Gates.Key18, Lines.Two) => FixingState.Detriment,
                (Gates.Key18, Lines.Six) => FixingState.Detriment,
                (Gates.Key19, Lines.One) => FixingState.Detriment,
                (Gates.Key19, Lines.Three) => FixingState.Detriment,
                (Gates.Key20, Lines.One) => FixingState.Detriment,
                (Gates.Key20, Lines.Two) => FixingState.Detriment,
                (Gates.Key21, Lines.One) => FixingState.Detriment,
                (Gates.Key22, Lines.One) => FixingState.Exalted,
                (Gates.Key23, Lines.Two) => FixingState.Detriment,
                (Gates.Key23, Lines.Five) => FixingState.Detriment,
                (Gates.Key24, Lines.Two) => FixingState.Exalted,
                (Gates.Key24, Lines.Five) => FixingState.Exalted,
                (Gates.Key26, Lines.Six) => FixingState.Detriment,
                (Gates.Key27, Lines.Two) => FixingState.Exalted,
                (Gates.Key27, Lines.Six) => FixingState.Exalted,
                (Gates.Key30, Lines.Six) => FixingState.Detriment,
                (Gates.Key31, Lines.Four) => FixingState.Exalted,
                (Gates.Key31, Lines.Five) => FixingState.Detriment,
                (Gates.Key31, Lines.Six) => FixingState.Detriment,
                (Gates.Key32, Lines.Five) => FixingState.Exalted,
                (Gates.Key34, Lines.Five) => FixingState.Detriment,
                (Gates.Key35, Lines.Two) => FixingState.Detriment,
                (Gates.Key35, Lines.Four) => FixingState.Exalted,
                (Gates.Key36, Lines.Two) => FixingState.Detriment,
                (Gates.Key36, Lines.Four) => FixingState.Detriment,
                (Gates.Key37, Lines.Four) => FixingState.Exalted,
                (Gates.Key38, Lines.Two) => FixingState.Detriment,
                (Gates.Key39, Lines.Two) => FixingState.Exalted,
                (Gates.Key39, Lines.Four) => FixingState.Exalted,
                (Gates.Key39, Lines.Six) => FixingState.Exalted,
                (Gates.Key40, Lines.One) => FixingState.Detriment,
                (Gates.Key40, Lines.Two) => FixingState.Detriment,
                (Gates.Key41, Lines.Three) => FixingState.Detriment,
                (Gates.Key42, Lines.Three) => FixingState.Detriment,
                (Gates.Key42, Lines.Four) => FixingState.Exalted,
                (Gates.Key42, Lines.Six) => FixingState.Exalted,
                (Gates.Key43, Lines.Two) => FixingState.Detriment,
                (Gates.Key43, Lines.Three) => FixingState.Detriment,
                (Gates.Key43, Lines.Five) => FixingState.Exalted,
                (Gates.Key46, Lines.Three) => FixingState.Exalted,
                (Gates.Key46, Lines.Five) => FixingState.Exalted,
                (Gates.Key47, Lines.Four) => FixingState.Detriment,
                (Gates.Key48, Lines.One) => FixingState.Exalted,
                (Gates.Key48, Lines.Three) => FixingState.Exalted,
                (Gates.Key48, Lines.Five) => FixingState.Detriment,
                (Gates.Key48, Lines.Six) => FixingState.Detriment,
                (Gates.Key49, Lines.Five) => FixingState.Exalted,
                (Gates.Key50, Lines.Three) => FixingState.Exalted,
                (Gates.Key50, Lines.Six) => FixingState.Detriment,
                (Gates.Key53, Lines.Two) => FixingState.Exalted,
                (Gates.Key53, Lines.Three) => FixingState.Exalted,
                (Gates.Key53, Lines.Four) => FixingState.Exalted,
                (Gates.Key53, Lines.Six) => FixingState.Exalted,
                (Gates.Key55, Lines.Six) => FixingState.Detriment,
                (Gates.Key56, Lines.One) => FixingState.Exalted,
                (Gates.Key56, Lines.Two) => FixingState.Detriment,
                (Gates.Key56, Lines.Four) => FixingState.Exalted,
                (Gates.Key57, Lines.One) => FixingState.Detriment,
                (Gates.Key57, Lines.Two) => FixingState.Detriment,
                (Gates.Key57, Lines.Five) => FixingState.Detriment,
                (Gates.Key58, Lines.One) => FixingState.Detriment,
                (Gates.Key58, Lines.Five) => FixingState.Exalted,
                (Gates.Key58, Lines.Six) => FixingState.Exalted,
                (Gates.Key61, Lines.Two) => FixingState.Exalted,
                (Gates.Key61, Lines.Three) => FixingState.Exalted,
                (Gates.Key62, Lines.Five) => FixingState.Exalted,
                (Gates.Key64, Lines.Two) => FixingState.Detriment,
                (Gates.Key64, Lines.Three) => FixingState.Detriment,
                (Gates.Key64, Lines.Four) => FixingState.Exalted,
                _ => FixingState.None
            },
            Planets.Uranus => (gate, line) switch
            {
                (Gates.Key1, Lines.One) => FixingState.Detriment,
                (Gates.Key1, Lines.Five) => FixingState.Detriment,
                (Gates.Key2, Lines.Three) => FixingState.Detriment,
                (Gates.Key3, Lines.Two) => FixingState.Detriment,
                (Gates.Key5, Lines.Four) => FixingState.Exalted,
                (Gates.Key7, Lines.Four) => FixingState.Detriment,
                (Gates.Key7, Lines.Six) => FixingState.Detriment,
                (Gates.Key10, Lines.Four) => FixingState.Exalted,
                (Gates.Key17, Lines.Five) => FixingState.Exalted,
                (Gates.Key18, Lines.Five) => FixingState.Detriment,
                (Gates.Key20, Lines.Five) => FixingState.Detriment,
                (Gates.Key25, Lines.Six) => FixingState.Detriment,
                (Gates.Key40, Lines.Four) => FixingState.Exalted,
                (Gates.Key40, Lines.Five) => FixingState.Exalted,
                (Gates.Key44, Lines.Five) => FixingState.Exalted,
                (Gates.Key45, Lines.Two) => FixingState.Exalted,
                (Gates.Key45, Lines.Five) => FixingState.Exalted,
                (Gates.Key45, Lines.Six) => FixingState.Exalted,
                (Gates.Key51, Lines.Four) => FixingState.Exalted,
                (Gates.Key55, Lines.Five) => FixingState.Exalted,
                (Gates.Key56, Lines.Two) => FixingState.Exalted,
                (Gates.Key56, Lines.Five) => FixingState.Exalted,
                (Gates.Key57, Lines.Six) => FixingState.Exalted,
                (Gates.Key58, Lines.Two) => FixingState.Detriment,
                (Gates.Key58, Lines.Three) => FixingState.Exalted,
                (Gates.Key59, Lines.Two) => FixingState.Exalted,
                (Gates.Key59, Lines.Five) => FixingState.Detriment,
                (Gates.Key60, Lines.Six) => FixingState.Exalted,
                (Gates.Key62, Lines.Three) => FixingState.Exalted,
                (Gates.Key63, Lines.Two) => FixingState.Detriment,
                _ => FixingState.None
            },
            Planets.Venus => (gate, line) switch
            {
                (Gates.Key1, Lines.Two) => FixingState.Exalted,
                (Gates.Key2, Lines.One) => FixingState.Exalted,
                (Gates.Key2, Lines.Four) => FixingState.Exalted,
                (Gates.Key3, Lines.Three) => FixingState.Exalted,
                (Gates.Key4, Lines.Three) => FixingState.Exalted,
                (Gates.Key5, Lines.Two) => FixingState.Exalted,
                (Gates.Key5, Lines.Five) => FixingState.Exalted,
                (Gates.Key6, Lines.Two) => FixingState.Exalted,
                (Gates.Key6, Lines.Five) => FixingState.Exalted,
                (Gates.Key6, Lines.Six) => FixingState.Detriment,
                (Gates.Key7, Lines.One) => FixingState.Exalted,
                (Gates.Key7, Lines.Five) => FixingState.Exalted,
                (Gates.Key8, Lines.Six) => FixingState.Exalted,
                (Gates.Key11, Lines.Three) => FixingState.Detriment,
                (Gates.Key12, Lines.One) => FixingState.Exalted,
                (Gates.Key13, Lines.One) => FixingState.Exalted,
                (Gates.Key13, Lines.Three) => FixingState.Detriment,
                (Gates.Key13, Lines.Four) => FixingState.Detriment,
                (Gates.Key14, Lines.Five) => FixingState.Detriment,
                (Gates.Key15, Lines.One) => FixingState.Exalted,
                (Gates.Key15, Lines.Six) => FixingState.Detriment,
                (Gates.Key17, Lines.One) => FixingState.Detriment,
                (Gates.Key19, Lines.Three) => FixingState.Exalted,
                (Gates.Key19, Lines.Four) => FixingState.Detriment,
                (Gates.Key20, Lines.One) => FixingState.Exalted,
                (Gates.Key20, Lines.Two) => FixingState.Exalted,
                (Gates.Key20, Lines.Six) => FixingState.Exalted,
                (Gates.Key21, Lines.Six) => FixingState.Detriment,
                (Gates.Key24, Lines.Three) => FixingState.Exalted,
                (Gates.Key25, Lines.Four) => FixingState.Exalted,
                (Gates.Key25, Lines.Five) => FixingState.Exalted,
                (Gates.Key26, Lines.Five) => FixingState.Detriment,
                (Gates.Key28, Lines.One) => FixingState.Detriment,
                (Gates.Key29, Lines.Two) => FixingState.Detriment,
                (Gates.Key29, Lines.Four) => FixingState.Detriment,
                (Gates.Key32, Lines.Two) => FixingState.Exalted,
                (Gates.Key34, Lines.Two) => FixingState.Detriment,
                (Gates.Key35, Lines.One) => FixingState.Exalted,
                (Gates.Key35, Lines.Two) => FixingState.Exalted,
                (Gates.Key37, Lines.One) => FixingState.Exalted,
                (Gates.Key37, Lines.Five) => FixingState.Exalted,
                (Gates.Key37, Lines.Six) => FixingState.Exalted,
                (Gates.Key41, Lines.Four) => FixingState.Detriment,
                (Gates.Key41, Lines.Five) => FixingState.Detriment,
                (Gates.Key42, Lines.One) => FixingState.Detriment,
                (Gates.Key42, Lines.Two) => FixingState.Detriment,
                (Gates.Key42, Lines.Four) => FixingState.Detriment,
                (Gates.Key42, Lines.Five) => FixingState.Detriment,
                (Gates.Key43, Lines.One) => FixingState.Detriment,
                (Gates.Key43, Lines.Five) => FixingState.Detriment,
                (Gates.Key44, Lines.One) => FixingState.Detriment,
                (Gates.Key47, Lines.Five) => FixingState.Exalted,
                (Gates.Key48, Lines.Two) => FixingState.Detriment,
                (Gates.Key48, Lines.Six) => FixingState.Exalted,
                (Gates.Key50, Lines.One) => FixingState.Detriment,
                (Gates.Key50, Lines.Two) => FixingState.Detriment,
                (Gates.Key50, Lines.Six) => FixingState.Exalted,
                (Gates.Key51, Lines.One) => FixingState.Detriment,
                (Gates.Key52, Lines.Two) => FixingState.Exalted,
                (Gates.Key52, Lines.Three) => FixingState.Detriment,
                (Gates.Key52, Lines.Six) => FixingState.Exalted,
                (Gates.Key53, Lines.One) => FixingState.Detriment,
                (Gates.Key53, Lines.Four) => FixingState.Detriment,
                (Gates.Key54, Lines.One) => FixingState.Detriment,
                (Gates.Key54, Lines.Three) => FixingState.Detriment,
                (Gates.Key55, Lines.One) => FixingState.Detriment,
                (Gates.Key55, Lines.Two) => FixingState.Exalted,
                (Gates.Key56, Lines.Three) => FixingState.Detriment,
                (Gates.Key57, Lines.One) => FixingState.Exalted,
                (Gates.Key57, Lines.Two) => FixingState.Exalted,
                (Gates.Key57, Lines.Four) => FixingState.Exalted,
                (Gates.Key58, Lines.One) => FixingState.Exalted,
                (Gates.Key59, Lines.Four) => FixingState.Exalted,
                (Gates.Key59, Lines.Six) => FixingState.Exalted,
                (Gates.Key60, Lines.One) => FixingState.Exalted,
                (Gates.Key60, Lines.Four) => FixingState.Detriment,
                (Gates.Key61, Lines.One) => FixingState.Detriment,
                (Gates.Key62, Lines.Three) => FixingState.Detriment,
                (Gates.Key62, Lines.Four) => FixingState.Exalted,
                (Gates.Key64, Lines.One) => FixingState.Exalted,
                (Gates.Key64, Lines.Two) => FixingState.Exalted,
                (Gates.Key64, Lines.Five) => FixingState.Exalted,
                (Gates.Key64, Lines.Six) => FixingState.Detriment,
                _ => FixingState.None
            },
            Planets.Mars => (gate, line) switch
            {
                (Gates.Key1, Lines.Two) => FixingState.Detriment,
                (Gates.Key1, Lines.Three) => FixingState.Exalted,
                (Gates.Key1, Lines.Five) => FixingState.Exalted,
                (Gates.Key2, Lines.One) => FixingState.Detriment,
                (Gates.Key2, Lines.Two) => FixingState.Detriment,
                (Gates.Key2, Lines.Four) => FixingState.Detriment,
                (Gates.Key3, Lines.Two) => FixingState.Exalted,
                (Gates.Key3, Lines.Four) => FixingState.Detriment,
                (Gates.Key3, Lines.Five) => FixingState.Exalted,
                (Gates.Key4, Lines.Two) => FixingState.Detriment,
                (Gates.Key4, Lines.Six) => FixingState.Detriment,
                (Gates.Key5, Lines.One) => FixingState.Exalted,
                (Gates.Key6, Lines.Two) => FixingState.Detriment,
                (Gates.Key9, Lines.One) => FixingState.Detriment,
                (Gates.Key9, Lines.Four) => FixingState.Detriment,
                (Gates.Key10, Lines.Two) => FixingState.Detriment,
                (Gates.Key10, Lines.Five) => FixingState.Detriment,
                (Gates.Key11, Lines.One) => FixingState.Detriment,
                (Gates.Key11, Lines.Two) => FixingState.Detriment,
                (Gates.Key12, Lines.Three) => FixingState.Detriment,
                (Gates.Key12, Lines.Five) => FixingState.Detriment,
                (Gates.Key13, Lines.Six) => FixingState.Exalted,
                (Gates.Key14, Lines.Two) => FixingState.Detriment,
                (Gates.Key14, Lines.Four) => FixingState.Detriment,
                (Gates.Key15, Lines.One) => FixingState.Detriment,
                (Gates.Key16, Lines.Three) => FixingState.Detriment,
                (Gates.Key16, Lines.Four) => FixingState.Detriment,
                (Gates.Key17, Lines.One) => FixingState.Exalted,
                (Gates.Key17, Lines.Five) => FixingState.Detriment,
                (Gates.Key18, Lines.Six) => FixingState.Exalted,
                (Gates.Key19, Lines.Four) => FixingState.Exalted,
                (Gates.Key19, Lines.Six) => FixingState.Detriment,
                (Gates.Key21, Lines.One) => FixingState.Exalted,
                (Gates.Key21, Lines.Two) => FixingState.Exalted,
                (Gates.Key22, Lines.One) => FixingState.Detriment,
                (Gates.Key22, Lines.Three) => FixingState.Detriment,
                (Gates.Key22, Lines.Four) => FixingState.Detriment,
                (Gates.Key22, Lines.Five) => FixingState.Detriment,
                (Gates.Key22, Lines.Six) => FixingState.Detriment,
                (Gates.Key23, Lines.One) => FixingState.Detriment,
                (Gates.Key23, Lines.Six) => FixingState.Exalted,
                (Gates.Key24, Lines.Two) => FixingState.Detriment,
                (Gates.Key24, Lines.Five) => FixingState.Detriment,
                (Gates.Key25, Lines.Two) => FixingState.Detriment,
                (Gates.Key25, Lines.Three) => FixingState.Exalted,
                (Gates.Key26, Lines.One) => FixingState.Detriment,
                (Gates.Key26, Lines.Five) => FixingState.Exalted,
                (Gates.Key27, Lines.Two) => FixingState.Detriment,
                (Gates.Key27, Lines.Three) => FixingState.Detriment,
                (Gates.Key27, Lines.Four) => FixingState.Detriment,
                (Gates.Key28, Lines.One) => FixingState.Exalted,
                (Gates.Key29, Lines.One) => FixingState.Exalted,
                (Gates.Key29, Lines.Three) => FixingState.Exalted,
                (Gates.Key29, Lines.Six) => FixingState.Exalted,
                (Gates.Key30, Lines.Two) => FixingState.Detriment,
                (Gates.Key30, Lines.Six) => FixingState.Exalted,
                (Gates.Key31, Lines.Four) => FixingState.Detriment,
                (Gates.Key32, Lines.One) => FixingState.Detriment,
                (Gates.Key32, Lines.Five) => FixingState.Detriment,
                (Gates.Key33, Lines.One) => FixingState.Detriment,
                (Gates.Key33, Lines.Three) => FixingState.Detriment,
                (Gates.Key34, Lines.Two) => FixingState.Exalted,
                (Gates.Key34, Lines.Four) => FixingState.Detriment,
                (Gates.Key34, Lines.Five) => FixingState.Exalted,
                (Gates.Key35, Lines.Four) => FixingState.Detriment,
                (Gates.Key35, Lines.Six) => FixingState.Detriment,
                (Gates.Key36, Lines.One) => FixingState.Exalted,
                (Gates.Key37, Lines.Three) => FixingState.Detriment,
                (Gates.Key37, Lines.Five) => FixingState.Detriment,
                (Gates.Key38, Lines.One) => FixingState.Detriment,
                (Gates.Key38, Lines.Four) => FixingState.Detriment,
                (Gates.Key39, Lines.One) => FixingState.Exalted,
                (Gates.Key39, Lines.Five) => FixingState.Detriment,
                (Gates.Key39, Lines.Six) => FixingState.Detriment,
                (Gates.Key40, Lines.Three) => FixingState.Detriment,
                (Gates.Key40, Lines.Four) => FixingState.Detriment,
                (Gates.Key41, Lines.Two) => FixingState.Detriment,
                (Gates.Key41, Lines.Five) => FixingState.Exalted,
                (Gates.Key42, Lines.Three) => FixingState.Exalted,
                (Gates.Key43, Lines.Six) => FixingState.Detriment,
                (Gates.Key44, Lines.Two) => FixingState.Detriment,
                (Gates.Key44, Lines.Three) => FixingState.Exalted,
                (Gates.Key44, Lines.Five) => FixingState.Detriment,
                (Gates.Key45, Lines.One) => FixingState.Detriment,
                (Gates.Key45, Lines.Two) => FixingState.Detriment,
                (Gates.Key45, Lines.Three) => FixingState.Detriment,
                (Gates.Key45, Lines.Four) => FixingState.Detriment,
                (Gates.Key46, Lines.Two) => FixingState.Detriment,
                (Gates.Key46, Lines.Three) => FixingState.Detriment,
                (Gates.Key47, Lines.Three) => FixingState.Detriment,
                (Gates.Key48, Lines.One) => FixingState.Detriment,
                (Gates.Key48, Lines.Five) => FixingState.Exalted,
                (Gates.Key49, Lines.Four) => FixingState.Detriment,
                (Gates.Key49, Lines.Five) => FixingState.Detriment,
                (Gates.Key50, Lines.One) => FixingState.Exalted,
                (Gates.Key50, Lines.Four) => FixingState.Detriment,
                (Gates.Key50, Lines.Five) => FixingState.Detriment,
                (Gates.Key51, Lines.Two) => FixingState.Exalted,
                (Gates.Key51, Lines.Five) => FixingState.Detriment,
                (Gates.Key52, Lines.One) => FixingState.Detriment,
                (Gates.Key52, Lines.Two) => FixingState.Detriment,
                (Gates.Key53, Lines.Two) => FixingState.Detriment,
                (Gates.Key53, Lines.Three) => FixingState.Detriment,
                (Gates.Key54, Lines.Two) => FixingState.Detriment,
                (Gates.Key55, Lines.Three) => FixingState.Detriment,
                (Gates.Key55, Lines.Four) => FixingState.Detriment,
                (Gates.Key56, Lines.One) => FixingState.Detriment,
                (Gates.Key56, Lines.Five) => FixingState.Detriment,
                (Gates.Key57, Lines.Four) => FixingState.Detriment,
                (Gates.Key57, Lines.Six) => FixingState.Detriment,
                (Gates.Key58, Lines.Three) => FixingState.Detriment,
                (Gates.Key59, Lines.Three) => FixingState.Detriment,
                (Gates.Key60, Lines.Three) => FixingState.Detriment,
                (Gates.Key61, Lines.Two) => FixingState.Detriment,
                (Gates.Key61, Lines.Three) => FixingState.Detriment,
                (Gates.Key61, Lines.Five) => FixingState.Detriment,
                (Gates.Key61, Lines.Six) => FixingState.Detriment,
                (Gates.Key62, Lines.One) => FixingState.Detriment,
                (Gates.Key63, Lines.One) => FixingState.Detriment,
                (Gates.Key63, Lines.Four) => FixingState.Detriment,
                (Gates.Key63, Lines.Five) => FixingState.Detriment,
                (Gates.Key64, Lines.One) => FixingState.Detriment,
                (Gates.Key64, Lines.Four) => FixingState.Detriment,
                _ => FixingState.None
            },
            Planets.Earth => (gate, line) switch
            {
                (Gates.Key1, Lines.Three) => FixingState.Detriment,
                (Gates.Key1, Lines.Four) => FixingState.Exalted,
                (Gates.Key1, Lines.Six) => FixingState.Exalted,
                (Gates.Key2, Lines.Five) => FixingState.Detriment,
                (Gates.Key3, Lines.One) => FixingState.Exalted,
                (Gates.Key3, Lines.Five) => FixingState.Detriment,
                (Gates.Key4, Lines.One) => FixingState.Detriment,
                (Gates.Key5, Lines.One) => FixingState.Detriment,
                (Gates.Key8, Lines.Two) => FixingState.Detriment,
                (Gates.Key9, Lines.Three) => FixingState.Exalted,
                (Gates.Key9, Lines.Five) => FixingState.Detriment,
                (Gates.Key10, Lines.Three) => FixingState.Exalted,
                (Gates.Key12, Lines.Four) => FixingState.Exalted,
                (Gates.Key12, Lines.Six) => FixingState.Detriment,
                (Gates.Key13, Lines.Three) => FixingState.Exalted,
                (Gates.Key14, Lines.Three) => FixingState.Exalted,
                (Gates.Key14, Lines.Six) => FixingState.Detriment,
                (Gates.Key15, Lines.Two) => FixingState.Detriment,
                (Gates.Key15, Lines.Three) => FixingState.Exalted,
                (Gates.Key16, Lines.One) => FixingState.Exalted,
                (Gates.Key17, Lines.Three) => FixingState.Detriment,
                (Gates.Key18, Lines.One) => FixingState.Exalted,
                (Gates.Key18, Lines.Four) => FixingState.Exalted,
                (Gates.Key19, Lines.Five) => FixingState.Exalted,
                (Gates.Key20, Lines.Three) => FixingState.Detriment,
                (Gates.Key21, Lines.Four) => FixingState.Detriment,
                (Gates.Key23, Lines.Four) => FixingState.Detriment,
                (Gates.Key25, Lines.Six) => FixingState.Exalted,
                (Gates.Key27, Lines.One) => FixingState.Detriment,
                (Gates.Key29, Lines.Five) => FixingState.Detriment,
                (Gates.Key31, Lines.One) => FixingState.Detriment,
                (Gates.Key34, Lines.Six) => FixingState.Exalted,
                (Gates.Key38, Lines.Three) => FixingState.Detriment,
                (Gates.Key38, Lines.Six) => FixingState.Detriment,
                (Gates.Key39, Lines.Three) => FixingState.Detriment,
                (Gates.Key40, Lines.Five) => FixingState.Detriment,
                (Gates.Key40, Lines.Six) => FixingState.Detriment,
                (Gates.Key41, Lines.Four) => FixingState.Exalted,
                (Gates.Key44, Lines.Six) => FixingState.Detriment,
                (Gates.Key46, Lines.Four) => FixingState.Exalted,
                (Gates.Key48, Lines.Four) => FixingState.Detriment,
                (Gates.Key49, Lines.Two) => FixingState.Exalted,
                (Gates.Key52, Lines.One) => FixingState.Exalted,
                (Gates.Key52, Lines.Five) => FixingState.Exalted,
                (Gates.Key53, Lines.Five) => FixingState.Detriment,
                (Gates.Key55, Lines.Two) => FixingState.Detriment,
                (Gates.Key60, Lines.Two) => FixingState.Detriment,
                _ => FixingState.None
            },
            Planets.Jupiter => (gate, line) switch
            {
                (Gates.Key1, Lines.Four) => FixingState.Detriment,
                (Gates.Key2, Lines.Three) => FixingState.Exalted,
                (Gates.Key4, Lines.Five) => FixingState.Exalted,
                (Gates.Key8, Lines.Four) => FixingState.Exalted,
                (Gates.Key8, Lines.Five) => FixingState.Exalted,
                (Gates.Key9, Lines.Two) => FixingState.Detriment,
                (Gates.Key9, Lines.Five) => FixingState.Exalted,
                (Gates.Key10, Lines.Five) => FixingState.Exalted,
                (Gates.Key11, Lines.Six) => FixingState.Detriment,
                (Gates.Key12, Lines.One) => FixingState.Detriment,
                (Gates.Key13, Lines.Five) => FixingState.Detriment,
                (Gates.Key14, Lines.One) => FixingState.Exalted,
                (Gates.Key14, Lines.Two) => FixingState.Exalted,
                (Gates.Key15, Lines.Four) => FixingState.Exalted,
                (Gates.Key15, Lines.Five) => FixingState.Exalted,
                (Gates.Key16, Lines.Four) => FixingState.Exalted,
                (Gates.Key16, Lines.Six) => FixingState.Detriment,
                (Gates.Key17, Lines.Four) => FixingState.Detriment,
                (Gates.Key17, Lines.Six) => FixingState.Detriment,
                (Gates.Key18, Lines.One) => FixingState.Detriment,
                (Gates.Key18, Lines.Three) => FixingState.Detriment,
                (Gates.Key19, Lines.Two) => FixingState.Exalted,
                (Gates.Key19, Lines.Five) => FixingState.Detriment,
                (Gates.Key19, Lines.Six) => FixingState.Exalted,
                (Gates.Key20, Lines.Four) => FixingState.Exalted,
                (Gates.Key21, Lines.Three) => FixingState.Detriment,
                (Gates.Key21, Lines.Four) => FixingState.Exalted,
                (Gates.Key21, Lines.Five) => FixingState.Exalted,
                (Gates.Key22, Lines.Two) => FixingState.Detriment,
                (Gates.Key22, Lines.Five) => FixingState.Exalted,
                (Gates.Key23, Lines.One) => FixingState.Exalted,
                (Gates.Key23, Lines.Two) => FixingState.Exalted,
                (Gates.Key23, Lines.Five) => FixingState.Exalted,
                (Gates.Key23, Lines.Six) => FixingState.Detriment,
                (Gates.Key24, Lines.Three) => FixingState.Detriment,
                (Gates.Key24, Lines.Six) => FixingState.Exalted,
                (Gates.Key25, Lines.Four) => FixingState.Exalted,
                (Gates.Key25, Lines.Five) => FixingState.Detriment,
                (Gates.Key27, Lines.Four) => FixingState.Exalted,
                (Gates.Key27, Lines.Five) => FixingState.Exalted,
                (Gates.Key28, Lines.Two) => FixingState.Detriment,
                (Gates.Key28, Lines.Three) => FixingState.Detriment,
                (Gates.Key28, Lines.Four) => FixingState.Exalted,
                (Gates.Key29, Lines.Three) => FixingState.Detriment,
                (Gates.Key29, Lines.Six) => FixingState.Detriment,
                (Gates.Key30, Lines.One) => FixingState.Detriment,
                (Gates.Key30, Lines.Three) => FixingState.Detriment,
                (Gates.Key30, Lines.Four) => FixingState.Detriment,
                (Gates.Key30, Lines.Five) => FixingState.Exalted,
                (Gates.Key31, Lines.Two) => FixingState.Exalted,
                (Gates.Key31, Lines.Three) => FixingState.Detriment,
                (Gates.Key32, Lines.Two) => FixingState.Detriment,
                (Gates.Key32, Lines.Three) => FixingState.Detriment,
                (Gates.Key32, Lines.Four) => FixingState.Exalted,
                (Gates.Key33, Lines.Two) => FixingState.Exalted,
                (Gates.Key33, Lines.Three) => FixingState.Exalted,
                (Gates.Key33, Lines.Five) => FixingState.Detriment,
                (Gates.Key33, Lines.Six) => FixingState.Detriment,
                (Gates.Key34, Lines.Six) => FixingState.Detriment,
                (Gates.Key35, Lines.Three) => FixingState.Exalted,
                (Gates.Key35, Lines.Five) => FixingState.Detriment,
                (Gates.Key36, Lines.One) => FixingState.Detriment,
                (Gates.Key36, Lines.Three) => FixingState.Detriment,
                (Gates.Key36, Lines.Six) => FixingState.Exalted,
                (Gates.Key37, Lines.Two) => FixingState.Exalted,
                (Gates.Key37, Lines.Three) => FixingState.Exalted,
                (Gates.Key39, Lines.Two) => FixingState.Detriment,
                (Gates.Key39, Lines.Three) => FixingState.Exalted,
                (Gates.Key43, Lines.Four) => FixingState.Detriment,
                (Gates.Key44, Lines.Two) => FixingState.Exalted,
                (Gates.Key45, Lines.One) => FixingState.Exalted,
                (Gates.Key45, Lines.Four) => FixingState.Exalted,
                (Gates.Key45, Lines.Five) => FixingState.Detriment,
                (Gates.Key45, Lines.Six) => FixingState.Detriment,
                (Gates.Key46, Lines.One) => FixingState.Detriment,
                (Gates.Key47, Lines.Three) => FixingState.Exalted,
                (Gates.Key49, Lines.One) => FixingState.Exalted,
                (Gates.Key49, Lines.Four) => FixingState.Exalted,
                (Gates.Key51, Lines.Three) => FixingState.Detriment,
                (Gates.Key52, Lines.Four) => FixingState.Detriment,
                (Gates.Key54, Lines.Six) => FixingState.Detriment,
                (Gates.Key55, Lines.One) => FixingState.Exalted,
                (Gates.Key55, Lines.Four) => FixingState.Exalted,
                (Gates.Key60, Lines.Five) => FixingState.Detriment,
                (Gates.Key61, Lines.Four) => FixingState.Detriment,
                (Gates.Key63, Lines.Two) => FixingState.Exalted,
                (Gates.Key63, Lines.Three) => FixingState.Exalted,
                (Gates.Key63, Lines.Six) => FixingState.Exalted,
                (Gates.Key64, Lines.Five) => FixingState.Detriment,
                _ => FixingState.None
            },
            Planets.Pluto => (gate, line) switch
            {
                (Gates.Key1, Lines.Six) => FixingState.Detriment,
                (Gates.Key3, Lines.Three) => FixingState.Detriment,
                (Gates.Key3, Lines.Six) => FixingState.Detriment,
                (Gates.Key4, Lines.Three) => FixingState.Detriment,
                (Gates.Key4, Lines.Five) => FixingState.Detriment,
                (Gates.Key5, Lines.Two) => FixingState.Detriment,
                (Gates.Key5, Lines.Five) => FixingState.Detriment,
                (Gates.Key6, Lines.One) => FixingState.Exalted,
                (Gates.Key6, Lines.Three) => FixingState.Detriment,
                (Gates.Key6, Lines.Four) => FixingState.Detriment,
                (Gates.Key8, Lines.Six) => FixingState.Detriment,
                (Gates.Key9, Lines.One) => FixingState.Exalted,
                (Gates.Key9, Lines.Two) => FixingState.Exalted,
                (Gates.Key9, Lines.Six) => FixingState.Detriment,
                (Gates.Key10, Lines.Six) => FixingState.Exalted,
                (Gates.Key11, Lines.Three) => FixingState.Exalted,
                (Gates.Key13, Lines.Four) => FixingState.Exalted,
                (Gates.Key15, Lines.Five) => FixingState.Detriment,
                (Gates.Key15, Lines.Six) => FixingState.Exalted,
                (Gates.Key16, Lines.Five) => FixingState.Exalted,
                (Gates.Key17, Lines.Three) => FixingState.Exalted,
                (Gates.Key17, Lines.Four) => FixingState.Exalted,
                (Gates.Key18, Lines.Two) => FixingState.Exalted,
                (Gates.Key21, Lines.Five) => FixingState.Detriment,
                (Gates.Key21, Lines.Six) => FixingState.Exalted,
                (Gates.Key23, Lines.Three) => FixingState.Detriment,
                (Gates.Key24, Lines.Six) => FixingState.Detriment,
                (Gates.Key25, Lines.Three) => FixingState.Detriment,
                (Gates.Key26, Lines.Two) => FixingState.Detriment,
                (Gates.Key26, Lines.Four) => FixingState.Exalted,
                (Gates.Key27, Lines.Three) => FixingState.Exalted,
                (Gates.Key27, Lines.Six) => FixingState.Detriment,
                (Gates.Key28, Lines.Five) => FixingState.Exalted,
                (Gates.Key28, Lines.Six) => FixingState.Exalted,
                (Gates.Key30, Lines.Three) => FixingState.Exalted,
                (Gates.Key30, Lines.Four) => FixingState.Exalted,
                (Gates.Key30, Lines.Five) => FixingState.Detriment,
                (Gates.Key31, Lines.Five) => FixingState.Exalted,
                (Gates.Key32, Lines.Six) => FixingState.Exalted,
                (Gates.Key33, Lines.Four) => FixingState.Exalted,
                (Gates.Key33, Lines.Five) => FixingState.Exalted,
                (Gates.Key34, Lines.One) => FixingState.Detriment,
                (Gates.Key34, Lines.Four) => FixingState.Exalted,
                (Gates.Key36, Lines.Three) => FixingState.Exalted,
                (Gates.Key36, Lines.Four) => FixingState.Exalted,
                (Gates.Key36, Lines.Five) => FixingState.Exalted,
                (Gates.Key38, Lines.Two) => FixingState.Exalted,
                (Gates.Key38, Lines.Four) => FixingState.Exalted,
                (Gates.Key38, Lines.Five) => FixingState.Detriment,
                (Gates.Key40, Lines.Three) => FixingState.Exalted,
                (Gates.Key41, Lines.Six) => FixingState.Detriment,
                (Gates.Key43, Lines.One) => FixingState.Exalted,
                (Gates.Key43, Lines.Two) => FixingState.Exalted,
                (Gates.Key43, Lines.Three) => FixingState.Exalted,
                (Gates.Key44, Lines.One) => FixingState.Exalted,
                (Gates.Key44, Lines.Four) => FixingState.Exalted,
                (Gates.Key44, Lines.Six) => FixingState.Exalted,
                (Gates.Key46, Lines.Four) => FixingState.Detriment,
                (Gates.Key48, Lines.Two) => FixingState.Exalted,
                (Gates.Key49, Lines.Two) => FixingState.Detriment,
                (Gates.Key49, Lines.Three) => FixingState.Detriment,
                (Gates.Key51, Lines.One) => FixingState.Exalted,
                (Gates.Key51, Lines.Six) => FixingState.Detriment,
                (Gates.Key52, Lines.Five) => FixingState.Detriment,
                (Gates.Key53, Lines.Six) => FixingState.Detriment,
                (Gates.Key54, Lines.One) => FixingState.Exalted,
                (Gates.Key54, Lines.Three) => FixingState.Exalted,
                (Gates.Key56, Lines.Six) => FixingState.Detriment,
                (Gates.Key57, Lines.Five) => FixingState.Exalted,
                (Gates.Key58, Lines.Four) => FixingState.Exalted,
                (Gates.Key59, Lines.Two) => FixingState.Detriment,
                (Gates.Key61, Lines.Six) => FixingState.Exalted,
                (Gates.Key62, Lines.Four) => FixingState.Detriment,
                (Gates.Key63, Lines.Six) => FixingState.Detriment,
                _ => FixingState.None
            },
            Planets.Saturn => (gate, line) switch
            {
                (Gates.Key2, Lines.Two) => FixingState.Exalted,
                (Gates.Key2, Lines.Six) => FixingState.Detriment,
                (Gates.Key4, Lines.Four) => FixingState.Detriment,
                (Gates.Key8, Lines.Three) => FixingState.Detriment,
                (Gates.Key10, Lines.Six) => FixingState.Detriment,
                (Gates.Key12, Lines.Two) => FixingState.Exalted,
                (Gates.Key15, Lines.Four) => FixingState.Detriment,
                (Gates.Key18, Lines.Five) => FixingState.Exalted,
                (Gates.Key20, Lines.Five) => FixingState.Exalted,
                (Gates.Key22, Lines.Three) => FixingState.Exalted,
                (Gates.Key24, Lines.Four) => FixingState.Exalted,
                (Gates.Key26, Lines.Three) => FixingState.Detriment,
                (Gates.Key26, Lines.Four) => FixingState.Detriment,
                (Gates.Key27, Lines.Five) => FixingState.Detriment,
                (Gates.Key28, Lines.Three) => FixingState.Exalted,
                (Gates.Key29, Lines.Four) => FixingState.Exalted,
                (Gates.Key32, Lines.Four) => FixingState.Detriment,
                (Gates.Key34, Lines.One) => FixingState.Exalted,
                (Gates.Key34, Lines.Three) => FixingState.Exalted,
                (Gates.Key35, Lines.Six) => FixingState.Exalted,
                (Gates.Key36, Lines.Six) => FixingState.Detriment,
                (Gates.Key37, Lines.Four) => FixingState.Detriment,
                (Gates.Key38, Lines.Five) => FixingState.Exalted,
                (Gates.Key38, Lines.Six) => FixingState.Exalted,
                (Gates.Key41, Lines.Two) => FixingState.Exalted,
                (Gates.Key41, Lines.Three) => FixingState.Exalted,
                (Gates.Key41, Lines.Six) => FixingState.Exalted,
                (Gates.Key42, Lines.Six) => FixingState.Detriment,
                (Gates.Key46, Lines.Six) => FixingState.Exalted,
                (Gates.Key47, Lines.One) => FixingState.Exalted,
                (Gates.Key47, Lines.Two) => FixingState.Exalted,
                (Gates.Key47, Lines.Four) => FixingState.Exalted,
                (Gates.Key49, Lines.Six) => FixingState.Detriment,
                (Gates.Key50, Lines.Four) => FixingState.Exalted,
                (Gates.Key50, Lines.Five) => FixingState.Exalted,
                (Gates.Key52, Lines.Three) => FixingState.Exalted,
                (Gates.Key52, Lines.Four) => FixingState.Exalted,
                (Gates.Key54, Lines.Two) => FixingState.Exalted,
                (Gates.Key54, Lines.Six) => FixingState.Exalted,
                (Gates.Key55, Lines.Three) => FixingState.Exalted,
                (Gates.Key55, Lines.Six) => FixingState.Exalted,
                (Gates.Key59, Lines.Three) => FixingState.Exalted,
                (Gates.Key60, Lines.Two) => FixingState.Exalted,
                (Gates.Key60, Lines.Three) => FixingState.Exalted,
                (Gates.Key61, Lines.Four) => FixingState.Exalted,
                (Gates.Key61, Lines.Five) => FixingState.Exalted,
                (Gates.Key62, Lines.Two) => FixingState.Exalted,
                (Gates.Key62, Lines.Six) => FixingState.Exalted,
                (Gates.Key63, Lines.Three) => FixingState.Detriment,
                (Gates.Key64, Lines.Three) => FixingState.Exalted,
                _ => FixingState.None
            },
            Planets.Mercury => (gate, line) switch
            {
                (Gates.Key2, Lines.Five) => FixingState.Exalted,
                (Gates.Key2, Lines.Six) => FixingState.Exalted,
                (Gates.Key3, Lines.One) => FixingState.Detriment,
                (Gates.Key4, Lines.Six) => FixingState.Exalted,
                (Gates.Key6, Lines.One) => FixingState.Detriment,
                (Gates.Key6, Lines.Six) => FixingState.Exalted,
                (Gates.Key7, Lines.One) => FixingState.Detriment,
                (Gates.Key7, Lines.Two) => FixingState.Detriment,
                (Gates.Key7, Lines.Three) => FixingState.Detriment,
                (Gates.Key7, Lines.Six) => FixingState.Exalted,
                (Gates.Key8, Lines.One) => FixingState.Detriment,
                (Gates.Key8, Lines.Four) => FixingState.Detriment,
                (Gates.Key10, Lines.Two) => FixingState.Exalted,
                (Gates.Key10, Lines.Four) => FixingState.Detriment,
                (Gates.Key11, Lines.Five) => FixingState.Detriment,
                (Gates.Key12, Lines.Two) => FixingState.Detriment,
                (Gates.Key12, Lines.Four) => FixingState.Detriment,
                (Gates.Key13, Lines.Six) => FixingState.Detriment,
                (Gates.Key14, Lines.One) => FixingState.Detriment,
                (Gates.Key15, Lines.Three) => FixingState.Detriment,
                (Gates.Key16, Lines.One) => FixingState.Detriment,
                (Gates.Key16, Lines.Two) => FixingState.Detriment,
                (Gates.Key18, Lines.Four) => FixingState.Detriment,
                (Gates.Key19, Lines.Two) => FixingState.Detriment,
                (Gates.Key20, Lines.Four) => FixingState.Detriment,
                (Gates.Key20, Lines.Six) => FixingState.Detriment,
                (Gates.Key25, Lines.One) => FixingState.Detriment,
                (Gates.Key25, Lines.Two) => FixingState.Exalted,
                (Gates.Key28, Lines.Four) => FixingState.Detriment,
                (Gates.Key31, Lines.Two) => FixingState.Detriment,
                (Gates.Key32, Lines.Three) => FixingState.Exalted,
                (Gates.Key34, Lines.Three) => FixingState.Detriment,
                (Gates.Key35, Lines.Five) => FixingState.Exalted,
                (Gates.Key36, Lines.Five) => FixingState.Detriment,
                (Gates.Key37, Lines.Two) => FixingState.Detriment,
                (Gates.Key37, Lines.Six) => FixingState.Detriment,
                (Gates.Key39, Lines.One) => FixingState.Detriment,
                (Gates.Key41, Lines.One) => FixingState.Detriment,
                (Gates.Key43, Lines.Four) => FixingState.Exalted,
                (Gates.Key47, Lines.Two) => FixingState.Detriment,
                (Gates.Key48, Lines.Three) => FixingState.Detriment,
                (Gates.Key50, Lines.Three) => FixingState.Detriment,
                (Gates.Key51, Lines.Two) => FixingState.Detriment,
                (Gates.Key51, Lines.Four) => FixingState.Detriment,
                (Gates.Key56, Lines.Four) => FixingState.Detriment,
                (Gates.Key57, Lines.Three) => FixingState.Exalted,
                (Gates.Key58, Lines.Six) => FixingState.Detriment,
                (Gates.Key59, Lines.One) => FixingState.Detriment,
                (Gates.Key59, Lines.Four) => FixingState.Detriment,
                (Gates.Key59, Lines.Six) => FixingState.Detriment,
                (Gates.Key60, Lines.One) => FixingState.Detriment,
                (Gates.Key60, Lines.Four) => FixingState.Exalted,
                (Gates.Key60, Lines.Six) => FixingState.Detriment,
                (Gates.Key62, Lines.Two) => FixingState.Detriment,
                (Gates.Key62, Lines.Six) => FixingState.Detriment,
                (Gates.Key63, Lines.Four) => FixingState.Exalted,
                (Gates.Key64, Lines.Six) => FixingState.Exalted,
                _ => FixingState.None
            },
            Planets.Neptune => (gate, line) switch
            {
                (Gates.Key3, Lines.Four) => FixingState.Exalted,
                (Gates.Key5, Lines.Three) => FixingState.Exalted,
                (Gates.Key5, Lines.Six) => FixingState.Exalted,
                (Gates.Key6, Lines.Three) => FixingState.Exalted,
                (Gates.Key7, Lines.Two) => FixingState.Exalted,
                (Gates.Key7, Lines.Five) => FixingState.Detriment,
                (Gates.Key8, Lines.One) => FixingState.Exalted,
                (Gates.Key11, Lines.Two) => FixingState.Exalted,
                (Gates.Key11, Lines.Six) => FixingState.Exalted,
                (Gates.Key12, Lines.Three) => FixingState.Exalted,
                (Gates.Key13, Lines.Five) => FixingState.Exalted,
                (Gates.Key14, Lines.Three) => FixingState.Detriment,
                (Gates.Key16, Lines.Six) => FixingState.Exalted,
                (Gates.Key18, Lines.Three) => FixingState.Exalted,
                (Gates.Key21, Lines.Two) => FixingState.Detriment,
                (Gates.Key21, Lines.Three) => FixingState.Exalted,
                (Gates.Key22, Lines.Four) => FixingState.Exalted,
                (Gates.Key24, Lines.One) => FixingState.Detriment,
                (Gates.Key24, Lines.Four) => FixingState.Detriment,
                (Gates.Key25, Lines.One) => FixingState.Exalted,
                (Gates.Key26, Lines.One) => FixingState.Exalted,
                (Gates.Key28, Lines.Six) => FixingState.Detriment,
                (Gates.Key29, Lines.One) => FixingState.Detriment,
                (Gates.Key32, Lines.Six) => FixingState.Detriment,
                (Gates.Key33, Lines.Two) => FixingState.Detriment,
                (Gates.Key33, Lines.Four) => FixingState.Detriment,
                (Gates.Key35, Lines.One) => FixingState.Detriment,
                (Gates.Key36, Lines.Two) => FixingState.Exalted,
                (Gates.Key38, Lines.One) => FixingState.Exalted,
                (Gates.Key39, Lines.Five) => FixingState.Exalted,
                (Gates.Key41, Lines.One) => FixingState.Exalted,
                (Gates.Key44, Lines.Three) => FixingState.Detriment,
                (Gates.Key45, Lines.Three) => FixingState.Exalted,
                (Gates.Key46, Lines.One) => FixingState.Exalted,
                (Gates.Key46, Lines.Five) => FixingState.Detriment,
                (Gates.Key46, Lines.Six) => FixingState.Detriment,
                (Gates.Key47, Lines.One) => FixingState.Detriment,
                (Gates.Key49, Lines.Three) => FixingState.Exalted,
                (Gates.Key49, Lines.Six) => FixingState.Exalted,
                (Gates.Key52, Lines.Six) => FixingState.Detriment,
                (Gates.Key53, Lines.One) => FixingState.Exalted,
                (Gates.Key53, Lines.Five) => FixingState.Exalted,
                (Gates.Key58, Lines.Four) => FixingState.Detriment,
                (Gates.Key60, Lines.Five) => FixingState.Exalted,
                (Gates.Key61, Lines.One) => FixingState.Exalted,
                (Gates.Key62, Lines.One) => FixingState.Exalted,
                (Gates.Key62, Lines.Five) => FixingState.Detriment,
                _ => FixingState.None
            },
            Planets.Sun => (gate, line) switch
            {
                (Gates.Key3, Lines.Six) => FixingState.Exalted,
                (Gates.Key4, Lines.Four) => FixingState.Exalted,
                (Gates.Key5, Lines.Four) => FixingState.Detriment,
                (Gates.Key6, Lines.Four) => FixingState.Exalted,
                (Gates.Key7, Lines.Four) => FixingState.Exalted,
                (Gates.Key8, Lines.Two) => FixingState.Exalted,
                (Gates.Key8, Lines.Five) => FixingState.Detriment,
                (Gates.Key9, Lines.Three) => FixingState.Detriment,
                (Gates.Key10, Lines.One) => FixingState.Exalted,
                (Gates.Key11, Lines.Four) => FixingState.Detriment,
                (Gates.Key12, Lines.Five) => FixingState.Exalted,
                (Gates.Key12, Lines.Six) => FixingState.Exalted,
                (Gates.Key13, Lines.Two) => FixingState.Detriment,
                (Gates.Key14, Lines.Five) => FixingState.Exalted,
                (Gates.Key14, Lines.Six) => FixingState.Exalted,
                (Gates.Key15, Lines.Two) => FixingState.Exalted,
                (Gates.Key16, Lines.Two) => FixingState.Exalted,
                (Gates.Key17, Lines.Two) => FixingState.Exalted,
                (Gates.Key19, Lines.One) => FixingState.Exalted,
                (Gates.Key20, Lines.Three) => FixingState.Exalted,
                (Gates.Key22, Lines.Two) => FixingState.Exalted,
                (Gates.Key22, Lines.Six) => FixingState.Exalted,
                (Gates.Key23, Lines.Three) => FixingState.Exalted,
                (Gates.Key23, Lines.Four) => FixingState.Exalted,
                (Gates.Key24, Lines.One) => FixingState.Exalted,
                (Gates.Key26, Lines.Two) => FixingState.Exalted,
                (Gates.Key26, Lines.Three) => FixingState.Exalted,
                (Gates.Key26, Lines.Six) => FixingState.Exalted,
                (Gates.Key27, Lines.One) => FixingState.Exalted,
                (Gates.Key28, Lines.Two) => FixingState.Exalted,
                (Gates.Key28, Lines.Five) => FixingState.Detriment,
                (Gates.Key29, Lines.Two) => FixingState.Exalted,
                (Gates.Key29, Lines.Five) => FixingState.Exalted,
                (Gates.Key30, Lines.One) => FixingState.Exalted,
                (Gates.Key30, Lines.Two) => FixingState.Exalted,
                (Gates.Key31, Lines.One) => FixingState.Exalted,
                (Gates.Key31, Lines.Three) => FixingState.Exalted,
                (Gates.Key31, Lines.Six) => FixingState.Exalted,
                (Gates.Key32, Lines.One) => FixingState.Exalted,
                (Gates.Key33, Lines.One) => FixingState.Exalted,
                (Gates.Key33, Lines.Six) => FixingState.Exalted,
                (Gates.Key35, Lines.Three) => FixingState.Detriment,
                (Gates.Key38, Lines.Three) => FixingState.Exalted,
                (Gates.Key39, Lines.Four) => FixingState.Detriment,
                (Gates.Key40, Lines.One) => FixingState.Exalted,
                (Gates.Key40, Lines.Two) => FixingState.Exalted,
                (Gates.Key40, Lines.Six) => FixingState.Exalted,
                (Gates.Key42, Lines.One) => FixingState.Exalted,
                (Gates.Key42, Lines.Two) => FixingState.Exalted,
                (Gates.Key42, Lines.Five) => FixingState.Exalted,
                (Gates.Key43, Lines.Six) => FixingState.Exalted,
                (Gates.Key44, Lines.Four) => FixingState.Detriment,
                (Gates.Key46, Lines.Two) => FixingState.Exalted,
                (Gates.Key47, Lines.Six) => FixingState.Detriment,
                (Gates.Key48, Lines.Four) => FixingState.Exalted,
                (Gates.Key49, Lines.One) => FixingState.Detriment,
                (Gates.Key50, Lines.Two) => FixingState.Exalted,
                (Gates.Key51, Lines.Three) => FixingState.Exalted,
                (Gates.Key51, Lines.Five) => FixingState.Exalted,
                (Gates.Key51, Lines.Six) => FixingState.Exalted,
                (Gates.Key54, Lines.Five) => FixingState.Exalted,
                (Gates.Key55, Lines.Five) => FixingState.Detriment,
                (Gates.Key56, Lines.Three) => FixingState.Exalted,
                (Gates.Key56, Lines.Six) => FixingState.Exalted,
                (Gates.Key58, Lines.Five) => FixingState.Detriment,
                (Gates.Key59, Lines.One) => FixingState.Exalted,
                (Gates.Key59, Lines.Five) => FixingState.Exalted,
                (Gates.Key63, Lines.One) => FixingState.Exalted,
                (Gates.Key63, Lines.Five) => FixingState.Exalted,
                _ => FixingState.None
            },
            _ => FixingState.None
        };
    }
    
    private static FixingState _aggregateStateFromHarmonicGate(Gates gate, Lines line, Gates harmonicGate, 
        Dictionary<Planets, Activation> activations)
    {
        var state = FixingState.None;
        var states = activations
            .Where(x => x.Value.Gate == harmonicGate)
            .Select(x => _getStateFromStatesTable(x.Key, gate, line))
            .ToArray();
        if (states.Length != 0)
        {
            state |= states.Aggregate((a, b) => a | b);
        }

        return state;
    }

    internal static PlanetaryFixation CalculateState(Planets planet,
        Dictionary<Planets, Activation> activations,
        Dictionary<Planets, Activation> comparerActivations)
    {
        var result = new PlanetaryFixation();
        var activation = activations[planet];
        result.FixingState = _getStateFromStatesTable(planet, activation.Gate, activation.Line);
        foreach (var harmonicGate in activation.Gate.HarmonicGates())
        {
            result.FixingState |= _aggregateStateFromHarmonicGate(activation.Gate, activation.Line, harmonicGate, activations);
            var fixingState = _aggregateStateFromHarmonicGate(activation.Gate, activation.Line, harmonicGate, comparerActivations);
            if (result.FixingState.HasFlag(fixingState)) continue;
            result.FixingState |= fixingState;
            result.FixingStateChangedByComparer = true;
        }

        return result;
    }

    internal static PlanetaryFixation CalculateState(Planets planet,
        Dictionary<Planets, Activation> activations1,
        Dictionary<Planets, Activation> activations2,
        Dictionary<Planets, Activation> activations3,
        bool firstComparator = false)
    {
        var result = new PlanetaryFixation();
        var activation = activations1[planet];
        result.FixingState = _getStateFromStatesTable(planet, activation.Gate, activation.Line);
        if (firstComparator)
        {
            foreach (var harmonicGate in activation.Gate.HarmonicGates())
            {
                result.FixingState |= _aggregateStateFromHarmonicGate(activation.Gate, activation.Line, harmonicGate, activations1);
                var fixingState = _aggregateStateFromHarmonicGate(activation.Gate, activation.Line, harmonicGate, activations2);
                fixingState |= _aggregateStateFromHarmonicGate(activation.Gate, activation.Line, harmonicGate, activations3);
                if (result.FixingState.HasFlag(fixingState)) continue;
                result.FixingState |= fixingState;
                result.FixingStateChangedByComparer = true;
            }

            return result;
        }
        foreach (var harmonicGate in activation.Gate.HarmonicGates())
        {
            result.FixingState |= _aggregateStateFromHarmonicGate(activation.Gate, activation.Line, harmonicGate, activations1);
            result.FixingState |= _aggregateStateFromHarmonicGate(activation.Gate, activation.Line, harmonicGate, activations2);
            var fixingState = _aggregateStateFromHarmonicGate(activation.Gate, activation.Line, harmonicGate, activations3);
            if (result.FixingState.HasFlag(fixingState)) continue;
            result.FixingState |= fixingState;
            result.FixingStateChangedByComparer = true;
        }
        return result;
    }

    internal static PlanetaryFixation CalculateState(Planets planet,
        Dictionary<Planets, Activation> activations1,
        Dictionary<Planets, Activation> activations2,
        Dictionary<Planets, Activation> comparatorActivations1,
        Dictionary<Planets, Activation> comparatorActivations2)
    {
        var result = new PlanetaryFixation();
        var activation = activations1[planet];
        result.FixingState = _getStateFromStatesTable(planet, activation.Gate, activation.Line);
        foreach (var harmonicGate in activation.Gate.HarmonicGates())
        {
            result.FixingState |= _aggregateStateFromHarmonicGate(activation.Gate, activation.Line, harmonicGate, activations1);
            result.FixingState |= _aggregateStateFromHarmonicGate(activation.Gate, activation.Line, harmonicGate, activations2);
            var fixingState = _aggregateStateFromHarmonicGate(activation.Gate, activation.Line, harmonicGate, comparatorActivations1);
            fixingState |= _aggregateStateFromHarmonicGate(activation.Gate, activation.Line, harmonicGate, comparatorActivations2);
            if (result.FixingState.HasFlag(fixingState)) continue;
            result.FixingState |= fixingState;
            result.FixingStateChangedByComparer = true;
        }
        
        return result;
    }

    internal static Dictionary<Gates, ActivationTypes> GateActivations(IEnumerable<Gates> gates1, IEnumerable<Gates> gates2)
    {
        var gateActivation = Enum.GetValues<Gates>().ToDictionary(x => x, x => ActivationTypes.None);
        foreach (var gate in gates1)
        {
            gateActivation[gate] = ActivationTypes.FirstComparator;
        }

        foreach (var gate in gates2)
        {
            if (gateActivation[gate] == ActivationTypes.FirstComparator)
            {
                gateActivation[gate] = ActivationTypes.Mixed;
            }
            else
            {
                gateActivation[gate] = ActivationTypes.SecondComparator;
            }
        }

        return gateActivation;
    }

    internal static Dictionary<Channels, ChannelActivationType> CompositeChannelActivations(
        HashSet<Gates> gates1, HashSet<Gates> gates2)
    {
        var result = new Dictionary<Channels, ChannelActivationType>();
        int gate1State;
        int gate2State;
        foreach (var channel in Enum.GetValues<Channels>())
        {
            gate1State = 0;
            gate2State = 0;
            var (gate1, gate2) = channel.ToGates();
            if (gates1.Contains(gate1)) gate1State = 1;
            if (gates2.Contains(gate1)) gate1State = gate1State == 1 ? 3 : 2;
            if (gates1.Contains(gate2)) gate2State = 1;
            if (gates2.Contains(gate2)) gate2State = gate2State == 1 ? 3 : 2;

            if (gate1State == 0 || gate2State == 0)
            {
                result[channel] = ChannelActivationType.None;
                continue;
            }
            if (gate1State == gate2State)
            {
                switch (gate1State)
                {
                    case 1:
                        result[channel] = ChannelActivationType.FirstDominating;
                        continue;
                    case 2:
                        result[channel] = ChannelActivationType.SecondDominating;
                        continue;
                    case 3:
                        result[channel] = ChannelActivationType.Companion;
                        continue;
                }
            }
            if (gate1State == 3)
            {
                switch (gate2State)
                {
                    case 1:
                        result[channel] = ChannelActivationType.CompromiseFirstDominating;
                        continue;
                    case 2:
                        result[channel] = ChannelActivationType.CompromiseSecondDominating;
                        continue;
                }
            }
            if (gate2State == 3)
            {
                switch (gate1State)
                {
                    case 1:
                        result[channel] = ChannelActivationType.CompromiseFirstDominating;
                        continue;
                    case 2:
                        result[channel] = ChannelActivationType.CompromiseSecondDominating;
                        continue;
                }
            }
            
            result[channel] = ChannelActivationType.Magnetic;
        }

        return result;
    }
    
    internal static Dictionary<Centers, ActivationTypes> CenterActivations(
        Dictionary<Centers, int> connectedComponents, Dictionary<Channels, ChannelActivationType> channelActivations)
    {
        var result = new Dictionary<Centers, ActivationTypes>();
        foreach (var center in Enum.GetValues<Centers>())
        {
            if (!connectedComponents.ContainsKey(center))
            {
                result[center] = ActivationTypes.None;
                continue;
            }

            ActivationTypes? activationType = null;
            foreach (var channel in center.ConnectedChannels())
            {
                if (!channelActivations.TryGetValue(channel, out var activation))
                {
                    continue;
                }

                if (activation == ChannelActivationType.FirstDominating)
                {
                    if (activationType == ActivationTypes.SecondComparator)
                    {
                        activationType = ActivationTypes.Mixed;
                        break;
                    }

                    activationType = ActivationTypes.FirstComparator;
                    continue;
                }

                if (activation == ChannelActivationType.SecondDominating)
                {
                    if (activationType == ActivationTypes.FirstComparator)
                    {
                        activationType = ActivationTypes.Mixed;
                        break;
                    }
                    activationType = ActivationTypes.SecondComparator;
                    continue;
                }
                
                activationType = ActivationTypes.Mixed;
                break;
            }

            result[center] = activationType!.Value;
        }

        return result;
    }

}