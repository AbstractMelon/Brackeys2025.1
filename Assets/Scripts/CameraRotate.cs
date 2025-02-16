using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public Transform target;  // The object to orbit around
    public float orbitSpeed = 10f; // Speed of orbit
    public float orbitRadius = 5f; // Distance from the target
    public Vector3 orbitAxis = Vector3.up; // Orbit axis

    private float angle = 0f;

    void Update()
    {
        if (target == null) return;
        
        // Update angle based on speed and time
        angle += orbitSpeed * Time.deltaTime;
        
        // Calculate new position
        Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * orbitRadius;
        transform.position = target.position + offset;
        // Make the camera look at the target
        transform.LookAt(target);
    }
}
