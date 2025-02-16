using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    private bool isJumping = false;
    [SerializeField]
    private float maxSpeed;
    private Rigidbody rb;
    [SerializeField]
    private float mouseSensitivity;
    private Camera cam;
    private Vector3 lastGroundedVelocity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Camera.main.gameObject.SetActive(false);
        cam = GetComponentInChildren<Camera>();
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0, moveVertical);

        rb.AddForce(transform.rotation * movement * moveSpeed);

        Vector2 movementV2 = Vector2.ClampMagnitude(new Vector2(rb.linearVelocity.x, rb.linearVelocity.z), maxSpeed);
        rb.linearVelocity = new Vector3(movementV2.x, rb.linearVelocity.y, movementV2.y);

        if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
        {
            rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
            isJumping = true;
        }

        cam.transform.localEulerAngles = new Vector3(ClampAngle(cam.transform.localEulerAngles.x - Input.GetAxis("Mouse Y") * mouseSensitivity), cam.transform.localEulerAngles.y, cam.transform.localEulerAngles.z);
        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mouseSensitivity); 
    }
     public static float ClampAngle(float angle) {
        if (angle > 180)
            angle -= 360;
        else if (angle < -180)
            angle += 360;

        if (angle > 90)
            angle = 90;
        if (angle < -90)
            angle = -90;

        return angle;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJumping = false;
        }
    }
    bool IsGrounded()
    {
        return true;
    }
}