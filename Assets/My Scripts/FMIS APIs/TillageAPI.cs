using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class TillageAPI : FMIS_API
{
    public TMP_Text label;
    public TMP_Text infoText;


    public void GetTillageData(string parcelID, Action<string> onTillageDataReceived)
    {
        // Start the coroutine to fetch the pest and disease data
        StartCoroutine(GetTillageDataEnumerator(parcelID, onTillageDataReceived));
    }

    // Coroutine to fetch data from the pest and disease API
    IEnumerator GetTillageDataEnumerator(string parcelID, Action<string> onTillageDataReceived)
    {
        // Check if apiKey and parcelID are not empty or null
        if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(parcelID))
        {
            Debug.LogError("API key or Parcel ID is missing.");
            yield break;
        }

        Debug.Log("FROM date: " + fromDate);

        string url = $"https://api.cropapp.gr/external/tillage?parcel_id={parcelID}&from_date={fromDate}&to_date={toDate}";

        UnityWebRequest request = UnityWebRequest.Get(url);

        // Set the API key in the request header
        request.SetRequestHeader("apiKey", apiKey);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
            onTillageDataReceived?.Invoke(null);
        }
        else
        {
            // Get the JSON response
            string jsonResponse = request.downloadHandler.text;
            Debug.Log("Tillage Data: " + jsonResponse);

            onTillageDataReceived?.Invoke(jsonResponse);

            ParseTillageData(jsonResponse);
        }
    }


    void ParseTillageData(string jsonResponse)
    {
        // Parse the JSON response using the PestDiseaseData class
        TillageData tillageData = JsonUtility.FromJson<TillageData>(jsonResponse);

        // Example: Log the data for the first date
        if (tillageData.tillage_chart_data.Length > 0)
        {
            Debug.Log("Date: " + tillageData.tillage_chart_data[0].date);
            Debug.Log("Soil Moisture: " + tillageData.tillage_chart_data[0].value);
            infoText.text = "Date: " + tillageData.tillage_chart_data[0].date;
            infoText.text += "\nSoil Moisture: " + tillageData.tillage_chart_data[0].value;
        }

        // Log the thresholds
        Debug.Log("Threshold 1 (Ideal): " + tillageData.threshold_1);
        Debug.Log("Threshold 2 (Good): " + tillageData.threshold_2);
        Debug.Log("Threshold 3 (Average): " + tillageData.threshold_3);
        Debug.Log("Threshold 4 (Bad): " + tillageData.threshold_4);
        infoText.text += "\n\nThreshold 1 (Ideal): " + tillageData.threshold_1;
        infoText.text += "\nThreshold 1 (Ideal): " + tillageData.threshold_2;
        infoText.text += "\nThreshold 1 (Ideal): " + tillageData.threshold_3;
        infoText.text += "\nThreshold 1 (Ideal): " + tillageData.threshold_4;
    }
}
