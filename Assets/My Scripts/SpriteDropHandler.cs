using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

#if UNITY_ANDROID
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

        XRGrabInteractable objManipulator;

        for (int i = 0; i < transform.childCount; i++)
        {
            
            spriteInstance[i] = transform.GetChild(i).gameObject;

            // Subscribe to the OnManipulationEnded event
            if ( !spriteInstance[i].TryGetComponent<XRGrabInteractable>(out objManipulator) ) 
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

                        modelInstance[i].GetComponent<XRGrabInteractable>().lastSelectExited.AddListener(OnObjectDropped);

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

                        spriteInstance[i].GetComponent<XRGrabInteractable>().lastSelectExited.AddListener(OnObjectDropped);

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
#endif

#if !UNITY_ANDROID



public class SpriteDropHandler : MonoBehaviour
{
    [Header("Make sure model and sprite prefab index in the array match")]
    public GameObject[] modelPrefab;  // Assign the 3D model prefab
    public GameObject[] spritePrefab;

    [Header("Set parent panel transform")]
    public RectTransform panelTransform;  // Assign your panel's RectTransform to compare positions

    private GameObject[] spriteInstance;
    [HideInInspector] public GameObject[] modelInstance;

    public ParticleSystem smokeEffectPrefab;

    private Camera mainCamera;

    int spriteSiblingIndex;


    private void Start()
    {
        spriteInstance = new GameObject[transform.childCount];
        modelInstance = new GameObject[transform.childCount];
        mainCamera = Camera.main;

        for (int i = 0; i < transform.childCount; i++)
        {
            spriteInstance[i] = transform.GetChild(i).gameObject;
            spriteInstance[i].AddComponent<PrefabReference>().prefabSource = spritePrefab[i];

            // Add EventTrigger for drag-and-drop functionality
            AddDragHandlers(spriteInstance[i]);
        }
    }

    private void AddDragHandlers(GameObject obj)
    {
        var eventTrigger = obj.AddComponent<EventTrigger>();

        // Pointer Down
        var pointerDown = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerDown
        };
        pointerDown.callback.AddListener((data) => OnPointerDown(obj));
        eventTrigger.triggers.Add(pointerDown);

        // Drag
        var drag = new EventTrigger.Entry
        {
            eventID = EventTriggerType.Drag
        };
        drag.callback.AddListener((data) => OnDrag(obj, (PointerEventData)data));
        eventTrigger.triggers.Add(drag);

        // Pointer Up
        var pointerUp = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerUp
        };
        pointerUp.callback.AddListener((data) => OnPointerUp(obj));
        eventTrigger.triggers.Add(pointerUp);
    }

    private void OnPointerDown(GameObject obj)
    {
        // Logic for selecting the object (if needed)
    }

    private void OnDrag(GameObject obj, PointerEventData eventData)
    {
        Vector3 worldPosition;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(panelTransform, eventData.position, mainCamera, out worldPosition);
        obj.transform.position = worldPosition;
    }

    private void OnPointerUp(GameObject obj)
    {
        if (!obj.TryGetComponent<PrefabReference>(out PrefabReference reference))
            return;

        GameObject prefabSource = reference.prefabSource;
        bool isSprite = obj.TryGetComponent<SpriteRenderer>(out _);

        if (isSprite)
        {
            for (int i = 0; i < spritePrefab.Length; i++)
            {
                if (spritePrefab[i] == prefabSource)
                {
                    if (!IsInsidePanel(obj.transform.position))
                    {
                        Instantiate(smokeEffectPrefab, obj.transform.position + new Vector3(0, -0.02f, 0), Quaternion.Euler(-90, 0, 0));
                        modelInstance[i] = Instantiate(modelPrefab[i], obj.transform.position, Quaternion.identity);
                        modelInstance[i].AddComponent<PrefabReference>().prefabSource = modelPrefab[i];
                        Destroy(obj);
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
                    if (IsInsidePanel(obj.transform.position))
                    {
                        spriteInstance[i] = Instantiate(spritePrefab[i], obj.transform.position, Quaternion.identity);
                        spriteInstance[i].AddComponent<PrefabReference>().prefabSource = spritePrefab[i];
                        Destroy(obj);
                    }
                }
            }
        }
    }

    private bool IsInsidePanel(Vector3 position)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(panelTransform, mainCamera.WorldToScreenPoint(position), mainCamera, out localPoint);
        return panelTransform.rect.Contains(localPoint);
    }
}



#endif