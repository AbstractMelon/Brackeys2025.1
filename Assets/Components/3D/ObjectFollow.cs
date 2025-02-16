
using UnityEngine;

public class SimpleFollow : MonoBehaviour
{
    [Tooltip("What to follow")]
    public Transform target;
    
    [Tooltip("Follow speed")]
    public float smoothness = 5f;
    
    [Tooltip("Offset from target")]
    public Vector3 offset = new Vector3(0, 5, -10);

    void LateUpdate()
    {
        if(target == null) return;
        
        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.Lerp(
            transform.position, 
            targetPosition, 
            smoothness * Time.deltaTime
        );
    }
}