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
}