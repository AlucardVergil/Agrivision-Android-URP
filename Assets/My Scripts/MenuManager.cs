using MixedReality.Toolkit.SpatialManipulation;
using MixedReality.Toolkit.UX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject[] slate;

    [SerializeField] private GameObject[] pestsPrefabs;
    [SerializeField] private GameObject[] pestsInfoPanels;


    private GameObject pest;
    private GameObject pestPanel;

    private GameObject activePanel;


    private void Start()
    {
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
        GameObject.FindGameObjectWithTag("FollowButton")?.GetComponent<PressableButton>().ForceSetToggled(false);

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

        if (slate[i].activeSelf)
        {
            slate[i].GetComponent<Follow>().enabled = true;

            yield return new WaitForEndOfFrame();

            slate[i].GetComponent<Follow>().enabled = false;
        }

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
        pest.GetComponent<Follow>().enabled = true;

        if (!onlyHologram)
        {
            pestPanel = Instantiate(pestsInfoPanels[0], new Vector3(0, 0, 0), Quaternion.identity);
            pestPanel.GetComponent<Follow>().enabled = true;
        }

        yield return new WaitForEndOfFrame();

        pest.GetComponent<Follow>().enabled = false;

        if (!onlyHologram)
        {
            pestPanel.GetComponent<Follow>().enabled = false;

            // Add onClick event during runtime because I can't assign the event from inspector due to pestPanel being an instantiated prefab and the MenuManager is already in scene
            pestPanel.transform.Find("TitleBar/Buttons/ToggleHologramButton").GetComponent<PressableButton>().OnClicked.AddListener(() => EnableDisableHolograms());

            pestPanel.transform.Find("TitleBar/Buttons/CloseButton").GetComponent<PressableButton>().OnClicked.AddListener(() => DestroyPest3DModel());
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
        pestPanel.GetComponent<Follow>().enabled = true;

        yield return new WaitForEndOfFrame();

        pestPanel.GetComponent<Follow>().enabled = false;

        // Add onClick event during runtime because I can't assign the event from inspector due to pestPanel being an instantiated prefab and the MenuManager is already in scene
        pestPanel.transform.Find("TitleBar/Buttons/ToggleHologramButton").GetComponent<PressableButton>().OnClicked.AddListener(() => EnableDisableHolograms2());

        pestPanel.transform.Find("TitleBar/Buttons/CloseButton").GetComponent<PressableButton>().OnClicked.AddListener(() => DestroyPest3DModels());
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
