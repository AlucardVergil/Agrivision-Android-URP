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



        apisManager.GetComponent<PestDiseaseAPI>().GetPestDiseaseData(parcelId, (jsonResponsePests) => //"11256"
        {
            pestsData = JsonUtility.FromJson<PestDiseaseData>(jsonResponsePests);
            
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
            
                pests.text = numOfDiseasesToday.ToString();
            }
        });


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
