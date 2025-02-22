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

    private void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        audioClip = AudioClip.Create("VoiceChat", BufferSize, 1, SamplingFrequency, false);
        audioSource.clip = audioClip;
        audioSource.loop = true;
        audioSource.Play();

        audioBuffer = new float[ChunkSize];
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
