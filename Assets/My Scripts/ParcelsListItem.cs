using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using Newtonsoft.Json;

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
                    pestsData = JsonConvert.DeserializeObject<PestDiseaseData>(jsonResponsePests);

                    Debug.Log("CodlingMoth " + pestsData.diseases);

                    if (pestsData != null)
                    {
                        int numOfDiseasesToday = 0;

                        for (int i = 0; i < 7; i++)
                        {
                            if (pestsData.diseases?.Altenaria != null && pestsData.diseases.Altenaria[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("Altenaria");
                            }

                            //Debug.Log("Anthracnose " + pestsData.diseases.Anthracnose[i]);
                            if (pestsData.diseases?.Anthracnose != null && pestsData.diseases.Anthracnose[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("Anthracnose");
                            }

                            if (pestsData.diseases?.BlackAphid != null && pestsData.diseases.BlackAphid[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("BlackAphid");
                            }

                            if (pestsData.diseases?.BlackParlatoriaScale != null && pestsData.diseases.BlackParlatoriaScale[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("BlackParlatoriaScale");
                            }

                            if (pestsData.diseases?.CitrusLeafminer != null && pestsData.diseases.CitrusLeafminer[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("CitrusLeafminer");
                            }

                            if (pestsData.diseases?.CitrusMealybug != null && pestsData.diseases.CitrusMealybug[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("CitrusMealybug");
                            }

                            if (pestsData.diseases?.CitrusSpinyWhitefly != null && pestsData.diseases.CitrusSpinyWhitefly[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("CitrusSpinyWhitefly");
                            }

                            if (pestsData.diseases?.CottonyCushionScale != null && pestsData.diseases.CottonyCushionScale[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("CottonyCushionScale");
                            }


                            /////

                            if (pestsData.diseases?.PinkBollworm != null && pestsData.diseases.PinkBollworm[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("PinkBollworm");
                            }

                            if (pestsData.diseases?.Bollworm != null && pestsData.diseases.Bollworm[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("Bollworm");
                            }

                            if (pestsData.diseases?.TarnishedPlantBug != null && pestsData.diseases.TarnishedPlantBug[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("TarnishedPlantBug");
                            }

                            if (pestsData.diseases?.Jassids != null && pestsData.diseases.Jassids[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("Jassids");
                            }

                            if (pestsData.diseases?.Aphids != null && pestsData.diseases.Aphids[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("Aphids");
                            }

                            if (pestsData.diseases?.Thrips != null && pestsData.diseases.Thrips[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("Thrips");
                            }

                            if (pestsData.diseases?.Whitefly != null && pestsData.diseases.Whitefly[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("Whitefly");
                            }

                            if (pestsData.diseases?.TwoSpottedSpiderMite != null && pestsData.diseases.TwoSpottedSpiderMite[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("TwoSpottedSpiderMite");
                            }

                            if (pestsData.diseases?.BotrytisGreyMould != null && pestsData.diseases.BotrytisGreyMould[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("BotrytisGreyMould");
                            }

                            //Debug.Log("PowderyMildew " + pestsData.diseases.PowderyMildew[i]);
                            if (pestsData.diseases?.PowderyMildew != null && pestsData.diseases.PowderyMildew[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("PowderyMildew");
                            }

                            if (pestsData.diseases?.PhomopsisLeafSpot != null && pestsData.diseases.PhomopsisLeafSpot[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("PhomopsisLeafSpot");
                            }

                            if (pestsData.diseases?.DownyMildew != null && pestsData.diseases.DownyMildew[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("DownyMildew");
                            }

                            if (pestsData.diseases?.GrapeBerryMoth != null && pestsData.diseases.GrapeBerryMoth[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("GrapeBerryMoth");
                            }

                            if (pestsData.diseases?.PlanococcusFicus != null && pestsData.diseases.PlanococcusFicus[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("PlanococcusFicus");
                            }

                            if (pestsData.diseases?.Leafhopper != null && pestsData.diseases.Leafhopper[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("Leafhopper");
                            }

                            if (pestsData.diseases?.Earworm != null && pestsData.diseases.Earworm[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("Earworm");
                            }

                            if (pestsData.diseases?.MediterraneanStalkborer != null && pestsData.diseases.MediterraneanStalkborer[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("MediterraneanStalkborer");
                            }

                            if (pestsData.diseases?.WesternRootWormBeetle != null && pestsData.diseases.WesternRootWormBeetle[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("WesternRootWormBeetle");
                            }

                            if (pestsData.diseases?.Helminthosporium != null && pestsData.diseases.Helminthosporium[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("Helminthosporium");
                            }

                            if (pestsData.diseases?.SpeckledLeafBlotch != null && pestsData.diseases.SpeckledLeafBlotch[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("SpeckledLeafBlotch");
                            }

                            if (pestsData.diseases?.LeafBeetle != null && pestsData.diseases.LeafBeetle[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("LeafBeetle");
                            }

                            if (pestsData.diseases?.Rynchosporium != null && pestsData.diseases.Rynchosporium[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("Rynchosporium");
                            }

                            if (pestsData.diseases?.Beetle != null && pestsData.diseases.Beetle[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("Beetle");
                            }

                            if (pestsData.diseases?.EarlyBlight != null && pestsData.diseases.EarlyBlight[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("EarlyBlight");
                            }

                            if (pestsData.diseases?.LateBlight != null && pestsData.diseases.LateBlight[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("LateBlight");
                            }

                            if (pestsData.diseases?.Tuberworm != null && pestsData.diseases.Tuberworm[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("Tuberworm");
                            }

                            if (pestsData.diseases?.BlackSpot != null && pestsData.diseases.BlackSpot[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("BlackSpot");
                            }

                            if (pestsData.diseases?.FruitFly != null && pestsData.diseases.FruitFly[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("FruitFly");
                            }

                            Debug.Log($"Leafspot: {pestsData.diseases?.LeafSpot}");
                            if (pestsData.diseases?.LeafSpot != null && pestsData.diseases.LeafSpot[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("LeafSpot");
                            }

                            if (pestsData.diseases?.Moth != null && pestsData.diseases.Moth[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("Moth");
                            }

                            if (pestsData.diseases?.Psyllid != null && pestsData.diseases.Psyllid[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("Psyllid");
                            }

                            if (pestsData.diseases?.Scale != null && pestsData.diseases.Scale[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("Scale");
                            }

                            if (pestsData.diseases?.CalocorisBug != null && pestsData.diseases.CalocorisBug[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("CalocorisBug");
                            }

                            if (pestsData.diseases?.CercosporaLeafSpot != null && pestsData.diseases.CercosporaLeafSpot[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("CercosporaLeafSpot");
                            }

                            if (pestsData.diseases?.LeafMiner != null && pestsData.diseases.LeafMiner[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("LeafMiner");
                            }

                            if (pestsData.diseases?.TetranychusUrticae != null && pestsData.diseases.TetranychusUrticae[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("TetranychusUrticae");
                            }

                            if (pestsData.diseases?.GrayMold != null && pestsData.diseases.GrayMold[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("GrayMold");
                            }

                            if (pestsData.diseases?.Alternaria != null && pestsData.diseases.Alternaria[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("Suf lowerAlternaria");
                            }

                            if (pestsData.diseases?.Phomopsis != null && pestsData.diseases.Phomopsis[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("Phomopsis");
                            }

                            if (pestsData.diseases?.CodlingMoth != null && pestsData.diseases.CodlingMoth[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("CodlingMoth");
                            }

                            if (pestsData.diseases?.SpiderMites != null && pestsData.diseases.SpiderMites[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("SpiderMites");
                            }

                            //Debug.Log("SanJoseScale " + pestsData.diseases.SanJoseScale[i]);
                            if (pestsData.diseases?.SanJoseScale != null && pestsData.diseases.SanJoseScale[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("SanJoseScale");
                            }

                            if (pestsData.diseases?.Scab != null && pestsData.diseases.Scab[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("Scab");
                            }

                            if (pestsData.diseases?.FireBlight != null && pestsData.diseases.FireBlight[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("FireBlight");
                            }

                            if (pestsData.diseases?.TwigBorer != null && pestsData.diseases.TwigBorer[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("TwigBorer");
                            }

                            if (pestsData.diseases?.OrientalFruitMoth != null && pestsData.diseases.OrientalFruitMoth[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("OrientalFruitMoth");
                            }

                            if (pestsData.diseases?.SummerFruitTortrix != null && pestsData.diseases.SummerFruitTortrix[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("SummerFruitTortrix");
                            }

                            if (pestsData.diseases?.BrownRot != null && pestsData.diseases.BrownRot[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("BrownRot");
                            }

                            if (pestsData.diseases?.LeafCurl != null && pestsData.diseases.LeafCurl[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("LeafCurl");
                            }

                            if (pestsData.diseases?.Weevil != null && pestsData.diseases.Weevil[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("Weevil");
                            }

                            if (pestsData.diseases?.SpottedWingDrosophila != null && pestsData.diseases.SpottedWingDrosophila[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("SpottedWingDrosophila");
                            }

                            if (pestsData.diseases?.ShotHole != null && pestsData.diseases.ShotHole[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("ShotHole");
                            }

                            if (pestsData.diseases?.LeafScorch != null && pestsData.diseases.LeafScorch[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("LeafScorch");
                            }

                            if (pestsData.diseases?.WhitePeachScale != null && pestsData.diseases.WhitePeachScale[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("WhitePeachScale");
                            }

                            if (pestsData.diseases?.AlternariaLeafSpot != null && pestsData.diseases.AlternariaLeafSpot[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("AlternariaLeafSpot");
                            }

                            if (pestsData.diseases?.BrownMarmoratedStinkBug != null && pestsData.diseases.BrownMarmoratedStinkBug[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("BrownMarmoratedStinkBug");
                            }

                            if (pestsData.diseases?.CabbageStemWeevil != null && pestsData.diseases.CabbageStemWeevil[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("CabbageStemWeevil");
                            }

                            if (pestsData.diseases?.CabbageStemFleaBeetle != null && pestsData.diseases.CabbageStemFleaBeetle[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("CabbageStemFleaBeetle");
                            }

                            if (pestsData.diseases?.PollenBeetle != null && pestsData.diseases.PollenBeetle[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("PollenBeetle");
                            }

                            if (pestsData.diseases?.AlternariaLeafBlight != null && pestsData.diseases.AlternariaLeafBlight[i])
                            {
                                numOfDiseasesToday++;
                                Debug.Log("AlternariaLeafBlight");
                            }
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
