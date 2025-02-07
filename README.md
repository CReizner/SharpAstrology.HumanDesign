# SharpAstrology.HumanDesign - A SharpAstrology package for the Human Design system

## About

This package is part of the SharpAstrology project. It provides all the tools to calculate the common Human Design Body Graph.

## SharpAstrology Packages
| Package                                                                                                                |     Version     | Description                                   | Licence  |
|:-----------------------------------------------------------------------------------------------------------------------|:---------------:|:----------------------------------------------|:--------:|
| [SharpAstrology.Base](https://github.com/CReizner/SharpAstrology.Base)                                                 |     0.10.0      | Base library                                  |   MIT    |
| [SharpAstrology.SwissEph](https://github.com/CReizner/SharpAstrology.SwissEph)                                         |      0.2.2      | Ephemerides package based on SwissEphNet      | AGPL-3.0 |
| [SharpAstrology.HumanDesign](https://github.com/CReizner/SharpAstrology.HumanDesign)                                   |      1.1.2      | Extensions for the Human Design system        |   MIT    |
| [SharpAstrology.HumanDesign.BlazorComponents](https://github.com/CReizner/SharpAstrology.HumanDesign.BlazorComponents) |      0.2.1      | Human Design charts as Blazor components      |   MIT    |
| [SharpAstrology.Vedic](https://github.com/CReizner/SharpAstrology.Vedic)                                               |      0.1.0      | Extensions for Vedic astrology systems        |   MIT    |
| [SharpAstrology.West](https://github.com/CReizner/SharpAstrology.West)                                                 | 0.1.0-preview.4 | Extensions for western astrology systems      |   MIT    |
| [SharpAstrology.West.BlazorComponents](https://github.com/CReizner/SharpAstrology.West.BlazorComponents)               | 0.1.0-preview.1 | Western astrology charts as Blazor components |   MIT    |

## With this package you can calculate:
- Human Design charts with:
  - type
  - profile
  - incarnation cross
  - split definition
  - channel and gate activations
  - strategy
  - variables
- Transit charts
- Composite charts
- Planetary states (exalted, detriment, juxtaposed)

## Install

```
dotnet add package SharpAstrology.HumanDesign
```

## Dependencies

The project uses [SharpAstrology.Base](https://github.com/CReizner/SharpAstrology.Base).
Include the [SharpAstrology.SwissEph](https://github.com/CReizner/SharpAstrology.SwissEph) package, if you don't want to implement IEphemerides yourself. Be aware of the
dual license of the swisseph project.

```dotnet add package SharpAstrology.SwissEph```

SharpAstrology.SwissEph will include SharpAstrology.Base automatically. **Use JPL files or swiss eph files for exact calculations that are in alignment with online chart calculators.**
For explanation how to do that, please visit the original [swisseph project](https://github.com/aloistr/swisseph) on GitHub and look for **Download location of files**.

## Examples

### How to get chart information from a point in time?
```C#
using SharpAstrology.DataModels;
using SharpAstrology.Enums;
using SharpAstrology.Ephemerides;
using SharpAstrology.Interfaces;

// Using the IEphemerides implementation from SharpAstrology.SwissEph.
// Use swiss eph files or jpl files for more accuracy so that the results are in alignment with online HD chart calculators.
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
Console.WriteLine($"Profile: {chart.Profile.ToText()}");
// Profile: 1 / 3
Console.WriteLine($"Strategy: {chart.Strategy}");
// Strategy: Emotional
Console.WriteLine($"Split definition: {chart.SplitDefinition.ToText()}");
// Split definition: Split
Console.WriteLine($"Incarnation Cross: {chart.IncarnationCross.ToText()}");
// Incarnation Cross: The Right Angle Cross of Tension 4

Console.WriteLine("\nActive channels:");
foreach (var channel in chart.ActiveChannels)
{
    Console.WriteLine($"\t{channel.ToGates()}");
}
// (Key29, Key46)
// (Key39, Key55)

Console.WriteLine("\nActive Personality Gates:");
foreach (var (planet, activation) in chart.PersonalityActivation)
{
    Console.WriteLine($"\t{planet} {activation.Gate}-{activation.Line}-{activation.FixingState}");
}

Console.WriteLine("\nActive Design Gates:");
foreach (var (planet, activation) in chart.DesignActivation)
{
    Console.WriteLine($"\t{planet} {activation.Gate}-{activation.Line}-{activation.FixingState}");
}

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

### Notes on parallelism with SharpAstrology.SwissEph
If you are using the **IEphemerides** implementation from **SharpAstrology.SwissEph**, then the only option to calculate charts in parallel is to create one IEphemerides context for each chart you want to calculate. 
The underlying bindings, provided by **SwissEphNet**, are not thread save.
```C#
using SharpAstrology.DataModels;
using SharpAstrology.Ephemerides;
using SharpAstrology.HumanDesign.Benchmarks;

SwissEphemeridesService ephService =
    new SwissEphemeridesService("[PATH_TO_EPHE_FILES]");
DateTime start = new DateTime(1988, 9, 4, 0, 0, 0, DateTimeKind.Utc);

// works...
Parallel.Invoke(
    () => new HumanDesignChart(start, ephService.CreateContext()),
    () => new HumanDesignChart(start, ephService.CreateContext())
    );
```


### If you don't know the exact time of birth

You can specify a time range and see which different charts appear in this period. Different means that the charts differ in at least one active gate or in the line of the sun.

```C#
using SharpAstrology.DataModels;
using SharpAstrology.Enums;
using SharpAstrology.Ephemerides;


var start = new DateTime(1988, 9, 4, 0, 0, 0, DateTimeKind.Utc);
var end = new DateTime(1988, 9, 4, 3, 0, 0, DateTimeKind.Utc);

var eph = new SwissEphemeridesService([YOUR_PATH_THE_EPHE_FILES]).CreateContext();

var results = HumanDesignChart.Guess(start, end, eph);

foreach (var r in results)
{
    Console.WriteLine($"Probability {r.Probability}");
    foreach (var (planet, activation) in r.Chart.PersonalityActivation)
    {
        Console.WriteLine($"{planet.ToName()}: {activation.Gate}-{activation.Line}\t{r.Chart.DesignActivation[planet].Gate}-{r.Chart.DesignActivation[planet].Line}");
    }
    Console.WriteLine("---------------------------");
}
```

### How to create a composite chart?
Composite charts allow the mutual effects of the respective charts to be analyzed. 
The most important components here are the channels. 
Is there an interplay between gates of both channels? 
Does one person dominate the channel of the other or are the activations equal?
```C#
using SharpAstrology.DataModels;
using SharpAstrology.Enums;
using SharpAstrology.Ephemerides;

var date1 = new DateTime(1988, 9, 4, 1, 15, 0, DateTimeKind.Utc);
var date2 = new DateTime(1990, 2, 6, 22, 55, 0, DateTimeKind.Utc);
using var eph = new SwissEphemeridesService(ephType:EphType.Moshier).CreateContext();

// Create composite chart
var result = new HumanDesignCompositeChart(date1, date2, eph);

foreach (var (channel, activationType) in result
             .ChannelActivations
             .Where(x=>x.Value != ChannelActivationType.None)
             .OrderBy(x=>x.Value))
{
    Console.WriteLine($"{channel} - {activationType}");
}
// Key11Key56 - FirstDominating
// Key12Key22 - FirstDominating
// Key21Key45 - FirstDominating
// Key47Key64 - FirstDominating
// Key28Key38 - SecondDominating
// Key10Key20 - CompromiseFirstDominating
// Key18Key58 - CompromiseFirstDominating
// Key23Key43 - CompromiseSecondDominating
// Key4Key63 - Magnetic
// Key5Key15 - Magnetic
```

### How to create a transit chart?
```C#
using SharpAstrology.DataModels;
using SharpAstrology.Enums;
using SharpAstrology.Ephemerides;

var chart = new DateTime(1988, 9, 4, 1, 15, 0, DateTimeKind.Utc);
var transit = new DateTime(2024, 5, 7, 12, 0, 0, DateTimeKind.Utc);
using var eph = new SwissEphemeridesService(ephType:EphType.Moshier).CreateContext();

// Create transit chart
var result = new HumanDesignTransitChart(chart, transit, eph);

foreach (var (channel, activationType) in result
             .ChannelActivations
             .Where(x=>x.Value != ChannelActivationType.None)
             .OrderBy(x=>x.Value))
{
    Console.WriteLine($"{channel} - {activationType}");
}
```

### I would like to visualize a chart
SharpAstrology provides Blazor components: [SharpAstrology.HumanDesign.BlazorComponents](https://github.com/CReizner/SharpAstrology.HumanDesign.BlazorComponents)

```dotnet add package SharpAstrology.HumanDesign.BlazorComponents```

![A Human Design chart example](https://github.com/CReizner/SharpAstrology.HumanDesign.BlazorComponents/blob/master/.github_assets/hd_chart_0.png)

### Notes on performance
- You can save about 18% of construction time for a HumanDesignChart class, if you save and reuse the julian date.
- Reuse already calculated charts if constructing a composite or transit chart.
- The calculation speed is strongly dependent on the IEphemerides implementation.
