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
    // ORANGE
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


    // COTTON
    public bool[] PinkBollworm;
    public bool[] Bollworm;
    public bool[] TarnishedPlantBug;
    public bool[] Jassids;
    public bool[] Aphids;
    public bool[] Thrips;
    public bool[] Whitefly;
    public bool[] TwoSpottedSpiderMite;

    // GRAPEVINE
    public bool[] BotrytisGreyMould;
    public bool[] PowderyMildew;
    public bool[] PhomopsisLeafSpot;
    public bool[] DownyMildew;
    public bool[] GrapeBerryMoth;
    public bool[] PlanococcusFicus;
    public bool[] Leafhopper;

    // MAIZE
    //public bool[] Aphids;
    public bool[] Earworm;
    public bool[] MediterraneanStalkborer;
    public bool[] WesternRootWormBeetle;

    // WHEAT
    public bool[] Helminthosporium;
    public bool[] SpeckledLeafBlotch;
    //public bool[] PowderyMildew;
    public bool[] LeafBeetle;

    // SOFT WHEAT
    //public bool[] Helminthosporium;
    //public bool[] SpeckledLeafBlotch;
    //public bool[] PowderyMildew;
    //public bool[] LeafBeetle;

    // BARLEY
    public bool[] Rynchosporium;
    //public bool[] Helminthosporium;
    //public bool[] PowderyMildew;
    //public bool[] LeafBeetle;

    // POTATO
    //public bool[] Aphids;
    public bool[] Beetle;
    public bool[] EarlyBlight;
    public bool[] LateBlight;
    public bool[] Tuberworm;

    // OLIVE
    public bool[] BlackSpot;
    public bool[] FruitFly;
    public bool[] LeafSpot;
    public bool[] Moth;
    public bool[] Psyllid;
    public bool[] Scale;
    public bool[] CalocorisBug;
    public bool[] CercosporaLeafSpot;

    // TOMATO
    //public bool[] Bollworm;
    //public bool[] EarlyBlight;
    public bool[] LeafMiner;
    //public bool[] LateBlight;
    public bool[] TetranychusUrticae;
    public bool[] GrayMold;
    //public bool[] Whitefly;
    //public bool[] PowderyMildew;

    // SOYA
    //public bool[] TetranychusUrticae;
    //public bool[] Aphids;
    //public bool[] DownyMildew;
    //public bool[] CercosporaLeafSpot;

    // SUNFLOWER
    public bool[] Alternaria;
    public bool[] Phomopsis;
    //public bool[] DownyMildew;

    // APPLE
    public bool[] CodlingMoth;
    public bool[] SpiderMites;
    public bool[] SanJoseScale;
    //public bool[] Aphids;
    public bool[] Scab;
    public bool[] FireBlight;
    //public bool[] PowderyMildew;

    // PEAR
    //public bool[] CodlingMoth;
    //public bool[] Psylla;
    //public bool[] SpiderMites;
    //public bool[] SanJoseScale;
    //public bool[] Aphids;
    //public bool[] Scab;
    //public bool[] FireBlight;

    // PEACH
    public bool[] TwigBorer;
    public bool[] OrientalFruitMoth;
    public bool[] SummerFruitTortrix;
    //public bool[] Aphids;
    public bool[] BrownRot;
    public bool[] LeafCurl;
    //public bool[] Scab;
    //public bool[] PowderyMildew;
    public bool[] Weevil;

    // NECTARINE
    //public bool[] TwigBorer;
    //public bool[] OrientalFruitMoth;
    //public bool[] SummerFruitTortrix;
    //public bool[] Aphids;
    //public bool[] BrownRot;
    //public bool[] LeafCurl;
    //public bool[] Scab;
    //public bool[] PowderyMildew;
    //public bool[] Weevil;

    // CHERRY
    //public bool[] FruitFly;
    public bool[] SpottedWingDrosophila;
    //public bool[] Aphids;
    //public bool[] BrownRot;
    public bool[] ShotHole;
    public bool[] LeafScorch;
    //public bool[] LeafSpot;

    // KIWI
    //public bool[] GrayMold;
    public bool[] WhitePeachScale;
    public bool[] AlternariaLeafSpot;
    public bool[] BrownMarmoratedStinkBug;

    // CANOLA
    //public bool[] PowderyMildew;
    public bool[] CabbageStemWeevil;
    public bool[] CabbageStemFleaBeetle;
    //public bool[] Aphids;
    public bool[] PollenBeetle;
    public bool[] AlternariaLeafBlight;



}