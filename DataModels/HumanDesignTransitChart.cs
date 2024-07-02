using SharpAstrology.Enums;
using SharpAstrology.HumanDesign.Mathematics;
using SharpAstrology.Interfaces;
using SharpAstrology.Utility;

namespace SharpAstrology.DataModels;

public sealed class HumanDesignTransitChart : IHumanDesignChart
{
    private readonly HashSet<Gates> _personActiveGates;
    private readonly HashSet<Gates> _transitActiveGates;
    
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
            _personalityFixation ??= _planetaryFixations(PersonalityActivation, DesignActivation, TransitActivation);
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
            _designFixation ??= _planetaryFixations(DesignActivation, PersonalityActivation, TransitActivation);
            return _designFixation;
        }
    }
    
    /// <summary>
    /// Gets a dictionary of transit activations corresponding to each celestial body. 
    /// </summary>
    public Dictionary<Planets, Activation> TransitActivation { get; }
    
    private Dictionary<Planets, PlanetaryFixation>? _transitFixation;
    /// <summary>
    /// Gets a dictionary of planetary fixing states for each transit planet.
    /// The value will be calculated on the first call of this property.
    /// </summary>
    public Dictionary<Planets, PlanetaryFixation> TransitFixation
    {
        get
        {
            _transitFixation ??= _planetaryFixations(TransitActivation, PersonalityActivation, DesignActivation, true);
            return _transitFixation;
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

    /// <summary>
    /// Gets a dictionary of connected components, where each center is associated with its components' id.
    /// </summary>
    public Dictionary<Centers, int> ConnectedComponents { get; }
    
    /// <summary>
    /// Gets the number of connected components of the Human Design graph.
    /// </summary>
    public int Splits { get; }

    public SplitDefinitions SplitDefinition => HumanDesignUtility.SplitDefinition(Splits);
    
    /// <summary>
    /// Gets the set of active gates of the composite chart.
    /// </summary>
    public HashSet<Gates> ActiveGates { get; }

    private Dictionary<Gates, ActivationTypes>? _gateActivations;

    public Dictionary<Gates, ActivationTypes> GateActivations
    {
        get
        {
            _gateActivations ??= HumanDesignUtility.GateActivations(_personActiveGates, _transitActiveGates);
            return _gateActivations;
        }
    }

    private Dictionary<Channels, ChannelActivationType>? _channelActivations;
    public Dictionary<Channels, ChannelActivationType> ChannelActivations
    {
        get
        {
            _channelActivations ??= HumanDesignUtility.CompositeChannelActivations(_personActiveGates, _transitActiveGates);
            return _channelActivations;
        }
    }
    
    #region Constructor
    
    public HumanDesignTransitChart(DateTime birthDate, DateTime transitDate, IEphemerides eph, EphCalculationMode mode = EphCalculationMode.Tropic)
        : this(new HumanDesignChart(birthDate, eph, mode), transitDate, eph, mode) { }
    
    public HumanDesignTransitChart(HumanDesignChart person, DateTime transitDate, IEphemerides eph, EphCalculationMode mode = EphCalculationMode.Tropic)
    {
        PersonalityActivation = person.PersonalityActivation.ToDictionary();
        DesignActivation = person.DesignActivation.ToDictionary();
        TransitActivation = Definitions.HumanDesignDefaults.HumanDesignPlanets.ToDictionary(
            p => p,
            p => HumanDesignUtility.ActivationOf(eph.PlanetsPosition(p, transitDate, mode).Longitude));
        _personActiveGates = person.ActiveGates;
        _transitActiveGates = TransitActivation.Select(x => x.Value.Gate).ToHashSet();
        ActiveGates = _personActiveGates.ToHashSet();
        ActiveGates.UnionWith(_transitActiveGates);
        (ConnectedComponents, Splits) = GraphService.ConnectedCenters(HumanDesignUtility.ActiveChannels(ActiveGates));
    }
    
    #endregion
    
    private Dictionary<Planets, PlanetaryFixation> _planetaryFixations(
        Dictionary<Planets, Activation> activations1,
        Dictionary<Planets, Activation> activations2,
        Dictionary<Planets, Activation> activations3,
        bool firstComparator = false)
    {
        return activations1.ToDictionary(p => p.Key,
            p => HumanDesignUtility.CalculateState(p.Key, activations1, activations2, activations3, firstComparator));
    }
}