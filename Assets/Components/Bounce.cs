using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BounceOnCollision : MonoBehaviour
{
    [Tooltip("How much bounce force to apply")]
    public float bounceForce = 10f;

    void OnCollisionEnter(Collision collision)
    {
        Vector3 bounceDirection = collision.contacts[0].normal;
        GetComponent<Rigidbody>().AddForce(-bounceDirection * bounceForce, ForceMode.Impulse);
    }
}