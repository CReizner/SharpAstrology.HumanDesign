using SharpAstrology.DataModels;
using SharpAstrology.Definitions;
using SharpAstrology.Enums;
using SharpAstrology.Interfaces;
using SharpAstrology.HumanDesign.Mathematics;
using SharpAstrology.Utility;

namespace SharpAstrology.ExtensionMethods;

public static class HumanDesignIEphemeridesContextExtensionMethods
{
    /// <summary>
    /// Calculates the design Julian day for a given birthdate using the provided ephemerides service.
    /// The design Julian day is determined by finding the day when the Sun's position is 88 degrees ahead of the birthdate's Sun position.
    /// </summary>
    /// <param name="ephService">The ephemerides service used for planetary positions calculations.</param>
    /// <param name="birthdate">The UTC date of birth for which the design Julian day is being calculated.</param>
    /// <returns>The design Julian day corresponding to the given birthdate.</returns>
    /// <exception cref="ArgumentException">Thrown if the provided birthdate is not in UTC.</exception>
    public static DateTime DesignJulianDay(this IEphemerides ephService, DateTime birthdate, EphCalculationMode mode = EphCalculationMode.Tropic)
    {
        if (birthdate.Kind != DateTimeKind.Utc)
        {
            throw new ArgumentException("The given birthdate is not in UTC");
        }
        var dateOfBirthJulian = birthdate.ToJulianDate();
        var sunsLongitude = ephService.PlanetsPosition(Planets.Sun, birthdate, mode).Longitude;
        var dateOfIncomingJulian = RootFinder.FindRoot(jd => AstrologyUtility.AngleDifference(
            AstrologyUtility.SubtractDegree(sunsLongitude, 88),
            ephService.PlanetsPosition(Planets.Sun, AstrologyUtility.DateTimeFromJulianDate(jd)).Longitude
        ), dateOfBirthJulian - 110, dateOfBirthJulian - 70, maxIterations: 1000);
        
        return AstrologyUtility.DateTimeFromJulianDate(dateOfIncomingJulian);
    }

    private static readonly Dictionary<Planets, TimeSpan> _transitStepByPlanet = new()
    {
        { Planets.Moon,      TimeSpan.FromMinutes(30) },
        { Planets.Mercury,   TimeSpan.FromHours(4) },
        { Planets.Venus,     TimeSpan.FromHours(8) },
        { Planets.Sun,       TimeSpan.FromHours(8) },
        { Planets.Earth,     TimeSpan.FromHours(8) },
        { Planets.Mars,      TimeSpan.FromHours(8) },
        { Planets.Jupiter,   TimeSpan.FromDays(1) },
        { Planets.Saturn,    TimeSpan.FromDays(1) },
        { Planets.Uranus,    TimeSpan.FromDays(1) },
        { Planets.Neptune,   TimeSpan.FromDays(1) },
        { Planets.Pluto,     TimeSpan.FromDays(1) },
        { Planets.NorthNode, TimeSpan.FromDays(1) },
        { Planets.SouthNode, TimeSpan.FromDays(1) },
    };

    /// <summary>
    /// Computes the gate/line transit range for each Human Design planet across [start, end] (UTC).
    /// Each planet's list begins with the state at <paramref name="start"/>;
    /// every subsequent entry marks the exact UTC moment the planet enters a new line.
    /// </summary>
    public static Dictionary<Planets, List<TransitRangeEntry>> TransitRange(
        this IEphemerides ephService,
        DateTime start,
        DateTime end,
        EphCalculationMode mode = EphCalculationMode.Tropic)
    {
        if (start.Kind != DateTimeKind.Utc || end.Kind != DateTimeKind.Utc)
            throw new ArgumentException("start and end must be UTC");
        if (end <= start)
            throw new ArgumentException("end must be after start");

        var result = new Dictionary<Planets, List<TransitRangeEntry>>();
        foreach (var planet in HumanDesignDefaults.HumanDesignPlanets)
        {
            result[planet] = _transitRangeForPlanet(ephService, planet, start, end, mode);
        }
        return result;
    }

    private static List<TransitRangeEntry> _transitRangeForPlanet(
        IEphemerides eph, Planets planet, DateTime start, DateTime end, EphCalculationMode mode)
    {
        var entries = new List<TransitRangeEntry>();
        var step = _transitStepByPlanet.TryGetValue(planet, out var s) ? s : TimeSpan.FromHours(8);

        var prevActivation = HumanDesignUtility.ActivationOf(eph.PlanetsPosition(planet, start, mode).Longitude);
        entries.Add(new TransitRangeEntry
        {
            Planet = planet,
            Gate = prevActivation.Gate,
            Line = prevActivation.Line,
            DateTime = start,
        });

        var prevTime = start;
        var t = start + step;
        while (prevTime < end)
        {
            if (t > end) t = end;
            var currActivation = HumanDesignUtility.ActivationOf(eph.PlanetsPosition(planet, t, mode).Longitude);

            if (currActivation.Gate != prevActivation.Gate || currActivation.Line != prevActivation.Line)
            {
                var (crossingTime, crossingActivation) = _findLineCrossing(eph, planet, mode, prevTime, t, prevActivation);
                if (crossingTime <= end)
                {
                    entries.Add(new TransitRangeEntry
                    {
                        Planet = planet,
                        Gate = crossingActivation.Gate,
                        Line = crossingActivation.Line,
                        DateTime = crossingTime,
                    });
                }
                prevActivation = currActivation;
                prevTime = t;
                // If multiple line boundaries fell in this coarse interval, the next iteration
                // from (prevTime=t) will continue detecting them; fast planets have small steps.
            }
            else
            {
                prevTime = t;
            }

            if (t >= end) break;
            t += step;
        }

        return entries;
    }

    private static (DateTime, Activation) _findLineCrossing(
        IEphemerides eph, Planets planet, EphCalculationMode mode,
        DateTime lo, DateTime hi, Activation loActivation)
    {
        // Binary-search for the first instant inside (lo, hi] whose activation differs from loActivation.
        while ((hi - lo).TotalSeconds > 1)
        {
            var mid = lo + TimeSpan.FromTicks((hi - lo).Ticks / 2);
            var midActivation = HumanDesignUtility.ActivationOf(eph.PlanetsPosition(planet, mid, mode).Longitude);
            if (midActivation.Gate == loActivation.Gate && midActivation.Line == loActivation.Line)
                lo = mid;
            else
                hi = mid;
        }
        var hiActivation = HumanDesignUtility.ActivationOf(eph.PlanetsPosition(planet, hi, mode).Longitude);
        return (hi, hiActivation);
    }
}