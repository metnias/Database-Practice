using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CameraController : MonoBehaviour
{
    private static CameraController _instance;
    public static CameraController Instance() => _instance;

    [Header("MoveVariables")]
    [SerializeField, Range(0f, 100f)]
    private float moveAcc = 1f;
    [SerializeField, Range(0f, 50f)]
    private float moveMaxSpeed = 4f;
    [SerializeField, Range(0f, 10f)]
    private float waypointBound = 0.2f;
    [SerializeField, Range(0f, 30f)]
    private float drag = 1f;

    [Header("Waypoints")]
    [SerializeField]
    private CameraWaypoint waypointLogIn = null;
    [SerializeField]
    private CameraWaypoint waypointSignUp = null;
    [SerializeField]
    private CameraWaypoint waypointUserPage = null;

    private void Start()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
        {
            Destroy(gameObject);
            return;
        }
        rBody = GetComponent<Rigidbody>();
    }

    private Queue<CameraWaypoint> waypoints = new();
    private Coroutine camerawork = null;

    public void SetGoal(UIController.FormType formType)
    {
        var startPoint = CameraWaypointManager.Instance().FindNearestPoint(transform);
        var goalPoint = formType switch
        {
            UIController.FormType.SignUp => waypointSignUp,
            UIController.FormType.UserPage => waypointUserPage,
            _ => waypointLogIn,
        };
        if (!startPoint || !goalPoint) return;

        waypoints = CameraWaypointManager.Instance().FindPath(startPoint, goalPoint);

        if (camerawork != null) StopAllCoroutines();
        if (waypoints.Count > 0) camerawork = StartCoroutine(CameraworkCoroutine());
    }

    private Rigidbody rBody = null;
    private Vector3 vel;

    private IEnumerator CameraworkCoroutine()
    {
        vel = Vector3.zero;
        CameraWaypoint wp = null;
        while (waypoints.Count > 0)
        {
            wp = waypoints.Peek();
            var relPos = wp.transform.position - transform.position;
            vel *= Mathf.Clamp01(1f - (drag * (waypoints.Count > 2 ? 1f : 2f) * Time.deltaTime));
            vel += moveAcc * Time.deltaTime * Vector3.ClampMagnitude(relPos, 1f);
            vel = Vector3.ClampMagnitude(vel, moveMaxSpeed);
            rBody.velocity = vel;
            transform.rotation = Quaternion.Slerp(transform.rotation, wp.transform.rotation, 2f * Time.deltaTime);
            yield return null;
            if (Vector3.Distance(transform.position, wp.transform.position) < waypointBound
                && Quaternion.Angle(transform.rotation, wp.transform.rotation) < 10f)
                waypoints.Dequeue();
        }
        while (Vector3.Distance(transform.position, wp.transform.position) > 0.1f
            || Quaternion.Angle(transform.rotation, wp.transform.rotation) > 0.1f)
        {
            var relPos = wp.transform.position - transform.position;
            vel *= Mathf.Clamp01(1f - (drag * 3f * Time.deltaTime));
            vel += moveAcc * Time.deltaTime * relPos.normalized;
            vel = Vector3.ClampMagnitude(vel, moveMaxSpeed);
            rBody.velocity = vel;
            transform.rotation = Quaternion.Slerp(transform.rotation, wp.transform.rotation, 5f * Time.deltaTime);
            yield return null;
        }
        rBody.velocity = Vector3.zero;
        transform.rotation = wp.transform.rotation;
        camerawork = null;
    }
}
