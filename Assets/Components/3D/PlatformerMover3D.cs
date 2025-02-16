using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlatformerMover3D : MonoBehaviour
{
    [Tooltip("Movement speed")]
    public float speed = 5f;
    [Tooltip("Jump force")]
    public float jumpForce = 7f;

    private Rigidbody rb;
    private bool isGrounded;

    void Start() => rb = GetComponent<Rigidbody>();

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        rb.linearVelocity = new Vector3(moveX * speed, rb.linearVelocity.y, moveZ * speed);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.contacts[0].normal.y > 0.5f) isGrounded = true;
    }
}