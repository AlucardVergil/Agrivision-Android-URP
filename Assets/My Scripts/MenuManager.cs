using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject[] slate;

    [SerializeField] private GameObject[] pestsPrefabs;
    [SerializeField] private GameObject[] pestsInfoPanels;


    private GameObject pest;
    private GameObject pestPanel;

    private GameObject activePanel;

    private GameObject apisManager;
    private GameObject GPSReceiverGameObject;


    private void Start()
    {
        apisManager = GameObject.FindGameObjectWithTag("APIsManager");
        GPSReceiverGameObject = GameObject.Find("GPSReceiverGameObject");

        for (int j = 0; j < slate.Length; j++)
        {
            slate[j].SetActive(false);
        }
    }


    public void OpenCloseSlateMenu(int index)
    {
        StartCoroutine(OpenCloseSlateMenuEnumerator(index));
    }



    IEnumerator OpenCloseSlateMenuEnumerator(int i)
    {
        // Find 1st game object with FollowButton tag and untoggle it since i only have 1 panel active at a time
        // Placed ? for nullable to fix null reference exception
       // GameObject.FindGameObjectWithTag("FollowButton")?.GetComponent<Button>().ForceSetToggled(false);

        for (int slateIndex = 0; slateIndex < slate.Length; slateIndex++)
        {
            if (i != slateIndex && slate[slateIndex].activeSelf)
            {
                if (slate[slateIndex].GetComponentInChildren<SpriteDropHandler>() != null)
                {
                    for (int modelIndex = 0; modelIndex < slate[slateIndex].GetComponentInChildren<SpriteDropHandler>().modelInstance.Length; modelIndex++)
                    {
                        if (slate[slateIndex].GetComponentInChildren<SpriteDropHandler>().modelInstance[modelIndex] != null)
                            Destroy(slate[slateIndex].GetComponentInChildren<SpriteDropHandler>().modelInstance[modelIndex]);
                    }
                }

                //slate[slateIndex].GetComponent<Follow>().enabled = false;

                yield return new WaitForEndOfFrame();

                slate[slateIndex].SetActive(false);
                break;  // Break the loop early once active panel is found. ONLY if i want one active panel at a time
            }
                
        }

        slate[i].SetActive(!slate[i].activeSelf);

        activePanel = slate[i];


        // Execute API calls based on which panel is active and display them
        if (apisManager.GetComponent<ParcelsListAPI>().selectedParcelId != null)
        {
            switch (i)
            {
                case 1: // Pest Info Panel
                    apisManager.GetComponent<ParcelsListAPI>().GetParcelsListData((jsonResponseParcelsList) =>
                    {
                        Debug.Log("Parcels List API => " + jsonResponseParcelsList);
                    });
                    break;

                case 3: // Pest Info Panel

                    break;

                case 4: // Parcel Info Panel

                    break;

                case 5: // Crop Growth Panel
                    apisManager.GetComponent<CropGrowthImageAPI>().GetCropGrowthImage(apisManager.GetComponent<ParcelsListAPI>().selectedParcelId, (jsonResponseCropGrowthImage) =>   // "157212"
                    {
                        if (jsonResponseCropGrowthImage != null)
                        {
                            Debug.Log("CROP IMAGE EXISTS");

                            //CreateFieldMesh(jsonResponseCropGrowthImage);
                        }
                        else
                            Debug.Log("CROP IMAGE DOES NOT EXIST");
                    });
                    break;

                case 6: // Fertilization Panel
                    apisManager.GetComponent<FertilizationAPI>().GetFertilizationData(apisManager.GetComponent<ParcelsListAPI>().selectedParcelId, (jsonResponseFertilization) =>   // "5836"
                    {
                        Debug.Log("fertilizationData => " + jsonResponseFertilization);

                        Texture2D texture = apisManager.GetComponent<FertilizationAPI>().ParseFertilizationData(jsonResponseFertilization);


                        //var croppedTexture = ARFieldVisualizer.CropWhiteSpaces(texture);

                        GPSReceiverGameObject.GetComponent<ARFieldVisualizer>().CreateFieldMesh(texture);
                    });
                    break;

                case 7: // Irrigation Panel
                    apisManager.GetComponent<IrrigationAPI>().GetIrrigationData(apisManager.GetComponent<ParcelsListAPI>().selectedParcelId, (jsonResponseIrrigation) =>  // "11256"
                    {
                        Debug.Log(jsonResponseIrrigation);


                    });
                    break;

                case 8: // Tillage Panel
                    apisManager.GetComponent<TillageAPI>().GetTillageData(apisManager.GetComponent<ParcelsListAPI>().selectedParcelId, (jsonResponseTillage) =>  // "10478"
                    {
                        Debug.Log(jsonResponseTillage);


                    });
                    break;

                case 9: // Weather Panel
                    apisManager.GetComponent<WeatherWeeklyForecastAPI>().GetWeatherData(apisManager.GetComponent<ParcelsListAPI>().selectedParcelId, (jsonResponseTillage) =>   // "10478"
                    {
                        Debug.Log(jsonResponseTillage);


                    });
                    break;
            }
        }

        //if (slate[i].activeSelf)
        //{
        //    slate[i].GetComponent<Follow>().enabled = true;

        //    yield return new WaitForEndOfFrame();

        //    slate[i].GetComponent<Follow>().enabled = false;
        //}

        yield return null;
    }


    public void Destroy3DModels()
    {
        GameObject obj;

        for (int j = 0; j < activePanel.GetComponentInChildren<SpriteDropHandler>().modelInstance.Length; j++)
        {
            obj = activePanel.GetComponentInChildren<SpriteDropHandler>().modelInstance[j];

            if (obj != null)
                Destroy(obj);
        }


    }

    public void EnableDisablePanelHolograms()
    {
        GameObject obj;

        for (int j = 0; j < activePanel.GetComponentInChildren<SpriteDropHandler>().modelInstance.Length; j++)
        {
            obj = activePanel.GetComponentInChildren<SpriteDropHandler>().modelInstance[j];

            if (obj != null)
                obj.SetActive(!obj.activeSelf);

        }
    }


    #region Maybe to Delete

    IEnumerator InstantiatePestsPanelAnd3DModelEnumerator(bool onlyHologram)
    {
        // Instantiate pest 3D model and info panel and place them at 0,0,0 because the position will change with the follow component anyway
        pest = Instantiate(pestsPrefabs[0], new Vector3(0, 0, 0), Quaternion.identity);

        // Bring them in front of you
        //pest.GetComponent<Follow>().enabled = true;

        if (!onlyHologram)
        {
            pestPanel = Instantiate(pestsInfoPanels[0], new Vector3(0, 0, 0), Quaternion.identity);
            //pestPanel.GetComponent<Follow>().enabled = true;
        }

        yield return new WaitForEndOfFrame();

        //pest.GetComponent<Follow>().enabled = false;

        if (!onlyHologram)
        {
            //pestPanel.GetComponent<Follow>().enabled = false;

            // Add onClick event during runtime because I can't assign the event from inspector due to pestPanel being an instantiated prefab and the MenuManager is already in scene
            pestPanel.transform.Find("TitleBar/Buttons/ToggleHologramButton").GetComponent<Button>().onClick.AddListener(() => EnableDisableHolograms());

            pestPanel.transform.Find("TitleBar/Buttons/CloseButton").GetComponent<Button>().onClick.AddListener(() => DestroyPest3DModel());
        }

    }


    public void InstantiatePestsPanelAnd3DModel(bool onlyHologram)
    {
        // If panel is not null and onlyHologram is false just destroy panel and 3D model
        if (pestPanel != null && !onlyHologram)
        {
            // If panel is disabled by pressing the close button on panel instead of the main menu button to open and close it, then destroy and then instantiate it again
            if (!pestPanel.activeSelf)
            {
                Destroy(pestPanel);
                Destroy(pest);

                StartCoroutine(InstantiatePestsPanelAnd3DModelEnumerator(onlyHologram));
                return;
            }

            Destroy(pestPanel);
            Destroy(pest);
            return;
        }

        StartCoroutine(InstantiatePestsPanelAnd3DModelEnumerator(onlyHologram));
    }


    public void EnableDisableHolograms()
    {
        if (pest == null)
        {
            InstantiatePestsPanelAnd3DModel(true);
        }
        else
        {
            Destroy(pest);
        }
    }


    void DestroyPest3DModel()
    {
        if (pest != null)
        {
            Destroy(pest);
        }
    }

