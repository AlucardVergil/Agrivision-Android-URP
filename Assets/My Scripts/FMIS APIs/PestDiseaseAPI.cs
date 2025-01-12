using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class PestDiseaseAPI : FMIS_API
{
    //private string apiKey = "AD37!edcNmJk9*k#23QeAX"; 

    void Start()
    {
        // Start the coroutine to fetch the pest and disease data
        //StartCoroutine(GetPestDiseaseDataEnumerator());
    }


    public void GetPestDiseaseData(string parcelID, Action<string> onPestsDataReceived)
    {
        StartCoroutine(GetPestDiseaseDataEnumerator(parcelID, onPestsDataReceived));
    }


    // Coroutine to fetch data from the pest and disease API
    IEnumerator GetPestDiseaseDataEnumerator(string parcelID, Action<string> onPestsDataReceived)
    {
        // Check if apiKey and parcelID are not empty or null
        if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(parcelID))
        {
            Debug.LogError("API key or Parcel ID is missing.");
            yield break;
        }

        string url = $"https://api.cropapp.gr/external/pd/summary?parcel_id={parcelID}";

        UnityWebRequest request = UnityWebRequest.Get(url);

        // Set the API key in the request header
        request.SetRequestHeader("apiKey", apiKey);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
            onPestsDataReceived?.Invoke(null); // Notify the caller that an error occurred
        }
        else
        {
            // Get the JSON response
            string jsonResponse = request.downloadHandler.text;
            Debug.Log("Pest and Disease Data: " + jsonResponse);

            onPestsDataReceived?.Invoke(jsonResponse);

            //ParsePestDiseaseData(jsonResponse);
        }
    }


    void ParsePestDiseaseData(string jsonResponse)
    {
        // Parse the JSON response using the PestDiseaseData class
        PestDiseaseData pdData = JsonUtility.FromJson<PestDiseaseData>(jsonResponse);

        Debug.Log("Dates " + pdData.dates);
        Debug.Log("Dates " + pdData.diseases.Altenaria);

        // Example: Log the data for Cotton Pink Bollworm on the first day
        if (pdData.dates.Length > 0 && pdData.diseases.Altenaria.Length > 0)
        {
            Debug.Log("Date: " + pdData.dates[0].date);
            Debug.Log("Altenaria present: " + pdData.diseases.Altenaria[0]);
        }
    }



    /*
     void ParsePestDiseaseData(string jsonResponse)
    {
        // Parse the JSON response using the PestDiseaseData class
        PestDiseaseData pdData = JsonUtility.FromJson<PestDiseaseData>(jsonResponse);

        Debug.Log("Dates " + pdData.dates);
        Debug.Log("Dates " + pdData.diseases.CottonPinkBollworm);

        // Example: Log the data for Cotton Pink Bollworm on the first day
        if (pdData.dates.Length > 0)// && pdData.diseases.CottonPinkBollworm.Length > 0)
        {
            Debug.Log("Date: " + pdData.dates[0].date);
            Debug.Log("Cotton Pink Bollworm present: " + pdData.diseases.CottonPinkBollworm[0]);
        }
    }
     */
}
