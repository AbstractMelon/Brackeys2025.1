using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityOpus;

public class PlayerController : MonoBehaviour
{
    // Variables
    [SerializeField] private float jumpForce = 5f; // Force of the jump
    [SerializeField] private float maxSpeed = 10f; // Maximum speed when walking
    [SerializeField] private float maxSpeedSprint = 20f; // Maximum speed when sprinting
    [SerializeField]  public float mouseSensitivity = 2f; // Speed of the mouse
    [SerializeField] private ParticleSystem stepParticles;
    [SerializeField] private AudioClip walkingSFX;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float accelerationMultiplyer = 1f;
    [SerializeField] private float gravity = -9f;
    [SerializeField] private float friction = 7f;
    [SerializeField] private float interactDistance;
    private VampireTCP networkManager;
    private MultiplayerManager multiplayerManager;
    private HealthSystem healthSystem;
    private CharacterController controller;
    private Vector3 velocity;

    // Components
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
        if (SceneManager.GetActiveScene().name == "Game")
        {
            GameObject[] spawnPositions = GameObject.FindGameObjectsWithTag("SpawnPosition");
            Transform spawnPos = spawnPositions[UnityEngine.Random.Range(0, spawnPositions.Length)].transform;
            transform.position = spawnPos.transform.position;
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
        UpdateMovement();

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = new Ray(cam.transform.position, cam.transform.forward);
            Debug.DrawRay(cam.transform.position, cam.transform.forward * interactDistance, Color.red, 10f);
            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance) && hit.transform.gameObject.layer == 11)
            {
                MultiplayerManager.instance.StartGame();
                if (GameManager.instance != null)
                    GameManager.instance.DecideDemon();
                else
                    GameManager.DoDecideDemonOnLoad();
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

