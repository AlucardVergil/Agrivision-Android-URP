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

        // Return the rectangle coordinates as an array
        return new Vector2[]
        {
            new Vector2(maxLat, minLon), // Top-left
            new Vector2(maxLat, maxLon), // Top-right
            new Vector2(minLat, maxLon), // Bottom-right
            new Vector2(minLat, minLon)  // Bottom-left
        };
    }
}
