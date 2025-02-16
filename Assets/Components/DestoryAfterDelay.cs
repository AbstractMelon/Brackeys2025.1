using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    [Tooltip("Time until destruction in seconds")]
    public float timer = 5f;
    public bool destroyOnCollision = false;

    void Start() => Destroy(gameObject, timer);

    void OnCollisionEnter(Collision collision)
    {
        if(destroyOnCollision) Destroy(gameObject);
    }
}