using SharpAstrology.Enums;
using SharpAstrology.ExtensionMethods;
using SharpAstrology.HumanDesign.Mathematics;
using SharpAstrology.Interfaces;
using SharpAstrology.Utility;

namespace SharpAstrology.DataModels;

public sealed class HumanDesignCompositeChart : IHumanDesignChart
{
    private readonly HashSet<Gates> _p1ActiveGates;
    private readonly HashSet<Gates> _p2ActiveGates;
    
    /// <summary>
    /// Gets a dictionary of personality activations corresponding to each celestial body for person 1. 
    /// </summary>
    public Dictionary<Planets, Activation> P1PersonalityActivation { get; }
    
    private Dictionary<Planets, PlanetaryFixation>? _p1PersonalityFixation;
    /// <summary>
    /// Gets a dictionary of planetary fixing states for each personality planet of person 1.
    /// The value will be calculated on the first call of this property.
    /// </summary>
    public Dictionary<Planets, PlanetaryFixation> P1PersonalityFixation
    {
        get
        {
            _p1PersonalityFixation ??= _planetaryFixations(P1PersonalityActivation, P1DesignActivation, P2PersonalityActivation, P2DesignActivation);
            return _p1PersonalityFixation;
        }
    }
    
    /// <summary>
    /// Gets a dictionary of design activations corresponding to each celestial body for person 1. 
    /// </summary>
    public Dictionary<Planets, Activation> P1DesignActivation { get; }
    
    private Dictionary<Planets, PlanetaryFixation>? _p1DesignFixation;
    /// <summary>
    /// Gets a dictionary of planetary fixing states for each personality planet of person 1.
    /// The value will be calculated on the first call of this property.
    /// </summary>
    public Dictionary<Planets, PlanetaryFixation> P1DesignFixation
    {
        get
        {
            _p1DesignFixation ??= _planetaryFixations(P1DesignActivation, P1PersonalityActivation, P2PersonalityActivation, P2DesignActivation);
            return _p1DesignFixation;
        }
    }
    
    /// <summary>
    /// Gets a dictionary of personality activations corresponding to each celestial body for person 2. 
    /// </summary>
    public Dictionary<Planets, Activation> P2PersonalityActivation { get; }
    
    private Dictionary<Planets, PlanetaryFixation>? _p2PersonalityFixation;
    /// <summary>
    /// Gets a dictionary of planetary fixing states for each personality planet of person 2.
    /// The value will be calculated on the first call of this property.
    /// </summary>
    public Dictionary<Planets, PlanetaryFixation> P2PersonalityFixation
    {
        get
        {
            _p2PersonalityFixation ??= _planetaryFixations(P2PersonalityActivation, P2DesignActivation, P1PersonalityActivation, P1DesignActivation);
            return _p2PersonalityFixation;
        }
    }
    
    /// <summary>
    /// Gets a dictionary of design activations corresponding to each celestial body for person 2. 
    /// </summary>
    public Dictionary<Planets, Activation> P2DesignActivation { get; }
    
    private Dictionary<Planets, PlanetaryFixation>? _p2DesignFixation;
    /// <summary>
    /// Gets a dictionary of planetary fixing states for each personality planet of person 2.
    /// The value will be calculated on the first call of this property.
    /// </summary>
    public Dictionary<Planets, PlanetaryFixation> P2DesignFixation
    {
        get
        {
            _p2DesignFixation ??= _planetaryFixations(P2DesignActivation, P2PersonalityActivation, P1PersonalityActivation, P1DesignActivation);
            return _p2DesignFixation;
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
            _gateActivations ??= HumanDesignUtility.GateActivations(_p1ActiveGates, _p2ActiveGates);
            return _gateActivations;
        }
    }
    
    private Dictionary<Channels, ChannelActivationType>? _channelActivation;
    public Dictionary<Channels, ChannelActivationType> ChannelActivations
    {
        get
        {
            _channelActivation ??= HumanDesignUtility.CompositeChannelActivations(_p1ActiveGates, _p2ActiveGates);
            return _channelActivation;
        }
    }
    
    #region Constructor
    
