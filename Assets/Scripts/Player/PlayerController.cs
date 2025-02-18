using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // Variables
    [SerializeField] private float moveSpeed = 5f; // Speed of movement when walking
    [SerializeField] private float jumpForce = 5f; // Force of the jump
    [SerializeField] private float maxSpeed = 10f; // Maximum speed when walking
    [SerializeField] private float maxSpeedSprint = 20f; // Maximum speed when sprinting
    [SerializeField]  public float mouseSensitivity = 100f; // Speed of the mouse
    [SerializeField] private float collectItemDistance = 5f;
    [SerializeField] private int itemLayer;
    [SerializeField] private int playerLayer;
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float deceleration = 5f;

    private VampireTCP networkManager;
    private MultiplayerManager multiplayerManager;
    private HealthSystem healthSystem;

    // Components
    private Rigidbody rb;
    private Camera cam;
    private Vector3 lastGroundedVelocity;

    // Inventory
    private PlayerInventory inventory;

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            networkManager = FindObjectsByType<VampireTCP>(FindObjectsSortMode.None)[0];
            multiplayerManager = FindObjectsByType<MultiplayerManager>(FindObjectsSortMode.None)[0];
        }
        catch {
            Debug.LogError("Unable to get network manager, ignoring for now");
        }

        // Get components
        cam = GetComponentInChildren<Camera>();
        rb = GetComponent<Rigidbody>();
        inventory = GetComponentInChildren<PlayerInventory>();
        healthSystem = GetComponent<HealthSystem>();

        // Lock the cursor
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the screen
    }

    void Update()
    {
        // Sprinting
        bool isSprinting = Input.GetKey(KeyCode.LeftShift); // Check if the left shift key is pressed

        // Inputs
        float horizontalInput = Input.GetAxis("Horizontal"); // Get the horizontal input
        float verticalInput = Input.GetAxis("Vertical"); // Get the vertical input

        // Movement
        Vector3 movement = new Vector3(horizontalInput, 0, verticalInput); // Create a new vector with the inputs
        rb.AddForce(transform.rotation * movement * moveSpeed, ForceMode.Acceleration); // Apply the movement to the player

        // Limiting Velocity
        Vector2 velocity2D = Vector2.ClampMagnitude(new Vector2(rb.linearVelocity.x, rb.linearVelocity.z), isSprinting ? maxSpeedSprint : maxSpeed); // Clamp the velocity to the maximum speed
        rb.linearVelocity = new Vector3(velocity2D.x, rb.linearVelocity.y, velocity2D.y); // Set the linear velocity of the player

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded()) // Check if the space key is pressed and if the player is on the ground
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // Apply a force up to make the player jump
        }
        if (Input.GetKeyDown(KeyCode.B) && multiplayerManager.numPlayers >= 2 && !multiplayerManager.beginningGame)
        {
            networkManager.BroadcastNewMessage("beginGame", new { t = new Vector3(transform.position.x, transform.position.y, transform.position.z).ToString() });
            multiplayerManager.beginningGame = true;
            Debug.Log("Starting");
            SceneManager.LoadScene("Lobby");
        }

        // Look
        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mouseSensitivity); // Rotate the player based on the mouse input

        // Inventory
        if (Input.GetKeyDown(KeyCode.Mouse0))
            inventory.UseHeldItem();

        if (Input.GetKeyDown(KeyCode.E))
            CheckForItem();

        if (Input.GetKeyDown(KeyCode.Q))
            inventory.DiscardHeldItem();

        // Inventory
        if (Input.mouseScrollDelta.y != 0)
        {
            inventory.Scroll(Input.mouseScrollDelta.y < 0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Alpha5))
            inventory.SetHeldSlot(Input.inputString switch
            {
                "1" => 0,
                "2" => 1,
                "3" => 2,
                "4" => 3,
                "5" => 4,
                _ => inventory.heldSlot
            });
        HighlightItem();
    }

    void FixedUpdate()
    {
        if (networkManager != null) networkManager.BroadcastNewMessage("updatePlayerPosition", new { t = new Vector3(transform.position.x, transform.position.y, transform.position.z).ToString() });
    }

    // Check if the player is on the ground
    bool IsGrounded()
    {
        // Raycast to check if the player is on the ground
        return Physics.Raycast(transform.position, Vector3.down, 1.1f); // Check if there is a collision within 1.1f units down from the player
    }
    public bool CheckForItem()
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        Debug.DrawRay(cam.transform.position, cam.transform.forward * collectItemDistance, Color.red, 10f);
        if (Physics.Raycast(ray, out RaycastHit hit, collectItemDistance) && hit.transform.gameObject.layer == itemLayer && !hit.transform.GetComponent<Item>().isHeld)
        {
            Debug.Log("Raycast successful");
            OnPickupItem(hit.transform.GetComponent<Item>());
            return true;
        }
        Debug.Log("Raycast failure");
        return false;
    }
    public void OnPickupItem(Item item)
    {
        bool success = GetComponentInChildren<PlayerInventory>().PickupItem(item);
        if (success)
        {
            item.transform.SetParent(transform.GetChild(1));
            item.Pickup();
        }
    }
    void HighlightItem()
    {
        Ray ray = new(cam.transform.position, cam.transform.forward);
        //Debug.DrawRay(cam.transform.position, cam.transform.forward * collectItemDistance, Color.red, 10f);
        if (Physics.Raycast(ray, out RaycastHit hit, collectItemDistance) && hit.transform.gameObject.layer == itemLayer)
        {
            //Debug.Log("Raycast successful");
            Item item = hit.transform.GetComponent<Item>();
            if (item != null)
            {
                item.Highlight();
            }
        }
        else
        {
            Item[] items = FindObjectsByType<Item>(FindObjectsSortMode.None);
            foreach (Item item in items)
            {
                item.Unhighlight();
            }
        }
    }

    public bool IsDead()
    {
        return healthSystem.currentHealth <= 0;
    }
}

