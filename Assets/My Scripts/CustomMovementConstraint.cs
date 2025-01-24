using UnityEngine;

public class CustomMovementConstraint : MonoBehaviour
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

    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.position;
    }

    void LateUpdate()
    {
        Vector3 position = transform.position;

        if ((RestrictedAxes & AxisFlags.XAxis) != 0)
            position.x = initialPosition.x;

        if ((RestrictedAxes & AxisFlags.YAxis) != 0)
            position.y = initialPosition.y;

        if ((RestrictedAxes & AxisFlags.ZAxis) != 0)
            position.z = initialPosition.z;

        transform.position = position;
    }
}
