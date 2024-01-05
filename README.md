# SharpAstrology.HumanDesign - A SharpAstrology package for the Human Design system

## Examples

### How to get chart information from a point in time?
```C#
using SharpAstrology.DataModels;
using SharpAstrology.Enums;
using SharpAstrology.Ephemerides;
using SharpAstrology.Interfaces;

// Using the IEphemerides implementation from SharpAstrology.SwissEph
var ephemeridesService = new SwissEphemeridesService(ephType: EphType.Moshier);
using IEphemerides eph = ephemeridesService.CreateContext();

// Calculate the chart for January 1st 2024
//
// The given date must be in utc
var pointInTime = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

// Calculate the chart
var chart = new HumanDesignChart(pointInTime, eph);

Console.WriteLine($"Type: {chart.Type}");
// Type: Generator
Console.WriteLine($"Profile: {chart.Profile.GetAsString()}");
// Profile: 1 / 3
Console.WriteLine($"Strategy: {chart.Strategy}");
// Strategy: Emotional
Console.WriteLine($"Split definition: {chart.SplitDefinition.GetAsString()}");
// Split definition: Split

Console.WriteLine("\nActive channels:");
foreach (var channel in chart.ActiveChannels)
{
    Console.WriteLine($"\t{channel.ToGates()}");
}
// (Key29, Key46)
// (Key39, Key55)

Console.WriteLine("\nVariables:");
Console.WriteLine($"Digestion: {chart.Variables.Digestion.Orientation}, {chart.Variables.Digestion.Color.ToNumber()}-{chart.Variables.Digestion.Tone.ToNumber()}");
// Digestion: Left, 5-2
Console.WriteLine($"Perspective: {chart.Variables.Perspective.Orientation}, {chart.Variables.Perspective.Color.ToNumber()}-{chart.Variables.Perspective.Tone.ToNumber()}");
// Perspective: Left, 4-3
Console.WriteLine($"Environment: {chart.Variables.Environment.Orientation}, {chart.Variables.Environment.Color.ToNumber()}-{chart.Variables.Environment.Tone.ToNumber()}");
// Environment: Right, 4-1
Console.WriteLine($"Awareness: {chart.Variables.Awareness.Orientation}, {chart.Variables.Awareness.Color.ToNumber()}-{chart.Variables.Awareness.Tone.ToNumber()}");
// Awareness: Left, 3-1
```
