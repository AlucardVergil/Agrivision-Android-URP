using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using TMPro;

public class FertilizationAPI : FMIS_API
{
    public TMP_Text label;
    public TMP_Text infoText;



    public void GetFertilizationData(string parcelID, Action<string> onFertilizationDataReceived)
    {
        // Start the coroutine to fetch the fertilization data
        StartCoroutine(GetFertilizationDataEnumerator(parcelID, onFertilizationDataReceived));
    }

    // Coroutine to fetch data from the management zones API
    IEnumerator GetFertilizationDataEnumerator(string parcelID, Action<string> onFertilizationDataReceived)
    {
        if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(parcelID))
        {
            Debug.LogError("API key or Parcel ID is missing.");
            yield break;
        }

        string url = $"https://api.cropapp.gr/external/management-zones?parcel_id={parcelID}";

        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("apiKey", apiKey);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
            onFertilizationDataReceived?.Invoke(null);
        }
        else if (request.responseCode == 412)  // Handle HTTP 412 error
        {
            Debug.LogError("Error 412: Precondition Failed. Check if the parcel_id or apiKey is correct.");
            onFertilizationDataReceived?.Invoke(null);
        }
        else
        {
            // Get the JSON response
            string jsonResponse = request.downloadHandler.text;
            Debug.Log("Fertilization Data: " + jsonResponse);

            onFertilizationDataReceived?.Invoke(jsonResponse);

            //ParseFertilizationData(jsonResponse);
        }
    }


    public Texture2D ParseFertilizationData(string jsonResponse)
    {
        // Parse the JSON response using the FertilizationData class
        FertilizationData fertilizationData = JsonUtility.FromJson<FertilizationData>("{\"managementZones\":" + jsonResponse + "}");

        // Example: Log the data for the most recent season
        if (fertilizationData.managementZones.Length > 0)
        {
            Debug.Log("Season: " + fertilizationData.managementZones[0].season);
            Debug.Log("Image (Base64): " + (string.IsNullOrEmpty(fertilizationData.managementZones[0].image) ? "No image available" : "Image available"));
            infoText.text = "Season: " + fertilizationData.managementZones[0].season;
            infoText.text += "\nImage (Base64): " + (string.IsNullOrEmpty(fertilizationData.managementZones[0].image) ? "No image available" : "Image available");
        }

        // Loop through the seasons and log each one
        for (int i = 0; i < fertilizationData.managementZones.Length; i++)
        {
            Debug.Log($"Season: {fertilizationData.managementZones[i].season}, Image: {(string.IsNullOrEmpty(fertilizationData.managementZones[i].image) ? "No image available" : "Image available")}");
            infoText.text += $"\n\nSeason: {fertilizationData.managementZones[i].season}, Image: {(string.IsNullOrEmpty(fertilizationData.managementZones[i].image) ? "No image available" : "Image available")}";
        }


        // Set the image texture
        if (fertilizationData.managementZones.Length > 0 && !string.IsNullOrEmpty(fertilizationData.managementZones[0].image))
        {
            Texture2D parcelImage = Base64ToTexture(fertilizationData.managementZones[0].image);
            //imageUI.texture = parcelImage;  // Set the texture to the RawImage component
            return parcelImage;
        }

        return null;
    }



    // Function to convert Base64 string to Texture2D
    public Texture2D Base64ToTexture(string base64)
    {
        byte[] imageBytes = System.Convert.FromBase64String(base64);
        Texture2D texture = new Texture2D(9, 16);
        texture.LoadImage(imageBytes); // Automatically resizes the texture based on image size
        return texture;
    }

}