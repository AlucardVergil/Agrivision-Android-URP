using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class MeshToVFX : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public VisualEffect VFXGraph;
    public float refreshRate = 0.02f;

    private MeshFilter meshFilter;

    // Start is called before the first frame update
    void Start()
    {

        meshFilter = meshRenderer.GetComponent<MeshFilter>(); // Get the MeshFilter component
        StartCoroutine(UpdateVFXGraph()); // Call IEnumerator function using StartCoroutine

        /*
        for (int i = 0; i < meshRenderer.Length; i++)
        {
            meshFilter = meshRenderer[i].GetComponent<MeshFilter>(); // Get the MeshFilter component
            StartCoroutine(UpdateVFXGraph()); // Call IEnumerator function using StartCoroutine
        }
        */
    }

    IEnumerator UpdateVFXGraph()
    {
        // While the game object is active
        while (gameObject.activeSelf)
        {
            Mesh mesh = meshFilter.mesh; // Directly reference the mesh

            // Get vertices of the mesh to set as "mesh" positions for particles
            Vector3[] vertices = mesh.vertices;
            Mesh mesh2 = new Mesh();
            mesh2.vertices = vertices;

            VFXGraph.SetMesh("Mesh", mesh2); // Set the mesh for particles

            yield return new WaitForSeconds(refreshRate); // Suspend execution
        }
    }
}
