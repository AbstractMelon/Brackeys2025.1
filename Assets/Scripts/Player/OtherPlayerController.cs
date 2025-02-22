using System;
using System.Net;
using UnityEngine;

public class OtherPlayerController : MonoBehaviour
{
    private const int ChunkSize = 160; // Must match sender's chunk size
    private const int SamplingFrequency = 16000;
    private const int BufferSize = ChunkSize * 4; // Buffer 4 chunks

    private AudioSource audioSource;
    private AudioClip audioClip;
    private float[] audioBuffer;
    private int writePosition = 0;

    private GameObject Player;

    public HealthSystem health;

    private void Start()
    {
        Player = GameObject.Find("Player");
        audioSource = gameObject.GetComponent<AudioSource>();
        audioClip = AudioClip.Create("VoiceChat", BufferSize, 1, SamplingFrequency, false);
        audioSource.clip = audioClip;
        audioSource.loop = true;
        audioSource.Play();

        audioBuffer = new float[ChunkSize];
    }

    public float GetScaledDistance(GameObject obj1, GameObject obj2)
    {
        float distance = Vector3.Distance(obj1.transform.position, obj2.transform.position);

        if (distance >= 50f) return 0f;
        if (distance <= 4f) return 1f;

        // Normalize the distance between 4 and 50
        float normalizedDistance = (distance - 4f) / (50f - 4f);

        // Apply logarithmic scaling and invert (1 - result) so closer = 1, farther = 0
        return 1f - (Mathf.Log(normalizedDistance * 9f + 1f) / Mathf.Log(10f));
    }

    private void Update()
    {
        if (Player.GetComponent<HealthSystem>().currentHealth > 0 && health.currentHealth <= 0)
        {
            audioSource.volume = 0;
        } else
        {
            audioSource.volume = GetScaledDistance(gameObject, Player);
        }
    }

    public void OnVoiceDataReceived(byte[] data)
    {
        // Decompress audio data from bytes back to float
        for (int i = 0; i < ChunkSize; i++)
        {
            audioBuffer[i] = (data[i] / 127.5f) - 1f;
        }

        // Write the chunk to the AudioClip
        audioClip.SetData(audioBuffer, writePosition);

        writePosition += ChunkSize;
        if (writePosition >= audioClip.samples)
        {
            writePosition = 0;
        }
    }

    private void OnDisable()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }
}
