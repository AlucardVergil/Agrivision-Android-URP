using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static UnityEngine.UI.GridLayoutGroup;
using MixedReality.Toolkit.SpatialManipulation;
using UnityEngine.Timeline;
using MixedReality.Toolkit;
using System.Collections;
using static UnityEngine.Rendering.DebugUI;
using System.Linq;

public class ARFieldVisualizer : MonoBehaviour
{
    // NOTE the order of the corner coords needs to be clockwise or else the mesh is upside down
    [SerializeField] private Vector2[] fieldCorners; // Store GPS coordinates of the 4 corners
    
    #region Corner Markers Not Sure If Will Be Used
    [SerializeField] private GameObject fieldMarkerPrefab; // Prefab to represent corners and borders
    [SerializeField] private Material insideFieldMaterial; // Material to color inside field area

    private List<GameObject> fieldMarkers = new List<GameObject>();
    #endregion

    [SerializeField] private Material fieldMaterial; // Material for the field area (semi-transparent)
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    private const float EarthRadius = 6371000f; // Earth's radius in meters (for converting gps area to actual square meters)

    public TMP_Text debugText; // Reference to the TextMeshPro UI component
    private string lastReceivedMessage = "";


    [SerializeField] private GameObject hologramPrefab; // Reference to your hologram prefab
    [SerializeField] private float heightAboveField = 2.0f; // How high the hologram should appear above the field

    private GameObject hologramInstance;

    private GameObject field;

    private GameObject[] panelsThatDisplayFieldMesh;

    bool doOnceBool = true;
    bool allFieldMeshPanelsDisabled;

    GameObject apisManager;
    FertilizationData fertilizationData;


    void Awake()
    {
        panelsThatDisplayFieldMesh = GameObject.FindGameObjectsWithTag("panelsThatDisplayFieldMesh");
    }



    void Start()
    {
        apisManager = GameObject.FindGameObjectWithTag("APIsManager");

        if (fieldCorners.Length != 4)
        {
            debugText.text = "You must assign exactly 4 corners.";
            return;
        }

        // Calculate and log the area of the field
        float area = CalculateArea(fieldCorners);
        lastReceivedMessage = $"Field Area: {area} square meters";
               

        //Place semi-transparent mesh in field area
        //RecalibrateFieldMesh();

        InstantiateHologramAboveField();
        // Place markers at the corners
        //PlaceFieldMarkers();

        // Update the field coloring based on the current position
        InvokeRepeating("UpdateFieldVisualization", 1.0f, 1.0f);
    }



    // Shoelace formula to calculate the area of a quadrilateral
    public float CalculateArea(Vector2[] corners)
    {
        float area = 0;
        int n = corners.Length;

        for (int i = 0; i < n; i++)
        {
            // Convert latitude and longitude from degrees to radians
            Vector2 current = new Vector2(Mathf.Deg2Rad * corners[i].x, Mathf.Deg2Rad * corners[i].y);
            Vector2 next = new Vector2(Mathf.Deg2Rad * corners[(i + 1) % n].x, Mathf.Deg2Rad * corners[(i + 1) % n].y);

            // Use the spherical excess formula to approximate the area on the Earth's surface
            area += (next.x - current.x) * (2 + Mathf.Sin(current.y) + Mathf.Sin(next.y));
        }

        area = Mathf.Abs(area) / 2f;

        // Convert the area in radians to square meters
        area *= EarthRadius * EarthRadius;

        return area;
    }

    // Check if a point is inside the quadrilateral using the ray-casting method
    public bool IsPointInside(Vector2 point)
    {
        int crossings = 0;
        int n = fieldCorners.Length;

        for (int i = 0; i < n; i++)
        {
            Vector2 v1 = fieldCorners[i];
            Vector2 v2 = fieldCorners[(i + 1) % n];

            // Check if the point is within the y-range of the edge
            if (((v1.y > point.y) != (v2.y > point.y)) &&
                (point.x < (v2.x - v1.x) * (point.y - v1.y) / (v2.y - v1.y) + v1.x))
            {
                crossings++;
            }
        }

        // The point is inside if the number of crossings is odd
        return (crossings % 2 != 0);
    }

