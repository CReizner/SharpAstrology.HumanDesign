using SharpAstrology.Enums;
using SharpAstrology.DataModels;

namespace SharpAstrology.ExtensionMethods;

public static class HumanDesignChartExtensionMethods
{
    /// <summary>
    /// Retrieves the activation type for a given gate. 
    /// It looks for any celestial object that was in this gate on design time or birth time.
    /// </summary>
    public static ActivationTypes ActivationType(this HumanDesignChart chart, Gates key)
    {
        var isActivatedConscious = chart.PersonalityActivation.Any(p => p.Value.Gate == key);
        var isActivatedUnconscious = chart.DesignActivation.Any(p => p.Value.Gate == key);
        
        return isActivatedConscious switch
        {
            true when !isActivatedUnconscious => ActivationTypes.Personality,
            true when isActivatedUnconscious => ActivationTypes.Mixed,
            false when isActivatedUnconscious => ActivationTypes.Design,
            _ => ActivationTypes.None
        };
    }
    
    /// <summary>
    /// Retrieves the activation type for a given channel. It looks for the activation types on both gates.
    /// </summary>
    public static ActivationTypes ActivationType(this HumanDesignChart chart, Channels channel)
    {
        var (key1, key2) = channel.ToGates();

        return (chart.ActivationType(key1), chart.ActivationType(key2)) switch
        {
            (_, ActivationTypes.None) => ActivationTypes.None,
            (ActivationTypes.None, _) => ActivationTypes.None,
            (ActivationTypes.Mixed, _) => ActivationTypes.Mixed,
            (_, ActivationTypes.Mixed) => ActivationTypes.Mixed,
            (ActivationTypes.Personality, ActivationTypes.Design) => ActivationTypes.Mixed,
            (ActivationTypes.Design, ActivationTypes.Personality) => ActivationTypes.Mixed,
            (ActivationTypes.Personality, ActivationTypes.Personality) => ActivationTypes.Personality,
            (ActivationTypes.Design, ActivationTypes.Design) => ActivationTypes.Design,
            _ => ActivationTypes.None
        };
    }
    
    public static bool IsActivated(this HumanDesignChart chart, Centers center) => chart.ConnectedComponents.ContainsKey(center);
    
    public static bool IsActivated(this HumanDesignChart chart, Gates key) => chart.ActivationType(key) != ActivationTypes.None;
    
    public static bool IsActivated(this HumanDesignChart chart, Channels channel) => chart.ActivationType(channel) != ActivationTypes.None;
    
    public static Profiles Profile(this HumanDesignChart chart)
    {
        return (chart.PersonalityActivation[Planets.Sun].Line, chart.DesignActivation[Planets.Sun].Line).ToProfile();
    }

    public static SplitDefinitions SplitDefinition(this HumanDesignChart chart)
    {
        return chart.Splits switch
        {
            0 => SplitDefinitions.Empty,
            1 => SplitDefinitions.SingleDefinition,
            2 => SplitDefinitions.SplitDefinition,
            3 => SplitDefinitions.TripleSplit,
            4 => SplitDefinitions.QuadrupleSplit,
            _ => throw new ArgumentException($"To much splits: {chart.Splits}")
        };
    }
    
