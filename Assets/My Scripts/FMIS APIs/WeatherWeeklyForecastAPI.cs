using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using TMPro;
using UnityEngine.UIElements;
using System.Collections.Generic;


public class WeatherWeeklyForecastAPI : FMIS_API
{
    //private string apiKey = "AD37!edcNmJk9*k#23QeAX"; 
    //private string parcelID = "90328"; 

    private string jsonResponse;

    public TMP_Text weatherNowText;
    public TMP_Text temperatureText;   // Temperature unit (°C)
    public TMP_Text weatherIconText;
    public Image weatherIcon;
    public TMP_Text precipitation_probText; // Precipitation probability unit (%)
    public TMP_Text precipitationText; // Precipitation unit (mm)
    public TMP_Text humidityText;      // Humidity unit (%)
    public TMP_Text evapotranspirationText; // Evapotranspiration unit (mm)
    public TMP_Text windText;
    public TMP_Text alertText;

    public Sprite[] weatherIconSprites;
    private Dictionary<string, Sprite> weatherIconsDictionary;
    private string[] weatherConditions =
    {
        "sun_clouds"
    };


    void Start()
    {
        // Start the coroutine to fetch weather data
        //StartCoroutine(GetWeatherDataEnumerator());
    }



    public void GetWeatherData(string parcelID, Action<string> onWeatherDataReceived)
    {
        weatherIconsDictionary = new Dictionary<string, Sprite>();

        if (weatherIconSprites.Length != weatherConditions.Length)
        {
            Debug.LogError("The number of weather condition keys and sprites do not match.");
            return;
        }

        for (int i = 0; i < weatherConditions.Length; i++)
        {
            weatherIconsDictionary[weatherConditions[i]] = weatherIconSprites[i];
        }

        StartCoroutine(GetWeatherDataEnumerator(parcelID, onWeatherDataReceived));
    }



    // Coroutine to fetch data from the weather API
    IEnumerator GetWeatherDataEnumerator(string parcelID, Action<string> onWeatherDataReceived)
    {
        // Create the request URL by appending the parcel ID
        string url = $"https://api.cropapp.gr/external/weather-weekly?parcel_id={parcelID}";

        // Create the UnityWebRequest with GET method
        UnityWebRequest request = UnityWebRequest.Get(url);

        // Set the API key header
        request.SetRequestHeader("apiKey", apiKey);

        // Send the request and wait for the response
        yield return request.SendWebRequest();

        // Check for network or HTTP errors
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
            onWeatherDataReceived?.Invoke(null); // Notify the caller that an error occurred
        }
        else
        {
            // Successful request, log the response
            jsonResponse = request.downloadHandler.text;
            Debug.Log("Weather Data: " + jsonResponse);

            // Pass the result back through the callback
            onWeatherDataReceived?.Invoke(jsonResponse);


            // Parse the JSON response into C# objects
            ParseWeatherData(jsonResponse);
        }
    }

    // Method to parse the JSON data and log some information
    void ParseWeatherData(string jsonResponse)
    {
        // Parse the JSON response into the WeatherData structure
        WeatherData weatherData = JsonUtility.FromJson<WeatherData>(jsonResponse);

        // Log some information about the forecast
        if (weatherData.data.Length > 0)
        {
            weatherNowText.text = "Date: " + weatherData.data[0].span_label + "\n\n" + weatherData.data[0].temperature + weatherData.units.temperature + "\n" +
                weatherData.data[0].weather_icon;

            if (weatherIconsDictionary.ContainsKey(weatherData.data[0].weather_icon))
            {                
                weatherIcon.sprite = weatherIconsDictionary[weatherData.data[0].weather_icon];
            }
                

            //temperatureText.text = weatherData.data[0].temperature + weatherData.units.temperature;
            precipitation_probText.text = weatherData.data[0].precipitation_prob + weatherData.units.precipitation_prob + "\nPrecipitation Probability";

            windText.text = weatherData.data[0].wind + weatherData.units.wind + "\nWind Speed";

            for (int i = 0; i < weatherData.data.Length; i++)
            {
                Debug.Log("Date: " + weatherData.data[i].span_label);
                Debug.Log("Temperature: " + weatherData.data[i].temperature + weatherData.units.temperature);
                Debug.Log("Precipitation Probability: " + weatherData.data[i].precipitation_prob + weatherData.units.precipitation_prob);
                Debug.Log("Wind Speed: " + weatherData.data[i].wind + weatherData.units.wind);

                


                // If there are alerts, display them
                if (weatherData.data[i].alerts.Length > 0)
                {
                    foreach (var alert in weatherData.data[i].alerts)
                    {
                        Debug.Log("Alert: " + alert.title + " | Type: " + alert.type + " | Severity: " + alert.severity_level);
                        alertText.text += "\n\nAlert: " + alert.title + " | Type: " + alert.type + " | Severity: " + alert.severity_level;
                    }
                }
            }
        }
    }

}