#endregion


    /////////
    ///

    public void InstantiatePanel(int i)
    {
        // If panel is not null and onlyHologram is false just destroy panel and 3D model
        if (pestPanel != null)
        {
            // If panel is disabled by pressing the close button on panel instead of the main menu button to open and close it, then destroy and then instantiate it again
            if (!pestPanel.activeSelf)
            {
                Destroy(pestPanel);

                if (pestPanel.GetComponentInChildren<SpriteDropHandler>() != null)
                {
                    for (int j = 0; j < pestPanel.GetComponentInChildren<SpriteDropHandler>().modelInstance.Length; j++)
                    {
                        if (pestPanel.GetComponentInChildren<SpriteDropHandler>().modelInstance[j] != null)
                            Destroy(pestPanel.GetComponentInChildren<SpriteDropHandler>().modelInstance[j]);
                    }
                }
                               

                StartCoroutine(InstantiatePanelEnumerator(i));
                return;
            }

            Destroy(pestPanel);


            if (pestPanel.GetComponentInChildren<SpriteDropHandler>() != null)
            {
                for (int j = 0; j < pestPanel.GetComponentInChildren<SpriteDropHandler>().modelInstance.Length; j++)
                {
                    if (pestPanel.GetComponentInChildren<SpriteDropHandler>().modelInstance[j] != null)
                        Destroy(pestPanel.GetComponentInChildren<SpriteDropHandler>().modelInstance[j]);
                }
            }                

            return;
        }

        StartCoroutine(InstantiatePanelEnumerator(i));
    }



    IEnumerator InstantiatePanelEnumerator(int i)
    {
        pestPanel = Instantiate(pestsInfoPanels[i], new Vector3(0, 0, 0), Quaternion.identity);
        //pestPanel.GetComponent<Follow>().enabled = true;

        yield return new WaitForEndOfFrame();

        //pestPanel.GetComponent<Follow>().enabled = false;

        // Add onClick event during runtime because I can't assign the event from inspector due to pestPanel being an instantiated prefab and the MenuManager is already in scene
        pestPanel.transform.Find("TitleBar/Buttons/ToggleHologramButton").GetComponent<Button>().onClick.AddListener(() => EnableDisableHolograms2());

        pestPanel.transform.Find("TitleBar/Buttons/Close Button").GetComponent<Button>().onClick.AddListener(() => DestroyPest3DModels());
    }


    void DestroyPest3DModels()
    {
        GameObject obj;

        for (int j = 0; j < pestPanel.GetComponentInChildren<SpriteDropHandler>().modelInstance.Length; j++)
        {
            obj = pestPanel.GetComponentInChildren<SpriteDropHandler>().modelInstance[j];

            if (obj != null)
                Destroy(obj);
        }
            

    }

    public void EnableDisableHolograms2()
    {
        GameObject obj;

        for (int j = 0; j < pestPanel.GetComponentInChildren<SpriteDropHandler>().modelInstance.Length; j++)
        {
            obj = pestPanel.GetComponentInChildren<SpriteDropHandler>().modelInstance[j];

            if (obj != null)
                obj.SetActive(!obj.activeSelf);

        }
    }
}
