[System.Serializable]
public class IrrigationData
{
    public IrrigationChart[] chart;  // Array of date and values
}

[System.Serializable]
public class IrrigationChart
{
    public string date;              // Date of the analysis
    public float crop_et;            // Water loss from crop to atmosphere
    public float precipitation;      // Precipitation value
    public float? irrigation_amount; // Nullable field for irrigation amount
}
