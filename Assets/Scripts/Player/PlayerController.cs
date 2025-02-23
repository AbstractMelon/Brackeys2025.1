using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // Variables
    [SerializeField] private float jumpForce = 5f; // Force of the jump
    [SerializeField] private float maxSpeed = 10f; // Maximum speed when walking
    [SerializeField] private float maxSpeedSprint = 20f; // Maximum speed when sprinting
    [SerializeField]  public float mouseSensitivity = 2f; // Speed of the mouse
    [SerializeField] private ParticleSystem stepParticles;
    [SerializeField] private AudioClip walkingSFX;
    [SerializeField] private AudioClip beginSFX;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float accelerationMultiplyer = 1f;
    [SerializeField] private float gravity = -9f;
    [SerializeField] private float friction = 7f;
    [SerializeField] private float interactDistance;
    [SerializeField] private GameObject demonIndicator;
    private VampireTCP networkManager;
    private MultiplayerManager multiplayerManager;
    private HealthSystem healthSystem;
    private CharacterController controller;
    private Vector3 velocity;
    private static bool startedGame;
    [SerializeField] private Image demonOverlay;
    public bool allowControl = true;

    // Components
    private Camera cam;


    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Game")
        {
            if (startedGame)
            {
                StartCoroutine(GameManager.instance.DecideDemon());
            }
            GameObject[] spawnPositions = GameObject.FindGameObjectsWithTag("SpawnPosition");
            Transform spawnPos = spawnPositions[UnityEngine.Random.Range(0, spawnPositions.Length)].transform;
            transform.position = spawnPos.transform.position;
            AudioSource audioSource2 = gameObject.AddComponent<AudioSource>();
            audioSource2.clip = beginSFX;
            audioSource2.Play();
        }
        try
        {
            networkManager = FindObjectsByType<VampireTCP>(FindObjectsSortMode.None)[0];
            multiplayerManager = FindObjectsByType<MultiplayerManager>(FindObjectsSortMode.None)[0];
        }
        catch {
            Debug.LogError("Unable to get network manager, ignoring for now");
        }

        // Get components
        controller = GetComponent<CharacterController>();
        cam = GetComponentInChildren<Camera>();
        healthSystem = GetComponent<HealthSystem>();

        // Lock the cursor
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the screen
    }

    void Update()
    {
        UpdateMovement();

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = new Ray(cam.transform.position, cam.transform.forward);
            Debug.DrawRay(cam.transform.position, cam.transform.forward * interactDistance, Color.red, 10f);
            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance) && hit.transform.gameObject.layer == 11)
            {
                startedGame = true;
                MultiplayerManager.instance.StartGame();
            }
        }
    }

    void FixedUpdate()
    {
        if (networkManager != null) networkManager.BroadcastNewMessage("updatePlayerPosition", new { t = new Vector3(transform.position.x, transform.position.y, transform.position.z).ToString(), r = new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z).ToString(), s = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z).ToString() });
    }

    public bool IsDead()
    {
        return healthSystem.currentHealth <= 0;
    }
    
    private void UpdateMovement()
    {
        // Sprinting
        bool isSprinting = Input.GetKey(KeyCode.LeftShift); // Check if the left shift key is pressed

        // Look
        Vector3 movement = Vector3.zero;
        if (allowControl) 
        {
            transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mouseSensitivity); // Rotate the player based on the mouse input
            movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        }
        
        
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
        if (movement.magnitude > 0 && controller.isGrounded && !audioSource.isPlaying)
        {
            audioSource.clip = walkingSFX;
            audioSource.Play();
        }
        else if (movement.magnitude == 0 || !controller.isGrounded)
        {
            audioSource.Stop();
        }

        if (movement.magnitude > 0 && controller.isGrounded && !stepParticles.isPlaying)
        {
            stepParticles.transform.position = transform.position + Vector3.down * 0.4f;
            stepParticles.Play();
        }
        else if (movement.magnitude == 0 || !controller.isGrounded)
        {
            stepParticles.Stop();
        }


        // Show the demon indicator if the demon is nearby
        if (GameObject.Find("Player" + GameManager.instance.demonId) == null) return;
        float distanceToDemon = Vector3.Distance(transform.position, GameObject.Find("Player" + GameManager.instance.demonId).transform.position);

        if (distanceToDemon < 25f)
        {
            demonOverlay.color = new Color(1f, 0f, 0f, 1f - (distanceToDemon / 25f));
            demonIndicator.SetActive(true);
        }
        else
        {
            demonOverlay.color = new Color(1f, 0f, 0f, 0f);
            demonIndicator.SetActive(false);
        }
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

