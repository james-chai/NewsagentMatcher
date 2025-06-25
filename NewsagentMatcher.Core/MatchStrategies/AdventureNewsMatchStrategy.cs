using NewsagentMatcher.Core.Interfaces;
using NewsagentMatcher.Core.Models;

namespace NewsagentMatcher.Core.MatchStrategies;

public sealed class AdventureNewsMatchStrategy(double maxDistanceMeters = 100) : INewsagentMatchStrategy
{
    private const double EarthRadiusMeters = 6_371_000;
    private const string ChainIdentifier = "ADV";

    public string ChainId => ChainIdentifier;
    public string Description => $"Geolocation within {maxDistanceMeters}m (Adventure News)";

    public bool IsMatch(Newsagent newsagent, ZineCoNewsagent zineCoNewsagent)
    {
        if (newsagent == null) throw new ArgumentNullException(nameof(newsagent));
        if (zineCoNewsagent == null) throw new ArgumentNullException(nameof(zineCoNewsagent));

        return NameMatches(newsagent.Name, zineCoNewsagent.Name) &&
               IsWithinDistance(newsagent, zineCoNewsagent);
    }

    private static bool NameMatches(string newsagentName, string zineCoName)
        => string.Equals(newsagentName, zineCoName, StringComparison.OrdinalIgnoreCase);

    private bool IsWithinDistance(Newsagent newsagent, ZineCoNewsagent zineCoNewsagent)
        => CalculateHaversineDistance(
            newsagent.Latitude,
            newsagent.Longitude,
            zineCoNewsagent.Latitude,
            zineCoNewsagent.Longitude) <= maxDistanceMeters;

    private static double CalculateHaversineDistance(double lat1, double lon1, double lat2, double lon2)
    {
        var φ1 = DegreesToRadians(lat1);
        var φ2 = DegreesToRadians(lat2);
        var Δφ = DegreesToRadians(lat2 - lat1);
        var Δλ = DegreesToRadians(lon2 - lon1);

        var a = Math.Pow(Math.Sin(Δφ / 2), 2) +
                Math.Cos(φ1) * Math.Cos(φ2) *
                Math.Pow(Math.Sin(Δλ / 2), 2);

        return EarthRadiusMeters * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    }

    private static double DegreesToRadians(double degrees)
        => degrees * Math.PI / 180;
}