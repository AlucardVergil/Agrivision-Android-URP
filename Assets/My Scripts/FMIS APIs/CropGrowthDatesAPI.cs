using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class CropGrowthDatesAPI : FMIS_API
{


    public void GetCropGrowthDatesData(string parcelID, Action<string> onCropGrowthDatesDataReceived)
    {
        StartCoroutine(GetCropGrowthDatesDataEnumerator(parcelID, onCropGrowthDatesDataReceived));
    }



    // Coroutine to fetch data from the crop growth timeline API
    IEnumerator GetCropGrowthDatesDataEnumerator(string parcelID, Action<string> onCropGrowthDatesDataReceived)
    {
        if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(parcelID))
        {
            Debug.LogError("API key or Parcel ID is missing.");
            yield break;
        }

        string url = $"https://api.cropapp.gr/external/crop-growth-timeline?parcel_id={parcelID}";

        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("apiKey", apiKey);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
            onCropGrowthDatesDataReceived?.Invoke(null);
        }
        else if (request.responseCode == 412)  // Handle HTTP 412 error
        {
            Debug.LogError("Error 412: Precondition Failed. Check if the parcel_id or apiKey is correct.");
            onCropGrowthDatesDataReceived?.Invoke(null);
        }
        else
        {
            // Get the JSON response
            string jsonResponse = request.downloadHandler.text;
            Debug.Log("Crop Growth Data: " + jsonResponse);

            onCropGrowthDatesDataReceived?.Invoke(jsonResponse);

            ParseCropGrowthDatesData(jsonResponse);
        }
    }


    void ParseCropGrowthDatesData(string jsonResponse)
    {
        // Parse the JSON response using the CropGrowthData class
        CropGrowthDatesData cropGrowthData = JsonUtility.FromJson<CropGrowthDatesData>(jsonResponse);

        // Example: Log the data for each date
        foreach (var data in cropGrowthData.timelineData)
        {
            Debug.Log($"Date: {data.date}, Image Available: {data.flag}");
        }
    }
}
