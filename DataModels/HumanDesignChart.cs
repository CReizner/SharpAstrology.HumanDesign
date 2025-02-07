using System.Runtime.InteropServices.JavaScript;
using SharpAstrology.Interfaces;
using SharpAstrology.Enums;
using SharpAstrology.ExtensionMethods;
using SharpAstrology.HumanDesign.Mathematics;
using SharpAstrology.Utility;


namespace SharpAstrology.DataModels;

/// <summary>
/// Represents a Human Design Chart. It utilizes planetary positions and activations
/// to provide several complex characteristics like Types, Profiles, Strategies, Channels, Gates and Variables.
/// </summary>
public sealed class HumanDesignChart : IHumanDesignChart
{
    private readonly HashSet<Gates> _personalityGates;
    private readonly HashSet<Gates> _designGates;
    
    #region Properties
    
    /// <summary>
    /// Gets a dictionary of personality activations corresponding to each celestial body. 
    /// </summary>
    public Dictionary<Planets, Activation> PersonalityActivation { get; }

    private Dictionary<Planets, PlanetaryFixation>? _personalityFixation;
    /// <summary>
    /// Gets a dictionary of planetary fixing states for each personality planet.
    /// The value will be calculated on the first call of this property.
    /// </summary>
    public Dictionary<Planets, PlanetaryFixation> PersonalityFixation
    {
        get
        {
            _personalityFixation ??= _planetaryFixations(PersonalityActivation, DesignActivation);
            return _personalityFixation;
        }
    }

    /// <summary>
    /// Gets a dictionary of design activations corresponding to each celestial body. 
    /// </summary>
    public Dictionary<Planets, Activation> DesignActivation { get; }

    private Dictionary<Planets, PlanetaryFixation>? _designFixation;
    /// <summary>
    /// Gets a dictionary of planetary fixing states for each design planet.
    /// The value will be calculated on the first call of this property.
    /// </summary>
    public Dictionary<Planets, PlanetaryFixation> DesignFixation
    {
        get
        {
            _designFixation ??= _planetaryFixations(DesignActivation, PersonalityActivation);
            return _designFixation;
        }
    }

    private Dictionary<Centers, ActivationTypes>? _centerActivations;
    public Dictionary<Centers, ActivationTypes> CenterActivations
    {
        get
        {
            _centerActivations ??= HumanDesignUtility.CenterActivations(ConnectedComponents, ChannelActivations);
            return _centerActivations;
        }
    }

    public Dictionary<Centers, int> ConnectedComponents { get; }
    public int Splits { get; }

    public SplitDefinitions SplitDefinition => HumanDesignUtility.SplitDefinition(Splits);

    private Profiles? _profile;
    /// <summary>
    /// Gets the profile associated with this chart. 
    /// The value will be calculated on the first call of this property.
    /// </summary>
    public Profiles Profile
    {
        get
        {
            _profile ??= _Profile();
            return _profile!.Value;
        }
    }
    
    private Dictionary<Gates, ActivationTypes>? _gateActivations;
    public Dictionary<Gates, ActivationTypes> GateActivations
    {
        get
        {
            _gateActivations ??= HumanDesignUtility.GateActivations(_personalityGates, _designGates);
            return _gateActivations;
        }
    }

    private Dictionary<Channels, ChannelActivationType>? _channelActivation;
    public Dictionary<Channels, ChannelActivationType> ChannelActivations
    {
        get
        {
            _channelActivation ??= HumanDesignUtility.CompositeChannelActivations(_personalityGates, _designGates);
            return _channelActivation;
        }
    }

    private Types? _types;
    /// <summary>
    /// Gets the type associated with this chart.
    /// The value will be calculated on the first call of this property.
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
    /// The value will be calculated on the first call of this property.
    /// </summary>
    public Strategies Strategy
    {
        get
        {
            _strategy ??= _Strategy();
            return _strategy!.Value;
        }
    }

    private HashSet<Gates>? _activeGates;
    /// <summary>
    /// Gets the set of active gates of the chart.
    /// The value will be calculated on the first call of this property.
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
    /// The value will be calculated on the first call of this property.
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
    /// The value will be calculated on the first call of this property.
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
    /// The value will be calculated on the first call of this property.
    /// </summary>
    public Variables Variables
    {
        get
        {
            _variables ??= _Variables();
            return _variables;
        }
    }

    #endregion

    #region Constructor
    
