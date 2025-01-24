using UnityEngine;

public class CustomRotationConstraint : MonoBehaviour
{
    [System.Flags]
    public enum AxisFlags
    {
        None = 0,
        XAxis = 1 << 0,
        YAxis = 1 << 1,
        ZAxis = 1 << 2,
    }

    public AxisFlags RestrictedAxes;

    private Quaternion initialRotation;

    void Start()
    {
        initialRotation = transform.rotation;
    }

    void LateUpdate()
    {
        Vector3 eulerAngles = transform.rotation.eulerAngles;

        if ((RestrictedAxes & AxisFlags.XAxis) != 0)
            eulerAngles.x = initialRotation.eulerAngles.x;

        if ((RestrictedAxes & AxisFlags.YAxis) != 0)
            eulerAngles.y = initialRotation.eulerAngles.y;

        if ((RestrictedAxes & AxisFlags.ZAxis) != 0)
            eulerAngles.z = initialRotation.eulerAngles.z;

        transform.rotation = Quaternion.Euler(eulerAngles);
    }
}
