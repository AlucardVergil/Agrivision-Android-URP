using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeleteFilePathAndMessage : MonoBehaviour
{
    public GameObject rainbowGameobject;    
    

    private void OnDisable()
    {
        rainbowGameobject.GetComponent<FileManager>().selectedFilePath = "";
        rainbowGameobject.GetComponent<FileManager>().uploadFilePath.text = "";
        rainbowGameobject.GetComponent<FileManager>().uploadFilePathBubble.text = "";
    }
}