    /// <summary>
    /// Initializes a new instance of the <see cref="HumanDesignChart"/> class. It calculates the design date itself.
    /// </summary>
    /// <param name="dateOfBirth">The date of birth for which the Human Design Chart is being created.</param>
    /// <param name="eph">The ephemerides used for planetary positions calculations.</param>
    /// <param name="mode">Tropical or sidereal calculation mode. Default is tropical.</param>
    /// <exception cref="ArgumentException">Thrown if dateOfBirth is not in UTC.</exception>
    public HumanDesignChart(DateTime dateOfBirth, IEphemerides eph, EphCalculationMode mode = EphCalculationMode.Tropic) 
        : this(dateOfBirth, eph.DesignJulianDay(dateOfBirth, mode), eph, mode) { }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="HumanDesignChart"/> class.
    /// </summary>
    /// <param name="dateOfBirth">The date of birth for which the Human Design Chart is being created.</param>
    /// <param name="designDate">The corresponding design date.</param>
    /// <param name="eph">The ephemerides used for planetary positions calculations.</param>
    /// <param name="mode">Tropical or sidereal calculation mode. Default is tropical.</param>
    /// <exception cref="ArgumentException">Thrown if dateOfBirth or designDate parameters are not in UTC.</exception>
    public HumanDesignChart(DateTime dateOfBirth, DateTime designDate, IEphemerides eph, EphCalculationMode mode = EphCalculationMode.Tropic)
    {
        PersonalityActivation = _PlanetActivations(eph, dateOfBirth, mode);
        DesignActivation = _PlanetActivations(eph, designDate, mode);
        _personalityGates = PersonalityActivation.Values.Select(x => x.Gate).ToHashSet();
        _designGates = DesignActivation.Values.Select(x => x.Gate).ToHashSet();
        // HumanDesignUtility.CalculateState(PersonalityActivation, DesignActivation);
        (ConnectedComponents, Splits) = GraphService.ConnectedCenters(HumanDesignUtility.ActiveChannels(ActiveGates));
    }
    
    #endregion

    #region Utility
    
    /// <summary>
    /// Determines whether the specified <see cref="HumanDesignChart"/> object is equal to the current object,
    /// considering different levels of comparison depth specified by <see cref="ComparatorDepth"/>.
    /// </summary>
    /// <param name="other">The <see cref="HumanDesignChart"/> object to compare with the current object.</param>
    /// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
    public bool Equals(HumanDesignChart? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        
        foreach (var (p,a) in PersonalityActivation)
        {
            if (!PersonalityActivation[p].Equals(other.PersonalityActivation[p], ComparatorDepth.Gate)) return false;
        }
        foreach (var (p,a) in DesignActivation)
        {
            if (!DesignActivation[p].Equals(other.DesignActivation[p], ComparatorDepth.Gate)) return false;
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
    
    #endregion
    
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
    
    private HashSet<Gates> _ActiveGates()
    {
        return HumanDesignUtility.ActiveGates(PersonalityActivation, DesignActivation);
    }
    
    private HashSet<Channels> _ActiveChannels()
    {
        return HumanDesignUtility.ActiveChannels(ActiveGates);
    }
    
    private Profiles _Profile()
    {
        return (PersonalityActivation[Planets.Sun].Line, DesignActivation[Planets.Sun].Line).ToProfile();
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
    
    private Variables _Variables()
    {
        var pSunActivation = PersonalityActivation[Planets.Sun];
        var dSunActivation = DesignActivation[Planets.Sun];
        var pNodeActivation = PersonalityActivation[Planets.NorthNode];
        var dNodeActivation = DesignActivation[Planets.NorthNode];
        return new Variables
        {
            Digestion = new()
            {
                Orientation = dSunActivation.Tone.ToOrientation(),
                Color = dSunActivation.Color, 
                Tone = dSunActivation.Tone,
                Base = dSunActivation.Base
            },
            Awareness = new()
            {
                Orientation = pSunActivation.Tone.ToOrientation(),
                Color = pSunActivation.Color, 
                Tone = pSunActivation.Tone,
                Base = pSunActivation.Base
            },
            Environment = new ()
            {
                Orientation = dNodeActivation.Tone.ToOrientation(),
                Color = dNodeActivation.Color, 
                Tone = dNodeActivation.Tone,
                Base = dNodeActivation.Base
            },
            Perspective = new ()
            {
                Orientation = pNodeActivation.Tone.ToOrientation(),
                Color = pNodeActivation.Color, 
                Tone = pNodeActivation.Tone,
                Base = pNodeActivation.Base
            }
        };
    }

    private IncarnationCrosses _IncarnationCross()
    {
        return (PersonalityActivation[Planets.Sun].Gate, Profile.ToAngle())
            .ToIncarnationCross();
    }
    
    private Dictionary<Planets, Activation> _PlanetActivations(IEphemerides eph, DateTime date, EphCalculationMode mode = EphCalculationMode.Tropic)
    {
        var result = new Dictionary<Planets, Activation>();
        foreach (var p in Definitions.HumanDesignDefaults.HumanDesignPlanets)
        {
            result[p] = HumanDesignUtility.ActivationOf(eph.PlanetsPosition(p, date, mode).Longitude);
        }

        return result;
    }

    private Dictionary<Planets, PlanetaryFixation> _planetaryFixations(
        Dictionary<Planets, Activation> activations,
        Dictionary<Planets, Activation> comparerActivations)
    {
        return activations.ToDictionary(p => p.Key,
            p => HumanDesignUtility.CalculateState(p.Key, activations, comparerActivations));
    }

    #endregion
}

file sealed class ChartProbability
{
    public HumanDesignChart Chart = default!;
    public double Probability;
};