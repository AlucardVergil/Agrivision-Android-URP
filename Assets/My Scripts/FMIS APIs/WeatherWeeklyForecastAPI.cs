using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;


public class WeatherWeeklyForecastAPI : FMIS_API
{
    //private string apiKey = "AD37!edcNmJk9*k#23QeAX"; 
    //private string parcelID = "90328"; 

    private string jsonResponse;

    void Start()
    {
        // Start the coroutine to fetch weather data
        //StartCoroutine(GetWeatherDataEnumerator());
    }



    public void GetWeatherData(string parcelID, Action<string> onWeatherDataReceived)
    {
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
                    }
                }
            }
        }
    }

}
