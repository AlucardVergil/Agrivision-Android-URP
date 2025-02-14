using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Globalization;

public class CropGrowthImageAPI : FMIS_API
{

    private string sensingDate = "2024-09-04";//"2025-01-04"; // Replace with the sensing date (YYYY-MM-DD)
    private int width = 400; // Replace with desired image width
    private int height = 400; // Replace with desired image height
    //private string bbox = "39.62955272340573,21.68499768346035,39.64197943254144,21.702216607555318"; // Replace with the bounding box coordinates
    //private string bbox = "39.63779011, 21.691270232, 39.634601604, 21.694571705"; // Replace with the bounding box coordinates
    //private string bbox = "39.636541, 21.692269, 39.635877, 21.693572";    
    //private string bbox = "39.635601, 21.693132, 39.636791, 21.692695";
    //private string bbox = "39.634601604, 21.691270232, 39.63779011, 21.694571705";
    //private string bbox =  "39.63571733485831, 21.69207602226019, 39.63726890772919, 21.694413601235784";
    private string bbox = "39.635601, 21.692269, 39.636791, 21.693572";

    public SpriteRenderer spriteRenderer; // Renderer to display the texture 
    public Image test;

    public ARFieldVisualizer arFieldVisualizer;


    public void GetCropGrowthImage(string parcelID, Action<Texture2D> onCropGrowthImageDataReceived)
    {
        // Start the coroutine to fetch the crop growth image
        StartCoroutine(GetCropGrowthImageEnumerator(parcelID, onCropGrowthImageDataReceived));
    }

    // Coroutine to fetch the crop growth image from the API
    IEnumerator GetCropGrowthImageEnumerator(string parcelID, Action<Texture2D> onCropGrowthImageDataReceived)
    {
        if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(parcelID) || string.IsNullOrEmpty(bbox))
        {
            Debug.LogError("API key, Parcel ID, or BBOX is missing.");
            yield break;
        }

        string bbox2 = GetSelectedParcelBbox();

        Debug.Log($"bbox ===> {bbox2}");

        string url = $"https://api.cropapp.gr/external/crop-growth-image?parcel_id={parcelID}&sensing_date={sensingDate}&WIDTH={width}&HEIGHT={height}&BBOX={bbox2}";

        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        request.SetRequestHeader("apiKey", apiKey);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
            onCropGrowthImageDataReceived?.Invoke(null);
        }
        else if (request.responseCode == 412)  // Handle HTTP 412 error
        {
            Debug.LogError("Error 412: Precondition Failed. Check if the parcel_id, apiKey, sensing_date, or BBOX is correct.");
            onCropGrowthImageDataReceived?.Invoke(null);
        }
        else
        {
            // Get the texture from the API response
            Texture2D cropGrowthImage = DownloadHandlerTexture.GetContent(request);

            string jsonResponse = request.downloadHandler.text;
            Debug.Log("CropGrowthImage Data: " + jsonResponse);

            if (cropGrowthImage != null)
            {
                Debug.Log("Crop Growth Image fetched successfully. " + cropGrowthImage);
            }                
            else
            {
                Debug.Log("Crop Growth Image fetch failed.");
                Debug.Log(cropGrowthImage);
            }
            
            Texture2D texture = cropGrowthImage; // Assuming cropGrowthImage is Texture2D
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            test.sprite = sprite;


            onCropGrowthImageDataReceived?.Invoke(cropGrowthImage);

            // Assign the texture to a material's main texture to display the image
            if (spriteRenderer != null)
            {
                spriteRenderer.material.mainTexture = cropGrowthImage;
            }
        }
    }


    // Calculates the correct bbox based on parcel coordinates (bottom-left corner and top-right corner). These are not the actual parcel coords but the box surrounding the parcel
    private string GetSelectedParcelBbox()
    {
        Vector2[] fieldCorners = GPSBoundingBox.GetBoundingSquare(arFieldVisualizer.fieldCorners);

        //float bboxAdjustmentValue = 0.00119f;
        float bboxAdjustmentValue = 0.0013f; // Adjust the bbox to make it slightly bigger that the actual parcel coordinates so it doesn't cut off a little bit of the edges

        // Return the bbox coords and replace the decimal from comma to dot
        return $"{(fieldCorners[1].x - bboxAdjustmentValue).ToString(CultureInfo.InvariantCulture)}, {(fieldCorners[1].y - bboxAdjustmentValue).ToString(CultureInfo.InvariantCulture)}, " +
            $"{(fieldCorners[3].x + bboxAdjustmentValue).ToString(CultureInfo.InvariantCulture)}, {(fieldCorners[3].y + bboxAdjustmentValue).ToString(CultureInfo.InvariantCulture)}";        
    }

}
