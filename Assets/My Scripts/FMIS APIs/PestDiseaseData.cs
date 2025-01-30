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
    public bool[] CornAphids;
    public bool[] CornEarworm;
    public bool[] CornMediterraneanStalkborer;
    public bool[] CornWesternRootWormBeetle;

    // WHEAT
    public bool[] Helminthosporium;
    public bool[] SpeckledLeafBlotch;
    public bool[] WheatPowderyMildew;
    public bool[] WheatLeafBeetle;

    // SOFT WHEAT
    public bool[] SoftWheatHelminthosporium;
    public bool[] SoftWheatSpeckledLeafBlotch;
    public bool[] SoftWheatPowderyMildew;
    public bool[] SoftWheatLeafBeetle;

    // BARLEY
    public bool[] Rynchosporium;
    public bool[] BarleyHelminthosporium;
    public bool[] BarleyPowderyMildew;
    public bool[] BarleyLeafBeetle;

    // POTATO
    public bool[] PotatoAphids;
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
    public bool[] OliveCalocorisBug;
    public bool[] OliveCercosporaLeafSpot;

    // TOMATO
    public bool[] TomatoBollworm;
    public bool[] TomatoEarlyBlight;
    public bool[] LeafMiner;
    public bool[] TomatoLateBlight;
    public bool[] TomatoTetranychusUrticae;
    public bool[] GrayMold;
    public bool[] TomatoWhitefly;
    public bool[] TomatoPowderyMildew;

    // SOYA
    public bool[] SoyaTetranychusUrticae;
    public bool[] SoyaAphids;
    public bool[] SoyaDownyMildew;
    public bool[] SoyaCercosporaLeafSpot;

    // SUNFLOWER
    public bool[] SunflowerAlternaria;
    public bool[] SunflowerPhomopsis;
    public bool[] SunflowerDownyMildew;

    // APPLE
    public bool[] AppleCodlingMoth;
    public bool[] AppleSpiderMites;
    public bool[] AppleSanJoseScale;
    public bool[] AppleAphids;
    public bool[] AppleScab;
    public bool[] AppleFireBlight;
    public bool[] ApplePowderyMildew;

    // PEAR
    public bool[] PearCodlingMoth;
    public bool[] PearPsylla;
    public bool[] PearSpiderMites;
    public bool[] PearSanJoseScale;
    public bool[] PearAphids;
    public bool[] PearScab;
    public bool[] PearFireBlight;

    // PEACH
    public bool[] PeachTwigBorer;
    public bool[] PeachOrientalFruitMoth;
    public bool[] PeachSummerFruitTortrix;
    public bool[] PeachAphids;
    public bool[] PeachBrownRot;
    public bool[] PeachLeafCurl;
    public bool[] PeachScab;
    public bool[] PeachPowderyMildew;
    public bool[] PeachWeevil;

    // NECTARINE
    public bool[] NectarineTwigBorer;
    public bool[] NectarineOrientalFruitMoth;
    public bool[] NectarineSummerFruitTortrix;
    public bool[] NectarineAphids;
    public bool[] NectarineBrownRot;
    public bool[] NectarineLeafCurl;
    public bool[] NectarineScab;
    public bool[] NectarinePowderyMildew;
    public bool[] NectarineWeevil;

    // CHERRY
    public bool[] CherryFruitFly;
    public bool[] CherrySpottedWingDrosophila;
    public bool[] CherryAphids;
    public bool[] CherryBrownRot;
    public bool[] CherryShotHole;
    public bool[] CherryLeafScorch;
    public bool[] CherryLeafSpot;

    // KIWI
    public bool[] KiwiGrayMold;
    public bool[] KiwiWhitePeachScale;
    public bool[] KiwiAlternariaLeafSpot;
    public bool[] KiwiBrownMarmoratedStinkBug;

    // CANOLA
    public bool[] CanolaPowderyMildew;
    public bool[] CanolaCabbageStemWeevil;
    public bool[] CanolaCabbageStemFleaBeetle;
    public bool[] CanolaAphids;
    public bool[] CanolaPollenBeetle;
    public bool[] CanolaAlternariaLeafBlight;



}