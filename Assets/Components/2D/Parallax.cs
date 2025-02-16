using UnityEngine;

public class Parallax : MonoBehaviour
{
    [Tooltip("Main camera")]
    public Transform cameraTransform;
    [Tooltip("Parallax effect strength (0 = no movement)")]
    [Range(0f, 1f)] public float parallaxEffect = 0.5f;
    
    private Vector3 lastCameraPosition;

    void Start() => lastCameraPosition = cameraTransform.position;

    void LateUpdate()
    {
        Vector3 delta = cameraTransform.position - lastCameraPosition;
        transform.position += new Vector3(delta.x * parallaxEffect, delta.y * parallaxEffect, 0);
        lastCameraPosition = cameraTransform.position;
    }
}