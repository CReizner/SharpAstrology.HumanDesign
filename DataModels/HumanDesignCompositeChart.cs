using SharpAstrology.Enums;
using SharpAstrology.ExtensionMethods;
using SharpAstrology.HumanDesign.Mathematics;
using SharpAstrology.Interfaces;

namespace SharpAstrology.DataModels;

public sealed class HumanDesignCompositeChart
{
    /// <summary>
    /// Gets a dictionary of personality activations corresponding to each celestial body for person 1. 
    /// </summary>
    public Dictionary<Planets, Activation> P1PersonalityActivation { get; }
    
    /// <summary>
    /// Gets a dictionary of design activations corresponding to each celestial body for person 1. 
    /// </summary>
    public Dictionary<Planets, Activation> P1DesignActivation { get; }
    
    /// <summary>
    /// Gets a dictionary of personality activations corresponding to each celestial body for person 2. 
    /// </summary>
    public Dictionary<Planets, Activation> P2PersonalityActivation { get; }
    
    /// <summary>
    /// Gets a dictionary of design activations corresponding to each celestial body for person 2. 
    /// </summary>
    public Dictionary<Planets, Activation> P2DesignActivation { get; }
    
    /// <summary>
    /// Gets a dictionary of connected components, where each center is associated with its components id.
    /// </summary>
    public Dictionary<Centers, int> ConnectedComponents { get; }
    
    /// <summary>
    /// Gets the number of connected components of the Human Design graph.
    /// </summary>
    public int Splits { get; }

    /// <summary>
    /// Gets the set of active gates of the composite chart.
    /// </summary>
    public HashSet<Gates> ActiveGates { get; }

    private HashSet<Gates> _p1activeGates;
    private HashSet<Gates> _p2activeGates;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="HumanDesignCompositeChart"/> class using two HumanDesignChart objects.
    /// </summary>
    /// <param name="person1">The first person's Human Design chart.</param>
    /// <param name="person2">The second person's Human Design chart.</param>
    public HumanDesignCompositeChart(HumanDesignChart person1, HumanDesignChart person2)
    {
        P1PersonalityActivation = person1.PersonalityActivation;
        P1DesignActivation = person1.DesignActivation;
        P2PersonalityActivation = person2.PersonalityActivation;
        P2DesignActivation = person2.DesignActivation;

        _p1activeGates = person1.ActiveGates;
        _p2activeGates = person2.ActiveGates;
        
        ActiveGates = _p1activeGates.ToHashSet();
        ActiveGates.UnionWith(_p2activeGates);
        
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
            p => Utility.HumanDesignUtility.ActivationOf(eph.PlanetsPosition(p, p1dateOfBirth, mode).Longitude));
        P1DesignActivation = Definitions.HumanDesignDefaults.HumanDesignPlanets.ToDictionary(
            p => p,
            p => Utility.HumanDesignUtility.ActivationOf(eph.PlanetsPosition(p, p1designDate, mode).Longitude));
        Utility.HumanDesignUtility.CalculateState(P1PersonalityActivation, P1DesignActivation);
        
        P2PersonalityActivation = Definitions.HumanDesignDefaults.HumanDesignPlanets.ToDictionary(
            p => p,
            p => Utility.HumanDesignUtility.ActivationOf(eph.PlanetsPosition(p, p2dateOfBirth, mode).Longitude));
        P2DesignActivation = Definitions.HumanDesignDefaults.HumanDesignPlanets.ToDictionary(
            p => p,
            p => Utility.HumanDesignUtility.ActivationOf(eph.PlanetsPosition(p, p2designDate, mode).Longitude));
        Utility.HumanDesignUtility.CalculateState(P2PersonalityActivation, P2DesignActivation);
        
        _p1activeGates = P1PersonalityActivation.Select(pair => pair.Value.Gate).ToHashSet();
        _p1activeGates.UnionWith(P1DesignActivation.Select(pair => pair.Value.Gate).ToHashSet());
        _p2activeGates = P2PersonalityActivation.Select(pair => pair.Value.Gate).ToHashSet();
        _p2activeGates.UnionWith(P2DesignActivation.Select(pair => pair.Value.Gate).ToHashSet());
        ActiveGates = _p1activeGates.ToHashSet();
        ActiveGates.UnionWith(_p2activeGates);
        
        (ConnectedComponents, Splits) = GraphService.ConnectedCenters(Utility.HumanDesignUtility.ActiveChannels(ActiveGates));
    }

    private Dictionary<Channels, (CompositeChannelType, int)>? _activeChannels;
    /// <summary>
    /// Gets the active channels of the composite chart. The dictionary values are tuples of composite channel type (one of Companion,
    /// Dominance, Compromise, Magnetic or None) and the dominating partner (1 or 2 correspond to the first or second parameter in the constructor).
    /// The value will be calculated on the first call of this property.
    /// </summary>
    public Dictionary<Channels, (CompositeChannelType, int)> ActiveChannels
    {
        get
        {
            _activeChannels ??= _ActiveChannels();
            return _activeChannels;
        }
    }

    private SplitDefinitions? _splitDefinition;
    /// <summary>
    /// Gets the spilt definition associated with this chart.
    /// If the value has already been calculated, it retrieves the calculated value.
    /// The value will be calculated on the first call of this property.
    /// </summary>
    public SplitDefinitions SplitDefinition
    {
        get
        {
            _splitDefinition ??= _SplitDefinition();
            return _splitDefinition!.Value;
        }
    }
    
    #region private Methods

    private Dictionary<Channels, (CompositeChannelType, int)> _ActiveChannels()
    {
        var result = new Dictionary<Channels, (CompositeChannelType, int)>();
        int gate1State;
        int gate2State;
        foreach (var channel in Enum.GetValues<Channels>())
        {
            gate1State = 0;
            gate2State = 0;
            var (gate1, gate2) = channel.ToGates();
            if (_p1activeGates.Contains(gate1)) gate1State = 1;
            if (_p2activeGates.Contains(gate1)) gate1State = gate1State == 1 ? 3 : 2;
            if (_p1activeGates.Contains(gate2)) gate2State = 1;
            if (_p2activeGates.Contains(gate2)) gate2State = gate2State == 1 ? 3 : 2;

            if (gate1State == 0 || gate2State == 0)
            {
                continue;
            }
            if (gate1State == gate2State)
            {
                switch (gate1State)
                {
                    case 1:
                        result[channel] = (CompositeChannelType.Dominance, 1);
                        continue;
                    case 2:
                        result[channel] = (CompositeChannelType.Dominance, 2);
                        continue;
                    case 3:
                        result[channel] = (CompositeChannelType.Companion, 3);
                        continue;
                }
            }
            if (gate1State == 3)
            {
                switch (gate2State)
                {
                    case 1:
                        result[channel] = (CompositeChannelType.Compromise, 1);
                        continue;
                    case 2:
                        result[channel] = (CompositeChannelType.Compromise, 2);
                        continue;
                }
            }
            if (gate2State == 3)
            {
                switch (gate1State)
                {
                    case 1:
                        result[channel] = (CompositeChannelType.Compromise, 1);
                        continue;
                    case 2:
                        result[channel] = (CompositeChannelType.Compromise, 2);
                        continue;
                }
            }
            
            result[channel] = (CompositeChannelType.Magnetic, 0);
        }

        return result;
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
    
    #endregion
    
}