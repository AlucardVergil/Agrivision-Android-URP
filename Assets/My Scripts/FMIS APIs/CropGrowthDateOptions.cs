using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CropGrowthDateOptions : MonoBehaviour
{
    private GameObject apisManager;
    private GameObject GPSReceiverGameObject;

    [HideInInspector] public string sensingDate;

    // Start is called before the first frame update
    void Start()
    {
        apisManager = GameObject.FindGameObjectWithTag("APIsManager");

        GPSReceiverGameObject = GameObject.Find("GPSReceiverGameObject");

        GetComponent<Button>().onClick.AddListener(() =>
        {        
            apisManager.GetComponent<CropGrowthImageAPI>().GetCropGrowthImage(apisManager.GetComponent<ParcelsListAPI>().selectedParcelId, sensingDate, (jsonResponseCropGrowthImage) =>   // "157212"
            {
                if (jsonResponseCropGrowthImage != null)
                {
                    Debug.Log("CROP IMAGE EXISTS");


                    Texture texture = TextureProcessor.DeleteWhitePixels(jsonResponseCropGrowthImage);

                    GPSReceiverGameObject.GetComponent<ARFieldVisualizer>().CreateFieldMesh(texture);
                }
                else
                    Debug.Log("CROP IMAGE DOES NOT EXIST");
            });
        });
    }

    
}