using UnityEngine;

public class GPSBoundingBox
{
    public static Vector2[] GetBoundingSquare(Vector2[] gpsCoords)
    {
        if (gpsCoords == null || gpsCoords.Length == 0)
            return null;

        float minLat = float.MaxValue, maxLat = float.MinValue;
        float minLon = float.MaxValue, maxLon = float.MinValue;

        // Find min/max latitude and longitude
        foreach (var coord in gpsCoords)
        {
            if (coord.x < minLat) minLat = coord.x;
            if (coord.x > maxLat) maxLat = coord.x;
            if (coord.y < minLon) minLon = coord.y;
            if (coord.y > maxLon) maxLon = coord.y;
        }

        // Compute the center
        float centerLat = (minLat + maxLat) / 2;
        float centerLon = (minLon + maxLon) / 2;

        // Find the largest range (to ensure it's a square)
        float latRange = maxLat - minLat;
        float lonRange = maxLon - minLon;
        float maxRange = Mathf.Max(latRange, lonRange);

        // Compute new square boundaries
        float newMinLat = centerLat - (maxRange / 2);
        float newMaxLat = centerLat + (maxRange / 2);
        float newMinLon = centerLon - (maxRange / 2);
        float newMaxLon = centerLon + (maxRange / 2);

        // Return the square coordinates as an array
        return new Vector2[]
        {
            new Vector2(newMaxLat, newMinLon), // Top-left
            new Vector2(newMaxLat, newMaxLon), // Top-right
            new Vector2(newMinLat, newMaxLon), // Bottom-right
            new Vector2(newMinLat, newMinLon)  // Bottom-left
        };
    }
}
