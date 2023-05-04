using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private static CameraController _instance;
    public static CameraController Instance() => _instance;

    [SerializeField, Range(0f, 5f)]
    private float moveSpeed = 1f;

    private void Start()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    private Queue<CameraWaypoint> waypoints = new();
    private Coroutine camerawork = null;

    public void SetGoal(CameraWaypoint goalPoint)
    {
        var startPoint = CameraWaypointManager.Instance().FindNearestPoint(transform);
        waypoints = CameraWaypointManager.Instance().FindPath(startPoint, goalPoint);
        if (camerawork != null) StopCoroutine(camerawork);
        if (waypoints.Count > 0) StartCoroutine(CameraworkCoroutine());
    }

    private IEnumerator CameraworkCoroutine()
    {
        while (waypoints.Count > 0)
        {
            yield return null;
        }
        camerawork = null;
    }
}
