using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static UnityEngine.UI.GridLayoutGroup;
using UnityEngine.Timeline;

using System.Collections;
using static UnityEngine.Rendering.DebugUI;
using System.Linq;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ARFieldVisualizer : MonoBehaviour
{
    // NOTE the order of the corner coords needs to be clockwise or else the mesh is upside down
    //[SerializeField] private Vector2[] fieldCorners; // Store GPS coordinates of the 4 corners

    //private Vector2[] fieldCorners = {
    //    new Vector2((float)40.62588, (float)22.95959),
    //    new Vector2((float)40.62573, (float)22.95973),
    //    new Vector2((float)40.62563, (float)22.95949),
    //    new Vector2((float)40.62579, (float)22.95937)
    //};

    private Vector2[] fieldCorners;
    private Vector2[] fieldCorners2 = {
        new Vector2(21.692269f, 39.636541f),
        new Vector2(21.693132f, 39.635601f),
        new Vector2(21.693572f, 39.635877f),
        new Vector2(21.692695f, 39.636791f)
    };


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

    public Parcel[] parcels;


    void Awake()
    {
        panelsThatDisplayFieldMesh = GameObject.FindGameObjectsWithTag("panelsThatDisplayFieldMesh");
    }



    void Start()
    {
        fieldCorners = OrderFieldCornersClockwise(fieldCorners2);

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



    // Returns the current selected parcel field coordinates and puts them in clockwise order
    public void GetSelectedParcelCoordinates()
    {
        foreach (var parcel in parcels)
        {
            if (apisManager.GetComponent<ParcelsListAPI>().selectedParcelId != null && parcel.id.ToString() == apisManager.GetComponent<ParcelsListAPI>().selectedParcelId)
            {
                Vector2[] tempFieldCorners = new Vector2[4];

                for (int i = 0; i < parcel.shape.coordinates[0].Length; i++)
                {
                    tempFieldCorners[i].x = parcel.shape.coordinates[0][i][0];
                    tempFieldCorners[i].y = parcel.shape.coordinates[0][i][1];
                }

                fieldCorners = OrderFieldCornersClockwise(tempFieldCorners);
            }            
        }

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
        //Vector2 currentPosition = new Vector2(40.62573397234498f, 22.959545477275366f);
        //Vector2 currentPosition = new Vector2(21.69290f, 39.63610f); // NOTE: also see Vector2 userReferenceGPS in this script
        Vector2 currentPosition = new Vector2(21.69290f, 39.63610f); // NOTE: also see Vector2 userReferenceGPS in this script
        foreach (var parcel in parcels)
        {
            if (apisManager.GetComponent<ParcelsListAPI>().selectedParcelId != null && parcel.id.ToString() == apisManager.GetComponent<ParcelsListAPI>().selectedParcelId)
            {
                currentPosition.x = parcel.shape.coordinates[0][0][0]; // NOTE: also see Vector2 userReferenceGPS in this script
                currentPosition.y = parcel.shape.coordinates[0][0][1];
            }
        }
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

                //if (field != null)
                //    DestroyImmediate(field);
            }
        }
        else
        {
            debugText.text = lastReceivedMessage + "\nYou are outside the field.";
            ColorFieldArea(false);

            doOnceBool = true;

            //if (field != null)
            //    DestroyImmediate(field);
        }
    }


    // Create the mesh for the field area
    //public void CreateFieldMeshBackup()
    //{
    //    if (field != null)
    //        DestroyImmediate(field);

    //    field = new GameObject("FieldMesh", typeof(MeshFilter), typeof(MeshRenderer)); //Note: box collider needs to be added before ObjectManipulator
    //    meshFilter = field.GetComponent<MeshFilter>();
    //    meshRenderer = field.GetComponent<MeshRenderer>();
        
    //    meshRenderer.material = fieldMaterial;

    //    Mesh mesh = new Mesh();
    //    mesh.name = "Mesh";

    //    Vector3[] vertices = new Vector3[fieldCorners.Length];
    //    for (int i = 0; i < fieldCorners.Length; i++)
    //    {
    //        vertices[i] = GPSPositionToWorldPosition(fieldCorners[i]);
    //        PlaceFieldMarkers(vertices[i]);
    //    }

    //    // Set up how the mesh's triangles connect (counter-clockwise)
    //    int[] triangles = new int[]
    //    {
    //        0, 1, 2, // First triangle
    //        0, 2, 3  // Second triangle
    //    };

    //    mesh.vertices = vertices;
    //    mesh.triangles = triangles;
    //    mesh.RecalculateNormals();

    //    meshFilter.mesh = mesh;

    //    var meshCollider = field.AddComponent<MeshCollider>();
    //    meshCollider.sharedMesh = mesh;


    //    var manipulator = field.AddComponent<ObjectManipulator>();
    //    // Can grab with both hands to rotate and scale field mesh
    //    manipulator.selectMode = UnityEngine.XR.Interaction.Toolkit.Interactables.InteractableSelectMode.Multiple; 

    //    RotationAxisConstraint rotationConstraint = field.AddComponent<RotationAxisConstraint>();

    //    // Disable manipulation on all axes, except y
    //    rotationConstraint.ConstraintOnRotation = AxisFlags.XAxis | AxisFlags.ZAxis;

    //    MoveAxisConstraint moveConstraint = field.AddComponent<MoveAxisConstraint>();

    //    // Disable manipulation on y axis
    //    moveConstraint.ConstraintOnMovement = AxisFlags.YAxis;

    //    // Make movement, rotation and scaling of field mesh smoother / slower
    //    manipulator.MoveLerpTime = 0.3f;
    //    manipulator.RotateLerpTime = 0.3f;
    //    manipulator.ScaleLerpTime = 0.3f;

    //    // Allow manipulation for movement and rotation only
    //    manipulator.AllowedManipulations = TransformFlags.Move | TransformFlags.Rotate;

    //    manipulator.enabled = false;
    //}


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
        //uv[0] = new Vector2(0, 0); // Bottom-left
        //uv[1] = new Vector2(1, 0); // Bottom-right
        //uv[2] = new Vector2(1, 1); // Top-right
        //uv[3] = new Vector2(0, 1); // Top-left
        //uv[0] = new Vector2(0.5f, 0); // Midpoint on the bottom edge
        //uv[1] = new Vector2(1f, 0.5f); // Midpoint on the right edge
        //uv[2] = new Vector2(0.5f, 1); // Midpoint on the top edge
        //uv[3] = new Vector2(0, 0.5f); // Midpoint on the left edge
        //uv[0] = new Vector2(0.1f, 0); // Left edge of the field
        //uv[1] = new Vector2(0.9f, 0); // Right edge of the field
        //uv[2] = new Vector2(0.9f, 1); // Top-right
        //uv[3] = new Vector2(0.1f, 1); // Top-left
        //uv[0] = new Vector2(0.1f, 0.1f); // Adjusted Bottom-left
        //uv[1] = new Vector2(0.9f, 0);    // Adjusted Bottom-right
        //uv[2] = new Vector2(0.8f, 0.9f); // Adjusted Top-right
        //uv[3] = new Vector2(0.2f, 1);    // Adjusted Top-left

        uv[0] = new Vector2(0, 1); // Top-left
        uv[1] = new Vector2(0, 0); // Bottom-left
        uv[2] = new Vector2(1, 0); // Bottom-right
        uv[3] = new Vector2(1, 1); // Top-right

        //uv = TextureProcessor.AdjustUVsBasedOnRotation(uv);


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

        var rigidbody = field.AddComponent<Rigidbody>();
        rigidbody.isKinematic = true;






        //var manipulator = field.AddComponent<XRGrabInteractable>();
        //// Can grab with both hands to rotate and scale field mesh
        //manipulator.selectMode = UnityEngine.XR.Interaction.Toolkit.Interactables.InteractableSelectMode.Multiple;

        //RotationAxisConstraint rotationConstraint = field.AddComponent<RotationAxisConstraint>();

        //// Disable manipulation on all axes, except y
        //rotationConstraint.ConstraintOnRotation = AxisFlags.XAxis | AxisFlags.ZAxis;

        //MoveAxisConstraint moveConstraint = field.AddComponent<MoveAxisConstraint>();

        //// Disable manipulation on y axis
        //moveConstraint.ConstraintOnMovement = AxisFlags.YAxis;

        //// Make movement, rotation and scaling of field mesh smoother / slower
        //manipulator.MoveLerpTime = 0.3f;
        //manipulator.RotateLerpTime = 0.3f;
        //manipulator.ScaleLerpTime = 0.3f;

        //// Allow manipulation for movement and rotation only
        //manipulator.AllowedManipulations = TransformFlags.Move | TransformFlags.Rotate;

        //manipulator.enabled = false;

        var grabInteractable = field.AddComponent<XRGrabInteractable>();

        // Set the grab interactable to allow rotation and scaling
        grabInteractable.trackRotation = true; // Allows rotation
        grabInteractable.trackPosition = true; // Allows position changes
        grabInteractable.throwOnDetach = false; // Prevent throwing on release

        // Implement rotation constraints (Custom Script)
        var rotationConstraint = field.AddComponent<CustomRotationConstraint>();
        rotationConstraint.RestrictedAxes = CustomRotationConstraint.AxisFlags.XAxis | CustomRotationConstraint.AxisFlags.ZAxis;

        // Implement movement constraints (Custom Script)
        var movementConstraint = field.AddComponent<CustomMovementConstraint>();
        movementConstraint.RestrictedAxes = CustomMovementConstraint.AxisFlags.YAxis;

        // Configure grab interaction settings
        grabInteractable.movementType = XRBaseInteractable.MovementType.VelocityTracking;

        // To simulate "smoother/slower" movement/rotation
        grabInteractable.attachEaseInTime = 0.3f; // Smooth interaction attachment

        // If you want to disable interactions initially
        grabInteractable.enabled = false;


    }



    // This method will convert GPS coordinates to Unity world coordinates based on a reference point.
    private Vector3 GPSPositionToWorldPosition(Vector2 gpsPosition)
    {
        // Reference point (the "origin" GPS point to convert everything relative to)
#if UNITY_EDITOR
        //Vector2 userReferenceGPS = new Vector2(40.62573397234498f, 22.959545477275366f);
        //Vector2 userReferenceGPS = new Vector2(21.69290f, 39.63610f); // NOTE: also see Vector2 currentPosition in this script
        Vector2 userReferenceGPS = new Vector2(21.69290f, 39.63610f);
        foreach (var parcel in parcels)
        {
            if (apisManager.GetComponent<ParcelsListAPI>().selectedParcelId != null && parcel.id.ToString() == apisManager.GetComponent<ParcelsListAPI>().selectedParcelId)
            {
                userReferenceGPS.x = parcel.shape.coordinates[0][0][0]; // NOTE: also see Vector2 currentPosition in this script
                userReferenceGPS.y = parcel.shape.coordinates[0][0][1];
            }
        }
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



        //apisManager.GetComponent<FertilizationAPI>().GetFertilizationData("5836", (jsonResponseFertilization) =>
        //{
        //    Debug.Log("fertilizationData => " + jsonResponseFertilization);

        //    Texture2D texture = apisManager.GetComponent<FertilizationAPI>().ParseFertilizationData(jsonResponseFertilization);


        //    var croppedTexture = CropWhiteSpaces(texture);

        //    CreateFieldMesh(croppedTexture);
        //});


        //apisManager.GetComponent<CropGrowthImageAPI>().GetCropGrowthImage("157212", (jsonResponseCropGrowthImage) =>
        //{
        //    if (jsonResponseCropGrowthImage != null)
        //    {
        //        Debug.Log("CROP IMAGE EXISTS");

        //        //CreateFieldMesh(jsonResponseCropGrowthImage);
        //    }                
        //    else
        //        Debug.Log("CROP IMAGE DOES NOT EXIST");

            
        //});


        //apisManager.GetComponent<TillageAPI>().GetTillageData("10478", (jsonResponseTillage) =>
        //{
        //    Debug.Log(jsonResponseTillage);


        //});


        //apisManager.GetComponent<IrrigationAPI>().GetIrrigationData("11256", (jsonResponseIrrigation) =>
        //{
        //    Debug.Log(jsonResponseIrrigation);


        //});


        //apisManager.GetComponent<CropGrowthDatesAPI>().GetCropGrowthDatesData("157212", (jsonResponseCropGrowthDates) =>
        //{
        //    Debug.Log("CropGrowthDatesAPI => " + jsonResponseCropGrowthDates);


        //});


        //apisManager.GetComponent<ParcelsListAPI>().GetParcelsListData((jsonResponseParcelsList) =>
        //{
        //    Debug.Log("Parcels List API => " + jsonResponseParcelsList);
        //});



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
        marker.AddComponent<XRGrabInteractable>();

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
        var fieldManipulator = field.GetComponent<XRGrabInteractable>();
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


    public static Texture2D CropWhiteSpaces(Texture2D originalTexture, float tolerance = 0.9f)
    {
        RotateTexture(originalTexture, 45);

        // Get the pixel colors from the texture
        Color[] pixels = originalTexture.GetPixels();
        int width = originalTexture.width;
        int height = originalTexture.height;

        Debug.Log("Original w: " + width + " h: " + height);

        // Define crop boundaries
        int minX = width, minY = height, maxX = 0, maxY = 0;

        // Find the boundaries of the non-white area
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color pixel = pixels[y * width + x];

                // Check if the pixel is not white (within tolerance)
                if (pixel.r < tolerance || pixel.g < tolerance || pixel.b < tolerance) // || pixel.a < tolerance
                {
                    if (x < minX) minX = x;
                    if (x > maxX) maxX = x;
                    if (y < minY) minY = y;
                    if (y > maxY) maxY = y;
                }
            }
        }

        // Calculate the new width and height
        int croppedWidth = maxX - minX + 1;
        int croppedHeight = maxY - minY + 1;


        Debug.Log("Cropped w: " + croppedWidth + " h: " + croppedHeight);
        Debug.Log("minX: " + minX + " minY: " + minY + " maxX: " + maxX + " maxY: "  + maxY);

        // Get the cropped pixels
        Color[] croppedPixels = originalTexture.GetPixels(minX, minY, croppedWidth, croppedHeight);

        // Create a new texture and apply the cropped pixels
        Texture2D croppedTexture = new Texture2D(croppedWidth, croppedHeight);
        croppedTexture.SetPixels(croppedPixels);
        croppedTexture.Apply();

        return croppedTexture;
    }



    public static Texture2D RotateTexture(Texture2D originalTexture, float angleDegrees)
    {
        int width = originalTexture.width;
        int height = originalTexture.height;

        Texture2D rotatedTexture = new Texture2D(width, height);
        Color[] originalPixels = originalTexture.GetPixels();
        Color[] rotatedPixels = new Color[originalPixels.Length];

        float angleRadians = Mathf.Deg2Rad * angleDegrees;
        float cos = Mathf.Cos(angleRadians);
        float sin = Mathf.Sin(angleRadians);

        int centerX = width / 2;
        int centerY = height / 2;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Translate to origin
                int translatedX = x - centerX;
                int translatedY = y - centerY;

                // Rotate around origin
                int rotatedX = Mathf.RoundToInt(translatedX * cos - translatedY * sin);
                int rotatedY = Mathf.RoundToInt(translatedX * sin + translatedY * cos);

                // Translate back
                int finalX = rotatedX + centerX;
                int finalY = rotatedY + centerY;

                // Assign pixel if within bounds
                if (finalX >= 0 && finalX < width && finalY >= 0 && finalY < height)
                {
                    rotatedPixels[y * width + x] = originalPixels[finalY * width + finalX];
                }
                else
                {
                    rotatedPixels[y * width + x] = Color.clear; // Transparent
                }
            }
        }

        rotatedTexture.SetPixels(rotatedPixels);
        rotatedTexture.Apply();

        return rotatedTexture;
    }

}