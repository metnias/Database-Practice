using UnityEngine;

public class ColliderDisabler : MonoBehaviour
{
    private void Start()
    {
        var colliders = GetComponentsInChildren<Collider>();
        foreach (var collider in colliders)
            collider.enabled = false;
    }
}
