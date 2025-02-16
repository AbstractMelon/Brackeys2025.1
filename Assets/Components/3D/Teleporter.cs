using UnityEngine;

public class TeleportOnCollision : MonoBehaviour
{
    [Tooltip("Where to teleport object")]
    public Transform targetPosition;
    
    [Tooltip("Cooldown in seconds")]
    public float cooldown = 2f;

    private float lastTeleportTime;

    void OnTriggerEnter(Collider other)
    {
        if(Time.time > lastTeleportTime + cooldown)
        {
            other.transform.position = targetPosition.position;
            lastTeleportTime = Time.time;
        }
    }
}