    /// <summary>
    /// Initializes a new instance of the <see cref="HumanDesignCompositeChart"/> class using two HumanDesignChart objects.
    /// </summary>
    /// <param name="person1">The first person's Human Design chart.</param>
    /// <param name="person2">The second person's Human Design chart.</param>
    public HumanDesignCompositeChart(HumanDesignChart person1, HumanDesignChart person2)
    {
        P1PersonalityActivation = person1.PersonalityActivation.ToDictionary(x=>x.Key, x=>new Activation
        {
            Gate = x.Value.Gate,
            Line = x.Value.Line,
            Color = x.Value.Color,
            Tone = x.Value.Tone,
            Base = x.Value.Base
        });
        P1DesignActivation = person1.DesignActivation.ToDictionary(x=>x.Key, x=>new Activation
        {
            Gate = x.Value.Gate,
            Line = x.Value.Line,
            Color = x.Value.Color,
            Tone = x.Value.Tone,
            Base = x.Value.Base
        });
        P2PersonalityActivation = person2.PersonalityActivation.ToDictionary(x=>x.Key, x=>new Activation
        {
            Gate = x.Value.Gate,
            Line = x.Value.Line,
            Color = x.Value.Color,
            Tone = x.Value.Tone,
            Base = x.Value.Base
        });
        P2DesignActivation = person2.DesignActivation.ToDictionary(x=>x.Key, x=>new Activation
        {
            Gate = x.Value.Gate,
            Line = x.Value.Line,
            Color = x.Value.Color,
            Tone = x.Value.Tone,
            Base = x.Value.Base
        });

        _p1ActiveGates = person1.ActiveGates;
        _p2ActiveGates = person2.ActiveGates;
        
        ActiveGates = _p1ActiveGates.ToHashSet();
        ActiveGates.UnionWith(_p2ActiveGates);
        
        (ConnectedComponents, Splits) = GraphService.ConnectedCenters(Utility.HumanDesignUtility.ActiveChannels(ActiveGates));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HumanDesignCompositeChart"/> class using the birthdates of two individuals.
    /// </summary>
    /// <param name="p1dateOfBirth">The date of birth of the first person.</param>
    /// <param name="p2dateOfBirth">The date of birth of the second person.</param>
    /// <param name="eph">The ephemerides service used to calculate planetary positions.</param>
    /// <param name="mode">The calculation mode, defaulting to Tropic.</param>
    public HumanDesignCompositeChart(DateTime p1dateOfBirth, DateTime p2dateOfBirth, IEphemerides eph, EphCalculationMode mode = EphCalculationMode.Tropic)
    {
        var p1designDate = eph.DesignJulianDay(p1dateOfBirth, mode);
        var p2designDate = eph.DesignJulianDay(p2dateOfBirth, mode);
        
        P1PersonalityActivation = Definitions.HumanDesignDefaults.HumanDesignPlanets.ToDictionary(
            p => p,
            p => HumanDesignUtility.ActivationOf(eph.PlanetsPosition(p, p1dateOfBirth, mode).Longitude));
        P1DesignActivation = Definitions.HumanDesignDefaults.HumanDesignPlanets.ToDictionary(
            p => p,
            p => HumanDesignUtility.ActivationOf(eph.PlanetsPosition(p, p1designDate, mode).Longitude));
        
        P2PersonalityActivation = Definitions.HumanDesignDefaults.HumanDesignPlanets.ToDictionary(
            p => p,
            p => HumanDesignUtility.ActivationOf(eph.PlanetsPosition(p, p2dateOfBirth, mode).Longitude));
        P2DesignActivation = Definitions.HumanDesignDefaults.HumanDesignPlanets.ToDictionary(
            p => p,
            p => HumanDesignUtility.ActivationOf(eph.PlanetsPosition(p, p2designDate, mode).Longitude));
        
        _p1ActiveGates = P1PersonalityActivation.Select(pair => pair.Value.Gate).ToHashSet();
        _p1ActiveGates.UnionWith(P1DesignActivation.Select(pair => pair.Value.Gate).ToHashSet());
        _p2ActiveGates = P2PersonalityActivation.Select(pair => pair.Value.Gate).ToHashSet();
        _p2ActiveGates.UnionWith(P2DesignActivation.Select(pair => pair.Value.Gate).ToHashSet());
        ActiveGates = _p1ActiveGates.ToHashSet();
        ActiveGates.UnionWith(_p2ActiveGates);
        
        (ConnectedComponents, Splits) = GraphService.ConnectedCenters(HumanDesignUtility.ActiveChannels(ActiveGates));
    }

    #endregion
    
    private Dictionary<Planets, PlanetaryFixation> _planetaryFixations(
        Dictionary<Planets, Activation> activations1,
        Dictionary<Planets, Activation> activations2,
        Dictionary<Planets, Activation> comparatorActivations1,
        Dictionary<Planets, Activation> comparatorActivations2)
    {
        return activations1.ToDictionary(p => p.Key,
            p => HumanDesignUtility.CalculateState(p.Key, activations1, activations2, comparatorActivations1, comparatorActivations2));
    }
}