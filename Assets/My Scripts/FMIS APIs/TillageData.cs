[System.Serializable]
public class TillageData
{
    public string title;                 // Title of the analysis
    public string unit;                  // Unit of the measurement
    public TillageChartData[] tillage_chart_data;  // Array of date and values
    public float threshold_1;            // Ideal condition threshold
    public float threshold_2;            // Good condition threshold
    public float threshold_3;            // Average condition threshold
    public float threshold_4;            // Bad condition threshold
}

[System.Serializable]
public class TillageChartData
{
    public string date;                  // Date of the analysis
    public float value;                  // Value of the soil moisture
}
