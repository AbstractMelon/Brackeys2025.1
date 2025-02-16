using UnityEngine;

public class SpinObject : MonoBehaviour
{
    [Tooltip("Rotation speed per second")]
    public Vector3 rotationSpeed = new Vector3(0, 90, 0);

    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}