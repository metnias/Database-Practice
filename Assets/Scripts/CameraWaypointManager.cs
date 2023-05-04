using System.Collections.Generic;
using UnityEngine;

public class CameraWaypointManager : MonoBehaviour
{
    private static CameraWaypointManager _instance;

    public static CameraWaypointManager Instance() => _instance;

    private void Start()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
        {
            Destroy(gameObject);
            return;
        }
        RefreshCost();
    }

    #region Floyd
    private const float INF = 999999f;
    private CameraWaypoint[] waypoints;
    private readonly Dictionary<CameraWaypoint, int> flagIndexes = new();

    public int GetFlagIndex(CameraWaypoint waypoint) =>
        flagIndexes.TryGetValue(waypoint, out int index) ? index : -1;
    private int[,] pathNode;
    private int WaypointNum => waypoints.Length;

    private void RefreshCost()
    {
        // Get flags
        waypoints = GetComponentsInChildren<CameraWaypoint>();
        flagIndexes.Clear();
        for (int i = 0; i < WaypointNum; ++i) flagIndexes.Add(waypoints[i], i);

        pathNode = new int[WaypointNum, WaypointNum];
        for (int i = 0; i < WaypointNum; ++i) for (int j = 0; j < WaypointNum; ++j) pathNode[i, j] = -1;

        // Get cost
        float[,] cost = new float[WaypointNum, WaypointNum];
        for (int i = 0; i < WaypointNum; ++i) for (int j = 0; j < WaypointNum; ++j) cost[i, j] = INF;
        for (int i = 0; i < WaypointNum; ++i)
        {
            cost[i, i] = 0f;
            foreach (var nextFlag in waypoints[i].nextWaypoints)
            {
                if (GetFlagIndex(nextFlag) < 0) continue; // flag no longer exist
                cost[i, GetFlagIndex(nextFlag)] =
                    Vector3.Distance(waypoints[i].transform.position, nextFlag.transform.position);
                pathNode[i, GetFlagIndex(nextFlag)] = i;
            }
        }

        // Use Floyd to precalculate
        for (int m = 0; m < WaypointNum; ++m)
            for (int s = 0; s < WaypointNum; ++s)
                for (int e = 0; e < WaypointNum; ++e)
                {
                    float bridge = cost[s, m] + cost[m, e];
                    if (cost[s, e] > bridge)
                    {
                        cost[s, e] = bridge;
                        pathNode[s, e] = pathNode[m, e]; // save middle path
                    }
                }
    }

    internal Queue<CameraWaypoint> FindPath(CameraWaypoint startPoint, CameraWaypoint goalPoint)
    {
        if (!startPoint || !goalPoint) return new();
        if (startPoint == goalPoint)
        {
            Queue<CameraWaypoint> onePath = new();
            onePath.Enqueue(goalPoint);
            return onePath;
        }

        int start = GetFlagIndex(startPoint);
        int goal = GetFlagIndex(goalPoint);
        if (start < 0 || goal < 0) // targeted to unknown flag
        {
            if (startPoint.transform.parent != transform ||
                goalPoint.transform.parent != transform)
            { // Not part of this manager
                Debug.LogError("Target Flag is not managed by this manager");
                return new();
            }
            RefreshCost(); // targeted to new flag, so refresh data
            return FindPath(startPoint, goalPoint);
        }

        if (pathNode[start, goal] < 0)
        {
            Debug.Log($"{startPoint.gameObject.name} > {goalPoint.gameObject.name} path not found!");
            return new();
        }
        Stack<int> stack = new(); // reverse trace
        while (goal != start)
        {
            stack.Push(goal);
            goal = pathNode[start, goal];
        }
        Queue<CameraWaypoint> path = new();
        path.Enqueue(startPoint);
        while (stack.Count > 0)
            path.Enqueue(waypoints[stack.Pop()]);

        return path;
    }
    #endregion Floyd

    public CameraWaypoint FindNearestPoint(Transform tf)
    {
        CameraWaypoint closestWp = null; float dist = float.MaxValue;
        foreach (var wp in waypoints)
        {
            if (!wp) continue;
            float curDist = Vector3.Distance(wp.transform.position, tf.position);
            if (dist > curDist)
            {
                dist = curDist;
                closestWp = wp;
            }
        }
        if (closestWp) return closestWp;
        return waypoints[0];
    }
}
