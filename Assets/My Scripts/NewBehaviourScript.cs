using UnityEngine;

public class FindObjectsWithTag : MonoBehaviour
{
    public string targetTag = "panelsThatDisplayFieldMesh"; // Replace "MyTag" with the tag you want to search for

    void Start()
    {
        // Find all game objects with the specified tag
        GameObject[] objectsWithTag1 = GameObject.FindGameObjectsWithTag(targetTag);
        AudioListener[] objectsWithTag = GameObject.FindObjectsOfType<AudioListener>();


        // Check if any objects were found
        if (objectsWithTag.Length > 0)
        {
            Debug.Log($"Found {objectsWithTag.Length} GameObject(s) with the tag '{targetTag}':");
            foreach (AudioListener obj in objectsWithTag)
            {
                Debug.Log($" Audio Test- {obj.gameObject.name}", obj); // Logs the name of each GameObject
            }
        }
        else
        {
            Debug.Log($"No GameObjects found with the tag '{targetTag}'.");
        }
    }
}
