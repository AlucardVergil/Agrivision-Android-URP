using System.Collections.Generic;

[System.Serializable]
public class FertilizationData
{
    public FertilizationSeason[] managementZones;  // Array of seasons with images
    //public List<FertilizationSeason> managementZones;
}

[System.Serializable]
public class FertilizationSeason
{
    public int season;         // Year of the season
    public string image;       // Base64 encoded image blob (can be null)
}
