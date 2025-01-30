using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class ParcelsListItem : MonoBehaviour
{
    public string parcelId;
    public TMP_Text parcelTitle;
    public TMP_Text parcelAcreage;
    public TMP_Text temperatureRange;
    public TMP_Text parcelText;
    public TMP_Text pests;
    public TMP_Text temperatureToday;

    GameObject apisManager;

    WeatherData weatherData;
    PestDiseaseData pestsData;

    [HideInInspector] public Parcel parcelData;
    private GameObject GPSReceiverGameObject;



    // Start is called before the first frame update
    public void InstantiateParcelItem()
    {
        apisManager = GameObject.FindGameObjectWithTag("APIsManager");
        GPSReceiverGameObject = GameObject.Find("GPSReceiverGameObject");


        parcelTitle.text = parcelData.name;
        parcelAcreage.text = parcelData.size.ToString() + " sq.m.";

        Debug.Log("PEST PARCEL " + parcelId + " " + parcelData.crop_type_support_diseases);

        if (parcelData.crop_type_support_diseases)
        {
            apisManager.GetComponent<PestDiseaseAPI>().GetPestDiseaseData(parcelId, (jsonResponsePests) => //"11256"
            {
                if (jsonResponsePests != "[]") // There are no pest data for this parcel at these dates
                {
                    pestsData = JsonUtility.FromJson<PestDiseaseData>(jsonResponsePests);

                    Debug.Log("CodlingMoth " + pestsData.diseases);

                    if (pestsData != null)
                    {
                        int numOfDiseasesToday = 0;

                        if (pestsData.diseases?.Altenaria != null && pestsData.diseases.Altenaria[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("Altenaria");
                        }

                        //Debug.Log("Anthracnose " + pestsData.diseases.Anthracnose[0]);
                        if (pestsData.diseases?.Anthracnose != null && pestsData.diseases.Anthracnose[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("Anthracnose");
                        }

                        if (pestsData.diseases?.BlackAphid != null && pestsData.diseases.BlackAphid[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("BlackAphid");
                        }

                        if (pestsData.diseases?.BlackParlatoriaScale != null && pestsData.diseases.BlackParlatoriaScale[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("BlackParlatoriaScale");
                        }

                        if (pestsData.diseases?.CitrusLeafminer != null && pestsData.diseases.CitrusLeafminer[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("CitrusLeafminer");
                        }

                        if (pestsData.diseases?.CitrusMealybug != null && pestsData.diseases.CitrusMealybug[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("CitrusMealybug");
                        }

                        if (pestsData.diseases?.CitrusSpinyWhitefly != null && pestsData.diseases.CitrusSpinyWhitefly[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("CitrusSpinyWhitefly");
                        }

                        if (pestsData.diseases?.CottonyCushionScale != null && pestsData.diseases.CottonyCushionScale[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("CottonyCushionScale");
                        }


                        /////

                        if (pestsData.diseases?.PinkBollworm != null && pestsData.diseases.PinkBollworm[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("PinkBollworm");
                        }

                        if (pestsData.diseases?.Bollworm != null && pestsData.diseases.Bollworm[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("Bollworm");
                        }

                        if (pestsData.diseases?.TarnishedPlantBug != null && pestsData.diseases.TarnishedPlantBug[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("TarnishedPlantBug");
                        }

                        if (pestsData.diseases?.Jassids != null && pestsData.diseases.Jassids[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("Jassids");
                        }

                        if (pestsData.diseases?.Aphids != null && pestsData.diseases.Aphids[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("Aphids");
                        }

                        if (pestsData.diseases?.Thrips != null && pestsData.diseases.Thrips[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("Thrips");
                        }

                        if (pestsData.diseases?.Whitefly != null && pestsData.diseases.Whitefly[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("Whitefly");
                        }

                        if (pestsData.diseases?.TwoSpottedSpiderMite != null && pestsData.diseases.TwoSpottedSpiderMite[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("TwoSpottedSpiderMite");
                        }

                        if (pestsData.diseases?.BotrytisGreyMould != null && pestsData.diseases.BotrytisGreyMould[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("BotrytisGreyMould");
                        }

                        Debug.Log("PowderyMildew " + pestsData.diseases.PowderyMildew[0]);
                        if (pestsData.diseases?.PowderyMildew != null && pestsData.diseases.PowderyMildew[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("PowderyMildew");
                        }

                        if (pestsData.diseases?.PhomopsisLeafSpot != null && pestsData.diseases.PhomopsisLeafSpot[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("PhomopsisLeafSpot");
                        }

                        if (pestsData.diseases?.DownyMildew != null && pestsData.diseases.DownyMildew[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("DownyMildew");
                        }

                        if (pestsData.diseases?.GrapeBerryMoth != null && pestsData.diseases.GrapeBerryMoth[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("GrapeBerryMoth");
                        }

                        if (pestsData.diseases?.PlanococcusFicus != null && pestsData.diseases.PlanococcusFicus[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("PlanococcusFicus");
                        }

                        if (pestsData.diseases?.Leafhopper != null && pestsData.diseases.Leafhopper[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("Leafhopper");
                        }

                        if (pestsData.diseases?.Earworm != null && pestsData.diseases.Earworm[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("Earworm");
                        }

                        if (pestsData.diseases?.MediterraneanStalkborer != null && pestsData.diseases.MediterraneanStalkborer[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("MediterraneanStalkborer");
                        }

                        if (pestsData.diseases?.WesternRootWormBeetle != null && pestsData.diseases.WesternRootWormBeetle[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("WesternRootWormBeetle");
                        }

                        if (pestsData.diseases?.Helminthosporium != null && pestsData.diseases.Helminthosporium[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("Helminthosporium");
                        }

                        if (pestsData.diseases?.SpeckledLeafBlotch != null && pestsData.diseases.SpeckledLeafBlotch[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("SpeckledLeafBlotch");
                        }

                        if (pestsData.diseases?.LeafBeetle != null && pestsData.diseases.LeafBeetle[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("LeafBeetle");
                        }

                        if (pestsData.diseases?.Rynchosporium != null && pestsData.diseases.Rynchosporium[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("Rynchosporium");
                        }

                        if (pestsData.diseases?.Beetle != null && pestsData.diseases.Beetle[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("Beetle");
                        }

                        if (pestsData.diseases?.EarlyBlight != null && pestsData.diseases.EarlyBlight[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("EarlyBlight");
                        }

                        if (pestsData.diseases?.LateBlight != null && pestsData.diseases.LateBlight[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("LateBlight");
                        }

                        if (pestsData.diseases?.Tuberworm != null && pestsData.diseases.Tuberworm[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("Tuberworm");
                        }

                        if (pestsData.diseases?.BlackSpot != null && pestsData.diseases.BlackSpot[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("BlackSpot");
                        }

                        if (pestsData.diseases?.FruitFly != null && pestsData.diseases.FruitFly[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("FruitFly");
                        }

                        if (pestsData.diseases?.LeafSpot != null && pestsData.diseases.LeafSpot[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("LeafSpot");
                        }

                        if (pestsData.diseases?.Moth != null && pestsData.diseases.Moth[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("Moth");
                        }

                        if (pestsData.diseases?.Psyllid != null && pestsData.diseases.Psyllid[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("Psyllid");
                        }

                        if (pestsData.diseases?.Scale != null && pestsData.diseases.Scale[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("Scale");
                        }

                        if (pestsData.diseases?.CalocorisBug != null && pestsData.diseases.CalocorisBug[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("CalocorisBug");
                        }

                        if (pestsData.diseases?.CercosporaLeafSpot != null && pestsData.diseases.CercosporaLeafSpot[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("CercosporaLeafSpot");
                        }

                        if (pestsData.diseases?.LeafMiner != null && pestsData.diseases.LeafMiner[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("LeafMiner");
                        }

                        if (pestsData.diseases?.TetranychusUrticae != null && pestsData.diseases.TetranychusUrticae[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("TetranychusUrticae");
                        }

                        if (pestsData.diseases?.GrayMold != null && pestsData.diseases.GrayMold[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("GrayMold");
                        }

                        if (pestsData.diseases?.Alternaria != null && pestsData.diseases.Alternaria[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("Suf lowerAlternaria");
                        }

                        if (pestsData.diseases?.Phomopsis != null && pestsData.diseases.Phomopsis[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("Phomopsis");
                        }

                        if (pestsData.diseases?.CodlingMoth != null && pestsData.diseases.CodlingMoth[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("CodlingMoth");
                        }

                        if (pestsData.diseases?.SpiderMites != null && pestsData.diseases.SpiderMites[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("SpiderMites");
                        }

                        Debug.Log("SanJoseScale " + pestsData.diseases.SanJoseScale[0]);
                        if (pestsData.diseases?.SanJoseScale != null && pestsData.diseases.SanJoseScale[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("SanJoseScale");
                        }

                        if (pestsData.diseases?.Scab != null && pestsData.diseases.Scab[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("Scab");
                        }

                        if (pestsData.diseases?.FireBlight != null && pestsData.diseases.FireBlight[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("FireBlight");
                        }

                        if (pestsData.diseases?.TwigBorer != null && pestsData.diseases.TwigBorer[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("TwigBorer");
                        }

                        if (pestsData.diseases?.OrientalFruitMoth != null && pestsData.diseases.OrientalFruitMoth[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("OrientalFruitMoth");
                        }

                        if (pestsData.diseases?.SummerFruitTortrix != null && pestsData.diseases.SummerFruitTortrix[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("SummerFruitTortrix");
                        }

                        if (pestsData.diseases?.BrownRot != null && pestsData.diseases.BrownRot[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("BrownRot");
                        }

                        if (pestsData.diseases?.LeafCurl != null && pestsData.diseases.LeafCurl[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("LeafCurl");
                        }

                        if (pestsData.diseases?.Weevil != null && pestsData.diseases.Weevil[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("Weevil");
                        }

                        if (pestsData.diseases?.SpottedWingDrosophila != null && pestsData.diseases.SpottedWingDrosophila[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("SpottedWingDrosophila");
                        }

                        if (pestsData.diseases?.ShotHole != null && pestsData.diseases.ShotHole[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("ShotHole");
                        }

                        if (pestsData.diseases?.LeafScorch != null && pestsData.diseases.LeafScorch[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("LeafScorch");
                        }

                        if (pestsData.diseases?.WhitePeachScale != null && pestsData.diseases.WhitePeachScale[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("WhitePeachScale");
                        }

                        if (pestsData.diseases?.AlternariaLeafSpot != null && pestsData.diseases.AlternariaLeafSpot[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("AlternariaLeafSpot");
                        }

                        if (pestsData.diseases?.BrownMarmoratedStinkBug != null && pestsData.diseases.BrownMarmoratedStinkBug[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("BrownMarmoratedStinkBug");
                        }

                        if (pestsData.diseases?.CabbageStemWeevil != null && pestsData.diseases.CabbageStemWeevil[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("CabbageStemWeevil");
                        }

                        if (pestsData.diseases?.CabbageStemFleaBeetle != null && pestsData.diseases.CabbageStemFleaBeetle[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("CabbageStemFleaBeetle");
                        }

                        if (pestsData.diseases?.PollenBeetle != null && pestsData.diseases.PollenBeetle[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("PollenBeetle");
                        }

                        if (pestsData.diseases?.AlternariaLeafBlight != null && pestsData.diseases.AlternariaLeafBlight[0])
                        {
                            numOfDiseasesToday++;
                            Debug.Log("AlternariaLeafBlight");
                        }

                        /////

                        pests.text = numOfDiseasesToday.ToString();
                    }
                }
            });
        }
        


        apisManager.GetComponent<WeatherWeeklyForecastAPI>().GetWeatherData(parcelId, (jsonResponseWeather) =>  //"90328"
        {
            weatherData = JsonUtility.FromJson<WeatherData>(jsonResponseWeather);

            if (weatherData != null)
            {
                temperatureToday.text = weatherData.data[0].temperature + weatherData.units.temperature;

                int minTemperature = 1000;
                int maxTemperature = (int)Math.Round(Mathf.NegativeInfinity);
                
                for (int i = 0; i < weatherData.data.Length; i++) 
                {
                    if (weatherData.data[i].temperature < minTemperature)
                        minTemperature = weatherData.data[i].temperature;

                    if (weatherData.data[i].temperature > maxTemperature)
                        maxTemperature = weatherData.data[i].temperature;
                }

                temperatureRange.text = minTemperature + "-" + maxTemperature + weatherData.units.temperature;
            }
        });



        GetComponent<Button>().onClick.AddListener(() =>
        {
            apisManager.GetComponent<ParcelsListAPI>().selectedParcelId = parcelId;

            GPSReceiverGameObject.GetComponent<ARFieldVisualizer>().GetSelectedParcelCoordinates();

            GPSReceiverGameObject.GetComponent<MenuManager>().OpenCloseSlateMenu(2);
        });


    }
}