    public static Types Type(this HumanDesignChart chart)
    {
        //No components => no active centers => Reflector
        if (chart.Splits == 0) return Types.Reflector;
                
        //Case: Sacral is undefined (can be Manifestor or Projector)
        if (!chart.ConnectedComponents.ContainsKey(Centers.Sacral))
        {
            //If Throat is undefined, it must be a Projector
            if (!chart.ConnectedComponents.ContainsKey(Centers.Throat)) return Types.Projector;
                    
            //Check connections from motor centers to throat ( => Manifestor)
            var component = chart.ConnectedComponents[Centers.Throat];
            if (chart.ConnectedComponents.ContainsKey(Centers.Heart))
            {
                if (chart.ConnectedComponents[Centers.Heart] == component) return Types.Manifestor;
            }

            if (chart.ConnectedComponents.ContainsKey(Centers.Emotions))
            {
                if (chart.ConnectedComponents[Centers.Emotions] == component) return Types.Manifestor;
            }

            if (chart.ConnectedComponents.ContainsKey(Centers.Root))
            {
                if (chart.ConnectedComponents[Centers.Root] == component) return Types.Manifestor;
            }
                    
            //If no motor center is connected to the throat, it is a Projector by definition
            return Types.Projector;
        }
        //Case: Sacral is defined (can be Generator or Manifesting Generator)
        if (!chart.ConnectedComponents.ContainsKey(Centers.Throat)) return Types.Generator;
                
        //Check connections from motor centers to throat ( => Manifesting Generator)
        if (chart.ConnectedComponents.ContainsKey(Centers.Heart))
        {
            if (chart.ConnectedComponents[Centers.Heart] == chart.ConnectedComponents[Centers.Throat]) return Types.ManifestingGenerator;
        }

        if (chart.ConnectedComponents.ContainsKey(Centers.Emotions))
        {
            if (chart.ConnectedComponents[Centers.Emotions] == chart.ConnectedComponents[Centers.Throat]) return Types.ManifestingGenerator;
        }

        if (chart.ConnectedComponents.ContainsKey(Centers.Root))
        {
            if (chart.ConnectedComponents[Centers.Root] == chart.ConnectedComponents[Centers.Throat]) return Types.ManifestingGenerator;
        }
        if (chart.ConnectedComponents[Centers.Sacral] == chart.ConnectedComponents[Centers.Throat]) return Types.ManifestingGenerator;
                
        //No connection to throat => Generator
        return Types.Generator;
    }
    
    public static Strategies Strategy(this HumanDesignChart chart)
    {
        if (chart.ConnectedComponents.ContainsKey(Centers.Emotions)) return Strategies.Emotional;
        if (chart.ConnectedComponents.ContainsKey(Centers.Sacral)) return Strategies.Sacral;
        if (chart.ConnectedComponents.ContainsKey(Centers.Spleen)) return Strategies.Spleen;
        if (chart.ConnectedComponents.ContainsKey(Centers.Heart))
        {
            if (chart.ConnectedComponents.ContainsKey(Centers.Throat))
            {
                return chart.ConnectedComponents[Centers.Heart] == chart.ConnectedComponents[Centers.Throat] ? Strategies.Heart : Strategies.Self;
            }
        }

        return chart.ConnectedComponents.ContainsKey(Centers.Self) ? Strategies.Self : Strategies.Outer;
    }
    
    public static HashSet<Gates> ActiveGates(this HumanDesignChart chart)
    {
        return Utility.HumanDesignUtility.ActiveGates(chart.PersonalityActivation, chart.DesignActivation);
    }
    
    public static HashSet<Channels> ActiveChannels(this HumanDesignChart chart)
    {
        return Utility.HumanDesignUtility.ActiveChannels(chart.ActiveGates);
    }

    public static Variables Variables(this HumanDesignChart chart)
    {
        return new Variables
        {
            Digestion = (
                chart.DesignActivation[Planets.Sun].Tone.ToOrientation(),
                chart.DesignActivation[Planets.Sun].Color, 
                chart.DesignActivation[Planets.Sun].Tone,
                chart.DesignActivation[Planets.Sun].Base),
            Perspective = (
                chart.PersonalityActivation[Planets.Sun].Tone.ToOrientation(),
                chart.PersonalityActivation[Planets.Sun].Color, 
                chart.PersonalityActivation[Planets.Sun].Tone,
                chart.PersonalityActivation[Planets.Sun].Base),
            Environment = (
                chart.DesignActivation[Planets.NorthNode].Tone.ToOrientation(),
                chart.DesignActivation[Planets.NorthNode].Color, 
                chart.DesignActivation[Planets.NorthNode].Tone,
                chart.DesignActivation[Planets.NorthNode].Base),
            Awareness = (
                chart.PersonalityActivation[Planets.NorthNode].Tone.ToOrientation(),
                chart.PersonalityActivation[Planets.NorthNode].Color, 
                chart.PersonalityActivation[Planets.NorthNode].Tone,
                chart.PersonalityActivation[Planets.NorthNode].Base),
        };
    }

    public static IncarnationCrosses IncarnationCross(this HumanDesignChart chart)
    {
        return (chart.PersonalityActivation[Planets.Sun].Gate, chart.Profile.ToAngle())
            .ToIncarnationCross();
    }
}