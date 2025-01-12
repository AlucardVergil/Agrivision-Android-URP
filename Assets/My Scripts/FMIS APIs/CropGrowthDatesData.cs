[System.Serializable]
public class CropGrowthDatesData
{
    public TimelineData[] timelineData;  // Array of crop growth timeline entries
}

[System.Serializable]
public class TimelineData
{
    public string date;  // Date when the crop growth image is available
    public bool flag;    // Flag indicating whether the crop growth image is available
}