    // Visualize the field and check if the current GPS position is inside
    void UpdateFieldVisualization()
    {
        float latitude = GetComponent<UDPListener>().latitude;
        float longitude = GetComponent<UDPListener>().longitude;

#if UNITY_EDITOR
        Vector2 currentPosition = new Vector2(40.62573397234498f, 22.959545477275366f);
#else
        Vector2 currentPosition = new Vector2(latitude, longitude);
#endif


        // Check if the current position is inside the field
        if (IsPointInside(currentPosition))
        {
            debugText.text = lastReceivedMessage + "\nYou are inside the field.";

            allFieldMeshPanelsDisabled = true;

            foreach (var panel in panelsThatDisplayFieldMesh)
            {
                if (panel.activeSelf)
                {                    
                    allFieldMeshPanelsDisabled = false;

                    if (doOnceBool)
                    {
                        doOnceBool = false;

                        RecalibrateFieldMesh();
                        ColorFieldArea(true);
                    }                    
                }
            }

            if (allFieldMeshPanelsDisabled)
            {
                doOnceBool = true;

                if (field != null)
                    DestroyImmediate(field);
            }
        }
        else
        {
            debugText.text = lastReceivedMessage + "\nYou are outside the field.";
            ColorFieldArea(false);

            doOnceBool = true;

            if (field != null)
                DestroyImmediate(field);
        }
    }


