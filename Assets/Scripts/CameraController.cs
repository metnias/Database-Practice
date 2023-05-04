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
    [SerializeField, Range(0f, 1f)]
    private float waypointBound = 0.2f;

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

        if (camerawork != null) StopCoroutine(camerawork);
        if (waypoints.Count > 0) StartCoroutine(CameraworkCoroutine());
    }

    private Rigidbody rBody = null;
    private Vector3 vel;
    private Vector3 angle;

    private IEnumerator CameraworkCoroutine()
    {
        vel = Vector3.zero;
        angle = transform.rotation.eulerAngles;
        while (waypoints.Count > 0)
        {
            var wp = waypoints.Peek();
            var relPos = wp.transform.position - transform.position;
            vel *= 1f - (moveAcc * 0.05f * Time.deltaTime);
            vel += moveAcc * Time.deltaTime * relPos.normalized;
            vel = Vector3.ClampMagnitude(vel, moveMaxSpeed);
            angle = Vector3.Slerp(angle, wp.transform.rotation.eulerAngles, 2f * Time.deltaTime);
            rBody.velocity = vel;
            rBody.rotation = Quaternion.Euler(angle);
            yield return null;
            if (Vector3.Distance(transform.position, wp.transform.position) < waypointBound
                && Quaternion.Angle(transform.rotation, wp.transform.rotation) < 10f)
                waypoints.Dequeue();
        }
        while (vel.magnitude > 0.1f)
        {
            vel *= 1f - (moveAcc * 0.05f * Time.deltaTime);
            rBody.velocity = vel;
            yield return null;
        }
        rBody.velocity = Vector3.zero;
        camerawork = null;
    }
}
