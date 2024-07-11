using SharpAstrology.Enums;

namespace SharpAstrology.Interfaces;

public interface IHumanDesignChart
{
    /// <summary>
    /// This dictionary indicates whether and how a gate has been activated.
    /// This property is useful for drawing the body graph.
    /// The value will be calculated on the first call of this property. 
    /// </summary>
    public Dictionary<Gates, ActivationTypes> GateActivations { get; }
    
    /// <summary>
    /// Calculates the composite channel activations between two sets of gates.
    /// This method determines the type and dominance of channel activations.
    /// The value will be calculated on the first call of this property.
    /// </summary>
    public Dictionary<Channels, ChannelActivationType> ChannelActivations { get; }

    /// <summary>
    /// This dictionary indicates whether and how a center has been activated.
    /// This property is useful for drawing the body graph.
    /// The value will be calculated on the first call of this property.
    /// </summary>
    public Dictionary<Centers, ActivationTypes> CenterActivations { get; }
    
    /// <summary>
    /// Gets a dictionary of connected components, where each center is associated with its component id.
    /// </summary>
    public Dictionary<Centers, int> ConnectedComponents { get; }
    
    /// <summary>
    /// Gets the number of connected components of the Human Design graph.
    /// </summary>
    public int Splits { get; }
    
    /// <summary>
    /// Gets the spilt definition associated with this chart.
    /// The value will be calculated on the first call of this property.
    /// </summary>
    public SplitDefinitions SplitDefinition { get; }

    
}