    // Create the mesh for the field area
    public void CreateFieldMeshBackup()
    {
        if (field != null)
            DestroyImmediate(field);

        field = new GameObject("FieldMesh", typeof(MeshFilter), typeof(MeshRenderer)); //Note: box collider needs to be added before ObjectManipulator
        meshFilter = field.GetComponent<MeshFilter>();
        meshRenderer = field.GetComponent<MeshRenderer>();
        
        meshRenderer.material = fieldMaterial;

        Mesh mesh = new Mesh();
        mesh.name = "Mesh";

        Vector3[] vertices = new Vector3[fieldCorners.Length];
        for (int i = 0; i < fieldCorners.Length; i++)
        {
            vertices[i] = GPSPositionToWorldPosition(fieldCorners[i]);
            PlaceFieldMarkers(vertices[i]);
        }

        // Set up how the mesh's triangles connect (counter-clockwise)
        int[] triangles = new int[]
        {
            0, 1, 2, // First triangle
            0, 2, 3  // Second triangle
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;

        var meshCollider = field.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;


        var manipulator = field.AddComponent<ObjectManipulator>();
        // Can grab with both hands to rotate and scale field mesh
        manipulator.selectMode = UnityEngine.XR.Interaction.Toolkit.Interactables.InteractableSelectMode.Multiple; 

        RotationAxisConstraint rotationConstraint = field.AddComponent<RotationAxisConstraint>();

        // Disable manipulation on all axes, except y
        rotationConstraint.ConstraintOnRotation = AxisFlags.XAxis | AxisFlags.ZAxis;

        MoveAxisConstraint moveConstraint = field.AddComponent<MoveAxisConstraint>();

        // Disable manipulation on y axis
        moveConstraint.ConstraintOnMovement = AxisFlags.YAxis;

        // Make movement, rotation and scaling of field mesh smoother / slower
        manipulator.MoveLerpTime = 0.3f;
        manipulator.RotateLerpTime = 0.3f;
        manipulator.ScaleLerpTime = 0.3f;

        // Allow manipulation for movement and rotation only
        manipulator.AllowedManipulations = TransformFlags.Move | TransformFlags.Rotate;

        manipulator.enabled = false;
    }


    // Create the mesh for the field area
    public void CreateFieldMesh(Texture texture)
    {
        if (field != null)
            DestroyImmediate(field);

        field = new GameObject("FieldMesh", typeof(MeshFilter), typeof(MeshRenderer)); //Note: box collider needs to be added before ObjectManipulator
        meshFilter = field.GetComponent<MeshFilter>();
        meshRenderer = field.GetComponent<MeshRenderer>();

        // Create a material with texture
        Material material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        //Texture texture = Resources.Load<Texture>("Sprites/test");

        if (texture != null)
        {
            material.SetTexture("_BaseMap", texture); // Assign texture to the Base Map
            meshRenderer.material = material;
        }
        else
        {
            Debug.LogError("Texture not found. Check the Resources folder and texture path.");
        }

        Mesh mesh = new Mesh();
        mesh.name = "Mesh";

        Vector3[] vertices = new Vector3[fieldCorners.Length];
        for (int i = 0; i < fieldCorners.Length; i++)
        {
            vertices[i] = GPSPositionToWorldPosition(fieldCorners[i]);
            PlaceFieldMarkers(vertices[i]);
        }

        // Define UVs for texturing
        Vector2[] uv = new Vector2[vertices.Length];
        uv[0] = new Vector2(0, 0); // Bottom-left
        uv[1] = new Vector2(1, 0); // Bottom-right
        uv[2] = new Vector2(1, 1); // Top-right
        uv[3] = new Vector2(0, 1); // Top-left

        // Set up how the mesh's triangles connect (counter-clockwise)
        int[] triangles = new int[]
        {
            0, 1, 2, // First triangle
            0, 2, 3  // Second triangle
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv; // Assign UVs for texture mapping
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;

        var meshCollider = field.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;


        var manipulator = field.AddComponent<ObjectManipulator>();
        // Can grab with both hands to rotate and scale field mesh
        manipulator.selectMode = UnityEngine.XR.Interaction.Toolkit.Interactables.InteractableSelectMode.Multiple;

        RotationAxisConstraint rotationConstraint = field.AddComponent<RotationAxisConstraint>();

        // Disable manipulation on all axes, except y
        rotationConstraint.ConstraintOnRotation = AxisFlags.XAxis | AxisFlags.ZAxis;

        MoveAxisConstraint moveConstraint = field.AddComponent<MoveAxisConstraint>();

        // Disable manipulation on y axis
        moveConstraint.ConstraintOnMovement = AxisFlags.YAxis;

        // Make movement, rotation and scaling of field mesh smoother / slower
        manipulator.MoveLerpTime = 0.3f;
        manipulator.RotateLerpTime = 0.3f;
        manipulator.ScaleLerpTime = 0.3f;

        // Allow manipulation for movement and rotation only
        manipulator.AllowedManipulations = TransformFlags.Move | TransformFlags.Rotate;

        manipulator.enabled = false;
    }



    // This method will convert GPS coordinates to Unity world coordinates based on a reference point.
    private Vector3 GPSPositionToWorldPosition(Vector2 gpsPosition)
    {
        // Reference point (the "origin" GPS point to convert everything relative to)
#if UNITY_EDITOR
        Vector2 userReferenceGPS = new Vector2(40.62573397234498f, 22.959545477275366f);
#else
        Vector2 userReferenceGPS = new Vector2(GetComponent<UDPListener>().latitude, GetComponent<UDPListener>().longitude);
#endif

        // The scale factor to convert GPS degrees into meters (approximately, varies with location)
        float metersPerLat = 111320f; // Approximate meters per degree of latitude
        float metersPerLon = 111320f * Mathf.Cos(userReferenceGPS.x * Mathf.Deg2Rad); // Adjust for longitude based on latitude

        // Calculate offsets from the reference GPS point
        float deltaLat = gpsPosition.x - userReferenceGPS.x;
        float deltaLon = gpsPosition.y - userReferenceGPS.y;

        // Convert GPS offset into meters
        float xOffset = deltaLon * metersPerLon;
        float zOffset = deltaLat * metersPerLat;

        // Return the calculated Unity world position (on a flat plane)
        return new Vector3(xOffset, 0, zOffset);
    }



    // Color the field based on whether the user is inside or outside
    void ColorFieldArea(bool inside)
    {
        // Here I can change the material properties if needed, for example change color
        //if (meshRenderer != null)
        //    meshRenderer.material.color = inside ? new Color(0, 1, 0, 0.5f) : new Color(1, 0, 0, 0.5f); // Green if inside, Red if outside
    }


    public void RecalibrateFieldMesh()
    {
        if (fieldMarkers != null)
        {
            foreach (var marker in fieldMarkers)
                DestroyImmediate(marker);
        }        

        if (field != null)
            DestroyImmediate(field);

        apisManager.GetComponent<FertilizationAPI>().GetFertilizationData("5836", (jsonResponseFertilization) =>
        {
            Debug.Log("fertilizationData => " + jsonResponseFertilization);

            Texture texture = apisManager.GetComponent<FertilizationAPI>().ParseFertilizationData(jsonResponseFertilization);

            CreateFieldMesh(texture);
        });


        apisManager.GetComponent<CropGrowthImageAPI>().GetCropGrowthImage("157212", (jsonResponseCropGrowthImage) =>
        {
            if (jsonResponseCropGrowthImage != null)
            {
                Debug.Log("CROP IMAGE EXISTS");

                CreateFieldMesh(jsonResponseCropGrowthImage);
            }                
            else
                Debug.Log("CROP IMAGE DOES NOT EXIST");

            
        });


        apisManager.GetComponent<TillageAPI>().GetTillageData("10478", (jsonResponseTillage) =>
        {
            Debug.Log(jsonResponseTillage);


        });


        apisManager.GetComponent<IrrigationAPI>().GetIrrigationData("11256", (jsonResponseIrrigation) =>
        {
            Debug.Log(jsonResponseIrrigation);


        });


        apisManager.GetComponent<CropGrowthDatesAPI>().GetCropGrowthDatesData("157212", (jsonResponseCropGrowthDates) =>
        {
            Debug.Log("CropGrowthDatesAPI => " + jsonResponseCropGrowthDates);


        });



        // Recalibrate the position of the hologram as well to be in the center of the recalibrated field mesh
        if (hologramInstance != null)
        {
            // Calculate the center of the field
            Vector3 centerOfField = CalculateFieldCenter(fieldCorners);

            // Position the hologram above the center of the field
            Vector3 hologramPosition = centerOfField + new Vector3(0, heightAboveField, 0);

            hologramInstance.transform.position = hologramPosition;
        }
    }

        


    #region Corner Markers Not Sure If Will Be Used
    // Place visual markers at the corners of the field
    void PlaceFieldMarkers(Vector3 corner)
    {
        GameObject marker = Instantiate(fieldMarkerPrefab, corner, Quaternion.identity);
        
        marker.AddComponent<BoxCollider>();
        marker.AddComponent<ObjectManipulator>();

        fieldMarkers.Add(marker);

    }


    // Color the corner markers based on whether the user is inside or outside
    void ColorCornerMarkers(bool inside)
    {
        foreach (var marker in fieldMarkers)
        {
            marker.GetComponent<Renderer>().material = inside ? insideFieldMaterial : null;
        }
    }
    #endregion


    public void EnableManualFieldMeshAdjustment()
    {
        var fieldManipulator = field.GetComponent<ObjectManipulator>();
        fieldManipulator.enabled = !fieldManipulator.isActiveAndEnabled;
    }



    // Calculate the center of the field mesh, in order to add a hologram or 3D object hovering above it
    Vector3 CalculateFieldCenter(Vector2[] corners)
    {
        Vector3 center = Vector3.zero;

        // Sum all the corner positions
        for (int i = 0; i < corners.Length; i++)
        {
            center += GPSPositionToWorldPosition(corners[i]);
        }

        // Average the positions to get the center
        center /= corners.Length;

        return center;
    }



    void InstantiateHologramAboveField()
    {
        if (fieldCorners == null || fieldCorners.Length == 0)
        {
            Debug.LogError("Field corners are not set.");
            return;
        }

        // Calculate the center of the field
        Vector3 centerOfField = CalculateFieldCenter(fieldCorners);

        // Position the hologram above the center of the field
        Vector3 hologramPosition = centerOfField + new Vector3(0, heightAboveField, 0);

        // Instantiate the hologram prefab at the desired position
        hologramInstance = Instantiate(hologramPrefab, hologramPosition, Quaternion.identity);

        // Optionally, add some rotation or effects to the hologram
        //hologramInstance.transform.LookAt(Camera.main.transform); // Face the user's view
    }


    public void EnableDisableHolograms()
    {
        var wateringCan = GameObject.Find("WateringCanController(Clone)");
        if (wateringCan == null)
        {
            InstantiateHologramAboveField();
        }
        else
        {
            var dissolveController = wateringCan.GetComponent<DissolvingController>();
            dissolveController.CallDissolve();
        }        
    }


    #region Order Field Gps Coordinates Clockwise

    Vector2 GetCentroid(Vector2 [] corners)
    {
        float sumX = 0;
        float sumY = 0;

        foreach (var point in corners)
        {
            sumX += point.x; // Longitude
            sumY += point.y; // Latitude
        }
        return new Vector2(sumX / corners.Length, sumY / corners.Length);
    }


    Vector2 [] OrderFieldCornersClockwise(Vector2[] corners)
    {
        Vector2 centroid = GetCentroid(corners);
        return corners.OrderBy(p => Mathf.Atan2(p.y - centroid.y, p.x - centroid.x)).ToArray();
    }

    #endregion


    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    Vector2[] gpsCoords = new Vector2[] {
        //    new Vector2(40.66974042949055f, 22.895043858444026f), // Example coordinates
        //    new Vector2(40.64010325965247f, 22.92629424082333f),
        //    new Vector2(40.651522041036685f, 22.88997358540623f),
        //    new Vector2(40.68170335926916f, 22.964762460424758f)
        //    };



        //    var orderedCoords = OrderFieldCornersClockwise(gpsCoords);

        //    foreach (var point in orderedCoords)
        //    {
        //        Debug.Log(string.Format("Ordered GPS Coord: ({0:F8}, {1:F8})", point.x, point.y));
        //    }
        //}
    }
}