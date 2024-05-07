namespace SharpAstrology.Enums;

/// <summary>
/// The composite channel type describes how a channel is activated in a composite chart.
/// </summary>
public enum ChannelActivationType
{
    /// <summary>
    /// Channel is not activated.
    /// </summary>
    None,
    
    /// <summary>
    /// Channel was active and is also activated by the comparer.
    /// </summary>
    Companion,
    
    /// <summary>
    /// Channel is activated only by the first comparers.
    /// </summary>
    FirstDominating,
    
    /// <summary>
    /// Channel is activated only by the first comparers.
    /// </summary>
    SecondDominating,
    
    /// <summary>
    /// First comparer activates the channel, the second activates only one of the gates.
    /// </summary>
    CompromiseFirstDominating,
    
    /// <summary>
    /// Second comparer activates the channel, the first activates only one of the gates.
    /// </summary>
    CompromiseSecondDominating,
    
    /// <summary>
    /// Each of the comparers activates only one of the gates but together, they activate the channel.
    /// </summary>
    Magnetic
}