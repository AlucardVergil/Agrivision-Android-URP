using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using TMPro;

public class IrrigationAPI : FMIS_API
{
    public TMP_Text label;
    public TMP_Text infoText;


    public void GetIrrigationData(string parcelID, Action<string> onIrrigationDataReceived)
    {
        // Start the coroutine to fetch the irrigation data
        StartCoroutine(GetIrrigationDataEnumerator(parcelID, onIrrigationDataReceived));
    }

    // Coroutine to fetch data from the irrigation API
    IEnumerator GetIrrigationDataEnumerator(string parcelID, Action<string> onIrrigationDataReceived)
    {
        if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(parcelID) || string.IsNullOrEmpty(fromDate) || string.IsNullOrEmpty(toDate))
        {
            Debug.LogError("API key, Parcel ID, or date range is missing.");
            yield break;
        }

        string url = $"https://api.cropapp.gr/external/irrigation?parcel_id={parcelID}&from_date={fromDate}&to_date={toDate}";

        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("apiKey", apiKey);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
            onIrrigationDataReceived?.Invoke(null);
        }
        else if (request.responseCode == 412)  // Handle HTTP 412 error
        {
            Debug.LogError("Error 412: Precondition Failed. Check if the parcel_id, apiKey, or date range is correct.");
            onIrrigationDataReceived?.Invoke(null);
        }
        else
        {
            // Get the JSON response
            string jsonResponse = request.downloadHandler.text;
            Debug.Log("Irrigation Data: " + jsonResponse);

            onIrrigationDataReceived?.Invoke(jsonResponse);

            ParseIrrigationData(jsonResponse);
        }
    }


    void ParseIrrigationData(string jsonResponse)
    {
        // Parse the JSON response using the IrrigationData class
        IrrigationData irrigationData = JsonUtility.FromJson<IrrigationData>(jsonResponse);

        // Example: Log the data for the first date
        if (irrigationData.chart.Length > 0)
        {
            Debug.Log("Date: " + irrigationData.chart[0].date);
            Debug.Log("Crop ET: " + irrigationData.chart[0].crop_et);
            Debug.Log("Precipitation: " + irrigationData.chart[0].precipitation);
            Debug.Log("Irrigation Amount: " + (irrigationData.chart[0].irrigation_amount.HasValue ? irrigationData.chart[0].irrigation_amount.Value.ToString() : "null"));

            infoText.text = "\nDate: " + irrigationData.chart[0].date;
            infoText.text += "\nCrop ET: " + irrigationData.chart[0].crop_et;
            infoText.text += "\nPrecipitation: " + irrigationData.chart[0].precipitation;
            infoText.text += "\nIrrigation Amount: " + (irrigationData.chart[0].irrigation_amount.HasValue ? irrigationData.chart[0].irrigation_amount.Value.ToString() : "null");


        }

        // Loop through the data and log each record
        for (int i = 0; i < irrigationData.chart.Length; i++)
        {
            Debug.Log($"Date: {irrigationData.chart[i].date}, Crop ET: {irrigationData.chart[i].crop_et}, Precipitation: {irrigationData.chart[i].precipitation}, Irrigation Amount: {(irrigationData.chart[i].irrigation_amount.HasValue ? irrigationData.chart[i].irrigation_amount.Value.ToString() : "null")}");
        }
    }
}
