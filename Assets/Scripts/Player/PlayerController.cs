using System;
using System.Collections;
using UnityEngine;
using UnityOpus;

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
    [SerializeField] private ParticleSystem stepParticles;
    [SerializeField] private AudioClip walkingSFX;
    [SerializeField] private AudioSource audioSource;
    private VampireTCP networkManager;
    private MultiplayerManager multiplayerManager;
    private HealthSystem healthSystem;

    // Components
    private Rigidbody rb;
    private Camera cam;

    public string microphoneDevice;
    public AudioClip microphoneClip;

    // Opus parameters � adjust as needed
    private int sampleRate = 48000;         // Recommended sample rate for Opus
    private int channels = 1;               // Mono is typical for voice chat
    private int frameSize;                  // Number of samples per frame per channel (e.g. 20ms frames)

    private Encoder encoder;

    private int lastSamplePos = 0;

    private IEnumerator ProcessAudio()
    {
        // Allocate buffers:
        // A buffer for one frame of audio (float samples)
        float[] frameBuffer = new float[frameSize * channels];
        // A temporary buffer for the encoded data (size can be adjusted as needed)
        byte[] encodedBuffer = new byte[4000];

        while (Microphone.IsRecording(microphoneDevice))
        {
            yield return new WaitForSeconds(0.2f);

            // Get the current recording position
            int currentPos = Microphone.GetPosition(microphoneDevice);
            int samplesAvailable = currentPos - lastSamplePos;
            if (samplesAvailable < 0)
            {
                // Handle wrap-around
                samplesAvailable += microphoneClip.samples;
            }

            // Process as many full frames as available
            while (samplesAvailable >= frameSize)
            {
                // Get 'frameSize' samples from the clip starting at lastSamplePos
                microphoneClip.GetData(frameBuffer, lastSamplePos);

                // Encode the frame:
                // The Encoder.Encode method uses the length of frameBuffer to calculate the frame size.
                int encodedBytes = encoder.Encode(frameBuffer, encodedBuffer);
                if (encodedBytes > 0)
                {
                    // Copy the encoded data into an array of the correct length.
                    byte[] opusFrame = new byte[encodedBytes];
                    Array.Copy(encodedBuffer, opusFrame, encodedBytes);
                    // Send the encoded frame over the network.
                    networkManager.SendVoiceMessage(opusFrame);
                }

                // Update our position:
                lastSamplePos = (lastSamplePos + frameSize) % microphoneClip.samples;
                samplesAvailable -= frameSize;
            }
        }
    }


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
        healthSystem = GetComponent<HealthSystem>();

        // Lock the cursor
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the screen

        microphoneDevice = Microphone.devices[0];
        Debug.Log(microphoneDevice);
        microphoneClip = Microphone.Start(microphoneDevice, true, 1, 44100);
        frameSize = sampleRate / 50; // 20ms frame

        // Initialize the Opus encoder and decoder
        encoder = new Encoder(SamplingFrequency.Frequency_48000, NumChannels.Mono, OpusApplication.VoIP);
        encoder.Bitrate = 16000;      // Example bitrate
        encoder.Complexity = 5;       // Adjust complexity as needed
        encoder.Signal = OpusSignal.Voice;
        StartCoroutine(ProcessAudio());
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

        // Look
        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mouseSensitivity); // Rotate the player based on the mouse input

        // Inventory
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = new Ray(cam.transform.position, cam.transform.forward);
            Debug.DrawRay(cam.transform.position, cam.transform.forward * collectItemDistance, Color.red, 10f);
            if (Physics.Raycast(ray, out RaycastHit hit, collectItemDistance) && hit.transform.gameObject.layer == 11)
            {
                MultiplayerManager.instance.StartGame();
                if (GameManager.instance != null)
                    GameManager.instance.DecideDemon();
                else
                    GameManager.DoDecideDemonOnLoad();
            }
        }

        if (movement.magnitude > 0 && IsGrounded() && !audioSource.isPlaying)
        {
            audioSource.clip = walkingSFX;
            audioSource.Play();
        }
        else if (movement.magnitude == 0 || !IsGrounded())
        {
            audioSource.Stop();
        }

        if (movement.magnitude > 0 && IsGrounded() && !stepParticles.isPlaying)
        {
            stepParticles.transform.position = transform.position + Vector3.down * 0.4f;
            stepParticles.Play();
        }
        else if (movement.magnitude == 0 || !IsGrounded())
        {
            stepParticles.Stop();
        }
    }

    void FixedUpdate()
    {
        if (networkManager != null) networkManager.BroadcastNewMessage("updatePlayerPosition", new { t = new Vector3(transform.position.x, transform.position.y, transform.position.z).ToString(), r = new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z).ToString(), s = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z).ToString() });
    }

    // Check if the player is on the ground
    bool IsGrounded()
    {
        // Raycast to check if the player is on the ground
        return Physics.Raycast(transform.position, Vector3.down, 1.1f); // Check if there is a collision within 1.1f units down from the player
    }

    public bool IsDead()
    {
        return healthSystem.currentHealth <= 0;
    }
}

