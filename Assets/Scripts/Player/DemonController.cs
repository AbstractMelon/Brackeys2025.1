using System;
using Unity.VisualScripting;
using UnityEngine;

public class DemonController : MonoBehaviour
{
    public MultiplayerManager multiplayerManager;

    // Variables
    [SerializeField] private float moveSpeed = 6f; // Speed of movement when walking
    [SerializeField] private float jumpForce = 7f; // Force of the jump
    [SerializeField] private float maxSpeed = 15f; // Maximum speed when walking
    [SerializeField] private float maxSpeedSprint = 25f; // Maximum speed when sprinting
    [SerializeField] public float mouseSensitivity = 3f; // Speed of the mouse
    [SerializeField] private float attackPlayerDistance = 5f;
    [SerializeField] private int playerLayer = 6;
    [SerializeField] private int attackPower = 20;
    [SerializeField] private float shrinkSpeed = 0.01f;
    [SerializeField] private float endOnceReachedSize = 1f;

    private VampireTCP networkManager;

    // Components
    private Rigidbody rb;
    private Camera cam;
    private Vector3 lastGroundedVelocity;

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            networkManager = FindObjectsByType<VampireTCP>(FindObjectsSortMode.None)[0];
        }
        catch {}

        // Get components
        cam = GetComponentInChildren<Camera>();
        rb = GetComponent<Rigidbody>();

        // Lock the cursor
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the screen
    }

    void Update()
    {
        if (transform.localScale.x <= endOnceReachedSize)
        {
            return;
        }
        transform.localScale -= new Vector3(shrinkSpeed * Time.deltaTime, shrinkSpeed * Time.deltaTime, shrinkSpeed * Time.deltaTime);
        if (transform.localScale.x <= endOnceReachedSize)
        {
            Die();
            return;
        }
        // Sprinting
        bool isSprinting = Input.GetKey(KeyCode.LeftShift); // Check if the left shift key is pressed

        // Inputs
        float horizontalInput = Input.GetAxis("Horizontal"); // Get the horizontal input
        float verticalInput = Input.GetAxis("Vertical"); // Get the vertical input

        // Movement
        Vector3 movement = new Vector3(horizontalInput, 0, verticalInput); // Create a new vector with the inputs
        rb.AddForce(transform.rotation * movement * moveSpeed, ForceMode.Acceleration); // Apply the movement to the demon

        // Limiting Velocity
        Vector2 velocity2D = Vector2.ClampMagnitude(new Vector2(rb.linearVelocity.x, rb.linearVelocity.z), isSprinting ? maxSpeedSprint : maxSpeed); // Clamp the velocity to the maximum speed
        rb.linearVelocity = new Vector3(velocity2D.x, rb.linearVelocity.y, velocity2D.y); // Set the linear velocity of the demon

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded()) // Check if the space key is pressed and if the demon is on the ground
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // Apply a force up to make the player jump
        }

        if(Input.GetKeyDown(KeyCode.B) && multiplayerManager.numPlayers >= 2)
        {
            //multiplayerManager.StartGame();
        }

        // Look
        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mouseSensitivity); // Rotate the demon based on the mouse input

        // Inventory
        if (Input.GetKeyDown(KeyCode.Mouse0))
            Attack();

        HighlightPlayer();
    }
    public void Die()
    {
        Debug.Log("Demon has died!");
        Destroy(gameObject);
    }
    private bool Attack()
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        Debug.DrawRay(cam.transform.position, cam.transform.forward * attackPlayerDistance, Color.red, 10f);
        if (Physics.Raycast(ray, out RaycastHit hit, attackPlayerDistance) && hit.transform.gameObject.layer == playerLayer)
        {
            Debug.Log("Raycast successful");
            hit.transform.GetComponent<HealthSystem>().TakeDamage(attackPower);
            
            return true;
        }
        Debug.Log("Raycast failure");
        return false;
    }

    void FixedUpdate()
    {
        if (networkManager != null)
            networkManager.BroadcastNewMessage("updatePlayerPosition", new { t = new Vector3(transform.position.x, transform.position.y, transform.position.z).ToString() });
    }

    // Check if the player is on the ground
    bool IsGrounded()
    {
        // Raycast to check if the player is on the ground
        return Physics.Raycast(transform.position, Vector3.down, 2.1f); // Check if there is a collision within 1.1f units down from the player
    }
    void HighlightPlayer()
    {
        Ray ray = new(cam.transform.position, cam.transform.forward);
        //Debug.DrawRay(cam.transform.position, cam.transform.forward * collectItemDistance, Color.red, 10f);
        if (Physics.Raycast(ray, out RaycastHit hit, attackPlayerDistance) && hit.transform.gameObject.layer == playerLayer)
        {
            Debug.Log("Raycast successful");
            GameObject player = hit.transform.gameObject;
            player.GetComponent<Renderer>().material.color = Color.red;
        }
        else
        {
            OtherPlayerController[] players = FindObjectsByType<OtherPlayerController>(FindObjectsSortMode.None);
            foreach (OtherPlayerController player in players)
            {
                player.GetComponent<Renderer>().material.color = Color.white;
            }
            PlayerController localPlayer = FindFirstObjectByType<PlayerController>();
            if (localPlayer != null) localPlayer.GetComponent<Renderer>().material.color = Color.white;
        }
    }
}


