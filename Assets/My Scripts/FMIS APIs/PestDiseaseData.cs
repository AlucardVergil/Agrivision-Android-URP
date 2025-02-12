using Newtonsoft.Json;

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
    [JsonProperty("Mediterranean fruit fly")]
    public bool[] MediterraneanFruitFly;
    [JsonProperty("Red scale")]
    public bool[] RedScale;
    [JsonProperty("Citrus mealybug")]
    public bool[] CitrusMealybug;
    [JsonProperty("Cottony cushion scale")]
    public bool[] CottonyCushionScale;
    [JsonProperty("Citrus spiny whitefly")]
    public bool[] CitrusSpinyWhitefly;
    public bool[] Tetranychus;
    [JsonProperty("Black parlatoria scale")]
    public bool[] BlackParlatoriaScale;
    [JsonProperty("Citrus leafminer")]
    public bool[] CitrusLeafminer;
    public bool[] Anthracnose;
    public bool[] Altenaria;
    [JsonProperty("Black aphid")]
    public bool[] BlackAphid;


    // COTTON
    [JsonProperty("Pink bollworm")]
    public bool[] PinkBollworm;
    public bool[] Bollworm;
    [JsonProperty("Tarnished plant bug")]
    public bool[] TarnishedPlantBug;
    public bool[] Jassids;
    public bool[] Aphids;
    public bool[] Thrips;
    public bool[] Whitefly;
    [JsonProperty("Two spotted spider mite")]
    public bool[] TwoSpottedSpiderMite;

    // GRAPEVINE
    [JsonProperty("Botrytis grey mould")]
    public bool[] BotrytisGreyMould;
    [JsonProperty("Powdery mildew")]
    public bool[] PowderyMildew;
    [JsonProperty("Phomopsis leaf spot")]
    public bool[] PhomopsisLeafSpot;
    [JsonProperty("Downy mildew")]
    public bool[] DownyMildew;
    [JsonProperty("Grape berry moth")]
    public bool[] GrapeBerryMoth;
    [JsonProperty("Planococcus ficus")]
    public bool[] PlanococcusFicus;
    public bool[] Leafhopper;

    // MAIZE
    //public bool[] Aphids;
    public bool[] Earworm;
    [JsonProperty("Mediterranean stalkborer")]
    public bool[] MediterraneanStalkborer;
    [JsonProperty("Western root worm beetle")]
    public bool[] WesternRootWormBeetle;

    // WHEAT
    public bool[] Helminthosporium;
    [JsonProperty("Speckled leaf blotch")]
    public bool[] SpeckledLeafBlotch;
    //public bool[] PowderyMildew;
    [JsonProperty("Leaf beetle")]
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
    [JsonProperty("Early blight")]
    public bool[] EarlyBlight;
    [JsonProperty("Late blight")]
    public bool[] LateBlight;
    public bool[] Tuberworm;

    // OLIVE
    [JsonProperty("Black spot")]
    public bool[] BlackSpot;
    [JsonProperty("Fruit fly")]
    public bool[] FruitFly;
    [JsonProperty("Leaf spot")]
    public bool[] LeafSpot;
    public bool[] Moth;
    public bool[] Psyllid;
    public bool[] Scale;
    [JsonProperty("Calocoris bug")]
    public bool[] CalocorisBug;
    [JsonProperty("Cercospora leaf spot")]
    public bool[] CercosporaLeafSpot;

    // TOMATO
    //public bool[] Bollworm;
    //public bool[] EarlyBlight;
    [JsonProperty("Leaf miner")]
    public bool[] LeafMiner;
    //public bool[] LateBlight;
    [JsonProperty("Tetranychus urticae")]
    public bool[] TetranychusUrticae;
    [JsonProperty("Gray mold")]
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
    [JsonProperty("Codling moth")]
    public bool[] CodlingMoth;
    [JsonProperty("Spider mites")]
    public bool[] SpiderMites;
    [JsonProperty("San jose scale")]
    public bool[] SanJoseScale;
    //public bool[] Aphids;
    public bool[] Scab;
    [JsonProperty("Fire blight")]
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
    [JsonProperty("Twig borer")]
    public bool[] TwigBorer;
    [JsonProperty("Oriental fruit moth")]
    public bool[] OrientalFruitMoth;
    [JsonProperty("Summer fruit tortrix")]
    public bool[] SummerFruitTortrix;
    //public bool[] Aphids;
    [JsonProperty("Brown rot")]
    public bool[] BrownRot;
    [JsonProperty("Leaf curl")]
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
    [JsonProperty("Spotted wing drosophila")]
    public bool[] SpottedWingDrosophila;
    //public bool[] Aphids;
    //public bool[] BrownRot;
    [JsonProperty("Shot hole")]
    public bool[] ShotHole;
    [JsonProperty("Leaf scorch")]
    public bool[] LeafScorch;
    //public bool[] LeafSpot;

    // KIWI
    //public bool[] GrayMold;
    [JsonProperty("White peach scale")]
    public bool[] WhitePeachScale;
    [JsonProperty("Alternaria leaf spot")]
    public bool[] AlternariaLeafSpot;
    [JsonProperty("Brown marmorated stink bug")]
    public bool[] BrownMarmoratedStinkBug;

    // CANOLA
    //public bool[] PowderyMildew;
    [JsonProperty("Cabbage stem weevil")]
    public bool[] CabbageStemWeevil;
    [JsonProperty("Cabbage stem flea beetle")]
    public bool[] CabbageStemFleaBeetle;
    //public bool[] Aphids;
    [JsonProperty("Pollen beetle")]
    public bool[] PollenBeetle;
    [JsonProperty("Alternaria leaf blight")]
    public bool[] AlternariaLeafBlight;



}