using UnityEngine;
using System;

public class Voice : MonoBehaviour
{
    [SerializeField]
    private VampireTCP networkManager;

    private const int SamplingFrequency = 16000; // Reduced from 48000
    private const int ChunkSize = 160; // 10ms of audio at 16kHz
    private const int BufferSeconds = 1;

    private AudioClip microphoneClip;
    private int lastSamplePosition = 0;
    private float[] processBuffer;
    private byte[] byteBuffer;

    private void Start()
    {
        networkManager = FindObjectsByType<VampireTCP>(FindObjectsSortMode.None)[0];
        if (networkManager == null)
        {
            Debug.LogError("NetworkManager reference not set on VoiceSender!");
            enabled = false;
            return;
        }

        microphoneClip = Microphone.Start(null, true, BufferSeconds, SamplingFrequency);
        processBuffer = new float[ChunkSize];
        byteBuffer = new byte[ChunkSize]; // Only 1 byte per sample now
    }

    private void FixedUpdate()
    {
        if (!Microphone.IsRecording(null)) return;

        int currentPosition = Microphone.GetPosition(null);
        if (currentPosition < 0 || currentPosition == lastSamplePosition) return;

        int samplesToRead = GetSamplesCount(currentPosition);

        while (samplesToRead >= ChunkSize)
        {
            // Read chunk of audio
            microphoneClip.GetData(processBuffer, lastSamplePosition);

            // Compress audio data to bytes (convert float [-1,1] to byte [0,255])
            for (int i = 0; i < ChunkSize; i++)
            {
                byteBuffer[i] = (byte)((processBuffer[i] + 1f) * 127.5f);
            }

            // Send tiny chunk over network
            networkManager.SendVoiceMessage(byteBuffer);

            lastSamplePosition += ChunkSize;
            if (lastSamplePosition >= microphoneClip.samples)
            {
                lastSamplePosition = 0;
            }
            samplesToRead -= ChunkSize;
        }
    }

    private int GetSamplesCount(int currentPosition)
    {
        if (currentPosition > lastSamplePosition)
        {
            return currentPosition - lastSamplePosition;
        }
        return (microphoneClip.samples - lastSamplePosition) + currentPosition;
    }

    private void OnDisable()
    {
        if (Microphone.IsRecording(null))
        {
            Microphone.End(null);
        }
    }
}