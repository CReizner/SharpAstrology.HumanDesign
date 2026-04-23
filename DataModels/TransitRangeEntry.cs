using SharpAstrology.Enums;

namespace SharpAstrology.DataModels;

/// <summary>
/// A single gate/line transition entry in a transit range.
/// The first entry for each planet describes the state at the range start;
/// every subsequent entry marks the UTC instant the planet crosses into a new line.
/// </summary>
public sealed record TransitRangeEntry
{
    public required Planets Planet { get; init; }
    public required Gates Gate { get; init; }
    public required Lines Line { get; init; }
    public int Retrograde { get; init; }
    public required DateTime DateTime { get; init; }
}
