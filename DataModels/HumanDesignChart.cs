using SharpAstrology.Interfaces;
using SharpAstrology.Enums;
using SharpAstrology.ExtensionMethods;
using SharpAstrology.HumanDesign.Mathematics;


namespace SharpAstrology.DataModels;

/// <summary>
/// Represents a Human Design Chart. It utilizes planetary positions and activations
/// to provide several complex characteristics like Types, Profiles, Strategies, Channels, Gates and Variables.
/// </summary>
public sealed class HumanDesignChart
{
    /// <summary>
    /// Gets a dictionary of personality activations corresponding to each celestial body. 
    /// </summary>
    public Dictionary<Planets, Activation> PersonalityActivation { get; }
    
    /// <summary>
    /// Gets a dictionary of design activations corresponding to each celestial body. 
    /// </summary>
    public Dictionary<Planets, Activation> DesignActivation { get; }
    
    /// <summary>
    /// Gets a dictionary of connected components, where each center is associated with its components id.
    /// </summary>
    public Dictionary<Centers, int> ConnectedComponents { get; }
    
    /// <summary>
    /// Gets the number of connected components of the Human Design graph.
    /// </summary>
    public int Splits { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="HumanDesignChart"/> class. It calculates the design date itself.
    /// </summary>
    /// <param name="dateOfBirth">The date of birth for which the Human Design Chart is being created.</param>
    /// <param name="eph">The ephemerides used for planetary positions calculations.</param>
    /// <param name="mode">Tropical or sidereal calculation mode. Default is tropical.</param>

    public HumanDesignChart(DateTime dateOfBirth, IEphemerides eph, EphCalculationMode mode = EphCalculationMode.Tropic) 
        : this(dateOfBirth, eph.DesignJulianDay(dateOfBirth, mode), eph, mode) { }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="HumanDesignChart"/> class.
    /// </summary>
    /// <param name="dateOfBirth">The date of birth for which the Human Design Chart is being created.</param>
    /// <param name="designDate">The corresponding design date.</param>
    /// <param name="eph">The ephemerides used for planetary positions calculations.</param>
    /// <param name="mode">Tropical or sidereal calculation mode. Default is tropical.</param>
    public HumanDesignChart(DateTime dateOfBirth, DateTime designDate, IEphemerides eph, EphCalculationMode mode = EphCalculationMode.Tropic)
    {
        PersonalityActivation = Definitions.HumanDesignDefaults.HumanDesignPlanets.ToDictionary(
            p => p,
            p => Utility.HumanDesignUtility.ActivationOf(eph.PlanetsPosition(p, dateOfBirth, mode).Longitude));
        DesignActivation = Definitions.HumanDesignDefaults.HumanDesignPlanets.ToDictionary(
            p => p,
            p => Utility.HumanDesignUtility.ActivationOf(eph.PlanetsPosition(p, designDate, mode).Longitude));
        Utility.HumanDesignUtility.CalculateState(PersonalityActivation, DesignActivation);
        var activeGates = Utility.HumanDesignUtility
            .ActiveGates(PersonalityActivation, DesignActivation);
        (ConnectedComponents, Splits) = GraphService.ConnectedCenters(Utility.HumanDesignUtility.ActiveChannels(activeGates));
    }
    
    private Profiles? _profile;
    /// <summary>
    /// Gets the profile associated with this chart. 
    /// If the value has already been calculated, it retrieves the calculated value. 
    /// If it hasn't been calculated yet, it will call the associated extension method <see cref="HumanDesignChartExtensionMethods.Profile"/>
    /// to calculate its value.
    /// </summary>
    public Profiles Profile
    {
        get
        {
            _profile ??= _Profile();
            return _profile!.Value;
        }
    }

    private Types? _types;
    /// <summary>
    /// Gets the type associated with this chart.
    /// If the value has already been calculated, it retrieves the calculated value.
    /// If it hasn't been calculated yet, it will call the associated extension method <see cref="HumanDesignChartExtensionMethods.Type"/>
    /// to calculate its value.
    /// </summary>
    public Types Type
    {
        get
        {
            _types ??= _Type();
            return _types!.Value;
        }
    }

    private Strategies? _strategy;
    /// <summary>
    /// Gets the strategy associated with this chart.
    /// If the value has already been calculated, it retrieves the calculated value.
    /// If it hasn't been calculated yet, it will call the associated extension method <see cref="HumanDesignChartExtensionMethods.Strategy"/>
    /// to calculate its value.
    /// </summary>
    public Strategies Strategy
    {
        get
        {
            _strategy ??= _Strategy();
            return _strategy!.Value;
        }
    }

    private SplitDefinitions? _splitDefinition;
    /// <summary>
    /// Gets the spilt definition associated with this chart.
    /// If the value has already been calculated, it retrieves the calculated value.
    /// If it hasn't been calculated yet, it will call the associated extension method <see cref="HumanDesignChartExtensionMethods.SplitDefinition"/>
    /// to calculate its value.
    /// </summary>
    public SplitDefinitions SplitDefinition
    {
        get
        {
            _splitDefinition ??= _SplitDefinition();
            return _splitDefinition!.Value;
        }
    }

    private HashSet<Gates>? _activeGates;
    /// <summary>
    /// Gets the set of active gates of the chart.
    /// If the value has already been calculated, it retrieves the calculated value.
    /// If it hasn't been calculated yet, it will call the associated extension method <see cref="HumanDesignChartExtensionMethods.ActiveGates"/>
    /// to calculate the value.
    /// </summary>
    public HashSet<Gates> ActiveGates
    {
        get
        {
            _activeGates ??= _ActiveGates();
            return _activeGates;
        }
    }
    
    private HashSet<Channels>? _activeChannels;
    /// <summary>
    /// Gets the set of active channels of the chart.
    /// If the value has already been calculated, it retrieves the calculated value.
    /// If it hasn't been calculated yet, it will call the associated extension method <see cref="HumanDesignChartExtensionMethods.ActiveChannels"/>
    /// to calculate the value.
    /// </summary>
    public HashSet<Channels> ActiveChannels
    {
        get
        {
            _activeChannels ??= _ActiveChannels();
            return _activeChannels;
        }
    }

    private IncarnationCrosses? _incarnationCross;
    /// <summary>
    /// Gets the Incarnation Cross associated with this chart.
    /// If the value has already been calculated, it retrieves the calculated value.
    /// If it hasn't been calculated yet, it will call the associated extension method <see cref="HumanDesignChartExtensionMethods.IncarnationCross"/>
    /// to calculate its value.
    /// </summary>
    public IncarnationCrosses IncarnationCross
    {
        get
        {
            _incarnationCross ??= _IncarnationCross();
            return _incarnationCross.Value;
        }
    }
    
    private Variables? _variables;
    /// <summary>
    /// Gets the variables associated with this chart.
    /// If the value has already been calculated, it retrieves the calculated value.
    /// If it hasn't been calculated yet, it will call the associated extension method <see cref="HumanDesignChartExtensionMethods.Variables"/>
    /// to calculate its value.
    /// </summary>
    public Variables Variables
    {
        get
        {
            _variables ??= _Variables();
            return _variables;
        }
    }

    /// <summary>
    /// Determines whether the specified <see cref="HumanDesignChart"/> object is equal to the current object,
    /// considering different levels of comparison depth specified by <see cref="ComparerDepth"/>.
    /// </summary>
    /// <param name="other">The <see cref="HumanDesignChart"/> object to compare with the current object.</param>
    /// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
    public bool Equals(HumanDesignChart? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        
        foreach (var (p,a) in PersonalityActivation)
        {
            if (!PersonalityActivation[p].Equals(other.PersonalityActivation[p], ComparerDepth.Gate)) return false;
        }
        foreach (var (p,a) in DesignActivation)
        {
            if (!DesignActivation[p].Equals(other.DesignActivation[p], ComparerDepth.Gate)) return false;
        }

        if (PersonalityActivation[Planets.Sun].Line != other.PersonalityActivation[Planets.Sun].Line) return false;
        if (DesignActivation[Planets.Sun].Line != other.DesignActivation[Planets.Sun].Line) return false;
        
        return true;
    }

    /// <summary>
    /// Attempts to predict a list of <see cref="HumanDesignChart"/> with probabilities within a given time range.
    /// </summary>
    /// <param name="start">The starting DateTime in UTC to begin predictions.</param>
    /// <param name="end">The ending DateTime in UTC for the range of predictions.</param>
    /// <param name="eph">An IEphemerides instance providing ephemeris data for astrological calculations.</param>
    /// <param name="mode">An optional EphCalculationMode (defaults to Tropic) specifying the astrological calculation mode.</param>
    /// <returns>A List of tuples, each containing a <see cref="HumanDesignChart"/> and its associated probability, representing the predicted outcomes within the specified time range.</returns>
    /// <exception cref="ArgumentException">Thrown if start or end DateTime parameters are not in UTC.</exception>
    public static List<(HumanDesignChart Chart, double Probability)> 
        Guess(DateTime start, DateTime end, IEphemerides eph, EphCalculationMode mode = EphCalculationMode.Tropic)
    {
        if (start.Kind != DateTimeKind.Utc || end.Kind != DateTimeKind.Utc)
            throw new ArgumentException("The parameters must be of kind Utc.");

        var maxDepth = (int)Math.Log2((end - start).TotalMinutes / 10) + 1;

        List<(HumanDesignChart Chart, double Probability)> results = new((int)Math.Pow(2,maxDepth));
        _guess(results, null, null, start, end, 0, maxDepth, eph, mode);
        
        ChartProbability current = null;
        List<ChartProbability> probs = [];
        foreach (var result in results)
        {
            if (current is not null && current.Chart.Equals(result.Chart))
            {
                current.Probability += result.Probability;
                continue;
            }
    
            current = new ChartProbability()
            {
                Chart = result.Chart,
                Probability = result.Probability
            };
            probs.Add(current);
        }

        return probs.Select(x => (x.Chart, x.Probability)).ToList();
    }

    #region private Methods
    
    private static void _guess(List<(HumanDesignChart Chart, double Probability)> results, 
        HumanDesignChart? leftChart,
        HumanDesignChart? rightChart,
        DateTime start, DateTime end,
        int currentDepth, int maxDepth, IEphemerides eph, EphCalculationMode mode = EphCalculationMode.Tropic)
    {
        leftChart ??= new HumanDesignChart(start, eph, mode);
        rightChart ??= new HumanDesignChart(end, eph, mode);
        if (currentDepth > maxDepth || leftChart.Equals(rightChart))
        {
            results.Add((leftChart, 1.0 / Math.Pow(2, currentDepth)));
            return; 
        }
        
        var midPoint = start + (end - start) / 2;
        _guess(results, leftChart, null, start, midPoint, currentDepth + 1, maxDepth, eph, mode);
        _guess(results, null, rightChart, midPoint, end, currentDepth + 1, maxDepth, eph, mode);
    }
    
    /// <summary>
    /// Retrieves the activation type for a given gate. 
    /// It looks for any celestial object that was in this gate on design time or birth time.
    /// </summary>
    private ActivationTypes _ActivationType(Gates key)
    {
        var isActivatedConscious = PersonalityActivation.Any(p => p.Value.Gate == key);
        var isActivatedUnconscious = DesignActivation.Any(p => p.Value.Gate == key);
        
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
    private ActivationTypes _ActivationType(Channels channel)
    {
        var (key1, key2) = channel.ToGates();

        return (_ActivationType(key1), _ActivationType(key2)) switch
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
    
    private bool _IsActivated(Centers center) => ConnectedComponents.ContainsKey(center);
    
    private bool _IsActivated(Gates key) => _ActivationType(key) != ActivationTypes.None;
    
    private bool _IsActivated(Channels channel) => _ActivationType(channel) != ActivationTypes.None;
    
    private Profiles _Profile()
    {
        return (PersonalityActivation[Planets.Sun].Line, DesignActivation[Planets.Sun].Line).ToProfile();
    }

    private SplitDefinitions _SplitDefinition()
    {
        return Splits switch
        {
            0 => SplitDefinitions.Empty,
            1 => SplitDefinitions.SingleDefinition,
            2 => SplitDefinitions.SplitDefinition,
            3 => SplitDefinitions.TripleSplit,
            4 => SplitDefinitions.QuadrupleSplit,
            _ => throw new ArgumentException($"To much splits: {Splits}")
        };
    }
    
    private Types _Type()
    {
        //No components => no active centers => Reflector
        if (Splits == 0) return Types.Reflector;
                
        //Case: Sacral is undefined (can be Manifestor or Projector)
        if (!ConnectedComponents.ContainsKey(Centers.Sacral))
        {
            //If Throat is undefined, it must be a Projector
            if (!ConnectedComponents.ContainsKey(Centers.Throat)) return Types.Projector;
                    
            //Check connections from motor centers to throat ( => Manifestor)
            var component = ConnectedComponents[Centers.Throat];
            if (ConnectedComponents.ContainsKey(Centers.Heart))
            {
                if (ConnectedComponents[Centers.Heart] == component) return Types.Manifestor;
            }

            if (ConnectedComponents.ContainsKey(Centers.Emotions))
            {
                if (ConnectedComponents[Centers.Emotions] == component) return Types.Manifestor;
            }

            if (ConnectedComponents.ContainsKey(Centers.Root))
            {
                if (ConnectedComponents[Centers.Root] == component) return Types.Manifestor;
            }
                    
            //If no motor center is connected to the throat, it is a Projector by definition
            return Types.Projector;
        }
        //Case: Sacral is defined (can be Generator or Manifesting Generator)
        if (!ConnectedComponents.ContainsKey(Centers.Throat)) return Types.Generator;
                
        //Check connections from motor centers to throat ( => Manifesting Generator)
        if (ConnectedComponents.ContainsKey(Centers.Heart))
        {
            if (ConnectedComponents[Centers.Heart] == ConnectedComponents[Centers.Throat]) return Types.ManifestingGenerator;
        }

        if (ConnectedComponents.ContainsKey(Centers.Emotions))
        {
            if (ConnectedComponents[Centers.Emotions] == ConnectedComponents[Centers.Throat]) return Types.ManifestingGenerator;
        }

        if (ConnectedComponents.ContainsKey(Centers.Root))
        {
            if (ConnectedComponents[Centers.Root] == ConnectedComponents[Centers.Throat]) return Types.ManifestingGenerator;
        }
        if (ConnectedComponents[Centers.Sacral] == ConnectedComponents[Centers.Throat]) return Types.ManifestingGenerator;
                
        //No connection to throat => Generator
        return Types.Generator;
    }
    
    private Strategies _Strategy()
    {
        if (ConnectedComponents.ContainsKey(Centers.Emotions)) return Strategies.Emotional;
        if (ConnectedComponents.ContainsKey(Centers.Sacral)) return Strategies.Sacral;
        if (ConnectedComponents.ContainsKey(Centers.Spleen)) return Strategies.Spleen;
        if (ConnectedComponents.ContainsKey(Centers.Heart))
        {
            if (ConnectedComponents.ContainsKey(Centers.Throat))
            {
                return ConnectedComponents[Centers.Heart] == ConnectedComponents[Centers.Throat] ? Strategies.Heart : Strategies.Self;
            }
        }

        return ConnectedComponents.ContainsKey(Centers.Self) ? Strategies.Self : Strategies.Outer;
    }
    
    private HashSet<Gates> _ActiveGates()
    {
        return Utility.HumanDesignUtility.ActiveGates(PersonalityActivation, DesignActivation);
    }
    
    private HashSet<Channels> _ActiveChannels()
    {
        return Utility.HumanDesignUtility.ActiveChannels(ActiveGates);
    }

    private Variables _Variables()
    {
        return new Variables
        {
            Digestion = (
                DesignActivation[Planets.Sun].Tone.ToOrientation(),
                DesignActivation[Planets.Sun].Color, 
                DesignActivation[Planets.Sun].Tone,
                DesignActivation[Planets.Sun].Base),
            Perspective = (
                PersonalityActivation[Planets.Sun].Tone.ToOrientation(),
                PersonalityActivation[Planets.Sun].Color, 
                PersonalityActivation[Planets.Sun].Tone,
                PersonalityActivation[Planets.Sun].Base),
            Environment = (
                DesignActivation[Planets.NorthNode].Tone.ToOrientation(),
                DesignActivation[Planets.NorthNode].Color, 
                DesignActivation[Planets.NorthNode].Tone,
                DesignActivation[Planets.NorthNode].Base),
            Awareness = (
                PersonalityActivation[Planets.NorthNode].Tone.ToOrientation(),
                PersonalityActivation[Planets.NorthNode].Color, 
                PersonalityActivation[Planets.NorthNode].Tone,
                PersonalityActivation[Planets.NorthNode].Base),
        };
    }

    private IncarnationCrosses _IncarnationCross()
    {
        return (PersonalityActivation[Planets.Sun].Gate, Profile.ToAngle())
            .ToIncarnationCross();
    }

    #endregion
}

file sealed class ChartProbability
{
    public HumanDesignChart Chart;
    public double Probability;
};