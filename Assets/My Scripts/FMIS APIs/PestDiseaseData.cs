[System.Serializable]
public class PestDiseaseData
{
    public DateEntry[] dates;    // Array of date entries
    public Diseases diseases;    // List of diseases and pest occurrences
}

[System.Serializable]
public class DateEntry
{
    public string day;           // Day of the week (e.g., "Tue")
    public string date;          // Date (e.g., "10/09")
}

[System.Serializable]
public class Diseases
{
    public bool[] MediterraneanFruitFly;
    public bool[] RedScale;
    public bool[] CitrusMealybug;
    public bool[] CottonyCushionScale;
    public bool[] CitrusSpinyWhitefly;
    public bool[] Tetranychus;
    public bool[] BlackParlatoriaScale;
    public bool[] CitrusLeafminer;
    public bool[] Anthracnose;
    public bool[] Altenaria;
    public bool[] BlackAphid;


    /*
    public bool[] CottonPinkBollworm;
    public bool[] CottonBollworm;
    public bool[] TarnishedPlantBug;
    public bool[] Jassids;
    public bool[] Aphids;
    public bool[] Thrips;
    public bool[] Whitefly;
    public bool[] TwoSpottedSpiderMite;
    */
}