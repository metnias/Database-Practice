using UnityEngine;

public class HideOnPlay : MonoBehaviour
{
    private void Start()
    {
        var renderers = GetComponentsInChildren<Renderer>();
        foreach (var r in renderers) r.enabled = false;
    }
}
