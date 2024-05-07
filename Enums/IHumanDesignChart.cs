namespace SharpAstrology.Enums;

public interface IHumanDesignChart
{
    /// <summary>
    /// Gets a dictionary, where gates are mapped to there activation type.
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