using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(CameraWaypoint))]
public class DebugCameraWaypoint : MonoBehaviour
{
    private CameraWaypoint myWaypoint;

    [SerializeField]
    private Color lineColor = Color.white;

#if UNITY_EDITOR
    private void Awake()
    {
        myWaypoint = GetComponent<CameraWaypoint>();
    }

    private void Update()
    {
        if (!myWaypoint || myWaypoint.nextWaypoints == null) return;
        foreach (var waypoint in myWaypoint.nextWaypoints)
            if (waypoint) Debug.DrawLine(transform.position, waypoint.transform.position, lineColor);
    }
#endif
}
