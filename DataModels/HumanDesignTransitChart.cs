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
    
    /// <summary>
    /// Gets a dictionary of design activations corresponding to each celestial body. 
    /// </summary>
    public Dictionary<Planets, Activation> DesignActivation { get; }
    
    /// <summary>
    /// Gets a dictionary of transit activations corresponding to each celestial body. 
    /// </summary>
    public Dictionary<Planets, Activation> TransitActivation { get; }

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
        HumanDesignUtility.CalculateState(PersonalityActivation, DesignActivation, TransitActivation);
    }
    
    #endregion
}