using MixedReality.Toolkit.SpatialManipulation;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


public class SpriteDropHandler : MonoBehaviour
{
    [Header("Make sure model and sprite prefab index in the array match")]
    public GameObject [] modelPrefab;  // Assign the 3D model prefab
    public GameObject [] spritePrefab;

    [Header("Set parent panel transform")]
    public Transform panelTransform;  // Assign your panel's transform to compare positions

    private GameObject [] spriteInstance;
    [HideInInspector] public GameObject [] modelInstance;

    int spriteSiblingIndex;

    public ParticleSystem smokeEffectPrefab;


    private void Start()
    {
        spriteInstance = new GameObject[transform.childCount];
        modelInstance = new GameObject[transform.childCount];

        ObjectManipulator objManipulator;

        for (int i = 0; i < transform.childCount; i++)
        {
            
            spriteInstance[i] = transform.GetChild(i).gameObject;

            // Subscribe to the OnManipulationEnded event
            if ( !spriteInstance[i].TryGetComponent<ObjectManipulator>(out objManipulator) ) 
                return;

            objManipulator.lastSelectExited.AddListener(OnObjectDropped);

            spriteInstance[i].AddComponent<PrefabReference>().prefabSource = spritePrefab[i];
        }
        
    }

    private void OnObjectDropped(SelectExitEventArgs eventData)
    {
        if ( !eventData.interactableObject.transform.gameObject.TryGetComponent<PrefabReference>(out PrefabReference reference) )
            return;

        GameObject prefabSource = reference.prefabSource;

        bool isSprite = eventData.interactableObject.transform.TryGetComponent<SpriteRenderer>(out _);

        if (isSprite)
        {
            for (int i = 0; i < spritePrefab.Length; i++)
            {
                if (spritePrefab[i] == prefabSource)
                {
                    // Check if the object is outside the panel
                    if (!IsInsidePanel(eventData.interactableObject.transform.position))
                    {
                        Instantiate(smokeEffectPrefab, eventData.interactableObject.transform.position + new Vector3(0, -0.02f, 0), Quaternion.Euler(-90, 0, 0));

                        // Instantiate the 3D model at the release position
                        modelInstance[i] = Instantiate(modelPrefab[i], eventData.interactableObject.transform.position, Quaternion.identity);
                        modelInstance[i].AddComponent<PrefabReference>().prefabSource = modelPrefab[i];

                        modelInstance[i].GetComponent<ObjectManipulator>().lastSelectExited.AddListener(OnObjectDropped);

                        Destroy(eventData.interactableObject.transform.gameObject);
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < modelPrefab.Length; i++)                 
            {
                if (modelPrefab[i] == prefabSource)
                {
                    // Check if the object is inside the panel
                    if (IsInsidePanel(eventData.interactableObject.transform.position))
                    {
                        

                        // Instantiate the sprite at the release position
                        spriteInstance[i] = Instantiate(spritePrefab[i], eventData.interactableObject.transform.position, Quaternion.identity);
                        spriteInstance[i].AddComponent<PrefabReference>().prefabSource = spritePrefab[i];

                        var smoke = Instantiate(smokeEffectPrefab, eventData.interactableObject.transform.position + new Vector3(-0.1f, -0.02f, -0.1f), Quaternion.Euler(-90, 0, 0));
                        Destroy(smoke, 1);


                        spriteInstance[i].transform.SetParent(transform);
                        spriteInstance[i].transform.SetSiblingIndex(i);

                        spriteInstance[i].transform.localScale = spritePrefab[i].transform.localScale;
                        spriteInstance[i].transform.localPosition = spritePrefab[i].transform.localPosition;
                        spriteInstance[i].transform.localRotation = spritePrefab[i].transform.localRotation;

                        spriteInstance[i].GetComponent<ObjectManipulator>().lastSelectExited.AddListener(OnObjectDropped);

                        Destroy(eventData.interactableObject.transform.gameObject);
                    }
                }
            }

            
        }
        
    }

    private bool IsInsidePanel(Vector3 position)
    {
        // Check if the position is inside the panel's bounds
        return panelTransform.GetComponent<Collider>().bounds.Contains(position);
    }
}