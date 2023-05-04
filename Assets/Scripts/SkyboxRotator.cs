using UnityEngine;

public class SkyboxRotator : MonoBehaviour
{
    [SerializeField, Range(0f, 30f)]
    private float speed = 10f;

    private void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * speed);
        var rot = transform.rotation.eulerAngles;
        rot.y = Time.time * speed;
        transform.rotation = Quaternion.Euler(rot);
    }
}
