using UnityEngine;
using UnityEngine.UI;

public class VuzixCamera : MonoBehaviour
{
    private WebCamTexture camTexture;
    public RawImage cameraDisplay; // Assign in Inspector

    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length > 0)
        {
            camTexture = new WebCamTexture(devices[0].name);
            cameraDisplay.texture = camTexture;
            camTexture.Play();
        }
        else
        {
            Debug.LogError("No camera detected.");
        }
    }

    void OnDestroy()
    {
        if (camTexture != null)
        {
            camTexture.Stop();
        }
    }
}
