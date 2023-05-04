using UnityEngine;

[ExecuteInEditMode]
public class SkyboxReset : MonoBehaviour
{
#if UNITY_EDITOR
    private void Start()
    {
        RenderSettings.skybox.SetFloat("_Rotation", 0f);
    }
#endif
}
