using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraWaypoint : MonoBehaviour
{
    [SerializeField]
    internal List<CameraWaypoint> nextWaypoints = new();

    private void Awake()
    {
        foreach (var waypoint in nextWaypoints)
            if (!waypoint.nextWaypoints.Contains(this))
                waypoint.nextWaypoints.Add(this);
    }
}
