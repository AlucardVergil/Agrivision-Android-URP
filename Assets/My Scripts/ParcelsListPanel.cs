using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ParcelsListPanel : MonoBehaviour
{
    public TMP_Text parcelTitle;
    public TMP_Text parcelAcreage;
    public TMP_Text temperatureRange;
    public TMP_Text parcelText;
    public TMP_Text pests;
    public TMP_Text temperatureToday;

    GameObject apisManager;

    WeatherData weatherData;
    PestDiseaseData pestsData;


    // Start is called before the first frame update
    void Start()
    {
        apisManager = GameObject.FindGameObjectWithTag("APIsManager");
                
        apisManager.GetComponent<PestDiseaseAPI>().GetPestDiseaseData("11256", (jsonResponsePests) =>
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

                Debug.Log("Anthracnose " + pestsData.diseases.Anthracnose[0]);
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


        apisManager.GetComponent<WeatherWeeklyForecastAPI>().GetWeatherData("90328", (jsonResponseWeather) =>
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





    }
}
