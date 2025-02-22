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
    [SerializeField] public float mouseSensitivity = 2f; // Speed of the mouse
    [SerializeField] private float attackPlayerDistance = 5f;
    [SerializeField] private int playerLayer = 6;
    [SerializeField] private int attackPower = 20;
    [SerializeField] private float shrinkSpeed = 0.01f;
    [SerializeField] private float endOnceReachedSize = 1f;
    [SerializeField] private float accelerationMultiplyer = 1f;
    [SerializeField] private float gravity = -9f;
    [SerializeField] private float friction = 7f;
    [SerializeField] private float interactDistance;
    private Vector3 velocity;
    private VampireTCP networkManager;

    // Components
    private Camera cam;
    private CharacterController controller;

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
        controller = GetComponent<CharacterController>();

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
        if (transform.localScale.y <= endOnceReachedSize)
        {
            Die();
            return;
        }
        UpdateMovement();
        if (Input.GetKeyDown(KeyCode.Mouse0))
            Attack();

        HighlightPlayer();
    }
    public void Die()
    {
        Debug.Log("Demon has died!");
        GameManager.instance.EndGame(false);
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
            networkManager.BroadcastNewMessage( "hurtPlayer", new { id = hit.transform.gameObject.name.Substring(6), d = attackPower} );
            return true;
        }
        Debug.Log("Raycast failure");
        return false;
    }

    void FixedUpdate()
    {
        if (networkManager != null) networkManager.BroadcastNewMessage("updatePlayerPosition", new { t = new Vector3(transform.position.x, transform.position.y, transform.position.z).ToString(), r = new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z).ToString(), s = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z).ToString() });
    }

    // Check if the player is on the ground
    void HighlightPlayer()
    {
        Ray ray = new(cam.transform.position, cam.transform.forward);
        //Debug.DrawRay(cam.transform.position, cam.transform.forward * collectItemDistance, Color.red, 10f);
        if (Physics.Raycast(ray, out RaycastHit hit, attackPlayerDistance) && hit.transform.gameObject.layer == playerLayer)
        {
            //Debug.Log("Raycast successful");
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
        }
    }
    private void UpdateMovement()
    {
        // Sprinting
        bool isSprinting = Input.GetKey(KeyCode.LeftShift); // Check if the left shift key is pressed

        // Look
        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mouseSensitivity); // Rotate the player based on the mouse input

        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        
        
        velocity += movement * accelerationMultiplyer * Time.deltaTime * 100 * (isSprinting ? 2f : 1f);

        Vector2 vec = new Vector2(velocity.x, velocity.z);

        float frictionFactor = 1 - (friction * Time.deltaTime);
        if (frictionFactor < 0) frictionFactor = 0;
        vec *= frictionFactor;

        vec = LimitMagnitude(vec, isSprinting ? maxSpeedSprint : maxSpeed);
        velocity = new Vector3(vec.x, velocity.y, vec.y);
        if ((velocity.x <= 0.1 && velocity.x > 0) || (velocity.x >= -0.1 && velocity.x < 0)) velocity.x = 0;
        if ((velocity.z <= 0.1 && velocity.z > 0) || (velocity.z >= -0.1 && velocity.z < 0)) velocity.z = 0;
        if (controller.isGrounded)
        {
            velocity.y = -1f;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                velocity.y += jumpForce;
            }
        }
        else
        {
            velocity.y -= gravity * -2f * Time.deltaTime;
        }
        velocity.y = Mathf.Max(velocity.y, -20);
        controller.Move(transform.rotation * velocity * Time.deltaTime);

    }
    //private bool CheckGrounded()
    //{
    //    return Physics.CheckSphere(transform.position - new Vector3(0, collider.height / 2 + collider.radius - 0.1f, 0), collider.radius, groundLayer);
    //}
    private Vector2 LimitMagnitude(Vector2 vector, float maxMagnitude)
    {
        if (vector.sqrMagnitude > maxMagnitude * maxMagnitude)
        {
            return vector.normalized * maxMagnitude;
        }
        return vector;
    }
}


