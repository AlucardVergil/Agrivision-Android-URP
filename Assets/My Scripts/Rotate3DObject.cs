using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate3DObject : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 10f; // Speed of rotation (degrees per second)


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
