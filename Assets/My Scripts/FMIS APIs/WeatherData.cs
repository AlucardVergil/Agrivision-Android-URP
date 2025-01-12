[System.Serializable]
public class WeatherData
{
    public string title;         // Title (e.g., location name)
    public Units units;          // Units for measurement (e.g., °C, mm)
    public DayData[] data;       // Array of daily weather data
}

[System.Serializable]
public class Units
{
    public string temperature;   // Temperature unit (°C)
    public string precipitation; // Precipitation unit (mm)
    public string humidity;      // Humidity unit (%)
    public string precipitation_prob; // Precipitation probability unit (%)
    public string evapotranspiration; // Evapotranspiration unit (mm)
    public string wind;          // Wind speed unit (bft)
}

[System.Serializable]
public class DayData
{
    public string span_label;    // Date of the forecast
    public string weather_icon;  // Icon for the weather (e.g., "clouds_rain_light")
    public Alert[] alerts;       // Array of alerts (if any)
    public int temperature;      // Temperature for the day
    public float precipitation_prob; // Precipitation probability
    public float precipitation;  // Amount of precipitation
    public int humidity;         // Humidity percentage
    public int wind;             // Wind speed (in bft)
    public float evapotranspiration; // Evapotranspiration amount
}

[System.Serializable]
public class Alert
{
    public string title;         // Title of the alert (e.g., "Rain Warning")
    public string type;          // Type of alert (e.g., "precipitation")
    public int severity_level;   // Severity of the alert (e.g., 1 = low, 5 = high)
}