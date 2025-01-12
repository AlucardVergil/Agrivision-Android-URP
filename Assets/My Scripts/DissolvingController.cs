using MixedReality.Toolkit.UX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class DissolvingController : MonoBehaviour
{
    public MeshRenderer[] meshRendererArray; // Replaced SkinnedMeshRenderer with MeshRenderer
    public VisualEffect VFXGraph;
    public float dissolveRate = 0.02f;
    public float refreshRate = 0.05f;

    private List<Material> dissolveMaterialsList;

    private GameObject toggleHologramButton;

    // Start is called before the first frame update
    void Start()
    {        
        // Stop particles effects 
        if (VFXGraph != null)
        {
            VFXGraph.Stop(); // Stop particle effects
            VFXGraph.gameObject.SetActive(false);
        }

        dissolveMaterialsList = new List<Material>();

        // Save all materials inside every meshRenderer in the array to the list
        for (int i = 0; i < meshRendererArray.Length; i++)
        {
            foreach (Material mat in meshRendererArray[i].materials)
                dissolveMaterialsList.Add(mat);
        }
    }

    // Function to dissolve the object using a coroutine
    public IEnumerator Dissolve()
    {
        toggleHologramButton = GameObject.Find("ToggleHologramsButton");
        toggleHologramButton.GetComponent<PressableButton>().enabled = false;

        yield return new WaitForSeconds(0.2f); // Suspend execution for 0.2 secs

        // Activate particles and play them
        if (VFXGraph != null)
        {
            VFXGraph.gameObject.SetActive(true);
            VFXGraph.Play();
        }

        float counter = 0;

        if (dissolveMaterialsList.Count > 0)
        {
            // Check the first material's dissolve value
            while (dissolveMaterialsList[0].GetFloat("_DissolveAmount") < 1) // Assuming "_DissolveAmount" is a shader property
            {
                counter += dissolveRate; // Increase the dissolve amount based on the rate

                for (int i = 0; i < dissolveMaterialsList.Count; i++)
                    dissolveMaterialsList[i].SetFloat("_DissolveAmount", counter);

                yield return new WaitForSeconds(refreshRate); // Suspend execution for smooth dissolve
            }
        }

        yield return new WaitForSeconds(0.4f);

        toggleHologramButton.GetComponent<PressableButton>().enabled = true;

        yield return new WaitForEndOfFrame();

        Destroy(gameObject); // Destroy the object after 1 second
    }

    public void CallDissolve()
    {
        StartCoroutine(Dissolve());
    }
}
